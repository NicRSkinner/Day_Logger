using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day_Logger.Logging
{
    /// <summary>
    /// Provides an interface for logging information to a file.
    /// </summary>
    public interface ILogger
    {
        void Write(string info, LogPriority priority);
    }

    /// <summary>
    /// Handles logging for stamps.
    /// </summary>
    public class StampLogger : ILogger
    {
        /// <summary>
        /// Initializer for StampLogger.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="minPri">The minimum priority to log. Ignores anything lower.</param>
        public StampLogger(string path, LogPriority minPri = LogPriority.NORMAL)
        {
            FilePath = path;
            MinPriority = minPri;
        }

        /// <summary>
        /// Writes to the specified log file.
        /// </summary>
        /// <param name="info">The info to write to the log.</param>
        /// <param name="priority">The priority of the information.</param>
        public void Write(string info, LogPriority priority)
        {
            if (String.IsNullOrWhiteSpace(info))
                throw new NullReferenceException("No log information provided.");

            if (priority < MinPriority)
                return;

            StreamWriter sWrite = new StreamWriter(FilePath, true);

            sWrite.WriteLine(DateTime.UtcNow.ToString() + ',' + info + ',' + priority);

            sWrite.Close();
        }

        private string FilePath;
        private LogPriority MinPriority;
    }

    /// <summary>
    /// The LogService for the program to log information to.
    /// </summary>
    public class LogService
    {
        /// <summary>
        /// Default Initializer. Marked private as it should never be used.
        /// </summary>
        private LogService() { }

        /// <summary>
        /// Initalizes a log given a
        /// </summary>
        /// <param name="log"></param>
        public static void InitializeLog(ILogger log)
        {
            if (Logger != null)
                throw new Exception("A logger has already been instantiated");

            Logger = log;
        }

        // The static ILogger for the singleton.
        private static ILogger Logger;
    }

    public enum LogPriority { NORMAL, WARNING, FATAL }
}