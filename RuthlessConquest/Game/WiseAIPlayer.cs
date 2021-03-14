using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Serialization;

namespace Game
{
    internal class WiseAIPlayer : Player
    {
        private WiseAIPlayer()
        {
        }

        public WiseAIPlayer(PlayerDescription playerDescription, int level) : base(playerDescription)
        {
            this.Level = level;
        }

        public override void Serialize(InputArchive archive)
        {
            base.Serialize(archive);
            archive.Serialize("Level", ref this.Level);
            archive.Serialize("NextStep", ref this.NextStep);
            archive.Serialize<WiseAIPlayer.AttackPlan>("BestStrategyPlan", ref this.BestStrategyPlan);
        }

        public override void Serialize(OutputArchive archive)
        {
            base.Serialize(archive);
            archive.Serialize("Level", this.Level);
            archive.Serialize("NextStep", this.NextStep);
            archive.Serialize<WiseAIPlayer.AttackPlan>("BestStrategyPlan", this.BestStrategyPlan);
        }

        public override void Draw(Color colorTransform)
        {
        }

        public override void Step()
        {
            base.Step();
            int num;
            int num2;
            switch (this.Level)
            {
                case 0:
                    num = 180;
                    num2 = 1;
                    break;
                case 1:
                    num = 120;
                    num2 = 2;
                    break;
                case 2:
                    num = 60;
                    num2 = 3;
                    break;
                default:
                    throw new InvalidOperationException("Invalid level");
            }
            if (this.BestStrategyPlan != null && this.BestStrategyPlan.Target.Faction == Faction)
            {
                this.BestStrategyPlan = null;
            }
            if (StepModule.StepIndex >= this.NextStep)
            {
                this.NextStep = StepModule.StepIndex + StepModule.Random.Int(num / 2, num);
                for (int i = 0; i < num2; i++)
                {
                    this.Move();
                }
                this.Attack();
                this.Satellite();
            }
        }

        private void Move()
        {
            if (Game.CreationParameters.ShipRange > 20000)
            {
                return;
            }
            List<Planet> list = (from p in PlanetsModule.Planets
                                 where p.Faction == Faction
                                 select p).ToList<Planet>();
            if (list.Count > 0)
            {
                List<Planet> list2 = (from p in (from p in PlanetsModule.Planets
                                                 where p.Faction != Faction
                                                 select p).ToList<Planet>()
                                      where p.Faction != Faction && p.Faction != Faction.Neutral
                                      select p).ToList<Planet>();
                int num = 0;
                Planet planet = null;
                int num2 = 0;
                DynamicArray<Planet> dynamicArray = null;
                for (int i = 0; i < 100; i++)
                {
                    Planet source = list[StepModule.Random.Int(list.Count)];
                    Planet target = list[StepModule.Random.Int(list.Count)];
                    if (source != target && source.ShipsCount >= 8)
                    {
                        int routeDistance = RouteFinderModule.GetRouteDistance(source, target);
                        if (routeDistance > 0)
                        {
                            int num3 = (from p in list2
                                        where this.ShipsModule.TestReachability(p.Position, source.Position)
                                        select p).Sum((Planet p) => p.ShipsCount);
                            int num4 = source.ShipsCount - num3 * 7 / 10;
                            if (num4 >= 8)
                            {
                                int num5 = SafeMin(20000, list2, (Planet p) => this.RouteFinderModule.GetRouteDistance(source, p));
                                int num6 = SafeMin(20000, list2, (Planet p) => this.RouteFinderModule.GetRouteDistance(target, p));
                                int num7 = 100 * (num5 - num6) / routeDistance;
                                if (this.BestStrategyPlan != null)
                                {
                                    if (this.BestStrategyPlan.AttackRoutes.Any((WiseAIPlayer.AttackRoute r) => r.Source == target))
                                    {
                                        num7 += 60;
                                    }
                                    if (this.BestStrategyPlan.AttackRoutes.Any((WiseAIPlayer.AttackRoute r) => r.Source == source))
                                    {
                                        num7 -= 1000;
                                    }
                                }
                                if (source.ShipsCount >= target.MaxShipsCount * 9 / 10)
                                {
                                    num7 += 25;
                                }
                                if (target.ShipsCount >= target.MaxShipsCount * 9 / 10)
                                {
                                    num7 -= (target.CanLaunchSatellite(true) ? 25 : 50);
                                }
                                if (source.SizeClass == 0 && target.SizeClass != 0)
                                {
                                    num7 += 5;
                                }
                                if (num7 > num)
                                {
                                    DynamicArray<Planet> dynamicArray2 = RouteFinderModule.FindRoute(source, target, false);
                                    if (dynamicArray2.Count > 0)
                                    {
                                        num = num7;
                                        planet = source;
                                        dynamicArray = dynamicArray2;
                                        num2 = num4;
                                    }
                                }
                            }
                        }
                    }
                }
                if (num > 10 && planet != null && dynamicArray != null && num2 > 0)
                {
                    ShipsModule.SendShips(planet, dynamicArray, num2, Faction.None);
                }
            }
        }

