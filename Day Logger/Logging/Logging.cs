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
        public StampLogger(string path, LogPriority minPri = LogPriority.NORMAL)
        {
            FilePath = path;
            MinPriority = minPri;
        }

        public void Write(string info, LogPriority priority)
        {
            if (String.IsNullOrWhiteSpace(info))
                throw new NullReferenceException("No log information provided.");

            if (priority < MinPriority)
                return;
        }

        private string FilePath;
        private LogPriority MinPriority;
    }

    public class LogService
    {
        public LogService(ILogger log)
        {

        }
    }

    public enum LogPriority { NORMAL, WARNING, FATAL }
}