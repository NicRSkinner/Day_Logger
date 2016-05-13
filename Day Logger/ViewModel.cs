using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Day_Logger.TimeStamps;
using System.Collections.ObjectModel;

namespace Day_Logger
{
    /// <summary>
    /// The ViewModel to be used for UI interaction.
    /// </summary>
    public class ViewModel
    {
        public ViewModel()
        {
            this.Stamps = new ObservableCollection<TimeStamp>();
        }

        public ObservableCollection<TimeStamp> Stamps { get; set; }
    }
}
