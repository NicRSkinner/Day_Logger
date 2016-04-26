using Day_Logger.File_Operations;
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
using Day_Logger.TimeStamps;

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

            txtEndTime.Text = DateTime.Now.ToString("HH:mm");

            DispatcherTimer dispTimer = new DispatcherTimer();
            dispTimer.Tick += new EventHandler(dispTimer_Tick);
            dispTimer.Interval = new TimeSpan(0, 0, 10);
            dispTimer.Start();

            AddHandler(Keyboard.KeyDownEvent, (System.Windows.Input.KeyEventHandler)HandleKeyEvent);

            this.dgStamps.PreviewMouseLeftButtonDown +=
                new MouseButtonEventHandler(dgStamps_PreviewMouseLeftButtonDown);
            this.dgStamps.Drop += new DragEventHandler(dgStamps_Drop);
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

            foreach (string s in ConfigOperations.GetStatusConfig())
            {
                cStatusBox.Items.Add(s);
                cAverageSta.Items.Add(s);
            }

            foreach (string s in ConfigOperations.GetCallTypeConfig())
                cCallTypeBox.Items.Add(s);

            foreach (string s in ConfigOperations.GetCustomerTypeConfig())
                cCustomerTypeBox.Items.Add(s);

            cStatusBox.Items.Add("");
            cCallTypeBox.Items.Add("");
            cCustomerTypeBox.Items.Add("");
        }

        private void HandleChangedEvent(object sender, RoutedEventArgs e)
        {
            // Make sure the asterisk was not already added for the change.
            if (!this.Title.EndsWith("*"))
                this.Title += "*";

            this.changed = true;
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
            TimeStampCollection tStamps = (Resources["StmpDs"] as TimeStampCollection);

            List<string> matchedStamps = new List<string>();

            string currSelected = cAverageSta.SelectedValue.ToString();

            for(int i = 0; i < tStamps.Count; ++i)
            {
                TimeStamp currStamp = dgStamps.Items[i] as TimeStamp;

                if (currSelected == "All")
                    matchedStamps.Add(currStamp.Duration);
                else
                {
                    if (currStamp.Status == currSelected)
                        matchedStamps.Add(currStamp.Duration);
                }
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

        /// <summary>
        /// This function is used to verify that the mouse is on the
        ///     target row that will be moved.
        /// </summary>
        /// <param name="target">The Visual target row.</param>
        /// <param name="pos">The DragDropPosition of the mouse.</param>
        /// <returns type="bool">An indicator as to whether or not the mouse is on the target row.</returns>
        private bool IsMouseOnTargetRow(Visual target, GetDragDropPosition pos)
        {
            if (target == null)
                return false;

            Rect posBounds = VisualTreeHelper.GetDescendantBounds(target);
            Point mousePos = pos((IInputElement)target);
            return posBounds.Contains(mousePos);
        }

        /// <summary>
        /// This function is used to get the DataGridRow that will be
        ///     moved.
        /// </summary>
        /// <param name="index">The index for the row.</param>
        /// <returns type="DataGridRow">The DataGridRow if one is found, otherwise NULL</returns>
        private DataGridRow GetDataGridRowItem(int index)
        {
            if (dgStamps.ItemContainerGenerator.Status
                != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                return null;

            return dgStamps.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
        }

        /// <summary>
        /// This function is used to get the current row index
        ///     for the drag and drop position when the user hovers over one.
        /// </summary>
        /// <param name="pos">The DragDropPosition.</param>
        /// <returns type="int">The index for the current row item, -1 if no row is hovered over.</returns>
        private int GetDataGridItemCurrentRowIndex(GetDragDropPosition pos)
        {
            int curIndex = -1;
            for (int i = 0; i < dgStamps.Items.Count; i++)
            {
                DataGridRow itm = GetDataGridRowItem(i);
                if (IsMouseOnTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
        }

        /// <summary>
        /// This function is used to get the position of the mouse when hovering with a
        ///     row to move.
        /// </summary>
        /// <param name="sender">The Event sender.</param>
        /// <param name="e">The MouseButtonEventArgs.</param>
        private void dgStamps_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            prevRowIndex = GetDataGridItemCurrentRowIndex(e.GetPosition);

            if (prevRowIndex < 0)
                return;

            dgStamps.SelectedIndex = prevRowIndex;

            TimeStamp selectedStamp = dgStamps.Items[prevRowIndex] as TimeStamp;

            if (selectedStamp == null)
                return;

            DragDropEffects dragDropEffects = DragDropEffects.Move;

            if (DragDrop.DoDragDrop(dgStamps, selectedStamp, dragDropEffects) != DragDropEffects.None)
            {
                dgStamps.SelectedItem = selectedStamp;
            }
        }

        /// <summary>
        /// This function is used to move a timestamp in the DataGrid
        ///     when the user moves it to another row.
        /// </summary>
        /// <param name="sender">The Event sender.</param>
        /// <param name="e">The DragEventArgs.</param>
        private void dgStamps_Drop(object sender, DragEventArgs e)
        {
            if (prevRowIndex < 0)
                return;

            int index = this.GetDataGridItemCurrentRowIndex(e.GetPosition);

            if (index < 0 || index == prevRowIndex)
                return;

            if (index == dgStamps.Items.Count - 1)
            {
                MessageBox.Show("This row-index cannot be used for Drop Operations");
                return;
            }

            TimeStampCollection stamp = Resources["StmpDs"] as TimeStampCollection;

            TimeStamp movedStmp = stamp[prevRowIndex];
            stamp.RemoveAt(prevRowIndex);
            stamp.Insert(index, movedStmp);

            int prevIndex = prevRowIndex;

            changeHandler.AddChange(() => { stamp.RemoveAt(index); stamp.Insert(prevIndex, movedStmp); },
                                    () => { stamp.RemoveAt(prevIndex); stamp.Insert(index, movedStmp); });
        }
        #endregion
        #region Buttons
        /// <summary>
        /// Handles the event for adding timestamps to the ListView.
        /// </summary>
        /// <param name="sender">The Event sender</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void btnAddStamp_Click(object sender, RoutedEventArgs e)
        {
            TimeStampCollection tS = (Resources["StmpDs"] as TimeStampCollection);

            TimeStamp addition = new TimeStamp();

            addition.STime = txtStartTime.Text;
            addition.ETime = txtEndTime.Text;
            addition.Status = cStatusBox.SelectionBoxItem.ToString();
            addition.Description = TimestampFunctions.GetDesString(cCallTypeBox.SelectionBoxItem.ToString(), 
                                                                   cCustomerTypeBox.SelectionBoxItem.ToString(),
                                                                   txtReferenceNumber.Text);

            (Resources["StmpDs"] as TimeStampCollection).AddStamp(addition);
            StmpSave.AddStamp(addition);

            cStatusBox.Text = "";
            cCallTypeBox.Text = "";
            cCustomerTypeBox.Text = "";
            txtReferenceNumber.Text = "";

            txtStartTime.Text = txtEndTime.Text;

            TimeStampCollection stamps = (Resources["StmpDs"] as TimeStampCollection);
            changeHandler.AddChange(() => { stamps.RemoveAt(stamps.Count - 1); },
                                    () => { stamps.Insert(stamps.Count, addition); });
        }

        /// <summary>
        /// Handles the event for removing timestamps from the ListView.
        /// </summary>
        /// <param name="sender">The Event sender</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void btnRemoveStamp_Click(object sender, RoutedEventArgs e)
        {
            TimeStampCollection stamps = (Resources["StmpDs"] as TimeStampCollection);
            Dictionary<int, TimeStamp> stmpList = new Dictionary<int, TimeStamp>();

            try
            {
                foreach (TimeStamp tStamp in dgStamps.SelectedItems)
                {
                    stmpList.Add(dgStamps.Items.IndexOf(tStamp), tStamp);
                }
            }
            catch
            {
                MessageBox.Show("Cannot delete that item", "Error");
            }

            foreach (TimeStamp tStamp in stmpList.Values)
            {
                stamps.Remove(tStamp);
                StmpSave.RemoveStamp(tStamp);
            }

            changeHandler.AddChange(() =>
            {
                foreach (var stmp in stmpList.OrderBy(i => i.Key))
                {
                    stamps.Insert(stmp.Key, stmp.Value);
                }
            }, () =>
            {
                foreach (var stmp in stmpList.OrderByDescending(i => i.Key))
                {
                    stamps.RemoveAt(stmp.Key);
                }
            });
        }
        #endregion
        #region Menu
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
                                MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                    StmpSave.Save();
                else if (result == MessageBoxResult.Cancel)
                    return;
            }

            (Resources["StmpDs"] as TimeStampCollection).Clear();
            StmpSave.Stamps.Clear();

            txtStartTime.Text = String.Empty;
            changed = false;
            this.Title = "New Document - Day Logger";
            this.FileName = "New Document";
        }

        /// <summary>
        /// Handle the OnOpen event for saving the document.
        /// </summary>
        /// <param name="sender">The Event sender.</param>
        /// <param name="e">The RoutedEventArgs</param>
        private void OnOpen_Click(object sender, RoutedEventArgs e)
        {
            if (changed == true)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Would you like to save the current changes?", "Confirm Changes",
                                MessageBoxButton.YesNoCancel);

                if (result == MessageBoxResult.Yes)
                    StmpSave.Save();
                else if (result == MessageBoxResult.Cancel)
                    return;
            }

            (Resources["StmpDs"] as TimeStampCollection).Clear();
            StmpSave.Clear();

            StmpSave.Open();

            foreach (TimeStamp stamp in StmpSave.Stamps)
            {
                (Resources["StmpDs"] as TimeStampCollection).AddStamp(stamp);
            }

            this.FileName = System.IO.Path.GetFileName(StmpSave.FilePath);
            this.Title = FileName + " - Day Logger";
            changed = false;
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
                changed = false;

                // Set the FileName and the window title.
                this.FileName = System.IO.Path.GetFileName(StmpSave.FilePath);
                this.Title = FileName + " - Day Logger";
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

        private void OnUndo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OnRedo_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion
        #region Key Input
        /// <summary>
        /// Handles the keyboard macros for interacting with the main window.
        /// </summary>
        /// <param name="sender">The Event sender</param>
        /// <param name="e">The KeyEventArgs for key input</param>
        private void HandleKeyEvent(object sender, KeyEventArgs e)
        {
            bool isCtrlDown = (Keyboard.IsKeyDown(Key.RightCtrl) || Keyboard.IsKeyDown(Key.LeftCtrl));

            switch(e.Key)
            {
                case Key.S:
                    if (isCtrlDown)
                    {
                        OnSave_Click(this, new RoutedEventArgs());
                        e.Handled = true;
                    }
                    break;
                case Key.Y:
                    if (isCtrlDown)
                    {
                        changeHandler.Redo();
                        e.Handled = true;
                    }
                    break;
                case Key.Z:
                    if (isCtrlDown)
                    {
                        changeHandler.Undo();
                        e.Handled = true;
                    }
                    break;
                case Key.Delete:
                    btnRemoveStamp_Click(this, new RoutedEventArgs());
                    e.Handled = true;
                    break;
            }
        }
        #endregion
        #region Timer
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
        #region Delegates
        public delegate Point GetDragDropPosition(IInputElement element);
        #endregion
        #region Instance Fields
        private string FileName;
        private bool changed = false;
        private int prevRowIndex;
        private StampFile StmpSave;
        private DataGridChangeHandler changeHandler;
        #endregion
    }
}