using System;
using System.Collections.Generic;
using Engine;
using Engine.Serialization;

namespace Game
{
    internal class TickMessage : Message
    {
        public int Tick;

        public bool IsGameStarted;

        public SortedDictionary<Faction, DynamicArray<Order>> OrdersByFaction = new SortedDictionary<Faction, DynamicArray<Order>>();

        public DynamicArray<PlayerDescription> PlayerDescriptions = new DynamicArray<PlayerDescription>();

        private class Serializer : ISerializer<TickMessage>
        {
            public void Serialize(InputArchive archive, ref TickMessage value)
            {
                archive.Serialize("Tick", ref value.Tick);
                archive.Serialize("IsGameStarted", ref value.IsGameStarted);
                archive.Serialize<SortedDictionary<Faction, DynamicArray<Order>>>("OrdersByFaction", ref value.OrdersByFaction);
                archive.Serialize<DynamicArray<PlayerDescription>>("PlayerDescriptions", ref value.PlayerDescriptions);
            }

            public void Serialize(OutputArchive archive, TickMessage value)
            {
                archive.Serialize("Tick", value.Tick);
                archive.Serialize("IsGameStarted", value.IsGameStarted);
                archive.Serialize<SortedDictionary<Faction, DynamicArray<Order>>>("OrdersByFaction", value.OrdersByFaction);
                archive.Serialize<DynamicArray<PlayerDescription>>("PlayerDescriptions", value.PlayerDescriptions);
            }
        }
    }
}
