using System;
using Engine.Serialization;

namespace Game
{
    internal class RefusedMessage : Message
    {
        public string Reason;

        private class Serializer : ISerializer<RefusedMessage>
        {
            public void Serialize(InputArchive archive, ref RefusedMessage value)
            {
                archive.Serialize("Reason", ref value.Reason);
            }

            public void Serialize(OutputArchive archive, RefusedMessage value)
            {
                archive.Serialize("Reason", value.Reason);
            }
        }
    }
}
