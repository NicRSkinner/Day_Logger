using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Day_Logger
{
    public class TimeStamp
    {
        #region Variables
        public string STime { get; set; }
        public string ETime { get; set; }
        public string Duration { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        #endregion
    }

    public class TimeStampCollection : ObservableCollection<TimeStamp>
    {
        public TimeStampCollection()
        {
        }

        public void AddStamp(string sTime, string eTime, string dur, string sta, string des)
        {
            this.Add(new TimeStamp()
            {
                STime = sTime,
                ETime = eTime,
                Duration = dur,
                Status = sta,
                Description = des
            });
        }

        public void AddStamp(TimeStamp stamp)
        {
            this.Add(stamp);
        }
    }
}
