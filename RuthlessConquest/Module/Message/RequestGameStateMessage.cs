using System;
using Engine.Serialization;

namespace Game
{
    internal class RequestGameStateMessage : Message
    {
        private class Serializer : ISerializer<RequestGameStateMessage>
        {
            public void Serialize(InputArchive archive, ref RequestGameStateMessage value)
            {
            }

            public void Serialize(OutputArchive archive, RequestGameStateMessage value)
            {
            }
        }
    }
}
