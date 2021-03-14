using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Comms;
using Engine;

namespace Game
{
    internal static class ServersManager
    {
        public static bool IsDiscoveryStarted { get; private set; }

        public static ReadOnlyList<ServerDescription> DiscoveredServers
        {
            get
            {
                return new ReadOnlyList<ServerDescription>(InternalDiscoveredServers);
            }
        }

        public static Version? NewVersionServerDiscovered { get; private set; }

        public static void StartServerDiscovery(double internetDiscoveryPeriod = 3.0, double localDiscoveryPeriod = 1.0)
        {
            LocalDiscoveryPeriod = localDiscoveryPeriod;
            InternetDiscoveryPeriod = internetDiscoveryPeriod;
            if (!IsDiscoveryStarted)
            {
                IsDiscoveryStarted = true;
                try
                {
                    CreatePeer();
                    ServerAddresses.Clear();
                    InternalDiscoveredServers.Clear();
                    NewVersionServerDiscovered = null;
                    LastLocalDiscoveryTime = double.MinValue;
                    LastInternetDiscoveryTime = double.MinValue;
                }
                catch (Exception arg)
                {
                    Log.Warning(string.Format("Unable to start server discovery. Reason: {0}", arg));
                }
            }
        }

        public static void StopServerDiscovery()
        {
            if (IsDiscoveryStarted)
            {
                IsDiscoveryStarted = false;
                DisposePeer();
            }
        }

        public static void Update()
        {
            if (IsDiscoveryStarted && Time.FrameStartTime >= LastLocalDiscoveryTime + LocalDiscoveryPeriod)
            {
                LastLocalDiscoveryTime = Time.FrameStartTime;
                SendLocalDiscoveryRequest();
            }
            if (IsDiscoveryStarted && Time.FrameStartTime >= LastInternetDiscoveryTime + InternetDiscoveryPeriod)
            {
                LastInternetDiscoveryTime = Time.FrameStartTime;
                Task.Run(delegate ()
                {
                    List<IPEndPoint> addresses = DnsQueryServerAddresses();
                    Dispatcher.Dispatch(delegate
                    {
                        SendInternetDiscoveryRequests(addresses);
                    }, false);
                });
            }
            if (Time.PeriodicEvent(0.25, 0.0))
            {
                InternalDiscoveredServers.RemoveAll((ServerDescription s) => Time.FrameStartTime > s.DiscoveryTime + (s.IsLocal ? 3 : 7));
            }
        }

        private static List<IPEndPoint> DnsQueryServerAddresses()
        {
            List<IPEndPoint> serverAddresses = ServerAddresses;
            List<IPEndPoint> result;
            lock (serverAddresses)
            {
                double realTime = Time.RealTime;
                if (ServerAddresses.Count == 0)
                {
                    Task<List<IPEndPoint>>[] array = new Task<List<IPEndPoint>>[ServerNames.Length];
                    for (int i = 0; i < ServerNames.Length; i++)
                    {
                        int num = i;
                        string name = ServerNames[num];
                        array[num] = Task.Run<List<IPEndPoint>>(delegate ()
                        {
                            List<IPEndPoint> list = new List<IPEndPoint>();
                            try
                            {
                                foreach (IPAddress ipaddress in Dns.GetHostEntry(name).AddressList)
                                {
                                    if (ipaddress.AddressFamily == AddressFamily.InterNetwork || ipaddress.AddressFamily == AddressFamily.InterNetworkV6)
                                    {
                                        list.Add(new IPEndPoint(ipaddress, 40102));
                                    }
                                }
                            }
                            catch
                            {
                            }
                            return list;
                        });
                    }
                    Task[] tasks = array;
                    Task.WaitAll(tasks, 5000);
                    ServerAddresses = (from t in array
                                                      select t.Result).SelectMany((List<IPEndPoint> l) => l).ToList<IPEndPoint>();
                    Log.Information("Servers DNS query completed: " + ServerAddresses.Aggregate("", (string s, IPEndPoint a) => s = s + a.ToString() + " "));
                }
                result = ServerAddresses.ToList<IPEndPoint>();
            }
            return result;
        }

