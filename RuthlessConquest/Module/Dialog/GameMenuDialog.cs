// Decompiled with JetBrains decompiler
// Type: GameMenuDialog
// Assembly: RuthlessConquest, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 09ABF203-5B7E-4C78-ACFB-2EE5FE9ADF6E
// Assembly location: d:\Users\12464\Desktop\Ruthless Conquest\RuthlessConquest.exe

using Engine;
using Engine.Graphics;
using System;

namespace Game
{
    /// <summary>
    /// 保护类
    /// 游戏内菜单对话框
    /// 继承:对话框
    /// </summary>
    internal class GameMenuDialog : Dialog
    {
        private RectangleWidget Planet1Widget;
        private RectangleWidget Planet2Widget;
        private RectangleWidget ShipsWidget;
        private LabelWidget TipWidget;
        private ButtonWidget ResumeWidget;
        private ButtonWidget DisconnectWidget;
        private ButtonWidget StartWidget;
        private CheckboxWidget NoMoreWidget;
        private ContainerWidget ButtonsContainer1;
        private ContainerWidget ButtonsContainer2;
        private GameScreen GameScreen;
        private static int TipIndex;
        private static string[] Tips = new string[28]
        {
      "TIP: bigger planets produce more ships",
      "TIP: capture all special planets to win",
      "TIP: launch satellites to defend your planets",
      "TIP: a satellite will fire on any enemy ships in range",
      "TIP: disable satellites to let your enemy's enemy reinforcements pass",
      "TIP: watch out for the Antarans, they ruled this galaxy once!",
      "TIP: when your enemies weaken themselves against each other, attack!",
      "TIP: neutral satellites only fire on ships attacking neutral planets",
      "TIP: weaker players might occasionally receive reinforcements",
      "TIP: freshly captured planet stops production for a while",
      "TIP: empire productivity falls when it's very large",
      "TIP: numbers on planets turn grey when empire productivity is low",
      "TIP: don't expend your forces on neutrals when enemy is at the door",
      "TIP: send ships in small groups to reduce losses from satellites",
      "TIP: attack is silver, defence is gold, counterattack is priceless",
      "TIP: gift ships to neutrals separating you from a powerful enemy",
      "TIP: practice against AI to hone your skills",
      "TIP: it's often a mistake to attack the weaker of two opponents",
      "TIP: Antarans don't like factions that grow too big",
      "TIP: a planet stops production if it reaches capacity",
      "TIP: it's ruinous to sit between two warring factions",
      "TIP: ships will slowly die on an overcrowded planet",
      "TIP: neutral planets slowly gather strength",
      "TIP: rush his large planets if he stretches himself too thin",
      "TIP: if he sends ships, attack the planet they launched from",
      "TIP: set up continuous ship delivery from safe planets in the back",
      "TIP: build up reserves before attack",
      "TIP: against satellites, attack in small groups to reduce losses"
        };
        private static double[] TipLastSeenTimes = new double[Tips.Length];

