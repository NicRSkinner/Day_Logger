using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Day_Logger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Initializers
        /// <summary>
        /// Default initializer for the main window.
        /// </summary>
        public MainWindow()
        {
            InitializeWindow();
        }

        /// <summary>
        /// Initializer for opening a new file with a given filepath.
        /// </summary>
        /// <param name="filePath">The file path to get the information from.</param>
        public MainWindow(string filePath)
        {
            InitializeWindow();
            FilePath = filePath;

            // Get the stream to initialize the window with.
            StreamReader sRead = new StreamReader(filePath);

            // Position the reader to the second line, as the first is only for the user.
            sRead.ReadLine();

            while (!sRead.EndOfStream)
            {
                string input = sRead.ReadLine();

                string[] res = input.Split(',');

                this.lvStamps.Items.Add(new TimeStamp()
                {
                    STime = res[0],
                    ETime = res[1],
                    Duration = res[2],
                    Status = res[3],
                    Description = res[4]
                });
            }
        }

        /// <summary>
        /// This method is used to initialize the window.
        /// </summary>
        private void InitializeWindow()
        {
            InitializeComponent();

            txtEndTime.Text = DateTime.Now.ToString("HH:mm");

            DispatcherTimer dispTimer = new DispatcherTimer();
            dispTimer.Tick += new EventHandler(dispTimer_Tick);
            dispTimer.Interval = new TimeSpan(0, 0, 10);
            dispTimer.Start();

            AddHandler(Keyboard.KeyDownEvent, (System.Windows.Input.KeyEventHandler)HandleSaveKeyEvent);
        }
        #endregion
        #region Button Events
        /// <summary>
        /// Handles the event for adding timestamps to the ListView.
        /// </summary>
        /// <param name="sender">The Event sender</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void btnAddStamp_Click(object sender, RoutedEventArgs e)
        {
            TimeStamp addition = new TimeStamp();

            addition.STime = txtStartTime.Text;
            addition.ETime = txtEndTime.Text;
            addition.Status = cStatusBox.SelectionBoxItem.ToString();
            addition.Description = TimestampFunctions.GetDesString(cCallTypeBox.SelectionBoxItem.ToString(), 
                                                                   cCustomerTypeBox.SelectionBoxItem.ToString(),
                                                                   txtReferenceNumber.Text);

            try
            {
                addition.Duration = TimestampFunctions.CalculateDuration(txtStartTime.Text, txtEndTime.Text);

                this.lvStamps.Items.Add(addition);

                txtStartTime.Text = txtEndTime.Text;
            }
            catch
            {
                System.Windows.MessageBox.Show("FORMATTING FOR TIMESTAMP INCORRECT, PLEASE USE HH:MM");
            }

            this.changed = true;
        }

        /// <summary>
        /// Handles the event for removing timestamps from the ListView.
        /// </summary>
        /// <param name="sender">The Event sender</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void btnRemoveStamp_Click(object sender, RoutedEventArgs e)
        {
            List<TimeStamp> rList = new List<TimeStamp>();

            foreach (TimeStamp tStamp in this.lvStamps.SelectedItems)
            {
                rList.Add(tStamp);
            }

            for (int i = 0; i < rList.Count; ++i)
                lvStamps.Items.Remove(rList[i]);

            this.changed = true;
        }
        #endregion
        #region Menu Events
        /// <summary>
        /// Handle the OnNew event for creating a new document.
        /// </summary>
        /// <param name="sender">The Event sender.</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void OnNew_Click(object sender, RoutedEventArgs e)
        {
            if (changed == true)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Would you like to save the current changes?", "Confirm Changes",
                                MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                    OnSave_Click(this, new RoutedEventArgs());
            }

            FilePath = String.Empty;

            List<TimeStamp> rList = new List<TimeStamp>();

            foreach (TimeStamp tStamp in lvStamps.Items)
            {
                rList.Add(tStamp);
            }

            for (int i = 0; i < rList.Count; ++i)
                lvStamps.Items.Remove(rList[i]);

            txtStartTime.Text = String.Empty;
            changed = false;
        }

        /// <summary>
        /// Handle the OnOpen event for saving the document.
        /// </summary>
        /// <param name="sender">The Event sender.</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void OnOpen_Click(object sender, RoutedEventArgs e)
        {
            // Get the file path for the file to open.
            string filePath = TimestampFunctions.OpenFile();

            // Make sure that a file was selected.
            if (filePath != String.Empty)
            {
                // Create the new window with the StreamReader as the input.
                MainWindow mWin = new MainWindow(filePath);

                // Show the new window.
                mWin.Show();
            }
        }

        /// <summary>
        /// Handle the OnSave event for saving the document.
        /// </summary>
        /// <param name="sender">The Event sender.</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void OnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sString = new StringBuilder("Start Time, End Time, Duration, Status\r\n");

            foreach (TimeStamp tStamp in lvStamps.Items)
            {
                sString.AppendLine(tStamp.STime + "," + tStamp.ETime + "," + tStamp.Duration + "," + tStamp.Status + "," + tStamp.Description);
            }

            // Check to see if FilePath has been determined previously.
            if (FilePath == null || FilePath == String.Empty)
            {
                // Create FilePath if it was not aready determined.
                FilePath = TimestampFunctions.GetSaveFile();
            }

            // Check FilePath to see if the user hit "cancel"
            if (FilePath != String.Empty)
            {
                File.WriteAllText(FilePath, sString.ToString());
                changed = false;
            }
        }

        /// <summary>
        /// Handle the Save As event for saving the document.
        /// </summary>
        /// <param name="sender">The Event sender.</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void OnSaveAs_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Handle the OnExit event for closing the document.
        /// </summary>
        /// <param name="sender">The Event sender.</param>
        /// <param name="e">The RoutedEventArgs.</param>
        private void OnExit_Click(object sender, RoutedEventArgs e)
        {
            if(changed == true)
            {
                MessageBoxResult result = MessageBox.Show("Would you like to save your file before closing?", "Confirm Changes",
                                MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                    OnSave_Click(this, new RoutedEventArgs());
                else if (result == MessageBoxResult.Cancel)
                    return;
            }

            Application.Current.Shutdown();
        }
        #endregion
        #region Key Events
        /// <summary>
        /// Handles the CTRL + S keyboard macro to save documents.
        /// </summary>
        /// <param name="sender">The Event sender</param>
        /// <param name="e">The KeyEventArgs for key input</param>
        private void HandleSaveKeyEvent(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.S:
                    if (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl))
                        OnSave_Click(this, new RoutedEventArgs());
                    break;
                case Key.Delete:
                    btnRemoveStamp_Click(this, new RoutedEventArgs());
                    break;
            }
        }
        #endregion
        #region ListView Events
        private void OnText_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.IsReadOnly = false;
            textBox.SelectAll();
            textBox.Background = new SolidColorBrush(Colors.White);
        }

        private void OnText_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.IsReadOnly = true;
            textBox.Background = new SolidColorBrush(Colors.WhiteSmoke);
        }
        #endregion
        #region Timer Events
        /// <summary>
        /// The event handler for the tick event, which goes over once a minute.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispTimer_Tick(object sender, EventArgs e)
        {
            // Update the end time textbox with the current time.
            txtEndTime.Text = DateTime.Now.ToString("HH:mm");

            // Force the CommandManager to raise the RequerySuggest event.
            CommandManager.InvalidateRequerySuggested();
        }
        #endregion
        #region Variables
        private string FilePath;
        private bool changed = false;
        #endregion
    }
}