        private static void CreatePeer()
        {
            DisposePeer();
            Peer = new Peer(new DiagnosticPacketTransmitter(new UdpPacketTransmitter(0)));
            Peer.Error += delegate (Exception e)
            {
                Log.Error(e);
            };
            Peer.PeerDiscovered += delegate (Packet p)
            {
                if (p.Data.Length >= 5 && p.Data[0] == 0)
                {
                    Version version = new Version(BitConverter.ToInt32(p.Data, 1));
                    if (version.GetNetworkProtocolVersion() > VersionsManager.Version.GetNetworkProtocolVersion() && (NewVersionServerDiscovered == null || version > NewVersionServerDiscovered.Value))
                    {
                        NewVersionServerDiscovered = new Version?(version);
                        return;
                    }
                    return;
                }
                else
                {
                    GameListMessage gameListMessage = Message.Read(p.Data) as GameListMessage;
                    if (gameListMessage != null)
                    {
                        Handle(gameListMessage, p.Address);
                        return;
                    }
                    throw new InvalidOperationException("Unrecognized message.");
                }
            };
        }

        private static void DisposePeer()
        {
            Peer peer = Peer;
            Peer = null;
            Task.Run(delegate ()
            {
                Peer peer1 = peer;
                if (peer1 == null)
                {
                    return;
                }
                peer1.Dispose();
            });
        }

        private static void SendLocalDiscoveryRequest()
        {
            if (Peer != null)
            {
                Peer.DiscoverLocalPeers(40102, VersionsManager.Version.ToByteArray());
                GameListLocalRequestTime = Time.RealTime;
            }
        }

        private static void SendInternetDiscoveryRequests(IEnumerable<IPEndPoint> addresses)
        {
            if (Peer != null)
            {
                foreach (IPEndPoint ipendPoint in addresses)
                {
                    Peer.DiscoverPeer(ipendPoint, VersionsManager.Version.ToByteArray());
                    GameListRequestTimes[ipendPoint] = Time.RealTime;
                }
            }
        }

        private static void Handle(GameListMessage message, IPEndPoint address)
        {
            bool isLocal = false;
            float ping = float.PositiveInfinity;
            double num;
            if (GameListRequestTimes.TryGetValue(address, out num))
            {
                ping = (float)(Time.RealTime - num);
                isLocal = false;
            }
            else if (GameListLocalRequestTime != 0.0)
            {
                ping = (float)(Time.RealTime - GameListLocalRequestTime);
                isLocal = true;
            }
            Dispatcher.Dispatch(delegate
            {
                InternalDiscoveredServers.RemoveAll((ServerDescription sd) => Equals(sd.Address, address));
                ServerDescription discoveredServer = new ServerDescription
                {
                    Address = address,
                    IsLocal = isLocal,
                    DiscoveryTime = Time.RealTime,
                    Ping = ping,
                    Priority = message.ServerPriority,
                    Name = message.ServerName
                };
                discoveredServer.GameDescriptions.AddRange(message.GameDescriptions.Select(delegate (GameDescription gd)
                {
                    gd.ServerDescription = discoveredServer;
                    return gd;
                }));
                InternalDiscoveredServers.Add(discoveredServer);
            }, false);
        }

        private static string[] ServerNames = new string[]
{
            "ruthlessconquest.kaalus.com",
            "ruthlessconquest1.kaalus.com",
            "ruthlessconquest2.kaalus.com",
            "ruthlessconquest3.kaalus.com"
};

        private static Peer Peer;

        private static List<IPEndPoint> ServerAddresses = new List<IPEndPoint>();

        private static double LastLocalDiscoveryTime = double.MinValue;

        private static double LastInternetDiscoveryTime = double.MinValue;

        private static double LocalDiscoveryPeriod;

        private static double InternetDiscoveryPeriod;

        private static DynamicArray<ServerDescription> InternalDiscoveredServers = new DynamicArray<ServerDescription>();

        private static Dictionary<IPEndPoint, double> GameListRequestTimes = new Dictionary<IPEndPoint, double>();

        private static double GameListLocalRequestTime;
    }
}
