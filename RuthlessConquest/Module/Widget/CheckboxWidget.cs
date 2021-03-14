using System;
using Engine;
using Engine.Media;

namespace Game
{
    public class CheckboxWidget : CanvasWidget
    {
        public CheckboxWidget()
        {
            WidgetsList children = this.Children;
            StackPanelWidget stackPanelWidget = new StackPanelWidget();
            stackPanelWidget.Direction = LayoutDirection.Horizontal;
            WidgetsList children2 = stackPanelWidget.Children;
            CanvasWidget canvasWidget = new CanvasWidget();
            WidgetsList children3 = canvasWidget.Children;
            RectangleWidget rectangleWidget = new RectangleWidget();
            rectangleWidget.FillColor = Color.Transparent;
            rectangleWidget.OutlineThickness = 2f;
            RectangleWidget widget = rectangleWidget;
            this.RectangleWidget = rectangleWidget;
            children3.Add(widget);
            WidgetsList children4 = canvasWidget.Children;
            RectangleWidget rectangleWidget2 = new RectangleWidget();
            rectangleWidget2.IsVisible = false;
            rectangleWidget2.Margin = new Vector2(1f, 1f);
            widget = rectangleWidget2;
            this.TickWidget = rectangleWidget2;
            children4.Add(widget);
            canvasWidget.Children.Add(this.ClickableWidget = new ClickableWidget());
            CanvasWidget widget2 = canvasWidget;
            this.CanvasWidget = canvasWidget;
            children2.Add(widget2);
            WidgetsList children5 = stackPanelWidget.Children;
            LabelWidget labelWidget = new LabelWidget();
            labelWidget.Margin = new Vector2(10f, 0f);
            LabelWidget widget3 = labelWidget;
            this.LabelWidget = labelWidget;
            children5.Add(widget3);
            children.Add(stackPanelWidget);
            this.Margin = new Vector2(0f, 6f);
            this.Color = Colors.Fore;
            this.IsAutoCheckingEnabled = true;
            this.CheckboxSize = new Vector2(32f, 32f);
            this.Font = Fonts.Normal;
            this.TickSubtexture = Textures.Gui.Tick;
        }

        public bool IsPressed
        {
            get
            {
                return this.ClickableWidget.IsPressed;
            }
        }

        public bool IsClicked
        {
            get
            {
                return this.ClickableWidget.IsClicked;
            }
        }

        public bool IsTapped
        {
            get
            {
                return this.ClickableWidget.IsTapped;
            }
        }

        public bool IsChecked { get; set; }

        public bool IsAutoCheckingEnabled { get; set; }

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

        public Subtexture TickSubtexture
        {
            get
            {
                return this.TickWidget.Subtexture;
            }
            set
            {
                this.TickWidget.Subtexture = value;
            }
        }

        public Color Color { get; set; }

        public Vector2 CheckboxSize
        {
            get
            {
                return this.CanvasWidget.Size;
            }
            set
            {
                this.CanvasWidget.Size = value;
            }
        }

        public override void Update()
        {
            if (this.IsClicked && this.IsAutoCheckingEnabled)
            {
                this.IsChecked = !this.IsChecked;
            }
        }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            bool flag = WidgetsManager.IsWidgetEnabled(this);
            this.LabelWidget.Color = (flag ? this.Color : new Color(112, 112, 112));
            this.RectangleWidget.FillColor = new Color(0, 0, 0, 128);
            this.RectangleWidget.OutlineColor = (flag ? new Color(128, 128, 128) : new Color(112, 112, 112));
            this.TickWidget.IsVisible = this.IsChecked;
            this.TickWidget.FillColor = (flag ? this.Color : new Color(112, 112, 112));
            this.TickWidget.OutlineColor = Color.Transparent;
            this.TickWidget.Subtexture = this.TickSubtexture;
            base.MeasureOverride(parentAvailableSize);
        }

        private CanvasWidget CanvasWidget;

        private RectangleWidget RectangleWidget;

        private RectangleWidget TickWidget;

        private LabelWidget LabelWidget;

        private ClickableWidget ClickableWidget;
    }
}
