using System;
using Engine;

namespace Game
{
    internal static class IntMath
    {
        public static int ApproxLength(int dx, int dy)
        {
            if (dx < 0)
            {
                dx = -dx;
            }
            if (dy < 0)
            {
                dy = -dy;
            }
            int num;
            int num2;
            if (dx < dy)
            {
                num = dx;
                num2 = dy;
            }
            else
            {
                num = dy;
                num2 = dx;
            }
            int num3 = num2 * 1007 + num * 441;
            if (num2 < num << 4)
            {
                num3 -= num2 * 40;
            }
            return num3 + 512 >> 10;
        }

        public static int ApproxLength(Point2 p)
        {
            return ApproxLength(p.X, p.Y);
        }

        public static int ApproxDistance(Point2 p1, Point2 p2)
        {
            return ApproxLength(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point2 ApproxNormalize(Point2 p, int length)
        {
            return p * length / ApproxLength(p);
        }

        public static int LengthSquared(int dx, int dy)
        {
            return dx * dx + dy * dy;
        }

        public static int LengthSquared(Point2 p)
        {
            return LengthSquared(p.X, p.Y);
        }

        public static int Length(int dx, int dy)
        {
            return Sqrt(dx * dx + dy * dy);
        }

        public static int Length(Point2 p)
        {
            return Length(p.X, p.Y);
        }

        public static int DistanceSquared(Point2 p1, Point2 p2)
        {
            return LengthSquared(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static int Distance(Point2 p1, Point2 p2)
        {
            return Length(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static int Sqrt(int v)
        {
            if (v > 0)
            {
                int num = v / 2 + 1;
                for (int i = (num + v / num) / 2; i < num; i = (num + v / num) / 2)
                {
                    num = i;
                }
                return num;
            }
            return 0;
        }

        public static int Lerp(int x1, int y1, int x2, int y2, int x)
        {
            if (x1 != x2)
            {
                return (x * (y2 - y1) - x1 * y2 + x2 * y1) / (x2 - x1);
            }
            if (x < x1)
            {
                return y1;
            }
            if (x > x1)
            {
                return y2;
            }
            return (y1 + y2) / 2;
        }
    }
}
