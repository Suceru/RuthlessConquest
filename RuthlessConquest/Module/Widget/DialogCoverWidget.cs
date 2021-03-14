using System;
using Engine;

namespace Game
{
    internal class DialogCoverWidget : CanvasWidget
    {
        public RectangleWidget Rectangle { get; private set; }

        public ClickableWidget Clickable { get; private set; }

        public DialogCoverWidget()
        {
            WidgetsList children = this.Children;
            RectangleWidget rectangleWidget = new RectangleWidget();
            rectangleWidget.OutlineColor = Color.Transparent;
            rectangleWidget.FillColor = new Color(0, 0, 0, 192);
            RectangleWidget widget = rectangleWidget;
            this.Rectangle = rectangleWidget;
            children.Add(widget);
            this.Children.Add(this.Clickable = new ClickableWidget());
        }
    }
}
