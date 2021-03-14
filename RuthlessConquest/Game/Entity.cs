using System;

namespace Game
{
    internal class Entity : GameMember
    {
        protected Entity() : base(null)
        {
        }

        protected internal virtual void OnAdded()
        {
        }

        protected internal virtual void OnRemoved()
        {
        }
    }
}
