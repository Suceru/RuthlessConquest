using System;
using Engine.Serialization;

namespace Game
{
    internal class GameImageMessage : Message
    {
        public GameImage GameImage;

        private class Serializer : ISerializer<GameImageMessage>
        {
            public void Serialize(InputArchive archive, ref GameImageMessage value)
            {
                archive.Serialize<GameImage>("GameImage", ref value.GameImage);
            }

            public void Serialize(OutputArchive archive, GameImageMessage value)
            {
                archive.Serialize<GameImage>("GameImage", value.GameImage);
            }
        }
    }
}
