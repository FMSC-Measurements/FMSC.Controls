using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace FMSC.Controls
{
    public class SelectionChangingEventArgs
    {
        public bool Cancel;
        public object Item;
        public int RowNumber; 
        public bool IsRemoving;
    }

    public delegate void SelectionChangingEventHandler(object sender, SelectionChangingEventArgs e);

    public class SelectedItemsGridView : DataGridView
    {
        private IList _SelectedItems;

        private DataGridViewCheckBoxColumn _selectItemColumn = new DataGridViewCheckBoxColumn(false)
            {
                Name = "Select",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader,
                HeaderText = "Select",
            };

        public IList SelectedItems 
        {
            get { return _SelectedItems; }
            set
            {
                if (_SelectedItems == value) { return; }
                _SelectedItems = value;
                ClearSelection();
                Refresh();
            }
        }

        public event SelectionChangingEventHandler SelectionChanging;


        protected void OnSelectionChanging(SelectionChangingEventArgs e)
        {
            if (SelectionChanging != null)
            {
                SelectionChanging(this, e);
            }

            if (!e.Cancel)
            {
                if (e.IsRemoving)
                {
                    SelectedItems.Remove(e.Item);

                }
                else
                {
                    SelectedItems.Add(e.Item);
                }
            }
        }

        protected override void OnBindingContextChanged(EventArgs e)
        {
            base.OnBindingContextChanged(e);
        }

        public SelectedItemsGridView()
            : base()
        {
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            RowHeadersVisible = false;
            VirtualMode = true;

            if (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv") { return; }

            Columns.Insert(0, _selectItemColumn);
            //if (DesignMode == false)
            //{
            //    Columns.Insert(0, _selectItemColumn);
            //}
        }
        

        //protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
        //{
        //    if(e.Column.HeaderText == "Select" && DesignMode == true)
        //    {
        //        this.Columns.Remove(e.Column);
        //    }
        //    base.OnColumnAdded(e);
        //}




        //protected override void OnCreateControl()
        //{
        //    base.OnCreateControl();
            //for (int i = 0; i < this.Columns.Count; i++)
            //{
            //    if (this.Columns[i].HeaderText == "Select")
            //    {
            //        this.Columns.RemoveAt(i);
            //        i = 0;

            //    }
            //}
            
        //}

        public bool IsItemSelected(object obj)
        {
            if (SelectedItems == null) { return false; }
            return SelectedItems.Contains(obj);
        }

        protected override void OnCellValueNeeded(DataGridViewCellValueEventArgs e)
        {
            base.OnCellValueNeeded(e);
            IList list = this.DataSource as IList;
            int index = e.RowIndex;
            if (list == null || e.RowIndex >= list.Count ) { return; }
            if (index < 0 || index > list.Count) { return; }
            if (e.ColumnIndex == 0 )
            {
                e.Value = IsItemSelected(list[index]);
            }
        }

        protected override void OnCellContentClick(DataGridViewCellEventArgs e)
        {
            base.OnCellContentClick(e);
            if (e.RowIndex == -1) { return; }
            if (SelectedItems == null) { return; }
            if (DataSource == null || (DataSource is IList) == false) { return; }
            Object item = ((IList)DataSource)[e.RowIndex];
            if(e.ColumnIndex == 0)
            {
                var cellValue = this[e.ColumnIndex, e.RowIndex].Value;
                SelectionChangingEventArgs eArgs = new SelectionChangingEventArgs()
                {
                    Item = item,
                    IsRemoving = (cellValue != null && (bool)cellValue == true)
                    ,
                    RowNumber = e.RowIndex
                };
                this.OnSelectionChanging(eArgs);
                //if (eArgs.Cancel == true)
                //{
                //    this[e.ColumnIndex, e.RowIndex].Value = cellValue;
                //}
            }
        }

        protected override void OnColumnRemoved(DataGridViewColumnEventArgs e)
        {
            //base.OnColumnRemoved(e);
            //if (Columns.Contains(_selectItemColumn) == false)
            //{
            //    Columns.Insert(0, _selectItemColumn);
            //}
        }
    }
}
