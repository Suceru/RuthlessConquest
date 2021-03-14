using System;
using System.Collections.Generic;
using System.Linq;
using Engine;

namespace Game
{
    internal class RouteFinderModule : Module
    {
        public RouteFinderModule(Game game) : base(game)
        {
        }

        public void FindRoute(DynamicArray<Planet> result, Planet start, Planet destination, IEnumerable<Planet> planets)
        {
            result.Clear();
            if (start == destination)
            {
                return;
            }
            if (ShipsModule.TestReachability(start.Position, destination.Position))
            {
                result.Add(destination);
                return;
            }
            this.A.ShipsModule = ShipsModule;
            this.A.Faction = start.Faction;
            this.A.Destination = destination;
            this.A.Planets = planets;
            if (this.A.FindPath(result, start, 2147483647))
            {
                result.Reverse();
                return;
            }
            result.Clear();
        }

        public DynamicArray<Planet> FindRoute(Planet start, Planet destination, IEnumerable<Planet> planets)
        {
            DynamicArray<Planet> result = new DynamicArray<Planet>();
            this.FindRoute(result, start, destination, planets);
            return result;
        }

        public DynamicArray<Planet> FindRoute(Planet start, Planet destination, bool allowEnemyPlanets)
        {
            IEnumerable<Planet> planets;
            if (allowEnemyPlanets)
            {
                planets = PlanetsModule.Planets;
            }
            else
            {
                DynamicArray<Planet> dynamicArray = new DynamicArray<Planet>(from p in PlanetsModule.Planets
                                                                             where p.Faction == start.Faction
                                                                             select p);
                if (!dynamicArray.Contains(destination))
                {
                    dynamicArray.Add(destination);
                }
                planets = dynamicArray;
            }
            return this.FindRoute(start, destination, planets);
        }

        public int GetRouteDistance(Planet planet1, Planet planet2)
        {
            if (this.RouteDistances.Count == 0)
            {
                this.CalculateRouteDistances();
            }
            int result;
            //
            if (this.RouteDistances.TryGetValue(new ValueTuple<Planet,Planet>(planet1, planet2), out result))
            {
                return result;
            }
            return int.MaxValue;
        }

        private void CalculateRouteDistances()
        {
            DynamicArray<Planet> dynamicArray = new DynamicArray<Planet>();
            foreach (Planet planet in PlanetsModule.Planets)
            {
                foreach (Planet planet2 in PlanetsModule.Planets)
                {
                    if (planet != planet2)
                    {
                        int value;
                        if (this.RouteDistances.TryGetValue(new ValueTuple<Planet, Planet>(planet2, planet), out value))
                        {
                            this.RouteDistances[new ValueTuple<Planet, Planet>(planet, planet2)] = value;
                        }
                        else
                        {
                            this.A.ShipsModule = ShipsModule;
                            this.A.Faction = Faction.None;
                            this.A.Destination = planet2;
                            this.A.Planets = PlanetsModule.Planets;
                            dynamicArray.Clear();
                            if (this.A.FindPath(dynamicArray, planet, 2147483647))
                            {
                                dynamicArray.Add(planet);
                                int num = 0;
                                for (int i = 1; i < dynamicArray.Count; i++)
                                {
                                    num += IntMath.Distance(dynamicArray[i - 1].Position, dynamicArray[i].Position);
                                }
                                this.RouteDistances[new ValueTuple<Planet, Planet>(planet, planet2)] = num;
                            }
                            else
                            {
                                this.RouteDistances[new ValueTuple<Planet, Planet>(planet, planet2)] = int.MaxValue;
                            }
                        }
                    }
                    else
                    {
                        this.RouteDistances[new ValueTuple<Planet, Planet>(planet, planet2)] = 0;
                    }
                }
            }
        }

        private RouteFinderModule.AStar A = new RouteFinderModule.AStar();

        private Dictionary<ValueTuple<Planet, Planet>, int> RouteDistances = new Dictionary<ValueTuple<Planet, Planet>, int>();

        private class AStar : AStar<Planet>
        {
            protected override int Cost(Planet p1, Planet p2)
            {
                if (p2.Faction == this.Faction)
                {
                    return IntMath.Distance(p1.Position, p2.Position);
                }
                return 10 * IntMath.Distance(p1.Position, p2.Position);
            }

            protected override int Heuristic(Planet p)
            {
                return IntMath.ApproxDistance(p.Position, this.Destination.Position);
            }

            protected override void Neighbors(Planet p, DynamicArray<Planet> neighbors)
            {
                foreach (Planet planet in this.Planets)
                {
                    if (planet != p && this.ShipsModule.TestReachability(p.Position, planet.Position))
                    {
                        neighbors.Add(planet);
                    }
                }
            }

            protected override void StorageClear()
            {
                foreach (Planet planet in this.Planets)
                {
                    planet.AStarStorage = null;
                }
            }

            protected override void StorageAdd(Planet p, object data)
            {
                p.AStarStorage = data;
            }

            protected override object StorageGet(Planet p)
            {
                return p.AStarStorage;
            }

            public ShipsModule ShipsModule;

            public Faction Faction;

            public Planet Destination;

            public IEnumerable<Planet> Planets;
        }
    }
}
