using System;
using Engine;
using Engine.Graphics;
using Engine.Serialization;

namespace Game
{
    internal class GameImage
    {
        public static GameImage FromGame(Game game)
        {
            GameImage gameImage = new GameImage();
            gameImage.StepIndex = game.StepModule.StepIndex;
            foreach (Planet planet in game.PlanetsModule.Planets)
            {
                gameImage.PlanetImages.Add(new PlanetImage
                {
                    Position = new Point2(planet.Position.X / Scale, planet.Position.Y / Scale),
                    SizeClass = planet.SizeClass,
                    Faction = planet.Faction
                });
            }
            return gameImage;
        }

        public void Draw(RenderTarget2D renderTarget, bool hidePlanetOwnership)
        {
            RenderTarget2D renderTarget2 = Display.RenderTarget;
            try
            {
                Display.RenderTarget = renderTarget;
                Display.Clear(new Color?(Color.Transparent), null, null);
                PrimitivesRenderer2D primitivesRenderer2D = new PrimitivesRenderer2D();
                TexturedBatch2D texturedBatch2D = primitivesRenderer2D.TexturedBatch(Textures.Planet, false, 0, null, null, null, null);
                foreach (PlanetImage planetImage in this.PlanetImages)
                {
                    float v = 1.7f * Planet.GetRadius(planetImage.SizeClass);
                    Color color = Planet.GetColor(planetImage.Faction);
                    if (hidePlanetOwnership && planetImage.Faction != Faction.Neutral)
                    {
                        color = Color.White;
                    }
                    Point2 p = planetImage.Position * Scale;
                    texturedBatch2D.QueueQuad(new Vector2(p) - new Vector2(v), new Vector2(p) + new Vector2(v), 0f, Vector2.Zero, Vector2.One, color);
                }
                Vector2 vector = new Vector2(renderTarget.Width, renderTarget.Height) / new Vector2(Game.WorldSize);
                primitivesRenderer2D.Flush(Matrix.CreateScale(vector.X, vector.Y, 1f) * PrimitivesRenderer2D.ViewportMatrix(), true, int.MaxValue);
            }
            finally
            {
                Display.RenderTarget = renderTarget2;
            }
        }

        public Texture2D Draw(Point2 size, bool hidePlanetOwnership)
        {
            RenderTarget2D renderTarget2D = new RenderTarget2D(size.X, size.Y, 1, ColorFormat.Rgba8888, DepthFormat.None);
            this.Draw(renderTarget2D, hidePlanetOwnership);
            return renderTarget2D;
        }

        private static int Scale = (int)MathUtils.Ceiling(MathUtils.Max(Game.WorldSize.X, Game.WorldSize.Y) / 127f);

        public int StepIndex;

        public DynamicArray<PlanetImage> PlanetImages = new DynamicArray<PlanetImage>();

        private class Serializer : ISerializer<GameImage>
        {
            public void Serialize(InputArchive archive, ref GameImage value)
            {
                value = new GameImage();
                archive.Serialize("StepIndex", ref value.StepIndex);
                archive.Serialize<DynamicArray<PlanetImage>>("PlanetImages", ref value.PlanetImages);
            }

            public void Serialize(OutputArchive archive, GameImage value)
            {
                archive.Serialize("StepIndex", value.StepIndex);
                archive.Serialize<DynamicArray<PlanetImage>>("PlanetImages", value.PlanetImages);
            }
        }
    }
}
