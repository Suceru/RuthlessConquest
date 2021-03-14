using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Graphics;
using Engine.Serialization;

namespace Game
{
    internal class Ship : Entity
    {
        private Ship()
        {
            this.Body = new Body();
            this.Body.Tag = this;
        }

        public Ship(Faction faction, Faction giftToFaction, Point2 position, Planet source, IEnumerable<Planet> route) : this()
        {
            this.Faction = faction;
            this.GiftToFaction = giftToFaction;
            this.Position = position;
            this.Source = source;
            this.Route.AddRange(route);
        }

        public override void Serialize(InputArchive archive)
        {
            archive.Serialize<Body>("Body", this.Body);
            this.Velocity = archive.Serialize<Point2>("Velocity");
            this.Faction = archive.Serialize<Faction>("Faction");
            this.GiftToFaction = archive.Serialize<Faction>("GiftToFaction");
            this.Source = archive.Serialize<Planet>("Source");
            this.RouteIndex = archive.Serialize<int>("RouteIndex");
            archive.SerializeCollection<Planet>("Route", this.Route);
        }

        public override void Serialize(OutputArchive archive)
        {
            archive.Serialize<Body>("Body", this.Body);
            archive.Serialize<Point2>("Velocity", this.Velocity);
            archive.Serialize<Faction>("Faction", this.Faction);
            archive.Serialize<Faction>("GiftToFaction", this.GiftToFaction);
            archive.Serialize<Planet>("Source", this.Source);
            archive.Serialize("RouteIndex", this.RouteIndex);
            archive.SerializeCollection<Planet>("Route", "Planet", this.Route);
        }

        protected internal override void OnAdded()
        {
            this.Body.GridRadius = 90;
            this.Body.RepelFactor = Game.CreationParameters.ShipRepelFactor;
        }

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

        public Point2 Velocity { get; private set; }

        public Faction Faction { get; private set; }

        public Faction GiftToFaction { get; private set; }

        public Planet Source { get; private set; }

        public DynamicArray<Planet> Route { get; private set; } = new DynamicArray<Planet>();

        public void Step()
        {
            Point2 point = Point2.Zero;
            Planet planet = (this.Route.Count > this.RouteIndex) ? this.Route[this.RouteIndex] : null;
            Planet planet2 = (this.Route.Count > 0) ? this.Route[this.Route.Count - 1] : null;
            TmpBodyList.Clear();
            BodiesModule.QueryBodies(this.Position, this.Body.GridRadius, TmpBodyList, 10);
            foreach (Body body in TmpBodyList)
            {
                if (body != this.Body && body.Tag != this.Source && body.Tag != planet2)
                {
                    if (body.Tag == planet)
                    {
                        Planet planet3 = body.Tag as Planet;
                        if (planet3 != null && planet3.Faction != this.Faction)
                        {
                            continue;
                        }
                    }
                    Ship ship = body.Tag as Ship;
                    if (ship != null && ship.Faction != this.Faction && IntMath.ApproxDistance(this.Position, ship.Position) < 2 * this.Body.GridRadius)
                    {
                        this.RemoveShip(true, new Vector2(this.Velocity));
                        ship.RemoveShip(true, new Vector2(ship.Velocity));
                        return;
                    }
                    int num = body.GridRadius * 6 / 10;
                    int gridRadius = body.GridRadius;
                    Point2 point2 = body.Position - this.Position;
                    int num2 = IntMath.ApproxLength(point2);
                    int n = MathUtils.Clamp(-100 * (num2 - this.Body.GridRadius - gridRadius) / (gridRadius - num), 0, 100);
                    if (num2 > 0)
                    {
                        point -= n * point2 / num2 * body.RepelFactor / 32;
                    }
                    if (body.Tag is Planet)
                    {
                        Point2 point3 = (planet != null) ? (planet.Position - this.Position) : Point2.Zero;
                        if (point3.X * point2.X + point3.Y * point2.Y >= 0)
                        {
                            Point2 p = (point3.X * point2.Y - point3.Y * point2.X > 0) ? (-new Point2(point3.Y, -point3.X)) : new Point2(point3.Y, -point3.X);
                            int num3 = IntMath.ApproxLength(p);
                            if (num3 > 0)
                            {
                                point -= n * p / num3 * body.RepelFactor / 20;
                            }
                        }
                    }
                }
            }
            if (planet != null)
            {
                Point2 p2 = planet.Position - this.Position;
                int num4 = IntMath.ApproxLength(p2);
                if (planet == planet2 || planet.Faction != this.Faction)
                {
                    int num5 = planet.Radius + 20;
                    if (num4 <= num5)
                    {
                        this.EnterPlanet(planet);
                        return;
                    }
                }
                else
                {
                    int num6 = planet.Radius + 50;
                    if (num4 <= 2 * num6)
                    {
                        this.RouteIndex++;
                    }
                }
                if (num4 > 0)
                {
                    point += 100 * p2 / num4;
                }
            }
            int n2 = 800;
            this.Velocity += point * n2 / 1024;
            int num7 = IntMath.ApproxLength(this.Velocity);
            if (num7 > Game.CreationParameters.ShipSpeed)
            {
                this.Velocity = this.Velocity * Game.CreationParameters.ShipSpeed / num7;
            }
            this.Position += this.Velocity * 17 / 1024;
        }

