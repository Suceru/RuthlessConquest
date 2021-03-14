using System;
using Engine;
using Engine.Graphics;
using Engine.Serialization;

namespace Game
{
    internal abstract class Player : Entity
    {
        public Faction Faction
        {
            get
            {
                return this.Description.Faction;
            }
        }

        public PlayerType Type
        {
            get
            {
                return this.Description.Type;
            }
        }

        public string Name
        {
            get
            {
                return this.Description.Name;
            }
        }

        public Guid Guid
        {
            get
            {
                return this.Description.Guid;
            }
        }

        public Platform? Platform
        {
            get
            {
                return this.Description.Platform;
            }
        }

        public int FactionProductionFactor { get; private set; } = 100;

        public bool IsControllingPlayer
        {
            get
            {
                return PlayersModule.ControllingPlayer == this;
            }
        }

        public FactionStatus Status
        {
            get
            {
                return PlayersModule.GetFactionStatus(this.Faction);
            }
        }

        public double StatusChangeTime
        {
            get
            {
                return PlayersModule.GetFactionStatusChangeTime(this.Faction);
            }
        }

        public bool AreSatellitesEnabled { get; set; } = true;

        protected Player()
        {
        }

        public Player(PlayerDescription description)
        {
            this.Description = description;
        }

        public override void Serialize(InputArchive archive)
        {
            archive.Serialize<PlayerDescription>("Description", ref this.Description);
            this.FactionProductionFactor = archive.Serialize<int>("FactionProductionFactor");
            this.AreSatellitesEnabled = archive.Serialize<bool>("AreSatellitesEnabled");
        }

        public override void Serialize(OutputArchive archive)
        {
            archive.Serialize<PlayerDescription>("Description", this.Description);
            archive.Serialize("FactionProductionFactor", this.FactionProductionFactor);
            archive.Serialize("AreSatellitesEnabled", this.AreSatellitesEnabled);
        }

        public virtual void Step()
        {
            int factionProductionPercentage = PlanetsModule.GetFactionProductionPercentage(this.Faction);
            this.FactionProductionFactor = MathUtils.Clamp(150 - factionProductionPercentage * 3 / 2, 50, 100);
        }

        public virtual void Update()
        {
        }

        public virtual void Draw(Color colorTransform)
        {
        }

        public static string GetPlayerTypeName(PlayerType playerType)
        {
            switch (playerType)
            {
                case PlayerType.Human:
                    return "Human";
                case PlayerType.NoviceAI:
                    return "Novice";
                case PlayerType.EasyAI:
                    return "Easy";
                case PlayerType.ModerateAI:
                    return "Moderate";
                case PlayerType.HardAI:
                    return "Hard";
                case PlayerType.VeryHardAI:
                    return "Very Hard";
                case PlayerType.BrutalAI:
                    return "Brutal";
                default:
                    throw new InvalidOperationException("Unknown player type.");
            }
        }

        public static Player CreatePlayer(PlayerDescription playerDescription)
        {
            switch (playerDescription.Type)
            {
                case PlayerType.Human:
                    return new HumanPlayer(playerDescription);
                case PlayerType.NoviceAI:
                    return new RandomAIPlayer(playerDescription, 0);
                case PlayerType.EasyAI:
                    return new RandomAIPlayer(playerDescription, 1);
                case PlayerType.ModerateAI:
                    return new RandomAIPlayer(playerDescription, 2);
                case PlayerType.HardAI:
                    return new WiseAIPlayer(playerDescription, 0);
                case PlayerType.VeryHardAI:
                    return new WiseAIPlayer(playerDescription, 1);
                case PlayerType.BrutalAI:
                    return new WiseAIPlayer(playerDescription, 2);
                default:
                    throw new InvalidOperationException("Unknown player type.");
            }
        }

        public static Color GetColor(Faction faction)
        {
            if (faction == Faction.None)
            {
                return Color.Gray;
            }
            return Planet.GetColor(faction);
        }

        public static Texture2D GetPlatformTexture(Platform? platform)
        {
            if (platform != null)
            {
                switch (platform.GetValueOrDefault())
                {
                    case global::Game.Platform.Desktop:
                        return Textures.Gui.LaptopLogo;
                    case global::Game.Platform.Android:
                        return Textures.Gui.AndroidLogo;
                    case global::Game.Platform.Ios:
                        return Textures.Gui.AppleLogo;
                    case global::Game.Platform.Amazon:
                        return Textures.Gui.AmazonLogo;
                }
            }
            return null;
        }

        private PlayerDescription Description;
    }
}
