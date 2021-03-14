using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Engine;

namespace Game
{
    /// <summary>
    /// 保护类
    /// 正在连接对话框
    /// 继承:忙碌对话框
    /// </summary>
    internal class ConnectingDialog : BusyDialog
    {
        public ConnectingDialog(GameDescription gameDescription, bool spectate) : base("CONNECTING", "Waiting for " + gameDescription.ServerDescription.Name + "...")
        {
            try
            {
                this.Client = new Client(false);
                this.Client.JoinGame(gameDescription.ServerDescription.Address, gameDescription.GameId, spectate);
            }
            catch (Exception error)
            {
                Client client = this.Client;
                if (client != null)
                {
                    client.Dispose();
                }
                this.Error = error;
            }
        }

        public ConnectingDialog(GameCreationParameters creationParameters, ConnectingDialog.GameType gameType) : base(null, null)
        {
            try
            {
                //游戏处于
                switch (gameType)
                {
                    case GameType.InProcess:
                        LargeMessage = "STARTING GAME";
                        this.Server = new Server(false, true);
                        this.Client = new Client(true);
                        this.Client.CreateGame(this.Server.Peer.Address, creationParameters);
                        break;
                    case GameType.Internet:
                        LargeMessage = "CONNECTING";
                        SmallMessage = "Waiting for local server...";
                        this.Server = new Server(false, false);
                        this.Client = new Client(false);
                        IPEndPoint serverAddress = new IPEndPoint(this.Client.Peer.Address.Address, this.Server.Peer.Address.Port);
                        this.Client.CreateGame(serverAddress, creationParameters);
                        break;
                    case GameType.Local:
                        LargeMessage = "SEARCHING FOR SERVER";
                        this.CreationParameters = creationParameters;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown GameType.");
                }
                /* 原代码
                 * if (gameType == ConnectingDialog.GameType.InProcess)
                  {
                      base.LargeMessage = "STARTING GAME";
                      this.Server = new Server(false, true);
                      this.Client = new Client(true);
                      this.Client.CreateGame(this.Server.Peer.Address, creationParameters);
                  }
                  else if (gameType == ConnectingDialog.GameType.Local)
                  {
                      base.LargeMessage = "CONNECTING";
                      base.SmallMessage = "Waiting for local server...";
                      this.Server = new Server(false, false);
                      this.Client = new Client(false);
                      IPEndPoint serverAddress = new IPEndPoint(this.Client.Peer.Address.Address, this.Server.Peer.Address.Port);
                      this.Client.CreateGame(serverAddress, creationParameters);
                  }
                  else
                  {
                      if (gameType != ConnectingDialog.GameType.Internet)
                      {
                          throw new InvalidOperationException("Unknown GameType.");
                      }
                      base.LargeMessage = "SEARCHING FOR SERVER";
                      this.CreationParameters = creationParameters;
                  }*/
            }
            catch (Exception error)
            {
                Server server = this.Server;
                if (server != null)
                {
                    server.Dispose();
                }
                Client client = this.Client;
                if (client != null)
                {
                    client.Dispose();
                }
                this.Error = error;
            }
        }

        public override void Update()
        {
            base.Update();
            if (this.Error == null)
            {
                if (this.FirstUpdateTime == 0.0)
                {
                    this.FirstUpdateTime = Time.FrameStartTime;
                }
                if (this.Client != null)
                {
                    if (this.Client.Game != null)
                    {
                        if (Time.FrameStartTime - this.FirstUpdateTime > 0.75)
                        {
                            ScreensManager.SwitchScreen("Game", new object[]
                            {
                                this.Server,
                                this.Client
                            });
                            return;
                        }
                    }
                    else if (this.Client.Error != null)
                    {
                        DialogsManager.HideDialog(this, true);
                        DialogsManager.ShowDialog(null, new MessageDialog("CANNOT CONNECT", this.Client.Error.Message, "OK", null, null), true);
                        this.Client.Dispose();
                        Server server = this.Server;
                        if (server == null)
                        {
                            return;
                        }
                        server.Dispose();
                        return;
                    }
                }
                else if (ServersManager.DiscoveredServers.Count > 0)
                {
                    if (Time.FrameStartTime > this.FirstUpdateTime + 1.0)
                    {
                        ServerDescription serverDescription = (from s in ServersManager.DiscoveredServers
                                                               orderby 1000000.0 * s.Priority - s.Ping descending
                                                               select s).First<ServerDescription>();
                        this.Client = new Client(false);
                        this.Client.CreateGame(serverDescription.Address, this.CreationParameters);
                        SmallMessage = "Waiting for " + serverDescription.Name + "...";
                        return;
                    }
                }
                else
                {
                    ServersManager.StartServerDiscovery(1.0, 1.0);
                    if (Time.FrameStartTime > this.FirstUpdateTime + 6.0)
                    {
                        DialogsManager.HideDialog(this, true);
                        DialogsManager.ShowDialog(null, new MessageDialog("NO SERVER FOUND", "No internet server is currently available. You can still create a LAN/WIFI game.", "OK", null, null), true);
                    }
                }
                return;
            }
            DialogsManager.HideDialog(this, true);
            SocketException ex = this.Error as SocketException;
            if (ex != null)
            {
                DialogsManager.ShowDialog(null, new MessageDialog("NETWORK ERROR", string.Format("Error code: {0} ({1}).", ex.SocketErrorCode, (int)ex.SocketErrorCode), "OK", null, null), true);
                return;
            }
            DialogsManager.ShowDialog(null, new MessageDialog("ERROR", this.Error.Message, "OK", null, null), true);
        }

        public override void DialogHidden()
        {
            if (ServersManager.IsDiscoveryStarted)
            {
                ServersManager.StartServerDiscovery(3.0, 1.0);
            }
        }

        private Exception Error;

        private Server Server;

        private Client Client;

        private double FirstUpdateTime;

        private GameCreationParameters CreationParameters;
        /// <summary>
        /// 枚举
        /// 游戏类型
        /// </summary>
        public enum GameType
        {
            /// <summary>
            /// 进程中
            /// </summary>
            InProcess,
            /// <summary>
            /// 本地
            /// </summary>
            Local,
            /// <summary>
            /// 联网
            /// </summary>
            Internet
        }
    }
}
