using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Graphics;
using Engine.Serialization;

namespace Game
{
    internal class Planet : Entity
    {
        public Body Body { get; }

        public Point2 Position
        {
            get
            {
                return this.Body.Position;
            }
            set
            {
                this.Body.Position = value;
            }
        }

        public int SizeClass { get; private set; }

        public int Radius { get; private set; }

        public Faction Faction { get; private set; }

        public int ShipsCount { get; set; }

        public int MaxShipsCount { get; private set; }

        public int ProductionPeriod { get; private set; }

        public int NoProductionSteps { get; private set; }

        public int DefenceFactor { get; private set; }

        public bool IsSpecial { get; private set; }

        public bool IsSelected { get; set; }

        public bool ShowRangeDisc { get; set; }

        public bool ShowHighlight { get; set; }

        public IEnumerable<Planet> ContinuousOrderRoute { get; set; }

        public bool IsControllingPlayerPlanet
        {
            get
            {
                return PlayersModule.ControllingPlayer != null && PlayersModule.ControllingPlayer.Faction == this.Faction;
            }
        }

        private Planet()
        {
            this.Body = new Body();
            this.Body.Tag = this;
        }

        public Planet(Point2 position, Faction faction, int sizeClass, int shipsCount, bool isSpecial) : this()
        {
            this.Position = position;
            this.Faction = faction;
            this.SizeClass = sizeClass;
            this.ShipsCount = shipsCount;
            this.IsSpecial = isSpecial;
        }

        public override void Serialize(InputArchive archive)
        {
            archive.Serialize<Body>("Body", this.Body);
            this.Faction = archive.Serialize<Faction>("Faction");
            this.SizeClass = archive.Serialize<int>("SizeClass");
            this.ShipsCount = archive.Serialize<int>("ShipsCount");
            this.NoProductionSteps = archive.Serialize<int>("NoProductionSteps");
            this.IsSpecial = archive.Serialize<bool>("IsSpecial");
        }

        public override void Serialize(OutputArchive archive)
        {
            archive.Serialize<Body>("Body", this.Body);
            archive.Serialize<Faction>("Faction", this.Faction);
            archive.Serialize("SizeClass", this.SizeClass);
            archive.Serialize("ShipsCount", this.ShipsCount);
            archive.Serialize("NoProductionSteps", this.NoProductionSteps);
            archive.Serialize("IsSpecial", this.IsSpecial);
        }