        public GameMenuDialog(GameScreen gameScreen, bool initialInstructions)
        {
            this.GameScreen = gameScreen;
            this.Size = (720f, 420f);
            this.ClampToBounds = true;
            WidgetsList children1 = this.Children;
            Widget[] widgetArray = new Widget[3]
            {
         new RectangleWidget()
        {
          FillColor = Colors.Back,
          OutlineColor = Colors.ForeDim,
          OutlineThickness = 2f
        },
         new InterlaceWidget(),
        null
            };
            StackPanelWidget stackPanelWidget1 = new StackPanelWidget();
            stackPanelWidget1.Direction = LayoutDirection.Vertical;
            stackPanelWidget1.Margin = (10f, 10f);
            WidgetsList children2 = stackPanelWidget1.Children;
            LabelWidget labelWidget1 = new LabelWidget();
            labelWidget1.Text = "INSTRUCTIONS";
            labelWidget1.Margin = (0.0f, 5f);
            children2.Add(labelWidget1);
            stackPanelWidget1.Children.Add(new CanvasWidget()
            {
                Size = (0.0f, float.PositiveInfinity)
            });
            WidgetsList children3 = stackPanelWidget1.Children;
            LabelWidget labelWidget2 = new LabelWidget();
            labelWidget2.Font = Fonts.Small;
            labelWidget2.Color = Colors.ForeDim;
            labelWidget2.TextAnchor = TextAnchor.HorizontalCenter;
            labelWidget2.WordWrap = true;
            labelWidget2.Margin = (0.0f, 4f);
            labelWidget2.Text = "You need to capture all special planets to win:";
            children3.Add(labelWidget2);
            WidgetsList children4 = stackPanelWidget1.Children;
            StackPanelWidget stackPanelWidget2 = new StackPanelWidget();
            stackPanelWidget2.Margin = (0.0f, 8f);
            WidgetsList children5 = stackPanelWidget2.Children;
            CanvasWidget canvasWidget1 = new CanvasWidget();
            canvasWidget1.Size = (120f, 100f);
            WidgetsList children6 = canvasWidget1.Children;
            RectangleWidget rectangleWidget1 = new RectangleWidget();
            rectangleWidget1.Alignment = (0.0f, -1f);
            rectangleWidget1.Size = (76f, 76f);
            rectangleWidget1.Texcoord1 = (0.1f, 0.1f);
            rectangleWidget1.Texcoord2 = (0.9f, 0.9f);
            rectangleWidget1.FillColor = Color.White;
            rectangleWidget1.OutlineColor = Color.Transparent;
            rectangleWidget1.Subtexture = Textures.Planet;
            RectangleWidget rectangleWidget2 = rectangleWidget1;
            this.Planet1Widget = rectangleWidget1;
            RectangleWidget rectangleWidget3 = rectangleWidget2;
            children6.Add(rectangleWidget3);
            WidgetsList children7 = canvasWidget1.Children;
            LabelWidget labelWidget3 = new LabelWidget();
            labelWidget3.Alignment = (0.0f, 1f);
            labelWidget3.Font = Fonts.Small;
            labelWidget3.Color = Colors.HighDim;
            labelWidget3.TextAnchor = TextAnchor.HorizontalCenter;
            labelWidget3.Margin = (0.0f, 4f);
            labelWidget3.Text = "Your planet";
            children7.Add(labelWidget3);
            children5.Add(canvasWidget1);
            WidgetsList children8 = stackPanelWidget2.Children;
            CanvasWidget canvasWidget2 = new CanvasWidget();
            canvasWidget2.Size = (120f, 100f);
            WidgetsList children9 = canvasWidget2.Children;
            RectangleWidget rectangleWidget4 = new RectangleWidget();
            rectangleWidget4.Alignment = (0.0f, -1f);
            rectangleWidget4.Size = (76f, 76f);
            rectangleWidget4.FillColor = Color.White;
            rectangleWidget4.OutlineColor = Color.Transparent;
            rectangleWidget4.Subtexture = Textures.Gui.Ships;
            RectangleWidget rectangleWidget5 = rectangleWidget4;
            this.ShipsWidget = rectangleWidget4;
            RectangleWidget rectangleWidget6 = rectangleWidget5;
            children9.Add(rectangleWidget6);
            children8.Add(canvasWidget2);
            WidgetsList children10 = stackPanelWidget2.Children;
            CanvasWidget canvasWidget3 = new CanvasWidget();
            canvasWidget3.Size = (120f, 100f);
            WidgetsList children11 = canvasWidget3.Children;
            RectangleWidget rectangleWidget7 = new RectangleWidget();
            rectangleWidget7.Alignment = (0.0f, -1f);
            rectangleWidget7.Size = (76f, 76f);
            rectangleWidget7.Texcoord1 = (0.1f, 0.1f);
            rectangleWidget7.Texcoord2 = (0.9f, 0.9f);
            rectangleWidget7.FillColor = Color.White;
            rectangleWidget7.OutlineColor = Color.Transparent;
            rectangleWidget7.Subtexture = Textures.Planet2;
            RectangleWidget rectangleWidget8 = rectangleWidget7;
            this.Planet2Widget = rectangleWidget7;
            RectangleWidget rectangleWidget9 = rectangleWidget8;
            children11.Add(rectangleWidget9);
            WidgetsList children12 = canvasWidget3.Children;
            LabelWidget labelWidget4 = new LabelWidget();
            labelWidget4.Alignment = (0.0f, 1f);
            labelWidget4.Font = Fonts.Small;
            labelWidget4.Color = Colors.HighDim;
            labelWidget4.TextAnchor = TextAnchor.HorizontalCenter;
            labelWidget4.Margin = (0.0f, 4f);
            labelWidget4.Text = "Special planet";
            children12.Add(labelWidget4);
            children10.Add(canvasWidget3);
            children4.Add(stackPanelWidget2);
            WidgetsList children13 = stackPanelWidget1.Children;
            LabelWidget labelWidget5 = new LabelWidget();
            labelWidget5.Font = Fonts.Small;
            labelWidget5.Color = Colors.ForeDim;
            labelWidget5.TextAnchor = TextAnchor.HorizontalCenter;
            labelWidget5.WordWrap = true;
            labelWidget5.Margin = (0.0f, 4f);
            labelWidget5.Text = "Tap your planet, then tap on another planet to attack.\nLong-press a planet for more options.\nSelect multiple planets by dragging a rectangle across the screen.";
            children13.Add(labelWidget5);
            WidgetsList children14 = stackPanelWidget1.Children;
            LabelWidget labelWidget6 = new LabelWidget();
            labelWidget6.Font = Fonts.Small;
            labelWidget6.Color = Colors.HighDim;
            labelWidget6.TextAnchor = TextAnchor.HorizontalCenter;
            labelWidget6.WordWrap = true;
            labelWidget6.Margin = (0.0f, 12f);
            LabelWidget labelWidget7 = labelWidget6;
            this.TipWidget = labelWidget6;
            LabelWidget labelWidget8 = labelWidget7;
            children14.Add(labelWidget8);
            stackPanelWidget1.Children.Add(new CanvasWidget()
            {
                Size = (0.0f, float.PositiveInfinity)
            });
            stackPanelWidget1.Children.Add(new CanvasWidget()
            {
                Size = (0.0f, 10f)
            });
            WidgetsList children15 = stackPanelWidget1.Children;
            StackPanelWidget stackPanelWidget3 = new StackPanelWidget();
            stackPanelWidget3.Direction = LayoutDirection.Horizontal;
            stackPanelWidget3.Children.Add(new CanvasWidget()
            {
                Size = (float.PositiveInfinity, 0.0f)
            });
            //开始
            WidgetsList children16 = stackPanelWidget3.Children;
            BevelledButtonWidget bevelledButtonWidget1 = new BevelledButtonWidget();
            bevelledButtonWidget1.Text = "START";
            bevelledButtonWidget1.Size = (200f, 60f);
            bevelledButtonWidget1.IsCancelButton = true;
            bevelledButtonWidget1.IsOkButton = true;
            bevelledButtonWidget1.Sound = Sounds.Click2;
            ButtonWidget buttonWidget1 = bevelledButtonWidget1;
            this.StartWidget = bevelledButtonWidget1;
            ButtonWidget buttonWidget2 = buttonWidget1;
            children16.Add(buttonWidget2);
            stackPanelWidget3.Children.Add(new CanvasWidget()
            {
                Size = (float.PositiveInfinity, 0.0f)
            });
            //开始结束
            //不再显示
            WidgetsList children17 = stackPanelWidget3.Children;
            CheckboxWidget checkboxWidget1 = new CheckboxWidget();
            checkboxWidget1.Text = "DON'T SHOW AGAIN";
            checkboxWidget1.Size = (350f, 60f);
            CheckboxWidget checkboxWidget2 = checkboxWidget1;
            this.NoMoreWidget = checkboxWidget1;
            CheckboxWidget checkboxWidget3 = checkboxWidget2;
            children17.Add(checkboxWidget3);
            stackPanelWidget3.Children.Add(new CanvasWidget()
            {
                Size = (float.PositiveInfinity, 0.0f)
            });
            ContainerWidget containerWidget1 = stackPanelWidget3;
            this.ButtonsContainer1 = stackPanelWidget3;
            ContainerWidget containerWidget2 = containerWidget1;
            children15.Add(containerWidget2);
            WidgetsList children18 = stackPanelWidget1.Children;
            StackPanelWidget stackPanelWidget4 = new StackPanelWidget();
            stackPanelWidget4.Direction = LayoutDirection.Horizontal;
            stackPanelWidget4.Children.Add(new CanvasWidget()
            {
                Size = (float.PositiveInfinity, 0.0f)
            });
            //不再显示结束
            //重新开始
            WidgetsList children19 = stackPanelWidget4.Children;
            BevelledButtonWidget bevelledButtonWidget2 = new BevelledButtonWidget();
            bevelledButtonWidget2.Text = "RESUME";
            bevelledButtonWidget2.Size = (250f, 60f);
            bevelledButtonWidget2.IsCancelButton = true;
            bevelledButtonWidget2.Sound = Sounds.Click2;
            ButtonWidget buttonWidget3 = bevelledButtonWidget2;
            this.ResumeWidget = bevelledButtonWidget2;
            ButtonWidget buttonWidget4 = buttonWidget3;
            children19.Add(buttonWidget4);
            stackPanelWidget4.Children.Add(new CanvasWidget()
            {
                Size = (float.PositiveInfinity, 0.0f)
            });
            //重新开始结束
            //断开连接
            WidgetsList children20 = stackPanelWidget4.Children;
            BevelledButtonWidget bevelledButtonWidget3 = new BevelledButtonWidget();
            bevelledButtonWidget3.Text = "DISCONNECT";
            bevelledButtonWidget3.Size = (250f, 60f);
            bevelledButtonWidget3.Sound = Sounds.Click2;
            ButtonWidget buttonWidget5 = bevelledButtonWidget3;
            this.DisconnectWidget = bevelledButtonWidget3;
            ButtonWidget buttonWidget6 = buttonWidget5;
            children20.Add(buttonWidget6);
            stackPanelWidget4.Children.Add(new CanvasWidget()
            {
                Size = (float.PositiveInfinity, 0.0f)
            });
            //断开连接结束
            ContainerWidget containerWidget3 = stackPanelWidget4;
            this.ButtonsContainer2 = stackPanelWidget4;
            ContainerWidget containerWidget4 = containerWidget3;
            children18.Add(containerWidget4);
            widgetArray[2] = stackPanelWidget1;
            children1.Add(widgetArray);
            SettingsManager.InstructionsShown = true;
            this.ButtonsContainer1.IsVisible = initialInstructions;
            this.ButtonsContainer2.IsVisible = !initialInstructions;
            HumanPlayer controllingPlayer = this.GameScreen.Client.Game.PlayersModule.ControllingPlayer;
            Faction faction = controllingPlayer == null || controllingPlayer.Faction == Faction.None ? this.GameScreen.Client.Game.CreationParameters.CreatingPlayerFaction : controllingPlayer.Faction;
            this.Planet1Widget.FillColor = Planet.GetColor(faction);
            this.ShipsWidget.FillColor = Ship.GetColor(faction);
            this.Planet2Widget.FillColor = Planet.GetColor((Faction)((int)(faction + new Engine.Random().Int(4) + 1) % 6));
            int index = TipIndex % Tips.Length;
            if (TipLastSeenTimes[index] > 0.0 && Time.FrameStartTime - TipLastSeenTimes[index] < Tips.Length * 8)
            {
                this.TipWidget.Text = "It's very naughty to read too many tips in one go :-(";
                Array.Clear(TipLastSeenTimes, 0, TipLastSeenTimes.Length);
            }
            else
            {
                this.TipWidget.Text = Tips[index];
                TipLastSeenTimes[index] = Time.FrameStartTime;
                ++TipIndex;
            }
        }

