using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Engine.Serialization;

namespace Game
{
    internal class GameState
    {
        public uint CalculateHash()
        {
            if (this.Data is string)
            {
                return Crc32.ComputeChecksum(Encoding.UTF8.GetBytes((string)this.Data));
            }
            return Crc32.ComputeChecksum((byte[])this.Data);
        }

        public int CalculateSize()
        {
            if (this.Data is string)
            {
                return ((string)this.Data).Length;
            }
            return ((byte[])this.Data).Length;
        }

        public static GameState FromGame(Game game, bool forceBinary)
        {
            if (Message.UseXmlSerialization && !forceBinary)
            {
                XmlOutputArchive xmlOutputArchive = new XmlOutputArchive(new XElement("Game"), 0);
                xmlOutputArchive.Serialize<Game>(null, game);
                return new GameState
                {
                    StepIndex = game.StepModule.StepIndex,
                    Data = xmlOutputArchive.Node.ToString()
                };
            }
            MemoryStream memoryStream = new MemoryStream();
            new BinaryOutputArchive(memoryStream, 0)
            {
                Use7BitInts = true
            }.Serialize<Game>(null, game);
            return new GameState
            {
                StepIndex = game.StepModule.StepIndex,
                Data = memoryStream.ToArray()
            };
        }

        public static Game ToGame(GameState state)
        {
            if (state.Data is string)
            {
                return new XmlInputArchive(XElement.Parse((string)state.Data), 0).Serialize<Game>(null);
            }
            return new BinaryInputArchive(new MemoryStream((byte[])state.Data), 0)
            {
                Use7BitInts = true
            }.Serialize<Game>(null);
        }

        public int StepIndex;

        public object Data;

        private class Serializer : ISerializer<GameState>
        {
            public void Serialize(InputArchive archive, ref GameState value)
            {
                value = new GameState();
                archive.Serialize("StepIndex", ref value.StepIndex);
                archive.Serialize<object>("Data", ref value.Data);
            }

            public void Serialize(OutputArchive archive, GameState value)
            {
                archive.Serialize("StepIndex", value.StepIndex);
                archive.Serialize<object>("Data", value.Data);
            }
        }
    }
}
