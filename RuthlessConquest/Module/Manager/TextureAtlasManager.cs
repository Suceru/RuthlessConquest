using System;
using System.Collections.Generic;
using System.Globalization;
using Engine;
using Engine.Content;
using Engine.Graphics;

namespace Game
{
    public static class TextureAtlasManager
    {
        public static void LoadAtlases()
        {
        }

        public static Subtexture GetSubtexture(string name)
        {
            Subtexture subtexture;
            if (!m_subtextures.TryGetValue(name, out subtexture))
            {
                try
                {
                    subtexture = new Subtexture(ContentCache.Get<Texture2D>(name, true), Vector2.Zero, Vector2.One);
                    m_subtextures.Add(name, subtexture);
                }
                catch (Exception innerException)
                {
                    throw new InvalidOperationException(string.Format("Required subtexture {0} not found in TextureAtlasManager.", name), innerException);
                }
            }
            return subtexture;
        }

        private static void LoadTextureAtlas(Texture2D texture, string atlasDefinition, string prefix)
        {
            string[] array = atlasDefinition.Split(new char[]
            {
                '\n',
                '\r'
            }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < array.Length; i++)
            {
                string[] array2 = array[i].Split(new char[]
                {
                    ' '
                }, StringSplitOptions.RemoveEmptyEntries);
                if (array2.Length < 5)
                {
                    throw new InvalidOperationException("Invalid texture atlas definition.");
                }
                string key = prefix + array2[0];
                int num = int.Parse(array2[1], CultureInfo.InvariantCulture);
                int num2 = int.Parse(array2[2], CultureInfo.InvariantCulture);
                int num3 = int.Parse(array2[3], CultureInfo.InvariantCulture);
                int num4 = int.Parse(array2[4], CultureInfo.InvariantCulture);
                Vector2 topLeft = new Vector2(num / (float)texture.Width, num2 / (float)texture.Height);
                Vector2 bottomRight = new Vector2((num + num3) / (float)texture.Width, (num2 + num4) / (float)texture.Height);
                Subtexture value = new Subtexture(texture, topLeft, bottomRight);
                m_subtextures.Add(key, value);
            }
        }

        private static Dictionary<string, Subtexture> m_subtextures = new Dictionary<string, Subtexture>();
    }
}
