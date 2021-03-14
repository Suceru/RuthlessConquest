using System;
using Engine.Content;
using Engine.Graphics;

namespace Game
{
    public static class Textures
    {
        public static Texture2D Ship1
        {
            get
            {
                return _Ship1.Get();
            }
        }

        public static Texture2D Ship2
        {
            get
            {
                return _Ship2.Get();
            }
        }

        public static Texture2D Ship3
        {
            get
            {
                return _Ship3.Get();
            }
        }

        public static Texture2D Ship4
        {
            get
            {
                return _Ship4.Get();
            }
        }

        public static Texture2D Ship5
        {
            get
            {
                return _Ship5.Get();
            }
        }

        public static Texture2D Ship6
        {
            get
            {
                return _Ship6.Get();
            }
        }

        public static Texture2D Ship7
        {
            get
            {
                return _Ship7.Get();
            }
        }

        public static Texture2D Planet
        {
            get
            {
                return _Planet.Get();
            }
        }

        public static Texture2D Planet2
        {
            get
            {
                return _Planet2.Get();
            }
        }

        public static Texture2D Explosion
        {
            get
            {
                return _Explosion.Get();
            }
        }

        public static Texture2D Exhaust
        {
            get
            {
                return _Exhaust.Get();
            }
        }

        public static Texture2D Star
        {
            get
            {
                return _Star.Get();
            }
        }

        public static Texture2D StarTwinkle
        {
            get
            {
                return _StarTwinkle.Get();
            }
        }

        public static Texture2D Skull
        {
            get
            {
                return _Skull.Get();
            }
        }

        private static ContentCache.CachedItemWrapper<Texture2D> _Ship1 = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Ship1");

        private static ContentCache.CachedItemWrapper<Texture2D> _Ship2 = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Ship2");

        private static ContentCache.CachedItemWrapper<Texture2D> _Ship3 = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Ship3");

        private static ContentCache.CachedItemWrapper<Texture2D> _Ship4 = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Ship4");

        private static ContentCache.CachedItemWrapper<Texture2D> _Ship5 = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Ship5");

        private static ContentCache.CachedItemWrapper<Texture2D> _Ship6 = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Ship6");

        private static ContentCache.CachedItemWrapper<Texture2D> _Ship7 = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Ship7");

        private static ContentCache.CachedItemWrapper<Texture2D> _Planet = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Planet");

        private static ContentCache.CachedItemWrapper<Texture2D> _Planet2 = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Planet2");

        private static ContentCache.CachedItemWrapper<Texture2D> _Explosion = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Explosion");

        private static ContentCache.CachedItemWrapper<Texture2D> _Exhaust = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Exhaust");

        private static ContentCache.CachedItemWrapper<Texture2D> _Star = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Star");

        private static ContentCache.CachedItemWrapper<Texture2D> _StarTwinkle = new ContentCache.CachedItemWrapper<Texture2D>("Textures/StarTwinkle");

        private static ContentCache.CachedItemWrapper<Texture2D> _Skull = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Skull");

        public static class Gui
        {
            public static Texture2D EngineLogo
            {
                get
                {
                    return _EngineLogo.Get();
                }
            }

            public static Texture2D CandyRufusLogo
            {
                get
                {
                    return _CandyRufusLogo.Get();
                }
            }

            public static Texture2D Logo
            {
                get
                {
                    return _Logo.Get();
                }
            }

            public static Texture2D Panel
            {
                get
                {
                    return _Panel.Get();
                }
            }

            public static Texture2D PlayerPanel
            {
                get
                {
                    return _PlayerPanel.Get();
                }
            }

            public static Texture2D Menu
            {
                get
                {
                    return _Menu.Get();
                }
            }

            public static Texture2D Tick
            {
                get
                {
                    return _Tick.Get();
                }
            }

            public static Texture2D Ships
            {
                get
                {
                    return _Ships.Get();
                }
            }

            public static Texture2D Boid1
            {
                get
                {
                    return _Boid1.Get();
                }
            }

            public static Texture2D Boid2
            {
                get
                {
                    return _Boid2.Get();
                }
            }

            public static Texture2D Boid3
            {
                get
                {
                    return _Boid3.Get();
                }
            }

            public static Texture2D Boid4
            {
                get
                {
                    return _Boid4.Get();
                }
            }

