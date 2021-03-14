using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using Engine.Graphics;

namespace Game
{
    internal static class ScreensManager
    {
        public static bool IsAnimating
        {
            get
            {
                return m_animationData != null;
            }
        }

        public static Screen CurrentScreen { get; private set; }

        public static Screen PreviousScreen { get; private set; }

        public static T FindScreen<T>(string name) where T : Screen
        {
            Screen screen;
            m_screens.TryGetValue(name, out screen);
            return (T)screen;
        }

        public static void AddScreen(string name, Screen screen)
        {
            m_screens.Add(name, screen);
        }

        public static void SwitchScreen(string name, params object[] parameters)
        {
            SwitchScreen(string.IsNullOrEmpty(name) ? null : FindScreen<Screen>(name), parameters);
        }

        public static void SwitchScreen(Screen screen, params object[] parameters)
        {
            if (m_animationData != null)
            {
                EndAnimation();
            }
            m_animationData = new ScreensManager.AnimationData
            {
                NewScreen = screen,
                OldScreen = CurrentScreen,
                Parameters = parameters,
                Speed = ((CurrentScreen == null) ? float.MaxValue : 3f)
            };
            if (CurrentScreen != null)
            {
                WidgetsManager.IsUpdateEnabled = false;
                CurrentScreen.Input.Clear();
            }
            PreviousScreen = CurrentScreen;
            CurrentScreen = screen;
            UpdateAnimation();
            if (CurrentScreen != null)
            {
                Log.Verbose(string.Format("Entered screen \"{0}\"", GetScreenName(CurrentScreen)));
                AnalyticsManager.LogEvent(string.Format("[{0}] Entered screen", GetScreenName(CurrentScreen)), new AnalyticsParameter[]
                {
                    new AnalyticsParameter("Time", DateTime.Now.ToString("HH:mm:ss.fff"))
                });
            }
        }

        public static void Initialize()
        {
            LoadingScreen loadingScreen = new LoadingScreen();
            AddScreen("Loading", loadingScreen);
            loadingScreen.AddLoadAction(delegate
            {
                AddScreen("MainMenu", new MainMenuScreen());
            });
            loadingScreen.AddLoadAction(delegate
            {
                AddScreen("Settings", new SettingsScreen());
            });
            loadingScreen.AddLoadAction(delegate
            {
                AddScreen("Credits", new CreditsScreen());
            });
            loadingScreen.AddLoadAction(delegate
            {
                AddScreen("GameList", new GameListScreen());
            });
            loadingScreen.AddLoadAction(delegate
            {
                AddScreen("PracticeSetup", new PracticeSetupScreen());
            });
            loadingScreen.AddLoadAction(delegate
            {
                AddScreen("GameSetup", new GameSetupScreen());
            });
            loadingScreen.AddLoadAction(delegate
            {
                AddScreen("Game", new GameScreen());
            });
            SwitchScreen("Loading", Array.Empty<object>());
        }

        public static void Update()
        {
            if (m_animationData != null)
            {
                UpdateAnimation();
            }
        }

        public static void Draw()
        {
            if (m_animationData != null)
            {
                Display.Clear(new Color?(Color.Black), new float?(1f), new int?(0));
            }
        }

        private static void UpdateAnimation()
        {
            float num = MathUtils.Min(Time.FrameDuration, 0.1f);
            float factor = m_animationData.Factor;
            m_animationData.Factor = MathUtils.Min(m_animationData.Factor + m_animationData.Speed * num, 1f);
            if (m_animationData.Factor < 0.5f)
            {
                if (m_animationData.OldScreen != null)
                {
                    float num2 = 2f * (0.5f - m_animationData.Factor);
                    float scale = 1f;
                    m_animationData.OldScreen.ColorTransform = new Color(num2, num2, num2, 1f);
                    m_animationData.OldScreen.RenderTransform = Matrix.CreateTranslation(-m_animationData.OldScreen.ActualSize.X / 2f, -m_animationData.OldScreen.ActualSize.Y / 2f, 0f) * Matrix.CreateScale(scale) * Matrix.CreateTranslation(m_animationData.OldScreen.ActualSize.X / 2f, m_animationData.OldScreen.ActualSize.Y / 2f, 0f);
                }
            }
            else if (factor < 0.5f)
            {
                if (m_animationData.OldScreen != null)
                {
                    m_animationData.OldScreen.Leave();
                    WidgetsManager.RootWidget.Children.Remove(m_animationData.OldScreen);
                    DialogsManager.HideAllDialogs(m_animationData.OldScreen, false);
                }
                if (m_animationData.NewScreen != null)
                {
                    WidgetsManager.RootWidget.Children.Insert(0, m_animationData.NewScreen);
                    m_animationData.NewScreen.Enter(m_animationData.Parameters);
                    m_animationData.NewScreen.ColorTransform = Color.Transparent;
                    WidgetsManager.IsUpdateEnabled = true;
                }
            }
            else if (m_animationData.NewScreen != null)
            {
                float num3 = 2f * (m_animationData.Factor - 0.5f);
                float scale2 = 1f;
                m_animationData.NewScreen.ColorTransform = new Color(num3, num3, num3, 1f);
                m_animationData.NewScreen.RenderTransform = Matrix.CreateTranslation(-m_animationData.NewScreen.ActualSize.X / 2f, -m_animationData.NewScreen.ActualSize.Y / 2f, 0f) * Matrix.CreateScale(scale2) * Matrix.CreateTranslation(m_animationData.NewScreen.ActualSize.X / 2f, m_animationData.NewScreen.ActualSize.Y / 2f, 0f);
            }
            if (m_animationData.Factor >= 1f)
            {
                EndAnimation();
            }
        }

        private static void EndAnimation()
        {
            if (m_animationData.NewScreen != null)
            {
                m_animationData.NewScreen.ColorTransform = Color.White;
                m_animationData.NewScreen.RenderTransform = Matrix.CreateScale(1f);
            }
            m_animationData = null;
        }

        private static string GetScreenName(Screen screen)
        {
            string key = m_screens.FirstOrDefault((KeyValuePair<string, Screen> kvp) => kvp.Value == screen).Key;
            if (key == null)
            {
                return string.Empty;
            }
            return key;
        }

        private static Dictionary<string, Screen> m_screens = new Dictionary<string, Screen>();

        private static ScreensManager.AnimationData m_animationData;

        private class AnimationData
        {
            public Screen OldScreen;

            public Screen NewScreen;

            public float Factor;

            public float Speed;

            public object[] Parameters;
        }
    }
}
