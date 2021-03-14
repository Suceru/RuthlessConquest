using System;
using Engine;
using Engine.Audio;
using Engine.Media;

namespace Game
{
    public class SliderWidget : CanvasWidget
    {
        public SliderWidget()
        {
            this.Sound = Sounds.Slider;
            WidgetsList children = this.Children;
            StackPanelWidget stackPanelWidget = new StackPanelWidget();
            stackPanelWidget.Direction = LayoutDirection.Horizontal;
            stackPanelWidget.IsHitTestVisible = false;
            stackPanelWidget.Margin = new Vector2(0f, 10f);
            WidgetsList children2 = stackPanelWidget.Children;
            CanvasWidget canvasWidget = new CanvasWidget();
            canvasWidget.Children.Add(new RectangleWidget
            {
                Size = new Vector2(float.PositiveInfinity, 6f),
                OutlineColor = Color.Transparent,
                FillColor = Colors.HighDim,
                Margin = new Vector2(10f, 0f)
            });
            WidgetsList children3 = canvasWidget.Children;
            BevelledRectangleWidget bevelledRectangleWidget = new BevelledRectangleWidget();
            bevelledRectangleWidget.Size = new Vector2(30f, 45f);
            bevelledRectangleWidget.IsHitTestVisible = true;
            bevelledRectangleWidget.BevelSize = 2f;
            Widget widget = bevelledRectangleWidget;
            this.TabWidget = bevelledRectangleWidget;
            children3.Add(widget);
            CanvasWidget widget2 = canvasWidget;
            this.SliderCanvasWidget = canvasWidget;
            children2.Add(widget2);
            WidgetsList children4 = stackPanelWidget.Children;
            CanvasWidget canvasWidget2 = new CanvasWidget();
            canvasWidget2.Size = new Vector2(100f, -1f);
            WidgetsList children5 = canvasWidget2.Children;
            LabelWidget labelWidget = new LabelWidget();
            labelWidget.Alignment = new Vector2(1f, 0f);
            LabelWidget widget3 = labelWidget;
            this.LabelWidget = labelWidget;
            children5.Add(widget3);
            widget2 = canvasWidget2;
            this.LabelCanvasWidget = canvasWidget2;
            children4.Add(widget2);
            children.Add(stackPanelWidget);
        }

        public bool IsSliding { get; private set; }

        public LayoutDirection LayoutDirection { get; set; }

        public float MinValue
        {
            get
            {
                return this.InternalMinValue;
            }
            set
            {
                if (value != this.InternalMinValue)
                {
                    this.InternalMinValue = value;
                    this.MaxValue = MathUtils.Max(this.MinValue, this.MaxValue);
                    this.Value = MathUtils.Clamp(this.Value, this.MinValue, this.MaxValue);
                }
            }
        }

        public float MaxValue
        {
            get
            {
                return this.InternalMaxValue;
            }
            set
            {
                if (value != this.InternalMaxValue)
                {
                    this.InternalMaxValue = value;
                    this.MinValue = MathUtils.Min(this.MinValue, this.MaxValue);
                    this.Value = MathUtils.Clamp(this.Value, this.MinValue, this.MaxValue);
                }
            }
        }

        public float Value
        {
            get
            {
                return this.InternalValue;
            }
            set
            {
                if (this.InternalGranularity > 0f)
                {
                    this.InternalValue = MathUtils.Round(MathUtils.Clamp(value, this.MinValue, this.MaxValue) / this.InternalGranularity) * this.InternalGranularity;
                    return;
                }
                this.InternalValue = MathUtils.Clamp(value, this.MinValue, this.MaxValue);
            }
        }

        public float Granularity
        {
            get
            {
                return this.InternalGranularity;
            }
            set
            {
                this.InternalGranularity = MathUtils.Max(value, 0f);
            }
        }

        public string Text
        {
            get
            {
                return this.LabelWidget.Text;
            }
            set
            {
                this.LabelWidget.Text = value;
            }
        }

        public BitmapFont Font
        {
            get
            {
                return this.LabelWidget.Font;
            }
            set
            {
                this.LabelWidget.Font = value;
            }
        }

        public SoundBuffer Sound { get; set; }

