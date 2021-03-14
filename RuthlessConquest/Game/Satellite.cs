// Decompiled with JetBrains decompiler
// Type: Game.Satellite
// Assembly: RuthlessConquest, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 09ABF203-5B7E-4C78-ACFB-2EE5FE9ADF6E
// Assembly location: d:\Users\12464\Desktop\Ruthless Conquest\RuthlessConquest.exe

using Engine;
using Engine.Graphics;
using Engine.Serialization;
using System;
using System.Linq;

namespace Game
{
    internal class Satellite : Entity
    {
        private float Rotation;
        private int TargetAltitude;
        private int Index;
        private DynamicArray<Satellite.Laser> Lasers = new DynamicArray<Satellite.Laser>();
        private DynamicArray<Body> TmpBodies = new DynamicArray<Body>();
        private DynamicArray<Planet> TmpPlanets = new DynamicArray<Planet>();
        private DynamicArray<Ship> TmpShips = new DynamicArray<Ship>();

        public Planet Planet { get; private set; }

        public Point2 Position { get; private set; }

        public int Altitude { get; private set; }

        public bool IsEnabled
        {
            get
            {
                if (this.Planet == null)
                    return false;
                Player player = this.Planet.Game.PlayersModule.FindPlayer(this.Planet.Faction);
                return player == null || player.AreSatellitesEnabled;
            }
        }

        private Satellite() => this.TargetAltitude = 190;

        public Satellite(Planet planet, int altitude)
          : this()
        {
            this.Index = planet.Satellites.Count<Satellite>();
            this.Planet = planet;
            this.Altitude = altitude >= 0 ? altitude : this.TargetAltitude;
            if (planet.Satellites.Count<Satellite>() > 0)
            {
                Point2 point2 = planet.Satellites.First<Satellite>().Position - planet.Position;
                this.Position = planet.Position - point2;
            }
            else
                this.Position = planet.Position + new Point2(planet.Radius + this.Altitude, 0);
        }

        public override void Serialize(InputArchive archive)
        {
            this.Planet = archive.Serialize<Planet>("Planet");
            this.Position = archive.Serialize<Point2>("Position");
            this.Altitude = archive.Serialize<int>("Altitude");
            this.Index = archive.Serialize<int>("Index");
        }

        public override void Serialize(OutputArchive archive)
        {
            archive.Serialize<Planet>("Planet", this.Planet);
            archive.Serialize<Point2>("Position", this.Position);
            archive.Serialize("Altitude", this.Altitude);
            archive.Serialize("Index", this.Index);
        }

        public void Step()
        {
            if (this.Planet == null)
                return;
            int num1 = 40 + this.PlanetsModule.Planets.IndexOf(this.Planet) % 11;
            if ((int)this.Planet.Faction % 2 == 0)
                num1 = -num1;
            Point2 point2 = this.Position - this.Planet.Position;
            this.Position += new Point2(point2.Y, -point2.X) * num1 / 1000;
            this.Rotation += (float)(0.0700000002980232 * num1 * 0.0166666675359011);
            if (this.Altitude < this.TargetAltitude)
                this.Altitude += MathUtils.Max((this.TargetAltitude - this.Altitude) / 20, 1);
            else if (this.Altitude > this.TargetAltitude)
                this.Altitude -= MathUtils.Max((this.Altitude - this.TargetAltitude) / 20, 1);
            this.ClampToOrbit();
            if (!this.IsEnabled)
                return;
            int num2 = 33;
            int num3 = (MathUtils.Hash(this.PlanetsModule.Planets.IndexOf(this.Planet) + this.Index) % num2 + num2) % num2;
            if (this.StepModule.StepIndex % num2 != num3)
                return;
            this.Fire();
        }

        public void Draw(Color colorTransform)
        {
            if (this.Planet == null)
                return;
            FlatBatch2D batch = this.CameraModule.PrimitivesRenderer.FlatBatch();
            foreach (Satellite.Laser laser in this.Lasers)
            {
                Color ci1 = Color.White * MathUtils.Saturate(3.5f * laser.TimeToLive) * 1f;
                Color ci2 = Color.White * MathUtils.Saturate(3.5f * laser.TimeToLive) * 0.5f;
                DrawingUtils.QueueThickLine(batch, new Vector2(this.Position), new Vector2(laser.TargetPosition), 0.0f, 30f, 0.0f, 20f, ci1, Color.Transparent, ci2, Color.Transparent);
                laser.TimeToLive -= Time.FrameDuration;
            }
            if (this.Lasers.Count > 0)
                this.Lasers.RemoveAll(l => l.TimeToLive <= 0.0);
            Color color1 = this.IsEnabled ? Planet.GetColor(this.Planet.Faction) : Planet.GetColor(Faction.Neutral);
            Color color2 = Color.MultiplyColorOnly(color1, 0.7f);
            batch.QueueDisc(new Vector2(this.Position), new Vector2(35f), new Vector2(25f), 0.0f, color2, Color.Transparent, 5, this.Rotation, this.Rotation + 6.283185f);
            batch.QueueDisc(new Vector2(this.Position), new Vector2(65f), new Vector2(35f), 0.0f, color2, color2, 5, this.Rotation, this.Rotation + 6.283185f);
            batch.QueueDisc(new Vector2(this.Position), new Vector2(85f), new Vector2(65f), 0.0f, color1, color1, 5, this.Rotation, this.Rotation + 6.283185f);
            batch.QueueDisc(new Vector2(this.Position), new Vector2(95f), new Vector2(85f), 0.0f, Color.Transparent, color1, 5, this.Rotation, this.Rotation + 6.283185f);
        }

