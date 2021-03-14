using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
    public class CanvasWidget : ContainerWidget
    {
        public static void SetPosition(Widget widget, Vector2 position)
        {
            CanvasWidget canvasWidget = widget.ParentWidget as CanvasWidget;
            if (canvasWidget != null)
            {
                canvasWidget.SetWidgetPosition(widget, new Vector2?(position));
            }
        }

        public Vector2 Size { get; set; } = new Vector2(-1f);

        public Vector2? GetWidgetPosition(Widget widget)
        {
            Vector2 value;
            if (this.m_positions.TryGetValue(widget, out value))
            {
                return new Vector2?(value);
            }
            return null;
        }

        public void SetWidgetPosition(Widget widget, Vector2? position)
        {
            if (position != null)
            {
                this.m_positions[widget] = position.Value;
                return;
            }
            this.m_positions.Remove(widget);
        }

        public override void WidgetRemoved(Widget widget)
        {
            this.m_positions.Remove(widget);
        }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            Vector2 vector = Vector2.Zero;
            if (this.Size.X >= 0f)
            {
                parentAvailableSize.X = MathUtils.Min(parentAvailableSize.X, this.Size.X);
            }
            if (this.Size.Y >= 0f)
            {
                parentAvailableSize.Y = MathUtils.Min(parentAvailableSize.Y, this.Size.Y);
            }
            foreach (Widget widget in this.Children)
            {
                if (widget.IsVisible)
                {
                    Vector2? widgetPosition = this.GetWidgetPosition(widget);
                    Vector2 vector2 = (widgetPosition != null) ? widgetPosition.Value : Vector2.Zero;
                    widget.Measure(Vector2.Max(parentAvailableSize - vector2 - 2f * widget.Margin, Vector2.Zero));
                    vector = new Vector2
                    {
                        X = MathUtils.Max(vector.X, vector2.X + widget.ParentDesiredSize.X + 2f * widget.Margin.X),
                        Y = MathUtils.Max(vector.Y, vector2.Y + widget.ParentDesiredSize.Y + 2f * widget.Margin.Y)
                    };
                }
            }
            if (this.Size.X >= 0f)
            {
                vector.X = this.Size.X;
            }
            if (this.Size.Y >= 0f)
            {
                vector.Y = this.Size.Y;
            }
            DesiredSize = vector;
        }

        public override void ArrangeOverride()
        {
            foreach (Widget widget in this.Children)
            {
                if (widget.IsVisible)
                {
                    Vector2? widgetPosition = this.GetWidgetPosition(widget);
                    if (widgetPosition != null)
                    {
                        Vector2 zero = Vector2.Zero;
                        if (!float.IsPositiveInfinity(widget.ParentDesiredSize.X))
                        {
                            zero.X = widget.ParentDesiredSize.X;
                        }
                        else
                        {
                            zero.X = MathUtils.Max(ActualSize.X - widgetPosition.Value.X, 0f);
                        }
                        if (!float.IsPositiveInfinity(widget.ParentDesiredSize.Y))
                        {
                            zero.Y = widget.ParentDesiredSize.Y;
                        }
                        else
                        {
                            zero.Y = MathUtils.Max(ActualSize.Y - widgetPosition.Value.Y, 0f);
                        }
                        widget.Arrange(widgetPosition.Value, zero);
                    }
                    else
                    {
                        ArrangeChildWidgetInCell(Vector2.Zero, ActualSize, widget);
                    }
                }
            }
        }

        private Dictionary<Widget, Vector2> m_positions = new Dictionary<Widget, Vector2>();
    }
}
