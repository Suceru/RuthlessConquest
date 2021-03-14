using System;
using Engine;
using Engine.Graphics;

namespace Game
{
    internal class CreditsScreen : Screen
    {
        public CreditsScreen()
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
                Size = new Vector2(0f, 10f)
            });
            stackPanelWidget.Children.Add(new LabelWidget
            {
                Text = "RUTHLESS CONQUEST",
                FontScale = 1.5f,
                FontSpacing = new Vector2(1f, 0f),
                Color = Colors.High
            });
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 10f)
            });
            stackPanelWidget.Children.Add(new StackPanelWidget
            {
                Children =
                {
                    new LabelWidget
                    {
                        Text = "A GAME BY CANDYRUFUSGAMES"
                    }
                }
            });
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
                        Text = "Check out our other games:",
                        Font = Fonts.Small,
                        Color = Colors.ForeDim
                    }
                }
            });
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 25f)
            });
            WidgetsList children2 = stackPanelWidget.Children;
            UniformSpacingPanelWidget uniformSpacingPanelWidget = new UniformSpacingPanelWidget();
            uniformSpacingPanelWidget.Direction = LayoutDirection.Horizontal;
            WidgetsList children3 = uniformSpacingPanelWidget.Children;
            CanvasWidget canvasWidget = new CanvasWidget();
            WidgetsList children4 = canvasWidget.Children;
            ClickableWidget clickableWidget = new ClickableWidget();
            clickableWidget.Size = new Vector2(0f, 0f);
            clickableWidget.Alignment = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            ClickableWidget widget = clickableWidget;
            this.ClickableWidget1 = clickableWidget;
            children4.Add(widget);
            canvasWidget.Children.Add(new StackPanelWidget
            {
                Direction = LayoutDirection.Vertical,
                Margin = new Vector2(30f, 0f),
                IsHitTestVisible = false,
                Children =
                {
                    new RectangleWidget
                    {
                        Subtexture = Textures.Gui.SurvivalcraftIcon,
                        Size = new Vector2(96f, 96f),
                        FillColor = Color.White,
                        OutlineColor = Color.Transparent
                    },
                    new CanvasWidget
                    {
                        Size = new Vector2(0f, 15f)
                    },
                    new LabelWidget
                    {
                        Text = "SURVIVALCRAFT\n(free demo)",
                        Color = Colors.ForeDim,
                        FontScale = 0.75f,
                        TextAnchor = (TextAnchor.HorizontalCenter | TextAnchor.VerticalCenter)
                    }
                }
            });
            children3.Add(canvasWidget);
            WidgetsList children5 = uniformSpacingPanelWidget.Children;
            CanvasWidget canvasWidget2 = new CanvasWidget();
            WidgetsList children6 = canvasWidget2.Children;
            ClickableWidget clickableWidget2 = new ClickableWidget();
            clickableWidget2.Size = new Vector2(0f, 0f);
            clickableWidget2.Alignment = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            widget = clickableWidget2;
            this.ClickableWidget2 = clickableWidget2;
            children6.Add(widget);
            canvasWidget2.Children.Add(new StackPanelWidget
            {
                Direction = LayoutDirection.Vertical,
                Margin = new Vector2(30f, 0f),
                IsHitTestVisible = false,
                Children =
                {
                    new RectangleWidget
                    {
                        Subtexture = Textures.Gui.Survivalcraft2Icon,
                        Size = new Vector2(96f, 96f),
                        FillColor = Color.White,
                        OutlineColor = Color.Transparent
                    },
                    new CanvasWidget
                    {
                        Size = new Vector2(0f, 15f)
                    },
                    new LabelWidget
                    {
                        Text = "SURVIVALCRAFT 2\n",
                        Color = Colors.ForeDim,
                        FontScale = 0.75f,
                        TextAnchor = (TextAnchor.HorizontalCenter | TextAnchor.VerticalCenter)
                    }
                }
            });
            children5.Add(canvasWidget2);
            WidgetsList children7 = uniformSpacingPanelWidget.Children;
            CanvasWidget canvasWidget3 = new CanvasWidget();
            WidgetsList children8 = canvasWidget3.Children;
            ClickableWidget clickableWidget3 = new ClickableWidget();
            clickableWidget3.Size = new Vector2(0f, 0f);
            clickableWidget3.Alignment = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            widget = clickableWidget3;
            this.ClickableWidget3 = clickableWidget3;
            children8.Add(widget);
            canvasWidget3.Children.Add(new StackPanelWidget
            {
                Direction = LayoutDirection.Vertical,
                Margin = new Vector2(30f, 0f),
                IsHitTestVisible = false,
                Children =
                {
                    new RectangleWidget
                    {
                        Subtexture = Textures.Gui.BugsIcon,
                        Size = new Vector2(96f, 96f),
                        FillColor = Color.White,
                        OutlineColor = Color.Transparent
                    },
                    new CanvasWidget
                    {
                        Size = new Vector2(0f, 15f)
                    },
                    new LabelWidget
                    {
                        Text = "BUGS ATTACK\n",
                        Color = Colors.ForeDim,
                        FontScale = 0.75f,
                        TextAnchor = (TextAnchor.HorizontalCenter | TextAnchor.VerticalCenter)
                    }
                }
            });
            children7.Add(canvasWidget3);
            children2.Add(uniformSpacingPanelWidget);
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, float.PositiveInfinity)
            });
            WidgetsList children9 = stackPanelWidget.Children;
            BevelledButtonWidget bevelledButtonWidget = new BevelledButtonWidget();
            bevelledButtonWidget.Text = "OK";
            bevelledButtonWidget.Size = new Vector2(200f, 60f);
            bevelledButtonWidget.IsOkButton = true;
            bevelledButtonWidget.IsCancelButton = true;
            ButtonWidget widget2 = bevelledButtonWidget;
            this.OkButton = bevelledButtonWidget;
            children9.Add(widget2);
            stackPanelWidget.Children.Add(new CanvasWidget
            {
                Size = new Vector2(0f, 5f)
            });
            array[num] = stackPanelWidget;
            children.Add(array);
        }

        public override void Update()
        {
            for (int i = 0; i < this.Rectangles.Length; i++)
            {
                if (this.Rectangles[i] != null)
                {
                    float x = (float)MathUtils.Remainder(Time.RealTime, 10000.0) + i * 1000;
                    this.Rectangles[i].RenderTransform = Matrix.CreateTranslation(new Vector3(-this.Rectangles[i].ActualSize / 2f, 0f)) * Matrix.CreateRotationZ(6.28f * SimplexNoise.OctavedNoise(x, 0.25f, 4, 2f, 0.5f) + 3.14f) * Matrix.CreateTranslation(new Vector3(this.Rectangles[i].ActualSize / 2f, 0f));
                }
            }
            if (this.ClickableWidget1 != null)
            {
                bool isClicked = this.ClickableWidget1.IsClicked;
            }
            if (this.ClickableWidget2 != null)
            {
                bool isClicked2 = this.ClickableWidget2.IsClicked;
            }
            if (this.ClickableWidget3 != null)
            {
                bool isClicked3 = this.ClickableWidget3.IsClicked;
            }
            if (this.OkButton.IsClicked)
            {
                ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
            }
        }

        private ButtonWidget OkButton;

        private RectangleWidget[] Rectangles = new RectangleWidget[7];

        private ClickableWidget ClickableWidget1;

        private ClickableWidget ClickableWidget2;

        private ClickableWidget ClickableWidget3;
    }
}
