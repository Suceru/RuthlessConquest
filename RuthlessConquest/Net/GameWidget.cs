using Engine;
using System;

namespace Game
{
    internal class GameWidget : CanvasWidget
    {
        public Client Client { get; set; }

        public GameWidget()
        {
            IsDrawRequired = true;
            Size = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        }

        public override void Update()
        {
            Client client = this.Client;
            if (client == null)
            {
                return;
            }
            client.Update();
        }

        public override void Draw()
        {
            Client client = this.Client;
            if (client == null)
            {
                return;
            }
            client.Draw(GlobalColorTransform);
        }
    }
}
