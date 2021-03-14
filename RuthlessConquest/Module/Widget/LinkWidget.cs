using System;
using Engine;
using Engine.Graphics;
using Engine.Media;

namespace Game
{
    public class LinkWidget : FixedSizePanelWidget
    {
        public LinkWidget()
        {
            WidgetsList children = this.Children;
            Widget[] array = new Widget[2];
            array[0] = (this.LabelWidget = new LabelWidget());
            int num = 1;
            ClickableWidget clickableWidget = new ClickableWidget();
            clickableWidget.Sound = Sounds.Click;
            ClickableWidget clickableWidget2 = clickableWidget;
            this.ClickableWidget = clickableWidget;
            array[num] = clickableWidget2;
            children.Add(array);
        }

        public Vector2 Size
        {
            get
            {
                return this.LabelWidget.Size;
            }
            set
            {
                this.LabelWidget.Size = value;
            }
        }

        public bool IsClicked
        {
            get
            {
                return this.ClickableWidget.IsClicked;
            }
        }

        public bool IsPressed
        {
            get
            {
                return this.ClickableWidget.IsPressed;
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

        public TextAnchor TextAnchor
        {
            get
            {
                return this.LabelWidget.TextAnchor;
            }
            set
            {
                this.LabelWidget.TextAnchor = value;
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

        public Vector2 FontSpacing
        {
            get
            {
                return this.LabelWidget.FontSpacing;
            }
            set
            {
                this.LabelWidget.FontSpacing = value;
            }
        }

        public Color Color
        {
            get
            {
                return this.LabelWidget.Color;
            }
            set
            {
                this.LabelWidget.Color = value;
            }
        }

        public bool DropShadow
        {
            get
            {
                return this.LabelWidget.DropShadow;
            }
            set
            {
                this.LabelWidget.DropShadow = value;
            }
        }

        public string Url { get; set; }

        public override void Update()
        {
            if (!string.IsNullOrEmpty(this.Url) && this.IsClicked)
            {
                WebBrowserManager.LaunchBrowser(this.Url);
            }
        }

        private LabelWidget LabelWidget;

        private ClickableWidget ClickableWidget;
    }
}
