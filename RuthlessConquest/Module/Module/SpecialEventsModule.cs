using System;
using System.Linq;
using Engine;
using Engine.Serialization;

namespace Game
{
    internal class SpecialEventsModule : Module
    {
        public SpecialEventsModule(Game game) : base(game)
        {
        }

        public override void Serialize(InputArchive archive)
        {
            archive.Serialize("NextEventStep", ref this.NextEventStep);
        }

        public override void Serialize(OutputArchive archive)
        {
            archive.Serialize("NextEventStep", this.NextEventStep);
        }

        public void Step()
        {
            if (this.NextEventStep == 0)
            {
                this.NextEventStep = StepModule.StepIndex + Game.CreationParameters.SpecialEventsPeriod * StepModule.Random.Int(20, 40) / 10 * 60;
            }
            if (StepModule.StepIndex >= this.NextEventStep)
            {
                if (StepModule.Random.Int() % 2 == 0)
                {
                    this.SendAliens();
                }
                else
                {
                    this.SendReinforcements();
                }
                this.NextEventStep = StepModule.StepIndex + Game.CreationParameters.SpecialEventsPeriod * StepModule.Random.Int(15, 20) / 10 * 60;
            }
        }

        private void SendAliens()
        {
            int num = MathUtils.Clamp(StepModule.Random.Int(PlanetsModule.Planets.Count / 12, PlanetsModule.Planets.Count / 6), 1, 5);
            DynamicArray<Faction> dynamicArray = (from p in PlayersModule.Players
                                                  where p.Status == FactionStatus.Undecided
                                                  select p.Faction into f
                                                  orderby this.GetFactionStrength(f) descending
                                                  select f).ToDynamicArray<Faction>();
            if (dynamicArray.Count > 0)
            {
                Faction faction = dynamicArray[0];
                DynamicArray<Planet> dynamicArray2 = new DynamicArray<Planet>();
                int num2 = 0;
                while (num2 < 1000 && dynamicArray2.Count < num)
                {
                    Planet planet = PlanetsModule.Planets[StepModule.Random.Int(PlanetsModule.Planets.Count)];
                    if (planet.Faction == faction && planet.SizeClass >= 1 && !dynamicArray2.Contains(planet) && PlanetsModule.GetFactionPlanetsCount(faction) - dynamicArray2.Count - 1 > dynamicArray2.Count)
                    {
                        dynamicArray2.Add(planet);
                    }
                    num2++;
                }
                if (dynamicArray2.Count > 0)
                {
                    int factionStrength = this.GetFactionStrength(faction);
                    int num3 = (from f in dynamicArray
                                where f != faction
                                select f).Sum((Faction f) => this.GetFactionStrength(f)) / (dynamicArray.Count - 1);
                    int num4 = factionStrength * Game.CreationParameters.AliensFactors.X / 100;
                    int num5 = -num3 * Game.CreationParameters.AliensFactors.Y / 100;
                    int z = Game.CreationParameters.AliensFactors.Z;
                    int num6 = MathUtils.Clamp(num4 + num5 + z, 150, 700);
                    foreach (Planet destination in dynamicArray2)
                    {
                        ShipsModule.SendOutsideShips(Faction.Neutral, destination, num6 / dynamicArray2.Count);
                    }
                    Game.Screen.AliensAttackMessageWidget.SetMessage("ANTARANS ATTACK!");
                    AudioManager.PlaySound(Sounds.Aliens, false, 1f, 1f, 0f);
                }
            }
        }

        private void SendReinforcements()
        {
            DynamicArray<Faction> dynamicArray = (from p in PlayersModule.Players
                                                  where p.Status == FactionStatus.Undecided
                                                  select p.Faction into f
                                                  orderby this.GetFactionStrength(f)
                                                  select f).ToDynamicArray<Faction>();
            if (dynamicArray.Count >= 2)
            {
                Faction faction = dynamicArray[0];
                DynamicArray<Planet> dynamicArray2 = (from p in PlanetsModule.Planets
                                                      where p.Faction == faction
                                                      select p).ToDynamicArray<Planet>();
                Planet destination;
                if (dynamicArray2.Count > 0)
                {
                    destination = dynamicArray2[StepModule.Random.Int(dynamicArray2.Count)];
                }
                else
                {
                    destination = PlanetsModule.Planets[StepModule.Random.Int(PlanetsModule.Planets.Count)];
                }
                int factionStrength = this.GetFactionStrength(faction);
                int num = (from f in dynamicArray
                           where f != faction
                           select f).Sum((Faction f) => this.GetFactionStrength(f)) / (dynamicArray.Count - 1);
                int num2 = factionStrength * Game.CreationParameters.ReinforcementsFactors.X / 100;
                int num3 = -num * Game.CreationParameters.ReinforcementsFactors.Y / 100;
                int z = Game.CreationParameters.ReinforcementsFactors.Z;
                int shipsCount = MathUtils.Clamp(num2 + num3 + z, 150, 700);
                ShipsModule.SendOutsideShips(faction, destination, shipsCount);
                Game.Screen.ReinforcementsMessageWidget.SetMessage(faction, "REINFORCEMENTS ARRIVE!");
                AudioManager.PlaySound(Sounds.Reinforcements, false, 1f, 1f, 0f);
            }
        }

        private int GetFactionStrength(Faction faction)
        {
            int period = Game.CreationParameters.PlanetValue * 60;
            int num = ShipsModule.CountFactionShips(faction);
            int num2 = (from p in PlanetsModule.Planets
                        where p.Faction == faction
                        select p).Sum((Planet p) => period / p.ProductionPeriod);
            int num3 = Game.CreationParameters.SatelliteValue * Game.Entities.OfType<Satellite>().Count((Satellite s) => s.Planet != null && s.Planet.Faction == faction);
            return num + num2 + num3;
        }

        private int NextEventStep;
    }
}
