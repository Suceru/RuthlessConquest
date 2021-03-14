using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
    internal abstract class AStar<T>
    {
        protected abstract void Neighbors(T p, DynamicArray<T> neighbors);

        protected abstract int Cost(T p1, T p2);

        protected abstract int Heuristic(T p);

        protected virtual void StorageClear()
        {
            if (this.DefaultStorage == null)
            {
                this.DefaultStorage = new Dictionary<T, object>();
                return;
            }
            this.DefaultStorage.Clear();
        }

        protected virtual object StorageGet(T p)
        {
            object result;
            this.DefaultStorage.TryGetValue(p, out result);
            return result;
        }

        protected virtual void StorageAdd(T p, object data)
        {
            this.DefaultStorage.Add(p, data);
        }

        public bool FindPath(ICollection<T> path, T start, int maxPositionsToCheck = 2147483647)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            path.Clear();
            this.NodesCacheIndex = 0;
            this.OpenHeap.Clear();
            this.StorageClear();
            AStar<T>.Node node = this.NewNode(start, default(T), 0, 0);
            this.StorageAdd(start, node);
            this.HeapEnqueue(node);
            AStar<T>.Node node2 = null;
            int num = 0;
            AStar<T>.Node node3;
            for (; ; )
            {
                node3 = ((this.OpenHeap.Count > 0) ? this.HeapDequeue() : null);
                if (node3 == null || num >= maxPositionsToCheck)
                {
                    break;
                }
                if (this.Heuristic(node3.Position) <= 0)
                {
                    this.BuildPathFromEndNode(path, node, node3);
                    return true;
                }
                if (node3 != node && (node2 == null || node3.H < node2.H))
                {
                    node2 = node3;
                }
                node3.IsClosed = true;
                num++;
                this.NeighborsList.Clear();
                this.Neighbors(node3.Position, this.NeighborsList);
                for (int i = 0; i < this.NeighborsList.Count; i++)
                {
                    T t = this.NeighborsList[i];
                    AStar<T>.Node node4 = (AStar<T>.Node)this.StorageGet(t);
                    if (node4 == null || !node4.IsClosed)
                    {
                        int num2 = this.Cost(node3.Position, t);
                        if (num2 != 2147483647)
                        {
                            int num3 = node3.G + num2;
                            if (node4 != null)
                            {
                                if (num3 < node4.G)
                                {
                                    node4.G = num3;
                                    node4.F = num3 + node4.H;
                                    node4.PreviousPosition = node3.Position;
                                    this.HeapUpdate(node4);
                                }
                            }
                            else
                            {
                                int h = this.Heuristic(t);
                                node4 = this.NewNode(t, node3.Position, num3, h);
                                this.StorageAdd(t, node4);
                                this.HeapEnqueue(node4);
                            }
                        }
                    }
                }
            }
            if (node2 != null)
            {
                this.BuildPathFromEndNode(path, node, node2);
            }
            return false;

        }

        private void BuildPathFromEndNode(ICollection<T> path, AStar<T>.Node startNode, AStar<T>.Node endNode)
        {
            for (AStar<T>.Node node = endNode; node != startNode; node = (AStar<T>.Node)this.StorageGet(node.PreviousPosition))
            {
                path.Add(node.Position);
            }
        }

        private void HeapEnqueue(AStar<T>.Node node)
        {
            this.OpenHeap.Add(node);
            this.HeapifyFromPosToStart(this.OpenHeap.Count - 1);
        }

        private AStar<T>.Node HeapDequeue()
        {
            AStar<T>.Node result = this.OpenHeap[0];
            if (this.OpenHeap.Count <= 1)
            {
                this.OpenHeap.Clear();
                return result;
            }
            this.OpenHeap[0] = this.OpenHeap[this.OpenHeap.Count - 1];
            this.OpenHeap.RemoveAt(this.OpenHeap.Count - 1);
            this.HeapifyFromPosToEnd(0);
            return result;
        }

        private void HeapUpdate(AStar<T>.Node node)
        {
            int pos = -1;
            for (int i = 0; i < this.OpenHeap.Count; i++)
            {
                if (this.OpenHeap[i] == node)
                {
                    pos = i;
                    break;
                }
            }
            this.HeapifyFromPosToStart(pos);
        }

        private void HeapifyFromPosToEnd(int pos)
        {
            for (; ; )
            {
                int num = pos;
                int num2 = 2 * pos + 1;
                int num3 = 2 * pos + 2;
                if (num2 < this.OpenHeap.Count && this.OpenHeap[num2].F < this.OpenHeap[num].F)
                {
                    num = num2;
                }
                if (num3 < this.OpenHeap.Count && this.OpenHeap[num3].F < this.OpenHeap[num].F)
                {
                    num = num3;
                }
                if (num == pos)
                {
                    break;
                }
                AStar<T>.Node value = this.OpenHeap[num];
                this.OpenHeap[num] = this.OpenHeap[pos];
                this.OpenHeap[pos] = value;
                pos = num;
            }
        }

        private void HeapifyFromPosToStart(int pos)
        {
            int num;
            for (int i = pos; i > 0; i = num)
            {
                num = (i - 1) / 2;
                AStar<T>.Node node = this.OpenHeap[num];
                AStar<T>.Node node2 = this.OpenHeap[i];
                if (node.F <= node2.F)
                {
                    break;
                }
                this.OpenHeap[num] = node2;
                this.OpenHeap[i] = node;
            }
        }

        private AStar<T>.Node NewNode(T position, T previousPosition, int g, int h)
        {
            while (this.NodesCacheIndex >= this.NodesCache.Count)
            {
                this.NodesCache.Add(new AStar<T>.Node());
            }
            DynamicArray<AStar<T>.Node> nodesCache = this.NodesCache;
            int nodesCacheIndex = this.NodesCacheIndex;
            this.NodesCacheIndex = nodesCacheIndex + 1;
            AStar<T>.Node node = nodesCache[nodesCacheIndex];
            node.Position = position;
            node.PreviousPosition = previousPosition;
            node.F = g + h;
            node.G = g;
            node.H = h;
            node.IsClosed = false;
            return node;
        }

        private int NodesCacheIndex;

        private DynamicArray<AStar<T>.Node> NodesCache = new DynamicArray<AStar<T>.Node>();

        private DynamicArray<AStar<T>.Node> OpenHeap = new DynamicArray<AStar<T>.Node>();

        private DynamicArray<T> NeighborsList = new DynamicArray<T>();

        private Dictionary<T, object> DefaultStorage;

        private class Node
        {
            public T Position;

            public T PreviousPosition;

            public int F;

            public int G;

            public int H;

            public bool IsClosed;
        }
    }
}
