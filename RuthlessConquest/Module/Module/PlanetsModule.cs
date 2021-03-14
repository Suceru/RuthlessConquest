// Decompiled with JetBrains decompiler
// Type: Game.PlanetsModule
// Assembly: RuthlessConquest, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 09ABF203-5B7E-4C78-ACFB-2EE5FE9ADF6E
// Assembly location: d:\Users\12464\Desktop\Ruthless Conquest\RuthlessConquest.exe

using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    internal class PlanetsModule : Module
    {
        private DynamicArray<Planet> InternalPlanets = new DynamicArray<Planet>();
        private DynamicArray<Satellite> InternalSatellites = new DynamicArray<Satellite>();
        private DynamicArray<Planet> TmpPlanetsList = new DynamicArray<Planet>();

        public ReadOnlyList<Planet> Planets => new ReadOnlyList<Planet>(InternalPlanets);

        public ReadOnlyList<Satellite> Satellites => new ReadOnlyList<Satellite>(InternalSatellites);

        public PlanetsModule(Game game)
          : base(game)
        {
            game.EntityAdded += e =>
            {
                switch (e)
                {
                    case Planet planet:
                        this.InternalPlanets.Add(planet);
                        break;
                    case Satellite satellite:
                        this.InternalSatellites.Add(satellite);
                        break;
                }
            };
            game.EntityRemoved += e =>
            {
                switch (e)
                {
                    case Planet planet:
                        this.InternalPlanets.Remove(planet);
                        break;
                    case Satellite satellite:
                        this.InternalSatellites.Remove(satellite);
                        break;
                }
            };
        }

        public Planet PickPlanet(Vector2 screenPoint)
        {
            Vector2 v1 = Vector2.Transform(screenPoint, this.CameraModule.ScreenToWorldMatrix);
            foreach (Planet planet in this.Planets)
            {
                if (Vector2.Distance(v1, new Vector2(planet.Position)) <= planet.Radius * 1.39999997615814)
                    return planet;
            }
            return null;
        }

        public void Step()
        {
            foreach (Planet planet in this.Planets)
                planet.Step();
            foreach (Satellite satellite in this.Satellites)
                satellite.Step();
        }

        public void Draw(Color colorTransform)
        {
            foreach (Planet planet in this.Planets)
            {
                planet.ShowRangeDisc = false;
                planet.ShowHighlight = false;
            }
            this.TmpPlanetsList.Clear();
            this.TmpPlanetsList.AddRange(this.Planets.Where<Planet>(p => p.IsSelected));
            this.FindReachablePlanetsSet(this.TmpPlanetsList);
            foreach (Planet tmpPlanets in this.TmpPlanetsList)
            {
                tmpPlanets.ShowHighlight = true;
                if (tmpPlanets.Faction == this.PlayersModule.ControllingPlayer.Faction)
                    tmpPlanets.ShowRangeDisc = true;
            }
            foreach (Planet planet in this.Planets)
                planet.DrawPlanet(colorTransform);
            foreach (Satellite satellite in this.Satellites)
                satellite.Draw(colorTransform);
            foreach (Planet planet in this.Planets)
                planet.DrawOverlays(colorTransform);
            this.CameraModule.PrimitivesRenderer.Flush(this.CameraModule.WorldToScreenMatrix * PrimitivesRenderer2D.ViewportMatrix());
        }

        public void FindReachablePlanetsSet(DynamicArray<Planet> set)
        {
            if (this.PlayersModule.ControllingPlayer == null)
                return;
            for (int index = 0; index < set.Count; ++index)
            {
                Planet planet1 = set[index];
                if (planet1.Faction == this.PlayersModule.ControllingPlayer.Faction)
                {
                    foreach (Planet planet2 in this.Planets)
                    {
                        if (this.ShipsModule.TestReachability(planet2.Position, planet1.Position) && !set.Contains(planet2))
                            set.Add(planet2);
                    }
                }
            }
        }

        public int GetFactionPlanetsCount(Faction faction) => this.Planets.Where<Planet>(p => p.Faction == faction).Count<Planet>();

        public int GetFactionProductionPercentage(Faction faction)
        {
            int num1 = 0;
            int num2 = 0;
            foreach (Planet planet in this.Planets)
            {
                if (planet.ProductionPeriod > 0)
                {
                    if (planet.Faction == faction)
                        num1 += 10000 / planet.ProductionPeriod;
                    num2 += 10000 / planet.ProductionPeriod;
                }
            }
            return num2 <= 0 ? 0 : 100 * num1 / num2;
        }

        public void CreatePlanets()
        {
            Engine.Random random = new Engine.Random(MathUtils.Hash(this.Game.CreationParameters.Seed));
            Point2 worldSize = Game.WorldSize;
            DynamicArray<Point2> shifts = new DynamicArray<Point2>();
            for (int index1 = -1000; index1 <= 1000; index1 += 100)
            {
                for (int index2 = -1000; index2 <= 1000; index2 += 100)
                    shifts.Add(new Point2(index1, index2));
            }
            shifts = shifts.OrderBy<Point2, long>(s => 1000L * IntMath.LengthSquared(s) + shifts.IndexOf(s)).ToDynamicArray<Point2>();
            DynamicArray<Rectangle> forbiddenUISpaces = new DynamicArray<Rectangle>()
      {
        new Rectangle (0, 0, worldSize.X, worldSize.Y / 12),
         new Rectangle (0, worldSize.Y * 15 / 16, worldSize.X, worldSize.Y / 16)
      };
            int testR = 550;
            DynamicArray<Point2> source1 = new DynamicArray<Point2>();
            DynamicArray<Rectangle> source2 = new DynamicArray<Rectangle>();
            for (int index1 = 0; index1 < 1000; ++index1)
            {
                source2.Clear();
                bool flag1 = index1 >= 900;
                int num1 = flag1 ? 0 : random.Int(1, 2);
                for (int index2 = 0; index2 < num1; ++index2)
                    source2.Add(new Rectangle(worldSize.X * random.Int(0, 70) / 100, worldSize.Y * random.Int(0, 70) / 100, worldSize.X * random.Int(25, 32) / 100, worldSize.Y * random.Int(25, 32) / 100));
                int num2 = flag1 ? 0 : 5;
                for (int index2 = 0; index2 < num2; ++index2)
                    source2.Add(new Rectangle(worldSize.X * random.Int(0, 80) / 100, worldSize.Y * random.Int(0, 80) / 100, worldSize.X * random.Int(12, 22) / 100, worldSize.Y * random.Int(12, 22) / 100));
                int num3 = source2.Any<Rectangle>(r => r.Intersection(new Rectangle(0, 0, 3200, 1800))) ? 1 : 0;
                bool flag2 = source2.Any<Rectangle>(r => r.Intersection(new Rectangle(worldSize.X - 3200, 0, 3200, 1800)));
                bool flag3 = source2.Any<Rectangle>(r => r.Intersection(new Rectangle(worldSize.X - 3200, worldSize.Y - 1800, 3200, 1800)));
                bool flag4 = source2.Any<Rectangle>(r => r.Intersection(new Rectangle(0, worldSize.Y - 1800, 3200, 1800)));
                if (num3 == 0 && !flag3 || !flag2 && !flag4)
                    break;
            }
            for (int index1 = 0; index1 < this.Game.CreationParameters.PlanetsCount; ++index1)
            {
                for (int index2 = 0; index2 < 10000; ++index2)
                {
                    int x1 = MathUtils.Max(7000 - index2, 1300);
                    int num1 = this.Game.CreationParameters.ShipRange * 99 / 100;
                    int num2 = MathUtils.Min(x1, num1 - 300);
                    bool flag = index2 > 9500;
                    Point2 position = new Point2(random.Int(testR, worldSize.X - testR), random.Int(testR, worldSize.Y - testR));
                    int num3 = source1.Count > 0 ? source1.Min<Point2>(p => IntMath.ApproxDistance(p, position)) : 0;
                    if ((source1.Count == 0 || num3 >= num2 && num3 <= num1) && (!forbiddenUISpaces.Any<Rectangle>(r => r.Intersection(new Rectangle(position.X - testR, position.Y - testR, 2 * testR, 2 * testR))) && (flag || !source2.Any<Rectangle>(r => r.Intersection(new Rectangle(position.X - testR, position.Y - testR, 2 * testR, 2 * testR))))))
                    {
                        source1.Add(position);
                        break;
                    }
                }
            }
            foreach (Point2 point2_1 in shifts)
            {
                int val1_1 = int.MaxValue;
                int val1_2 = int.MaxValue;
                int val1_3 = int.MinValue;
                int val1_4 = int.MinValue;
                foreach (Point2 point2_2 in source1)
                {
                    val1_1 = Math.Min(val1_1, point2_2.X);
                    val1_2 = Math.Min(val1_2, point2_2.Y);
                    val1_3 = Math.Max(val1_3, point2_2.X);
                    val1_4 = Math.Max(val1_4, point2_2.Y);
                }
                Point2 point2_3 = new Point2(val1_1 + val1_3, val1_2 + val1_4) / 2;
                Point2 actualShift = worldSize / 2 - point2_3 + point2_1;
                IEnumerable<Point2> source3 = source1.Select<Point2, Point2>(p => p + actualShift);
                if (!source3.Any<Point2>(p => forbiddenUISpaces.Any<Rectangle>(r => r.Intersection(new Rectangle(p.X - testR, p.Y - testR, 2 * testR, 2 * testR)))))
                {
                    source1 = source3.ToDynamicArray<Point2>();
                    break;
                }
            }
            DynamicArray<Point2> playerPositions = new DynamicArray<Point2>();
            playerPositions.Add(source1.OrderByDescending<Point2, int>(p => IntMath.ApproxDistance(p, worldSize / 2)).ToArray<Point2>()[random.Int(0, source1.Count / 3)]);
            for (int index = 1; index < this.Game.CreationParameters.Factions.Count; ++index)
            {
                int max = source1.Max<Point2>(p1 => playerPositions.Min<Point2>(p2 => IntMath.ApproxDistance(p1, p2)));
                int num = random.Int(max * 8 / 10, max);
                foreach (Point2 point2 in source1)
                {
                    Point2 position = point2;
                    if (!playerPositions.Contains(position) && playerPositions.Min<Point2>(p => IntMath.ApproxDistance(p, position)) >= num)
                    {
                        playerPositions.Add(position);
                        break;
                    }
                }
            }
            DynamicArray<Point2> source4 = new DynamicArray<Point2>();
            for (int index = 0; index < this.Game.CreationParameters.SpecialPlanetsCount; ++index)
            {
                Point2? nullable = new Point2?();
                int num1 = 0;
                foreach (Point2 point2 in source1)
                {
                    Point2 position = point2;
                    if (!playerPositions.Contains(position) && !source4.Contains(position))
                    {
                        int num2 = MathUtils.Min(2 * playerPositions.Min<Point2>(p => IntMath.ApproxDistance(p, position)), source4.Count > 0 ? source4.Min<Point2>(p => IntMath.ApproxDistance(p, position)) : int.MaxValue);
                        if (!nullable.HasValue || num2 > num1)
                        {
                            nullable = new Point2?(position);
                            num1 = num2;
                        }
                    }
                }
                if (nullable.HasValue)
                    source4.Add(nullable.Value);
            }
            int num4 = 0;
            for (int index = 0; index < source1.Count; ++index)
            {
                Faction faction = playerPositions.Contains(source1[index]) ? this.Game.CreationParameters.Factions[num4++] : Faction.Neutral;
                bool isSpecial = source4.Contains(source1[index]);
                int num1 = IntMath.Length(100 * (source1[index] - Game.WorldSize / 2) / (IntMath.Length(Game.WorldSize) / 2));
                int num2 = num1 >= 25 ? (num1 >= 45 ? 70 : 350) : 700;
                int sizeClass;
                int shipsCount;
                if (faction != Faction.Neutral)
                {
                    sizeClass = 2;
                    shipsCount = 100;
                }
                else
                {
                    sizeClass = random.Int(1000) < num2 ? 2 : random.Int(0, 1);
                    if (isSpecial)
                    {
                        shipsCount = random.Int(60, 100);
                    }
                    else
                    {
                        int min = 1 + 9 * sizeClass;
                        int max = 15 + 9 * sizeClass;
                        shipsCount = MathUtils.Max(random.Int(min, max) * this.Game.CreationParameters.NeutralsPopulationFactor / 100, 1);
                    }
                }
                Planet planet = new Planet(source1[index], faction, sizeClass, shipsCount, isSpecial);
                this.Game.AddEntity(planet);
                if (isSpecial)
                {
                    this.Game.AddEntity(new Satellite(planet, -1));
                    if (random.Int() % 5 == 0)
                        this.Game.AddEntity(new Satellite(planet, -1));
                }
            }
        }
    }
}