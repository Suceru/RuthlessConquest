using System;
using Engine.Serialization;

namespace Game
{
    internal class EnumsSerializer : ISerializer<Faction>, ISerializer<Platform>, ISerializer<PlayerType>
    {
        public void Serialize(InputArchive archive, ref Faction value)
        {
            int num = 0;
            archive.Serialize(null, ref num);
            value = (Faction)num;
        }

        public void Serialize(OutputArchive archive, Faction value)
        {
            archive.Serialize(null, (int)value);
        }

        public void Serialize(InputArchive archive, ref Platform value)
        {
            int num = 0;
            archive.Serialize(null, ref num);
            value = (Platform)num;
        }

        public void Serialize(OutputArchive archive, Platform value)
        {
            archive.Serialize(null, (int)value);
        }

        public void Serialize(InputArchive archive, ref PlayerType value)
        {
            int num = 0;
            archive.Serialize(null, ref num);
            value = (PlayerType)num;
        }

        public void Serialize(OutputArchive archive, PlayerType value)
        {
            archive.Serialize(null, (int)value);
        }
    }
}
