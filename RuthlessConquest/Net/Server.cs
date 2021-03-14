using Comms;
using Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    /// <summary>
    /// 保护类
    /// 服务器
    /// 继承:销毁接口
    /// </summary>
    internal class Server : IDisposable
    {
        /// <summary>
        /// 端口号
        /// </summary>
        public const int PortNumber = 40102;
        /// <summary>
        /// 每秒的步数
        /// </summary>
        public const int StepsPerSecond = 60;
        /// <summary>
        /// 步进时间
        /// </summary>
        public const float StepDuration = 0.01666667f;
        /// <summary>
        /// 每个Tick的步数
        /// </summary>
        public const int StepsPerTick = 30;
        public const float TickDuration = 0.5f;
        private Thread Thread;
        private ServerConfig Config;
        private byte[] CachedGameListMessageBytes;
        private double CachedGameListMessageTime;
        private double TotalLoadTime;
        private double LastLoadReportTime;
        private Dictionary<IPEndPoint, double> RecentlySeenUsers = new Dictionary<IPEndPoint, double>();
        private DynamicArray<IPEndPoint> ToRemove = new DynamicArray<IPEndPoint>();

        public Peer Peer { get; }

        public bool IsDedicatedServer { get; }

        public bool IsUsingInProcessTransmitter { get; }

        public bool IsPaused { get; set; }

        public bool IsDisposing { get; private set; }

        public DynamicArray<ServerGame> Games { get; } = new DynamicArray<ServerGame>();

        public Server(bool isDedicatedServer, bool useInProcessTransmitter)
        {
            Log.Information(string.Format("Starting RuthlessConquest server on {0:dd/MM/yyyy HH:mm:ss} UTC, version {1}", DateTime.Now.ToUniversalTime(), Assembly.GetExecutingAssembly().GetName().Version));
            this.IsDedicatedServer = isDedicatedServer;
            this.IsUsingInProcessTransmitter = useInProcessTransmitter;
            this.Config = new ServerConfig(this);
            IPacketTransmitter transmitter = !useInProcessTransmitter ? new DiagnosticPacketTransmitter(new UdpPacketTransmitter(40102)) : new DiagnosticPacketTransmitter(new InProcessPacketTransmitter());
            Log.Information(string.Format("Server address is {0}", transmitter.Address));
            this.Peer = new Peer(transmitter);
            this.Peer.Settings.SendPeerConnectDisconnectNotifications = false;
            this.Peer.Comm.Settings.ResendPeriods = new float[5]
            {
        0.5f,
        0.5f,
        1f,
        1.5f,
        2f
            };
            this.Peer.Comm.Settings.MaxResends = 20;
            this.Peer.Settings.KeepAlivePeriod = this.IsUsingInProcessTransmitter ? float.PositiveInfinity : 5f;
            this.Peer.Settings.KeepAliveResendPeriod = this.IsUsingInProcessTransmitter ? float.PositiveInfinity : 2f;
            this.Peer.Settings.ConnectionLostPeriod = this.IsUsingInProcessTransmitter ? float.PositiveInfinity : 30f;
            this.Peer.Settings.ConnectTimeOut = 6f;
            this.Peer.Error += e => Log.Error(e);
            this.Peer.PeerDiscoveryRequest += p =>
            {
                if (this.IsDisposing)
                    return;
                if (p.Data.Length < 4)
                    throw new InvalidOperationException("Unrecognized message.");
                this.HandlePeerDiscovery(new Version(p.Data, 0), p.Address);
            };
            this.Peer.ConnectRequest += p =>
            {
                if (this.IsDisposing)
                    return;
                switch (Message.Read(p.Data))
                {
                    case CreateGameMessage message:
                        this.Handle(message, p.Peer);
                        break;
                    case JoinGameMessage message:
                        this.Handle(message, p.Peer);
                        break;
                    default:
                        throw new InvalidOperationException("Unrecognized message.");
                }
            };
            this.Peer.PeerDisconnected += peerData =>
            {
                if (this.IsDisposing)
                    return;
                this.HandleDisconnect(peerData);
            };
            this.Peer.DataMessageReceived += p =>
            {
                if (this.IsDisposing)
                    return;
                switch (Message.Read(p.Data))
                {
                    case StartGameMessage message:
                        this.Handle(message, p.Peer);
                        break;
                    case PlayerOrdersMessage message:
                        this.Handle(message, p.Peer);
                        break;
                    case GameImageMessage message:
                        this.Handle(message, p.Peer);
                        break;
                    case GameStateMessage message:
                        this.Handle(message, p.Peer);
                        break;
                    case GameStateHashMessage message:
                        this.Handle(message, p.Peer);
                        break;
                    default:
                        throw new InvalidOperationException("Unrecognized message.");
                }
            };
            if (this.IsDedicatedServer)
            {
                this.Run();
            }
            else
            {
                Window.Activated += new Action(this.WindowActivated);
                Window.Deactivated += new Action(this.WindowDeactivated);
                Window.Closed += new Action(this.WindowClosed);
                this.Thread = new Thread(new ThreadStart(this.Run));
                this.Thread.IsBackground = true;
                this.Thread.Start();
            }
        }

        public void Dispose()
        {
            Window.Activated -= new Action(this.WindowActivated);
            Window.Deactivated -= new Action(this.WindowDeactivated);
            Window.Closed -= new Action(this.WindowClosed);
            this.IsDisposing = true;
        }

        public ServerHumanPlayer GetPlayer(PeerData peerData) => peerData.Tag as ServerHumanPlayer;

        public string GetStatsString() => string.Format("{0} games with {1} players", Games.Count, this.Games.SelectMany<ServerGame, ServerHumanPlayer>(gd => gd.Players).Count<ServerHumanPlayer>());

        private void Run()
        {
            double realTime1 = Time.RealTime;
            while (!this.IsDisposing)
            {
                double realTime2 = Time.RealTime;
                this.Config.Run();
                lock (this.Peer.Lock)
                {
                    double realTime3 = Time.RealTime;
                    this.ToRemove.Clear();
                    foreach (KeyValuePair<IPEndPoint, double> recentlySeenUser in this.RecentlySeenUsers)
                    {
                        if (realTime3 > recentlySeenUser.Value + 60.0)
                            this.ToRemove.Add(recentlySeenUser.Key);
                    }
                    foreach (IPEndPoint key in this.ToRemove)
                        this.RecentlySeenUsers.Remove(key);
                }
                while (Time.RealTime >= realTime1)
                {
                    realTime1 += 0.5;
                    if (!this.IsPaused)
                    {
                        lock (this.Peer.Lock)
                        {
                            foreach (ServerGame serverGame in this.Games.ToArray<ServerGame>())
                                serverGame.Run();
                        }
                    }
                }
                double realTime4 = Time.RealTime;
                double num = realTime1 - Time.RealTime;
                if (num > 0.0)
                    Thread.Sleep(MathUtils.Clamp((int)(0.5 * num * 1000.0), 10, 1000));
                double realTime5 = Time.RealTime;
                this.TotalLoadTime += realTime4 - realTime2;
                if (this.LastLoadReportTime == 0.0 || realTime5 >= this.LastLoadReportTime + 60.0)
                {
                    if (this.LastLoadReportTime > 0.0 && this.Peer.Comm.Transmitter is DiagnosticPacketTransmitter transmitter)
                        Log.Information(string.Format("Server load {0:0.000000} ({1}), ", this.TotalLoadTime / (realTime5 - this.LastLoadReportTime), this.GetStatsString()) + string.Format("{0} kB sent, {1} kB received, ", transmitter.BytesSent / 1024L, transmitter.BytesReceived / 1024L) + string.Format("{0} users in the last 60 seconds", RecentlySeenUsers.Count));
                    this.LastLoadReportTime = realTime5;
                    this.TotalLoadTime = 0.0;
                }
            }
            this.Peer.DisconnectAllPeers();
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                this.Peer.Dispose();
            });
        }

        private void HandlePeerDiscovery(Version version, IPEndPoint address)
        {
            if (version.GetNetworkProtocolVersion() == VersionsManager.Version.GetNetworkProtocolVersion())
            {
                double realTime = Time.RealTime;
                this.RecentlySeenUsers[address] = realTime;
                if (this.CachedGameListMessageBytes == null || realTime > this.CachedGameListMessageTime + 0.5)
                {
                    GameListMessage gameListMessage = new GameListMessage()
                    {
                        ServerPriority = this.Config.ShutdownSequence ? 0 : this.Config.ServerPriority,
                        ServerName = this.Config.ServerName
                    };
                    int num = 0;
                    foreach (ServerGame serverGame in this.Games.OrderBy<ServerGame, int>(g => g.Tick))
                    {
                        if (num <= 40)
                        {
                            gameListMessage.GameDescriptions.Add(new GameDescription()
                            {
                                GameId = serverGame.GameId,
                                HumanPlayersCount = serverGame.Players.Count<ServerHumanPlayer>(p => p.Faction != Faction.None),
                                SpectatorsCount = serverGame.Players.Count<ServerHumanPlayer>(p => p.Faction == Faction.None),
                                TicksSinceStart = serverGame.GameStartTick >= 0 ? serverGame.Tick - serverGame.GameStartTick : -1,
                                CreationParameters = serverGame.CreationParameters,
                                GameImage = serverGame.GameImage
                            });
                            ++num;
                        }
                        else
                            break;
                    }
                    this.CachedGameListMessageBytes = Message.Write(gameListMessage);
                    this.CachedGameListMessageTime = realTime;
                }
                this.Peer.RespondToDiscovery(address, DeliveryMode.Unreliable, this.CachedGameListMessageBytes);
            }
            else
            {
                MemoryStream memoryStream = new MemoryStream();
                memoryStream.WriteByte(0);
                memoryStream.Write(VersionsManager.Version.ToByteArray(), 0, 4);
                this.Peer.RespondToDiscovery(address, DeliveryMode.Raw, memoryStream.ToArray());
            }
        }

        private void Handle(CreateGameMessage message, PeerData peerData)
        {
            if (VerifyPlayerName(message.CreationParameters.CreatingPlayerName))
            {
                this.RecentlySeenUsers[peerData.Address] = Time.RealTime;
                if (this.Config.ShutdownSequence)
                    this.Peer.RefuseConnect(peerData, Message.Write(new RefusedMessage()
                    {
                        Reason = "Server restarting, please wait a while and try again."
                    }));
                else if (this.Games.Count < 400)
                {
                    ServerGame serverGame = new ServerGame(this, message, peerData);
                    this.Games.Add(serverGame);
                    this.Peer.AcceptConnect(peerData, Message.Write(new GameCreatedMessage()
                    {
                        GameId = serverGame.GameId,
                        CreationParameters = message.CreationParameters
                    }));
                    Log.Information(string.Format("Player {0} at {1} ({2}, {3}) created game {4} ({5})", message.CreationParameters.CreatingPlayerName, peerData.Address, message.CreationParameters.CreatingPlayerPlatform, message.ReceivedVersion, serverGame.GameId, this.GetStatsString()));
                }
                else
                    this.Peer.RefuseConnect(peerData, Message.Write(new RefusedMessage()
                    {
                        Reason = "Too many games in progress, please wait a while and try again."
                    }));
            }
            else
                this.Peer.RefuseConnect(peerData, Message.Write(new RefusedMessage()
                {
                    Reason = "Please change your nickname in Settings."
                }));
        }

        private void Handle(JoinGameMessage message, PeerData peerData)
        {
            if (VerifyPlayerName(message.PlayerName))
            {
                this.RecentlySeenUsers[peerData.Address] = Time.RealTime;
                ServerGame serverGame = this.Games.FirstOrDefault<ServerGame>(g => g.GameId == message.GameId);
                if (serverGame != null)
                    serverGame.Handle(message, peerData);
                else
                    this.Peer.RefuseConnect(peerData, Message.Write(new RefusedMessage()
                    {
                        Reason = "Game does not exist"
                    }));
            }
            else
                this.Peer.RefuseConnect(peerData, Message.Write(new RefusedMessage()
                {
                    Reason = "Please change your nickname in Settings."
                }));
        }

        private void Handle(StartGameMessage message, PeerData peerData)
        {
            this.RecentlySeenUsers[peerData.Address] = Time.RealTime;
            ServerHumanPlayer player = this.GetPlayer(peerData);
            if (player == null)
                throw new InvalidOperationException(string.Format("Received StartGameMessage from unknown player at {0}.", peerData.Address));
            player.Game.Handle(message, player);
        }

        private void Handle(PlayerOrdersMessage message, PeerData peerData)
        {
            this.RecentlySeenUsers[peerData.Address] = Time.RealTime;
            ServerHumanPlayer player = this.GetPlayer(peerData);
            if (player == null)
                throw new InvalidOperationException(string.Format("Received PlayerOrdersMessage from unknown player at {0}.", peerData.Address));
            player.Game.Handle(message, player);
        }

        private void Handle(GameImageMessage message, PeerData peerData)
        {
            this.RecentlySeenUsers[peerData.Address] = Time.RealTime;
            ServerHumanPlayer player = this.GetPlayer(peerData);
            if (player == null)
                throw new InvalidOperationException(string.Format("Received GameImageMessage from unknown player at {0}.", peerData.Address));
            player.Game.Handle(message, player);
        }

        private void Handle(GameStateMessage message, PeerData peerData)
        {
            this.RecentlySeenUsers[peerData.Address] = Time.RealTime;
            ServerHumanPlayer player = this.GetPlayer(peerData);
            if (player == null)
                throw new InvalidOperationException(string.Format("Received GameStateMessage from unknown player at {0}.", peerData.Address));
            player.Game.Handle(message, player);
        }

        private void Handle(GameStateHashMessage message, PeerData peerData)
        {
            this.RecentlySeenUsers[peerData.Address] = Time.RealTime;
            ServerHumanPlayer player = this.GetPlayer(peerData);
            if (player == null)
                throw new InvalidOperationException(string.Format("Received GameStateHashMessage from unknown player at {0}.", peerData.Address));
            player.Game.Handle(message, player);
        }

        private void HandleDisconnect(PeerData peerData)
        {
            ServerHumanPlayer player = this.GetPlayer(peerData);
            if (player == null)
                throw new InvalidOperationException(string.Format("Received GameStateMessage from unknown player at {0}.", peerData.Address));
            player.Game.HandleDisconnect(player);
        }

        private void WindowActivated()
        {
            if (!this.IsUsingInProcessTransmitter || this.IsDedicatedServer)
                return;
            this.IsPaused = false;
        }

        private void WindowDeactivated()
        {
            if (!this.IsUsingInProcessTransmitter || this.IsDedicatedServer)
                return;
            this.IsPaused = true;
        }

        private void WindowClosed()
        {
            this.Dispose();
            if (this.IsUsingInProcessTransmitter)
                return;
            Thread.Sleep(750);
        }

        private static bool VerifyPlayerName(string playerName) => playerName == "Kaalus" || playerName.Replace(" ", "").ToLower() != "kaalus";
    }
}