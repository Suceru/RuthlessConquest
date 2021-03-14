using System;
using Engine.Serialization;

namespace Game
{
    /// <summary>
    /// 保护类
    /// 游戏描述
    /// </summary>
    internal class GameDescription
    {
        /// <summary>
        /// 服务器描述
        /// </summary>
        public ServerDescription ServerDescription;

        public int GameId;

        public int HumanPlayersCount;

        public int SpectatorsCount;

        public int TicksSinceStart;

        public GameCreationParameters CreationParameters;

        public GameImage GameImage;

        private class Serializer : ISerializer<GameDescription>
        {
            public void Serialize(InputArchive archive, ref GameDescription value)
            {
                value = new GameDescription();
                archive.Serialize("GameId", ref value.GameId);
                archive.Serialize("HumanPlayersCount", ref value.HumanPlayersCount);
                archive.Serialize("SpectatorsCount", ref value.SpectatorsCount);
                archive.Serialize("TicksSinceStart", ref value.TicksSinceStart);
                archive.Serialize<GameCreationParameters>("CreationParameters", ref value.CreationParameters);
                archive.Serialize<GameImage>("GameImage", ref value.GameImage);
            }

            public void Serialize(OutputArchive archive, GameDescription value)
            {
                archive.Serialize("GameId", value.GameId);
                archive.Serialize("HumanPlayersCount", value.HumanPlayersCount);
                archive.Serialize("SpectatorsCount", value.SpectatorsCount);
                archive.Serialize("TicksSinceStart", value.TicksSinceStart);
                archive.Serialize<GameCreationParameters>("CreationParameters", value.CreationParameters);
                archive.Serialize<GameImage>("GameImage", value.GameImage);
            }
        }
    }
}
