using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
    internal class PlayerLabelWidget : CanvasWidget
    {
        public bool IsSatelliteButtonClicked
        {
            get
            {
                return this.SatelliteClickableWidget != null && this.SatelliteClickableWidget.IsClicked;
            }
        }

        public Player Player
        {
            get
            {
                return this.InternalPlayer;
            }
            set
            {
                this.InternalPlayer = value;
                this.IsVisible = (value != null);
            }
        }

        public PlayerLabelWidget(GameScreen gameScreen, int labelIndex)
        {
            this.GameScreen = gameScreen;
            this.LabelIndex = labelIndex;
            Size = new Vector2(-1f, -1f);
            this.IsHitTestVisible = false;
            RectangleWidget rectangleWidget2;
            LabelWidget widget;
            if (this.LabelIndex == 0)
            {
                WidgetsList children = this.Children;
                Widget[] array = new Widget[6];
                int num = 0;
                RectangleWidget rectangleWidget = new RectangleWidget();
                rectangleWidget.Size = new Vector2(362f, 34f);
                rectangleWidget.Subtexture = Textures.Gui.PlayerPanel;
                rectangleWidget.OutlineColor = Color.Transparent;
                rectangleWidget2 = rectangleWidget;
                this.PanelRectangle = rectangleWidget;
                array[num] = rectangleWidget2;
                int num2 = 1;
                ClickableWidget clickableWidget = new ClickableWidget();
                clickableWidget.Sound = Sounds.Click;
                clickableWidget.Size = new Vector2(80f, 34f);
                clickableWidget.Alignment = new Vector2(-1f, 0f);
                clickableWidget.IsEnabled = false;
                ClickableWidget clickableWidget2 = clickableWidget;
                this.SatelliteClickableWidget = clickableWidget;
                array[num2] = clickableWidget2;
                int num3 = 2;
                ClickableWidget clickableWidget3 = new ClickableWidget();
                clickableWidget3.Sound = Sounds.Click;
                clickableWidget3.Size = new Vector2(80f, 34f);
                clickableWidget3.Alignment = new Vector2(1f, 0f);
                clickableWidget3.IsCancelButton = true;
                clickableWidget2 = clickableWidget3;
                this.MenuClickableWidget = clickableWidget3;
                array[num3] = clickableWidget2;
                int num4 = 3;
                StackPanelWidget stackPanelWidget = new StackPanelWidget();
                stackPanelWidget.Margin = new Vector2(0f, 3f);
                stackPanelWidget.Alignment = new Vector2(0f, -1f);
                WidgetsList children2 = stackPanelWidget.Children;
                LabelWidget labelWidget = new LabelWidget();
                labelWidget.Font = Fonts.Small;
                labelWidget.DropShadow = true;
                labelWidget.Margin = new Vector2(6f, 0f);
                widget = labelWidget;
                this.Label = labelWidget;
                children2.Add(widget);
                WidgetsList children3 = stackPanelWidget.Children;
                RectangleWidget rectangleWidget3 = new RectangleWidget();
                rectangleWidget3.Size = new Vector2(16f, 16f);
                rectangleWidget3.OutlineColor = Color.Transparent;
                rectangleWidget2 = rectangleWidget3;
                this.PlatformRectangle = rectangleWidget3;
                children3.Add(rectangleWidget2);
                array[num4] = stackPanelWidget;
                int num5 = 4;
                RectangleWidget rectangleWidget4 = new RectangleWidget();
                rectangleWidget4.Size = new Vector2(26f, 26f);
                rectangleWidget4.Margin = new Vector2(18f, 2f);
                rectangleWidget4.Alignment = new Vector2(-1f, -1f);
                rectangleWidget4.Subtexture = Textures.Gui.Satellite;
                rectangleWidget4.FillColor = Colors.ForeDisabled;
                rectangleWidget4.OutlineColor = Color.Transparent;
                rectangleWidget2 = rectangleWidget4;
                this.SatelliteRectangle = rectangleWidget4;
                array[num5] = rectangleWidget2;
                int num6 = 5;
                RectangleWidget rectangleWidget5 = new RectangleWidget();
                rectangleWidget5.Size = new Vector2(26f, 26f);
                rectangleWidget5.Margin = new Vector2(18f, 2f);
                rectangleWidget5.Alignment = new Vector2(1f, -1f);
                rectangleWidget5.Subtexture = Textures.Gui.Menu;
                rectangleWidget5.FillColor = Colors.Fore;
                rectangleWidget5.OutlineColor = Color.Transparent;
                rectangleWidget2 = rectangleWidget5;
                this.MenuRectangle = rectangleWidget5;
                array[num6] = rectangleWidget2;
                children.Add(array);
                return;
            }
            WidgetsList children4 = this.Children;
            StackPanelWidget stackPanelWidget2 = new StackPanelWidget();
            stackPanelWidget2.IsHitTestVisible = false;
            stackPanelWidget2.Margin = new Vector2(6f, 2f);
            WidgetsList children5 = stackPanelWidget2.Children;
            RectangleWidget rectangleWidget6 = new RectangleWidget();
            rectangleWidget6.Size = new Vector2(20f, 20f);
            rectangleWidget6.OutlineColor = Color.Transparent;
            rectangleWidget2 = rectangleWidget6;
            this.ShipRectangle = rectangleWidget6;
            children5.Add(rectangleWidget2);
            WidgetsList children6 = stackPanelWidget2.Children;
            LabelWidget labelWidget2 = new LabelWidget();
            labelWidget2.Font = Fonts.Small;
            labelWidget2.DropShadow = true;
            labelWidget2.Margin = new Vector2(6f, 0f);
            widget = labelWidget2;
            this.Label = labelWidget2;
            children6.Add(widget);
            WidgetsList children7 = stackPanelWidget2.Children;
            RectangleWidget rectangleWidget7 = new RectangleWidget();
            rectangleWidget7.Size = new Vector2(16f, 16f);
            rectangleWidget7.OutlineColor = Color.Transparent;
            rectangleWidget2 = rectangleWidget7;
            this.PlatformRectangle = rectangleWidget7;
            children7.Add(rectangleWidget2);
            StackPanelWidget widget2 = stackPanelWidget2;
            this.StackPanelWidget = stackPanelWidget2;
            children4.Add(widget2);
        }

        public override void Update()
        {
            if (this.Player == null || this.Player.Game == null)
            {
                return;
            }
            this.UpdateAlignment();
            bool flag = this.Player.Status < FactionStatus.Undecided;
            if (this.PanelRectangle != null)
            {
                this.PanelRectangle.FillColor = Player.GetColor(this.Player.Faction);
            }
            if (this.ShipRectangle != null)
            {
                this.ShipRectangle.Subtexture = (flag ? Textures.Skull : Ship.GetTexture(this.Player.Faction));
                this.ShipRectangle.FillColor = Ship.GetColor(this.Player.Faction);
            }
            this.Label.Color = Player.GetColor(flag ? Faction.Neutral : this.Player.Faction);
            this.PlatformRectangle.Subtexture = Player.GetPlatformTexture(this.Player.Platform);
            this.PlatformRectangle.IsVisible = (Player.GetPlatformTexture(this.Player.Platform) != null);
            this.PlatformRectangle.FillColor = Player.GetColor(flag ? Faction.Neutral : this.Player.Faction);
            this.Label.Text = ((this.Player.Faction == Faction.None) ? "Spectating" : this.Player.Name);
            if (this.MenuRectangle != null)
            {
                this.MenuRectangle.FillColor = ((this.MenuClickableWidget != null && this.MenuClickableWidget.IsPressed) ? Colors.High : Colors.Fore);
                //new ValueTuple<float, float, float>
                this.MenuRectangle.RenderTransform = Matrix.CreateTranslation(new Vector3(-this.MenuRectangle.ActualSize.X / 2f, -this.MenuRectangle.ActualSize.Y / 2f, 0f)) * Matrix.CreateScale((float)MathUtils.Max(MathUtils.Sin(2.0 * Time.RealTime) - 0.800000011920929, 0.0) + 1f) * Matrix.CreateTranslation(new Vector3(this.MenuRectangle.ActualSize.X / 2f, this.MenuRectangle.ActualSize.Y / 2f, 0f));
            }
            if (this.MenuClickableWidget != null && this.MenuClickableWidget.IsClicked)
            {
                DialogsManager.ShowDialog(null, new GameMenuDialog(this.GameScreen, false), true);
            }
            if (this.SatelliteClickableWidget != null)
            {
                this.SatelliteClickableWidget.IsEnabled = (this.Player.Game.StepModule.IsGameStarted && this.Player.Game.StepModule.CountdownStepsLeft <= 0 && this.Player.Faction != Faction.None && this.Player.Status >= FactionStatus.Undecided);
            }
            if (this.SatelliteRectangle != null)
            {
                this.SatelliteRectangle.FillColor = ((this.SatelliteClickableWidget != null && this.SatelliteClickableWidget.IsPressed) ? Colors.Fore : ((this.Player.AreSatellitesEnabled && this.SatelliteClickableWidget.IsEnabled) ? Ship.GetColor(this.Player.Faction) : Colors.ForeDisabled));
            }
        }

        private void UpdateAlignment()
        {
            int num = 0;
            using (IEnumerator<PlayerLabelWidget> enumerator = this.GameScreen.PlayerLabels.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsVisible)
                    {
                        num++;
                    }
                }
            }
            if (num == 1)
            {
                if (this.LabelIndex == 0)
                {
                    this.Alignment = new Vector2(0f, -1f);
                    return;
                }
            }
            else if (num == 2)
            {
                int labelIndex = this.LabelIndex;
                if (labelIndex == 0)
                {
                    this.Alignment = new Vector2(0f, -1f);
                    return;
                }
                if (labelIndex != 1)
                {
                    return;
                }
                this.Alignment = new Vector2(0f, 1f);
                return;
            }
            else if (num == 3)
            {
                switch (this.LabelIndex)
                {
                    case 0:
                        this.Alignment = new Vector2(0f, -1f);
                        return;
                    case 1:
                        this.Alignment = new Vector2(-1f, 1f);
                        return;
                    case 2:
                        this.Alignment = new Vector2(1f, 1f);
                        return;
                    default:
                        return;
                }
            }
            else if (num == 4)
            {
                switch (this.LabelIndex)
                {
                    case 0:
                        this.Alignment = new Vector2(0f, -1f);
                        return;
                    case 1:
                        this.Alignment = new Vector2(-1f, 1f);
                        return;
                    case 2:
                        this.Alignment = new Vector2(1f, 1f);
                        return;
                    case 3:
                        this.Alignment = new Vector2(0f, 1f);
                        return;
                    default:
                        return;
                }
            }
            else if (num == 5)
            {
                switch (this.LabelIndex)
                {
                    case 0:
                        this.Alignment = new Vector2(0f, -1f);
                        return;
                    case 1:
                        this.Alignment = new Vector2(-1f, -1f);
                        return;
                    case 2:
                        this.Alignment = new Vector2(1f, -1f);
                        return;
                    case 3:
                        this.Alignment = new Vector2(-1f, 1f);
                        return;
                    case 4:
                        this.Alignment = new Vector2(1f, 1f);
                        return;
                    default:
                        return;
                }
            }
            else if (num == 6)
            {
                switch (this.LabelIndex)
                {
                    case 0:
                        this.Alignment = new Vector2(0f, -1f);
                        return;
                    case 1:
                        this.Alignment = new Vector2(-1f, -1f);
                        return;
                    case 2:
                        this.Alignment = new Vector2(1f, -1f);
                        return;
                    case 3:
                        this.Alignment = new Vector2(0f, 1f);
                        return;
                    case 4:
                        this.Alignment = new Vector2(-1f, 1f);
                        return;
                    case 5:
                        this.Alignment = new Vector2(1f, 1f);
                        return;
                    default:
                        return;
                }
            }
            else if (num >= 7)
            {
                switch (this.LabelIndex)
                {
                    case 0:
                        this.Alignment = new Vector2(0f, -1f);
                        return;
                    case 1:
                        this.Alignment = new Vector2(-1f, -1f);
                        return;
                    case 2:
                        this.Alignment = new Vector2(1f, -1f);
                        return;
                    case 3:
                        this.Alignment = new Vector2(-1f, 1f);
                        return;
                    case 4:
                        this.Alignment = new Vector2(-0.33f, 1f);
                        return;
                    case 5:
                        this.Alignment = new Vector2(0.33f, 1f);
                        return;
                    case 6:
                        this.Alignment = new Vector2(1f, 1f);
                        break;
                    default:
                        return;
                }
            }
        }

        private GameScreen GameScreen;

        private StackPanelWidget StackPanelWidget;

        private RectangleWidget PanelRectangle;

        private RectangleWidget ShipRectangle;

        private LabelWidget Label;

        private RectangleWidget PlatformRectangle;

        private RectangleWidget SatelliteRectangle;

        private ClickableWidget SatelliteClickableWidget;

        private RectangleWidget MenuRectangle;

        private ClickableWidget MenuClickableWidget;

        private int LabelIndex;

        private Player InternalPlayer;
    }
}
