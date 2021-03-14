using System;
using Engine.Serialization;

namespace Game
{
    internal class GameMember : ISerializable
    {
        protected GameMember(Game game = null)
        {
            this.Game = game;
        }

        public Game Game { get; set; }

        protected StepModule StepModule
        {
            get
            {
                return this.Game.StepModule;
            }
        }

        protected PlayersModule PlayersModule
        {
            get
            {
                return this.Game.PlayersModule;
            }
        }

        protected RouteFinderModule RouteFinderModule
        {
            get
            {
                return this.Game.RouteFinderModule;
            }
        }

        protected BodiesModule BodiesModule
        {
            get
            {
                return this.Game.BodiesModule;
            }
        }

        protected PlanetsModule PlanetsModule
        {
            get
            {
                return this.Game.PlanetsModule;
            }
        }

        protected ShipsModule ShipsModule
        {
            get
            {
                return this.Game.ShipsModule;
            }
        }

        protected SpecialEventsModule SpecialEventsModule
        {
            get
            {
                return this.Game.SpecialEventsModule;
            }
        }

        protected ParticlesModule ParticlesModule
        {
            get
            {
                return this.Game.ParticlesModule;
            }
        }

        protected CameraModule CameraModule
        {
            get
            {
                return this.Game.CameraModule;
            }
        }

        protected MusicModule MusicModule
        {
            get
            {
                return this.Game.MusicModule;
            }
        }

        public virtual void Serialize(InputArchive archive)
        {
        }

        public virtual void Serialize(OutputArchive archive)
        {
        }
    }
}
