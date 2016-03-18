using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;

namespace Day_Logger
{
    /// <summary>
    /// Functions for calculating TimeStamps and getting files.
    /// </summary>
    public class TimestampFunctions
    {
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
            string[] startSpl = start.Split(new char[] { ':' });
            string[] endSpl = end.Split(new char[] { ':' });

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
            if(!bh1 || !bh2 || !bm1 || !bm2)
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

        /// <summary>
        /// This function is used to get the path to a file to save.
        /// </summary>
        /// <returns type="string">The path to the file.</returns>
        public static string GetSaveFile()
        {
            // Create the SaveFileDialog that we show the user.
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "New Document";
            dlg.DefaultExt = ".dlog";
            dlg.Filter = "Day Log (.dlog)|*.dlog";

            // Show the user the dialog and check if they selected a file.
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.FileName;
            }

            // If the user didn't select a file, return an empty string.
            return String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns type="string"></returns>
        public static string GetSaveAsFile()
        {

            return String.Empty;
        }

        /// <summary>
        /// This function is used to get the path to a file to open.
        /// </summary>
        /// <returns type="string">The path to the file.</returns>
        public static string OpenFile()
        {
            // Create the OpenFileDialog to show the user.
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".dlog";
            dlg.Filter = "Day Log (.dlog)|*.dlog";

            // Show the user the dialog and check if they selected a file to open.
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.FileName;
            }
            
            // If the user didn't select a file, return an empty string.
            return String.Empty;
        }

        /// <summary>
        /// This function is used to build the description string.
        /// </summary>
        /// <param name="callType">The call type.</param>
        /// <param name="cusType">The customer type.</param>
        /// <param name="refNum">The reference number.</param>
        /// <returns type="string">The description string.</returns>
        public static string GetDesString(string callType, string cusType, string refNum)
        {
            // Make sure the callType is valie (cusType and refNum can be null/empty).
            if (callType == String.Empty)
                return String.Empty;

            // Build the return string.
            string retString = callType 
                + ((cusType == String.Empty) ? String.Empty : " - " + cusType) 
                + ((refNum == String.Empty) ? String.Empty : " - " + refNum);

            // Return the result.
            return retString;
        }
    }
}
