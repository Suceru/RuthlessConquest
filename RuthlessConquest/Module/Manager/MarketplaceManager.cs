﻿using System;

namespace Game
{
    public static class MarketplaceManager
    {
        private static bool m_isInitialized;
        private static bool m_isTrialMode;

        public static bool IsTrialMode
        {
            get
            {
                if (!m_isInitialized)
                    throw new InvalidOperationException("MarketplaceManager not initialized.");
                return m_isTrialMode;
            }
            private set => m_isTrialMode = value;
        }

        public static void Initialize() => m_isInitialized = true;

        public static void ShowMarketplace()
        {
            AnalyticsManager.LogEvent("[MarketplaceManager] Show marketplace");
            WebBrowserManager.LaunchBrowser("https://kaalus.wordpress.com/ruthless-conquest");
        }
    }
}
