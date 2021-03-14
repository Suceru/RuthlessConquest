// Decompiled with JetBrains decompiler
// Type: Game.WebBrowserManager
// Assembly: RuthlessConquest, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 09ABF203-5B7E-4C78-ACFB-2EE5FE9ADF6E
// Assembly location: d:\Users\12464\Desktop\Ruthless Conquest\RuthlessConquest.exe

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Game
{
    public static class WebBrowserManager
    {
        public static void LaunchBrowser(string url)
        {
            AnalyticsManager.LogEvent("[WebBrowserManager] Launching browser", new AnalyticsParameter("Url", url));
            if (!url.Contains("://"))
                url = "http://" + url;
            try
            {
                try
                {
                    Process.Start(url);
                }
                catch (Win32Exception ex)
                {
                    if (ex.ErrorCode == -2147467259)
                        throw new InvalidOperationException("Browser not available.");
                    throw;
                }
            }
            catch (Exception ex)
            {
                Engine.Log.Error(string.Format("Error launching web browser with URL \"{0}\". Reason: {1}", url, ex.Message));
            }
        }
    }
}
