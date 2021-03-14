using System;
using Engine;
using Engine.Serialization;

namespace Game
{
    internal class Body
    {
        public Point2 Position { get; set; }

        public int GridRadius { get; set; }

        public int RepelFactor { get; set; }

        public int GridQueryIndex = -1;

        public object Tag;

        protected class Serializer : ISerializer<Body>
        {
            public void Serialize(InputArchive archive, ref Body value)
            {
                value.Position = archive.Serialize<Point2>("Position");
            }

            public void Serialize(OutputArchive archive, Body value)
            {
                archive.Serialize<Point2>("Position", value.Position);
            }
        }
    }
}
