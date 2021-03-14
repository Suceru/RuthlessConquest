
using Engine;
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    internal class StepModule : Module
    {
        private bool StepInProgress;

        private double StepsToTake;

        private Engine.Random InternalRandom = new Engine.Random(0);

        public Engine.Random Random
        {
            get
            {
                if (StepInProgress)
                {
                    return InternalRandom;
                }

                throw new InvalidOperationException();
            }
        }

        public int StepIndex
        {
            get;
            private set;
        }

        public int MaxAllowedStepIndex
        {
            get;
            private set;
        }

        public bool IsGameStarted
        {
            get;
            private set;
        }

        public int GameStartStepIndex
        {
            get;
            private set;
        }

        public int CountdownStepsLeft
        {
            get
            {
                if (!IsGameStarted)
                {
                    return 0;
                }

                return Math.Max(Game.CreationParameters.CountdownTicksCount * 30 - (StepIndex - GameStartStepIndex), 0);
            }
        }

        public double ConnectionInterruptionStartTime
        {
            get;
            private set;
        }

        public DynamicArray<TickMessage> IncomingTickMessages
        {
            get;
        } = new DynamicArray<TickMessage>();

        /// <summary>
        /// 发送命令
        /// </summary>
        public DynamicArray<Order> OutgoingOrders
        {
            get;
        } = new DynamicArray<Order>();

        /// <summary>
        /// 发送状态散列消息
        /// </summary>
        public DynamicArray<GameStateHashMessage> OutgoingStateHashMessages
        {
            get;
        } = new DynamicArray<GameStateHashMessage>();


        public StepModule(Game game)
            : base(game)
        {
        }

        public override void Serialize(InputArchive archive)
        {
            archive.Serialize("StepIndex", delegate (int v)
            {
                StepIndex = v;
            });
            archive.Serialize("IsGameStarted", delegate (bool v)
            {
                IsGameStarted = v;
            });
            archive.Serialize("GameStartStepIndex", delegate (int v)
            {
                GameStartStepIndex = v;
            });
        }

        public override void Serialize(OutputArchive archive)
        {
            archive.Serialize("StepIndex", StepIndex);
            archive.Serialize("IsGameStarted", IsGameStarted);
            archive.Serialize("GameStartStepIndex", GameStartStepIndex);
        }

        public void Update()
        {
            float num = (ConnectionInterruptionStartTime > 0.0) ? ((float)(Time.FrameStartTime - ConnectionInterruptionStartTime)) : 0f;
            if (num > 3f)
            {
                Game.Screen.ConnectionInterruptionMessageWidget.SetMessage($"CONNECTION INTERRUPTED...  {num:0}s", Color.White);
            }
            else if (MaxAllowedStepIndex > StepIndex + 300)
            {
                Game.Screen.ConnectionInterruptionMessageWidget.SetMessage($"CATCHING UP... {(MaxAllowedStepIndex - StepIndex) / 60f:0}s", Color.White);
            }
            else
            {
                Game.Screen.ConnectionInterruptionMessageWidget.ClearMessage();
            }

            TickMessage[] array = IncomingTickMessages.ToArray();
            foreach (TickMessage tickMessage in array)
            {
                MaxAllowedStepIndex = MathUtils.Max((tickMessage.Tick + 1) * 30, MaxAllowedStepIndex);
            }

            StepsToTake += Time.FrameDuration / 0.0166666675f;
            StepsToTake = MathUtils.Clamp(StepsToTake, 0.0, 10.0);
            double realTime = Time.RealTime;
            while (StepsToTake >= 1.0 && Time.RealTime - realTime < 0.10000000149011612)
            {
                if (StepIndex < MaxAllowedStepIndex)
                {
                    ConnectionInterruptionStartTime = 0.0;
                    Step();
                }
                else if (MaxAllowedStepIndex > 0 && ConnectionInterruptionStartTime == 0.0)
                {
                    ConnectionInterruptionStartTime = Time.RealTime;
                }

                double num2 = 0.4;
                if ((MaxAllowedStepIndex - StepIndex) * 0.0166666675f > 0.5 + num2)
                {
                    StepsToTake = 1.0;
                }
                else
                {
                    StepsToTake -= 1.0;
                }
            }
        }

        private void Step()
        {
            InternalRandom.Reset(MathUtils.Hash(Game.CreationParameters.Seed + StepIndex));
            StepInProgress = true;
            try
            {
                if (StepIndex % 600 == 0)
                {
                    GameState gameState = GameState.FromGame(Game, forceBinary: true);
                    OutgoingStateHashMessages.Add(new GameStateHashMessage
                    {
                        StepIndex = StepIndex,
                        StateHash = gameState.CalculateHash()
                    });
                }

                if (StepIndex % 30 == 0)
                {
                    int tick = StepIndex / 30;
                    TickMessage tickMessage = IncomingTickMessages.First((TickMessage o) => o.Tick == tick);
                    IncomingTickMessages.Remove(tickMessage);
                    HandleTickMessage(tickMessage);
                }

                if (IsGameStarted && CountdownStepsLeft <= 0)
                {
                    BodiesModule.BuildGrid();
                    PlanetsModule.Step();
                    ShipsModule.Step();
                    if (!PlayersModule.IsGameFinished)
                    {
                        PlayersModule.Step();
                        SpecialEventsModule.Step();
                    }
                }

                StepIndex++;
            }
            finally
            {
                StepInProgress = false;
            }
        }

        private void HandleTickMessage(TickMessage message)
        {
            if (message.IsGameStarted && !IsGameStarted)
            {
                IsGameStarted = true;
                GameStartStepIndex = StepIndex;
            }

            if (message.PlayerDescriptions.Count > 0)
            {
                DynamicArray<Player> dynamicArray = PlayersModule.Players.Concat(PlayersModule.Spectators).ToDynamicArray();
                foreach (Player player in dynamicArray)
                {
                    if (!message.PlayerDescriptions.Any((PlayerDescription pd) => pd.Guid == player.Guid))
                    {
                        Game.RemoveEntity(player);
                        if (player.Type == PlayerType.Human)
                        {
                            string text = (player.Faction == Faction.None) ? $"Spectator {player.Name} disconnected ({PlayersModule.Spectators.Count})" : (player.Name + " disconnected");
                            Game.Screen.MessagesListWidget.AddMessage(text, Player.GetColor(player.Faction), playSound: true);
                        }
                    }
                }

                bool flag = dynamicArray.Count == 0;
                foreach (PlayerDescription pd2 in message.PlayerDescriptions)
                {
                    Player player2 = dynamicArray.FirstOrDefault((Player p) => p.Guid == pd2.Guid);
                    if (player2 != null)
                    {
                        if (pd2.Faction != player2.Faction || pd2.Type != player2.Type || pd2.Name != player2.Name || pd2.Platform != player2.Platform)
                        {
                            int index = Game.Entities.IndexOf(player2);
                            Game.RemoveEntity(player2);
                            Game.AddEntity(index, Player.CreatePlayer(pd2));
                        }
                    }
                    else
                    {
                        Game.AddEntity(Player.CreatePlayer(pd2));
                    }

                    if (!flag && pd2.Guid != SettingsManager.PlayerGuid && pd2.Type == PlayerType.Human && player2 == null)
                    {
                        string text2 = (pd2.Faction == Faction.None) ? $"Spectator {pd2.Name} connected ({PlayersModule.Spectators.Count})" : (pd2.Name + " connected");
                        Game.Screen.MessagesListWidget.AddMessage(text2, Player.GetColor(pd2.Faction), playSound: true);
                    }
                }
            }

            foreach (KeyValuePair<Faction, DynamicArray<Order>> item in message.OrdersByFaction)
            {
                foreach (Order item2 in item.Value)
                {
                    Player player3 = PlayersModule.FindPlayer(item.Key);
                    Planet planet = PlanetsModule.Planets[item2.PlanetIndex];
                    if (item2.RouteIndexes != null)
                    {
                        IEnumerable<Planet> route = item2.RouteIndexes.Select((int i) => PlanetsModule.Planets[i]);
                        ShipsModule.SendShips(planet, route, item2.ShipsCount, item2.GiftToFaction);
                    }

                    if (item2.LaunchSatellite)
                    {
                        planet.LaunchSatellite();
                    }

                    if (item2.EnableDisableSatellites.HasValue && player3 != null)
                    {
                        player3.AreSatellitesEnabled = item2.EnableDisableSatellites.Value;
                    }
                }
            }
        }
    }
}