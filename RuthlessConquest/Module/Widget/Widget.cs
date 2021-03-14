using System;
using System.Xml.Linq;
using Engine;

namespace Game
{
    public class Widget
    {
        public Widget()
        {
            this.IsVisible = true;
            this.IsHitTestVisible = true;
            this.IsEnabled = true;
            this.IsUpdateEnabled = true;
            this.DesiredSize = new Vector2(float.PositiveInfinity);
        }

        public WidgetInput WidgetsHierarchyInput
        {
            get
            {
                return this.m_widgetsHierarchyInput;
            }
            set
            {
                if (value == null)
                {
                    if (this.m_widgetsHierarchyInput != null)
                    {
                        this.m_widgetsHierarchyInput.m_widget = null;
                        this.m_widgetsHierarchyInput = null;
                    }
                    return;
                }
                if (value.m_widget != null && value.m_widget != this)
                {
                    throw new InvalidOperationException("WidgetInput already assigned to another widget.");
                }
                value.m_widget = this;
                this.m_widgetsHierarchyInput = value;
            }
        }

        public WidgetInput Input
        {
            get
            {
                Widget widget = this;
                while (widget.WidgetsHierarchyInput == null)
                {
                    widget = widget.ParentWidget;
                    if (widget == null)
                    {
                        return WidgetInput.EmptyInput;
                    }
                }
                return widget.WidgetsHierarchyInput;
            }
        }

        public Matrix LayoutTransform
        {
            get
            {
                return this.m_layoutTransform;
            }
            set
            {
                this.m_layoutTransform = value;
                this.m_isLayoutTransformIdentity = (value == Matrix.Identity);
            }
        }

        public Matrix RenderTransform
        {
            get
            {
                return this.m_renderTransform;
            }
            set
            {
                this.m_renderTransform = value;
                this.m_isRenderTransformIdentity = (value == Matrix.Identity);
            }
        }

        public Matrix GlobalTransform
        {
            get
            {
                return this.m_globalTransform;
            }
        }

        public Matrix InvertedGlobalTransform
        {
            get
            {
                if (this.m_invertedGlobalTransform == null)
                {
                    this.m_invertedGlobalTransform = new Matrix?(Matrix.Invert(this.m_globalTransform));
                }
                return this.m_invertedGlobalTransform.Value;
            }
        }

        public BoundingRectangle GlobalBounds { get; private set; }

        public Color ColorTransform { get; set; } = Color.White;

        public Color GlobalColorTransform { get; private set; }

        public virtual string Name { get; set; }

        public object Tag { get; set; }

        public virtual bool IsVisible
        {
            get
            {
                return this.m_isVisible;
            }
            set
            {
                if (value != this.m_isVisible)
                {
                    this.m_isVisible = value;
                    if (!this.m_isVisible)
                    {
                        this.UpdateCeases();
                    }
                }
            }
        }

        public virtual bool IsEnabled
        {
            get
            {
                return this.m_isEnabled;
            }
            set
            {
                if (value != this.m_isEnabled)
                {
                    this.m_isEnabled = value;
                    if (!this.m_isEnabled)
                    {
                        this.UpdateCeases();
                    }
                }
            }
        }

        public virtual bool IsUpdateEnabled
        {
            get
            {
                return this.m_isUpdateEnabled;
            }
            set
            {
                if (value != this.m_isUpdateEnabled)
                {
                    this.m_isUpdateEnabled = value;
                    if (!this.m_isUpdateEnabled)
                    {
                        this.UpdateCeases();
                    }
                }
            }
        }

        public virtual bool IsHitTestVisible { get; set; }

        public bool ClampToBounds { get; set; }

        public virtual Vector2 Margin { get; set; }

        public virtual Vector2 Alignment { get; set; }

        public Vector2 ActualSize { get; private set; }

        public Vector2 DesiredSize { get; protected set; }

        public Vector2 ParentDesiredSize { get; private set; }

        public bool IsDrawRequired { get; protected set; }

        public bool IsOverdrawRequired { get; protected set; }

        public XElement Style
        {
            set
            {
                WidgetsManager.LoadWidgetContents(this, null, value);
            }
        }

        public ContainerWidget ParentWidget { get; private set; }

        public ContainerWidget RootWidget
        {
            get
            {
                if (this.ParentWidget == null)
                {
                    return this as ContainerWidget;
                }
                return this.ParentWidget.RootWidget;
            }
        }

        public bool IsChildWidgetOf(ContainerWidget containerWidget)
        {
            return containerWidget == this.ParentWidget || (this.ParentWidget != null && this.ParentWidget.IsChildWidgetOf(containerWidget));
        }

        public virtual void MeasureOverride(Vector2 parentAvailableSize)
        {
        }

        public virtual void ArrangeOverride()
        {
        }

        public virtual void UpdateCeases()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Draw()
        {
        }

        public virtual void Overdraw()
        {
        }

        public virtual bool HitTest(Vector2 point)
        {
            Vector2 vector = this.ScreenToWidget(point);
            return vector.X >= 0f && vector.Y >= 0f && vector.X <= this.ActualSize.X && vector.Y <= this.ActualSize.Y;
        }

        public Vector2 ScreenToWidget(Vector2 p)
        {
            return Vector2.Transform(p, this.InvertedGlobalTransform);
        }

        public Vector2 WidgetToScreen(Vector2 p)
        {
            return Vector2.Transform(p, this.GlobalTransform);
        }