        private void Attack()
        {
            int num;
            int num2;
            switch (this.Level)
            {
                case 0:
                    num = 15;
                    num2 = 100;
                    break;
                case 1:
                    num = 5;
                    num2 = 80;
                    break;
                case 2:
                    num = 3;
                    num2 = 60;
                    break;
                default:
                    throw new InvalidOperationException("Invalid level");
            }
            List<Planet> list = (from p in PlanetsModule.Planets
                                 where p.Faction == Faction
                                 select p).ToList<Planet>();
            List<Planet> list2 = (from p in PlanetsModule.Planets
                                  where p.Faction != Faction
                                  select p).ToList<Planet>();
            List<Planet> list3 = (from p in list2
                                  where p.Faction != Faction && p.Faction != Faction.Neutral
                                  select p).ToList<Planet>();
            Faction strongestEnemyFaction = this.FindStrongestEnemyFaction();
            if (list.Count <= 0 || list2.Count <= 0)
                return;
            DynamicArray<WiseAIPlayer.AttackPlan> source = new DynamicArray<WiseAIPlayer.AttackPlan>();
            foreach (Planet planet1 in list2)
            {
                Planet ep = planet1;
                if (!this.ShipsModule.Ships.Any<Ship>(s => s.Faction == this.Faction && s.Route.Contains(ep)))
                {
                    int num3 = SafeMin(20000, list, p => IntMath.ApproxDistance(p.Position, ep.Position)) + 3000;
                    WiseAIPlayer.AttackPlan plan = new WiseAIPlayer.AttackPlan()
                    {
                        Target = ep
                    };
                    foreach (Planet planet2 in list)
                    {
                        if (this.ShipsModule.TestReachability(planet2.Position, ep.Position))
                        {
                            int x2 = IntMath.ApproxDistance(ep.Position, planet2.Position);
                            if (x2 <= num3)
                            {
                                /* Func<Planet, bool> predicate;
                                 {
                                     // predicate= p => p != ep;
                                     predicate = p => p != ep;
                                 }*/
                                int num4 = MathUtils.Clamp(100 * SafeMin(20000, list3.Where(p => (p != ep)), p => this.RouteFinderModule.GetRouteDistance(plan.Target, p)) / 5000, 50, 100);
                                int num5 = planet2.ShipsCount * num4 / 100;
                                plan.ShipsAvailable += num5;
                                plan.AttackRoutes.Add(new WiseAIPlayer.AttackRoute()
                                {
                                    Source = planet2,
                                    Ships = num5
                                });
                                plan.Distance = MathUtils.Max(plan.Distance, x2);
                            }
                        }
                    }
                    if (plan.AttackRoutes.Count > 0)
                    {
                        plan.ReinforcementsCount = list2.Where<Planet>(p => p != ep && this.ShipsModule.TestReachability(p.Position, ep.Position)).Sum<Planet>(p => p.ShipsCount);
                        source.Add(plan);
                    }
                }
            }
            foreach (WiseAIPlayer.AttackPlan attackPlan in source)
            {
                WiseAIPlayer.AttackPlan plan = attackPlan;
                int num3 = SafeMin(20000, list3.Where<Planet>(p => p.Faction != plan.Target.Faction), p => this.RouteFinderModule.GetRouteDistance(plan.Target, p));
                int num4 = plan.Target.ShipsCount + plan.ReinforcementsCount / 5;
                for (int index = 0; index < plan.Target.Satellites.Count<Satellite>(); ++index)
                    num4 = num4 * 3 / 2 + 15;
                plan.ScoreStrength += MathUtils.Min(100 * plan.ShipsAvailable / (num4 + 1), 1000);
                plan.ScoreStrategy = 1L;
                plan.ScoreStrategy += 10000 / (plan.Target.ShipsCount + plan.ReinforcementsCount / 5 + 1);
                plan.ScoreStrategy *= 10000L;
                plan.ScoreStrategy /= plan.Target.ProductionPeriod + 50;
                plan.ScoreStrategy *= 1000L;
                plan.ScoreStrategy /= 1000 + plan.Distance;
                if (plan.Target.Faction == Faction.Neutral)
                    plan.ScoreStrategy = plan.ScoreStrategy * 80L / 100L;
                if (plan.Target.IsSpecial)
                    plan.ScoreStrategy *= 2L;
                plan.ScoreStrategy *= 2 + plan.AttackRoutes.Count;
                for (int index = 0; index < plan.Target.Satellites.Count<Satellite>(); ++index)
                    plan.ScoreStrategy = plan.ScoreStrategy * 9L / 10L;
                if (this.Level > 0)
                {
                    plan.ScoreStrategy *= 1000 + num3;
                    if (plan.Target.Faction == strongestEnemyFaction)
                        plan.ScoreStrategy = plan.ScoreStrategy * 15L / 10L;
                }
            }
            this.BestStrategyPlan = source.OrderByDescending<WiseAIPlayer.AttackPlan, long>(p => p.ScoreStrategy).FirstOrDefault<WiseAIPlayer.AttackPlan>();
            WiseAIPlayer.AttackPlan attackPlan1 = source.OrderByDescending<WiseAIPlayer.AttackPlan, long>(p => p.ScoreStrength * p.ScoreStrategy).FirstOrDefault<WiseAIPlayer.AttackPlan>();
            if (attackPlan1 == null)
                return;
            bool flag = attackPlan1.Target.Faction == Faction.Neutral;
            int num6 = flag ? 5 : 25;
            int num7 = 100 * attackPlan1.ShipsAvailable / (attackPlan1.Target.ShipsCount + num6);
            if (attackPlan1.Target.DefenceFactor > 0)
                num7 = num7 * attackPlan1.Target.DefenceFactor / 100;
            for (int index = 0; index < attackPlan1.Target.Satellites.Count<Satellite>(); ++index)
                num7 = num7 * 2 / 3;
            if (num7 <= 0)
                return;
            int num8 = flag ? 100 : 120;
            int num9 = num7 >= 80 ? (num7 >= num8 ? (num7 >= 200 ? 500 : 250) : 6 * num) : 2 * num;
            if (this.StepModule.Random.Int(1000) >= num9)
                return;
            foreach (WiseAIPlayer.AttackRoute attackRoute in attackPlan1.AttackRoutes)
            {
                DynamicArray<Planet> route = this.RouteFinderModule.FindRoute(attackRoute.Source, attackPlan1.Target, true);
                if (route.Count > 0)
                {
                    int num3 = MathUtils.Min(attackRoute.Ships * 100 / num7, attackRoute.Ships);
                    int num4 = (attackRoute.Ships - num3) * num2 / 100;
                    int count = MathUtils.Clamp(num3 + num4, 0, attackRoute.Ships);
                    this.ShipsModule.SendShips(attackRoute.Source, route, count, Faction.None);
                }
            }
            if (this.BestStrategyPlan != attackPlan1)
                return;
            this.BestStrategyPlan = null;
        }
        /* if (list.Count > 0 && list2.Count > 0)
         {
             DynamicArray<WiseAIPlayer.AttackPlan> dynamicArray = new DynamicArray<WiseAIPlayer.AttackPlan>();
             using (List<Planet>.Enumerator enumerator = list2.GetEnumerator())
             {
                 while (enumerator.MoveNext())
                 {
                     WiseAIPlayer.<> c__DisplayClass12_0 CS$<> 8__locals1 = new WiseAIPlayer.<> c__DisplayClass12_0();
                     CS$<> 8__locals1.<> 4__this = this;
                     CS$<> 8__locals1.ep = enumerator.Current;
                     if (!base.ShipsModule.Ships.Any((Ship s) => s.Faction == CS$<> 8__locals1.<> 4__this.Faction && s.Route.Contains(CS$<> 8__locals1.ep)))
                     {
                         int num3 = WiseAIPlayer.SafeMin(20000, list, (Planet p) => IntMath.ApproxDistance(p.Position, CS$<> 8__locals1.ep.Position)) + 3000;
                         WiseAIPlayer.AttackPlan plan = new WiseAIPlayer.AttackPlan
                         {
                             Target = CS$<> 8__locals1.ep
                         };
                         Func<Planet, int> <> 9__10;
        //
                         foreach (Planet planet in list)
                         {
                             if (base.ShipsModule.TestReachability(planet.Position, CS$<> 8__locals1.ep.Position))
                             {
                                 int num4 = IntMath.ApproxDistance(CS$<> 8__locals1.ep.Position, planet.Position);
                                 if (num4 <= num3)
                                 {
                                     int initial = 20000;
                                     IEnumerable<Planet> source = list3;
                                     Func<Planet, bool> predicate;
                                     if ((predicate = CS$<> 8__locals1.<> 9__9) == null)
                                     {
                                         predicate = (CS$<> 8__locals1.<> 9__9 = ((Planet p) => p != CS$<> 8__locals1.ep));
                                     }
                                     IEnumerable<Planet> e = source.Where(predicate);
                                     Func<Planet, int> selector;
                                     if ((selector = <> 9__10) == null)
                                     {
                                         selector = (<> 9__10 = ((Planet p) => CS$<> 8__locals1.<> 4__this.RouteFinderModule.GetRouteDistance(plan.Target, p)));
                                     }
                                     int num5 = WiseAIPlayer.SafeMin(initial, e, selector);
                                     int num6 = MathUtils.Clamp(100 * num5 / 5000, 50, 100);

       //推演

                                     Func<Planet, bool>(p => p != ep);
                                    
                                     IEnumerable<Planet> e = list3.Where(Func<Planet, bool>(p => p != ep));

                                     Func<Planet, int> selector;
                                     if ((selector = <> 9__10) == null)
                                     {
                                         selector = (<> 9__10 = ((Planet p) => this.RouteFinderModule.GetRouteDistance(plan.Target, p)));
                                     }
                                     int num6 = MathUtils.Clamp(100 * WiseAIPlayer.SafeMin(20000, e, selector)/ 5000, 50, 100);
                                    
        int num4 = 
        MathUtils.Clamp(100 * 
        WiseAIPlayer.SafeMin(20000,list3.Where<Planet>(closure_0 ?? (closure_0 = (Func<Planet, bool>) (p => p != ep))),
        (Func<Planet, int>) (p => this.RouteFinderModule.GetRouteDistance(plan.Target, p))) / 5000, 50, 100);
                               

        //推演结束



                                     int num7 = planet.ShipsCount * num6 / 100;
                                     plan.ShipsAvailable += num7;
                                     plan.AttackRoutes.Add(new WiseAIPlayer.AttackRoute
                                     {
                                         Source = planet,
                                         Ships = num7
                                     });
                                     plan.Distance = MathUtils.Max(plan.Distance, num4);
                                 }
                             }
                         }
                         if (plan.AttackRoutes.Count > 0)
                         {
                             plan.ReinforcementsCount = (from p in list2
                                                         where p != CS$<> 8__locals1.ep && CS$<> 8__locals1.<> 4__this.ShipsModule.TestReachability(p.Position, CS$<> 8__locals1.ep.Position)

                             select p).Sum((Planet p) => p.ShipsCount);
                             dynamicArray.Add(plan);
                         }
                     }
                 }
             }
             using (DynamicArray<WiseAIPlayer.AttackPlan>.Enumerator enumerator3 = dynamicArray.GetEnumerator())
             {
                 while (enumerator3.MoveNext())
                 {
                     WiseAIPlayer.AttackPlan plan = enumerator3.Current;
                     int num8 = WiseAIPlayer.SafeMin(20000, from p in list3
                                                            where p.Faction != plan.Target.Faction
                                                            select p, (Planet p) => this.RouteFinderModule.GetRouteDistance(plan.Target, p));
                     int num9 = plan.Target.ShipsCount + plan.ReinforcementsCount / 5;
                     for (int i = 0; i < plan.Target.Satellites.Count<Satellite>(); i++)
                     {
                         num9 = num9 * 3 / 2 + 15;
                     }
                     plan.ScoreStrength += (long)MathUtils.Min(100 * plan.ShipsAvailable / (num9 + 1), 1000);
                     plan.ScoreStrategy = 1L;
                     plan.ScoreStrategy += (long)(10000 / (plan.Target.ShipsCount + plan.ReinforcementsCount / 5 + 1));
                     plan.ScoreStrategy *= 10000L;
                     plan.ScoreStrategy /= (long)(plan.Target.ProductionPeriod + 50);
                     plan.ScoreStrategy *= 1000L;
                     plan.ScoreStrategy /= (long)(1000 + plan.Distance);
                     if (plan.Target.Faction == Faction.Neutral)
                     {
                         plan.ScoreStrategy = plan.ScoreStrategy * 80L / 100L;
                     }
                     if (plan.Target.IsSpecial)
                     {
                         plan.ScoreStrategy *= 2L;
                     }
                     plan.ScoreStrategy *= (long)(2 + plan.AttackRoutes.Count);
                     for (int j = 0; j < plan.Target.Satellites.Count<Satellite>(); j++)
                     {
                         plan.ScoreStrategy = plan.ScoreStrategy * 9L / 10L;
                     }
                     if (this.Level > 0)
                     {
                         plan.ScoreStrategy *= (long)(1000 + num8);
                         if (plan.Target.Faction == faction)
                         {
                             plan.ScoreStrategy = plan.ScoreStrategy * 15L / 10L;
                         }
                     }
                 }
             }
             this.BestStrategyPlan = (from p in dynamicArray
                                      orderby p.ScoreStrategy descending
                                      select p).FirstOrDefault<WiseAIPlayer.AttackPlan>();
             WiseAIPlayer.AttackPlan attackPlan = (from p in dynamicArray
                                                   orderby p.ScoreStrength * p.ScoreStrategy descending
                                                   select p).FirstOrDefault<WiseAIPlayer.AttackPlan>();
             if (attackPlan != null)
             {
                 bool flag = attackPlan.Target.Faction == Faction.Neutral;
                 int num10 = flag ? 5 : 25;
                 int num11 = 100 * attackPlan.ShipsAvailable / (attackPlan.Target.ShipsCount + num10);
                 if (attackPlan.Target.DefenceFactor > 0)
                 {
                     num11 = num11 * attackPlan.Target.DefenceFactor / 100;
                 }
                 for (int k = 0; k < attackPlan.Target.Satellites.Count<Satellite>(); k++)
                 {
                     num11 = num11 * 2 / 3;
                 }
                 if (num11 > 0)
                 {
                     int num12 = flag ? 100 : 120;
                     int num13;
                     if (num11 < 80)
                     {
                         num13 = 2 * num;
                     }
                     else if (num11 < num12)
                     {
                         num13 = 6 * num;
                     }
                     else if (num11 < 200)
                     {
                         num13 = 250;
                     }
                     else
                     {
                         num13 = 500;
                     }
                     if (base.StepModule.Random.Int(1000) < num13)
                     {
                         foreach (WiseAIPlayer.AttackRoute attackRoute in attackPlan.AttackRoutes)
                         {
                             DynamicArray<Planet> dynamicArray2 = base.RouteFinderModule.FindRoute(attackRoute.Source, attackPlan.Target, true);
                             if (dynamicArray2.Count > 0)
                             {
                                 int num14 = MathUtils.Min(attackRoute.Ships * 100 / num11, attackRoute.Ships);
                                 int num15 = (attackRoute.Ships - num14) * num2 / 100;
                                 int count = MathUtils.Clamp(num14 + num15, 0, attackRoute.Ships);
                                 base.ShipsModule.SendShips(attackRoute.Source, dynamicArray2, count, Faction.None);
                             }
                         }
                         if (this.BestStrategyPlan == attackPlan)
                         {
                             this.BestStrategyPlan = null;
                         }
                     }
                 }
             }
         }
     }*/

