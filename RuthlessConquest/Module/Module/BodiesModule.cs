using System;
using Engine;

namespace Game
{
    internal class BodiesModule : Module
    {
        public BodiesModule(Game game) : base(game)
        {
            this.Grid = new BodyGrid(Game.WorldSize, 250);
        }

        public void BuildGrid()
        {
            this.Grid.Clear();
            foreach (Planet planet in PlanetsModule.Planets)
            {
                this.Grid.Add(planet.Body);
            }
            foreach (Ship ship in ShipsModule.Ships)
            {
                this.Grid.Add(ship.Body);
            }
        }

        public void QueryBodies(Point2 position, int radius, DynamicArray<Body> result, int maxCount)
        {
            this.Grid.QueryBodies(position, radius, result, maxCount);
        }

        private BodyGrid Grid;
    }
}
