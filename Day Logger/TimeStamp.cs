using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Day_Logger
{
    public class TimeStamp
    {
        public string STime
        {
            get { return sTime; }
            set { sTime = value; }
        }

        public string ETime
        {
            get { return eTime; }
            set { eTime = value; }
        }

        public string Duration
        {
            get { return duration; }
            set { duration = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private string sTime;
        private string eTime;
        private string duration;
        private string status;
        private string description;
    }
}
