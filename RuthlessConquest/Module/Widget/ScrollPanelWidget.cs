using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    public class ScrollPanelWidget : ContainerWidget
    {
        public ScrollPanelWidget()
        {
            ClampToBounds = true;
        }

        public Vector2 Size { get; set; } = new Vector2(-1f);

        public virtual LayoutDirection Direction { get; set; }

        public virtual float ScrollPosition { get; set; }

        public virtual float ScrollSpeed { get; set; }

        public bool UseInitialScroll { get; set; } = true;

        public override void UpdateCeases()
        {
            base.UpdateCeases();
            this.m_initialScrollRequired = true;
        }

        public virtual float CalculateScrollAreaLength()
        {
            float num = 0f;
            foreach (Widget widget in this.Children)
            {
                if (widget.IsVisible)
                {
                    if (this.Direction == LayoutDirection.Horizontal)
                    {
                        num = MathUtils.Max(num, widget.ParentDesiredSize.X + 2f * widget.Margin.X);
                    }
                    else
                    {
                        num = MathUtils.Max(num, widget.ParentDesiredSize.Y + 2f * widget.Margin.Y);
                    }
                }
            }
            return num;
        }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            IsDrawRequired = true;
            Vector2 vector = Vector2.Zero;
            foreach (Widget widget in this.Children)
            {
                if (widget.IsVisible)
                {
                    if (this.Direction == LayoutDirection.Horizontal)
                    {
                        widget.Measure(new Vector2(float.MaxValue, MathUtils.Max(parentAvailableSize.Y - 2f * widget.Margin.Y, 0f)));
                    }
                    else
                    {
                        widget.Measure(new Vector2(MathUtils.Max(parentAvailableSize.X - 2f * widget.Margin.X, 0f), float.MaxValue));
                    }
                    vector = new Vector2
                    {
                        X = MathUtils.Max(vector.X, widget.ParentDesiredSize.X + 2f * widget.Margin.X),
                        Y = MathUtils.Max(vector.Y, widget.ParentDesiredSize.Y + 2f * widget.Margin.Y)
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
                Vector2 zero = Vector2.Zero;
                Vector2 actualSize = ActualSize;
                if (this.Direction == LayoutDirection.Horizontal)
                {
                    zero.X -= this.ScrollPosition;
                    actualSize.X = zero.X + widget.ParentDesiredSize.X;
                }
                else
                {
                    zero.Y -= this.ScrollPosition;
                    actualSize.Y = zero.Y + widget.ParentDesiredSize.Y;
                }
                ArrangeChildWidgetInCell(zero, actualSize, widget);
            }
        }

        public override void Update()
        {
            float num = 50f;
            float num2 = MathUtils.Min(Time.FrameDuration, 0.1f);
            if (num2 <= 0f)
            {
                return;
            }
            if (this.m_initialScrollRequired)
            {
                this.m_initialScrollRequired = false;
                if (this.UseInitialScroll)
                {
                    this.ScrollPosition = -40f;
                }
            }
            this.m_scrollAreaLength = this.CalculateScrollAreaLength();
            this.m_scrollBarAlpha = MathUtils.Max(this.m_scrollBarAlpha - 2f * num2, 0f);
            if (Input.Tap != null && this.HitTestPanel(Input.Tap.Value))
            {
                this.m_lastDragPosition = new Vector2?(ScreenToWidget(Input.Tap.Value));
            }
            if (this.m_lastDragPosition != null)
            {
                if (Input.Press != null)
                {
                    Vector2 vector = ScreenToWidget(Input.Press.Value);
                    Vector2 vector2 = vector - this.m_lastDragPosition.Value;
                    float num3;
                    if (this.Direction == LayoutDirection.Horizontal)
                    {
                        this.ScrollPosition += -vector2.X;
                        num3 = vector2.X / num2;
                    }
                    else
                    {
                        this.ScrollPosition += -vector2.Y;
                        num3 = vector2.Y / num2;
                    }
                    float num4 = (MathUtils.Abs(num3) < MathUtils.Abs(this.m_dragSpeed)) ? 20f : 16f;
                    this.m_dragSpeed += MathUtils.Saturate(num4 * num2) * (num3 - this.m_dragSpeed);
                    this.m_scrollBarAlpha = 4f;
                    this.m_lastDragPosition = new Vector2?(vector);
                    this.ScrollSpeed = 0f;
                }
                else
                {
                    this.ScrollSpeed = -this.m_dragSpeed;
                    this.m_dragSpeed = 0f;
                    this.m_lastDragPosition = null;
                }
            }
            if (this.ScrollSpeed != 0f)
            {
                this.ScrollSpeed *= MathUtils.Pow(0.33f, num2);
                if (MathUtils.Abs(this.ScrollSpeed) < 40f)
                {
                    this.ScrollSpeed = 0f;
                }
                this.ScrollPosition += this.ScrollSpeed * num2;
                this.m_scrollBarAlpha = 4f;
            }
            if (Input.Scroll != null && this.HitTestPanel(Input.Scroll.Value.XY))
            {
                this.ScrollPosition -= 40f * Input.Scroll.Value.Z;
                this.ScrollSpeed = 0f;
                num = 0f;
                this.m_scrollBarAlpha = 4f;
            }
            float num5 = MathUtils.Max(this.m_scrollAreaLength - ((this.Direction == LayoutDirection.Horizontal) ? ActualSize.X : ActualSize.Y), 0f);
            if (this.ScrollPosition < 0f)
            {
                if (this.m_lastDragPosition == null)
                {
                    this.ScrollPosition = MathUtils.Min(this.ScrollPosition + 8f * num2 * (0f - this.ScrollPosition + 5f), 0f);
                }
                this.ScrollPosition = MathUtils.Max(this.ScrollPosition, -num);
                this.ScrollSpeed = 0f;
            }
            if (this.ScrollPosition > num5)
            {
                if (this.m_lastDragPosition == null)
                {
                    this.ScrollPosition = MathUtils.Max(this.ScrollPosition + 8f * num2 * (num5 - this.ScrollPosition - 5f), num5);
                }
                this.ScrollPosition = MathUtils.Min(this.ScrollPosition, num5 + num);
                this.ScrollSpeed = 0f;
            }
            if (this.m_lastDragPosition != null && (Input.Drag != null || Input.Hold != null))
            {
                Input.Clear();
            }
        }

        public override void Draw()
        {
            Color color = new Color(128, 128, 128) * GlobalColorTransform * MathUtils.Saturate(this.m_scrollBarAlpha);
            if (color.A > 0 && this.m_scrollAreaLength > 0f)
            {
                FlatBatch2D flatBatch2D = WidgetsManager.PrimitivesRenderer2D.FlatBatch(0, DepthStencilState.None, null, null);
                int count = flatBatch2D.TriangleVertices.Count;
                if (this.Direction == LayoutDirection.Horizontal)
                {
                    float scrollPosition = this.ScrollPosition;
                    float x = ActualSize.X;
                    Vector2 corner = new Vector2(scrollPosition / this.m_scrollAreaLength * x, ActualSize.Y - 5f);
                    Vector2 corner2 = new Vector2((scrollPosition + x) / this.m_scrollAreaLength * x, ActualSize.Y - 1f);
                    flatBatch2D.QueueQuad(corner, corner2, 0f, color);
                }
                else
                {
                    float scrollPosition2 = this.ScrollPosition;
                    float y = ActualSize.Y;
                    Vector2 corner3 = new Vector2(ActualSize.X - 5f, scrollPosition2 / this.m_scrollAreaLength * y);
                    Vector2 corner4 = new Vector2(ActualSize.X - 1f, (scrollPosition2 + y) / this.m_scrollAreaLength * y);
                    flatBatch2D.QueueQuad(corner3, corner4, 0f, color);
                }
                flatBatch2D.TransformTriangles(GlobalTransform, count, -1);
            }
        }

        protected bool HitTestPanel(Vector2 position)
        {
            bool found = false;
            WidgetsManager.HitTest(position, delegate (Widget widget)
            {
                found = (widget.IsChildWidgetOf(this) || widget == this);
                return true;
            });
            return found;
        }

        private Vector2? m_lastDragPosition;

        private float m_dragSpeed;

        private float m_scrollBarAlpha;

        private float m_scrollAreaLength;

        private bool m_initialScrollRequired = true;
    }
}
