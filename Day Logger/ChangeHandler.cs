using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Day_Logger
{
    public class ChangeHandler
    {
        #region Initializers
        public ChangeHandler()
        {

        }
        #endregion
        #region Methods
        /// <summary>
        /// This function converts individual Change parameters into a change object for the list.
        /// </summary>
        /// <param name="undo">The function to be called when Undo is invoked.</param>
        /// <param name="redo">The function to be called when Redo is invoked.</param>
        /// <param name="index">The index of the TimeStamp in the DataGrid object.</param>
        /// <param name="stamp">The TimeStamp that was changed.</param>
        public void AddChange(Action<int, TimeStamp> undo, Action<int, TimeStamp> redo, int index, TimeStamp stamp)
        {
            Change chg = new Change()
            {
                UndoFunc = undo,
                RedoFunc = redo,
                index = index,
                stamp = stamp
            };

            AddChange(chg);
        }

        /// <summary>
        /// This method is used to add changes for the undo/redo operations.
        /// </summary>
        /// <param name="chg">The Change object.</param>
        public void AddChange(Change chg)
        {
            // Lazy initialization for the Undo list.
            if (lUndo == null)
                lUndo = new Stack<Change>();

            lUndo.Push(chg);

            // Lazy initialization for the Redo list.
            if (lRedo == null)
                lRedo = new Stack<Change>();

            // Clear the redo list because we added a new change.
            lRedo.Clear();

            // Raise the changed event.
            Onchanged();
        }

        /// <summary>
        /// This function undos the change that was made in teh DataGrid
        /// </summary>
        public void Undo()
        {
            if (lUndo == null || lUndo.Count == 0)
                return;

            Change chg = lUndo.Pop();
            chg.UndoFunc.Invoke(chg.index, chg.stamp);

            lRedo.Push(chg);
        }

        /// <summary>
        /// This function reverts a previous "undo" if it is able to.
        /// </summary>
        public void Redo()
        {
            if (lRedo == null || lRedo.Count == 0)
                return;

            Change chg = lRedo.Pop();
            chg.RedoFunc.Invoke(chg.index, chg.stamp);
        }

        /// <summary>
        /// This functions raises the changed event to notify any 
        ///     active listeners that changes have been made.
        /// </summary>
        private void Onchanged()
        {
            if (this.Changed != null)
                Changed(this, new RoutedEventArgs());
        }
        #endregion
        #region Events
        public event ChangedEventHandler Changed;
        #endregion
        #region Delegates
        public delegate void ChangedEventHandler(object sender, RoutedEventArgs e);
        private Stack<Change> lUndo;
        private Stack<Change> lRedo;
        #endregion
    }

    public class Change
    {
        public Action<int, TimeStamp> UndoFunc { get; set; }
        public Action<int, TimeStamp> RedoFunc { get; set; }
        public int index { get; set; }
        public TimeStamp stamp { get; set; }
    }
}