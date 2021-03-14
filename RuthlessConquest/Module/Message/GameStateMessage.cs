using System;
using Engine;
using Engine.Serialization;

namespace Game
{
    internal class GameStateMessage : Message
    {
        public GameState GameState;

        public DynamicArray<Faction> NonDefeatedFactions;

        private class Serializer : ISerializer<GameStateMessage>
        {
            public void Serialize(InputArchive archive, ref GameStateMessage value)
            {
                archive.Serialize<GameState>("GameState", ref value.GameState);
                archive.Serialize<DynamicArray<Faction>>("NonDefeatedFactions", ref value.NonDefeatedFactions);
            }

            public void Serialize(OutputArchive archive, GameStateMessage value)
            {
                archive.Serialize<GameState>("GameState", value.GameState);
                archive.Serialize<DynamicArray<Faction>>("NonDefeatedFactions", value.NonDefeatedFactions);
            }
        }
    }
}
