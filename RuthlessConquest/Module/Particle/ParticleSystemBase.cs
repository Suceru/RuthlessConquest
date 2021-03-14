using System;
using Engine;

namespace Game
{
    internal abstract class ParticleSystemBase
    {
        public Game Game { get; set; }

        public int Layer { get; protected set; }

        public abstract void Draw(CameraModule cameraModule, Color colorTransform);

        public abstract bool Run(float dt);
    }
}
