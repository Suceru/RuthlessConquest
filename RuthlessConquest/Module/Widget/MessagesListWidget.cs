using System;
using System.Collections.Generic;
using Engine;
using Engine.Graphics;

namespace Game
{
    public class MessagesListWidget : StackPanelWidget
    {
        public MessagesListWidget()
        {
            this.IsHitTestVisible = false;
            Direction = LayoutDirection.Vertical;
        }

        public void AddMessage(string text, Color color, bool playSound)
        {
            this.AddMessage(new MessagesListWidget.Message
            {
                CreationTime = Time.FrameStartTime,
                Color = color,
                LabelWidget = new LabelWidget
                {
                    Font = Fonts.Small,
                    Text = text,
                    TextAnchor = TextAnchor.HorizontalCenter,
                    DropShadow = true
                }
            });
            if (playSound)
            {
                AudioManager.PlaySound(Sounds.Message, true, 1f, 1f, 0f);
            }
        }

        public void ClearMessages()
        {
            foreach (MessagesListWidget.Message message in this.Messages.ToArray())
            {
                this.RemoveMessage(message);
            }
        }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            foreach (MessagesListWidget.Message message in this.Messages.ToArray())
            {
                float num = 5f;
                float num2 = num - (float)(Time.FrameStartTime - message.CreationTime);
                message.LabelWidget.Color = message.Color * MathUtils.Saturate(0.5f * num2) * MathUtils.Saturate(3f * (num - num2));
                if (num2 <= 0f)
                {
                    this.RemoveMessage(message);
                }
            }
            base.MeasureOverride(parentAvailableSize);
        }

        private void AddMessage(MessagesListWidget.Message message)
        {
            this.Messages.Add(message);
            this.Children.Add(message.LabelWidget);
        }

        private void RemoveMessage(MessagesListWidget.Message message)
        {
            this.Messages.Remove(message);
            this.Children.Remove(message.LabelWidget);
        }

        private List<MessagesListWidget.Message> Messages = new List<MessagesListWidget.Message>();

        private struct Message
        {
            public double CreationTime;

            public Color Color;

            public LabelWidget LabelWidget;
        }
    }
}