        public override void Update()
        {
            if (this.GameScreen.Server != null && this.GameScreen.Server.IsUsingInProcessTransmitter)
            {
                this.GameScreen.Server.IsPaused = true;
                this.GameScreen.Client.IsPaused = true;
            }
            HumanPlayer controllingPlayer = this.GameScreen.Client.Game.PlayersModule.ControllingPlayer;
            if (this.StartWidget.IsClicked)
                this.Dismiss(false);
            if (this.NoMoreWidget.IsClicked)
                SettingsManager.DontShowInstructions = this.NoMoreWidget.IsChecked;
            if (this.ResumeWidget.IsClicked || this.DialogCoverWidget.Clickable.IsClicked)
                this.Dismiss(false);
            //断开连接
            if (this.DisconnectWidget.IsClicked)
            {
                if (this.GameScreen.Server != null)
                {
                    if (this.GameScreen.Server.IsUsingInProcessTransmitter)
                        this.Dismiss(true);
                    else
                        DialogsManager.ShowDialog(this, new MessageDialog("WARNING", "You are the host, do you want to shutdown the game and disconnect all the players?", "YES", "NO", b =>
                        {
                            if (b != MessageDialogButton.Button1)
                                return;
                            this.Dismiss(true);
                        }));
                }
                else if (controllingPlayer != null && controllingPlayer.Faction != Faction.None && controllingPlayer.Status == FactionStatus.Undecided)
                    DialogsManager.ShowDialog(this, new MessageDialog("WARNING", "Do you want to disconnect from the game? Other players will continue and AI will take over your empire.", "YES", "NO", b =>
                    {
                        if (b != MessageDialogButton.Button1)
                            return;
                        this.Dismiss(true);
                    }));
                else
                    DialogsManager.ShowDialog(this, new MessageDialog("WARNING", "Do you want to disconnect from the game?", "YES", "NO", b =>
                    {
                        if (b != MessageDialogButton.Button1)
                            return;
                        this.Dismiss(true);
                    }));
            }
            this.NoMoreWidget.IsChecked = SettingsManager.DontShowInstructions;
        }

        private void Dismiss(bool disconnect)
        {
            DialogsManager.HideDialog(this);
            if (this.GameScreen.Server != null && this.GameScreen.Server.IsUsingInProcessTransmitter)
            {
                this.GameScreen.Client.IsPaused = false;
                this.GameScreen.Server.IsPaused = false;
            }
            if (!disconnect)
                return;
            this.GameScreen.Disconnect();
        }
    }
}

