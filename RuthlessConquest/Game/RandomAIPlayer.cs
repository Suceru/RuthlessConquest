using Engine;
using Engine.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    internal class RandomAIPlayer : Player
    {
        private int Level;

        private int NextMoveStep;

        private RandomAIPlayer()
        {
        }

        public RandomAIPlayer(PlayerDescription playerDescription, int level)
            : base(playerDescription)
        {
            Level = level;
        }

        public override void Serialize(InputArchive archive)
        {
            base.Serialize(archive);
            archive.Serialize("Level", ref Level);
            archive.Serialize("NextMoveStep", ref NextMoveStep);
        }

        public override void Serialize(OutputArchive archive)
        {
            base.Serialize(archive);
            archive.Serialize("Level", Level);
            archive.Serialize("NextMoveStep", NextMoveStep);
        }

        public override void Step()
        {
            base.Step();
            int num;
            switch (Level)
            {
                case 0:
                    num = 360;
                    break;
                case 1:
                    num = 120;
                    break;
                case 2:
                    num = 60;
                    break;
                default:
                    throw new InvalidOperationException("Invalid level");
            }

            if (NextMoveStep == 0)
            {
                NextMoveStep = StepModule.StepIndex + StepModule.Random.Int(num / 2, num);
            }

            if (StepModule.StepIndex < NextMoveStep)
            {
                return;
            }

            NextMoveStep = StepModule.StepIndex + StepModule.Random.Int(num / 2, num * 3 / 2);
            List<Planet> list = PlanetsModule.Planets.Where((Planet p) => p.Faction == Faction).ToList();
            List<Planet> list2 = PlanetsModule.Planets.Where((Planet p) => p.Faction != Faction).ToList();
            if (list.Count <= 0 || list2.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < 40; i++)
            {
                Planet planet = list[StepModule.Random.Int(list.Count)];
                Planet planet2 = list2[StepModule.Random.Int(list2.Count)];
                DynamicArray<Planet> dynamicArray = RouteFinderModule.FindRoute(planet, planet2, allowEnemyPlanets: false);
                if (dynamicArray.Count <= 0)
                {
                    continue;
                }

                int num2 = dynamicArray.Count((Planet p) => p.Faction != Faction);
                if (Level == 0)
                {
                    if (num2 > 1)
                    {
                        continue;
                    }
                }
                else if (Level == 1)
                {
                    if (num2 > 2 || (num2 > 1 && StepModule.Random.Int(100) > 10))
                    {
                        continue;
                    }
                }
                else if (num2 > 2 || (num2 > 1 && StepModule.Random.Int(100) > 30))
                {
                    continue;
                }

                int num3 = 100 * planet.ShipsCount / (planet2.ShipsCount + 1);
                int num4 = planet2.ShipsCount;
                if (planet2.DefenceFactor > 0)
                {
                    num3 = num3 * planet2.DefenceFactor / 100;
                    num4 = num4 * 100 / planet2.DefenceFactor;
                }

                for (int j = 0; j < planet2.Satellites.Count(); j++)
                {
                    num3 = num3 * 2 / 3;
                    num4 = num4 * 3 / 2 + 20;
                }

                num4 += StepModule.Random.Int(planet2.ShipsCount) + 5;
                if (Level == 0)
                {
                    ShipsModule.SendShips(planet, dynamicArray, num4, Faction.None);
                }
                else if (num3 > 200)
                {
                    ShipsModule.SendShips(planet, dynamicArray, num4, Faction.None);
                }
                else if (num3 > 100)
                {
                    int num5 = (Level == 1) ? 500 : 250;
                    if (StepModule.Random.Int(0, 1000) < num5)
                    {
                        ShipsModule.SendShips(planet, dynamicArray, num4, Faction.None);
                    }
                }
                else
                {
                    int num6 = (Level == 1) ? 100 : 50;
                    if (StepModule.Random.Int(0, 1000) < num6)
                    {
                        ShipsModule.SendShips(planet, dynamicArray, num4, Faction.None);
                    }
                }

                break;
            }
        }
    }
}