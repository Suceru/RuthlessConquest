using System;
using Engine;

namespace Game
{
    public class UniformSpacingPanelWidget : ContainerWidget
    {
        public LayoutDirection Direction { get; set; }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            this.Count = 0;
            using (WidgetsList.Enumerator enumerator = this.Children.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsVisible)
                    {
                        this.Count++;
                    }
                }
            }
            if (this.Direction == LayoutDirection.Horizontal)
            {
                parentAvailableSize = Vector2.Min(parentAvailableSize, new Vector2(parentAvailableSize.X / Count, parentAvailableSize.Y));
            }
            else
            {
                parentAvailableSize = Vector2.Min(parentAvailableSize, new Vector2(parentAvailableSize.X, parentAvailableSize.Y / Count));
            }
            float num = 0f;
            foreach (Widget widget in this.Children)
            {
                if (widget.IsVisible)
                {
                    widget.Measure(Vector2.Max(parentAvailableSize - 2f * widget.Margin, Vector2.Zero));
                    if (this.Direction == LayoutDirection.Horizontal)
                    {
                        num = MathUtils.Max(num, widget.ParentDesiredSize.Y + 2f * widget.Margin.Y);
                    }
                    else
                    {
                        num = MathUtils.Max(num, widget.ParentDesiredSize.X + 2f * widget.Margin.X);
                    }
                }
            }
            if (this.Direction == LayoutDirection.Horizontal)
            {
                DesiredSize = new Vector2(float.PositiveInfinity, num);
                return;
            }
            DesiredSize = new Vector2(num, float.PositiveInfinity);
        }

        public override void ArrangeOverride()
        {
            Vector2 zero = Vector2.Zero;
            foreach (Widget widget in this.Children)
            {
                if (widget.IsVisible)
                {
                    if (this.Direction == LayoutDirection.Horizontal)
                    {
                        float num = (this.Count > 0) ? (ActualSize.X / Count) : 0f;
                        ArrangeChildWidgetInCell(zero, new Vector2(zero.X + num, zero.Y + ActualSize.Y), widget);
                        zero.X += num;
                    }
                    else
                    {
                        float num2 = (this.Count > 0) ? (ActualSize.Y / Count) : 0f;
                        ArrangeChildWidgetInCell(zero, new Vector2(zero.X + ActualSize.X, zero.Y + num2), widget);
                        zero.Y += num2;
                    }
                }
            }
        }

        private int Count;
    }
}