            public static Texture2D Boid5
            {
                get
                {
                    return _Boid5.Get();
                }
            }

            public static Texture2D Boid6
            {
                get
                {
                    return _Boid6.Get();
                }
            }

            public static Texture2D PadCursor
            {
                get
                {
                    return _PadCursor.Get();
                }
            }

            public static Texture2D PadCursorDown
            {
                get
                {
                    return _PadCursorDown.Get();
                }
            }

            public static Texture2D PadCursorDrag
            {
                get
                {
                    return _PadCursorDrag.Get();
                }
            }

            public static Texture2D SurvivalcraftIcon
            {
                get
                {
                    return _SurvivalcraftIcon.Get();
                }
            }

            public static Texture2D Survivalcraft2Icon
            {
                get
                {
                    return _Survivalcraft2Icon.Get();
                }
            }

            public static Texture2D BugsIcon
            {
                get
                {
                    return _BugsIcon.Get();
                }
            }

            public static Texture2D AndroidLogo
            {
                get
                {
                    return _AndroidLogo.Get();
                }
            }

            public static Texture2D AppleLogo
            {
                get
                {
                    return _AppleLogo.Get();
                }
            }

            public static Texture2D AmazonLogo
            {
                get
                {
                    return _AmazonLogo.Get();
                }
            }

            public static Texture2D LaptopLogo
            {
                get
                {
                    return _LaptopLogo.Get();
                }
            }

            public static Texture2D Satellite
            {
                get
                {
                    return _Satellite.Get();
                }
            }

            private static ContentCache.CachedItemWrapper<Texture2D> _EngineLogo = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/EngineLogo");

            private static ContentCache.CachedItemWrapper<Texture2D> _CandyRufusLogo = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/CandyRufusLogo");

            private static ContentCache.CachedItemWrapper<Texture2D> _Logo = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/Logo");

            private static ContentCache.CachedItemWrapper<Texture2D> _Panel = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/Panel");

            private static ContentCache.CachedItemWrapper<Texture2D> _PlayerPanel = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/PlayerPanel");

            private static ContentCache.CachedItemWrapper<Texture2D> _Menu = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/Menu");

            private static ContentCache.CachedItemWrapper<Texture2D> _Tick = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/Tick");

            private static ContentCache.CachedItemWrapper<Texture2D> _Ships = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/Ships");

            private static ContentCache.CachedItemWrapper<Texture2D> _Boid1 = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/Boid1");

            private static ContentCache.CachedItemWrapper<Texture2D> _Boid2 = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/Boid2");

            private static ContentCache.CachedItemWrapper<Texture2D> _Boid3 = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/Boid3");

            private static ContentCache.CachedItemWrapper<Texture2D> _Boid4 = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/Boid4");

            private static ContentCache.CachedItemWrapper<Texture2D> _Boid5 = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/Boid5");

            private static ContentCache.CachedItemWrapper<Texture2D> _Boid6 = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/Boid6");

            private static ContentCache.CachedItemWrapper<Texture2D> _PadCursor = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/PadCursor");

            private static ContentCache.CachedItemWrapper<Texture2D> _PadCursorDown = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/PadCursorDown");

            private static ContentCache.CachedItemWrapper<Texture2D> _PadCursorDrag = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/PadCursorDrag");

            private static ContentCache.CachedItemWrapper<Texture2D> _SurvivalcraftIcon = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/SurvivalcraftIcon");

            private static ContentCache.CachedItemWrapper<Texture2D> _Survivalcraft2Icon = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/Survivalcraft2Icon");

            private static ContentCache.CachedItemWrapper<Texture2D> _BugsIcon = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/BugsIcon");

            private static ContentCache.CachedItemWrapper<Texture2D> _AndroidLogo = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/AndroidLogo");

            private static ContentCache.CachedItemWrapper<Texture2D> _AppleLogo = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/AppleLogo");

            private static ContentCache.CachedItemWrapper<Texture2D> _AmazonLogo = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/AmazonLogo");

            private static ContentCache.CachedItemWrapper<Texture2D> _LaptopLogo = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/LaptopLogo");

            private static ContentCache.CachedItemWrapper<Texture2D> _Satellite = new ContentCache.CachedItemWrapper<Texture2D>("Textures/Gui/Satellite");
        }
    }
}
