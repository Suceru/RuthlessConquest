using System;
using Engine;

namespace Game
{
    public class MessageWidget : CanvasWidget
    {
        public MessageWidget()
        {
            this.IsHitTestVisible = false;
            this.IsVisible = false;
            WidgetsList children = this.Children;
            LabelWidget labelWidget = new LabelWidget();
            labelWidget.DropShadow = true;
            LabelWidget widget = labelWidget;
            this.LabelWidget = labelWidget;
            children.Add(widget);
        }

        public void SetMessage(string text, Color color)
        {
            this.MessageTime = Time.FrameStartTime;
            this.LabelWidget.Text = text;
            this.LabelWidget.Color = color;
            this.IsVisible = true;
        }

        public void ClearMessage()
        {
            this.IsVisible = false;
        }

        private LabelWidget LabelWidget;

        private double MessageTime;
    }
}
