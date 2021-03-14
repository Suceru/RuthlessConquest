using System;
using System.Collections.Generic;
using Engine;
using Engine.Content;

namespace Game
{
    internal class LoadingScreen : Screen
    {
        public LoadingScreen()
        {
            this.Children.Add(new Widget[]
            {
                new ClearWidget
                {
                    ClearColor = true,
                    Color = Color.Black
                },
                new InterlaceWidget(),
                new RectangleWidget
                {
                    FillColor = Color.White,
                    OutlineColor = Color.Transparent,
                    Size = new Vector2(256f),
                    Subtexture = Textures.Gui.CandyRufusLogo
                },
                new RectangleWidget
                {
                    FillColor = Color.White,
                    OutlineColor = Color.Transparent,
                    Size = new Vector2(80f, 50f),
                    Subtexture = Textures.Gui.EngineLogo,
                    Alignment = new Vector2(1f, 1f)
                },
                new BusyBarWidget
                {
                    Alignment = new Vector2(0f, 1f),
                    Margin = new Vector2(0f, 40f)
                }
            });
            this.AddLoadAction(delegate
            {
                MotdManager.Initialize();
            });
            this.AddLoadAction(delegate
            {
                TextureAtlasManager.LoadAtlases();
            });
            foreach (ContentInfo localContentInfo2 in ContentCache.List(""))
            {
                ContentInfo localContentInfo = localContentInfo2;
                this.AddLoadAction(delegate
                {
                    ContentCache.Get(localContentInfo.Name, true);
                });
            }
            this.AddLoadAction(delegate
            {
                MusicManager.CurrentMix = MusicManager.Mix.Menu;
            });
        }

        public void AddLoadAction(Action action)
        {
            this.LoadActions.Add(action);
        }

        public override void Leave()
        {
            ContentCache.Dispose("Textures/Gui/CandyRufusLogo");
            ContentCache.Dispose("Textures/Gui/EngineLogo");
        }

        public override void Update()
        {
            if (!this.LoadingStarted)
            {
                this.LoadingStarted = true;
                return;
            }
            if (this.LoadingFinished)
            {
                return;
            }
            double realTime = Time.RealTime;
            while (!this.PauseLoading && this.Index < this.LoadActions.Count)
            {
                try
                {
                    List<Action> loadActions = this.LoadActions;
                    int index = this.Index;
                    this.Index = index + 1;
                    loadActions[index]();
                }
                catch (Exception ex)
                {
                    Log.Error("Loading error. Reason: " + ex.Message);
                    if (!this.LoadingErrorsSuppressed)
                    {
                        this.PauseLoading = true;
                        DialogsManager.ShowDialog(WidgetsManager.RootWidget, new MessageDialog("Loading Error", ExceptionManager.MakeFullErrorMessage(ex), "OK", "Suppress", delegate (MessageDialogButton b)
                        {
                            if (b == MessageDialogButton.Button1)
                            {
                                this.PauseLoading = false;
                                return;
                            }
                            if (b == MessageDialogButton.Button2)
                            {
                                this.LoadingErrorsSuppressed = true;
                            }
                        }), true);
                    }
                }
                if (Time.RealTime - realTime > 0.1)
                {
                    break;
                }
            }
            if (this.Index >= this.LoadActions.Count)
            {
                this.LoadingFinished = true;
                ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
            }
        }

        private List<Action> LoadActions = new List<Action>();

        private int Index;

        private bool LoadingStarted;

        private bool LoadingFinished;

        private bool PauseLoading;

        private bool LoadingErrorsSuppressed;
    }
}
