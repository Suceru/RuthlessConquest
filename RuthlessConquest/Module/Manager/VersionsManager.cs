using System;
using System.Reflection;

namespace Game
{
    public static class VersionsManager
    {
        public static Platform Platform
        {
            get
            {
                return Platform.Desktop;
            }
        }

        public static BuildConfiguration BuildConfiguration
        {
            get
            {
                return BuildConfiguration.Release;
            }
        }

        public static Version Version { get; private set; }

        public static Version LastLaunchedVersion { get; private set; }

        static VersionsManager()
        {
            AssemblyName assemblyName = new AssemblyName(typeof(VersionsManager).GetTypeInfo().Assembly.FullName);
            if (assemblyName.Version.Major > 255 || assemblyName.Version.Minor > 255 || assemblyName.Version.Build > 255 || assemblyName.Version.Revision > 255)
            {
                throw new InvalidOperationException("Unsupported version number.");
            }
            Version = new Version((byte)assemblyName.Version.Major, (byte)assemblyName.Version.Minor, (byte)assemblyName.Version.Build, (byte)assemblyName.Version.Revision);
        }

        public static void Initialize()
        {
            LastLaunchedVersion = SettingsManager.LastLaunchedVersion;
            SettingsManager.LastLaunchedVersion = Version;
            if (Version != LastLaunchedVersion)
            {
                AnalyticsManager.LogEvent("[VersionsManager] Upgrade game", new AnalyticsParameter[]
                {
                    new AnalyticsParameter("LastVersion", LastLaunchedVersion.ToString(4)),
                    new AnalyticsParameter("CurrentVersion", Version.ToString(4))
                });
            }
        }
    }
}
