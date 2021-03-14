using System;
using System.Xml.Linq;
using Engine;
using Engine.Content;
using Engine.Media;

namespace Game
{
    public class BitmapButtonWidget : ButtonWidget
    {
        public BitmapButtonWidget()
        {
            this.Color = Color.White;
            XElement node = ContentCache.Get<XElement>("Widgets/BitmapButtonContents", true);
            WidgetsManager.LoadWidgetChildren(this, this, node);
            this.RectangleWidget = this.Children.Find<RectangleWidget>("Button.Rectangle", true);
            this.ImageWidget = this.Children.Find<RectangleWidget>("Button.Image", true);
            this.LabelWidget = this.Children.Find<LabelWidget>("Button.Label", true);
            this.ClickableWidget = this.Children.Find<ClickableWidget>("Button.Clickable", true);
            WidgetsManager.LoadWidgetProperties(this, this, node);
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

        public Subtexture NormalSubtexture { get; set; }

        public Subtexture ClickedSubtexture { get; set; }

        public override Color Color { get; set; }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            bool flag = WidgetsManager.IsWidgetEnabled(this);
            this.LabelWidget.Color = (flag ? this.Color : new Color(112, 112, 112));
            this.ImageWidget.FillColor = (flag ? this.Color : new Color(112, 112, 112));
            if (this.ClickableWidget.IsPressed || this.IsChecked)
            {
                this.RectangleWidget.Subtexture = this.ClickedSubtexture;
            }
            else
            {
                this.RectangleWidget.Subtexture = this.NormalSubtexture;
            }
            base.MeasureOverride(parentAvailableSize);
        }

        private RectangleWidget RectangleWidget;

        private RectangleWidget ImageWidget;

        private LabelWidget LabelWidget;

        private ClickableWidget ClickableWidget;
    }
}