        private void Satellite()
        {
            Planet planet2 = null;
            int num = 0;
            List<Planet> list = (from p in PlanetsModule.Planets
                                 where p.Faction == Faction
                                 select p).ToList<Planet>();
            Faction faction = this.FindStrongestEnemyFaction();
            using (List<Planet>.Enumerator enumerator = list.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Planet planet = enumerator.Current;
                    if (planet.CanLaunchSatellite(false) && planet.ShipsCount > planet.GetSatelliteCost() + 70)
                    {
                        int x = (from p in PlanetsModule.Planets
                                 where p.Faction != Faction && p.Faction != Faction.Neutral
                                 select p).ToList<Planet>().Count((Planet p) => this.ShipsModule.TestReachability(planet.Position, p.Position));
                        bool flag = IntMath.ApproxLength(100 * (planet.Position - Game.WorldSize / 2) / (Game.WorldSize / 2)) < 60;
                        int num2 = 10 * MathUtils.Min(x, 4);
                        if (flag)
                        {
                            num2 *= 2;
                        }
                        if (planet.Faction == faction)
                        {
                            num2 *= 2;
                        }
                        if (num2 > num)
                        {
                            planet2 = planet;
                            num = num2;
                        }
                    }
                }
            }
            if (planet2 != null)
            {
                planet2.LaunchSatellite();
            }
        }

