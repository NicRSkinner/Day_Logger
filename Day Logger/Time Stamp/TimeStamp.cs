using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Day_Logger.TimeStamps
{
    /// <summary>
    /// Holds information about a given for timestamps.
    /// </summary>
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
                return CalculateDuration(STime, ETime);
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
        #region Methods
        /// <summary>
        /// This function is used to caluculate the difference between two timestamps
        /// </summary>
        /// <param name="start">The starting time FORMAT: HH:MM</param>
        /// <param name="end">The ending time FORMAT: HH:MM</param>
        /// <returns type="string">The duration between the start and ending times FORMAT: HH:MM
        ///          NOTE: WILL RETURN ##:## IF FORMAT IS INVALID.</returns>
        public static string CalculateDuration(string start, string end)
        {
            // Make sure that the input is valid!
            if (String.IsNullOrEmpty(start) || String.IsNullOrEmpty(end))
                return "##:##";

            // Temporary variables for storing the result time.
            int hr = 0;
            int min = 0;

            // Split the times so that we can subtract the times.
            string[] startSpl = start.Split(':');
            string[] endSpl = end.Split(':');

            // Temporary variables for attempting to parse the time.
            int hr1, hr2, min1, min2;
            bool bh1, bh2, bm1, bm2;

            // Attempt to parse the hours.
            bh1 = Int32.TryParse(startSpl[0], out hr1);
            bh2 = Int32.TryParse(endSpl[0], out hr2);

            // Attempt to parse the minutes.
            bm1 = Int32.TryParse(startSpl[1], out min1);
            bm2 = Int32.TryParse(endSpl[1], out min2);

            // Check if values were parsed correctly.
            if (!bh1 || !bh2 || !bm1 || !bm2)
                return "##:##";

            // Subtract the times.
            hr = hr2 - hr1;
            min = min2 - min1;

            // If the minutes go below zero, we need to subtract an hour.
            if (min < 0)
            {
                hr -= 1;
                min += 60;
            }

            // Put the result together and return it.
            return (hr + ":" + (min == 0 ? "00" : (min < 10) ? "0" + Convert.ToString(min) : Convert.ToString(min)));
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

    /// <summary>
    /// The TimeStampCollection that is used to store timestamps for the DataGrid.
    /// </summary>
    public class TimeStampCollection : ObservableCollection<TimeStamp>
    {
        public TimeStampCollection()
        {
        }

        /// <summary>
        /// Adds a stamp to the collection given individual parameters.
        /// </summary>
        /// <param name="sTime">The Start time.</param>
        /// <param name="eTime">The End time.</param>
        /// <param name="sta">The Status.</param>
        /// <param name="des">The Description.</param>
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

        /// <summary>
        /// Adds a stamp to the collection.
        /// </summary>
        /// <param name="stamp">The stamp to be added.</param>
        public void AddStamp(TimeStamp stamp)
        {
            this.Add(stamp);
        }
    }
}
