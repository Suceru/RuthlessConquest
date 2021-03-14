using System;
using System.Xml.Linq;
using Engine.Content;

namespace Game
{
    public static class Widgets
    {
        public static XElement BitmapButtonContents
        {
            get
            {
                return _BitmapButtonContents.Get();
            }
        }

        private static ContentCache.CachedItemWrapper<XElement> _BitmapButtonContents = new ContentCache.CachedItemWrapper<XElement>("Widgets/BitmapButtonContents");
    }
}
