using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Comms;
using Engine;

namespace Game
{
    internal class Client : IDisposable
    {
        public Game Game { get; private set; }

        public Peer Peer { get; private set; }

        public bool IsUsingInProcessTransmitter { get; }

        public bool IsPaused { get; set; }

        public bool IsGameCreator { get; private set; }
        /// <summary>
        /// 检查是否连接
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return this.Peer.ConnectedTo != null;
            }
        }

        public Exception Error { get; private set; }

        public Client(bool useInProcessTransmitter)
        {
            this.IsUsingInProcessTransmitter = useInProcessTransmitter;
            DiagnosticPacketTransmitter diagnosticPacketTransmitter;
            if (useInProcessTransmitter)
            {
                diagnosticPacketTransmitter = new DiagnosticPacketTransmitter(new InProcessPacketTransmitter());
            }
            else
            {
                diagnosticPacketTransmitter = new DiagnosticPacketTransmitter(new UdpPacketTransmitter(0));
            }
            Log.Information(string.Format("Starting client on address {0}", diagnosticPacketTransmitter.Address));
            this.Peer = new Peer(diagnosticPacketTransmitter);
            this.Peer.Comm.Settings.ResendPeriods = new float[]
            {
                0.5f,
                0.5f,
                1f,
                1.5f,
                2f
            };
            this.Peer.Comm.Settings.MaxResends = 20;
            this.Peer.Settings.KeepAlivePeriod = (this.IsUsingInProcessTransmitter ? float.PositiveInfinity : 5f);
            this.Peer.Settings.KeepAliveResendPeriod = (this.IsUsingInProcessTransmitter ? float.PositiveInfinity : 2f);
            this.Peer.Settings.ConnectionLostPeriod = (this.IsUsingInProcessTransmitter ? float.PositiveInfinity : 30f);
            this.Peer.Settings.ConnectTimeOut = 6f;
            this.Peer.Error += delegate (Exception e)
            {
                this.Error = e;
                Log.Error(e);
            };
            this.Peer.ConnectAccepted += delegate (PeerPacket p)
            {
                Message message = Message.Read(p.Data);
                GameCreatedMessage gameCreatedMessage = message as GameCreatedMessage;
                if (gameCreatedMessage != null)
                {
                    this.Handle(gameCreatedMessage, p.Peer);
                    return;
                }
                GameJoinedMessage gameJoinedMessage = message as GameJoinedMessage;
                if (gameJoinedMessage == null)
                {
                    throw new InvalidOperationException("Unrecognized message.");
                }
                this.Handle(gameJoinedMessage, p.Peer);
            };
            this.Peer.ConnectRefused += delegate (Packet p)
            {
                RefusedMessage refusedMessage = Message.Read(p.Data) as RefusedMessage;
                if (refusedMessage != null)
                {
                    throw new InvalidOperationException(refusedMessage.Reason);
                }
                throw new InvalidOperationException("Unrecognized message.");
            };
            this.Peer.ConnectTimedOut += delegate (IPEndPoint address)
            {
                throw new InvalidOperationException("Server did not respond.");
            };
            this.Peer.DataMessageReceived += delegate (PeerPacket p)
            {
                Message message = Message.Read(p.Data);
                TickMessage tickMessage = message as TickMessage;
                if (tickMessage != null)
                {
                    this.Handle(tickMessage, p.Peer);
                    return;
                }
                RequestGameImageMessage requestGameImageMessage = message as RequestGameImageMessage;
                if (requestGameImageMessage != null)
                {
                    this.Handle(requestGameImageMessage, p.Peer);
                    return;
                }
                RequestGameStateMessage requestGameStateMessage = message as RequestGameStateMessage;
                if (requestGameStateMessage == null)
                {
                    throw new InvalidOperationException("Unrecognized message.");
                }
                this.Handle(requestGameStateMessage, p.Peer);
            };
            Window.Activated += this.WindowActivated;
            Window.Deactivated += this.WindowDeactivated;
            Window.Closed += this.WindowClosed;
        }

        public void Dispose()
        {
            Window.Activated -= this.WindowActivated;
            Window.Deactivated -= this.WindowDeactivated;
            Window.Closed -= this.WindowClosed;
            this.DisconnectFromGame();
            Task.Run(delegate ()
            {
                Thread.Sleep(2000);
                Peer peer = this.Peer;
                if (peer == null)
                {
                    return;
                }
                peer.Dispose();
            });
        }
        /// <summary>
        /// 方法
        /// 创建游戏
        /// 如果Peer.ConnectedTo（）有地址，则异常，否则将客户端创建器设置就位，调用连接到地址Peer.Connect（）
        /// </summary>
        /// <param name="serverAddress">网络终结点</param>
        /// <param name="gameCreationParameters">游戏创建参数</param>
        public void CreateGame(IPEndPoint serverAddress, GameCreationParameters gameCreationParameters)
        {
            //基类赋值Lock
            object @lock = this.Peer.Lock;
            lock (@lock)
            {
                //如果连接到不为空，正在连接的不为空
                if (this.Peer.ConnectedTo != null || this.Peer.ConnectingTo != null)
                {
                    //异常已经连接
                    throw new InvalidOperationException("Already connected.");
                }
                //错误信息赋值空
                this.Error = null;
                //是否为游戏创建者，创建器是否就位
                this.IsGameCreator = true;
                //连接到，服务器地址，消息写入，创建游戏信息
                this.Peer.Connect(serverAddress, Message.Write(new CreateGameMessage
                {
                    //游戏参数进行写入
                    CreationParameters = gameCreationParameters
                }));
            }
        }
        /// <summary>
        /// 方法
        /// 加入游戏
        /// 将数据创建为加入游戏消息，发送给服务器
        /// </summary>
        /// <param name="serverAddress">网络终结点</param>
        /// <param name="gameId">游戏ID</param>
        /// <param name="spectate">观察者</param>
        public void JoinGame(IPEndPoint serverAddress, int gameId, bool spectate)
        {
            object @lock = this.Peer.Lock;
            lock (@lock)
            {
                ////如果连接到不为空，正在连接的不为空
                if (this.Peer.ConnectedTo != null || this.Peer.ConnectingTo != null)
                {
                    throw new InvalidOperationException("Already connected.");
                }
                this.Error = null;
                //设置客场标识
                this.IsGameCreator = false;
                //连接到服务器地址，创建加入消息，将消息发送出去
                /*@游戏ID
                 * @玩家名称
                 * @玩家全局标识
                 *@玩家平台
                 *@是否观战
                 */
                this.Peer.Connect(serverAddress, Message.Write(new JoinGameMessage
                {
                    GameId = gameId,
                    PlayerName = SettingsManager.PlayerName,
                    PlayerGuid = SettingsManager.PlayerGuid,
                    PlayerPlatform = VersionsManager.Platform,
                    PreferredFaction = (spectate ? Faction.None : SettingsManager.Faction)
                }));
            }
        }
        /// <summary>
        /// 方法
        /// 开始游戏
        /// 
        /// </summary>
        public void StartGame()
        {
            object @lock = this.Peer.Lock;
            lock (@lock)
            {
                //连接到后能创建
                if (this.Peer.ConnectedTo == null)
                {
                    throw new InvalidOperationException("Not connected.");
                }
                if (!this.IsGameCreator)
                {
                    throw new InvalidOperationException("Not game creator.");
                }
                //发送数据，匹配连接到，可靠串行，消息写入开始游戏消息
                this.Peer.SendDataMessage(this.Peer.ConnectedTo, DeliveryMode.ReliableSequenced, Message.Write(new StartGameMessage()));
            }
        }
        /// <summary>
        /// 方法
        /// 断开游戏
        /// </summary>
        public void DisconnectFromGame()
        {
            this.Peer.Disconnect();
        }
        /// <summary>
        /// 方法
        /// 更新消息
        /// </summary>
        public void Update()
        {
            //游戏不为空进入接口
            if (this.Game != null)
            {
                //如果没有暂停调用
                if (!this.IsPaused)
                {
                    this.Game.Update();
                }
                //步进模块，发送命令
                if (this.Game.StepModule.OutgoingOrders.Count > 0)
                {
                    object @lock = this.Peer.Lock;
                    lock (@lock)
                    {
                        //如果连接到
                        if (this.Peer.ConnectedTo != null)
                        {
                            //玩家命令消息
                            PlayerOrdersMessage playerOrdersMessage = new PlayerOrdersMessage();
                            //添加范围，发送命令消息
                            playerOrdersMessage.Orders.AddRange(this.Game.StepModule.OutgoingOrders);
                            //发送数据，可靠串行，写入玩家命令消息
                            this.Peer.SendDataMessage(this.Peer.ConnectedTo, DeliveryMode.Reliable, Message.Write(playerOrdersMessage));
                        }
                    }
                    //清除发送命令内容
                    this.Game.StepModule.OutgoingOrders.Clear();
                }
                //发送状态散列消息
                if (this.Game.StepModule.OutgoingStateHashMessages.Count > 0)
                {
                    object @lock = this.Peer.Lock;
                    lock (@lock)
                    {
                        //连接到的为空
                        if (this.Peer.ConnectedTo != null)
                        {
                            //从发送的状态散列消息取出游戏状态散列消息
                            foreach (GameStateHashMessage message in this.Game.StepModule.OutgoingStateHashMessages)
                            {
                                //发送数据消息
                                this.Peer.SendDataMessage(this.Peer.ConnectedTo, DeliveryMode.Reliable, Message.Write(message));
                            }
                        }
                    }
                    //清除发送状态hash消息内容
                    this.Game.StepModule.OutgoingStateHashMessages.Clear();
                }
            }
        }

        public void Draw(Color colorTransform)
        {
            Game game = this.Game;
            if (game == null)
            {
                return;
            }
            game.Draw(colorTransform);
        }

        private void Handle(GameCreatedMessage message, PeerData peerData)
        {
            Dispatcher.Dispatch(delegate
            {
                if (this.Game == null)
                {
                    this.Game = new Game(message.CreationParameters);
                    return;
                }
                throw new InvalidOperationException("Game already in progress.");
            }, false);
        }

        private void Handle(GameJoinedMessage message, PeerData peerData)
        {
            Dispatcher.Dispatch(delegate
            {
                if (this.Game == null)
                {
                    this.Game = GameState.ToGame(message.GameState);
                    this.Game.StepModule.IncomingTickMessages.AddRange(message.TickMessages);
                    return;
                }
                throw new InvalidOperationException("Game already in progress.");
            }, false);
        }

        private void Handle(TickMessage message, PeerData peerData)
        {
            Dispatcher.Dispatch(delegate
            {
                if (this.Game != null)
                {
                    this.Game.StepModule.IncomingTickMessages.Add(message);
                    return;
                }
                throw new InvalidOperationException("Game not in progress.");
            }, false);
        }

        private void Handle(RequestGameImageMessage message, PeerData peerData)
        {
            Dispatcher.Dispatch(delegate
            {
                if (this.Game != null)
                {
                    GameImageMessage message2 = new GameImageMessage
                    {
                        GameImage = GameImage.FromGame(this.Game)
                    };
                    this.Peer.SendDataMessage(this.Peer.ConnectedTo, DeliveryMode.Reliable, Message.Write(message2));
                    return;
                }
                throw new InvalidOperationException("Game not in progress.");
            }, false);
        }

        private void Handle(RequestGameStateMessage message, PeerData peerData)
        {
            Dispatcher.Dispatch(delegate
            {
                if (this.Game != null)
                {
                    GameStateMessage gameStateMessage = new GameStateMessage();
                    gameStateMessage.GameState = GameState.FromGame(this.Game, false);
                    gameStateMessage.NonDefeatedFactions = (from p in this.Game.PlayersModule.Players
                                                            where p.Status == FactionStatus.Undecided
                                                            select p.Faction into f
                                                            orderby this.Game.PlanetsModule.GetFactionProductionPercentage(f)
                                                            select f).ToDynamicArray<Faction>();
                    GameStateMessage message2 = gameStateMessage;
                    this.Peer.SendDataMessage(this.Peer.ConnectedTo, DeliveryMode.Reliable, Message.Write(message2));
                    return;
                }
                throw new InvalidOperationException("Game not in progress.");
            }, false);
        }
        /// <summary>
        /// 私有方法
        /// 窗口活动状态
        /// 如果检查使用进程开启，将暂停关闭
        /// </summary>
        private void WindowActivated()
        {
            if (this.IsUsingInProcessTransmitter)
            {
                this.IsPaused = false;
            }
        }
        /// <summary>
        /// 私有方法
        /// 窗口暂停
        /// 如果检查使用进程开启，将暂停开启
        /// </summary>
        private void WindowDeactivated()
        {
            if (this.IsUsingInProcessTransmitter)
            {
                this.IsPaused = true;
            }
        }
        /// <summary>
        /// 私有方法
        /// 窗口关闭
        /// 销毁资源，如果没有进程，将线程休眠750ms
        /// </summary>
        private void WindowClosed()
        {
            this.Dispose();
            if (!this.IsUsingInProcessTransmitter)
            {
                Thread.Sleep(750);
            }
        }
    }
}
