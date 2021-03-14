using System;
using System.Collections.Generic;
using System.IO;
using Engine;

namespace Game
{
    public class GameLogSink : ILogSink
    {
        public GameLogSink()
        {
            try
            {
                if (m_stream != null)
                {
                    throw new InvalidOperationException("GameLogSink already created.");
                }
                string text = "data:/Logs";
                string path = Storage.CombinePaths(new string[]
                {
                    text,
                    "Game.log"
                });
                Storage.CreateDirectory(text);
                m_stream = Storage.OpenFile(path, OpenFileMode.CreateOrOpen);
                if (m_stream.Length > 10485760L)
                {
                    m_stream.Dispose();
                    m_stream = Storage.OpenFile(path, OpenFileMode.Create);
                }
                m_stream.Position = m_stream.Length;
                m_writer = new StreamWriter(m_stream);
            }
            catch (Exception ex)
            {
                Engine.Log.Error("Error creating GameLogSink. Reason: {0}", new object[]
                {
                    ex.Message
                });
            }
        }

        public static string GetRecentLog(int bytesCount)
        {
            if (m_stream == null)
            {
                return string.Empty;
            }
            Stream stream = m_stream;
            string result;
            lock (stream)
            {
                try
                {
                    m_stream.Position = MathUtils.Max(m_stream.Position - bytesCount, 0L);
                    result = new StreamReader(m_stream).ReadToEnd();
                }
                finally
                {
                    m_stream.Position = m_stream.Length;
                }
            }
            return result;
        }

        public static List<string> GetRecentLogLines(int bytesCount)
        {
            if (m_stream == null)
            {
                return new List<string>();
            }
            Stream stream = m_stream;
            List<string> result;
            lock (stream)
            {
                try
                {
                    m_stream.Position = MathUtils.Max(m_stream.Position - bytesCount, 0L);
                    StreamReader streamReader = new StreamReader(m_stream);
                    List<string> list = new List<string>();
                    for (; ; )
                    {
                        string text = streamReader.ReadLine();
                        if (text == null)
                        {
                            break;
                        }
                        list.Add(text);
                    }
                    result = list;
                }
                finally
                {
                    m_stream.Position = m_stream.Length;
                }
            }
            return result;
        }

        public void Log(LogType type, string message)
        {
            if (m_stream == null)
            {
                return;
            }
            Stream stream = m_stream;
            lock (stream)
            {
                string value;
                switch (type)
                {
                    case LogType.Debug:
                        value = "DEBUG: ";
                        break;
                    case LogType.Verbose:
                        value = "INFO: ";
                        break;
                    case LogType.Information:
                        value = "INFO: ";
                        break;
                    case LogType.Warning:
                        value = "WARNING: ";
                        break;
                    case LogType.Error:
                        value = "ERROR: ";
                        break;
                    default:
                        value = string.Empty;
                        break;
                }
                m_writer.Write(DateTime.Now.ToString("HH:mm:ss.fff"));
                m_writer.Write(" ");
                m_writer.Write(value);
                m_writer.WriteLine(message);
                m_writer.Flush();
            }
        }

        private static Stream m_stream;

        private static StreamWriter m_writer;
    }
}
