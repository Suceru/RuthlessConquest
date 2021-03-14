using System;
using Engine.Content;
using Engine.Media;

namespace Game
{
    public static class Fonts
    {
        public static BitmapFont Normal
        {
            get
            {
                return _Normal.Get();
            }
        }

        public static BitmapFont Small
        {
            get
            {
                return _Small.Get();
            }
        }

        private static ContentCache.CachedItemWrapper<BitmapFont> _Normal = new ContentCache.CachedItemWrapper<BitmapFont>("Fonts/Normal");

        private static ContentCache.CachedItemWrapper<BitmapFont> _Small = new ContentCache.CachedItemWrapper<BitmapFont>("Fonts/Small");
    }
}
