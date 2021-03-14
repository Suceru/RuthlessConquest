using System;
using Engine;

namespace Game
{
    internal class BodyGrid
    {
        public BodyGrid(Point2 gridSize, int gridCellSize)
        {
            this.GridSize = gridSize;
            this.GridCellSize = gridCellSize;
            this.GridCount = this.GridSize / this.GridCellSize;
            this.Cells = new DynamicArray<Body>[this.GridCount.X * this.GridCount.Y];
            for (int i = 0; i < this.Cells.Length; i++)
            {
                this.Cells[i] = new DynamicArray<Body>();
            }
        }

        public void Add(Body body)
        {
            Rectangle rectangle = this.CalculateRectangle(body.Position, body.GridRadius);
            for (int i = rectangle.Top; i < rectangle.Bottom; i++)
            {
                for (int j = rectangle.Left; j < rectangle.Right; j++)
                {
                    this.GetGridCell(new Point2(j, i)).Add(body);
                }
            }
        }

        public void Clear()
        {
            DynamicArray<Body>[] cells = this.Cells;
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].Clear();
            }
        }

        public void QueryBodies(Point2 position, int radius, DynamicArray<Body> result, int maxCount)
        {
            Rectangle rectangle = this.CalculateRectangle(position, radius);
            int num = 0;
            for (int i = rectangle.Top; i < rectangle.Bottom; i++)
            {
                for (int j = rectangle.Left; j < rectangle.Right; j++)
                {
                    foreach (Body body in this.GetGridCell(new Point2(j, i)))
                    {
                        if (num >= maxCount)
                        {
                            break;
                        }
                        if (body.GridQueryIndex != this.QueryIndex && IntMath.ApproxDistance(body.Position, position) < radius + body.GridRadius)
                        {
                            result.Add(body);
                            body.GridQueryIndex = this.QueryIndex;
                            num++;
                        }
                    }
                }
            }
            this.QueryIndex++;
        }

        private Rectangle CalculateRectangle(Point2 position, int radius)
        {
            int num = MathUtils.Max((position.X - radius) / this.GridCellSize, 0);
            int num2 = MathUtils.Max((position.Y - radius) / this.GridCellSize, 0);
            int num3 = MathUtils.Min((position.X + radius) / this.GridCellSize, this.GridCount.X - 1);
            int num4 = MathUtils.Min((position.Y + radius) / this.GridCellSize, this.GridCount.Y - 1);
            return new Rectangle(num, num2, num3 - num + 1, num4 - num2 + 1);
        }

        private DynamicArray<Body> GetGridCell(Point2 p)
        {
            return this.Cells[p.X + p.Y * this.GridCount.X];
        }

        private Point2 GridCount;

        private DynamicArray<Body>[] Cells;

        private int QueryIndex;

        public readonly Point2 GridSize;

        public readonly int GridCellSize;
    }
}
