using System;
using Engine;
using Engine.Input;

namespace Game
{
    public static class ExceptionManager
    {
        public static Exception Error
        {
            get
            {
                return m_error;
            }
        }

        public static void ReportExceptionToUser(string additionalMessage, Exception e)
        {
            string arg = MakeFullErrorMessage(additionalMessage, e);
            Log.Error(string.Format("{0}\n{1}", arg, e.StackTrace));
            AnalyticsManager.LogError(additionalMessage, e);
        }

        public static void DrawExceptionScreen()
        {
        }

        public static void UpdateExceptionScreen()
        {
        }

        public static string MakeFullErrorMessage(Exception e)
        {
            return MakeFullErrorMessage(null, e);
        }

        public static string MakeFullErrorMessage(string additionalMessage, Exception e)
        {
            string text = string.Empty;
            if (!string.IsNullOrEmpty(additionalMessage))
            {
                text = additionalMessage;
            }
            for (Exception ex = e; ex != null; ex = ex.InnerException)
            {
                text = text + ((text.Length > 0) ? Environment.NewLine : string.Empty) + ex.Message;
            }
            return text;
        }

        private static bool CheckContinueKey()
        {
            return Keyboard.IsKeyDown(Key.F12) || Keyboard.IsKeyDown(Key.Back);
        }

        private static Exception m_error;
    }
}
