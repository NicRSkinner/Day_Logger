using Day_Logger.File_Operations;
using Day_Logger.TimeStamps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            InitializeConfig();
        }

        /// <summary>
        /// This method is used to initialize the window.
        /// </summary>
        private void InitializeWindow()
        {
            InitializeComponent();
            changeHandler = new DataGridChangeHandler();
            StmpSave = new StampFile();
            stampColl = (Resources["StmpDs"] as TimeStampCollection);

            txtEndTime.Text = DateTime.Now.ToString("HH:mm");

            DispatcherTimer dispTimer = new DispatcherTimer();
            dispTimer.Tick += new EventHandler(dispTimer_Tick);
            dispTimer.Interval = new TimeSpan(0, 0, 10);
            dispTimer.Start();

            AddHandler(Keyboard.KeyDownEvent, (System.Windows.Input.KeyEventHandler)HandleKeyEvent);

            this.dgStamps.CellEditEnding += dgStamps_CellEditEnding;
            this.dgStamps.PreviewKeyDown += HandleKeyEvent;

            changeHandler.Changed += new DataGridChangeHandler.ChangedEventHandler(HandleChangedEvent);
            StmpSave.AddHeader("Start Time,End Time,Duration,Status,Description", new char[] { ',' });
        }
        #endregion
        #region Window Functions
        /// <summary>
        /// This function is used to initialize the combo boxes from the applicaton
        ///     configuration file.
        /// </summary>
        private void InitializeConfig()
        {
            cAverageSta.Items.Add("All");

            foreach (string s in ConfigOperations.GetConfig("Status"))
            {
                cStatusBox.Items.Add(s);
                cAverageSta.Items.Add(s);
            }

            foreach (string s in ConfigOperations.GetConfig("CallType"))
                cCallTypeBox.Items.Add(s);

            foreach (string s in ConfigOperations.GetConfig("CustomerType"))
                cCustomerTypeBox.Items.Add(s);

            cStatusBox.Items.Add("");
            cCallTypeBox.Items.Add("");
            cCustomerTypeBox.Items.Add("");
        }

        /// <summary>
        /// Handles the event that the program changed.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The RoutedEventArgs.</param>
        private void HandleChangedEvent(object sender, RoutedEventArgs e)
        {
            // Make sure the asterisk was not already added for the change.
            if (!this.Title.EndsWith("*"))
                this.Title += "*";
        }
        #endregion
        #region Averages
        /// <summary>
        /// This function recalculates the averages when the ComboBox selection changes.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The RoutedEventArgs.</param>
        private void cAverageSta_SelectionChanged(object sender, RoutedEventArgs e)
        {
            List<string> matchedStamps = new List<string>();

            string currSelected = cAverageSta.SelectedValue.ToString();

            var sItems = from TimeStamp stamp in dgStamps.Items
                         where String.Compare(stamp.Status.ToString(), currSelected) == 0
                         select stamp;

            foreach(TimeStamp stamp in sItems)
            {

            }

            lAverageDur.Content = TimestampFunctions.CalculateAverageDuration(matchedStamps);
            lTotalDur.Content = TimestampFunctions.CalculateTotalDuration(matchedStamps);
        }
        #endregion
        #region DataGrid
        /// <summary>
        /// This function responds to the CellEditEnding event to update the DataGrid columns.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The cell that was changed.</param>
        private void dgStamps_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var classInstance = e.EditingElement.DataContext;
            string newValue = (e.EditingElement as TextBox).Text;

            TimeStamp stamp = (e.Row.Item as TimeStamp);

            string oldValue;
            Change cellChange = new Change();

            switch(e.Column.Header.ToString())
            {
                case "Start Time":
                    oldValue = stamp.STime;
                    stamp.STime = newValue;
                    cellChange.UndoFunc = () => stamp.STime = oldValue;
                    cellChange.RedoFunc = () => stamp.STime = newValue;
                    break;
                case "End Time":
                    oldValue = stamp.ETime;
                    stamp.ETime = newValue;
                    cellChange.UndoFunc = () => stamp.ETime = oldValue;
                    cellChange.RedoFunc = () => stamp.ETime = newValue;
                    break;
                case "Status":
                    oldValue = stamp.Status;
                    stamp.Status = newValue;
                    cellChange.UndoFunc = () => stamp.Status = oldValue;
                    cellChange.RedoFunc = () => stamp.Status = newValue;
                    break;
                case "Description":
                    oldValue = stamp.Description;
                    stamp.Description = newValue;
                    cellChange.UndoFunc = () => stamp.Description = oldValue;
                    cellChange.RedoFunc = () => stamp.Description = newValue;
                    break;
                default:
                    return;
            }

            changeHandler.AddChange(cellChange);
        }
        #endregion
        #region Input
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

            stampColl.AddStamp(addition);
            StmpSave.AddStamp(addition);

            cStatusBox.Text = "";
            cCallTypeBox.Text = "";
            cCustomerTypeBox.Text = "";
            txtReferenceNumber.Text = "";

            txtStartTime.Text = txtEndTime.Text;

            changeHandler.AddChange(() => { stampColl.RemoveAt(stampColl.Count - 1); },
                                    () => { stampColl.Insert(stampColl.Count, addition); });
        }

        /// <summary>
        /// Handles the event for removing timestamps from the ListView.
        /// </summary>
        /// <param name="sender">The Event sender</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void btnRemoveStamp_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<int, TimeStamp> stmpList = new Dictionary<int, TimeStamp>();

            // Make sure something is selected before continuing.
            if (dgStamps.SelectedItems == null)
                return;

            try
            {
                // Find all stamps that are selected and store them so that we can
                // remove them later.
                foreach (TimeStamp tStamp in dgStamps.SelectedItems)
                {
                    stmpList.Add(dgStamps.Items.IndexOf(tStamp), tStamp);
                }
            }
            catch
            {
                MessageBox.Show("Cannot delete that item", "Error");
            }

            // Loop through the selected stamps and remove them.
            foreach (TimeStamp tStamp in stmpList.Values)
            {
                stampColl.Remove(tStamp);
                StmpSave.RemoveStamp(tStamp);
            }

            // Add the change so that it can be undone/redone later.
            changeHandler.AddChange(() =>
            {
                foreach (var stmp in stmpList.OrderBy(i => i.Key))
                {
                    stampColl.Insert(stmp.Key, stmp.Value);
                }
            }, () =>
            {
                foreach (var stmp in stmpList.OrderByDescending(i => i.Key))
                {
                    stampColl.RemoveAt(stmp.Key);
                }
            });
        }

        /// <summary>
        /// Handle the OnNew event for creating a new document.
        /// </summary>
        /// <param name="sender">The Event sender.</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void OnNew_Click(object sender, RoutedEventArgs e)
        {
            if (changeHandler.HasChanged == true)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Would you like to save the current changes?", "Confirm Changes",
                                MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                    StmpSave.Save();
                else if (result == MessageBoxResult.Cancel)
                    return;
            }

            stampColl.Clear();
            StmpSave.Stamps.Clear();

            txtStartTime.Text = String.Empty;
            this.Title = "New Document - Day Logger";
            changeHandler.HasChanged = false;
        }

        /// <summary>
        /// Handle the OnOpen event for saving the document.
        /// </summary>
        /// <param name="sender">The Event sender.</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void OnOpen_Click(object sender, RoutedEventArgs e)
        {
            if (changeHandler.HasChanged == true)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Would you like to save the current changes?", "Confirm Changes",
                                MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                    StmpSave.Save();
                else if (result == MessageBoxResult.Cancel)
                    return;
            }

            stampColl.Clear();
            StmpSave.Clear();

            StmpSave.Open();

            foreach (TimeStamp stamp in StmpSave.Stamps)
            {
                stampColl.AddStamp(stamp);
            }

            // Set the window title.
            this.Title = System.IO.Path.GetFileName(StmpSave.FilePath) + " - Day Logger";
            changeHandler.HasChanged = false;
        }

        /// <summary>
        /// Handle the OnSave event for saving the document.
        /// </summary>
        /// <param name="sender">The Event sender.</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void OnSave_Click(object sender, RoutedEventArgs e)
        {
            if (StmpSave.Save())
            {
                changeHandler.HasChanged = false;

                // Set the window title.
                this.Title = System.IO.Path.GetFileName(StmpSave.FilePath) + " - Day Logger";
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
            if(changeHandler.HasChanged == true)
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

        private void OnUndo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OnRedo_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Handles the keyboard macros for interacting with the main window.
        /// </summary>
        /// <param name="sender">The Event sender</param>
        /// <param name="e">The KeyEventArgs for key input</param>
        private void HandleKeyEvent(object sender, KeyEventArgs e)
        {
            if ((Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl)))
            {
                switch (e.Key)
                {
                    case Key.E:
                        btnRemoveStamp_Click(this, new RoutedEventArgs());
                        e.Handled = true;
                        break;
                    case Key.Q:
                        btnAddStamp_Click(this, new RoutedEventArgs());
                        e.Handled = true;
                        break;
                    case Key.S:
                        OnSave_Click(this, new RoutedEventArgs());
                        e.Handled = true;
                        break;
                    case Key.Y:
                        changeHandler.Redo();
                        e.Handled = true;
                        break;
                    case Key.Z:
                        changeHandler.Undo();
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Delete:
                        btnRemoveStamp_Click(this, new RoutedEventArgs());
                        e.Handled = true;
                        break;
                }
            }
        }
        #endregion
        #region Ticks
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
        #region Instance Fields
        private StampFile StmpSave;
        private DataGridChangeHandler changeHandler;
        private TimeStampCollection stampColl;
        #endregion
    }
}