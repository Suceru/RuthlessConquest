using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    public class ClearWidget : Widget
    {
        public ClearWidget()
        {
            this.ClearColor = true;
            this.ClearDepth = true;
            this.ClearStencil = true;
            this.Color = Color.Black;
            this.Depth = 1f;
            this.Stencil = 0;
            this.IsHitTestVisible = false;
        }

        public Color Color { get; set; }

        public float Depth { get; set; }

        public int Stencil { get; set; }

        public bool ClearColor { get; set; }

        public bool ClearDepth { get; set; }

        public bool ClearStencil { get; set; }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            IsDrawRequired = true;
        }

        public override void Draw()
        {
            Display.Clear(this.ClearColor ? new Vector4?(new Vector4(this.Color * GlobalColorTransform)) : null, this.ClearDepth ? new float?(this.Depth) : null, this.ClearStencil ? new int?(this.Stencil) : null);
        }
    }
}
