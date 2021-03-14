using System;
using Comms;
using Engine.Serialization;

namespace Game
{
    /// <summary>
    /// 保护类
    /// 加入游戏消息
    /// 继承:消息
    /// </summary>
    internal class JoinGameMessage : Message
    {
        /// <summary>
        /// 游戏ID
        /// </summary>
        public int GameId;
        /// <summary>
        /// 玩家名称
        /// </summary>
        public string PlayerName;
        /// <summary>
        /// 玩家全局标识
        /// </summary>
        public Guid PlayerGuid;
        /// <summary>
        /// 玩家平台
        /// </summary>
        public Platform PlayerPlatform;
        /// <summary>
        /// 观战功能
        /// </summary>
        public Faction PreferredFaction;
        /// <summary>
        /// 发送器
        /// </summary>
        public PeerData Sender;
        /// <summary>
        /// 接受时间
        /// </summary>
        public double ReceiveTime;
        /// <summary>
        /// 私有类
        /// 串行器
        /// 继承:串行器接口
        /// </summary>
        private class Serializer : ISerializer<JoinGameMessage>
        {
            /// <summary>
            /// 方法
            /// 串行
            /// </summary>
            /// <param name="archive">输入档案</param>
            /// <param name="value">加入游戏消息</param>
            public void Serialize(InputArchive archive, ref JoinGameMessage value)
            {
                //游戏ID
                archive.Serialize("GameId", ref value.GameId);
                //玩家名称
                archive.Serialize("PlayerName", ref value.PlayerName);
                //玩家全局标识
                archive.Serialize<Guid>("PlayerGuid", ref value.PlayerGuid);
                //玩家平台
                archive.Serialize<Platform>("PlayerPlatform", ref value.PlayerPlatform);
                //观战功能
                archive.Serialize<Faction>("PreferredFaction", ref value.PreferredFaction);
            }
            /// <summary>
            /// 方法
            /// 串行
            /// </summary>
            /// <param name="archive">输出档案</param>
            /// <param name="value">加入游戏消息</param>
            public void Serialize(OutputArchive archive, JoinGameMessage value)
            {
                //游戏ID
                archive.Serialize("GameId", value.GameId);
                //玩家名称
                archive.Serialize("PlayerName", value.PlayerName);
                //玩家全局标识
                archive.Serialize<Guid>("PlayerGuid", value.PlayerGuid);
                //玩家平台
                archive.Serialize<Platform>("PlayerPlatform", value.PlayerPlatform);
                //观战功能
                archive.Serialize<Faction>("PreferredFaction", value.PreferredFaction);
            }
        }
    }
}
