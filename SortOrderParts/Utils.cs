using System;
using System.IO;

namespace Utils
{
    internal static class Utils
    {
        public static string Left(this string s, int count)
        {
            return s.Substring(0, count);
        }

        public static string Right(this string s, int count)
        {
            return s.Substring(s.Length - count);
        }
    }

    internal static class UMMLogger
    {
        internal static string LogPath { get; private set; }
        internal static bool LoggingOk { get; private set; }

        internal static void Init(string path, string text)
        {
            LogPath = path;
            LoggingOk = true;
            try
            {
                WriteLog(text);
            }
            catch (Exception e)
            {
                LoggingOk = false;
                throw e;
            }
        }

        internal static void WriteLog(string text)
        {
            if (LoggingOk)
            {
                File.AppendAllText(LogPath, "[" + DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified).ToString("yyyy-MM-dd HH:mm:ss") + "] " + text + "\n");
            }
        }

        internal static void LogException(Exception e)
        {
            string message = "Exception info: \n" + e.Message + "\n";
            if (e.InnerException != null)
            {
                message += e.InnerException.Message + "\n";
            }
            message += e.StackTrace;
            WriteLog(message);
        }
    }
}
