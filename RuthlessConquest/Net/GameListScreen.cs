using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Engine;
using Engine.Graphics;

namespace Game
{
    internal class GameListScreen : Screen
    {
        public GameListScreen()
        {
            IsDrawRequired = true;
            WidgetsList children = this.Children;
            Widget[] array = new Widget[4];
            array[0] = new BackgroundWidget();
            array[1] = new BoidsWidget();
            array[2] = new InterlaceWidget();
            int num = 3;
            StackPanelWidget stackPanelWidget = new StackPanelWidget();
            stackPanelWidget.Direction = LayoutDirection.Vertical;
            stackPanelWidget.Margin = new Vector2(20f, 0f);
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 10f)
            });
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(float.PositiveInfinity, -1f),
                Children =
                {
                    new LabelWidget
                    {
                        Text = "Searching for games...",
                        Font = Fonts.Small,
                        Color = Colors.Fore
                    },
                    new BusyBarWidget
                    {
                        Alignment = new Vector2(1f, 0f),
                        LitBarColor = Colors.High
                    }
                }
            });
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 10f)
            });
            WidgetsList children2 = stackPanelWidget.Children;
            CanvasWidget canvasWidget = new CanvasWidget();
            canvasWidget.Size = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            canvasWidget.Children.Add(new RectangleWidget());
            canvasWidget.Children.Add(new InterlaceWidget());
            WidgetsList children3 = canvasWidget.Children;
            ListPanelWidget listPanelWidget = new ListPanelWidget();
            listPanelWidget.Direction = LayoutDirection.Vertical;
            listPanelWidget.ItemSize = 82f;
            listPanelWidget.Margin = new Vector2(2f, 2f);
            listPanelWidget.UseInitialScroll = false;
            ListPanelWidget widget = listPanelWidget;
            this.GamesList = listPanelWidget;
            children3.Add(widget);
            children2.Add(canvasWidget);
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 20f)
            });

            WidgetsList children4 = stackPanelWidget.Children;
            StackPanelWidget stackPanelWidget2 = new StackPanelWidget();
            stackPanelWidget2.Direction = LayoutDirection.Horizontal;

            WidgetsList children5 = stackPanelWidget2.Children;
            BevelledButtonWidget bevelledButtonWidget = new BevelledButtonWidget();
            bevelledButtonWidget.Text = "JOIN GAME";
            bevelledButtonWidget.Size = new Vector2(240f, 60f);
            bevelledButtonWidget.Margin = new Vector2(20f, 0f);
            ButtonWidget widget2 = bevelledButtonWidget;
            this.JoinButton = bevelledButtonWidget;
            children5.Add(widget2);

            WidgetsList children6 = stackPanelWidget2.Children;
            BevelledButtonWidget bevelledButtonWidget2 = new BevelledButtonWidget();
            bevelledButtonWidget2.Text = "SPECTATE";
            bevelledButtonWidget2.Size = new Vector2(200f, 60f);
            bevelledButtonWidget2.Margin = new Vector2(20f, 0f);
            widget2 = bevelledButtonWidget2;
            this.SpectateButton = bevelledButtonWidget2;
            children6.Add(widget2);

            WidgetsList children7 = stackPanelWidget2.Children;
            BevelledButtonWidget bevelledButtonWidget3 = new BevelledButtonWidget();
            bevelledButtonWidget3.Text = "CREATE GAME";
            bevelledButtonWidget3.Size = new Vector2(280f, 60f);
            bevelledButtonWidget3.Margin = new Vector2(20f, 0f);
            widget2 = bevelledButtonWidget3;
            this.CreateButton = bevelledButtonWidget3;
            children7.Add(widget2);

            WidgetsList children8 = stackPanelWidget2.Children;
            BevelledButtonWidget bevelledButtonWidget4 = new BevelledButtonWidget();
            bevelledButtonWidget4.Text = "BACK";
            bevelledButtonWidget4.Size = new Vector2(200f, 60f);
            bevelledButtonWidget4.Margin = new Vector2(20f, 0f);
            bevelledButtonWidget4.IsCancelButton = true;
            bevelledButtonWidget4.Sound = Sounds.Click2;
            widget2 = bevelledButtonWidget4;
            this.BackButton = bevelledButtonWidget4;
            children8.Add(widget2);
            children4.Add(stackPanelWidget2);

            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 10f)
            });
            array[num] = stackPanelWidget;
            children.Add(array);
            this.GamesList.ItemWidgetFactory = delegate (object item)
            {
                GameDescription gameDescription = (GameDescription)item;
                string text;
                string text2;
                string text3;
                //本地局域网
                if (gameDescription.ServerDescription.IsLocal)
                {
                    text = "WIFI / LAN GAME";
                    text2 = " by " + gameDescription.CreationParameters.CreatingPlayerName;
                    text3 = "@ " + ShortenAddress(gameDescription.ServerDescription.Address.Address);
                }
                else
                {
                    text = "INTERNET GAME";
                    text2 = " by " + gameDescription.CreationParameters.CreatingPlayerName;
                    text3 = "@ " + gameDescription.ServerDescription.Name;
                }
                string text4 = string.Format("{0} planets", gameDescription.CreationParameters.PlanetsCount);
                text4 = text4 + " | range " + Ship.GetShipRangeName((ShipRange)gameDescription.CreationParameters.ShipRange).ToLower();
                text4 = text4 + " | AI " + Player.GetPlayerTypeName(gameDescription.CreationParameters.AILevel).ToLower();
                text4 += string.Format(" | population {0}%", gameDescription.CreationParameters.NeutralsPopulationFactor);
                string text5 = string.Format("{0}/{1} players", gameDescription.HumanPlayersCount, gameDescription.CreationParameters.MaxHumanPlayersCount);
                text5 += string.Format(" | {0} spectators", gameDescription.SpectatorsCount);
                if (gameDescription.ServerDescription.Ping > 0f && gameDescription.ServerDescription.Ping < 3f)
                {
                    text5 += string.Format(" | ping {0:0}ms", gameDescription.ServerDescription.Ping * 1000f);
                }
                else
                {
                    text5 += " | ping unknown";
                }
                if (gameDescription.TicksSinceStart > 0)
                {
                    int num2 = (int)(gameDescription.TicksSinceStart * 0.5f);
                    text5 += string.Format(" | started {0:00}:{1:00} ago", num2 / 60, num2 % 60);
                }
                Texture2D texture2D;
                if (!this.GameImageTexturesCache.TryGetValue(gameDescription, out texture2D))
                {
                    //new ValueTuple<int,int>
                    texture2D = ((gameDescription.GameImage != null) ? gameDescription.GameImage.Draw(new Point2(192, 108), gameDescription.TicksSinceStart < 0) : null);
                    this.GameImageTexturesCache.Add(gameDescription, texture2D);
                }
                return new StackPanelWidget
                {
                    Alignment = new Vector2(-1f, 0f),
                    Children =
                    {
                        new CanvasWidget
                        {
                            Size = new Vector2(8f, 0f)
                        },
                        new CanvasWidget
                        {
                            Children =
                            {
                                new RectangleWidget
                                {
                                    Size = new Vector2(128f, 72f),
                                    FillColor = Color.Black * 0.75f,
                                    OutlineColor = Color.Transparent,
                                    IsVisible = (texture2D == null)
                                },
                                new RectangleWidget
                                {
                                    Size = new Vector2(128f, 72f),
                                    Subtexture = texture2D,
                                    FillColor = Color.White,
                                    OutlineColor = Color.Transparent,
                                    IsVisible = (texture2D != null)
                                }
                            }
                        },
                        new CanvasWidget
                        {
                            Size = new Vector2(20f, 0f)
                        },
                        new StackPanelWidget
                        {
                            Direction = LayoutDirection.Vertical,
                            Alignment = new Vector2(-1f, 0f),
                            Children =
                            {
                                new StackPanelWidget
                                {
                                    Alignment = new Vector2(-1f, 0f),
                                    Children =
                                    {
                                        new LabelWidget
                                        {
                                            Text = text,
                                            Font = Fonts.Small
                                        },
                                        new LabelWidget
                                        {
                                            Text = text2,
                                            Font = Fonts.Small,
                                            Color = Colors.ForeDim
                                        },
                                        new RectangleWidget
                                        {
                                            Size = new Vector2(16f, 16f),
                                            Margin = new Vector2(4f, 0f),
                                            Subtexture = Player.GetPlatformTexture(new Platform?(gameDescription.CreationParameters.CreatingPlayerPlatform)),
                                            FillColor = Colors.ForeDim,
                                            OutlineColor = Color.Transparent,
                                            IsVisible = (Player.GetPlatformTexture(new Platform?(gameDescription.CreationParameters.CreatingPlayerPlatform)) != null)
                                        },
                                        new LabelWidget
                                        {
                                            Text = text3,
                                            Font = Fonts.Small,
                                            Color = Colors.ForeDim
                                        }
                                    }
                                },
                                new LabelWidget
                                {
                                    Text = text4,
                                    Font = Fonts.Small,
                                    Alignment = new Vector2(-1f, 0f),
                                    Color = Colors.ForeDim
                                },
                                new LabelWidget
                                {
                                    Text = text5,
                                    Font = Fonts.Small,
                                    Alignment = new Vector2(-1f, 0f),
                                    Color = Colors.ForeDim
                                }
                            }
                        }
                    }
                };
            };
        }

        public override void Enter(object[] parameters)
        {
            this.UpgradeDialogShown = false;
            this.GamesList.ClearItems();
            ServersManager.StartServerDiscovery(3.0, 1.0);
        }

        public override void Leave()
        {
            ServersManager.StopServerDiscovery();
        }

        public override void Update()
        {
            if (this.BackButton.IsClicked)
            {
                ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
                return;
            }
            if (this.CreateButton.IsClicked)
            {
                ScreensManager.SwitchScreen("GameSetup", Array.Empty<object>());
                return;
            }
            if (this.JoinButton.IsClicked && this.GamesList.SelectedItem != null)
            {
                NamesManager.EnsureValidPlayerNameExists(delegate
                {
                    DialogsManager.ShowDialog(this, new ConnectingDialog((GameDescription)this.GamesList.SelectedItem, false), true);
                });
            }
            if (this.SpectateButton.IsClicked && this.GamesList.SelectedItem != null)
            {
                NamesManager.EnsureValidPlayerNameExists(delegate
                {
                    DialogsManager.ShowDialog(this, new ConnectingDialog((GameDescription)this.GamesList.SelectedItem, true), true);
                });
            }
            if (Time.PeriodicEvent(0.25, 0.0))
            {
                GameDescription gameDescription = (GameDescription)this.GamesList.SelectedItem;
                IOrderedEnumerable<GameDescription> orderedEnumerable = from s in ServersManager.DiscoveredServers.SelectMany((ServerDescription s) => s.GameDescriptions)
                                                                        orderby (s.ServerDescription.IsLocal ? "0" : "1") + s.TicksSinceStart.ToString("D10") + s.GameId.ToString("D10")
                                                                        select s;
                this.GamesList.ClearItems();
                foreach (GameDescription gameDescription2 in orderedEnumerable)
                {
                    this.GamesList.AddItem(gameDescription2);
                    if (gameDescription != null && Equals(gameDescription2.ServerDescription.Address, gameDescription.ServerDescription.Address) && gameDescription2.GameId == gameDescription.GameId)
                    {
                        this.GamesList.SelectedIndex = new int?(this.GamesList.Items.Count - 1);
                    }
                }
                foreach (KeyValuePair<GameDescription, Texture2D> keyValuePair in this.GameImageTexturesCache.ToArray<KeyValuePair<GameDescription, Texture2D>>())
                {
                    if (!orderedEnumerable.Contains(keyValuePair.Key))
                    {
                        if (keyValuePair.Value != null)
                        {
                            keyValuePair.Value.Dispose();
                        }
                        this.GameImageTexturesCache.Remove(keyValuePair.Key);
                    }
                }
            }
            if (!this.UpgradeDialogShown && ServersManager.NewVersionServerDiscovered != null)
            {
                this.UpgradeDialogShown = true;
                DialogsManager.ShowDialog(null, new MessageDialog("UPDATE REQUIRED", string.Concat(new string[]
                {
                    "You need a newer version of the game to play online (current ",
                    VersionsManager.Version.ToString(2),
                    ", required ",
                    ServersManager.NewVersionServerDiscovered.Value.ToString(2),
                    ")."
                }), "UPDATE", "CANCEL", delegate (MessageDialogButton b)
                {
                    if (b == MessageDialogButton.Button1)
                    {
                        MarketplaceManager.ShowMarketplace();
                    }
                }), true);
            }
            this.JoinButton.IsVisible = (this.GamesList.SelectedItem != null);
            this.SpectateButton.IsVisible = (this.GamesList.SelectedItem != null);
            this.CreateButton.IsVisible = (this.GamesList.SelectedItem == null);
        }

        private static string ShortenAddress(IPAddress address)
        {
            string text = address.ToString();
            if (text.Length > 15)
            {
                text = text.Substring(0, 4) + "..." + text.Substring(text.Length - 8, 8);
            }
            return text;
        }

        private bool UpgradeDialogShown;

        private Dictionary<GameDescription, Texture2D> GameImageTexturesCache = new Dictionary<GameDescription, Texture2D>();

        private ListPanelWidget GamesList;

        private ButtonWidget JoinButton;

        private ButtonWidget SpectateButton;

        private ButtonWidget CreateButton;

        private ButtonWidget BackButton;
    }
}
