using Engine;
using Engine.Graphics;
using System;
using System.Linq;

namespace Game
{
    internal class PlayersModule : Module
    {
        public const int MaxFactions = 6;

        public const int MaxPlayers = 6;

        private DynamicArray<Player> InternalPlayers = new DynamicArray<Player>();

        private DynamicArray<Player> InternalSpectators = new DynamicArray<Player>();

        private DynamicArray<Player> TmpPlayersList = new DynamicArray<Player>();

        private FactionStatus[] FactionStatuses = new FactionStatus[6];

        private double[] FactionStatusChangeTimes = new double[6];

        private bool[] TmpHasAnything = new bool[6];

        public bool IsGameFinished
        {
            get;
            private set;
        }

        public HumanPlayer ControllingPlayer
        {
            get;
            private set;
        }

        public ReadOnlyList<Player> Players => new ReadOnlyList<Player>(InternalPlayers);

        public ReadOnlyList<Player> Spectators => new ReadOnlyList<Player>(InternalSpectators);

        public PlayersModule(Game game)
            : base(game)
        {
            game.EntityAdded += delegate (Entity e)
            {
                Player player2 = e as Player;
                if (player2 != null)
                {
                    if (player2.Faction == Faction.Neutral)
                    {
                        throw new InvalidOperationException();
                    }

                    if (player2.Faction == Faction.None)
                    {
                        InternalSpectators.Add(player2);
                        InternalSpectators = new DynamicArray<Player>(InternalSpectators.OrderBy((Player p) => p.Guid.ToString()));
                    }
                    else
                    {
                        if (InternalPlayers.Any((Player p) => p.Faction == player2.Faction))
                        {
                            throw new InvalidOperationException();
                        }

                        InternalPlayers.Add(player2);
                        InternalPlayers = new DynamicArray<Player>(InternalPlayers.OrderBy((Player p) => p.Guid.ToString()));
                    }

                    if (player2.Guid == SettingsManager.PlayerGuid)
                    {
                        ControllingPlayer = (HumanPlayer)player2;
                    }
                }
            };
            game.EntityRemoved += delegate (Entity e)
            {
                Player player = e as Player;
                if (player != null)
                {
                    InternalPlayers.Remove(player);
                    InternalSpectators.Remove(player);
                    if (player == ControllingPlayer)
                    {
                        ControllingPlayer = null;
                    }
                }
            };
        }

        public Player FindPlayer(Faction faction)
        {
            foreach (Player player in Players)
            {
                if (player.Faction == faction)
                {
                    return player;
                }
            }

            return null;
        }

        public FactionStatus GetFactionStatus(Faction faction)
        {
            if (faction < Faction.Faction1 || faction >= Faction.Neutral)
            {
                return FactionStatus.Undecided;
            }

            return FactionStatuses[(int)faction];
        }

        public double GetFactionStatusChangeTime(Faction faction)
        {
            if (faction < Faction.Faction1 || faction >= Faction.Neutral)
            {
                return 0.0;
            }

            return FactionStatusChangeTimes[(int)faction];
        }

        public void Update()
        {
            foreach (Player player in Players)
            {
                player.Update();
            }

            int num = 0;
            if (ControllingPlayer != null)
            {
                Game.Screen.PlayerLabels[num++].Player = ControllingPlayer;
            }

            foreach (Player player2 in Players)
            {
                if (num < Game.Screen.PlayerLabels.Count && player2 != ControllingPlayer && (player2.Status >= FactionStatus.Undecided || player2 is HumanPlayer))
                {
                    Game.Screen.PlayerLabels[num++].Player = player2;
                }
            }

            while (num < Game.Screen.PlayerLabels.Count)
            {
                Game.Screen.PlayerLabels[num++].Player = null;
            }
        }

        public void Draw(Color colorTransform)
        {
            foreach (Player player in Players)
            {
                player.Draw(colorTransform);
            }

            CameraModule.PrimitivesRenderer.Flush(CameraModule.WorldToScreenMatrix * PrimitivesRenderer2D.ViewportMatrix());
        }

        public void Step()
        {
            foreach (Player player in Players)
            {
                player.Step();
            }

            UpdateFactionStatuses();
        }

        private void UpdateFactionStatuses()
        {
            for (int i = 0; i < 6; i++)
            {
                TmpHasAnything[i] = false;
            }

            foreach (Planet planet in PlanetsModule.Planets)
            {
                if (planet.Faction >= Faction.Faction1 && planet.Faction < Faction.Neutral)
                {
                    TmpHasAnything[(int)planet.Faction] = true;
                }
            }

            foreach (Ship ship in ShipsModule.Ships)
            {
                if (ship.Faction >= Faction.Faction1 && ship.Faction < Faction.Neutral)
                {
                    TmpHasAnything[(int)ship.Faction] = true;
                }
            }

            Faction? faction = null;
            for (int j = 0; j < 6; j++)
            {
                bool flag = true;
                foreach (Planet planet2 in PlanetsModule.Planets)
                {
                    if (planet2.IsSpecial && planet2.Faction != (Faction)j)
                    {
                        flag = false;
                        break;
                    }
                }

                if (flag)
                {
                    faction = (Faction)j;
                    break;
                }
            }

            Faction? faction2 = null;
            for (int k = 0; k < 6; k++)
            {
                if (!TmpHasAnything[k])
                {
                    continue;
                }

                bool flag2 = true;
                for (int l = 0; l < 6; l++)
                {
                    if (l != k && TmpHasAnything[l])
                    {
                        flag2 = false;
                        break;
                    }
                }

                if (flag2)
                {
                    faction2 = (Faction)k;
                    break;
                }
            }

            IsGameFinished = true;
            for (int m = 0; m < 6; m++)
            {
                if (FactionStatuses[m] != 0)
                {
                    continue;
                }

                if (!TmpHasAnything[m])
                {
                    FactionStatuses[m] = FactionStatus.LostEliminated;
                    FactionStatusChangeTimes[m] = Time.FrameStartTime;
                }
                else if (faction.HasValue && faction != (Faction?)m)
                {
                    FactionStatuses[m] = FactionStatus.Lost;
                    FactionStatusChangeTimes[m] = Time.FrameStartTime;
                }
                else if (faction2 == (Faction?)m)
                {
                    FactionStatuses[m] = FactionStatus.WonEliminatedOthers;
                    FactionStatusChangeTimes[m] = Time.FrameStartTime;
                }
                else if (faction == (Faction?)m)
                {
                    FactionStatuses[m] = FactionStatus.Won;
                    FactionStatusChangeTimes[m] = Time.FrameStartTime;
                }
                else
                {
                    IsGameFinished = false;
                }

                if (FactionStatuses[m] > FactionStatus.Undecided)
                {
                    Player player = FindPlayer((Faction)m);
                    if (player != null)
                    {
                        Game.Screen.MessagesListWidget.AddMessage(player.Name + " is victorious!", Player.GetColor(player.Faction), playSound: true);
                    }
                }
                else if (FactionStatuses[m] < FactionStatus.Undecided)
                {
                    Player player2 = FindPlayer((Faction)m);
                    if (player2 != null)
                    {
                        Game.Screen.MessagesListWidget.AddMessage(player2.Name + " defeated!", Player.GetColor(player2.Faction), playSound: true);
                    }
                }
            }
        }
    }
}