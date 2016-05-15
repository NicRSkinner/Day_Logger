using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Day_Logger
{
    /// <summary>
    /// The interface for ChangeHandlers.
    /// </summary>
    public interface IChangeHandler
    {
        void AddChange(Action undo, Action redo);
        void AddChange(Change chg);
        void Undo();
        void Redo();
    }

    /// <summary>
    /// The ChangeHandler for a DataGrid
    /// </summary>
    public class DataGridChangeHandler : IChangeHandler
    {
        #region Initializers
        /// <summary>
        /// Default initializer for the DataGridChangeHandler class.
        /// </summary>
        public DataGridChangeHandler()
        {

        }
        #endregion
        #region Methods
        /// <summary>
        /// This function converts individual Change parameters into a change object for the list.
        /// </summary>
        /// <param name="undo">The function to be called when Undo is invoked.</param>
        /// <param name="redo">The function to be called when Redo is invoked.</param>
        public void AddChange(Action undo, Action redo)
        {
            Change chg = new Change()
            {
                UndoFunc = undo,
                RedoFunc = redo
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
        /// This function undoes the change that was made in the DataGrid.
        /// </summary>
        public void Undo()
        {
            if (lUndo == null || lUndo.Count == 0)
                return;

            Change chg = lUndo.Pop();
            chg.UndoFunc.Invoke();

            lRedo.Push(chg);

            // Raise the changed event.
            Onchanged();
        }

        /// <summary>
        /// This function reverts a previous "undo" if it is able to.
        /// </summary>
        public void Redo()
        {
            if (lRedo == null || lRedo.Count == 0)
                return;

            Change chg = lRedo.Pop();
            chg.RedoFunc.Invoke();

            lUndo.Push(chg);

            // Raise the changed event.
            Onchanged();
        }

        /// <summary>
        /// Clears the change stacks.
        /// </summary>
        public void Clear()
        {
            lUndo.Clear();
            lRedo.Clear();
            HasChanged = false;
        }

        /// <summary>
        /// This functions raises the changed event to notify any 
        ///     active listeners that changes have been made.
        /// </summary>
        private void Onchanged()
        {
            HasChanged = true;

            if (this.Changed != null)
                Changed(this, new RoutedEventArgs());
        }
        #endregion
        #region Events
        public event ChangedEventHandler Changed;
        #endregion
        #region Delegates
        public delegate void ChangedEventHandler(object sender, RoutedEventArgs e);
        public bool HasChanged { get; set; }
        private Stack<Change> lUndo;
        private Stack<Change> lRedo;
        #endregion  
    }

    /// <summary>
    /// Holds the functions for undo/redo operations.
    /// </summary>
    public class Change
    {
        public Action UndoFunc { get; set; }
        public Action RedoFunc { get; set; }
    }
}