using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Serialization;

namespace Game
{
    /// <summary>
    /// 保护结构
    /// 游戏创建参数
    /// </summary>
    internal struct GameCreationParameters
    {
        public GameCreationParameters(int seed, int planetsCount, int specialPlanetsCount, int neutralsPopulationFactor, int shipRange, int maxHumanPlayersCount, int totalPlayersCount, int warmupTime, Faction creatingPlayerFaction, string creatingPlayerName, Guid creatingPlayerGuid, Platform creatingPlayerPlatform, PlayerType aiLevel)
        {
            this.Seed = seed;
            this.PlanetsCount = planetsCount;
            this.SpecialPlanetsCount = specialPlanetsCount;
            this.NeutralsPopulationFactor = neutralsPopulationFactor;
            this.ShipRange = shipRange;
            this.ShipSpeed = 750;
            this.PlanetRepelFactor = 25;
            this.ShipRepelFactor = 15;
            this.MaxHumanPlayersCount = maxHumanPlayersCount;
            this.CountdownTicksCount = warmupTime * 60 / 30 + 1;
            this.CreatingPlayerFaction = creatingPlayerFaction;
            this.CreatingPlayerName = creatingPlayerName;
            this.CreatingPlayerGuid = creatingPlayerGuid;
            this.CreatingPlayerPlatform = creatingPlayerPlatform;
            this.AILevel = aiLevel;
            this.SpecialEventsPeriod = 60;
            this.NoProductionSteps = 480;
            this.PlanetValue = 90;
            this.SatelliteValue = 90;
            //new ValueTuple<int, int, int>
            this.AliensFactors = new Point3(80, 40, 0);
            this.ReinforcementsFactors = new Point3(-30, -30, 200);
            this.Factions = GenerateRandomFactions(seed, creatingPlayerFaction, totalPlayersCount);
        }

        public static int CalculateSeed(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return new Engine.Random().Int();
            }
            int num = 0;
            for (int i = 0; i < s.Length; i++)
            {
                num += (1171 * i + 997) * s[i];
            }
            return MathUtils.Hash(num);
        }

        private static DynamicArray<Faction> GenerateRandomFactions(int seed, Faction creatingPlayerFaction, int factionsCount)
        {
            DynamicArray<Faction> dynamicArray = new DynamicArray<Faction>();
            Engine.Random random = new Engine.Random(MathUtils.Hash(seed));
            List<Faction> list = Enumerable.Range(0, 6).Cast<Faction>().ToList<Faction>();
            if (creatingPlayerFaction != Faction.None)
            {
                list.Remove(creatingPlayerFaction);
                dynamicArray.Add(creatingPlayerFaction);
            }
            while (dynamicArray.Count < factionsCount)
            {
                Faction item = list[random.Int(list.Count)];
                list.Remove(item);
                dynamicArray.Add(item);
            }
            return dynamicArray;
        }

        public int Seed;

        public int PlanetsCount;

        public int SpecialPlanetsCount;

        public int NeutralsPopulationFactor;

        public int ShipRange;

        public int ShipSpeed;

        public int ShipRepelFactor;

        public int PlanetRepelFactor;

        public int MaxHumanPlayersCount;

        public int CountdownTicksCount;

        public Faction CreatingPlayerFaction;

        public string CreatingPlayerName;

        public Guid CreatingPlayerGuid;

        public Platform CreatingPlayerPlatform;

        public PlayerType AILevel;

        public int SpecialEventsPeriod;

        public int NoProductionSteps;

        public int PlanetValue;

        public int SatelliteValue;

        public Point3 AliensFactors;

        public Point3 ReinforcementsFactors;

        public DynamicArray<Faction> Factions;

        private class Serializer : ISerializer<GameCreationParameters>
        {
            public void Serialize(InputArchive archive, ref GameCreationParameters value)
            {
                archive.Serialize("Seed", ref value.Seed);
                archive.Serialize("PlanetsCount", ref value.PlanetsCount);
                archive.Serialize("SpecialPlanetsCount", ref value.SpecialPlanetsCount);
                archive.Serialize("NeutralsPopulationFactor", ref value.NeutralsPopulationFactor);
                archive.Serialize("ShipRange", ref value.ShipRange);
                archive.Serialize("ShipSpeed", ref value.ShipSpeed);
                archive.Serialize("ShipRepelFactor", ref value.ShipRepelFactor);
                archive.Serialize("PlanetRepelFactor", ref value.PlanetRepelFactor);
                archive.Serialize("MaxHumanPlayersCount", ref value.MaxHumanPlayersCount);
                archive.Serialize("CountdownTicksCount", ref value.CountdownTicksCount);
                archive.Serialize<Faction>("CreatingPlayerFaction", ref value.CreatingPlayerFaction);
                archive.Serialize("CreatingPlayerName", ref value.CreatingPlayerName);
                archive.Serialize<Guid>("CreatingPlayerGuid", ref value.CreatingPlayerGuid);
                archive.Serialize<Platform>("CreatingPlayerPlatform", ref value.CreatingPlayerPlatform);
                archive.Serialize<PlayerType>("AILevel", ref value.AILevel);
                archive.Serialize("SpecialEventsPeriod", ref value.SpecialEventsPeriod);
                archive.Serialize("NoProductionSteps", ref value.NoProductionSteps);
                archive.Serialize("PlanetValue", ref value.PlanetValue);
                archive.Serialize("SatelliteValue", ref value.SatelliteValue);
                archive.Serialize<Point3>("AliensFactors", ref value.AliensFactors);
                archive.Serialize<Point3>("ReinforcementsFactors", ref value.ReinforcementsFactors);
                archive.Serialize<DynamicArray<Faction>>("Factions", ref value.Factions);
            }

            public void Serialize(OutputArchive archive, GameCreationParameters value)
            {
                archive.Serialize("Seed", value.Seed);
                archive.Serialize("PlanetsCount", value.PlanetsCount);
                archive.Serialize("SpecialPlanetsCount", value.SpecialPlanetsCount);
                archive.Serialize("NeutralsPopulationFactor", value.NeutralsPopulationFactor);
                archive.Serialize("ShipRange", value.ShipRange);
                archive.Serialize("ShipSpeed", value.ShipSpeed);
                archive.Serialize("ShipRepelFactor", value.ShipRepelFactor);
                archive.Serialize("PlanetRepelFactor", value.PlanetRepelFactor);
                archive.Serialize("MaxHumanPlayersCount", value.MaxHumanPlayersCount);
                archive.Serialize("CountdownTicksCount", value.CountdownTicksCount);
                archive.Serialize<Faction>("CreatingPlayerFaction", value.CreatingPlayerFaction);
                archive.Serialize("CreatingPlayerName", value.CreatingPlayerName);
                archive.Serialize<Guid>("CreatingPlayerGuid", value.CreatingPlayerGuid);
                archive.Serialize<Platform>("CreatingPlayerPlatform", value.CreatingPlayerPlatform);
                archive.Serialize<PlayerType>("AILevel", value.AILevel);
                archive.Serialize("SpecialEventsPeriod", value.SpecialEventsPeriod);
                archive.Serialize("NoProductionSteps", value.NoProductionSteps);
                archive.Serialize("PlanetValue", value.PlanetValue);
                archive.Serialize("SatelliteValue", value.SatelliteValue);
                archive.Serialize<Point3>("AliensFactors", value.AliensFactors);
                archive.Serialize<Point3>("ReinforcementsFactors", value.ReinforcementsFactors);
                archive.Serialize<DynamicArray<Faction>>("Factions", value.Factions);
            }
        }
    }
}
