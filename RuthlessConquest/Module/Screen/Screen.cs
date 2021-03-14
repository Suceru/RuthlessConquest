using System;
using Engine;

namespace Game
{
    internal class Screen : CanvasWidget
    {
        public Screen()
        {
            Size = new Vector2(float.PositiveInfinity);
        }

        public virtual void Enter(object[] parameters)
        {
        }

        public virtual void Leave()
        {
        }
    }
}
