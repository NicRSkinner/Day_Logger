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
    //this.dgStamps.PreviewMouseLeftButtonDown +=
    //    new MouseButtonEventHandler(dgStamps_PreviewMouseLeftButtonDown);
    //this.dgStamps.Drop += new DragEventHandler(dgStamps_Drop);

    class DragAndDrop
    {
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

            TimeStampCollection stamp = new TimeStampCollection();// = Resources["StmpDs"] as TimeStampCollection;

            TimeStamp movedStmp = stamp[prevRowIndex];
            stamp.RemoveAt(prevRowIndex);
            stamp.Insert(index, movedStmp);

            int prevIndex = prevRowIndex;

            //changeHandler.AddChange(() => { stamp.RemoveAt(index); stamp.Insert(prevIndex, movedStmp); },
            //                        () => { stamp.RemoveAt(prevIndex); stamp.Insert(index, movedStmp); });
        }

        private int prevRowIndex;
        public delegate Point GetDragDropPosition(IInputElement element);
        DataGrid dgStamps = new DataGrid();
    }
}
