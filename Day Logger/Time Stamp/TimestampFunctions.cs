using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Day_Logger
{
    /// <summary>
    /// Functions for calculating TimeStamp durations and descriptions.
    /// </summary>
    public class TimestampFunctions
    {
        /// <summary>
        /// This function calculates the average
        /// </summary>
        /// <param name="stamps">The list of times to average. FORMAT: HH:MM</param>
        /// <returns type="string">The average duration of the times.
        ///                         NOTE: WILL RETURN ##:## IF FORMAT IS INVALID.</returns>
        public static string CalculateAverageDuration(List<string> stamps)
        {
            // Validity check
            if (stamps == null || stamps.Count == 0)
                return "##:##";

            string durTotal = CalculateTotalDuration(stamps);

            // Validity check
            if (durTotal == "##:##")
                return "##:##";

            string[] dur = durTotal.Split(':');

            // Convert the total time into minutes.
            int totalMin = (Int32.Parse(dur[0]) * 60) + Int32.Parse(dur[1]);

            // Find the average minutes.
            int avgMin = totalMin / stamps.Count;

            // Convert back into hours and minutes.
            int hrAverage = avgMin / 60;
            int minAverage = avgMin % 60;

            return (hrAverage.ToString() + ':' + (minAverage < 10 ? '0' + minAverage.ToString() : minAverage.ToString()));
        }

        /// <summary>
        /// This function is used to calculate the total duration of a list of times.
        /// </summary>
        /// <param name="stamps">The durations to get the total from.</param>
        /// <returns type="string">The total duration from the given stamps.
        ///                         NOTE: WILL RETURN ##:## IF FORMAT IS INVALID</returns>
        public static string CalculateTotalDuration(List<string> stamps)
        {
            // Validity check.
            if (stamps == null || stamps.Count == 0)
                return "##:##";

            int hrTotal = 0;
            int minTotal = 0;

            foreach(string s in stamps)
            {
                int hr;
                int min;

                string[] sSpl = s.Split(':');

                if (!Int32.TryParse(sSpl[0], out hr) || !Int32.TryParse(sSpl[1], out min))
                    return "##:##";

                hrTotal += hr;
                minTotal += min;
            }

            // Add any overflow minutes to the hour.
            hrTotal += minTotal / 60;
            minTotal %= 60;

            return hrTotal.ToString() + ':' + (minTotal < 10 ? "0" + minTotal.ToString() : minTotal.ToString());
        }

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
