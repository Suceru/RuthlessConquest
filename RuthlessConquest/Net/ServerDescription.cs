using System;
using System.Net;
using Engine;

namespace Game
{
    internal class ServerDescription
    {
        /// <summary>
        /// 地址
        /// </summary>
        public IPEndPoint Address;
        /// <summary>
        /// 是否本地局域网
        /// </summary>
        public bool IsLocal;
        /// <summary>
        /// 发现时间
        /// </summary>
        public double DiscoveryTime;
        /// <summary>
        /// ping
        /// </summary>
        public float Ping;

        public int Priority;

        public string Name;

        public DynamicArray<GameDescription> GameDescriptions = new DynamicArray<GameDescription>();
    }
}
