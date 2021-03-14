using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    internal class ParticlesModule : Module
    {
        public ReadOnlyList<ParticleSystemBase> ParticleSystems
        {
            get
            {
                return new ReadOnlyList<ParticleSystemBase>(this.InternalParticleSystems);
            }
        }

        public ParticlesModule(Game game) : base(game)
        {
        }

        public void AddParticleSystem(ParticleSystemBase particleSystem)
        {
            if (particleSystem.Game != null)
            {
                throw new InvalidOperationException();
            }
            this.InternalParticleSystems.Add(particleSystem);
            particleSystem.Game = Game;
        }

        public void RemoveParticleSystem(ParticleSystemBase particleSystem)
        {
            if (particleSystem.Game != Game)
            {
                throw new InvalidOperationException();
            }
            this.InternalParticleSystems.Remove(particleSystem);
            particleSystem.Game = null;
        }

        public void Draw(Color colorTransform, int layer)
        {
            this.TmpParticleSystemList.Clear();
            foreach (ParticleSystemBase particleSystemBase in this.InternalParticleSystems)
            {
                if (particleSystemBase.Layer == layer)
                {
                    this.TmpParticleSystemList.Add(particleSystemBase);
                }
            }
            foreach (ParticleSystemBase particleSystemBase2 in this.TmpParticleSystemList)
            {
                if (particleSystemBase2.Run(Time.FrameDuration))
                {
                    this.RemoveParticleSystem(particleSystemBase2);
                }
                else
                {
                    particleSystemBase2.Draw(CameraModule, colorTransform);
                }
            }
            CameraModule.PrimitivesRenderer.Flush(CameraModule.WorldToScreenMatrix * PrimitivesRenderer2D.ViewportMatrix(), true, int.MaxValue);
        }

        private DynamicArray<ParticleSystemBase> InternalParticleSystems = new DynamicArray<ParticleSystemBase>();

        private DynamicArray<ParticleSystemBase> TmpParticleSystemList = new DynamicArray<ParticleSystemBase>();
    }
}
