using System;
using Engine.Serialization;

namespace Game
{
    internal class GameStateHashMessage : Message
    {
        public int StepIndex;

        public uint StateHash;

        public object State;

        private class Serializer : ISerializer<GameStateHashMessage>
        {
            public void Serialize(InputArchive archive, ref GameStateHashMessage value)
            {
                archive.Serialize("StepIndex", ref value.StepIndex);
                archive.Serialize("StateHash", ref value.StateHash);
                archive.Serialize<object>("State", ref value.State);
            }

            public void Serialize(OutputArchive archive, GameStateHashMessage value)
            {
                archive.Serialize("StepIndex", value.StepIndex);
                archive.Serialize("StateHash", value.StateHash);
                archive.Serialize<object>("State", value.State);
            }
        }
    }
}
