using System;
using Engine;

namespace Game
{
    internal class CapturePlanetParticleSystem : ParticleSystem<CapturePlanetParticleSystem.Particle1>
    {
        public CapturePlanetParticleSystem(Vector2 position, float radius, Color color) : base(120, 1)
        {
            this.Position = position;
            this.Radius = radius;
            this.Color = color;
            Texture = Textures.Explosion;
            TextureSlotsCount = 1;
        }

        public override bool Run(float dt)
        {
            dt = MathUtils.Clamp(dt, 0f, 0.1f);
            float s = MathUtils.Pow(0.09f, dt);
            this.Duration += dt;
            this.ToGenerate += 400f * dt;
            bool flag = false;
            for (int i = 0; i < Particles.Length; i++)
            {
                CapturePlanetParticleSystem.Particle1 particle = Particles[i];
                if (particle.IsActive)
                {
                    flag = true;
                    particle.TimeToLive -= dt;
                    if (particle.TimeToLive > 0f)
                    {
                        particle.Position += particle.Velocity * dt;
                        particle.Velocity *= s;
                        particle.Color = particle.BaseColor * MathUtils.Saturate(0.5f * particle.TimeToLive);
                    }
                    else
                    {
                        particle.IsActive = false;
                    }
                }
                else if (this.Duration < 0.7f)
                {
                    flag = true;
                    if (this.ToGenerate >= 1f)
                    {
                        this.ToGenerate -= 1f;
                        particle.IsActive = true;
                        float num = this.Random.Float(0.1f, 1f);
                        Vector2 v = this.Random.Vector2(1f);
                        particle.Position = this.Position + this.Radius * v;
                        particle.BaseColor = this.Color;
                        particle.Size = new Vector2(MathUtils.Lerp(36f, 24f, num));
                        particle.TimeToLive = MathUtils.Lerp(3f, 1.5f, num) + this.Random.Float(-0.5f, 1f);
                        particle.Velocity = 1300f * num * v + 0f * this.Random.Vector2(1f);
                        particle.UseAdditiveBlending = true;
                    }
                }
            }
            return !flag;
        }

        private Engine.Random Random = new Engine.Random();

        private Vector2 Position;

        private float Radius;

        private Color Color;

        private float Duration;

        private float ToGenerate;

        public class Particle1 : Particle
        {
            public Vector2 Velocity;

            public float TimeToLive;

            public Color BaseColor;
        }
    }
}
