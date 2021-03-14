using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    public class Subtexture
    {
        public Subtexture(Texture2D texture, Vector2 topLeft, Vector2 bottomRight)
        {
            this.Texture = texture;
            this.TopLeft = topLeft;
            this.BottomRight = bottomRight;
        }

        public static implicit operator Subtexture(Texture2D texture)
        {
            if (texture == null)
            {
                return null;
            }
            return new Subtexture(texture, Vector2.Zero, Vector2.One);
        }

        public readonly Texture2D Texture;

        public readonly Vector2 TopLeft;

        public readonly Vector2 BottomRight;
    }
}
