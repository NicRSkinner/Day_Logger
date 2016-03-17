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
        /// <returns type="string">The duration between the start and ending times FORMAT: HH:MM
        ///          NOTE: WILL RETURN ##:## IF FORMAT IS INVALID.</returns>
        public static string CalculateDuration(string start, string end)
        {
            if(String.IsNullOrEmpty(start) || String.IsNullOrEmpty(end))
            {
                return "##:##";
            }

            int hr = 0;
            int min = 0;

            string[] startSpl = start.Split(new char[] { ':' });
            string[] endSpl = end.Split(new char[] { ':' });

            int hr1, hr2, min1, min2;
            bool bh1, bh2, bm1, bm2;

            bh1 = Int32.TryParse(startSpl[0], out hr1);
            bh2 = Int32.TryParse(endSpl[0], out hr2);

            bm1 = Int32.TryParse(startSpl[1], out min1);
            bm2 = Int32.TryParse(endSpl[1], out min2);
            
            if(!bh1 || !bh2 || !bm1 || !bm2)
                return "##:##";

            hr = hr2 - hr1;
            min = min2 - min1;

            if (min < 0)
            {
                hr -= 1;
                min += 60;
            }

             return (hr + ":" + (min == 0 ? "00" : (min < 10) ? "0" + Convert.ToString(min) : Convert.ToString(min)));
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
