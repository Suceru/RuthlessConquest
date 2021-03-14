using System;
using Engine.Serialization;

namespace Game
{
    /// <summary>
    /// 保护函数
    /// 开始游戏消息
    /// 继承:消息
    /// </summary>
    internal class StartGameMessage : Message
    {
        /// <summary>
        /// 私有类
        /// 串行器
        /// 继承:串行器接口
        /// </summary>
        private class Serializer : ISerializer<StartGameMessage>
        {
            /// <summary>
            /// 方法
            /// 串行
            /// </summary>
            /// <param name="archive">输入档案</param>
            /// <param name="value">开始游戏消息</param>
            public void Serialize(InputArchive archive, ref StartGameMessage value)
            {
            }
            /// <summary>
            /// 方法
            /// 串行
            /// </summary>
            /// <param name="archive">输出档案</param>
            /// <param name="value">开始游戏消息</param>
            public void Serialize(OutputArchive archive, StartGameMessage value)
            {
            }
        }
    }
}
