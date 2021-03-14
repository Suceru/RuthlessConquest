using System;
using Engine;

namespace Game
{
    internal class Particle
    {
        public bool IsActive;

        public Vector2 Position;

        public Vector2 Size;

        public float Rotation;

        public Color Color;

        public int TextureSlot;

        public bool UseAdditiveBlending;

        public bool FlipX;

        public bool FlipY;
    }
}
