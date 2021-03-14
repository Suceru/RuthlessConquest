using System;
using Engine;
using Engine.Serialization;

namespace Game
{
    internal struct Order
    {
        public int ShipsCount;

        public int PlanetIndex;

        public DynamicArray<int> RouteIndexes;

        public Faction GiftToFaction;

        public bool LaunchSatellite;

        public bool? EnableDisableSatellites;

        private class Serializer : ISerializer<Order>
        {
            public void Serialize(InputArchive archive, ref Order value)
            {
                archive.Serialize("ShipsCount", ref value.ShipsCount);
                archive.Serialize("PlanetIndex", ref value.PlanetIndex);
                archive.Serialize<DynamicArray<int>>("RouteIndexes", ref value.RouteIndexes);
                archive.Serialize<Faction>("GiftToFaction", ref value.GiftToFaction);
                archive.Serialize("LaunchSatellite", ref value.LaunchSatellite);
                archive.Serialize<bool?>("EnableDisableSatellites", ref value.EnableDisableSatellites);
            }

            public void Serialize(OutputArchive archive, Order value)
            {
                archive.Serialize("ShipsCount", value.ShipsCount);
                archive.Serialize("PlanetIndex", value.PlanetIndex);
                archive.Serialize<DynamicArray<int>>("RouteIndexes", value.RouteIndexes);
                archive.Serialize<Faction>("GiftToFaction", value.GiftToFaction);
                archive.Serialize("LaunchSatellite", value.LaunchSatellite);
                archive.Serialize<bool?>("EnableDisableSatellites", value.EnableDisableSatellites);
            }
        }
    }
}
