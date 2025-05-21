using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace QuestViewer
{
    public static class Utils
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// This method calls from Logger instance. Description there is in NLog.config file
        /// </summary>
        public static void LogToChronos(string level, string message)
        {
            try
            {
                LogToChronosInternal(level, message);
            }
            catch (Exception ex)
            {
                logger.Debug(ex, ex.GetBaseException().Message);
            }
        }

        private static void LogToChronosInternal(string level, string messageText)
        {
            var formattedMessage = messageText
                .Replace(@"\", @"/")
                .Replace("\r", "")
                .Replace("\n", @"\n");

            ChronosLogLevel logLevel = ChronosLogLevel.Info;
            var nLogLevel = LogLevel.FromString(level);

            if (nLogLevel == LogLevel.Debug || nLogLevel == LogLevel.Trace)
                return;

            if (nLogLevel == LogLevel.Fatal)
                logLevel = ChronosLogLevel.Fatal;
            else if (nLogLevel == LogLevel.Error)
                logLevel = ChronosLogLevel.Error;
            else if (nLogLevel == LogLevel.Warn)
                logLevel = ChronosLogLevel.Warning;
            else if (nLogLevel == LogLevel.Info)
                logLevel = ChronosLogLevel.Info;

            var metadata = new Json.JsonObject();
            metadata["username"] = string.IsNullOrWhiteSpace(Config.UserNameFromCurrentConnection) ? "Unauthorized" : Config.UserNameFromCurrentConnection;
            // ChronosAPI.Instance.AppLogAsync(new List<LogEntry> { new(logLevel, DateTimeOffset.UtcNow, metadata, formattedMessage) });
        }

        public static string StringToMD5(string value)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var valueBytes = Encoding.ASCII.GetBytes(value);
                var data = md5.ComputeHash(valueBytes);

                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                    sBuilder.Append(data[i].ToString("x2"));

                return sBuilder.ToString();
            }
        }

        public static string GetAppRevision()
        {
            var revFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}\\rev.conf";
            return File.Exists(revFilePath) ? File.ReadAllText(revFilePath).Trim() : "None";
        }

        public static string GetAppVersion()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.FileVersion;
        }

        public static string GetAppVersionFormat()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return $"{fvi.FileMajorPart}.{fvi.FileMinorPart}.{fvi.FileBuildPart}";
        }

        public static string GetAppVersionRevosionFormat()
        {
            return $"{GetAppVersionFormat()} Rev. {GetAppRevision()}";
        }
    }
}