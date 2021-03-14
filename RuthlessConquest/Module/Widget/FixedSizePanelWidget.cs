using System;
using Engine;

namespace Game
{
    public class FixedSizePanelWidget : ContainerWidget
    {
        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            Vector2 zero = Vector2.Zero;
            foreach (Widget widget in this.Children)
            {
                if (widget.IsVisible)
                {
                    widget.Measure(Vector2.Max(parentAvailableSize - 2f * widget.Margin, Vector2.Zero));
                    if (widget.ParentDesiredSize.X != float.PositiveInfinity)
                    {
                        zero.X = MathUtils.Max(zero.X, widget.ParentDesiredSize.X + 2f * widget.Margin.X);
                    }
                    if (widget.ParentDesiredSize.Y != float.PositiveInfinity)
                    {
                        zero.Y = MathUtils.Max(zero.Y, widget.ParentDesiredSize.Y + 2f * widget.Margin.Y);
                    }
                }
            }
            DesiredSize = zero;
        }

        public override void ArrangeOverride()
        {
            foreach (Widget widget in this.Children)
            {
                ArrangeChildWidgetInCell(Vector2.Zero, ActualSize, widget);
            }
        }
    }
}