        public void Draw(Color colorTransform)
        {
            TexturedBatch2D texturedBatch2D = CameraModule.PrimitivesRenderer.TexturedBatch(GetTexture(this.Faction), false, 0, null, null, BlendState.AlphaBlend, null);
            Vector2 v = new Vector2(this.Position);
            Vector2 vector = Vector2.Normalize(new Vector2(this.Velocity)) * 100f;
            Vector2 v2 = Vector2.Perpendicular(vector);
            Vector2 vector2 = v - v2 + vector;
            Vector2 p = v + v2 + vector;
            Vector2 vector3 = v + v2 - vector;
            Vector2 p2 = v - v2 - vector;
            Color color = this.GetColor() * colorTransform;
            texturedBatch2D.QueueTriangle(vector2, p, vector3, 0f, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), color);
            texturedBatch2D.QueueTriangle(vector3, p2, vector2, 0f, new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(0f, 0f), color);
        }

        public void RemoveShip(bool explode, Vector2 velocity)
        {
            if (Game != null)
            {
                if (explode)
                {
                    if (ParticlesModule.ParticleSystems.Count((ParticleSystemBase p) => p is ExplosionParticleSystem) < 75)
                    {
                        ParticlesModule.AddParticleSystem(new ExplosionParticleSystem(new Vector2(this.Position), velocity, 1f, 1f, this.GetColor()));
                    }
                }
                Game.RemoveEntity(this);
            }
        }

        public static string GetShipRangeName(ShipRange shipRange)
        {
            if (shipRange <= ShipRange.Medium)
            {
                if (shipRange == ShipRange.Tiny)
                {
                    return "Tiny";
                }
                if (shipRange == ShipRange.Short)
                {
                    return "Short";
                }
                if (shipRange == ShipRange.Medium)
                {
                    return "Medium";
                }
            }
            else
            {
                if (shipRange == ShipRange.Long)
                {
                    return "Long";
                }
                if (shipRange == ShipRange.VeryLong)
                {
                    return "Very Long";
                }
                if (shipRange == ShipRange.Unlimited)
                {
                    return "Unlimited";
                }
            }
            int num = (int)shipRange;
            return num.ToString();
        }

        public static Color GetColor(Faction faction)
        {
            if (faction == Faction.Neutral)
            {
                return Color.White;
            }
            return Planet.GetColor(faction);
        }

        public static Texture2D GetTexture(Faction faction)
        {
            switch (faction)
            {
                case Faction.Faction1:
                    return Textures.Ship1;
                case Faction.Faction2:
                    return Textures.Ship2;
                case Faction.Faction3:
                    return Textures.Ship3;
                case Faction.Faction4:
                    return Textures.Ship4;
                case Faction.Faction5:
                    return Textures.Ship5;
                case Faction.Faction6:
                    return Textures.Ship6;
                case Faction.Neutral:
                    return Textures.Ship7;
                default:
                    return Textures.Ship1;
            }
        }

        private Color GetColor()
        {
            if (this.GiftToFaction != Faction.None)
            {
                return GetColor(this.GiftToFaction);
            }
            return GetColor(this.Faction);
        }

        private void EnterPlanet(Planet planet)
        {
            if (planet.Faction == this.Faction || planet.Faction == this.GiftToFaction)
            {
                planet.ShipsCount = MathUtils.Min(planet.ShipsCount + 1, 999);
                this.RemoveShip(false, Vector2.Zero);
                return;
            }
            if (planet.ShipsCount >= 1)
            {
                if (StepModule.Random.Int(100) < planet.DefenceFactor)
                {
                    int shipsCount = planet.ShipsCount - 1;
                    planet.ShipsCount = shipsCount;
                }
                this.RemoveShip(true, new Vector2(-this.Velocity));
                return;
            }
            planet.ChangeOwner((this.GiftToFaction == Faction.None) ? this.Faction : this.GiftToFaction);
            this.RemoveShip(false, Vector2.Zero);
        }

        private static Engine.Random Random = new Engine.Random(0);

        private static DynamicArray<Body> TmpBodyList = new DynamicArray<Body>();

        private int RouteIndex;
    }
}
