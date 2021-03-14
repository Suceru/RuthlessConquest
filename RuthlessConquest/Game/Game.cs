using System;
using System.Collections.Generic;
using Engine;
using Engine.Serialization;

namespace Game
{
    internal class Game : ISerializable
    {
        public event Action<Entity> EntityAdded;

        public event Action<Entity> EntityRemoved;

        public ReadOnlyList<Module> Modules
        {
            get
            {
                return new ReadOnlyList<Module>(this.InternalModules);
            }
        }

        public ReadOnlyList<Entity> Entities
        {
            get
            {
                return new ReadOnlyList<Entity>(this.InternalEntities);
            }
        }

        public StepModule StepModule { get; }

        public PlayersModule PlayersModule { get; }

        public RouteFinderModule RouteFinderModule { get; }

        public BodiesModule BodiesModule { get; }

        public PlanetsModule PlanetsModule { get; }

        public ShipsModule ShipsModule { get; }

        public SpecialEventsModule SpecialEventsModule { get; }

        public ParticlesModule ParticlesModule { get; }

        public CameraModule CameraModule { get; }

        public MusicModule MusicModule { get; }

        public GameScreen Screen { get; set; }

        public GameCreationParameters CreationParameters { get; private set; }

        public static Point2 WorldSize { get; } = new Point2(16000, 9000);

        private Game()
        {
            this.InternalModules.Add(this.StepModule = new StepModule(this));
            this.InternalModules.Add(this.PlayersModule = new PlayersModule(this));
            this.InternalModules.Add(this.BodiesModule = new BodiesModule(this));
            this.InternalModules.Add(this.PlanetsModule = new PlanetsModule(this));
            this.InternalModules.Add(this.ShipsModule = new ShipsModule(this));
            this.InternalModules.Add(this.RouteFinderModule = new RouteFinderModule(this));
            this.InternalModules.Add(this.SpecialEventsModule = new SpecialEventsModule(this));
            this.InternalModules.Add(this.ParticlesModule = new ParticlesModule(this));
            this.InternalModules.Add(this.CameraModule = new CameraModule(this));
            this.InternalModules.Add(this.MusicModule = new MusicModule(this));
        }

        public Game(GameCreationParameters creationParameters) : this()
        {
            this.CreationParameters = creationParameters;
            this.PlanetsModule.CreatePlanets();
        }

        public void Serialize(InputArchive archive)
        {
            this.CreationParameters = archive.Serialize<GameCreationParameters>("CreationParameters");
            foreach (Module module in this.Modules)
            {
                archive.Serialize(module.GetType().Name, module.GetType(), module);
            }
            List<Entity> list = new List<Entity>();
            archive.SerializeCollection<Entity>("Entities", list);
            foreach (Entity entity in list)
            {
                this.AddEntity(entity);
            }
        }

        public void Serialize(OutputArchive archive)
        {
            archive.Serialize<GameCreationParameters>("CreationParameters", this.CreationParameters);
            foreach (Module module in this.Modules)
            {
                archive.Serialize(module.GetType().Name, module.GetType(), module);
            }
            archive.SerializeCollection<Entity>("Entities", "Entity", this.Entities);
        }

        public void AddEntity(Entity entity)
        {
            this.AddEntity(this.InternalEntities.Count, entity);
        }

        public void AddEntity(int index, Entity entity)
        {
            if (entity.Game != null)
            {
                throw new InvalidOperationException();
            }
            this.InternalEntities.Insert(index, entity);
            entity.Game = this;
            Action<Entity> entityAdded = this.EntityAdded;
            if (entityAdded != null)
            {
                entityAdded(entity);
            }
            entity.OnAdded();
        }

        public void RemoveEntity(Entity entity)
        {
            if (entity.Game != this)
            {
                throw new InvalidOperationException();
            }
            Action<Entity> entityRemoved = this.EntityRemoved;
            if (entityRemoved != null)
            {
                entityRemoved(entity);
            }
            entity.OnRemoved();
            this.InternalEntities.Remove(entity);
            entity.Game = null;
        }
        /// <summary>
        /// 方法
        /// 更新消息
        /// </summary>
        public void Update()
        {
            this.CameraModule.Update();
            this.MusicModule.Update();
            this.PlayersModule.Update();
            this.StepModule.Update();
        }

        public void Draw(Color colorTransform)
        {
            this.PlayersModule.Draw(colorTransform);
            this.ParticlesModule.Draw(colorTransform, 0);
            this.ShipsModule.Draw(colorTransform);
            this.PlanetsModule.Draw(colorTransform);
            this.ParticlesModule.Draw(colorTransform, 1);
        }

        private DynamicArray<Module> InternalModules = new DynamicArray<Module>();

        private DynamicArray<Entity> InternalEntities = new DynamicArray<Entity>();
    }
}
