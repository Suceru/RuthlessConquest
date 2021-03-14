using System;
using System.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
    /// <summary>
    /// 保护类
    /// 正在等待玩家对话框
    /// 继承:对话框
    /// </summary>
    internal class WaitingForPlayersDialog : Dialog
    {
        public WaitingForPlayersDialog(Client client)
        {
            this.Client = client;
            Size = new Vector2(600f, 370f);
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
            stackPanelWidget.Margin = new Vector2(10f, 10f);
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 5f)
            });
            stackPanelWidget.Children.Add(new LabelWidget
            {
                Text = "WAITING FOR PLAYERS TO JOIN"
            });
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, float.PositiveInfinity)
            });
            WidgetsList children2 = stackPanelWidget.Children;
            StackPanelWidget stackPanelWidget2 = new StackPanelWidget();
            WidgetsList children3 = stackPanelWidget2.Children;
            StackPanelWidget stackPanelWidget3 = new StackPanelWidget();
            stackPanelWidget3.Direction = LayoutDirection.Vertical;
            stackPanelWidget3.Children.Add(new LabelWidget
            {
                Text = "WORLD",
                Font = Fonts.Small,
                Color = Colors.ForeDim
            });
            stackPanelWidget3.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 8f)
            });
            WidgetsList children4 = stackPanelWidget3.Children;
            RectangleWidget rectangleWidget = new RectangleWidget();
            rectangleWidget.Size = new Vector2(128f, 72f);
            rectangleWidget.FillColor = Color.White;
            rectangleWidget.OutlineColor = Color.Transparent;
            RectangleWidget widget = rectangleWidget;
            this.GameImageRectangle = rectangleWidget;
            children4.Add(widget);
            children3.Add(stackPanelWidget3);
            stackPanelWidget2.Children.Add(new CanvasWidget
            {
                Size = new Vector2(50f, 0f)
            });
            WidgetsList children5 = stackPanelWidget2.Children;
            StackPanelWidget stackPanelWidget4 = new StackPanelWidget();
            stackPanelWidget4.Direction = LayoutDirection.Vertical;
            StackPanelWidget widget2 = stackPanelWidget4;
            this.PlayersPanelWidget = stackPanelWidget4;
            children5.Add(widget2);
            children2.Add(stackPanelWidget2);
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 10f)
            });
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, float.PositiveInfinity)
            });
            WidgetsList children6 = stackPanelWidget.Children;
            StackPanelWidget stackPanelWidget5 = new StackPanelWidget();
            stackPanelWidget5.Direction = LayoutDirection.Horizontal;
            WidgetsList children7 = stackPanelWidget5.Children;
            BevelledButtonWidget bevelledButtonWidget = new BevelledButtonWidget();
            bevelledButtonWidget.Text = "START NOW";
            bevelledButtonWidget.Size = new Vector2(250f, 60f);
            bevelledButtonWidget.IsOkButton = true;
            bevelledButtonWidget.Sound = Sounds.Click2;
            bevelledButtonWidget.Margin = new Vector2(30f, 0f);
            ButtonWidget widget3 = bevelledButtonWidget;
            this.StartButtonWidget = bevelledButtonWidget;
            children7.Add(widget3);
            WidgetsList children8 = stackPanelWidget5.Children;
            BevelledButtonWidget bevelledButtonWidget2 = new BevelledButtonWidget();
            bevelledButtonWidget2.Text = "CANCEL";
            bevelledButtonWidget2.Size = new Vector2(200f, 60f);
            bevelledButtonWidget2.IsCancelButton = true;
            bevelledButtonWidget2.Sound = Sounds.Click2;
            bevelledButtonWidget2.Margin = new Vector2(30f, 0f);
            widget3 = bevelledButtonWidget2;
            this.CancelButtonWidget = bevelledButtonWidget2;
            children8.Add(widget3);
            WidgetsList children9 = stackPanelWidget5.Children;
            BevelledButtonWidget bevelledButtonWidget3 = new BevelledButtonWidget();
            bevelledButtonWidget3.Text = "DISCONNECT";
            bevelledButtonWidget3.Size = new Vector2(250f, 60f);
            bevelledButtonWidget3.IsCancelButton = true;
            bevelledButtonWidget3.Sound = Sounds.Click2;
            bevelledButtonWidget3.Margin = new Vector2(30f, 0f);
            widget3 = bevelledButtonWidget3;
            this.DisconnectButtonWidget = bevelledButtonWidget3;
            children9.Add(widget3);
            children6.Add(stackPanelWidget5);
            array[num] = stackPanelWidget;
            children.Add(array);
            if (this.Client.IsGameCreator)
            {
                this.DisconnectButtonWidget.IsVisible = false;
                return;
            }
            this.StartButtonWidget.IsVisible = false;
            this.CancelButtonWidget.IsVisible = false;
        }

        public override void Update()
        {
            if (this.Client.Game.StepModule.IsGameStarted)
            {
                DialogsManager.ShowDialog(this.Client.Game.Screen.GameWidget, new CountdownDialog(this.Client), false, new Color(0, 0, 0, 64));
                DialogsManager.HideDialog(this, true);
                return;
            }
            if (GameImageTexture == null)
            {
                GameImageTexture = new RenderTarget2D(192, 108, 1, ColorFormat.Rgba8888, DepthFormat.None);
            }
            GameImage.FromGame(this.Client.Game).Draw(GameImageTexture, true);
            this.GameImageRectangle.Subtexture = GameImageTexture;
            bool flag;
            if (this.Client.IsGameCreator)
            {
                flag = (this.Client.Game.PlayersModule.Players.Count((Player p) => p is HumanPlayer) >= this.Client.Game.CreationParameters.MaxHumanPlayersCount);
            }
            else
            {
                flag = false;
            }
            bool flag2 = flag;
            if (this.StartButtonWidget.IsClicked || (this.StartButtonWidget.IsEnabled && this.StartButtonWidget.IsVisible && flag2))
            {
                this.Client.StartGame();
                this.StartButtonWidget.IsEnabled = false;
                this.CancelButtonWidget.IsEnabled = false;
            }
            if (this.CancelButtonWidget.IsClicked)
            {
                this.Client.DisconnectFromGame();
                this.StartButtonWidget.IsEnabled = false;
                this.CancelButtonWidget.IsEnabled = false;
            }
            if (this.DisconnectButtonWidget.IsClicked)
            {
                this.Client.DisconnectFromGame();
                this.DisconnectButtonWidget.IsEnabled = false;
            }
            if (!this.Client.Game.PlayersModule.Players.SequenceEqual(this.Players))
            {
                this.PlayersPanelWidget.Children.Clear();
                foreach (Player player in this.Client.Game.PlayersModule.Players.OrderBy(delegate (Player p)
                {
                    if (!(p is HumanPlayer))
                    {
                        return 1;
                    }
                    return 0;
                }))
                {
                    string text;
                    Color color;
                    if (player is HumanPlayer)
                    {
                        text = (player.IsControllingPlayer ? (player.Name + " (YOU)") : player.Name);
                        color = Player.GetColor(player.Faction);
                    }
                    else
                    {
                        text = "EMPTY SLOT - " + player.Name;
                        color = Colors.ForeDim;
                    }
                    this.PlayersPanelWidget.Children.Add(new StackPanelWidget
                    {
                        Direction = LayoutDirection.Horizontal,
                        Alignment = new Vector2(-1f, 0f),
                        Margin = new Vector2(0f, 2f),
                        Children =
                        {
                            new RectangleWidget
                            {
                                Size = new Vector2(26f, 26f),
                                Subtexture = Ship.GetTexture(player.Faction),
                                FillColor = Player.GetColor(player.Faction),
                                OutlineColor = Color.Transparent
                            },
                            new CanvasWidget
                            {
                                Size = new Vector2(16f, 0f)
                            },
                            new LabelWidget
                            {
                                Text = text,
                                Font = Fonts.Small,
                                Color = color
                            },
                            new CanvasWidget
                            {
                                Size = new Vector2(8f, 0f)
                            },
                            new RectangleWidget
                            {
                                Size = new Vector2(20f, 20f),
                                Subtexture = Player.GetPlatformTexture(player.Platform),
                                IsVisible = (Player.GetPlatformTexture(player.Platform) != null),
                                FillColor = Player.GetColor(player.Faction),
                                OutlineColor = Color.Transparent
                            }
                        }
                    });
                }
            }
        }

        private static RenderTarget2D GameImageTexture;

        private Client Client;

        private DynamicArray<Player> Players = new DynamicArray<Player>();

        private RectangleWidget GameImageRectangle;

        private StackPanelWidget PlayersPanelWidget;

        private ButtonWidget StartButtonWidget;

        private ButtonWidget CancelButtonWidget;

        private ButtonWidget DisconnectButtonWidget;
    }
}
