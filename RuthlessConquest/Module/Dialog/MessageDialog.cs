using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    /// <summary>
    /// 保护类
    /// 消息对话框
    /// 继承:对话框
    /// </summary>
    internal class MessageDialog : Dialog
    {
        public MessageDialog(string largeMessage, string smallMessage, string button1Text, string button2Text, Vector2 size, Action<MessageDialogButton> handler)
        {
            this.Handler = handler;
            Size = new Vector2((size.X >= 0f) ? size.X : 600f, (size.Y >= 0f) ? size.Y : 250f);
            ClampToBounds = true;
            WidgetsList children = this.Children;
            Widget[] array = new Widget[3];
            array[0] = new RectangleWidget
            {
                FillColor = Colors.Back,
                OutlineColor = Colors.ForeDim,
                OutlineThickness = 2f
            };
            array[1] = new InterlaceWidget();
            int num = 2;
            StackPanelWidget stackPanelWidget = new StackPanelWidget();
            stackPanelWidget.Direction = LayoutDirection.Vertical;
            //new ValueTuple<float, float>
            stackPanelWidget.Alignment = new Vector2(0f, -1f);
            stackPanelWidget.Margin = new Vector2(10f, 10f);
            WidgetsList children2 = stackPanelWidget.Children;
            LabelWidget labelWidget = new LabelWidget();
            labelWidget.Alignment = new Vector2(0f, -1f);
            labelWidget.Margin = new Vector2(0f, 5f);
            LabelWidget widget = labelWidget;
            this.LargeLabelWidget = labelWidget;
            children2.Add(widget);
            WidgetsList children3 = stackPanelWidget.Children;
            ScrollPanelWidget scrollPanelWidget = new ScrollPanelWidget();
            scrollPanelWidget.Direction = LayoutDirection.Vertical;
            scrollPanelWidget.Margin = new Vector2(20f, 5f);
            scrollPanelWidget.Size = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            scrollPanelWidget.UseInitialScroll = false;
            WidgetsList children4 = scrollPanelWidget.Children;
            LabelWidget labelWidget2 = new LabelWidget();
            labelWidget2.Font = Fonts.Small;
            labelWidget2.Color = Colors.ForeDim;
            labelWidget2.Alignment = new Vector2(0f, -1f);
            labelWidget2.TextAnchor = TextAnchor.HorizontalCenter;
            labelWidget2.WordWrap = true;
            widget = labelWidget2;
            this.SmallLabelWidget = labelWidget2;
            children4.Add(widget);
            children3.Add(scrollPanelWidget);
            WidgetsList children5 = stackPanelWidget.Children;
            StackPanelWidget stackPanelWidget2 = new StackPanelWidget();
            stackPanelWidget2.Direction = LayoutDirection.Horizontal;
            stackPanelWidget2.Alignment = new Vector2(0f, 1f);
            stackPanelWidget2.Margin = new Vector2(0f, 5f);
            WidgetsList children6 = stackPanelWidget2.Children;
            BevelledButtonWidget bevelledButtonWidget = new BevelledButtonWidget();
            bevelledButtonWidget.Size = new Vector2(200f, 60f);
            bevelledButtonWidget.Margin = new Vector2(40f, 0f);
            bevelledButtonWidget.Sound = Sounds.Click2;
            ButtonWidget widget2 = bevelledButtonWidget;
            this.Button1Widget = bevelledButtonWidget;
            children6.Add(widget2);
            WidgetsList children7 = stackPanelWidget2.Children;
            BevelledButtonWidget bevelledButtonWidget2 = new BevelledButtonWidget();
            bevelledButtonWidget2.Size = new Vector2(200f, 60f);
            bevelledButtonWidget2.Margin = new Vector2(40f, 0f);
            bevelledButtonWidget2.Sound = Sounds.Click2;
            widget2 = bevelledButtonWidget2;
            this.Button2Widget = bevelledButtonWidget2;
            children7.Add(widget2);
            children5.Add(stackPanelWidget2);
            array[num] = stackPanelWidget;
            children.Add(array);
            this.LargeLabelWidget.IsVisible = !string.IsNullOrEmpty(largeMessage);
            this.LargeLabelWidget.Text = (largeMessage ?? string.Empty);
            this.SmallLabelWidget.IsVisible = !string.IsNullOrEmpty(smallMessage);
            this.SmallLabelWidget.Text = (smallMessage ?? string.Empty);
            this.Button1Widget.IsVisible = !string.IsNullOrEmpty(button1Text);
            this.Button1Widget.Text = (button1Text ?? string.Empty);
            this.Button2Widget.IsVisible = !string.IsNullOrEmpty(button2Text);
            this.Button2Widget.Text = (button2Text ?? string.Empty);
            if (!this.Button1Widget.IsVisible && !this.Button2Widget.IsVisible)
            {
                throw new InvalidOperationException("MessageDialog must have at least one button.");
            }
            this.AutoHide = true;
        }

        public MessageDialog(string largeMessage, string smallMessage, string button1Text, string button2Text, Action<MessageDialogButton> handler) : this(largeMessage, smallMessage, button1Text, button2Text, new Vector2(-1f), handler)
        {
        }

        public bool AutoHide { get; set; }

        public override void Update()
        {
            if (this.Button1Widget.IsVisible)
            {
                this.Button1Widget.IsOkButton = this.Button1Widget.IsVisible;
                this.Button1Widget.IsCancelButton = !this.Button2Widget.IsVisible;
                this.Button2Widget.IsOkButton = !this.Button1Widget.IsVisible;
                this.Button2Widget.IsCancelButton = this.Button2Widget.IsVisible;
            }
            if (this.Button1Widget.IsClicked)
            {
                this.Dismiss(MessageDialogButton.Button1);
                return;
            }
            if (this.Button2Widget.IsClicked)
            {
                this.Dismiss(MessageDialogButton.Button2);
            }
        }

        private void Dismiss(MessageDialogButton button)
        {
            if (this.AutoHide)
            {
                DialogsManager.HideDialog(this, true);
            }
            Action<MessageDialogButton> handler = this.Handler;
            if (handler == null)
            {
                return;
            }
            handler(button);
        }

        private Action<MessageDialogButton> Handler;

        private LabelWidget LargeLabelWidget;

        private LabelWidget SmallLabelWidget;

        private ButtonWidget Button1Widget;

        private ButtonWidget Button2Widget;
    }
}
