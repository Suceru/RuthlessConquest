using System;

namespace Game
{
    /// <summary>
    /// 功能状态
    /// </summary>
    internal enum FactionStatus
    {
        /// <summary>
        /// 被击败
        /// </summary>
        LostEliminated = -2,
        /// <summary>
        /// 失败
        /// </summary>
        Lost,
        /// <summary>
        /// 无法判定
        /// </summary>
        Undecided,
        /// <summary>
        /// 胜利
        /// </summary>
        Won,
        /// <summary>
        /// 击败其他玩家
        /// </summary>
        WonEliminatedOthers
    }
}
