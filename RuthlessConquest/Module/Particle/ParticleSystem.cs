using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    internal class ParticleSystem<T> : ParticleSystemBase where T : Particle, new()
    {
        protected ParticleSystem(int particlesCount, int layer)
        {
            Layer = layer;
            this.Particles = new T[particlesCount];
            for (int i = 0; i < this.Particles.Length; i++)
            {
                this.Particles[i] = Activator.CreateInstance<T>();
            }
        }

        protected T[] Particles { get; }

        protected Texture2D Texture
        {
            get
            {
                return this.InternalTexture;
            }
            set
            {
                if (value != this.InternalTexture)
                {
                    this.InternalTexture = value;
                    this.AdditiveBatch = null;
                    this.AlphaBlendedBatch = null;
                }
            }
        }

        protected int TextureSlotsCount { get; set; }

        public override void Draw(CameraModule cameraModule, Color colorTransform)
        {
            if (this.AdditiveBatch == null || this.AlphaBlendedBatch == null)
            {
                this.AdditiveBatch = cameraModule.PrimitivesRenderer.TexturedBatch(this.InternalTexture, true, 0, null, null, BlendState.Additive, SamplerState.LinearClamp);
                this.AlphaBlendedBatch = cameraModule.PrimitivesRenderer.TexturedBatch(this.InternalTexture, true, 0, null, null, BlendState.AlphaBlend, SamplerState.LinearClamp);
            }
            float s = 1f / TextureSlotsCount;
            for (int i = 0; i < this.Particles.Length; i++)
            {
                Particle particle = this.Particles[i];
                if (particle.IsActive)
                {
                    Vector2 position = particle.Position;
                    Vector2 size = particle.Size;
                    float rotation = particle.Rotation;
                    int textureSlot = particle.TextureSlot;
                    Vector2 p;
                    Vector2 p2;
                    Vector2 p3;
                    Vector2 p4;
                    if (rotation != 0f)
                    {
                        Vector2 vector = new Vector2(MathUtils.Cos(rotation), MathUtils.Sin(rotation));
                        Vector2 vector2 = Vector2.Perpendicular(vector);
                        vector2 *= size.Y;
                        vector *= size.X;
                        p = position + (-vector2 - vector);
                        p2 = position + (vector2 - vector);
                        p3 = position + (vector2 + vector);
                        p4 = position + (-vector2 + vector);
                    }
                    else
                    {
                        Vector2 vector3 = Vector2.UnitX * size.X;
                        Vector2 v = Vector2.UnitY * size.Y;
                        p = position + (-vector3 - v);
                        p2 = position + (vector3 - v);
                        p3 = position + (vector3 + v);
                        p4 = position + (-vector3 + v);
                    }
                    TexturedBatch2D texturedBatch2D = particle.UseAdditiveBlending ? this.AdditiveBatch : this.AlphaBlendedBatch;
                    Vector2 v2 = new Vector2(textureSlot % this.TextureSlotsCount, textureSlot / this.TextureSlotsCount);
                    float num = 0f;
                    float num2 = 1f;
                    float num3 = 1f;
                    float num4 = 0f;
                    if (particle.FlipX)
                    {
                        num = 1f - num;
                        num2 = 1f - num2;
                    }
                    if (particle.FlipY)
                    {
                        num3 = 1f - num3;
                        num4 = 1f - num4;
                    }
                    Vector2 texCoord = (v2 + new Vector2(num, num3)) * s;
                    Vector2 texCoord2 = (v2 + new Vector2(num2, num3)) * s;
                    Vector2 texCoord3 = (v2 + new Vector2(num2, num4)) * s;
                    Vector2 texCoord4 = (v2 + new Vector2(num, num4)) * s;
                    texturedBatch2D.QueueQuad(p, p2, p3, p4, 0f, texCoord, texCoord2, texCoord3, texCoord4, (colorTransform == Color.White) ? particle.Color : (particle.Color * colorTransform));
                }
            }
        }

        public override bool Run(float dt)
        {
            return false;
        }

        private Texture2D InternalTexture;

        private TexturedBatch2D AdditiveBatch;

        private TexturedBatch2D AlphaBlendedBatch;
    }
}
