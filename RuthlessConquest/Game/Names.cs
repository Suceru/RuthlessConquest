using System;
using Engine.Content;

namespace Game
{
    public static class Names
    {
        public static string Prefixes
        {
            get
            {
                return _Prefixes.Get();
            }
        }

        public static string Adjectives
        {
            get
            {
                return _Adjectives.Get();
            }
        }

        public static string Nouns
        {
            get
            {
                return _Nouns.Get();
            }
        }

        public static string Forbidden
        {
            get
            {
                return _Forbidden.Get();
            }
        }

        public static string ForbiddenAnywhere
        {
            get
            {
                return _ForbiddenAnywhere.Get();
            }
        }

        private static ContentCache.CachedItemWrapper<string> _Prefixes = new ContentCache.CachedItemWrapper<string>("Names/Prefixes");

        private static ContentCache.CachedItemWrapper<string> _Adjectives = new ContentCache.CachedItemWrapper<string>("Names/Adjectives");

        private static ContentCache.CachedItemWrapper<string> _Nouns = new ContentCache.CachedItemWrapper<string>("Names/Nouns");

        private static ContentCache.CachedItemWrapper<string> _Forbidden = new ContentCache.CachedItemWrapper<string>("Names/Forbidden");

        private static ContentCache.CachedItemWrapper<string> _ForbiddenAnywhere = new ContentCache.CachedItemWrapper<string>("Names/ForbiddenAnywhere");
    }
}