        public bool IsLabelVisible
        {
            get
            {
                return this.LabelCanvasWidget.IsVisible;
            }
            set
            {
                this.LabelCanvasWidget.IsVisible = value;
            }
        }

        public float LabelWidth
        {
            get
            {
                return this.LabelCanvasWidget.Size.X;
            }
            set
            {
                this.LabelCanvasWidget.Size = new Vector2(value, this.LabelCanvasWidget.Size.Y);
            }
        }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            base.MeasureOverride(parentAvailableSize);
            IsDrawRequired = true;
        }

        public override void ArrangeOverride()
        {
            base.ArrangeOverride();
            float num = (this.LayoutDirection == LayoutDirection.Horizontal) ? this.SliderCanvasWidget.ActualSize.X : this.SliderCanvasWidget.ActualSize.Y;
            float num2 = (this.LayoutDirection == LayoutDirection.Horizontal) ? this.TabWidget.ActualSize.X : this.TabWidget.ActualSize.Y;
            float num3 = (this.MaxValue > this.MinValue) ? ((this.Value - this.MinValue) / (this.MaxValue - this.MinValue)) : 0f;
            if (this.LayoutDirection == LayoutDirection.Horizontal)
            {
                Vector2 zero = Vector2.Zero;
                zero.X = num3 * (num - num2);
                zero.Y = MathUtils.Max((this.SliderCanvasWidget.ActualSize.Y - this.TabWidget.ActualSize.Y) / 2f, 0f);
                this.SliderCanvasWidget.SetWidgetPosition(this.TabWidget, new Vector2?(zero));
            }
            else
            {
                Vector2 zero2 = Vector2.Zero;
                zero2.X = MathUtils.Max(this.SliderCanvasWidget.ActualSize.X - this.TabWidget.ActualSize.X, 0f) / 2f;
                zero2.Y = num3 * (num - num2);
                this.SliderCanvasWidget.SetWidgetPosition(this.TabWidget, new Vector2?(zero2));
            }
            base.ArrangeOverride();
        }

        public override void Update()
        {
            float num = (this.LayoutDirection == LayoutDirection.Horizontal) ? this.SliderCanvasWidget.ActualSize.X : this.SliderCanvasWidget.ActualSize.Y;
            float num2 = (this.LayoutDirection == LayoutDirection.Horizontal) ? this.TabWidget.ActualSize.X : this.TabWidget.ActualSize.Y;
            if (Input.Tap != null && WidgetsManager.HitTest(Input.Tap.Value) == this.TabWidget)
            {
                this.DragStartPoint = new Vector2?(ScreenToWidget(Input.Press.Value));
            }
            if (Input.Press != null)
            {
                if (this.DragStartPoint != null)
                {
                    Vector2 vector = ScreenToWidget(Input.Press.Value);
                    float value = this.Value;
                    if (this.LayoutDirection == LayoutDirection.Horizontal)
                    {
                        float f = (vector.X - num2 / 2f) / (num - num2);
                        this.Value = MathUtils.Lerp(this.MinValue, this.MaxValue, f);
                    }
                    else
                    {
                        float f2 = (vector.Y - num2 / 2f) / (num - num2);
                        this.Value = MathUtils.Lerp(this.MinValue, this.MaxValue, f2);
                    }
                    if (this.Value != value && this.InternalGranularity > 0f && this.Sound != null)
                    {
                        AudioManager.PlaySound(this.Sound, false, 1f, 1f, 0f);
                    }
                }
            }
            else
            {
                this.DragStartPoint = null;
            }
            this.IsSliding = (this.DragStartPoint != null && WidgetsManager.IsWidgetEnabled(this) && WidgetsManager.IsWidgetVisible(this));
            if (this.DragStartPoint != null)
            {
                Input.Clear();
            }
        }

        private CanvasWidget SliderCanvasWidget;

        private CanvasWidget LabelCanvasWidget;

        private Widget TabWidget;

        private LabelWidget LabelWidget;

        private float InternalMinValue;

        private float InternalMaxValue = 1f;

        private float InternalGranularity = 0.1f;

        private float InternalValue;

        private Vector2? DragStartPoint;
    }
}
