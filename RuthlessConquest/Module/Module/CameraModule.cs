using System;
using System.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
    internal class CameraModule : Module
    {
        public Vector2 ViewCenter { get; set; }

        public float ViewScale { get; set; }

        public Matrix WorldToScreenMatrix { get; private set; }

        public Matrix ScreenToWorldMatrix { get; private set; }

        public PrimitivesRenderer2D PrimitivesRenderer { get; } = new PrimitivesRenderer2D();

        public CameraModule(Game game) : base(game)
        {
            this.ViewCenter = new Vector2(Game.WorldSize.X, Game.WorldSize.Y) / 2f;
            this.ViewScale = 1f;
        }

        public void Update()
        {
            if (this.StartingPlanet == null)
            {
                this.StartingPlanet = PlanetsModule.Planets.FirstOrDefault((Planet p) => p.IsControllingPlayerPlanet);
            }
            else
            {
                if (!StepModule.IsGameStarted)
                {
                    this.StartTime = Time.FrameStartTime;
                }
                double num = Time.FrameStartTime - this.StartTime;
                float num2 = (float)MathUtils.Saturate(0.30000001192092896 * (num - 1.0));
                num2 = MathUtils.Sigmoid(num2, 7f);
                this.ViewCenter = Vector2.Lerp(new Vector2(this.StartingPlanet.Position), new Vector2(Game.WorldSize) / 2f, num2);
                this.ViewScale = MathUtils.Pow(3f, 1f - num2);
            }
            GameWidget gameWidget = Game.Screen.GameWidget;
            BoundingRectangle boundingRectangle = new BoundingRectangle(Vector2.Zero, gameWidget.ActualSize);
            Vector2 vector = (boundingRectangle.Min + boundingRectangle.Max) / 2f;
            Vector2 vector2 = boundingRectangle.Max - boundingRectangle.Min;
            float x = vector2.X / Game.WorldSize.X;
            float x2 = vector2.Y / Game.WorldSize.Y;
            float num3 = MathUtils.Min(x, x2);
            this.WorldToScreenMatrix = Matrix.CreateTranslation(-this.ViewCenter.X, -this.ViewCenter.Y, 0f) * Matrix.CreateScale(this.ViewScale * num3, this.ViewScale * num3, 1f) * Matrix.CreateTranslation(vector.X, vector.Y, 0f) * gameWidget.GlobalTransform;
            this.ScreenToWorldMatrix = Matrix.Invert(this.WorldToScreenMatrix);
        }

        private double StartTime = double.MinValue;

        private Planet StartingPlanet;
    }
}
