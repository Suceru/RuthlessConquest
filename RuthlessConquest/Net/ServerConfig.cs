using System;
using System.Reflection;
using System.Xml.Linq;
using Engine;

namespace Game
{
    internal class ServerConfig
    {
        public ServerConfig(Server server)
        {
            this.Server = server;
            if (this.Server.IsDedicatedServer)
            {
                this.LoadConfigFile();
            }
        }

        public void Run()
        {
            if (!this.Server.IsDisposing && this.ShutdownSequence && this.Server.Games.Count == 0)
            {
                Log.Information("No games and ShutdownSequence started, server shutting down.");
                this.Server.Dispose();
            }
            if (Time.RealTime >= this.NextUpdateTime)
            {
                this.NextUpdateTime = Time.RealTime + 5.0;
                if (this.Server.IsDedicatedServer)
                {
                    this.LoadConfigFile();
                    DateTime fileLastWriteTime = Storage.GetFileLastWriteTime("system:" + Assembly.GetEntryAssembly().Location);
                    if (this.LastAssemblyTime != null)
                    {
                        if (fileLastWriteTime != this.LastAssemblyTime)
                        {
                            this.ShutdownSequence = true;
                            return;
                        }
                    }
                    else
                    {
                        this.LastAssemblyTime = new DateTime?(fileLastWriteTime);
                    }
                }
            }
        }

        private void LoadConfigFile()
        {
            string text = "app:ServerConfig.xml";
            try
            {
                string text2 = Storage.ReadAllText(text);
                if (text2 != this.LastLoadedConfig)
                {
                    this.LastLoadedConfig = text2;
                    XElement xelement = XElement.Parse(text2);
                    this.ServerName = xelement.Attribute("ServerName").Value;
                    this.ServerPriority = int.Parse(xelement.Attribute("ServerPriority").Value);
                    this.ShutdownSequence = bool.Parse(xelement.Attribute("ShutdownSequence").Value);
                    Log.Information("Loaded new config:\n" + text2);
                }
            }
            catch (Exception ex)
            {
                Log.Warning("Error loading config file " + text + ", reason: " + ex.Message);
            }
        }

        private Server Server;

        private string LastLoadedConfig = string.Empty;

        private DateTime? LastAssemblyTime;

        private double NextUpdateTime;

        public string ServerName = "LocalServer";

        public int ServerPriority = 100;

        public bool ShutdownSequence;
    }
}
