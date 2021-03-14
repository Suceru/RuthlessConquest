using System;
using Engine;

namespace Game
{
    public class StackPanelWidget : ContainerWidget
    {
        public LayoutDirection Direction { get; set; }

        public bool IsInverted { get; set; }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            this.m_fixedSize = 0f;
            this.m_fillCount = 0;
            float num = 0f;
            foreach (Widget widget in this.Children)
            {
                if (widget.IsVisible)
                {
                    widget.Measure(Vector2.Max(parentAvailableSize - 2f * widget.Margin, Vector2.Zero));
                    if (this.Direction == LayoutDirection.Horizontal)
                    {
                        if (widget.ParentDesiredSize.X != float.PositiveInfinity)
                        {
                            this.m_fixedSize += widget.ParentDesiredSize.X + 2f * widget.Margin.X;
                            parentAvailableSize.X = MathUtils.Max(parentAvailableSize.X - (widget.ParentDesiredSize.X + 2f * widget.Margin.X), 0f);
                        }
                        else
                        {
                            this.m_fillCount++;
                        }
                        num = MathUtils.Max(num, widget.ParentDesiredSize.Y + 2f * widget.Margin.Y);
                    }
                    else
                    {
                        if (widget.ParentDesiredSize.Y != float.PositiveInfinity)
                        {
                            this.m_fixedSize += widget.ParentDesiredSize.Y + 2f * widget.Margin.Y;
                            parentAvailableSize.Y = MathUtils.Max(parentAvailableSize.Y - (widget.ParentDesiredSize.Y + 2f * widget.Margin.Y), 0f);
                        }
                        else
                        {
                            this.m_fillCount++;
                        }
                        num = MathUtils.Max(num, widget.ParentDesiredSize.X + 2f * widget.Margin.X);
                    }
                }
            }
            if (this.Direction == LayoutDirection.Horizontal)
            {
                if (this.m_fillCount == 0)
                {
                    DesiredSize = new Vector2(this.m_fixedSize, num);
                    return;
                }
                DesiredSize = new Vector2(float.PositiveInfinity, num);
                return;
            }
            else
            {
                if (this.m_fillCount == 0)
                {
                    DesiredSize = new Vector2(num, this.m_fixedSize);
                    return;
                }
                DesiredSize = new Vector2(num, float.PositiveInfinity);
                return;
            }
        }

        public override void ArrangeOverride()
        {
            float num = 0f;
            foreach (Widget widget in this.Children)
            {
                if (widget.IsVisible)
                {
                    if (this.Direction == LayoutDirection.Horizontal)
                    {
                        float num2;
                        if (widget.ParentDesiredSize.X != float.PositiveInfinity)
                        {
                            num2 = widget.ParentDesiredSize.X + 2f * widget.Margin.X;
                        }
                        else
                        {
                            num2 = ((this.m_fillCount > 0) ? (MathUtils.Max(ActualSize.X - this.m_fixedSize, 0f) / m_fillCount) : 0f);
                        }
                        Vector2 c;
                        Vector2 c2;
                        if (this.IsInverted)
                        {
                            c = new Vector2(ActualSize.X - (num + num2), 0f);
                            c2 = new Vector2(ActualSize.X - num, ActualSize.Y);
                        }
                        else
                        {
                            c = new Vector2(num, 0f);
                            c2 = new Vector2(num + num2, ActualSize.Y);
                        }
                        ArrangeChildWidgetInCell(c, c2, widget);
                        num += num2;
                    }
                    else
                    {
                        float num3;
                        if (widget.ParentDesiredSize.Y != float.PositiveInfinity)
                        {
                            num3 = widget.ParentDesiredSize.Y + 2f * widget.Margin.Y;
                        }
                        else
                        {
                            num3 = ((this.m_fillCount > 0) ? (MathUtils.Max(ActualSize.Y - this.m_fixedSize, 0f) / m_fillCount) : 0f);
                        }
                        Vector2 c3;
                        Vector2 c4;
                        if (this.IsInverted)
                        {
                            c3 = new Vector2(0f, ActualSize.Y - (num + num3));
                            c4 = new Vector2(ActualSize.X, ActualSize.Y - num);
                        }
                        else
                        {
                            c3 = new Vector2(0f, num);
                            c4 = new Vector2(ActualSize.X, num + num3);
                        }
                        ArrangeChildWidgetInCell(c3, c4, widget);
                        num += num3;
                    }
                }
            }
        }

        private float m_fixedSize;

        private int m_fillCount;
    }
}