        protected internal override void OnAdded()
        {
            switch (this.SizeClass)
            {
                case 0:
                    this.MaxShipsCount = 150;
                    this.ProductionPeriod = 130;
                    this.DefenceFactor = 75;
                    break;
                case 1:
                    this.MaxShipsCount = 200;
                    this.ProductionPeriod = 90;
                    this.DefenceFactor = 75;
                    break;
                case 2:
                    this.MaxShipsCount = 250;
                    this.ProductionPeriod = 50;
                    this.DefenceFactor = 75;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            this.Body.RepelFactor = Game.CreationParameters.PlanetRepelFactor;
            this.Body.GridRadius = GetRadius(this.SizeClass) * 2;
            this.Radius = GetRadius(this.SizeClass);
            this.ProductionPeriod += (int)(MathUtils.Hash((uint)this.Position.X) % 7U);
            this.Color1 = (this.Color2 = GetColor(this.Faction));
            this.ColorFactor = 1f;
        }

        public IEnumerable<Satellite> Satellites
        {
            get
            {
                return from s in PlanetsModule.Satellites
                       where s.Planet == this
                       select s;
            }
        }

        public int GetSatelliteCost()
        {
            if (this.Satellites.Count<Satellite>() != 0)
            {
                return 300;
            }
            return 150;
        }

        public bool CanLaunchSatellite(bool ignoreCost)
        {
            return this.Satellites.Count<Satellite>() < 2 && (ignoreCost || this.ShipsCount >= this.GetSatelliteCost());
        }

        public void LaunchSatellite()
        {
            if (this.CanLaunchSatellite(false))
            {
                this.ShipsCount -= this.GetSatelliteCost();
                Game.AddEntity(new Satellite(this, 0));
                AudioManager.PlaySound(Sounds.SatelliteLaunch, true, 1f, 1f, 0f);
            }
        }

        public void ChangeOwner(Faction faction)
        {
            if (PlayersModule.ControllingPlayer != null)
            {
                if (this.Faction == PlayersModule.ControllingPlayer.Faction && faction != PlayersModule.ControllingPlayer.Faction)
                {
                    AudioManager.PlaySound(Sounds.PlanetLost, true, 1f, 1f, 0f);
                }
                else if (this.Faction != PlayersModule.ControllingPlayer.Faction && faction == PlayersModule.ControllingPlayer.Faction)
                {
                    AudioManager.PlaySound(Sounds.PlanetWon, true, 1f, 1f, 0f);
                }
            }
            if (ParticlesModule.ParticleSystems.Count((ParticleSystemBase p) => p is CapturePlanetParticleSystem) < 5)
            {
                ParticlesModule.AddParticleSystem(new CapturePlanetParticleSystem(new Vector2(this.Position), Radius, Ship.GetColor(faction)));
            }
            this.Faction = faction;
            this.NoProductionSteps = Game.CreationParameters.NoProductionSteps;
            this.IsSelected = false;
            this.Color1 = Color.Lerp(this.Color1, this.Color2, this.ColorFactor);
            this.Color2 = GetColor(faction);
            this.ColorFactor = 0f;
            this.ContinuousOrderRoute = null;
            foreach (Satellite entity in this.Satellites.ToArray<Satellite>())
            {
                Game.RemoveEntity(entity);
            }
        }

        public void Step()
        {
            if (this.NoProductionSteps <= 0)
            {
                Player player = PlayersModule.FindPlayer(this.Faction);
                int num;
                if (player != null)
                {
                    num = ((player.FactionProductionFactor > 0) ? (this.ProductionPeriod * 100 / player.FactionProductionFactor) : int.MaxValue);
                }
                else
                {
                    num = this.ProductionPeriod * 7;
                }
                if (this.ShipsCount < this.MaxShipsCount && StepModule.StepIndex % num == num - 1)
                {
                    int num2 = this.ShipsCount + 1;
                    this.ShipsCount = num2;
                }
            }
            else
            {
                int num2 = this.NoProductionSteps - 1;
                this.NoProductionSteps = num2;
            }
            if (this.ShipsCount > this.MaxShipsCount)
            {
                int num3 = 100 * (this.ShipsCount - this.MaxShipsCount) / this.MaxShipsCount;
                int num4;
                if (num3 < 150)
                {
                    num4 = 90;
                }
                else if (num3 < 250)
                {
                    num4 = 60;
                }
                else
                {
                    num4 = 40;
                }
                if (num4 > 0 && StepModule.StepIndex % num4 == 0)
                {
                    int num2 = this.ShipsCount - 1;
                    this.ShipsCount = num2;
                }
            }
            if (this.ContinuousOrderRoute != null && this.ShipsCount > 0)
            {
                HumanPlayer humanPlayer = PlayersModule.FindPlayer(this.Faction) as HumanPlayer;
                if (humanPlayer != null)
                {
                    int num5 = 120 + PlanetsModule.Planets.IndexOf(this);
                    if (StepModule.StepIndex % num5 == 0)
                    {
                        humanPlayer.AddOrderToOutgoingList(this, this.ContinuousOrderRoute, this.ShipsCount, false, Faction.None);
                    }
                }
            }
        }

        public void DrawPlanet(Color colorTransform)
        {
            Texture2D texture = this.IsSpecial ? Textures.Planet2 : Textures.Planet;
            TexturedBatch2D texturedBatch2D = CameraModule.PrimitivesRenderer.TexturedBatch(texture, false, 0, null, null, BlendState.AlphaBlend, null);
            float v = 1.7f * Radius;
            Color color = Color.Lerp(this.Color1, this.Color2, this.ColorFactor) * colorTransform;
            this.ColorFactor = MathUtils.Min(this.ColorFactor + 1f * Time.FrameDuration, 1f);
            texturedBatch2D.QueueQuad(new Vector2(this.Position) - new Vector2(v), new Vector2(this.Position) + new Vector2(v), 0f, Vector2.Zero, Vector2.One, color);
        }

        public void DrawOverlays(Color colorTransform)
        {
            FlatBatch2D flatBatch2D = CameraModule.PrimitivesRenderer.FlatBatch(1, null, null, null);
            FontBatch2D fontBatch2D = CameraModule.PrimitivesRenderer.FontBatch(Fonts.Normal, 2, null, null, BlendState.AlphaBlend, null);
            float num = Radius;
            Color color = Color.White * colorTransform;
            if (this.IsSelected)
            {
                float num2 = 90f;
                float num3 = 70f;
                flatBatch2D.QueueDisc(new Vector2(this.Position), new Vector2(num - num3), new Vector2(num - num2), 0f, color, Color.Transparent, 48, 0f, 6.28318548f);
                flatBatch2D.QueueDisc(new Vector2(this.Position), new Vector2(num + num3), new Vector2(num - num3), 0f, color, color, 48, 0f, 6.28318548f);
                flatBatch2D.QueueDisc(new Vector2(this.Position), new Vector2(num + num2), new Vector2(num + num3), 0f, Color.Transparent, color, 48, 0f, 6.28318548f);
            }
            else if (this.ShowHighlight)
            {
                float num4 = 30f;
                float num5 = 10f;
                flatBatch2D.QueueDisc(new Vector2(this.Position), new Vector2(num - num5), new Vector2(num - num4), 0f, color, Color.Transparent, 48, 0f, 6.28318548f);
                flatBatch2D.QueueDisc(new Vector2(this.Position), new Vector2(num + num5), new Vector2(num - num5), 0f, color, color, 48, 0f, 6.28318548f);
                flatBatch2D.QueueDisc(new Vector2(this.Position), new Vector2(num + num4), new Vector2(num + num5), 0f, Color.Transparent, color, 48, 0f, 6.28318548f);
            }
            else if (this.IsControllingPlayerPlanet)
            {
                if (PlayersModule.ControllingPlayer.BlinkPlanets)
                {
                    if (MathUtils.Remainder(Time.FrameStartTime * 2.0, 1.0) < 0.5)
                    {
                        float num6 = 50f;
                        float num7 = 30f;
                        flatBatch2D.QueueDisc(new Vector2(this.Position), new Vector2(num - num7), new Vector2(num - num6), 0f, color, Color.Transparent, 48, 0f, 6.28318548f);
                        flatBatch2D.QueueDisc(new Vector2(this.Position), new Vector2(num + num7), new Vector2(num - num7), 0f, color, color, 48, 0f, 6.28318548f);
                        flatBatch2D.QueueDisc(new Vector2(this.Position), new Vector2(num + num6), new Vector2(num + num7), 0f, Color.Transparent, color, 48, 0f, 6.28318548f);
                    }
                }
                else
                {
                    float num8 = 20f;
                    flatBatch2D.QueueDisc(new Vector2(this.Position), new Vector2(num - 0f), new Vector2(num - num8), 0f, color, Color.Transparent, 48, 0f, 6.28318548f);
                    flatBatch2D.QueueDisc(new Vector2(this.Position), new Vector2(num + num8), new Vector2(num + 0f), 0f, Color.Transparent, color, 48, 0f, 6.28318548f);
                }
            }
            if (this.IsControllingPlayerPlanet || this.Faction == Faction.Neutral)
            {
                Player player = PlayersModule.FindPlayer(this.Faction);
                Color color2;
                Color color3;
                if (player != null)
                {
                    float f = MathUtils.Saturate(MathUtils.Min(1f - NoProductionSteps * 0.05f, player.FactionProductionFactor / 100f));
                    color2 = Color.Lerp(Color.Lerp(Color.Gray, Color.White, f) * colorTransform, Color.Red, MathUtils.Saturate((this.ShipsCount - this.MaxShipsCount) / (float)this.MaxShipsCount));
                    color3 = Color.Black * colorTransform;
                }
                else
                {
                    color2 = Color.White;
                    color3 = Color.Black;
                }
                color2 *= colorTransform;
                color3 *= colorTransform;
                string text;
                if (!CountStrings.TryGetValue(this.ShipsCount, out text))
                {
                    text = this.ShipsCount.ToString();
                    CountStrings.Add(this.ShipsCount, text);
                }
                fontBatch2D.QueueText(text, new Vector2(this.Position) + new Vector2(20f), 0f, color3, TextAnchor.HorizontalCenter | TextAnchor.VerticalCenter, new Vector2(9.5f), new Vector2(0f, 0f), 0f);
                fontBatch2D.QueueText(text, new Vector2(this.Position), 0f, color2, TextAnchor.HorizontalCenter | TextAnchor.VerticalCenter, new Vector2(9.5f), new Vector2(0f, 0f), 0f);
            }
            if (this.ShowRangeDisc && Game.CreationParameters.ShipRange < 20000)
            {
                DrawingUtils.QueueRangeDisc(CameraModule.PrimitivesRenderer.FlatBatch(2, RangeFillDepthStencilState, null, null), new Vector2(this.Position), Game.CreationParameters.ShipRange, 0.5f, Color.White * 0.07f, GetColor(this.Faction) * 0.3f, false);
                DrawingUtils.QueueRangeDisc(CameraModule.PrimitivesRenderer.FlatBatch(3, RangeOutlineDepthStencilState, null, null), new Vector2(this.Position), Game.CreationParameters.ShipRange, 0.5f, Color.White * 0.07f, GetColor(this.Faction) * 0.3f, true);
            }
            if (this.ShowHighlight && this.IsControllingPlayerPlanet && Game.CreationParameters.ShipRange < 20000)
            {
                FlatBatch2D batch = CameraModule.PrimitivesRenderer.FlatBatch(-1, null, null, null);
                foreach (Planet planet in from p in PlanetsModule.Planets
                                          where ShipsModule.TestReachability(p.Position, this.Position)
                                          select p)
                {
                    if (!planet.IsControllingPlayerPlanet || PlanetsModule.Planets.IndexOf(planet) > PlanetsModule.Planets.IndexOf(this))
                    {
                        Vector2 p3 = new Vector2(this.Position);
                        Vector2 p2 = new Vector2(planet.Position);
                        DrawingUtils.QueueThickLine(batch, p3, p2, 0f, 20f, 0f, 20f, 0.3f * GetColor(this.Faction), Color.Transparent);
                    }
                }
            }
        }

        public static Color GetColor(Faction faction)
        {
            switch (faction)
            {
                case Faction.Faction1:
                    return new Color(230, 25, 75);
                case Faction.Faction2:
                    return new Color(245, 130, 48);
                case Faction.Faction3:
                    return new Color(255, 220, 0);
                case Faction.Faction4:
                    return new Color(60, 200, 75);
                case Faction.Faction5:
                    return new Color(0, 140, 255);
                case Faction.Faction6:
                    return new Color(240, 50, 255);
                default:
                    return Color.DarkGray;
            }
        }

        public static int GetRadius(int sizeClass)
        {
            switch (sizeClass)
            {
                case 0:
                    return 300;
                case 1:
                    return 400;
                case 2:
                    return 500;
                default:
                    throw new InvalidOperationException();
            }
        }

        private static Dictionary<int, string> CountStrings = new Dictionary<int, string>();

        private static DepthStencilState RangeFillDepthStencilState = new DepthStencilState
        {
            DepthBufferFunction = CompareFunction.NotEqual,
            DepthBufferTestEnable = true,
            DepthBufferWriteEnable = true
        };

        private static DepthStencilState RangeOutlineDepthStencilState = new DepthStencilState
        {
            DepthBufferFunction = CompareFunction.NotEqual,
            DepthBufferTestEnable = true,
            DepthBufferWriteEnable = false
        };

        private Color Color1;

        private Color Color2;

        private float ColorFactor;

        private const int MaxSatellites = 2;

        public object AStarStorage;
    }
}
