using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Engine;
using Engine.Serialization;

namespace Game
{
    internal static class SettingsManager
    {
        public static void Initialize()
        {
            PlayerName = string.Empty;
            PlayerGuid = Guid.NewGuid();
            Faction = (Faction)new Engine.Random().Int(6);
            DefaultFleetStrength = 0.6f;
            LastColorScheme = new Engine.Random().Int();
            InternalMusicVolume = 0.5f;
            InternalSfxVolume = 1f;
            PresentationInterval = 1;
            GamepadDeadZone = 0.16f;
            GamepadCursorSpeed = 1f;
            MotdUpdateUrl = "https://scresdir.appspot.com/motd/RuthlessConquest/Motd-{0}.zip";
            MotdBackupUpdateUrl = "https://scresdir.appspot.com/motd/RuthlessConquest/Motd-{0}.zip";
            MotdLastUpdateTime = DateTime.MinValue;
            MotdLastDownloadedData = string.Empty;
            LoadSettings();
            MinimumHoldDuration = 0.3f;
            MinimumDragDistance = 15f;
            Window.Deactivated += delegate ()
            {
                SaveSettings();
            };
        }

        public static Version LastLaunchedVersion { get; set; }

        public static string PlayerName { get; set; }

        public static Guid PlayerGuid { get; set; }

        public static Faction Faction
        {
            get
            {
                return InternalFaction;
            }
            set
            {
                InternalFaction = (Faction)MathUtils.Clamp((int)value, 0, 5);
            }
        }

        public static float DefaultFleetStrength
        {
            get
            {
                return InternalDefaultFleetStrength;
            }
            set
            {
                InternalDefaultFleetStrength = MathUtils.Clamp(value, 0.2f, 1f);
            }
        }
        /// <summary>
        /// 不再提示
        /// </summary>
        public static bool DontShowInstructions { get; set; }

        public static bool InstructionsShown { get; set; }

        public static int LastColorScheme { get; set; }

        public static float MusicVolume
        {
            get
            {
                return InternalMusicVolume;
            }
            set
            {
                InternalMusicVolume = MathUtils.Saturate(value);
            }
        }

        public static float SfxVolume
        {
            get
            {
                return InternalSfxVolume;
            }
            set
            {
                InternalSfxVolume = MathUtils.Saturate(value);
            }
        }

        public static int PresentationInterval { get; set; }

        public static float MinimumHoldDuration { get; set; }

        public static float MinimumDragDistance { get; set; }

        public static float GamepadDeadZone { get; set; }

        public static float GamepadCursorSpeed { get; set; }

        public static string MotdUpdateUrl { get; set; }

        public static string MotdBackupUpdateUrl { get; set; }

        public static bool MotdUseBackupUrl { get; set; }

        public static double MotdUpdatePeriodHours { get; set; }

        public static DateTime MotdLastUpdateTime { get; set; }

        public static string MotdLastDownloadedData { get; set; }

        public static bool DisplayFpsCounter { get; set; }

        public static bool DisplayFpsRibbon { get; set; }

        public static bool UpsideDownLayout { get; set; }

        public static void LoadSettings()
        {
            try
            {
                if (Storage.FileExists("data:Settings.xml"))
                {
                    using (Stream stream = Storage.OpenFile("data:Settings.xml", OpenFileMode.Read))
                    {
                        foreach (XElement node in XmlUtils.LoadXmlFromStream(stream, null, true).Elements())
                        {
                            string name = "<unknown>";
                            try
                            {
                                name = XmlUtils.GetAttributeValue<string>(node, "Name");
                                string attributeValue = XmlUtils.GetAttributeValue<string>(node, "Value");
                                PropertyInfo propertyInfo = (from pi in typeof(SettingsManager).GetRuntimeProperties()
                                                             where pi.Name == name && pi.GetMethod.IsStatic && pi.GetMethod.IsPublic && pi.SetMethod.IsPublic
                                                             select pi).FirstOrDefault<PropertyInfo>();
                                if (propertyInfo != null)
                                {
                                    object value = HumanReadableConverter.ConvertFromString(propertyInfo.PropertyType, attributeValue);
                                    propertyInfo.SetValue(null, value, null);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Warning(string.Format("Setting \"{0}\" could not be loaded. Reason: {1}", name, ex.Message));
                            }
                        }
                    }
                    Log.Information("Loaded settings.");
                }
            }
            catch (Exception e)
            {
                ExceptionManager.ReportExceptionToUser("Loading settings failed.", e);
            }
        }

        public static void SaveSettings()
        {
            try
            {
                XElement xelement = new XElement("Settings");
                foreach (PropertyInfo propertyInfo in from pi in typeof(SettingsManager).GetRuntimeProperties()
                                                      where pi.GetMethod.IsStatic && pi.GetMethod.IsPublic && pi.SetMethod.IsPublic
                                                      select pi)
                {
                    try
                    {
                        string value = HumanReadableConverter.ConvertToString(propertyInfo.GetValue(null, null));
                        XElement node = XmlUtils.AddElement(xelement, "Setting");
                        XmlUtils.SetAttributeValue(node, "Name", propertyInfo.Name);
                        XmlUtils.SetAttributeValue(node, "Value", value);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(string.Format("Setting \"{0}\" could not be saved. Reason: {1}", propertyInfo.Name, ex.Message));
                    }
                }
                using (Stream stream = Storage.OpenFile("data:Settings.xml", OpenFileMode.Create))
                {
                    XmlUtils.SaveXmlToStream(xelement, stream, null, true);
                }
                Log.Information("Saved settings");
            }
            catch (Exception e)
            {
                ExceptionManager.ReportExceptionToUser("Saving settings failed.", e);
            }
        }

        private const string m_settingsFileName = "data:Settings.xml";

        private static Faction InternalFaction;

        private static float InternalDefaultFleetStrength;

        private static float InternalMusicVolume;

        private static float InternalSfxVolume;
    }
}
