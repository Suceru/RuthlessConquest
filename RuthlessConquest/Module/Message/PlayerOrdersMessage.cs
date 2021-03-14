using System;
using Engine;
using Engine.Serialization;

namespace Game
{
    /// <summary>
    /// 保护类
    /// 玩家命令消息
    /// 继承:消息
    /// </summary>
    internal class PlayerOrdersMessage : Message
    {
        /// <summary>
        /// 命令表
        /// </summary>
        public DynamicArray<Order> Orders = new DynamicArray<Order>();

        private class Serializer : ISerializer<PlayerOrdersMessage>
        {
            public void Serialize(InputArchive archive, ref PlayerOrdersMessage value)
            {
                archive.Serialize<DynamicArray<Order>>("Orders", ref value.Orders);
            }

            public void Serialize(OutputArchive archive, PlayerOrdersMessage value)
            {
                archive.Serialize<DynamicArray<Order>>("Orders", value.Orders);
            }
        }
    }
}
