using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Engine;
using Engine.Content;
using Engine.Graphics;
namespace Game
{
    public static class Program
    {
        public static float LastFrameTime { get; private set; }

        public static float LastCpuFrameTime { get; private set; }

        [STAThread]
        public static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            if (args.Length == 1 && args[0] == "s")
            {
                try
                {
                    Stream stream = Storage.OpenFile("system:" + Storage.CombinePaths(new string[]
                    {
                        Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                        "ServerLog.txt"
                    }), OpenFileMode.CreateOrOpen);
                    stream.Position = stream.Length;
                    Log.AddLogSink(new StreamLogSink(stream));
                    new Server(true, false);
                }
                catch (Exception message)
                {
                    Log.Error(message);
                }
                Log.Information("Server exiting");
                return;
            }
            Log.RemoveAllLogSinks();
            Log.AddLogSink(new GameLogSink());
            Window.HandleUri += HandleUriHandler;
            Window.Deactivated += DeactivatedHandler;
            Window.Frame += FrameHandler;
            Window.UnhandledException += delegate (UnhandledExceptionInfo e)
            {
                ExceptionManager.ReportExceptionToUser("Unhandled exception.", e.Exception);
                e.IsHandled = true;
            };
            Window.Run(0, 0, WindowMode.Resizable, "Ruthless Conquest");
        }

        public static event Action<Uri> HandleUri;

        private static void HandleUriHandler(Uri uri)
        {
            UrisToHandle.Add(uri);
        }

        private static void DeactivatedHandler()
        {
            GC.Collect();
        }

        private static void FrameHandler()
        {
            if (Time.FrameIndex < 0)
            {
                Display.Clear(new Vector4?(Vector4.Zero), new float?(1), null);
                return;
            }
            if (Time.FrameIndex == 0)
            {
                Initialize();
                return;
            }
            Run();
        }

        private static void Initialize()
        {
            Log.Information(string.Format("Ruthless Conquest starting up at {0:dd/MM/yyyy HH:mm:ss} UTC, Version={1}, BuildConfiguration={2}, Platform={3}, Storage.AvailableFreeSpace={4}MB, ApproximateScreenDpi={5:0.0}, ApproxScreenInches={6:0.0}, ScreenResolution={7}, ProcessorsCount={8}", new object[]
            {
                DateTime.Now.ToUniversalTime(),
                VersionsManager.Version,
                VersionsManager.BuildConfiguration,
                VersionsManager.Platform,
                Storage.FreeSpace / 1024L / 1024L,
                ScreenResolutionManager.ApproximateScreenDpi,
                ScreenResolutionManager.ApproximateWindowInches,
                Window.ScreenSize,
                Environment.ProcessorCount
            }));
            try
            {
                Window.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            }
            catch (Exception ex)
            {
                Log.Warning("Unable to set application icon. Reason: {0}", new object[]
                {
                    ex
                });
            }
            string s = "rXhjy84BcuQc035F0Qx5n9lanMUEuVSKDoOikYM1Qx3AF9oZDxyeFwAhmACTstkl3Zu3PKumRsYJJ5xUVw53gwTwuwDHIPis7y4jVOqrnZmJujlH00cDmvnLu9uw3LdfH8d9a";
            //将Content/Content.pak设置为复制到输出目录（始终复制）
            ContentCache.AddPackage("app:Content/Content.pak", Encoding.UTF8.GetBytes(s), new byte[] { 63 });
            MarketplaceManager.Initialize();
            SettingsManager.Initialize();
            AnalyticsManager.Initialize();
            VersionsManager.Initialize();
            WidgetsManager.Initialize();
            ScreensManager.Initialize();
        }
        //运行
        private static void Run()
        {
            //获取时间
            double realTime = Time.RealTime;
            LastFrameTime = (float)(realTime - FrameBeginTime);
            LastCpuFrameTime = (float)(CpuEndTime - FrameBeginTime);
            FrameBeginTime = realTime;
            //窗口设置
            Window.PresentationInterval = SettingsManager.PresentationInterval;
            try
            {
                if (ExceptionManager.Error == null)
                {
                    while (UrisToHandle.Count > 0)
                    {
                        Uri obj = UrisToHandle[0];
                        UrisToHandle.RemoveAt(0);
                        Action<Uri> handleUri = HandleUri;
                        if (handleUri != null)
                        {
                            handleUri(obj);
                        }
                    }
                    PerformanceManager.Update();
                    ServersManager.Update();
                    MotdManager.Update();
                    MusicManager.Update();
                    ScreensManager.Update();
                    DialogsManager.Update();
                    WidgetsManager.Update();
                }
                else
                {
                    ExceptionManager.UpdateExceptionScreen();
                }
            }
            catch (Exception e)
            {
                ExceptionManager.ReportExceptionToUser(null, e);
                ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
            }
            try
            {
                Display.RenderTarget = null;
                if (ExceptionManager.Error == null)
                {
                    ScreensManager.Draw();
                    WidgetsManager.Draw();
                    PerformanceManager.Draw();
                }
                else
                {
                    ExceptionManager.DrawExceptionScreen();
                }
                CpuEndTime = Time.RealTime;
            }
            catch (Exception e2)
            {
                ExceptionManager.ReportExceptionToUser(null, e2);
                ScreensManager.SwitchScreen("MainMenu", Array.Empty<object>());
            }
        }

        private static double FrameBeginTime;

        private static double CpuEndTime;

        private static List<Uri> UrisToHandle = new List<Uri>();
    }
}
