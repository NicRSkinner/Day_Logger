using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Day_Logger
{
    public class TimeStamp : INotifyPropertyChanged
    {
        #region Initializers
        public TimeStamp()
        {

        }

        public TimeStamp(string sTime, string eTime, string sta, string des)
        {
            this.sTime = sTime;
            this.eTime = eTime;
            this.status = sta;
            this.description = des;
        }
        #endregion
        #region Accessors
        public string STime
        {
            get { return sTime; } 
            set
            {
                sTime = value;
                OnPropertyChanged("STime");
                OnPropertyChanged("Duration"); // Force Duration to be updated.
            }
        }
        
        public string ETime 
        {
            get { return eTime; }
            set
            {
                eTime = value;
                OnPropertyChanged("ETime");
                OnPropertyChanged("Duration"); // Force Duration to be updated.
            }
        }
        
        public string Duration 
        {
            get
            {
                return TimestampFunctions.CalculateDuration(STime, ETime);
            }
        }
        public string Status 
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        public string Description 
        {
            get { return description; }
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }
        #endregion
        #region Variables
        private string sTime;
        private string eTime;
        private string status;
        private string description;
        #endregion
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This function raises the PropertyChanged event to signal
        ///     to any listeners that the property value was changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that was changed.</param>
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

    public class TimeStampCollection : ObservableCollection<TimeStamp>
    {
        public TimeStampCollection()
        {
        }

        public void AddStamp(string sTime, string eTime, string sta, string des)
        {
            this.Add(new TimeStamp()
            {
                STime = sTime,
                ETime = eTime,
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
