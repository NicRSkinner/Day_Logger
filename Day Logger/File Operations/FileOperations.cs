using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Day_Logger.File_Operations
{
    public class FileOperations
    {
        #region File
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
                return dlg.FileName;

            // If the user didn't select a file, return an empty string.
            return String.Empty;
        }

        /// <summary>
        /// This function is used to get a save file with a different extension
        ///     then the default "dlog"
        /// </summary>
        /// <returns type="string">The path to the file.</returns>
        public static string GetSaveAsFile()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.Filter = "Day Log (.dlog)|*.dlog|CSV (.csv)|*.csv";

            if (dlg.ShowDialog() == DialogResult.OK)
                return dlg.FileName;

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
                return dlg.FileName;

            // If the user didn't select a file, return an empty string.
            return String.Empty;
        }

        public static void SaveFile(string fileExt, string filePath, string info)
        {
            if (String.IsNullOrEmpty(fileExt) || String.IsNullOrEmpty(filePath) || String.IsNullOrEmpty(info))
                return;

            if (fileExt.StartsWith("."))
                fileExt = fileExt.Remove(0, 1);

            switch (fileExt)
            {
                case "dlog":
                    break;
                case "csv":
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