        private void Fire()
        {
            if (this.Planet == null)
                return;
            int num1 = 1750;
            int radius = 250;
            int num2 = 250;
            DynamicArray<Ship> source = new DynamicArray<Ship>();
            for (int dx = -num1; dx <= num1; dx += num2)
            {
                for (int dy = -num1; dy <= num1; dy += num2)
                {
                    if (IntMath.LengthSquared(dx, dy) <= num1 * num1)
                    {
                        int num3 = this.Position.X + dx;
                        int num4 = this.Position.Y + dy;
                        this.TmpBodies.Clear();
                        this.BodiesModule.QueryBodies((num3, num4), radius, this.TmpBodies, int.MaxValue);
                        if (this.TmpBodies.Count > 0)
                        {
                            this.TmpPlanets.Clear();
                            foreach (Body tmpBody in this.TmpBodies)
                            {
                                if (tmpBody.Tag is Planet tag)
                                    this.TmpPlanets.Add(tag);
                            }
                            this.TmpShips.Clear();
                            foreach (Body tmpBody in this.TmpBodies)
                            {
                                if (tmpBody.Tag is Ship tag && tag.Faction != this.Planet.Faction && tag.GiftToFaction != this.Planet.Faction && ((this.Planet.Faction != Faction.Neutral || tag.Route.Contains(this.Planet)) && (tag.Faction != Faction.Neutral || tag.Route.Any<Planet>(p => p.Faction == this.Planet.Faction))))
                                {
                                    bool flag = false;
                                    foreach (Planet tmpPlanet in this.TmpPlanets)
                                    {
                                        if (IntMath.ApproxDistance(tag.Position, tmpPlanet.Position) <= tag.Body.GridRadius + tmpPlanet.Radius)
                                        {
                                            flag = true;
                                            break;
                                        }
                                    }
                                    if (!flag)
                                        this.TmpShips.Add(tag);
                                }
                            }
                            if (this.TmpShips.Count > 0 && this.TmpShips.Count > source.Count)
                            {
                                source.Clear();
                                source.AddRange(this.TmpShips);
                            }
                        }
                    }
                }
            }
            if (source.Count <= 0)
                return;
            Point2 zero = Point2.Zero;
            foreach (Ship ship in source)
                zero += ship.Position;
            Point2 p2 = zero / source.Count;
            Point2 position = source.First<Ship>().Position;
            foreach (Ship ship in source)
            {
                if (IntMath.DistanceSquared(ship.Position, p2) < IntMath.DistanceSquared(position, p2))
                    position = ship.Position;
            }
            foreach (Ship ship in source)
            {
                int num3 = IntMath.ApproxDistance(position, ship.Position);
                int num4 = 100 * (radius - num3) / radius;
                if (this.StepModule.Random.Int(100) < num4)
                    ship.RemoveShip(true, 1000f * Vector2.Normalize(new Vector2(ship.Position - position)));
            }
            this.Lasers.Add(new Satellite.Laser()
            {
                TargetPosition = position
            });
            Vector2 vector2 = 1000f * Vector2.Normalize(new Vector2(position) - new Vector2(this.Position));
            AudioManager.PlaySound(Sounds.Laser, true, 0.4f);
        }

        private void ClampToOrbit()
        {
            if (this.Planet == null)
                return;
            int num1 = this.Planet.Radius + this.Altitude;
            if (num1 <= 0)
                return;
            Point2 p = this.Position - this.Planet.Position;
            int num2 = 1000 * IntMath.Length(p) / num1;
            if (num2 <= 0)
                return;
            this.Position = this.Planet.Position + p * 1000 / num2;
        }

        private class Laser
        {
            public Point2 TargetPosition;
            public float TimeToLive = 0.45f;
        }
    }
}


