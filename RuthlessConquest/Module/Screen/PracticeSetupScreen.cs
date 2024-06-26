﻿using Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    internal class PracticeSetupScreen : Screen
    {
        public PracticeSetupScreen()
        {
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
                        Text = "Set practice options:",
                        Font = Fonts.Small,
                        Color = Colors.Fore
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
            ScrollPanelWidget scrollPanelWidget = new ScrollPanelWidget();
            scrollPanelWidget.Direction = LayoutDirection.Vertical;
            scrollPanelWidget.Margin = new Vector2(2f, 2f);
            WidgetsList children4 = scrollPanelWidget.Children;
            StackPanelWidget stackPanelWidget2 = new StackPanelWidget();
            stackPanelWidget2.Direction = LayoutDirection.Vertical;
            WidgetsList children5 = stackPanelWidget2.Children;
            UniformSpacingPanelWidget uniformSpacingPanelWidget = new UniformSpacingPanelWidget();
            uniformSpacingPanelWidget.Margin = new Vector2(0f, 3f);
            uniformSpacingPanelWidget.Children.Add(new LabelWidget
            {
                Text = "AI LEVEL",
                Alignment = new Vector2(1f, 0f),
                Margin = new Vector2(20f, 0f)
            });
            WidgetsList children6 = uniformSpacingPanelWidget.Children;
            BevelledButtonWidget bevelledButtonWidget = new BevelledButtonWidget();
            bevelledButtonWidget.Margin = new Vector2(20f, 0f);
            bevelledButtonWidget.Alignment = new Vector2(-1f, 0f);
            bevelledButtonWidget.Size = new Vector2(float.PositiveInfinity, 60f);
            BevelledButtonWidget widget = bevelledButtonWidget;
            this.AILevelButton = bevelledButtonWidget;
            children6.Add(widget);
            children5.Add(uniformSpacingPanelWidget);
            WidgetsList children7 = stackPanelWidget2.Children;
            UniformSpacingPanelWidget uniformSpacingPanelWidget2 = new UniformSpacingPanelWidget();
            uniformSpacingPanelWidget2.Margin = new Vector2(0f, 3f);
            uniformSpacingPanelWidget2.Children.Add(new LabelWidget
            {
                Text = "SHIP RANGE",
                Alignment = new Vector2(1f, 0f),
                Margin = new Vector2(20f, 0f)
            });
            WidgetsList children8 = uniformSpacingPanelWidget2.Children;
            BevelledButtonWidget bevelledButtonWidget2 = new BevelledButtonWidget();
            bevelledButtonWidget2.Margin = new Vector2(20f, 0f);
            bevelledButtonWidget2.Alignment = new Vector2(-1f, 0f);
            bevelledButtonWidget2.Size = new Vector2(float.PositiveInfinity, 60f);
            widget = bevelledButtonWidget2;
            this.ShipRangeButton = bevelledButtonWidget2;
            children8.Add(widget);
            children7.Add(uniformSpacingPanelWidget2);
            WidgetsList children9 = stackPanelWidget2.Children;
            UniformSpacingPanelWidget uniformSpacingPanelWidget3 = new UniformSpacingPanelWidget();
            uniformSpacingPanelWidget3.Margin = new Vector2(0f, 3f);
            uniformSpacingPanelWidget3.Children.Add(new LabelWidget
            {
                Text = "WORLD SEED",
                Alignment = new Vector2(1f, 0f),
                Margin = new Vector2(20f, 0f)
            });
            WidgetsList children10 = uniformSpacingPanelWidget3.Children;
            BevelledButtonWidget bevelledButtonWidget3 = new BevelledButtonWidget();
            bevelledButtonWidget3.Margin = new Vector2(20f, 0f);
            bevelledButtonWidget3.Alignment = new Vector2(-1f, 0f);
            bevelledButtonWidget3.Size = new Vector2(float.PositiveInfinity, 60f);
            widget = bevelledButtonWidget3;
            this.WorldSeedButton = bevelledButtonWidget3;
            children10.Add(widget);
            children9.Add(uniformSpacingPanelWidget3);
            WidgetsList children11 = stackPanelWidget2.Children;
            UniformSpacingPanelWidget uniformSpacingPanelWidget4 = new UniformSpacingPanelWidget();
            uniformSpacingPanelWidget4.Margin = new Vector2(0f, 3f);
            uniformSpacingPanelWidget4.Children.Add(new LabelWidget
            {
                Text = "PLAYERS",
                Alignment = new Vector2(1f, 0f),
                Margin = new Vector2(20f, 0f)
            });
            WidgetsList children12 = uniformSpacingPanelWidget4.Children;
            SliderWidget sliderWidget = new SliderWidget();
            sliderWidget.MinValue = 2f;
            sliderWidget.MaxValue = 6f;
            sliderWidget.Granularity = 1f;
            sliderWidget.Margin = new Vector2(20f, 0f);
            SliderWidget widget2 = sliderWidget;
            this.PlayersCountSlider = sliderWidget;
            children12.Add(widget2);
            children11.Add(uniformSpacingPanelWidget4);
            WidgetsList children13 = stackPanelWidget2.Children;
            UniformSpacingPanelWidget uniformSpacingPanelWidget5 = new UniformSpacingPanelWidget();
            uniformSpacingPanelWidget5.Margin = new Vector2(0f, 3f);
            uniformSpacingPanelWidget5.Children.Add(new LabelWidget
            {
                Text = "PLANETS",
                Alignment = new Vector2(1f, 0f),
                Margin = new Vector2(20f, 0f)
            });
            WidgetsList children14 = uniformSpacingPanelWidget5.Children;
            SliderWidget sliderWidget2 = new SliderWidget();
            sliderWidget2.MinValue = 12f;
            sliderWidget2.MaxValue = 32f;
            sliderWidget2.Granularity = 2f;
            sliderWidget2.Margin = new Vector2(20f, 0f);
            widget2 = sliderWidget2;
            this.PlanetsCountSlider = sliderWidget2;
            children14.Add(widget2);
            children13.Add(uniformSpacingPanelWidget5);
            WidgetsList children15 = stackPanelWidget2.Children;
            UniformSpacingPanelWidget uniformSpacingPanelWidget6 = new UniformSpacingPanelWidget();
            uniformSpacingPanelWidget6.Margin = new Vector2(0f, 3f);
            uniformSpacingPanelWidget6.Children.Add(new LabelWidget
            {
                Text = "SPECIAL PLANETS",
                Alignment = new Vector2(1f, 0f),
                Margin = new Vector2(20f, 0f)
            });
            WidgetsList children16 = uniformSpacingPanelWidget6.Children;
            SliderWidget sliderWidget3 = new SliderWidget();
            sliderWidget3.MinValue = 2f;
            sliderWidget3.MaxValue = 6f;
            sliderWidget3.Granularity = 1f;
            sliderWidget3.Margin = new Vector2(20f, 0f);
            widget2 = sliderWidget3;
            this.SpecialPlanetsCountSlider = sliderWidget3;
            children16.Add(widget2);
            children15.Add(uniformSpacingPanelWidget6);
            WidgetsList children17 = stackPanelWidget2.Children;
            UniformSpacingPanelWidget uniformSpacingPanelWidget7 = new UniformSpacingPanelWidget();
            uniformSpacingPanelWidget7.Margin = new Vector2(0f, 3f);
            uniformSpacingPanelWidget7.Children.Add(new LabelWidget
            {
                Text = "PLANET POPULATION",
                Alignment = new Vector2(1f, 0f),
                Margin = new Vector2(20f, 0f)
            });
            WidgetsList children18 = uniformSpacingPanelWidget7.Children;
            SliderWidget sliderWidget4 = new SliderWidget();
            sliderWidget4.MinValue = 0f;
            sliderWidget4.MaxValue = GameSetupScreen.NeutralPopulationFactors.Length - 1;
            sliderWidget4.Granularity = 1f;
            sliderWidget4.Margin = new Vector2(20f, 0f);
            widget2 = sliderWidget4;
            this.NeutralsPopulationSlider = sliderWidget4;
            children18.Add(widget2);
            children17.Add(uniformSpacingPanelWidget7);
            children4.Add(stackPanelWidget2);
            children3.Add(scrollPanelWidget);
            children2.Add(canvasWidget);
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 20f)
            });
            WidgetsList children19 = stackPanelWidget.Children;
            StackPanelWidget stackPanelWidget3 = new StackPanelWidget();
            stackPanelWidget3.Direction = LayoutDirection.Horizontal;
            WidgetsList children20 = stackPanelWidget3.Children;
            BevelledButtonWidget bevelledButtonWidget4 = new BevelledButtonWidget();
            bevelledButtonWidget4.Text = "START GAME";
            bevelledButtonWidget4.Size = new Vector2(300f, 60f);
            bevelledButtonWidget4.Margin = new Vector2(20f, 0f);
            ButtonWidget widget3 = bevelledButtonWidget4;
            this.PlayButton = bevelledButtonWidget4;
            children20.Add(widget3);
            WidgetsList children21 = stackPanelWidget3.Children;
            BevelledButtonWidget bevelledButtonWidget5 = new BevelledButtonWidget();
            bevelledButtonWidget5.Text = "BACK";
            bevelledButtonWidget5.Size = new Vector2(200f, 60f);
            bevelledButtonWidget5.Margin = new Vector2(20f, 0f);
            bevelledButtonWidget5.IsCancelButton = true;
            bevelledButtonWidget5.Sound = Sounds.Click2;
            widget3 = bevelledButtonWidget5;
            this.BackButton = bevelledButtonWidget5;
            children21.Add(widget3);
            children19.Add(stackPanelWidget3);
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 10f)
            });
            array[num] = stackPanelWidget;
            children.Add(array);
            this.PlayersCountSlider.Value = 4f;
            this.PlanetsCountSlider.Value = 24f;
            this.SpecialPlanetsCountSlider.Value = 3f;
            this.NeutralsPopulationSlider.Value = GameSetupScreen.NeutralPopulationFactors.Length / 2;
        }

        public override void Update()
        {
            if (this.AILevelButton.IsClicked)
            {
                IEnumerable<PlayerType> items = from t in Enum.GetValues(typeof(PlayerType)).OfType<PlayerType>()
                                                where t > PlayerType.Human
                                                select t;
                DialogsManager.ShowDialog(this, new ListSelectionDialog("Select AI Level", items, 56f, (object o) => Player.GetPlayerTypeName((PlayerType)o).ToUpper(), delegate (object o)
                {
                    if (o != null)
                    {
                        this.AILevel = (PlayerType)o;
                    }
                }), true);
            }
            if (this.ShipRangeButton.IsClicked)
            {
                Array values = Enum.GetValues(typeof(ShipRange));
                DialogsManager.ShowDialog(this, new ListSelectionDialog("Select Ship Range", values, 56f, (object o) => Ship.GetShipRangeName((ShipRange)o).ToUpper(), delegate (object o)
                {
                    if (o != null)
                    {
                        this.ShipRange = (ShipRange)o;
                    }
                }), true);
            }
            if (this.WorldSeedButton.IsClicked)
            {
                DialogsManager.ShowDialog(null, new TextBoxDialog("World Seed (leave blank for random)", this.WorldSeed, 12, delegate (string s)
                {
                    if (s != null)
                    {
                        this.WorldSeed = s;
                    }
                }), true);
            }
            if (this.PlayButton.IsClicked)
            {
                NamesManager.EnsureValidPlayerNameExists(delegate
                {
                    GameCreationParameters creationParameters = new GameCreationParameters(GameCreationParameters.CalculateSeed(this.WorldSeed), (int)this.PlanetsCountSlider.Value, (int)this.SpecialPlanetsCountSlider.Value, GameSetupScreen.NeutralPopulationFactors[(int)this.NeutralsPopulationSlider.Value], (int)this.ShipRange, 1, (int)this.PlayersCountSlider.Value, 7, SettingsManager.Faction, SettingsManager.PlayerName, SettingsManager.PlayerGuid, VersionsManager.Platform, this.AILevel);
                    DialogsManager.ShowDialog(this, new ConnectingDialog(creationParameters, ConnectingDialog.GameType.InProcess), true);
                });
            }
            if (this.BackButton.IsClicked)
            {
                ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
            }
            this.AILevelButton.Text = Player.GetPlayerTypeName(this.AILevel).ToUpper();
            this.ShipRangeButton.Text = Ship.GetShipRangeName(this.ShipRange).ToUpper();
            this.WorldSeedButton.Text = (string.IsNullOrEmpty(this.WorldSeed) ? "<random>" : this.WorldSeed);
            this.PlayersCountSlider.Text = this.PlayersCountSlider.Value.ToString();
            this.PlanetsCountSlider.Text = this.PlanetsCountSlider.Value.ToString();
            this.SpecialPlanetsCountSlider.Text = this.SpecialPlanetsCountSlider.Value.ToString();
            this.NeutralsPopulationSlider.Text = GameSetupScreen.NeutralPopulationFactors[(int)this.NeutralsPopulationSlider.Value].ToString() + "%";
        }

        private SliderWidget PlayersCountSlider;

        private SliderWidget PlanetsCountSlider;

        private SliderWidget SpecialPlanetsCountSlider;

        private SliderWidget NeutralsPopulationSlider;

        private BevelledButtonWidget AILevelButton;

        private BevelledButtonWidget ShipRangeButton;

        private BevelledButtonWidget WorldSeedButton;

        private ButtonWidget PlayButton;

        private ButtonWidget BackButton;

        private PlayerType AILevel = PlayerType.EasyAI;

        private ShipRange ShipRange = ShipRange.Short;

        private string WorldSeed = string.Empty;
    }
}