        private Faction FindStrongestEnemyFaction()
        {
            int num1 = -1;
            Faction faction1 = Faction.Neutral;
            for (Faction faction = Faction.Faction1; faction <= Faction.Faction6; ++faction)
            {
                if (faction != this.Faction)
                {
                    int num2 = 0;
                    foreach (Planet planet in this.PlanetsModule.Planets.Where<Planet>(p => p.Faction == faction))
                    {
                        if (planet.ProductionPeriod > 0)
                            num2 += 10000 / planet.ProductionPeriod;
                    }
                    if (num2 > num1)
                    {
                        num1 = num2;
                        faction1 = faction;
                    }
                }
            }
            return faction1;
        }
        /* private Faction FindStrongestEnemyFaction()
         {
             int num = -1;
             Faction result = Faction.Neutral;
             Faction faction;
             Func<Planet, bool> <> 9__0;
             Faction faction2;
             for (faction = Faction.Faction1; faction <= Faction.Faction6; faction = faction2)
             {
                 if (faction != base.Faction)
                 {
                     int num2 = 0;
                     IEnumerable<Planet> source = base.PlanetsModule.Planets;
                     Func<Planet, bool> predicate;
                     if ((predicate = <> 9__0) == null)
                     {
                         predicate = (<> 9__0 = ((Planet p) => p.Faction == faction));
                     }
                     foreach (Planet planet in source.Where(predicate))
                     {
                         if (planet.ProductionPeriod > 0)
                         {
                             num2 += 10000 / planet.ProductionPeriod;
                         }
                     }
                     if (num2 > num)
                     {
                         num = num2;
                         result = faction;
                     }
                 }
                 faction2 = faction + 1;
             }
             return result;
         }
 */
        private static int SafeMin(int initial, IEnumerable<Planet> e, Func<Planet, int> selector)
        {
            int num = initial;
            foreach (Planet arg in e)
            {
                num = Math.Min(selector(arg), num);
            }
            return num;
        }

