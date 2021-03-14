using System;
using Engine;
using Engine.Serialization;

namespace Game
{
    /// <summary>
    /// 保护类
    /// 游戏列表消息
    /// 继承:消息
    /// </summary>
    internal class GameListMessage : Message
    {
        /// <summary>
        /// 服务器优先级
        /// </summary>
        public int ServerPriority;
        /// <summary>
        /// 服务器名
        /// </summary>
        public string ServerName;
        /// <summary>
        /// 游戏描述
        /// </summary>
        public DynamicArray<GameDescription> GameDescriptions = new DynamicArray<GameDescription>();

        /// <summary>
        /// 私有类
        /// 串行器
        /// </summary>
        private class Serializer : ISerializer<GameListMessage>
        {
            public void Serialize(InputArchive archive, ref GameListMessage value)
            {
                archive.Serialize("ServerPriority", ref value.ServerPriority);
                archive.Serialize("ServerName", ref value.ServerName);
                archive.Serialize<DynamicArray<GameDescription>>("GameDescriptions", ref value.GameDescriptions);
            }

            public void Serialize(OutputArchive archive, GameListMessage value)
            {
                archive.Serialize("ServerPriority", value.ServerPriority);
                archive.Serialize("ServerName", value.ServerName);
                archive.Serialize<DynamicArray<GameDescription>>("GameDescriptions", value.GameDescriptions);
            }
        }
    }
}
