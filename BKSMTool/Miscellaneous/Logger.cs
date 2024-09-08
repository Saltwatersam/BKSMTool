// Copyright (c) 2024 Saltwatersam
// 
// This file is part of BKSMTool.
// 
// BKSMTool is licensed under the GPLv3 License:
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using System.Linq;
using System.Text;
using System;
using System.IO;

namespace BKSMTool.Miscellaneous
{
    /// <summary>
    /// Logger class to log messages and exceptions to a log file.
    /// It also includes functionality to clean up old log entries.
    /// </summary>
    public static class Logger
    {
        #region METHODS

        #region Get Log File Path

        /// <summary>
        /// Gets the path to the log file. It ensures the log directory exists in the AppData folder.
        /// </summary>
        /// <returns>The full path to the log file.</returns>
        private static string GetLogFilePath()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var logDirectory = Path.Combine(appDataFolder, "BKSMTool");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            return Path.Combine(logDirectory, "BKSMTool.log");
        }

        #endregion

        #region Log Message

        /// <summary>
        /// Logs a message to the log file with a timestamp.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(string message)
        {
            CleanUpOldLogs();
            var logFilePath = GetLogFilePath();

            using (var writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine("***"); // Start marker
                writer.WriteLine(DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));
                writer.WriteLine(message);
                writer.WriteLine("***"); // End marker
            }
        }

        #endregion

        #region Log Exception

        /// <summary>
        /// Logs an exception to the log file, including the message and stack trace.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        public static void Log(Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Exception: {ex.Message}");
            sb.AppendLine($"Stack Trace: {ex.StackTrace}");
            Log(sb.ToString());
        }

        #endregion

        #region Clean Up Old Logs

        /// <summary>
        /// Cleans up old log entries that are older than one day from the log file.
        /// </summary>
        private static void CleanUpOldLogs()
        {
            var logFilePath = GetLogFilePath();

            if (!File.Exists(logFilePath)) return;
            // Read and filter the log entries to keep only recent ones
            var recentEntries = File.ReadAllText(logFilePath)
                .Split(new[] { "***" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(entry => entry.Trim())
                .Where(IsRecentLogEntry)
                .Select(entry => $"***\n{entry}\n***")
                .ToList();

            // Write the filtered entries back to the log file
            File.WriteAllText(logFilePath, string.Join(Environment.NewLine, recentEntries));
        }

        #endregion

        #region Is Recent Log Entry

        /// <summary>
        /// Determines if a log entry is recent, defined as being within the last 24 hours.
        /// </summary>
        /// <param name="logEntry">The log entry to check.</param>
        /// <returns>True if the log entry is recent, false otherwise.</returns>
        private static bool IsRecentLogEntry(string logEntry)
        {
            var lines = logEntry.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            if (lines.Length <= 1) return false;

            // Parse the timestamp of the log entry to determine its recency
            if (DateTime.TryParseExact(
                lines[0].Trim(),
                "dd.MM.yyyy HH:mm:ss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out var logDate))
            {
                return (DateTime.Now - logDate).TotalDays <= 1;
            }

            return false;
        }

        #endregion

        #endregion
    }
}
