using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Day_Logger.TimeStamps;

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

    /// <summary>
    /// Class used for opening/saving the formatted information.
    /// </summary>
    public class StampFile
    {
        #region Initializers
        /// <summary>>
        /// Default Initializer
        /// </summary>
        public StampFile()
        {
            FilePath = String.Empty;
            Initialize();
        }

        /// <summary>
        /// Initialize the StampFile with a file path.
        /// </summary>
        /// <param name="filePath"></param>
        public StampFile(string filePath)
        {
            FilePath = filePath;
            Initialize();
        }

        /// <summary>
        /// Initializes the instance fields.
        /// </summary>
        private void Initialize()
        {
            Header = new List<string>();
            Stamps = new List<TimeStamp>();
        }
        #endregion
        #region Saving
        /// <summary>
        /// Save the current working file.
        /// </summary>
        public void Save()
        {
            if (String.IsNullOrEmpty(FilePath))
            {
                FilePath = GetSaveFile();

                if (String.IsNullOrEmpty(FilePath))
                    return;

                SetFormat();
            }

            switch (FileFormat)
            {
                case FileFormatting.CSV:
                    SaveFile(AsCSV());
                    break;
                case FileFormatting.DLOG:
                default:
                    SaveFile(AsDlog());
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        private void SaveFile(string info)
        {
            File.WriteAllText(FilePath, info);  //  TODO: Verify that the file path still exists.
        }

        /// <summary>
        /// Ask the user to select the file path for saving.
        /// </summary>
        /// <returns></returns>
        private string GetSaveFile()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.Filter = "Day Log (.dlog)|*.dlog|CSV (.csv)|*.csv";

            if (dlg.ShowDialog() == DialogResult.OK)
                return dlg.FileName;

            return String.Empty;
        }
        #endregion
        #region Opening
        /// <summary>
        /// Open a file with the current FilePath
        /// </summary>
        public void Open()
        {
            // Ask the user to specify the new file path.
            string newPath = GetOpenFile();

            // Check to see if the user cancelled.
            if (String.IsNullOrEmpty(newPath))
                return;

            FilePath = newPath;
            SetFormat();

            // Empty the save information.
            Header.Clear();
            Stamps.Clear();

            OpenFile();
        }

        /// <summary>
        /// Opens the information from the given file path and
        ///     stores it in the stamps collection.
        /// </summary>
        /// <NOTE>Format is all saved the same,
        ///         add different formatting if this changes.</NOTE>
        private void OpenFile()
        {
            try
            {
                using (StreamReader sRead = new StreamReader(FilePath))
                {
                    string[] header = sRead.ReadLine().Split(',');

                    foreach (string s in header)
                    {
                        if (!String.IsNullOrEmpty(s))
                            Header.Add(s);
                    }

                    while (!sRead.EndOfStream)
                    {
                        string[] stampInfo = sRead.ReadLine().Split(',');

                        // Create a new Timestamp to add to the save file.
                        // NOTE: I skipped stampInfo[2] since that is the duration.
                        TimeStamp newStamp = new TimeStamp(stampInfo[0], stampInfo[1],
                                                            stampInfo[3], stampInfo[4]);

                        Stamps.Add(newStamp);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Gets the file path from the user using OpenFileDialog.
        /// </summary>
        /// <returns>The file path the user selects.
        ///             NOTE: Returns an empty string if the user cancels.</returns>
        private string GetOpenFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.AddExtension = true;
            dlg.Filter = "Day Log (.dlog)|*.dlog|CSV (.csv)|*.csv";

            if (dlg.ShowDialog() == DialogResult.OK)
                return dlg.FileName;

            return String.Empty;
        }
        #endregion
        #region File Formatting
        /// <summary>
        /// Formats the file information into a DLOG format.
        /// </summary>
        /// <returns>The formatted file information.</returns>
        private string AsDlog()
        {
            return AsCSV();  //  Change this when the DLOG format changes.
        }

        /// <summary>
        /// Formats the file information into a CSV format.
        /// </summary>
        /// <returns>The formatted file information.</returns>
        private string AsCSV()
        {
            StringBuilder sString = new StringBuilder();

            foreach (string s in Header)
                sString.Append(s + ',');

            sString.Append("\r\n");

            foreach (TimeStamp stamp in Stamps)
                sString.AppendLine(stamp.STime + ',' + stamp.ETime + ',' + stamp.Duration + ','
                                    + stamp.Status + ',' + stamp.Description + ',');

            return sString.ToString();
        }

        /// <summary>
        /// Sets the format for the current working file.
        /// </summary>
        private void SetFormat()
        {
            switch (Path.GetExtension(FilePath).ToLower())
            {
                case ".csv":
                    FileFormat = FileFormatting.CSV;
                    break;

                case ".txt":
                    FileFormat = FileFormatting.TXT;
                    break;

                case ".dlog":
                default:
                    FileFormat = FileFormatting.DLOG;
                    break;
            }
        }
        #endregion
        #region Adding/Removing Information
        /// <summary>
        /// Adds a header from a delimited header string.
        /// </summary>
        /// <param name="header">The delimited string.</param>
        /// <param name="delim">The delimiter to split the string by.</param>
        public void AddHeader(string header, char[] delim)
        {
            if (String.IsNullOrEmpty(header))
                throw new NullReferenceException("The header cannot be null or empty.");

            string[] hSplit = header.Split(delim);

            foreach (string s in hSplit)
            {
                Header.Add(s);
            }
        }

        /// <summary>
        /// Adds a TimeStamp to the current save information.
        /// </summary>
        /// <param name="stamp">The stamp to add.</param>
        public void AddStamp(TimeStamp stamp)
        {
            if (stamp == null)
                throw new NullReferenceException("The stamp cannot be null.");

            Stamps.Add(stamp);
        }

        /// <summary>
        /// Removes a stamp from the current save information.
        /// </summary>
        /// <param name="stamp">The stamp to remove.</param>
        public void RemoveStamp(TimeStamp stamp)
        {
            if (stamp == null)
                throw new NullReferenceException("The stamp cannot be null.");

            foreach (TimeStamp s in Stamps)
            {
                if (s == stamp)
                    Stamps.Remove(s);
            }
        }
        #endregion
        #region Instance Variables
        public string FilePath { get; private set; }
        private FileFormatting FileFormat { get; set; }
        private List<string> Header { get; set; }
        private List<TimeStamp> Stamps { get; set; }
        #endregion
    }

    public enum FileFormatting { CSV, DLOG, TXT };
}