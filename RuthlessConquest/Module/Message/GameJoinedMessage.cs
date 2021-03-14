using System;
using Engine;
using Engine.Serialization;

namespace Game
{
    internal class GameJoinedMessage : Message
    {
        public GameState GameState;

        public DynamicArray<TickMessage> TickMessages;

        private class Serializer : ISerializer<GameJoinedMessage>
        {
            public void Serialize(InputArchive archive, ref GameJoinedMessage value)
            {
                archive.Serialize<GameState>("GameState", ref value.GameState);
                archive.Serialize<DynamicArray<TickMessage>>("TickMessages", ref value.TickMessages);
            }

            public void Serialize(OutputArchive archive, GameJoinedMessage value)
            {
                archive.Serialize<GameState>("GameState", value.GameState);
                archive.Serialize<DynamicArray<TickMessage>>("TickMessages", value.TickMessages);
            }
        }
    }
}
