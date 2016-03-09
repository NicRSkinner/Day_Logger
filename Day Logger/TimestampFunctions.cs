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
    public class TimestampFunctions
    {
        /// <summary>
        /// This function is used to caluculate the difference between two timestamps
        /// </summary>
        /// <param name="start">The starting time FORMAT: HH:MM</param>
        /// <param name="end">The ending time FORMAT: HH:MM</param>
        /// <returns>The duration between the start and ending times FORMAT: HH:MM</returns>
        public static string CalculateDuration(string start, string end)
        {
            int hr = 0;
            int min = 0;

            try
            {
                string[] startSpl = start.Split(new char[] { ':' });
                string[] endSpl = end.Split(new char[] { ':' });

                hr = Convert.ToInt32(endSpl[0]) - Convert.ToInt32(startSpl[0]);
                min = Convert.ToInt32(endSpl[1]) - Convert.ToInt32(startSpl[1]);

                if (min < 0)
                {
                    hr -= 1;
                    min += 60;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            string result = hr + ":" + (min == 0 ? "00" : (min < 10) ? "0" + Convert.ToString(min) : Convert.ToString(min));

            return result;
        }

        /// <summary>
        /// This function is used to get the path to a file to save.
        /// </summary>
        /// <returns type="string">The path to the file.</returns>
        public static string GetSaveFile()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "New Document";
            dlg.DefaultExt = ".dlog";
            dlg.Filter = "Day Log (.dlog)|*.dlog";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.FileName;
            }

            return String.Empty;
        }

        /// <summary>
        /// This function is used to get the path to a file to open.
        /// </summary>
        /// <returns type="string">The path to the file.</returns>
        public static string OpenFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".dlog";
            dlg.Filter = "Day Log (.dlog)|*.dlog";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                return dlg.FileName;
            }
            else
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
            if (callType == String.Empty)
                return String.Empty;

            string retString = callType 
                + ((cusType == String.Empty) ? String.Empty : " - " + cusType) 
                + ((refNum == String.Empty) ? String.Empty : " - " + refNum);

            return retString;
        }
    }
}