        private int Level;

        private int NextStep;

        private WiseAIPlayer.AttackPlan BestStrategyPlan;

        private class AttackPlan : ISerializable
        {
            public void Serialize(InputArchive archive)
            {
                archive.Serialize<Planet>("Target", ref this.Target);
                archive.Serialize("ReinforcementsCount", ref this.ReinforcementsCount);
                archive.Serialize("ShipsAvailable", ref this.ShipsAvailable);
                archive.Serialize("Distance", ref this.Distance);
                archive.Serialize("ScoreStrength", ref this.ScoreStrength);
                archive.Serialize("ScoreStrategy", ref this.ScoreStrategy);
                archive.Serialize<DynamicArray<WiseAIPlayer.AttackRoute>>("AttackRoutes", ref this.AttackRoutes);
            }

            public void Serialize(OutputArchive archive)
            {
                archive.Serialize<Planet>("Target", this.Target);
                archive.Serialize("ReinforcementsCount", this.ReinforcementsCount);
                archive.Serialize("ShipsAvailable", this.ShipsAvailable);
                archive.Serialize("Distance", this.Distance);
                archive.Serialize("ScoreStrength", this.ScoreStrength);
                archive.Serialize("ScoreStrategy", this.ScoreStrategy);
                archive.Serialize<DynamicArray<WiseAIPlayer.AttackRoute>>("AttackRoutes", this.AttackRoutes);
            }

            public Planet Target;

            public int ReinforcementsCount;

            public int ShipsAvailable;

            public int Distance;

            public long ScoreStrength;

            public long ScoreStrategy;

            public DynamicArray<WiseAIPlayer.AttackRoute> AttackRoutes = new DynamicArray<WiseAIPlayer.AttackRoute>();
        }

        private class AttackRoute : ISerializable
        {
            public void Serialize(InputArchive archive)
            {
                archive.Serialize<Planet>("Source", ref this.Source);
                archive.Serialize("Ships", ref this.Ships);
            }

            public void Serialize(OutputArchive archive)
            {
                archive.Serialize<Planet>("Source", this.Source);
                archive.Serialize("Ships", this.Ships);
            }

            public Planet Source;

            public int Ships;
        }
    }
}
