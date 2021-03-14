using System;
using Engine.Serialization;

namespace Game
{
    internal struct PlayerDescription
    {
        public Faction Faction;

        public PlayerType Type;

        public string Name;

        public Guid Guid;

        public Platform? Platform;

        private class Serializer : ISerializer<PlayerDescription>
        {
            public void Serialize(InputArchive archive, ref PlayerDescription value)
            {
                archive.Serialize<Faction>("Faction", ref value.Faction);
                archive.Serialize<PlayerType>("Type", ref value.Type);
                archive.Serialize("Name", ref value.Name);
                archive.Serialize<Guid>("Guid", ref value.Guid);
                archive.Serialize<Platform?>("Platform", ref value.Platform);
            }

            public void Serialize(OutputArchive archive, PlayerDescription value)
            {
                archive.Serialize<Faction>("Faction", value.Faction);
                archive.Serialize<PlayerType>("Type", value.Type);
                archive.Serialize("Name", value.Name);
                archive.Serialize<Guid>("Guid", value.Guid);
                archive.Serialize<Platform?>("Platform", value.Platform);
            }
        }
    }
}
