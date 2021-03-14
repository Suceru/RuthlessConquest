using System;
using System.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
    /// <summary>
    /// 保护类
    /// 游戏结束对话框
    /// 继承:对话框
    /// </summary>
    internal class GameResultDialog : Dialog
    {
        public GameResultDialog(HumanPlayer player)
        {
            Size = new Vector2(500f, 250f);
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
            stackPanelWidget.Alignment = new Vector2(0f, -1f);
            stackPanelWidget.Margin = new Vector2(10f, 10f);
            WidgetsList children2 = stackPanelWidget.Children;
            LabelWidget labelWidget = new LabelWidget();
            labelWidget.Margin = new Vector2(0f, 5f);
            LabelWidget widget = labelWidget;
            this.LargeLabelWidget = labelWidget;
            children2.Add(widget);
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, float.PositiveInfinity)
            });
            WidgetsList children3 = stackPanelWidget.Children;
            LabelWidget labelWidget2 = new LabelWidget();
            labelWidget2.Font = Fonts.Small;
            labelWidget2.Color = Colors.ForeDim;
            labelWidget2.TextAnchor = TextAnchor.HorizontalCenter;
            labelWidget2.WordWrap = true;
            widget = labelWidget2;
            this.SmallLabelWidget = labelWidget2;
            children3.Add(widget);
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, float.PositiveInfinity)
            });
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 20f)
            });
            WidgetsList children4 = stackPanelWidget.Children;
            StackPanelWidget stackPanelWidget2 = new StackPanelWidget();
            stackPanelWidget2.Direction = LayoutDirection.Horizontal;
            stackPanelWidget2.Margin = new Vector2(0f, 5f);
            WidgetsList children5 = stackPanelWidget2.Children;
            BevelledButtonWidget bevelledButtonWidget = new BevelledButtonWidget();
            bevelledButtonWidget.Size = new Vector2(200f, 60f);
            bevelledButtonWidget.Margin = new Vector2(20f, 0f);
            bevelledButtonWidget.Sound = Sounds.Click2;
            BevelledButtonWidget widget2 = bevelledButtonWidget;
            this.Button1Widget = bevelledButtonWidget;
            children5.Add(widget2);
            WidgetsList children6 = stackPanelWidget2.Children;
            BevelledButtonWidget bevelledButtonWidget2 = new BevelledButtonWidget();
            bevelledButtonWidget2.Size = new Vector2(200f, 60f);
            bevelledButtonWidget2.Margin = new Vector2(20f, 0f);
            bevelledButtonWidget2.Sound = Sounds.Click2;
            widget2 = bevelledButtonWidget2;
            this.Button2Widget = bevelledButtonWidget2;
            children6.Add(widget2);
            children4.Add(stackPanelWidget2);
            array[num] = stackPanelWidget;
            children.Add(array);
            switch (player.Status)
            {
                case FactionStatus.WonEliminatedOthers:
                    //击败其他玩家
                    this.LargeLabelWidget.Text = "VICTORY!";
                    this.SmallLabelWidget.Text = "You have crushed all your enemies.";
                    this.Button1Widget.Text = "FABULOUS";
                    this.Button1Widget.IsCancelButton = true;
                    this.Button1Widget.IsOkButton = true;
                    this.Button2Widget.IsVisible = false;
                    AudioManager.PlaySound(Sounds.Victory, false, 1f, 1f, 0f);
                    return;
                case FactionStatus.Won:
                    //胜利
                    this.LargeLabelWidget.Text = "VICTORY!";
                    this.SmallLabelWidget.Text = "You have captured all special planets.";
                    this.Button1Widget.Text = "NEAT";
                    this.Button1Widget.IsCancelButton = true;
                    this.Button1Widget.IsOkButton = true;
                    this.Button2Widget.IsVisible = false;
                    AudioManager.PlaySound(Sounds.Victory, false, 1f, 1f, 0f);
                    return;
                case FactionStatus.Lost:
                    //失败
                    this.LargeLabelWidget.Text = "DEFEAT!";
                    Player player2 = player.Game.PlayersModule.Players.FirstOrDefault((Player p) => p.Status == FactionStatus.Won);
                    if (player2 != null)
                    {
                        this.SmallLabelWidget.Text = player2.Name + " has managed to capture all special planets.";
                        this.SmallLabelWidget.Color = Player.GetColor(player2.Faction);
                    }
                    this.Button1Widget.Text = "WHAT?!";
                    this.Button1Widget.IsCancelButton = true;
                    this.Button1Widget.IsOkButton = true;
                    this.Button2Widget.IsVisible = false;
                    AudioManager.PlaySound(Sounds.Defeat, false, 1f, 1f, 0f);
                    return;
                case FactionStatus.LostEliminated:
                    //被击败
                    this.LargeLabelWidget.Text = "DEFEAT!";
                    this.SmallLabelWidget.Text = "Your empire has crumbled to dust.";
                    this.Button1Widget.Text = "LEAVE";
                    this.Button1Widget.IsOkButton = true;
                    this.Button2Widget.Text = "SPECTATE";
                    this.Button2Widget.IsCancelButton = true;
                    AudioManager.PlaySound(Sounds.Defeat, false, 1f, 1f, 0f);
                    return;
            }
            /*            if (player.Status == FactionStatus.WonEliminatedOthers)
                        {
                            this.LargeLabelWidget.Text = "VICTORY!";
                            this.SmallLabelWidget.Text = "You have crushed all your enemies.";
                            this.Button1Widget.Text = "FABULOUS";
                            this.Button1Widget.IsCancelButton = true;
                            this.Button1Widget.IsOkButton = true;
                            this.Button2Widget.IsVisible = false;
                            AudioManager.PlaySound(Sounds.Victory, false, 1f, 1f, 0f);
                            return;
                        }
                        if (player.Status == FactionStatus.Won)
                        {
                            this.LargeLabelWidget.Text = "VICTORY!";
                            this.SmallLabelWidget.Text = "You have captured all special planets.";
                            this.Button1Widget.Text = "NEAT";
                            this.Button1Widget.IsCancelButton = true;
                            this.Button1Widget.IsOkButton = true;
                            this.Button2Widget.IsVisible = false;
                            AudioManager.PlaySound(Sounds.Victory, false, 1f, 1f, 0f);
                            return;
                        }
                        if (player.Status == FactionStatus.Lost)
                        {
                            this.LargeLabelWidget.Text = "DEFEAT!";
                            Player player2 = player.Game.PlayersModule.Players.FirstOrDefault((Player p) => p.Status == FactionStatus.Won);
                            if (player2 != null)
                            {
                                this.SmallLabelWidget.Text = player2.Name + " has managed to capture all special planets.";
                                this.SmallLabelWidget.Color = Player.GetColor(player2.Faction);
                            }
                            this.Button1Widget.Text = "WHAT?!";
                            this.Button1Widget.IsCancelButton = true;
                            this.Button1Widget.IsOkButton = true;
                            this.Button2Widget.IsVisible = false;
                            AudioManager.PlaySound(Sounds.Defeat, false, 1f, 1f, 0f);
                            return;
                        }
                        if (player.Status == FactionStatus.LostEliminated)
                        {
                            this.LargeLabelWidget.Text = "DEFEAT!";
                            this.SmallLabelWidget.Text = "Your empire has crumbled to dust.";
                            this.Button1Widget.Text = "LEAVE";
                            this.Button1Widget.IsOkButton = true;
                            this.Button2Widget.Text = "SPECTATE";
                            this.Button2Widget.IsCancelButton = true;
                            AudioManager.PlaySound(Sounds.Defeat, false, 1f, 1f, 0f);
                        }
            */
        }

        public override void Update()
        {
            if (Input.Cancel)
            {
                if (this.Button2Widget.IsVisible)
                {
                    this.Dismiss(MessageDialogButton.Button2);
                    return;
                }
                this.Dismiss(MessageDialogButton.Button1);
                return;
            }
            else
            {
                if (Input.Ok || this.Button1Widget.IsClicked)
                {
                    this.Dismiss(MessageDialogButton.Button1);
                    return;
                }
                if (this.Button2Widget.IsClicked)
                {
                    this.Dismiss(MessageDialogButton.Button2);
                }
                return;
            }
        }

        private void Dismiss(MessageDialogButton button)
        {
            DialogsManager.HideDialog(this, true);
            if (button == MessageDialogButton.Button1)
            {
                ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
            }
        }

        private LabelWidget LargeLabelWidget;

        private LabelWidget SmallLabelWidget;

        private BevelledButtonWidget Button1Widget;

        private BevelledButtonWidget Button2Widget;
    }
}
