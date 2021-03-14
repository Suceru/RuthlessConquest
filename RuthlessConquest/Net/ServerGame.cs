using System;
using System.Collections.Generic;
using System.Linq;
using Comms;
using Engine;

namespace Game
{
    internal class ServerGame
    {
        public GameCreationParameters CreationParameters { get; }

        public DynamicArray<ServerHumanPlayer> Players { get; } = new DynamicArray<ServerHumanPlayer>();

        public int Tick { get; private set; }

        public int GameStartTick { get; private set; } = -1;

        public GameImage GameImage { get; private set; }

        public bool DesyncDetected { get; private set; }

        public ServerGame(Server server, CreateGameMessage message, PeerData peerData)
        {
            this.GameId = NextGameId++;
            this.Server = server;
            this.LastActivityTime = Time.RealTime;
            this.CreationParameters = message.CreationParameters;
            this.Players.Add(new ServerHumanPlayer(this, true, this.CreationParameters.CreatingPlayerFaction, this.CreationParameters.CreatingPlayerName, this.CreationParameters.CreatingPlayerGuid, this.CreationParameters.CreatingPlayerPlatform, peerData, message.ReceivedVersion));
        }
        public void Run()
        {
            double time = Time.RealTime;
            double num1 = time - this.LastActivityTime;
            if (num1 > 600.0)
            {
                Log.Information(string.Format("Game {0} with {1} players was idle for {2:0} seconds, ending", GameId, Players.Count, num1));
                this.EndGame();
            }
            else
            {
                TickMessage tickMessage = new TickMessage();
                tickMessage.Tick = this.Tick;
                tickMessage.IsGameStarted = this.GameStartTick >= 0;
                if (this.PlayersChanged)
                {
                    this.PlayersChanged = false;
                    foreach (ServerHumanPlayer player in this.Players)
                        tickMessage.PlayerDescriptions.Add(new PlayerDescription()
                        {
                            Faction = player.Faction,
                            Name = player.Name,
                            Guid = player.Guid,
                            Platform = new Platform?(player.Platform)
                        });
                    foreach (Faction faction1 in this.CreationParameters.Factions)
                    {
                        Faction faction = faction1;
                        if (!tickMessage.PlayerDescriptions.Any<PlayerDescription>(player => player.Faction == faction))
                        {
                            tickMessage.PlayerDescriptions.Count<PlayerDescription>(pd => pd.Type == this.CreationParameters.AILevel);
                            PlayerType aiLevel = this.CreationParameters.AILevel;
                            if (aiLevel > PlayerType.NoviceAI && (int)faction % 2 == 0)
                                --aiLevel;
                            tickMessage.PlayerDescriptions.Add(new PlayerDescription()
                            {
                                Faction = faction,
                                Type = aiLevel,
                                Name = Player.GetPlayerTypeName(aiLevel) + " (AI)",
                                Guid = new Guid((int)faction, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)
                            });
                        }
                    }
                }
                foreach (ServerHumanPlayer player in this.Players)
                {
                    if (player.Orders.Count > 0)
                    {
                        tickMessage.OrdersByFaction.Add(player.Faction, player.Orders.ToDynamicArray<Order>());
                        player.Orders.Clear();
                    }
                }
                foreach (ServerHumanPlayer player in this.Players)
                    this.Server.Peer.SendDataMessage(player.PeerData, DeliveryMode.ReliableSequenced, Message.Write(tickMessage));
                this.TickMessages.Add(tickMessage);
                this.TickMessages.RemoveAll(m => this.Tick > m.Tick + 30);
                if (this.JoinGameMessages.Count > 0 && this.Players.Count > 0)
                {
                    if (time >= this.NextGameStateRequestTime)
                    {
                        this.Server.Peer.SendDataMessage(this.Players[this.NextGameStateRequestPlayer++ % this.Players.Count].PeerData, DeliveryMode.ReliableSequenced, Message.Write(new RequestGameStateMessage()));
                        this.NextGameStateRequestTime = time + 1.0;
                    }
                    int num2 = this.JoinGameMessages.RemoveAll(m => time > m.ReceiveTime + Server.Peer.Settings.ConnectTimeOut);
                    if (num2 > 0)
                        Log.Warning(string.Format("{0} join requests for game {1} timed out.", num2, GameId));
                }
                if (time >= this.NextGameImageRequestTime && this.Players.Count > 0)
                {
                    this.Server.Peer.SendDataMessage(this.Players[this.NextGameImageRequestPlayer++ % this.Players.Count].PeerData, DeliveryMode.Reliable, Message.Write(new RequestGameImageMessage()));
                    this.NextGameImageRequestTime = time + 5.0;
                }
                ++this.Tick;
            }
        }


        public void Handle(JoinGameMessage message, PeerData peerData)
        {
            message.Sender = peerData;
            message.ReceiveTime = Time.RealTime;
            this.JoinGameMessages.Add(message);
            this.NextGameStateRequestTime = 0.0;
        }

        public void Handle(StartGameMessage message, ServerHumanPlayer player)
        {
            if (!player.IsGameCreator)
            {
                throw new InvalidOperationException(string.Format("Received start game from a non-creator player at {0}.", player.PeerData.Address));
            }
            if (this.GameStartTick < 0)
            {
                this.GameStartTick = this.Tick;
                Log.Information(string.Format("Game {0} was started.", player.Game.GameId));
                return;
            }
            throw new InvalidOperationException(string.Format("Received start game for an already started game from {0}.", player.PeerData.Address));
        }

