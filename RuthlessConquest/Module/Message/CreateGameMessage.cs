using System;
using Engine.Serialization;

namespace Game
{
    /// <summary>
    /// 类
    /// 创建游戏消息
    /// 继承:消息
    /// </summary>
    internal class CreateGameMessage : Message
    {
        /// <summary>
        /// 游戏创建参数
        /// </summary>
        public GameCreationParameters CreationParameters;

        /// <summary>
        /// 类
        /// 串行（化）器
        /// 继承:串行（化）器接口
        /// </summary>
        private class Serializer : ISerializer<CreateGameMessage>
        {
            /// <summary>
            /// 方法
            /// 串行
            /// </summary>
            /// <param name="archive">输入档案</param>
            /// <param name="value">创建游戏消息</param>
            public void Serialize(InputArchive archive, ref CreateGameMessage value)
            {
                //将游戏创建参数串行为:游戏创建参数类
                archive.Serialize<GameCreationParameters>("CreationParameters", ref value.CreationParameters);
            }
            /// <summary>
            /// 方法
            /// 串行
            /// </summary>
            /// <param name="archive">输出档案</param>
            /// <param name="value">创建游戏消息</param>
            public void Serialize(OutputArchive archive, CreateGameMessage value)
            {
                //将游戏创建参数串行为:游戏创建参数类
                archive.Serialize<GameCreationParameters>("CreationParameters", value.CreationParameters);
            }
        }
    }
}
