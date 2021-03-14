using System;
using Engine;

namespace Game
{
    internal class SettingsScreen : Screen
    {
        public SettingsScreen()
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
                        Text = "Settings:",
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
                Text = "NICKNAME",
                Alignment = new Vector2(1f, 0f),
                Margin = new Vector2(20f, 0f)
            });
            WidgetsList children6 = uniformSpacingPanelWidget.Children;
            BevelledButtonWidget bevelledButtonWidget = new BevelledButtonWidget();
            bevelledButtonWidget.Margin = new Vector2(20f, 0f);
            bevelledButtonWidget.Alignment = new Vector2(-1f, 0f);
            bevelledButtonWidget.Size = new Vector2(float.PositiveInfinity, 60f);
            ButtonWidget widget = bevelledButtonWidget;
            this.NicknameButton = bevelledButtonWidget;
            children6.Add(widget);
            children5.Add(uniformSpacingPanelWidget);
            WidgetsList children7 = stackPanelWidget2.Children;
            UniformSpacingPanelWidget uniformSpacingPanelWidget2 = new UniformSpacingPanelWidget();
            uniformSpacingPanelWidget2.Margin = new Vector2(0f, 3f);
            uniformSpacingPanelWidget2.Children.Add(new LabelWidget
            {
                Text = "PREFERRED FACTION",
                Alignment = new Vector2(1f, 0f),
                Margin = new Vector2(20f, 0f)
            });
            WidgetsList children8 = uniformSpacingPanelWidget2.Children;
            StackPanelWidget stackPanelWidget3 = new StackPanelWidget();
            stackPanelWidget3.Margin = new Vector2(20f, 0f);
            stackPanelWidget3.Alignment = new Vector2(-1f, 0f);
            WidgetsList children9 = stackPanelWidget3.Children;
            CanvasWidget canvasWidget2 = new CanvasWidget();
            WidgetsList children10 = canvasWidget2.Children;
            BevelledButtonWidget bevelledButtonWidget2 = new BevelledButtonWidget();
            bevelledButtonWidget2.Size = new Vector2(float.PositiveInfinity, 60f);
            BevelledButtonWidget widget2 = bevelledButtonWidget2;
            this.FactionButton = bevelledButtonWidget2;
            children10.Add(widget2);
            WidgetsList children11 = canvasWidget2.Children;
            RectangleWidget rectangleWidget = new RectangleWidget();
            rectangleWidget.Size = new Vector2(32f, 32f);
            rectangleWidget.FillColor = Color.Black;
            rectangleWidget.OutlineColor = Color.Transparent;
            RectangleWidget widget3 = rectangleWidget;
            this.FactionRectangle = rectangleWidget;
            children11.Add(widget3);
            children9.Add(canvasWidget2);
            children8.Add(stackPanelWidget3);
            children7.Add(uniformSpacingPanelWidget2);
            WidgetsList children12 = stackPanelWidget2.Children;
            UniformSpacingPanelWidget uniformSpacingPanelWidget3 = new UniformSpacingPanelWidget();
            uniformSpacingPanelWidget3.Margin = new Vector2(0f, 3f);
            uniformSpacingPanelWidget3.Children.Add(new LabelWidget
            {
                Text = "DEFAULT SEND FLEET %",
                Alignment = new Vector2(1f, 0f),
                Margin = new Vector2(20f, 0f)
            });
            WidgetsList children13 = uniformSpacingPanelWidget3.Children;
            SliderWidget sliderWidget = new SliderWidget();
            sliderWidget.MinValue = 0.2f;
            sliderWidget.MaxValue = 1f;
            sliderWidget.Granularity = 0.1f;
            sliderWidget.Margin = new Vector2(20f, 0f);
            SliderWidget widget4 = sliderWidget;
            this.DefaultFleetStrengthSlider = sliderWidget;
            children13.Add(widget4);
            children12.Add(uniformSpacingPanelWidget3);
            WidgetsList children14 = stackPanelWidget2.Children;
            UniformSpacingPanelWidget uniformSpacingPanelWidget4 = new UniformSpacingPanelWidget();
            uniformSpacingPanelWidget4.Margin = new Vector2(0f, 3f);
            uniformSpacingPanelWidget4.Children.Add(new LabelWidget
            {
                Text = "SFX VOLUME",
                Alignment = new Vector2(1f, 0f),
                Margin = new Vector2(20f, 0f)
            });
            WidgetsList children15 = uniformSpacingPanelWidget4.Children;
            SliderWidget sliderWidget2 = new SliderWidget();
            sliderWidget2.MinValue = 0f;
            sliderWidget2.MaxValue = 1f;
            sliderWidget2.Granularity = 0.1f;
            sliderWidget2.Margin = new Vector2(20f, 0f);
            widget4 = sliderWidget2;
            this.SfxSlider = sliderWidget2;
            children15.Add(widget4);
            children14.Add(uniformSpacingPanelWidget4);
            WidgetsList children16 = stackPanelWidget2.Children;
            UniformSpacingPanelWidget uniformSpacingPanelWidget5 = new UniformSpacingPanelWidget();
            uniformSpacingPanelWidget5.Margin = new Vector2(0f, 3f);
            uniformSpacingPanelWidget5.Children.Add(new LabelWidget
            {
                Text = "MUSIC VOLUME",
                Alignment = new Vector2(1f, 0f),
                Margin = new Vector2(20f, 0f)
            });
            WidgetsList children17 = uniformSpacingPanelWidget5.Children;
            SliderWidget sliderWidget3 = new SliderWidget();
            sliderWidget3.MinValue = 0f;
            sliderWidget3.MaxValue = 1f;
            sliderWidget3.Granularity = 0.1f;
            sliderWidget3.Margin = new Vector2(20f, 0f);
            widget4 = sliderWidget3;
            this.MusicSlider = sliderWidget3;
            children17.Add(widget4);
            children16.Add(uniformSpacingPanelWidget5);
            children4.Add(stackPanelWidget2);
            children3.Add(scrollPanelWidget);
            children2.Add(canvasWidget);
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 20f)
            });
            WidgetsList children18 = stackPanelWidget.Children;
            BevelledButtonWidget bevelledButtonWidget3 = new BevelledButtonWidget();
            bevelledButtonWidget3.Text = "BACK";
            bevelledButtonWidget3.Size = new Vector2(200f, 60f);
            bevelledButtonWidget3.IsCancelButton = true;
            bevelledButtonWidget3.Sound = Sounds.Click2;
            widget = bevelledButtonWidget3;
            this.BackButton = bevelledButtonWidget3;
            children18.Add(widget);
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 10f)
            });
            array[num] = stackPanelWidget;
            children.Add(array);
        }

        public override void Update()
        {
            if (this.NicknameButton.IsClicked)
            {
                NamesManager.ShowSetPlayerNameDialog(this, null);
            }
            if (this.FactionButton.IsClicked)
            {
                DialogsManager.ShowDialog(this, new FactionSelectionDialog(delegate (object o)
                {
                    if (o != null)
                    {
                        SettingsManager.Faction = (Faction)o;
                    }
                }), true);
            }
            if (this.DefaultFleetStrengthSlider.IsSliding)
            {
                SettingsManager.DefaultFleetStrength = this.DefaultFleetStrengthSlider.Value;
            }
            if (this.SfxSlider.IsSliding)
            {
                SettingsManager.SfxVolume = this.SfxSlider.Value;
            }
            if (this.MusicSlider.IsSliding)
            {
                SettingsManager.MusicVolume = this.MusicSlider.Value;
            }
            if (this.BackButton.IsClicked)
            {
                ScreensManager.SwitchScreen(ScreensManager.PreviousScreen, Array.Empty<object>());
            }
            this.NicknameButton.Text = (string.IsNullOrEmpty(SettingsManager.PlayerName) ? "<click to choose>" : SettingsManager.PlayerName);
            this.FactionButton.CenterColor = Player.GetColor(SettingsManager.Faction);
            this.FactionButton.BevelColor = Player.GetColor(SettingsManager.Faction);
            this.FactionRectangle.Subtexture = Ship.GetTexture(SettingsManager.Faction);
            this.DefaultFleetStrengthSlider.Value = SettingsManager.DefaultFleetStrength;
            this.DefaultFleetStrengthSlider.Text = string.Format("{0:0}%", SettingsManager.DefaultFleetStrength * 100f);
            this.SfxSlider.Value = SettingsManager.SfxVolume;
            this.SfxSlider.Text = string.Format("{0:0}%", SettingsManager.SfxVolume * 100f);
            this.MusicSlider.Value = SettingsManager.MusicVolume;
            this.MusicSlider.Text = string.Format("{0:0}%", SettingsManager.MusicVolume * 100f);
        }

        private ButtonWidget NicknameButton;

        private BevelledButtonWidget FactionButton;

        private RectangleWidget FactionRectangle;

        private SliderWidget DefaultFleetStrengthSlider;

        private SliderWidget SfxSlider;

        private SliderWidget MusicSlider;

        private ButtonWidget BackButton;
    }
}