        public void Handle(PlayerOrdersMessage message, ServerHumanPlayer player)
        {
            if (this.Tick >= this.CreationParameters.CountdownTicksCount)
            {
                player.Orders.AddRange(message.Orders);
                this.LastActivityTime = Time.RealTime;
                return;
            }
            throw new InvalidOperationException(string.Format("Received PlayerOrdersMessage during countdown time (tick {0}) from player at {1}.", this.Tick, player.PeerData.Address));
        }

        public void Handle(GameImageMessage message, ServerHumanPlayer player)
        {
            if (this.GameImage == null || message.GameImage.StepIndex > this.GameImage.StepIndex)
            {
                this.GameImage = message.GameImage;
            }
        }

        public void Handle(GameStateMessage message, ServerHumanPlayer player)
        {
            foreach (JoinGameMessage message2 in this.JoinGameMessages.ToArray<JoinGameMessage>())
            {
                this.ProcessJoinGameMessage(message2, message.GameState, message.NonDefeatedFactions);
            }
        }

        public void Handle(GameStateHashMessage message, ServerHumanPlayer player)
        {
            uint num;
            if (player.Game.StateHashes.TryGetValue(message.StepIndex, out num))
            {
                if (message.StateHash != num)
                {
                    this.DesyncDetected = true;
                    Log.Error(string.Format("State desync detected in game {0} at step {1}, Hash1={2}, Hash2={3}", new object[]
                    {
                        player.Game.GameId,
                        message.StepIndex,
                        message.StateHash,
                        num
                    }));
                    return;
                }
            }
            else
            {
                player.Game.StateHashes.Add(message.StepIndex, message.StateHash);
            }
        }

        public void HandleDisconnect(ServerHumanPlayer player)
        {
            Log.Information(string.Format("Player {0} at {1} ({2}, {3}) left game {4}", new object[]
            {
                player.Name,
                player.PeerData.Address,
                player.Platform,
                player.Version,
                this.GameId
            }));
            this.PlayersChanged = true;
            this.LastActivityTime = Time.RealTime;
            this.Players.Remove(player);
            player.PeerData.Tag = null;
            if (this.Players.Count == 0)
            {
                this.EndGame();
            }
        }

        private void EndGame()
        {
            foreach (ServerHumanPlayer serverHumanPlayer in this.Players.ToArray<ServerHumanPlayer>())
            {
                this.Server.Peer.DisconnectPeer(serverHumanPlayer.PeerData);
            }
            this.Server.Games.Remove(this);
            Log.Information(string.Format("Game {0} ended ({1})", this.GameId, this.Server.GetStatsString()));
        }

        private void ProcessJoinGameMessage(JoinGameMessage message, GameState gameState, DynamicArray<Faction> nonDefeatedFactions)
        {
            if (!this.Players.Any((ServerHumanPlayer player) => player.Guid == message.PlayerGuid))
            {
                int minimumTick = (gameState.StepIndex + 30 - 1) / 30;
                DynamicArray<TickMessage> dynamicArray = (from m in this.TickMessages
                                                          where m.Tick >= minimumTick
                                                          select m).ToDynamicArray<TickMessage>();
                if (gameState.StepIndex > 0 && (dynamicArray.Count == 0 || dynamicArray[0].Tick != minimumTick))
                {
                    Log.Warning("Not enough stored TickMessages for received GameState to process JoinGameMessage");
                    return;
                }
                Faction faction = this.FindFactionForPlayer(message.PreferredFaction, nonDefeatedFactions);
                nonDefeatedFactions.Remove(faction);
                ServerHumanPlayer item = new ServerHumanPlayer(this, false, faction, message.PlayerName, message.PlayerGuid, message.PlayerPlatform, message.Sender, message.ReceivedVersion);
                this.Players.Add(item);
                this.PlayersChanged = true;
                this.LastActivityTime = Time.RealTime;
                this.Server.Peer.AcceptConnect(message.Sender, Message.Write(new GameJoinedMessage
                {
                    GameState = gameState,
                    TickMessages = dynamicArray
                }));
                Log.Information(string.Format("Player {0} at {1} ({2}, {3}) joined game {4} as player {5} at Tick {6}, State StepIndex {7}, State size {8} bytes", new object[]
                {
                    message.PlayerName,
                    message.Sender.Address,
                    message.PlayerPlatform,
                    message.ReceivedVersion,
                    this.GameId,
                    faction,
                    this.Tick,
                    gameState.StepIndex,
                    gameState.CalculateSize()
                }));
            }
            else
            {
                this.Server.Peer.RefuseConnect(message.Sender, Message.Write(new RefusedMessage
                {
                    Reason = "Player is already connected to this game"
                }));
            }
            this.JoinGameMessages.Remove(message);
        }

        private Faction FindFactionForPlayer(Faction preferredFaction, DynamicArray<Faction> nonDefeatedFactions)
        {
            if (preferredFaction == Faction.None)
            {
                return Faction.None;
            }
            using (DynamicArray<Faction>.Enumerator enumerator = nonDefeatedFactions.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Faction faction = enumerator.Current;
                    if (!this.Players.Any((ServerHumanPlayer player) => faction == player.Faction))
                    {
                        return faction;
                    }
                }
            }
            return Faction.None;
        }

        private static int NextGameId;

        private Server Server;

        private double NextGameStateRequestTime;

        private int NextGameStateRequestPlayer;

        private double NextGameImageRequestTime;

        private int NextGameImageRequestPlayer;

        private bool PlayersChanged = true;

        private double LastActivityTime;

        private DynamicArray<TickMessage> TickMessages = new DynamicArray<TickMessage>();

        private DynamicArray<JoinGameMessage> JoinGameMessages = new DynamicArray<JoinGameMessage>();

        private SortedDictionary<int, uint> StateHashes = new SortedDictionary<int, uint>();

        private SortedDictionary<int, object> States = new SortedDictionary<int, object>();

        public int GameId;
    }
}
