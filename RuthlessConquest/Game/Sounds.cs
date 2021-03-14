using System;
using Engine.Audio;
using Engine.Content;
using Engine.Media;

namespace Game
{
    public static class Sounds
    {
        public static SoundBuffer Slider
        {
            get
            {
                return _Slider.Get();
            }
        }

        public static SoundBuffer Countdown
        {
            get
            {
                return _Countdown.Get();
            }
        }

        public static SoundBuffer Click
        {
            get
            {
                return _Click.Get();
            }
        }

        public static SoundBuffer Click2
        {
            get
            {
                return _Click2.Get();
            }
        }

        public static SoundBuffer Select
        {
            get
            {
                return _Select.Get();
            }
        }

        public static SoundBuffer Deselect
        {
            get
            {
                return _Deselect.Get();
            }
        }

        public static SoundBuffer Order
        {
            get
            {
                return _Order.Get();
            }
        }

        public static SoundBuffer Message
        {
            get
            {
                return _Message.Get();
            }
        }

        public static SoundBuffer PlanetLost
        {
            get
            {
                return _PlanetLost.Get();
            }
        }

        public static SoundBuffer PlanetWon
        {
            get
            {
                return _PlanetWon.Get();
            }
        }

        public static SoundBuffer Laser
        {
            get
            {
                return _Laser.Get();
            }
        }

        public static SoundBuffer Start
        {
            get
            {
                return _Start.Get();
            }
        }

        public static SoundBuffer Spawn
        {
            get
            {
                return _Spawn.Get();
            }
        }

        public static SoundBuffer SatelliteLaunch
        {
            get
            {
                return _SatelliteLaunch.Get();
            }
        }

        public static SoundBuffer Victory
        {
            get
            {
                return _Victory.Get();
            }
        }

        public static SoundBuffer Defeat
        {
            get
            {
                return _Defeat.Get();
            }
        }

        public static SoundBuffer Aliens
        {
            get
            {
                return _Aliens.Get();
            }
        }

        public static SoundBuffer Reinforcements
        {
            get
            {
                return _Reinforcements.Get();
            }
        }

        public static SoundBuffer Startup
        {
            get
            {
                return _Startup.Get();
            }
        }

        public static SoundBuffer Shutdown
        {
            get
            {
                return _Shutdown.Get();
            }
        }

        public static StreamingSource SpeedOfLight
        {
            get
            {
                return _SpeedOfLight.Get();
            }
        }

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Slider = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Slider");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Countdown = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Countdown");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Click = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Click");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Click2 = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Click2");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Select = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Select");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Deselect = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Deselect");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Order = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Order");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Message = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Message");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _PlanetLost = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/PlanetLost");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _PlanetWon = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/PlanetWon");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Laser = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Laser");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Start = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Start");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Spawn = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Spawn");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _SatelliteLaunch = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/SatelliteLaunch");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Victory = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Victory");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Defeat = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Defeat");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Aliens = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Aliens");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Reinforcements = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Reinforcements");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Startup = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Startup");

        private static ContentCache.CachedItemWrapper<SoundBuffer> _Shutdown = new ContentCache.CachedItemWrapper<SoundBuffer>("Sounds/Shutdown");

        private static ContentCache.CachedItemWrapper<StreamingSource> _SpeedOfLight = new ContentCache.CachedItemWrapper<StreamingSource>("Sounds/SpeedOfLight");
    }
}
