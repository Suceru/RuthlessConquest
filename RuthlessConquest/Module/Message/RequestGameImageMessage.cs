using System;
using Engine.Serialization;

namespace Game
{
    internal class RequestGameImageMessage : Message
    {
        private class Serializer : ISerializer<RequestGameImageMessage>
        {
            public void Serialize(InputArchive archive, ref RequestGameImageMessage value)
            {
            }

            public void Serialize(OutputArchive archive, RequestGameImageMessage value)
            {
            }
        }
    }
}
