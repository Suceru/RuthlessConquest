using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    public class BoidsWidget : Widget
    {
        public BoidsWidget()
        {
            IsDrawRequired = true;
            this.IsHitTestVisible = false;
        }

        public override void Draw()
        {
            if (Time.FrameIndex > LastAnimateFrame)
            {
                LastAnimateFrame = Time.FrameIndex;
                Point2 point;
                point.X = ((Window.Size.Y > 0) ? (9000 * Window.Size.X / Window.Size.Y) : 9000);
                point.Y = 9000;
                if (Boids == null || Grid == null || Grid.GridSize != point)
                {
                    Boids = new DynamicArray<BoidsWidget.Boid>();
                    Grid = new BodyGrid(point, 250);
                    int num = 80;
                    for (int i = 0; i < num; i++)
                    {
                        BoidsWidget.Boid item = new BoidsWidget.Boid
                        {
                            Faction = i / 16,
                            Position = point / 2 + new Point2((int)(4000f * MathUtils.Sin(6.28f * i / num)), (int)(4000f * MathUtils.Cos(6.28f * i / num))),
                            Velocity = Random.Vector2(750f)
                        };
                        Boids.Add(item);
                    }
                }
                Grid.Clear();
                foreach (BoidsWidget.Boid body in Boids)
                {
                    Grid.Add(body);
                }
                foreach (BoidsWidget.Boid boid in Boids)
                {
                    boid.Update();
                }
            }
            Vector2 vector = new Vector2(Grid.GridSize.X / 2, Grid.GridSize.Y / 2);
            float x = Window.Size.X / (float)Grid.GridSize.X;
            float x2 = Window.Size.Y / (float)Grid.GridSize.Y;
            float num2 = MathUtils.Min(x, x2);
            Matrix m = Matrix.CreateTranslation(-vector.X, -vector.Y, 0f) * Matrix.CreateScale(num2, num2, 1f) * Matrix.CreateTranslation(Window.Size.X / 2f, Window.Size.Y / 2f, 0f);
            foreach (BoidsWidget.Boid boid2 in Boids)
            {
                boid2.Draw(this.PrimitivesRenderer);
            }
            this.PrimitivesRenderer.Flush(m * PrimitivesRenderer2D.ViewportMatrix(), true, int.MaxValue);
        }

        private static BodyGrid Grid;

        private static DynamicArray<BoidsWidget.Boid> Boids;

        private static DynamicArray<Body> TmpBodyList = new DynamicArray<Body>();

        private static Engine.Random Random = new Engine.Random();

        private static int LastAnimateFrame = -1;

        private static Color Color = Color.White * 0.2f;

        private PrimitivesRenderer2D PrimitivesRenderer = new PrimitivesRenderer2D();

        private class Boid : Body
        {
            public void Update()
            {
                float num = MathUtils.Clamp(Time.FrameDuration, 0f, 0.1f);
                TmpBodyList.Clear();
                Grid.QueryBodies(Position, 1000, TmpBodyList, 30);
                if (TmpBodyList.Count > 0)
                {
                    int num2 = 0;
                    int num3 = 0;
                    Vector2 vector = Vector2.Zero;
                    Vector2 vector2 = Vector2.Zero;
                    Vector2 vector3 = Vector2.Zero;
                    Vector2 vector4 = Vector2.Zero;
                    foreach (Body body in TmpBodyList)
                    {
                        BoidsWidget.Boid boid = (BoidsWidget.Boid)body;
                        Vector2 v = new Vector2(Position - boid.Position);
                        float num4 = v.Length();
                        Vector2 v2 = v / (num4 + 0.1f);
                        if (boid.Faction == this.Faction)
                        {
                            this.Velocity += 2000f * v2 * MathUtils.Lerp(1f, 0f, MathUtils.Saturate((num4 - 100f) / 300f)) * num;
                            vector += new Vector2(boid.Position);
                            vector2 += boid.Velocity;
                            num2++;
                        }
                        else
                        {
                            this.Velocity += 2000f * v2 * MathUtils.Lerp(1f, 0f, MathUtils.Saturate((num4 - 100f) / 900f)) * num;
                            vector3 += new Vector2(boid.Position);
                            vector4 += boid.Velocity;
                            num3++;
                        }
                    }
                    if (num2 > 0)
                    {
                        vector /= num2;
                        vector2 /= num2;
                        Vector2 v3 = Vector2.Normalize(vector - new Vector2(Position));
                        this.Velocity += 200f * v3 * num;
                        this.Velocity += 2f * vector2 * num;
                    }
                    if (num3 > 0)
                    {
                        vector3 /= num3;
                        vector4 /= num3;
                        Vector2 v4 = Vector2.Normalize(vector3 - new Vector2(Position));
                        this.Velocity -= 100f * v4 * num;
                        this.Velocity -= 1f * vector4 * num;
                    }
                    float num5 = 500f;
                    if (Position.X < num5)
                    {
                        this.Velocity += 10f * num * new Vector2(1f, 0f) * (num5 - Position.X);
                    }
                    if (Position.Y < num5)
                    {
                        this.Velocity += 10f * num * new Vector2(0f, 1f) * (num5 - Position.Y);
                    }
                    if (Position.X > Grid.GridSize.X - num5)
                    {
                        this.Velocity += 10f * num * new Vector2(-1f, 0f) * (Position.X - Grid.GridSize.X + num5);
                    }
                    if (Position.Y > Grid.GridSize.Y - num5)
                    {
                        this.Velocity += 10f * num * new Vector2(0f, -1f) * (Position.Y - Grid.GridSize.Y + num5);
                    }
                }
                float num6 = this.Velocity.Length();
                float num7 = 250f;
                float num8 = 500f;
                if (num6 > num8)
                {
                    this.Velocity *= num8 / num6;
                }
                if (num6 < num7)
                {
                    this.Velocity *= num7 / num6;
                }
                Position = new Point2(Position.X + (int)MathUtils.Round(this.Velocity.X * num), Position.Y + (int)MathUtils.Round(this.Velocity.Y * num));
            }

            public void Draw(PrimitivesRenderer2D primitivesRenderer)
            {
                TexturedBatch2D texturedBatch2D = primitivesRenderer.TexturedBatch(GetTexture(this.Faction), false, 0, null, null, null, null);
                Vector2 v = new Vector2(Position);
                Vector2 vector = Vector2.Normalize(this.Velocity) * 150f;
                Vector2 v2 = Vector2.Perpendicular(vector);
                Vector2 vector2 = v - v2 + vector;
                Vector2 p = v + v2 + vector;
                Vector2 vector3 = v + v2 - vector;
                Vector2 p2 = v - v2 - vector;
                texturedBatch2D.QueueTriangle(vector2, p, vector3, 0f, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(1f, 1f), Color);
                texturedBatch2D.QueueTriangle(vector3, p2, vector2, 0f, new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(0f, 0f), Color);
            }

            private static Texture2D GetTexture(int faction)
            {
                switch (faction)
                {
                    case 0:
                        return Textures.Gui.Boid1;
                    case 1:
                        return Textures.Gui.Boid2;
                    case 2:
                        return Textures.Gui.Boid3;
                    case 3:
                        return Textures.Gui.Boid4;
                    case 4:
                        return Textures.Gui.Boid5;
                    case 5:
                        return Textures.Gui.Boid6;
                    default:
                        return Textures.Gui.Boid1;
                }
            }

            public int Faction;

            public Vector2 Velocity;
        }
    }
}
