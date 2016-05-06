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
        void Open();
        void Close();
    }

    public class StampLogger : ILogger
    {
        public StampLogger(string path)
        {
            FilePath = path;
        }

        public void Write(string info, LogPriority priority)
        {

        }

        private string FilePath;
    }

    public class LogService
    {
        public LogService(ILogger log)
        {

        }
    }

    public enum LogPriority { NORMAL, WARNING, FATAL }
}