        public static bool TestOverlap(Widget w1, Widget w2)
        {
            return w2.GlobalBounds.Min.X < w1.GlobalBounds.Max.X - 0.001f && w2.GlobalBounds.Min.Y < w1.GlobalBounds.Max.Y - 0.001f && w1.GlobalBounds.Min.X < w2.GlobalBounds.Max.X - 0.001f && w1.GlobalBounds.Min.Y < w2.GlobalBounds.Max.Y - 0.001f;
        }

        internal virtual void ChangeParent(ContainerWidget parentWidget)
        {
            if (parentWidget != this.ParentWidget)
            {
                this.ParentWidget = parentWidget;
                if (parentWidget == null)
                {
                    this.UpdateCeases();
                }
            }
        }

        internal void Measure(Vector2 parentAvailableSize)
        {
            this.MeasureOverride(parentAvailableSize);
            if (this.DesiredSize.X != float.PositiveInfinity && this.DesiredSize.Y != float.PositiveInfinity)
            {
                BoundingRectangle boundingRectangle = this.TransformBoundsToParent(this.DesiredSize);
                this.ParentDesiredSize = boundingRectangle.Size();
                this.m_parentOffset = -boundingRectangle.Min;
                return;
            }
            this.ParentDesiredSize = this.DesiredSize;
            this.m_parentOffset = Vector2.Zero;
        }

        internal void Arrange(Vector2 position, Vector2 parentActualSize)
        {
            float num = this.m_layoutTransform.M11 * this.m_layoutTransform.M11;
            float num2 = this.m_layoutTransform.M12 * this.m_layoutTransform.M12;
            float num3 = this.m_layoutTransform.M21 * this.m_layoutTransform.M21;
            float num4 = this.m_layoutTransform.M22 * this.m_layoutTransform.M22;
            this.ActualSize = new Vector2((num * parentActualSize.X + num3 * parentActualSize.Y) / (num + num3), (num2 * parentActualSize.X + num4 * parentActualSize.Y) / (num2 + num4));
            this.m_parentOffset = -this.TransformBoundsToParent(this.ActualSize).Min;
            if (this.ParentWidget != null)
            {
                this.GlobalColorTransform = this.ParentWidget.GlobalColorTransform * this.ColorTransform;
            }
            else
            {
                this.GlobalColorTransform = this.ColorTransform;
            }
            if (this.m_isRenderTransformIdentity)
            {
                this.m_globalTransform = this.m_layoutTransform;
            }
            else if (this.m_isLayoutTransformIdentity)
            {
                this.m_globalTransform = this.m_renderTransform;
            }
            else
            {
                this.m_globalTransform = this.m_renderTransform * this.m_layoutTransform;
            }
            this.m_globalTransform.M41 = this.m_globalTransform.M41 + (position.X + this.m_parentOffset.X);
            this.m_globalTransform.M42 = this.m_globalTransform.M42 + (position.Y + this.m_parentOffset.Y);
            if (this.ParentWidget != null)
            {
                this.m_globalTransform *= this.ParentWidget.GlobalTransform;
            }
            this.m_invertedGlobalTransform = null;
            this.GlobalBounds = this.TransformBoundsToGlobal(this.ActualSize);
            this.ArrangeOverride();
        }

        private BoundingRectangle TransformBoundsToParent(Vector2 size)
        {
            float num = this.m_layoutTransform.M11 * size.X;
            float num2 = this.m_layoutTransform.M21 * size.Y;
            float x = num + num2;
            float num3 = this.m_layoutTransform.M12 * size.X;
            float num4 = this.m_layoutTransform.M22 * size.Y;
            float x2 = num3 + num4;
            float x3 = MathUtils.Min(0f, num, num2, x);
            float x4 = MathUtils.Max(0f, num, num2, x);
            float y = MathUtils.Min(0f, num3, num4, x2);
            float y2 = MathUtils.Max(0f, num3, num4, x2);
            return new BoundingRectangle(x3, y, x4, y2);
        }

        private BoundingRectangle TransformBoundsToGlobal(Vector2 size)
        {
            float num = this.m_globalTransform.M11 * size.X;
            float num2 = this.m_globalTransform.M21 * size.Y;
            float x = num + num2;
            float num3 = this.m_globalTransform.M12 * size.X;
            float num4 = this.m_globalTransform.M22 * size.Y;
            float x2 = num3 + num4;
            float num5 = MathUtils.Min(0f, num, num2, x);
            float num6 = MathUtils.Max(0f, num, num2, x);
            float num7 = MathUtils.Min(0f, num3, num4, x2);
            float num8 = MathUtils.Max(0f, num3, num4, x2);
            return new BoundingRectangle(num5 + this.m_globalTransform.M41, num7 + this.m_globalTransform.M42, num6 + this.m_globalTransform.M41, num8 + this.m_globalTransform.M42);
        }

        private bool m_isVisible;

        private bool m_isEnabled;

        private bool m_isUpdateEnabled;

        private Vector2 m_parentOffset;

        private bool m_isLayoutTransformIdentity = true;

        private bool m_isRenderTransformIdentity = true;

        private Matrix m_layoutTransform = Matrix.Identity;

        private Matrix m_renderTransform = Matrix.Identity;

        private Matrix m_globalTransform = Matrix.Identity;

        private Matrix? m_invertedGlobalTransform;

        private WidgetInput m_widgetsHierarchyInput;
    }
}
