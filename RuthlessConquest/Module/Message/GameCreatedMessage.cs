using System;
using Engine.Serialization;

namespace Game
{
    internal class GameCreatedMessage : Message
    {
        public int GameId;

        public GameCreationParameters CreationParameters;

        private class Serializer : ISerializer<GameCreatedMessage>
        {
            public void Serialize(InputArchive archive, ref GameCreatedMessage value)
            {
                archive.Serialize("GameId", ref value.GameId);
                archive.Serialize<GameCreationParameters>("CreationParameters", ref value.CreationParameters);
            }

            public void Serialize(OutputArchive archive, GameCreatedMessage value)
            {
                archive.Serialize("GameId", value.GameId);
                archive.Serialize<GameCreationParameters>("CreationParameters", value.CreationParameters);
            }
        }
    }
}