/*using System;
using System.Linq;
using Engine;
using Engine.Graphics;
using Engine.Serialization;

namespace Game
{
    internal class Satellite : Entity
    {
        public Planet Planet { get; private set; }

        public Point2 Position { get; private set; }

        public int Altitude { get; private set; }

        public bool IsEnabled
        {
            get
            {
                if (this.Planet != null)
                {
                    Player player = this.Planet.Game.PlayersModule.FindPlayer(this.Planet.Faction);
                    return player == null || player.AreSatellitesEnabled;
                }
                return false;
            }
        }

        private Satellite()
        {
            this.TargetAltitude = 190;
        }

        public Satellite(Planet planet, int altitude) : this()
        {
            this.Index = planet.Satellites.Count<Satellite>();
            this.Planet = planet;
            this.Altitude = ((altitude >= 0) ? altitude : this.TargetAltitude);
            if (planet.Satellites.Count<Satellite>() > 0)
            {
                Point2 p = planet.Satellites.First<Satellite>().Position - planet.Position;
                this.Position = planet.Position - p;
                return;
            }
            this.Position = planet.Position + new Point2(planet.Radius + this.Altitude, 0);
        }

        public override void Serialize(InputArchive archive)
        {
            this.Planet = archive.Serialize<Planet>("Planet");
            this.Position = archive.Serialize<Point2>("Position");
            this.Altitude = archive.Serialize<int>("Altitude");
            this.Index = archive.Serialize<int>("Index");
        }

        public override void Serialize(OutputArchive archive)
        {
            archive.Serialize<Planet>("Planet", this.Planet);
            archive.Serialize<Point2>("Position", this.Position);
            archive.Serialize("Altitude", this.Altitude);
            archive.Serialize("Index", this.Index);
        }

        public void Step()
        {
            if (this.Planet == null)
            {
                return;
            }
            int num = 40 + base.PlanetsModule.Planets.IndexOf(this.Planet) % 11;
            if (this.Planet.Faction % Faction.Faction3 == Faction.Faction1)
            {
                num = -num;
            }
            Point2 point = this.Position - this.Planet.Position;
            this.Position += new Point2(point.Y, -point.X) * num / 1000;
            this.Rotation += 0.07f * (float)num * 0.0166666675f;
            if (this.Altitude < this.TargetAltitude)
            {
                this.Altitude += MathUtils.Max((this.TargetAltitude - this.Altitude) / 20, 1);
            }
            else if (this.Altitude > this.TargetAltitude)
            {
                this.Altitude -= MathUtils.Max((this.Altitude - this.TargetAltitude) / 20, 1);
            }
            this.ClampToOrbit();
            if (this.IsEnabled)
            {
                int num2 = 33;
                int num3 = (MathUtils.Hash(base.PlanetsModule.Planets.IndexOf(this.Planet) + this.Index) % num2 + num2) % num2;
                if (base.StepModule.StepIndex % num2 == num3)
                {
                    this.Fire();
                }
            }
        }

        public void Draw(Color colorTransform)
        {
            if (this.Planet == null)
            {
                return;
            }
            FlatBatch2D flatBatch2D = base.CameraModule.PrimitivesRenderer.FlatBatch(0, null, null, null);
            foreach (Satellite.Laser laser in this.Lasers)
            {
                Color ci = Color.White * MathUtils.Saturate(3.5f * laser.TimeToLive) * 1f;
                Color ci2 = Color.White * MathUtils.Saturate(3.5f * laser.TimeToLive) * 0.5f;
                DrawingUtils.QueueThickLine(flatBatch2D, new Vector2(this.Position), new Vector2(laser.TargetPosition), 0f, 30f, 0f, 20f, ci, Color.Transparent, ci2, Color.Transparent);
                laser.TimeToLive -= Time.FrameDuration;
            }
            if (this.Lasers.Count > 0)
            {
                this.Lasers.RemoveAll((Satellite.Laser l) => l.TimeToLive <= 0f);
            }
            Color color = this.IsEnabled ? Planet.GetColor(this.Planet.Faction) : Planet.GetColor(Faction.Neutral);
            Color color2 = Color.MultiplyColorOnly(color, 0.7f);
            flatBatch2D.QueueDisc(new Vector2(this.Position), new Vector2(35f), new Vector2(25f), 0f, color2, Color.Transparent, 5, this.Rotation, this.Rotation + 6.28318548f);
            flatBatch2D.QueueDisc(new Vector2(this.Position), new Vector2(65f), new Vector2(35f), 0f, color2, color2, 5, this.Rotation, this.Rotation + 6.28318548f);
            flatBatch2D.QueueDisc(new Vector2(this.Position), new Vector2(85f), new Vector2(65f), 0f, color, color, 5, this.Rotation, this.Rotation + 6.28318548f);
            flatBatch2D.QueueDisc(new Vector2(this.Position), new Vector2(95f), new Vector2(85f), 0f, Color.Transparent, color, 5, this.Rotation, this.Rotation + 6.28318548f);
        }

        private void Fire()
        {
            if (this.Planet == null)
            {
                return;
            }
            int num = 1750;
            int num2 = 250;
            int num3 = 250;
            DynamicArray<Ship> dynamicArray = new DynamicArray<Ship>();
            for (int i = -num; i <= num; i += num3)
            {
                for (int j = -num; j <= num; j += num3)
                {
                    if (IntMath.LengthSquared(i, j) <= num * num)
                    {
                        int item = this.Position.X + i;
                        int item2 = this.Position.Y + j;
                        this.TmpBodies.Clear();
                        base.BodiesModule.QueryBodies(new Point2(item, item2), num2, this.TmpBodies, int.MaxValue);
                        if (this.TmpBodies.Count > 0)
                        {
                            this.TmpPlanets.Clear();
                            foreach (Body body in this.TmpBodies)
                            {
                                Planet planet = body.Tag as Planet;
                                if (planet != null)
                                {
                                    this.TmpPlanets.Add(planet);
                                }
                            }
                            this.TmpShips.Clear();
                            foreach (Body body2 in this.TmpBodies)
                            {
                                Ship ship = body2.Tag as Ship;
                                if (ship != null && ship.Faction != this.Planet.Faction && ship.GiftToFaction != this.Planet.Faction && (this.Planet.Faction != Faction.Neutral || ship.Route.Contains(this.Planet)) && (ship.Faction != Faction.Neutral || ship.Route.Any((Planet p) => p.Faction == this.Planet.Faction)))
                                {
                                    bool flag = false;
                                    foreach (Planet planet2 in this.TmpPlanets)
                                    {
                                        if (IntMath.ApproxDistance(ship.Position, planet2.Position) <= ship.Body.GridRadius + planet2.Radius)
                                        {
                                            flag = true;
                                            break;
                                        }
                                    }
                                    if (!flag)
                                    {
                                        this.TmpShips.Add(ship);
                                    }
                                }
                            }
                            if (this.TmpShips.Count > 0 && this.TmpShips.Count > dynamicArray.Count)
                            {
                                dynamicArray.Clear();
                                dynamicArray.AddRange(this.TmpShips);
                            }
                        }
                    }
                }
            }
            if (dynamicArray.Count > 0)
            {
                Point2 point = Point2.Zero;
                foreach (Ship ship2 in dynamicArray)
                {
                    point += ship2.Position;
                }
                point /= dynamicArray.Count;
                Point2 position = dynamicArray.First<Ship>().Position;
                foreach (Ship ship3 in dynamicArray)
                {
                    if (IntMath.DistanceSquared(ship3.Position, point) < IntMath.DistanceSquared(position, point))
                    {
                        position = ship3.Position;
                    }
                }
                foreach (Ship ship4 in dynamicArray)
                {
                    int num4 = IntMath.ApproxDistance(position, ship4.Position);
                    int num5 = 100 * (num2 - num4) / num2;
                    if (base.StepModule.Random.Int(100) < num5)
                    {
                        ship4.RemoveShip(true, 1000f * Vector2.Normalize(new Vector2(ship4.Position - position)));
                    }
                }
                this.Lasers.Add(new Satellite.Laser
                {
                    TargetPosition = position
                });
                1000f * Vector2.Normalize(new Vector2(position) - new Vector2(this.Position));
                AudioManager.PlaySound(Sounds.Laser, true, 0.4f, 1f, 0f);
            }
        }

        private void ClampToOrbit()
        {
            if (this.Planet != null)
            {
                int num = this.Planet.Radius + this.Altitude;
                if (num > 0)
                {
                    Point2 point = this.Position - this.Planet.Position;
                    int num2 = IntMath.Length(point);
                    int num3 = 1000 * num2 / num;
                    if (num3 > 0)
                    {
                        point = point * 1000 / num3;
                        this.Position = this.Planet.Position + point;
                    }
                }
            }
        }

        private float Rotation;

        private int TargetAltitude;

        private int Index;

        private DynamicArray<Satellite.Laser> Lasers = new DynamicArray<Satellite.Laser>();

        private DynamicArray<Body> TmpBodies = new DynamicArray<Body>();

        private DynamicArray<Planet> TmpPlanets = new DynamicArray<Planet>();

        private DynamicArray<Ship> TmpShips = new DynamicArray<Ship>();

        private class Laser
        {
            public Point2 TargetPosition;

            public float TimeToLive = 0.45f;
        }
    }
}
*/