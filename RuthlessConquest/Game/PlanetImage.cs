using System;
using Engine;
using Engine.Serialization;

namespace Game
{
    internal struct PlanetImage
    {
        public Point2 Position;

        public int SizeClass;

        public Faction Faction;

        private class Serializer : ISerializer<PlanetImage>
        {
            public void Serialize(InputArchive archive, ref PlanetImage value)
            {
                archive.Serialize<Point2>("Position", ref value.Position);
                archive.Serialize("SizeClass", ref value.SizeClass);
                archive.Serialize<Faction>("Faction", ref value.Faction);
            }

            public void Serialize(OutputArchive archive, PlanetImage value)
            {
                archive.Serialize<Point2>("Position", value.Position);
                archive.Serialize("SizeClass", value.SizeClass);
                archive.Serialize<Faction>("Faction", value.Faction);
            }
        }
    }
}
