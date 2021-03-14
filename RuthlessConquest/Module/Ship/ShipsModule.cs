using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
    internal class ShipsModule : Module
    {
        public ReadOnlyList<Ship> Ships
        {
            get
            {
                return new ReadOnlyList<Ship>(this.InternalShips);
            }
        }

        public ShipsModule(Game game) : base(game)
        {
            game.EntityAdded += delegate (Entity e)
            {
                Ship ship = e as Ship;
                if (ship != null)
                {
                    this.InternalShips.Add(ship);
                }
            };
            game.EntityRemoved += delegate (Entity e)
            {
                Ship ship = e as Ship;
                if (ship != null)
                {
                    this.InternalShips.Remove(ship);
                }
            };
        }

        public void SendShips(Planet source, Planet destination, int count, Faction giftToFaction)
        {
            this.SendShips(source, new Planet[]
            {
                destination
            }, count, giftToFaction);
        }

        public void SendShips(Planet source, IEnumerable<Planet> route, int count, Faction giftToFaction)
        {
            count = MathUtils.Min(count, source.ShipsCount);
            if (count > 0 && route.Count<Planet>() > 0 && route.First<Planet>() != source)
            {
                Point2 p = route.First<Planet>().Position - source.Position;
                int num = IntMath.ApproxLength(p);
                Point2 p2 = (num > 0) ? (p * source.Radius / num / 2) : Point2.Zero;
                for (int i = 0; i < count; i++)
                {
                    Point2 position = source.Position + p2 + new Point2(StepModule.Random.Int(-100, 100), StepModule.Random.Int(-100, 100));
                    Game.AddEntity(new Ship(source.Faction, giftToFaction, position, source, route));
                }
                source.ShipsCount -= count;
                float volume = MathUtils.Lerp(0f, 0.5f, MathUtils.Saturate(0.01f * (count - 7)));
                float pitch = MathUtils.Lerp(1f, 0.5f, MathUtils.Saturate(0.003f * (count - 100)));
                float pan = MathUtils.Clamp(0.8f * (source.Position.X - Game.WorldSize.X / 2) / (Game.WorldSize.X / 2), -1f, 1f);
                if (volume > 0.01f)
                {
                    Time.QueueTimeDelayedExecution(Time.FrameStartTime + 0.2f * (source.GetHashCode() % 1000 / 1000f), delegate
                    {
                        AudioManager.PlaySound(Sounds.Spawn, true, volume, pitch, pan);
                    });
                }
            }
        }

        public void SendOutsideShips(Faction faction, Planet destination, int shipsCount)
        {
            Rectangle rectangle = new Rectangle(0, 0, Game.WorldSize.X, Game.WorldSize.Y);
            Point2 point;
            for (; ; )
            {
                point = new Point2(StepModule.Random.Int(-1000000, 1000000), StepModule.Random.Int(-1000000, 1000000));
                if (!rectangle.Contains(point))
                {
                    point.X = MathUtils.Clamp(point.X, rectangle.Left, rectangle.Right);
                    point.Y = MathUtils.Clamp(point.Y, rectangle.Top, rectangle.Bottom);
                    if (MathUtils.Abs(point.X - destination.Position.X) >= rectangle.Size.X / 3 && MathUtils.Abs(point.Y - destination.Position.Y) >= rectangle.Size.Y / 3 && IntMath.ApproxDistance(point, destination.Position) >= IntMath.ApproxLength(rectangle.Width, rectangle.Height) / 3)
                    {
                        break;
                    }
                }
            }
            Point2 point2 = point;
            Point2 point3 = Point2.Zero;
            if (point2.X <= rectangle.Left)
            {
                point3 = new Point2(-1, 0);
            }
            else if (point2.X >= rectangle.Right)
            {
                point3 = new Point2(1, 0);
            }
            else if (point2.Y <= rectangle.Top)
            {
                point3 = new Point2(0, -1);
            }
            else if (point2.Y >= rectangle.Bottom)
            {
                point3 = new Point2(0, 1);
            }
            Point2 p = new Point2(point3.Y, -point3.X);
            Planet[] route = new Planet[]
            {
                destination
            };
            for (int i = 0; i < shipsCount; i++)
            {
                Point2 point4 = point2;
                point4 += point3 * StepModule.Random.Int(5000, 5000 + shipsCount * 30);
                point4 += p * StepModule.Random.Int(-shipsCount * 20, shipsCount * 20);
                Game.AddEntity(new Ship(faction, Faction.None, point4, null, route));
            }
        }

        public void Draw(Color colorTransform)
        {
            foreach (Ship ship in this.Ships)
            {
                ship.Draw(colorTransform);
            }
            CameraModule.PrimitivesRenderer.Flush(CameraModule.WorldToScreenMatrix * PrimitivesRenderer2D.ViewportMatrix(), true, int.MaxValue);
        }

        public int CountFactionShips(Faction faction)
        {
            int num = (from p in PlanetsModule.Planets
                       where p.Faction == faction
                       select p).Sum((Planet p) => p.ShipsCount);
            int num2 = (from s in this.Ships
                        where s.Faction == faction
                        select s).Count<Ship>();
            return num + num2;
        }

        public bool TestReachability(Point2 p1, Point2 p2)
        {
            return IntMath.ApproxDistance(p1, p2) <= Game.CreationParameters.ShipRange;
        }

        public void Step()
        {
            this.TmpShipList.Clear();
            this.TmpShipList.AddRange(this.InternalShips);
            foreach (Ship ship in this.TmpShipList)
            {
                if (ship.Game == Game)
                {
                    ship.Step();
                }
            }
        }

        private DynamicArray<Ship> InternalShips = new DynamicArray<Ship>();

        private DynamicArray<Ship> TmpShipList = new DynamicArray<Ship>();
    }
}
