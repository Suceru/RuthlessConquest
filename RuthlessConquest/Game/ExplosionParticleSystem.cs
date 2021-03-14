using System;
using Engine;

namespace Game
{
    internal class ExplosionParticleSystem : ParticleSystem<ExplosionParticleSystem.Particle1>
    {
        public ExplosionParticleSystem(Vector2 position, Vector2 velocity, float density, float size, Color color) : base((int)(30f * density), 1)
        {
            Texture = Textures.Explosion;
            TextureSlotsCount = 1;
            for (int i = 0; i < Particles.Length; i++)
            {
                ExplosionParticleSystem.Particle1 particle = Particles[i];
                particle.IsActive = true;
                float num = this.Random.Float(0.2f, 1f);
                Vector2 v = this.Random.Vector2(num);
                particle.Position = position + 100f * v * size;
                particle.BaseColor = color;
                particle.Size = new Vector2(MathUtils.Lerp(30f, 24f, num));
                particle.TimeToLive = MathUtils.Lerp(2f, 1f, num) * this.Random.Float(0.4f, 1f);
                particle.Velocity = size * 1500f * v * this.Random.Float(0.5f, 1.2f) + velocity;
                particle.UseAdditiveBlending = true;
            }
        }

        public override bool Run(float dt)
        {
            dt = MathUtils.Clamp(dt, 0f, 0.1f);
            float s = MathUtils.Pow(0.02f, dt);
            bool flag = false;
            for (int i = 0; i < Particles.Length; i++)
            {
                ExplosionParticleSystem.Particle1 particle = Particles[i];
                if (particle.IsActive)
                {
                    flag = true;
                    particle.TimeToLive -= dt;
                    if (particle.TimeToLive > 0f)
                    {
                        particle.Position += particle.Velocity * dt;
                        particle.Velocity *= s;
                        particle.Color = particle.BaseColor * MathUtils.Saturate(0.8f * particle.TimeToLive);
                    }
                    else
                    {
                        particle.IsActive = false;
                    }
                }
            }
            return !flag;
        }

        private Engine.Random Random = new Engine.Random();

        public class Particle1 : Particle
        {
            public Vector2 Velocity;

            public float TimeToLive;

            public Color BaseColor;
        }
    }
}
