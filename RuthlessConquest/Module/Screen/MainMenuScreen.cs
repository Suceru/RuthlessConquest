using System;
using Engine;
using Engine.Input;

namespace Game
{
    internal class MainMenuScreen : Screen
    {
        public MainMenuScreen()
        {
            WidgetsList children = this.Children;
            Widget[] array = new Widget[4];
            array[0] = new BackgroundWidget();
            array[1] = new BoidsWidget();
            array[2] = new InterlaceWidget();
            int num = 3;
            StackPanelWidget stackPanelWidget = new StackPanelWidget();
            stackPanelWidget.Direction = LayoutDirection.Vertical;
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, float.PositiveInfinity)
            });
            WidgetsList children2 = stackPanelWidget.Children;
            RectangleWidget rectangleWidget = new RectangleWidget();
            rectangleWidget.Subtexture = Textures.Gui.Logo;
            rectangleWidget.Size = new Vector2(425f, 154.7f);
            rectangleWidget.Texcoord1 = new Vector2(0.0f, 0.04f);
            rectangleWidget.Texcoord2 = new Vector2(1f, 0.95f);
            rectangleWidget.FillColor = Colors.High;
            rectangleWidget.OutlineColor = Color.Transparent;
            RectangleWidget widget = rectangleWidget;
            this.LogoWidget = rectangleWidget;
            children2.Add(widget);
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, float.PositiveInfinity)
            });
            WidgetsList children3 = stackPanelWidget.Children;
            MotdWidget motdWidget = new MotdWidget();
            motdWidget.Size = new Vector2(float.PositiveInfinity, 70f);
            MotdWidget widget2 = motdWidget;
            this.MotdWidget = motdWidget;
            children3.Add(widget2);
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, float.PositiveInfinity)
            });
            WidgetsList children4 = stackPanelWidget.Children;
            StackPanelWidget stackPanelWidget2 = new StackPanelWidget();
            WidgetsList children5 = stackPanelWidget2.Children;
            BevelledButtonWidget bevelledButtonWidget = new BevelledButtonWidget();
            bevelledButtonWidget.Text = "PRACTICE";
            bevelledButtonWidget.Size = new Vector2(300f, 60f);
            ButtonWidget widget3 = bevelledButtonWidget;
            this.PracticeButton = bevelledButtonWidget;
            children5.Add(widget3);
            stackPanelWidget2.Children.Add(new CanvasWidget
            {
                Size = new Vector2(8f, 0f)
            });
            //多人
            WidgetsList children6 = stackPanelWidget2.Children;
            BevelledButtonWidget bevelledButtonWidget2 = new BevelledButtonWidget();
            bevelledButtonWidget2.Text = "MULTIPLAYER";
            bevelledButtonWidget2.Size = new Vector2(300f, 60f);
            widget3 = bevelledButtonWidget2;
            this.PlayButton = bevelledButtonWidget2;
            children6.Add(widget3);
            children4.Add(stackPanelWidget2);
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 5f)
            });
            //多人结束
            WidgetsList children7 = stackPanelWidget.Children;
            StackPanelWidget stackPanelWidget3 = new StackPanelWidget();
            WidgetsList children8 = stackPanelWidget3.Children;
            BevelledButtonWidget bevelledButtonWidget3 = new BevelledButtonWidget();
            bevelledButtonWidget3.Text = "SETTINGS";
            bevelledButtonWidget3.Size = new Vector2(300f, 60f);
            widget3 = bevelledButtonWidget3;
            this.SettingsButton = bevelledButtonWidget3;
            children8.Add(widget3);
            stackPanelWidget3.Children.Add(new CanvasWidget
            {
                Size = new Vector2(8f, 0f)
            });
            WidgetsList children9 = stackPanelWidget3.Children;
            BevelledButtonWidget bevelledButtonWidget4 = new BevelledButtonWidget();
            bevelledButtonWidget4.Text = "MORE GAMES...";
            bevelledButtonWidget4.Size = new Vector2(300f, 60f);
            widget3 = bevelledButtonWidget4;
            this.MoreGamesButton = bevelledButtonWidget4;
            children9.Add(widget3);
            children7.Add(stackPanelWidget3);
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, float.PositiveInfinity)
            });
            stackPanelWidget.Children.Add(new StackPanelWidget
            {
                Children =
                {
                    new LabelWidget
                    {
                        Text = "Copyright 2018-2019  ",
                        Font = Fonts.Small
                    },
                    new LinkWidget
                    {
                        Text = "Candy Rufus Games",
                        Url = "http://kaalus.wordpress.com",

                        Color = Colors.High,
                        Font = Fonts.Small
                    },
                    new LabelWidget
                    {
                        Text = "  Version " + VersionsManager.Version.ToString(),
                        Font = Fonts.Small
                    }
                }
            });
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 5f)
            });
            array[num] = stackPanelWidget;
            children.Add(array);
        }

        public override void Enter(object[] parameters)
        {
            this.Children.Find<MotdWidget>(true).Restart();
        }

        public override void Leave()
        {
            Keyboard.BackButtonQuitsApp = false;
        }

        public override void Update()
        {
            Keyboard.BackButtonQuitsApp = true;
            if (this.PracticeButton.IsClicked)
            {
                ScreensManager.SwitchScreen("PracticeSetup", Array.Empty<object>());
            }
            if (this.PlayButton.IsClicked)
            {
                if (!SettingsManager.InstructionsShown)
                {
                    DialogsManager.ShowDialog(null, new MessageDialog("PRACTICE", "Do you want a quick practice against the AI first?", "YES", "NO", delegate (MessageDialogButton b)
                    {
                        if (b == MessageDialogButton.Button1)
                        {
                            ScreensManager.SwitchScreen("PracticeSetup", Array.Empty<object>());
                            return;
                        }
                        ScreensManager.SwitchScreen("GameList", Array.Empty<object>());
                    }), true);
                }
                else
                {
                    ScreensManager.SwitchScreen("GameList", Array.Empty<object>());
                }
            }
            if (this.SettingsButton.IsClicked)
            {
                ScreensManager.SwitchScreen("Settings", Array.Empty<object>());
            }
            if (this.MoreGamesButton.IsClicked)
            {
                ScreensManager.SwitchScreen("Credits", Array.Empty<object>());
            }
            if (Input.Back)
            {
                Window.Close();
            }
        }

        private string VersionString = string.Empty;

        private RectangleWidget LogoWidget;

        private MotdWidget MotdWidget;

        private ButtonWidget PracticeButton;

        private ButtonWidget PlayButton;

        private ButtonWidget SettingsButton;

        private ButtonWidget MoreGamesButton;
    }
}
