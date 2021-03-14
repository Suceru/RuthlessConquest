using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Engine;

namespace Game
{
    public static class MotdManager
    {
        public static event Action MessageOfTheDayUpdated;

        public static MotdManager.Message MessageOfTheDay
        {
            get
            {
                return m_message;
            }
            set
            {
                m_message = value;
                if (MessageOfTheDayUpdated != null)
                {
                    MessageOfTheDayUpdated();
                }
            }
        }

        public static void ForceRedownload()
        {
            SettingsManager.MotdLastUpdateTime = DateTime.MinValue;
        }

        public static void Initialize()
        {
            if (VersionsManager.Version != VersionsManager.LastLaunchedVersion)
            {
                ForceRedownload();
            }
        }

        public static void Update()
        {
            if (Time.PeriodicEvent(1.0, 0.0))
            {
                TimeSpan t = TimeSpan.FromHours(SettingsManager.MotdUpdatePeriodHours);
                DateTime now = DateTime.Now;
                if (now >= SettingsManager.MotdLastUpdateTime + t)
                {
                    SettingsManager.MotdLastUpdateTime = now;
                    Log.Information("Downloading MOTD");
                    AnalyticsManager.LogEvent("[MotdManager] Downloading MOTD", new AnalyticsParameter[]
                    {
                        new AnalyticsParameter("Time", DateTime.Now.ToString("HH:mm:ss.fff"))
                    });
                    string url = GetMotdUrl();
                    WebManager.Get(url, null, null, null, delegate (byte[] result)
                    {
                        try
                        {
                            string motdLastDownloadedData = UnpackMotd(result);
                            MessageOfTheDay = null;
                            SettingsManager.MotdLastDownloadedData = motdLastDownloadedData;
                            Log.Information("Downloaded MOTD");
                            AnalyticsManager.LogEvent("[MotdManager] Downloaded MOTD", new AnalyticsParameter[]
                            {
                                new AnalyticsParameter("Time", DateTime.Now.ToString("HH:mm:ss.fff")),
                                new AnalyticsParameter("Url", url)
                            });
                            SettingsManager.MotdUseBackupUrl = false;
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Failed processing MOTD string. Reason: " + ex.Message);
                            SettingsManager.MotdUseBackupUrl = !SettingsManager.MotdUseBackupUrl;
                        }
                    }, delegate (Exception error)
                    {
                        Log.Error("Failed downloading MOTD. Reason: {0}", new object[]
                        {
                            error.Message
                        });
                        SettingsManager.MotdUseBackupUrl = !SettingsManager.MotdUseBackupUrl;
                    });
                }
            }
            if (MessageOfTheDay == null && !string.IsNullOrEmpty(SettingsManager.MotdLastDownloadedData))
            {
                MessageOfTheDay = ParseMotd(SettingsManager.MotdLastDownloadedData);
                if (MessageOfTheDay == null)
                {
                    SettingsManager.MotdLastDownloadedData = string.Empty;
                }
            }
        }

        private static string UnpackMotd(byte[] data)
        {
            string text = "motd.xml";
            using (MemoryStream memoryStream = new MemoryStream(data))
            {
                using (ZipArchive zipArchive = ZipArchive.Open(memoryStream, false))
                {
                    foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.ReadCentralDir())
                    {
                        if (zipArchiveEntry.FilenameInZip.ToLower() == text)
                        {
                            MemoryStream memoryStream2 = new MemoryStream();
                            zipArchive.ExtractFile(zipArchiveEntry, memoryStream2);
                            memoryStream2.Position = 0L;
                            using (StreamReader streamReader = new StreamReader(memoryStream2))
                            {
                                return streamReader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            throw new InvalidOperationException(string.Format("\"{0}\" file not found in Motd zip archive.", text));
        }

        private static MotdManager.Message ParseMotd(string dataString)
        {
            try
            {
                int num = dataString.IndexOf("<Motd");
                if (num >= 0)
                {
                    int num2 = dataString.IndexOf("</Motd>");
                    if (num2 >= 0 && num2 > num)
                    {
                        num2 += 7;
                    }
                    XElement xelement = XmlUtils.LoadXmlFromString(dataString.Substring(num, num2 - num), true);
                    SettingsManager.MotdUpdatePeriodHours = XmlUtils.GetAttributeValue<int>(xelement, "UpdatePeriodHours", 24);
                    SettingsManager.MotdUpdateUrl = XmlUtils.GetAttributeValue<string>(xelement, "UpdateUrl", SettingsManager.MotdUpdateUrl);
                    SettingsManager.MotdBackupUpdateUrl = XmlUtils.GetAttributeValue<string>(xelement, "BackupUpdateUrl", SettingsManager.MotdBackupUpdateUrl);
                    MotdManager.Message message = new MotdManager.Message();
                    foreach (XElement xelement2 in xelement.Elements())
                    {
                        if (WidgetsManager.IsNodeIncludedOnCurrentPlatform(xelement2))
                        {
                            MotdManager.Line item = new MotdManager.Line
                            {
                                Time = XmlUtils.GetAttributeValue<float>(xelement2, "Time"),
                                Node = xelement2.Elements().FirstOrDefault<XElement>(),
                                Text = xelement2.Value
                            };
                            message.Lines.Add(item);
                        }
                    }
                    return message;
                }
                throw new InvalidOperationException("Invalid MOTD data string.");
            }
            catch (Exception ex)
            {
                Log.Warning("Failed extracting MOTD string. Reason: " + ex.Message);
            }
            return null;
        }

        private static string GetMotdUrl()
        {
            if (SettingsManager.MotdUseBackupUrl)
            {
                return string.Format(SettingsManager.MotdBackupUpdateUrl, VersionsManager.Version.ToString(2));
            }
            return string.Format(SettingsManager.MotdUpdateUrl, VersionsManager.Version.ToString(2));
        }

        private static MotdManager.Message m_message;

        public class Message
        {
            public List<MotdManager.Line> Lines = new List<MotdManager.Line>();
        }

        public class Line
        {
            public float Time;

            public XElement Node;

            public string Text;
        }
    }
}
