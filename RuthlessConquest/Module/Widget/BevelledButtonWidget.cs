using System;
using Engine;
using Engine.Audio;
using Engine.Media;

namespace Game
{
    public class BevelledButtonWidget : ButtonWidget
    {
        public BevelledRectangleWidget RectangleWidget { get; private set; }

        public RectangleWidget ImageWidget { get; private set; }

        public LabelWidget LabelWidget { get; private set; }

        public ClickableWidget ClickableWidget { get; private set; }

        public BevelledButtonWidget()
        {
            WidgetsList children = this.Children;
            CanvasWidget canvasWidget = new CanvasWidget();
            canvasWidget.Margin = new Vector2(6f, 6f);
            WidgetsList children2 = canvasWidget.Children;
            BevelledRectangleWidget bevelledRectangleWidget = new BevelledRectangleWidget();
            bevelledRectangleWidget.Texture = Textures.Gui.Panel;
            BevelledRectangleWidget widget = bevelledRectangleWidget;
            this.RectangleWidget = bevelledRectangleWidget;
            children2.Add(widget);
            WidgetsList children3 = canvasWidget.Children;
            StackPanelWidget stackPanelWidget = new StackPanelWidget();
            WidgetsList children4 = stackPanelWidget.Children;
            RectangleWidget rectangleWidget = new RectangleWidget();
            rectangleWidget.IsVisible = false;
            RectangleWidget widget2 = rectangleWidget;
            this.ImageWidget = rectangleWidget;
            children4.Add(widget2);
            stackPanelWidget.Children.Add(this.LabelWidget = new LabelWidget());
            children3.Add(stackPanelWidget);
            WidgetsList children5 = canvasWidget.Children;
            ClickableWidget clickableWidget = new ClickableWidget();
            clickableWidget.Sound = Sounds.Click;
            ClickableWidget widget3 = clickableWidget;
            this.ClickableWidget = clickableWidget;
            children5.Add(widget3);
            children.Add(canvasWidget);
            this.Color = Colors.Fore;
            this.DisabledColor = Colors.ForeDisabled;
            this.BevelSize = 2f;
        }

        public override bool IsClicked
        {
            get
            {
                return this.ClickableWidget.IsClicked;
            }
        }

        public override bool IsChecked
        {
            get
            {
                return this.ClickableWidget.IsChecked;
            }
            set
            {
                this.ClickableWidget.IsChecked = value;
            }
        }

        public override bool IsAutoCheckingEnabled
        {
            get
            {
                return this.ClickableWidget.IsAutoCheckingEnabled;
            }
            set
            {
                this.ClickableWidget.IsAutoCheckingEnabled = value;
            }
        }

        public override bool IsOkButton
        {
            get
            {
                return this.ClickableWidget.IsOkButton;
            }
            set
            {
                this.ClickableWidget.IsOkButton = value;
            }
        }

        public override bool IsCancelButton
        {
            get
            {
                return this.ClickableWidget.IsCancelButton;
            }
            set
            {
                this.ClickableWidget.IsCancelButton = value;
            }
        }

        public override string Text
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

        public override BitmapFont Font
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

        public Subtexture Subtexture
        {
            get
            {
                return this.ImageWidget.Subtexture;
            }
            set
            {
                this.ImageWidget.Subtexture = value;
                this.ImageWidget.IsVisible = (value != null);
            }
        }

        public override Color Color { get; set; }

        public Color DisabledColor { get; set; }

        public Color BevelColor
        {
            get
            {
                return this.RectangleWidget.BevelColor;
            }
            set
            {
                this.RectangleWidget.BevelColor = value;
            }
        }

        public Color CenterColor
        {
            get
            {
                return this.RectangleWidget.CenterColor;
            }
            set
            {
                this.RectangleWidget.CenterColor = value;
            }
        }

        public float AmbientLight
        {
            get
            {
                return this.RectangleWidget.AmbientLight;
            }
            set
            {
                this.RectangleWidget.AmbientLight = value;
            }
        }

        public float DirectionalLight
        {
            get
            {
                return this.RectangleWidget.DirectionalLight;
            }
            set
            {
                this.RectangleWidget.DirectionalLight = value;
            }
        }

        public float BevelSize { get; set; }

        public SoundBuffer Sound
        {
            get
            {
                return this.ClickableWidget.Sound;
            }
            set
            {
                this.ClickableWidget.Sound = value;
            }
        }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            bool flag = WidgetsManager.IsWidgetEnabled(this);
            this.LabelWidget.Color = (flag ? this.Color : this.DisabledColor);
            this.ImageWidget.FillColor = (flag ? this.Color : this.DisabledColor);
            if (this.ClickableWidget.IsPressed || this.IsChecked)
            {
                this.RectangleWidget.BevelSize = -0.5f * this.BevelSize;
            }
            else
            {
                this.RectangleWidget.BevelSize = this.BevelSize;
            }
            base.MeasureOverride(parentAvailableSize);
        }
    }
}
