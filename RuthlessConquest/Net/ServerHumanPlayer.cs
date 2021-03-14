using System;
using Comms;
using Engine;

namespace Game
{
    internal class ServerHumanPlayer
    {
        public ServerHumanPlayer(ServerGame game, bool isGameCreator, Faction faction, string name, Guid guid, Platform platform, PeerData peerData, Version version)
        {
            this.Game = game;
            this.IsGameCreator = isGameCreator;
            this.Faction = faction;
            this.Name = name;
            this.Guid = guid;
            this.Platform = platform;
            this.PeerData = peerData;
            peerData.Tag = this;
            this.Version = version;
        }

        public ServerGame Game;

        public bool IsGameCreator;

        public Faction Faction;

        public string Name;

        public Guid Guid;

        public Platform Platform;

        public PeerData PeerData;

        public Version Version;

        public DynamicArray<Order> Orders = new DynamicArray<Order>();
    }
}
