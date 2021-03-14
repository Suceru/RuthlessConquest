using System;
using Engine;

namespace Game
{
    internal class ReinforcementsMessageWidget : CanvasWidget
    {
        public ReinforcementsMessageWidget()
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

        public void SetMessage(Faction faction, string text)
        {
            this.Faction = faction;
            this.MessageTime = Time.FrameStartTime;
            this.LabelWidget.Text = text;
            this.LabelWidget.Color = Color.Transparent;
            this.IsVisible = true;
        }

        public override void Update()
        {
            float num = (float)(Time.FrameStartTime - this.MessageTime);
            float num2 = MathUtils.Saturate(MathUtils.Min(6f * num, 0.4f * (5f - num)));
            this.LabelWidget.Color = Player.GetColor(this.Faction) * num2;
            if (num2 <= 0f)
            {
                this.IsVisible = false;
            }
        }

        private LabelWidget LabelWidget;

        private double MessageTime;

        private Faction Faction;
    }
}
