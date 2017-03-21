using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.WindowsCE.Forms;

namespace FMSC.Controls
{


    public delegate void EditableDataGridCellValidatingEventHandler(Object sender, EditableDataGridCellValidatingEventArgs e);
    public delegate void EditableDataGridCellValueChangedEventHandler(Object sender, EditableDataGridCellEventArgs e);

    public partial class EditableDataGrid : DataGrid, System.ComponentModel.ISupportInitialize, IKeyPressProcessor
    {
        #region static members

#if !NET_CF
        private static  FieldInfo _alternatingBackBrushAccessor;
#endif
        private static FieldInfo _firstRowVisableAccessor;
        private static FieldInfo _gridRecAccessor;
        private static FieldInfo _gridRendererAccessor;
        private static FieldInfo _horzScrollBarAccessor;
        private static FieldInfo _listManagerAccessor;
        private static FieldInfo _rowHeightAccessor;
        private static FieldInfo _tableStyleAccessor;
        private static FieldInfo _vertScrollBarAccessor;

        static EditableDataGrid()
        {
            _firstRowVisableAccessor = typeof(EditableDataGrid).GetField("m_irowVisibleFirst", BindingFlags.NonPublic | BindingFlags.Instance);
            _horzScrollBarAccessor = typeof(EditableDataGrid).GetField("m_sbHorz", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            _vertScrollBarAccessor = typeof(EditableDataGrid).GetField("m_sbVert", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            _tableStyleAccessor = typeof(EditableDataGrid).GetField("m_tabstyActive", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            _rowHeightAccessor = typeof(EditableDataGrid).GetField("m_cyRow", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            _listManagerAccessor = typeof(EditableDataGrid).GetField("m_cmData", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

            _gridRendererAccessor = typeof(EditableDataGrid).GetField("m_renderer", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            Type gridRenderer = (_gridRendererAccessor != null) ? _gridRendererAccessor.FieldType : null;
            if (gridRenderer != null)
            {
                _gridRecAccessor = gridRenderer.GetField("m_rcGrid", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            }
        }

        #endregion static fields

        Color _alternatingBackColor;
        SolidBrush _altTextBrush;
        Color _errorColor;
        Pen _forePen;

        EditableColumnBase _editColumn;        
        bool _inCommitEdit;
        bool _isEditing;
        bool _isScrolling;
        bool _readOnly;

        int _homeColumnIndex = 0;
        InputPanel _sip;

        public EditableDataGrid()
        {
            this.WireGridEvents();
        }

        public event EditableDataGridCellValidatingEventHandler CellValidating;

        public event EditableDataGridCellValueChangedEventHandler CellValueChanged;

        #region props

        #region Behavior
        public bool AllowUserToAddRows { get; set; }

        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                if (_readOnly == value) { return; }
                _readOnly = value;
                this.OnReadOnlyChanged();
            }
        }
        #endregion

        #region Appearance
        public Color AlternatingBackColor
        {
            get { return _alternatingBackColor; }
            set
            {
                if (_alternatingBackColor == value)
                {
                    return;
                }
                if (AlternatingBackBrush != null)
                {
                    AlternatingBackBrush.Dispose();
                }
                AlternatingBackBrush = new SolidBrush(value);
                _alternatingBackColor = value;
            }
        }

        public Color ErrorColor
        {
            get { return _errorColor; }
            set
            {
                if (_errorColor == value)
                {
                    return;
                }
                if (ErrorBrush != null)
                {
                    ErrorBrush.Dispose();
                }
                ErrorBrush = new SolidBrush(value);
                _errorColor = value;
            }
        }

        internal SolidBrush AlternatingBackBrush { get; private set; }

        private SolidBrush AltTextBrush
        {
            get
            {
                if (_altTextBrush == null)
                {
                    _altTextBrush = new SolidBrush(this.HeaderForeColor);
                }
                return _altTextBrush;
            }
        }

        internal SolidBrush ErrorBrush { get; set; }

        internal Pen ForePen
        {
            get
            {
                if (_forePen == null)
                {
                    this._forePen = new Pen(this.ForeColor, 2);
                }
                return this._forePen;
            }
        }        
        #endregion


        public int ColumnCount
        {
            get
            {
                if (this.TableStyle == null) { return 0; }
                return this.TableStyle.GridColumnStyles.Count;
            }
        }

        public DataGridColumnStyle CurrentCollumn
        {
            get
            {
                DataGridTableStyle ts = this.TableStyle;
                if (ts != null)
                {
                    return ts.GridColumnStyles[this.CurrentColumnIndex];
                }
                else
                {
                    return null;
                }
            }
        }

        public int CurrentColumnIndex
        {
            get
            {
                return this.CurrentCell.ColumnNumber;
            }
            set
            {
                this.CurrentCell = new DataGridCell(this.CurrentCell.RowNumber, value);
            }
        }

        internal CurrencyManager CurrencyManager
        {
            get
            {
                return (CurrencyManager)_listManagerAccessor.GetValue(this);
            }
        }

        public new object DataSource
        {
            get
            {
                return base.DataSource;
            }
            set
            {
                if (base.DataSource != null && !base.DataSource.Equals(value))
                {
                    if (this.CurrencyManager != null)
                    {
                        this.UnwireDataSource();
                    }
                }
                base.DataSource = value;

                if (this.CurrencyManager != null)
                {
                    this.WireDataSource();
                }
            }
        }

        internal EditableColumnBase EditColumn
        {
            get
            {
                return _editColumn;
            }
            set
            {
                if (value == _editColumn) { return; }
                _editColumn = value;
            }
        }

        internal int FirstVisibleRow { get { return (int)_firstRowVisableAccessor.GetValue(this); } }
        
        protected Rectangle GridRec
        {
            get
            {
                if (_gridRecAccessor != null)
                {
                    object render = _gridRendererAccessor.GetValue(this);
                    return (Rectangle)(_gridRecAccessor.GetValue(render) ?? Rectangle.Empty);
                }
                return Rectangle.Empty;
            }
        }

        //public string HomeColumn
        //{
        //    get
        //    {
        //        try
        //        {
        //            return TableStyle.GridColumnStyles[HomeColumnIndex].MappingName;
        //        }
        //        catch (IndexOutOfRangeException)
        //        { return null; }
        //    }
        //    set
        //    {
        //        var tabSty = TableStyle;
        //        if (tabSty != null
        //            && tabSty.GridColumnStyles.Contains(value))
        //        {
        //            var colSty = TableStyle.GridColumnStyles[value];
        //            HomeColumnIndex = TableStyle.GridColumnStyles.IndexOf(colSty);
        //        }
        //    }
        //}

        //public int HomeColumnIndex
        //{
        //    get { return _homeColumnIndex; }
        //    set { _homeColumnIndex = (value > 0) ? value : 0; }
        //}

        public int RowCount
        {
            get
            {
                return this.CurrencyManager.Count;
            }
        }

        public InputPanel SIP
        {
            get { return _sip; }

            set
            {
                if (value == _sip)
                {
                    return;
                }
                if (_sip != null)
                {
                    _sip.Dispose();
                }
                _sip = value;
                _sip.EnabledChanged += new EventHandler(OnSIPDisplayed);
            }
        }

        public DataGridTableStyle TableStyle
        {
            get
            {
                return (DataGridTableStyle)_tableStyleAccessor.GetValue(this);
            }
        }

        public ScrollBar HorizScrollBar { get { return (ScrollBar)_horzScrollBarAccessor.GetValue(this); } }

        public ScrollBar VertScrollBar { get { return (ScrollBar)_vertScrollBarAccessor.GetValue(this); } }

        private int RowHeight
        {
            get
            {
                if (_rowHeightAccessor != null)
                {
                    object value = _rowHeightAccessor.GetValue(this);
                    if (value != null && value is int)
                    {
                        return (int)value;
                    }
                }
                return 22;
            }
        }
        #endregion

        public void AddRow()
        {
            if (this.CurrencyManager == null || this.TableStyle == null) { return; }
            this.CurrencyManager.AddNew();
            this.Invalidate();
        }

        public void Edit()
        {
            EndEdit();//end any existing edits

            if (ReadOnly) { return; } //do nothing if dataGrid set to readOnly

            var col = CurrentCollumn as EditableColumnBase;

            if (col == null
                || !IsColumnDisplayable(col)
                || col.ReadOnly) { return; }

            try
            {
                EditColumn = col;
                var curCell = CurrentCell;
                EditColumn.Edit(CurrencyManager
                    , curCell.RowNumber
                    , curCell.ColumnNumber);

                _isEditing = true;

                UpdateEditCell();
            }
            catch
            {
                //reset editing state, if all else fails
                EndEdit();
            }
        }

        private void CommitEdit()
        {
            if (!_isEditing || _inCommitEdit) { return; }

            var editCol = EditColumn;
            if (editCol != null)
            {
                _inCommitEdit = true;
                editCol.CommitEdit();
                _inCommitEdit = false;
            }
        }

        public void EndEdit()
        {
            if (!_isEditing) { return; }
            _isEditing = false;

            var editCol = EditColumn;
            if (editCol != null)
            {
                editCol.CommitEdit();
            }

            ////DataGrid recieves back focus only if it is Ending the edit
            ////that ways if the column loses focus to something other than the dataGrid,
            ////the dataGrid isn't fighting for focus
            //if (!_isScrolling)
            //{
            //    this.Focus();
            //}
        }

        public bool MoveFirstEmptyCell()
        {
            var rowIndex = this.CurrentRowIndex;
            if (rowIndex < 0) { return false; }
            if (this.TableStyle == null) { return false; }
            object rowData = this.CurrencyManager.List[rowIndex];
            for (int i = 0; i < this.TableStyle.GridColumnStyles.Count; i++)
            {
                var col = this.TableStyle.GridColumnStyles[i] as EditableColumnBase;
                if (col == null 
                    || !this.IsColumnDisplayable(col)
                    || col.ReadOnly) { continue; }

                object cellValue = col.PropertyDescriptor.GetValue(rowData);
                if (cellValue == null)
                {
                    this.CurrentCell = new DataGridCell(rowIndex, i);
                    return true;
                }
                Type t = cellValue.GetType();
                if (cellValue is String && String.IsNullOrEmpty(cellValue as String))
                {
                    this.CurrentCell = new DataGridCell(rowIndex, i);
                    return true;
                }
                else if (t.IsValueType && object.Equals(cellValue, Activator.CreateInstance(t)))//if value is a value type(int, double, bool ....) compare cellValue to type's default vaule
                {
                    this.CurrentCell = new DataGridCell(rowIndex, i);
                    return true;
                }
            }
            return false;
        }

        public bool MoveSeclection(Direction direction)
        {
            bool success = false;
            try
            {
                switch (direction)
                {
                    case Direction.Up:
                        {
                            if (this.CurrentRowIndex > 0)
                            {
                                this.CurrentRowIndex--;
                                success = true;
                            }
                            break;
                        }
                    case Direction.Down:
                        {
                            if (CurrentRowIndex < this.RowCount - 1)
                            {
                                this.CurrentRowIndex++;
                                success = true;
                            }
                            else
                            {
                                this.SelectNextRow();
                            }
                            break;
                        }
                    case Direction.Left:
                        {
                            if (this.CurrentColumnIndex > 0)
                            {
                                this.CurrentColumnIndex--;
                                success = true;
                            }
                            break;
                        }
                    case Direction.Right:
                        {
                            if (CurrentColumnIndex < ColumnCount - 1)
                            {
                                this.CurrentColumnIndex++;
                                success = true;
                            }
                            break;
                        }
                }
            }
            catch
            {
                success = false;
            }

            if (success && !this.IsColumnDisplayable(this.CurrentCollumn))
            {
                return this.MoveSeclection(direction);
            }

            EnsureCurrentCellFocused();// ? finally
            return success;
        }

        public bool SelectNextCell(bool forward)
        {
            //are we going forward from the last column
            if (forward && this.CurrentColumnIndex >= this.ColumnCount - 1)                         // are we going forward from the last column
            {
                return SelectNextRow();                                                             //go to next row
            }
            //are we going back from the first column
            else if (!forward && this.CurrentColumnIndex < 1 && this.CurrentRowIndex > 0)
            {
                //go to previous row
                this.CurrentCell = new DataGridCell(this.CurrentRowIndex - 1, this.ColumnCount - 1);
                this.EnsureCurrentCellFocused();
            }
            else
            {
                //move cell (left or right)
                this.MoveSeclection((forward) ? Direction.Right : Direction.Left);
            }

            return true;
        }

        public bool SelectNextRow()
        {
            if ((this.CurrentRowIndex < this.RowCount - 1))                     //if we aren't at the end OR
            {
                this.CurrentCell = new DataGridCell(this.CurrentRowIndex + 1, this.CurrentColumnIndex);//select next cell in the next row
            }
            else if (this.CurrentRowIndex == this.RowCount - 1 && this.UserAddRow())//we are at the end and can make a new row
            {
                this.CurrentCell = new DataGridCell(this.CurrentRowIndex, 0);   //select first cell in the next row OR
            }
            else                                                                //we are at the end and can't make a new row
            {
                return false;
            }
            EnsureCurrentCellFocused();
            return true;
        }

        public bool UserAddRow()
        {
            if (AllowUserToAddRows)
            {
                this.AddRow();
                return true;
            }
            else
            {
                return false;
            }
        }

        internal void OnCellValidatingInternal(EditableDataGridCellValidatingEventArgs e)
        {
            this.OnCellValidating(e);
        }

        internal void OnCellValueChangedInternal(EditableDataGridCellEventArgs e)
        {
            this.OnCellValueChanged(e);
        }
        protected void EnsureCurrentCellFocused()
        {
            EditableColumnBase col = this.TableStyle.GridColumnStyles[this.CurrentColumnIndex] as EditableColumnBase;
            if (col == null) //if current column doesn't have a control to have focus then, send focus to grid
            {
                this.Focus();
            }
            else
            {
                if (col.HostedControl.Focused == false)
                {
                    col.HostedControl.Focus();
                }
            }
        }

        protected void EnsureRowVisible(int Row)
        {
            try
            {
                Rectangle currentCellBounds = this.GetCellBounds(this.CurrentCell.RowNumber, this.CurrentCell.ColumnNumber);
                currentCellBounds = this.RectangleToScreen(currentCellBounds); //because the cell bounds we have is only relitive to
                //our data grid and we need to translate it to be relitive to the whole screen

                if (_sip.Bounds.IntersectsWith(currentCellBounds))
                {
                    VertScrollBar.Visible = true;                                       //ensure that the scroll bar is visable
                    VertScrollBar.Value = this.CurrentRowIndex;
                    //re check cell bounds and make see if it's overlaping the sip
                    currentCellBounds = this.GetCellBounds(this.CurrentCell.RowNumber, this.CurrentCell.ColumnNumber);
                    currentCellBounds = this.RectangleToScreen(currentCellBounds);
                    //this case will likely only hit if the scroll bar was maxed out
                    //so lets increase the max and retry
                    if (_sip.Bounds.IntersectsWith(currentCellBounds))
                    {
                        VertScrollBar.Maximum += this.VisibleRowCount - 1;// this is the important bit
                        //you can't increase the maximum on the scroll bar too much
                        //if you do then it will allow the user to scroll past all the rows
                        //and this causes the app to crash.
                        VertScrollBar.Value = this.CurrentRowIndex;
                    }
                }
            }
            catch
            {
                //catches general exception that may be thrown on GetCellBounds if no rows exist.
                //but if that happens then we just jump out of this method.
            }
        }

        protected int GetDisplayableColumnCount()
        {
            int count = 0;
            if (this.TableStyle == null) { return 0; }
            for (int i = 0; i < this.TableStyle.GridColumnStyles.Count; i++)
            {
                if (this.IsColumnDisplayable(i))
                {
                    count++;
                }
            }
            return count;
        }

        protected bool IsColumnDisplayable(int colNum)
        {
            if (colNum < 0 || colNum > this.ColumnCount) { return false; }
            return this.IsColumnDisplayable(this.TableStyle.GridColumnStyles[colNum]);
        }

        protected bool IsColumnDisplayable(DataGridColumnStyle col)
        {
            if (col == null
                || col.Width <= 0
                || col.PropertyDescriptor == null)
            { return false; }
            else
            { return true; }
        }

        protected virtual void OnCellValidating(EditableDataGridCellValidatingEventArgs e)
        {
            if (CellValidating != null)
            {
                this.CellValidating(this, e);
            }
        }

        protected virtual void OnCellValueChanged(EditableDataGridCellEventArgs e)
        {
            if (this.CellValueChanged != null)
            {
                this.CellValueChanged(this, e);
            }
        }

        protected override void OnCurrentCellChanged(EventArgs e)
        {
            this.EndEdit();
            base.OnCurrentCellChanged(e);//causes bound dataSource to update current
            this.Edit();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = this.ProcessKeyPress(e.KeyData);
            if (e.Handled == true) { return; }
            base.OnKeyDown(e);
        }

        //listen to mouse clicks to allow IClickableDataGridColumns to handle MouseDown events
        protected override void OnMouseDown(MouseEventArgs mea)
        {
            base.OnMouseDown(mea);

            DataGrid.HitTestInfo hitTest;
            try
            {
                hitTest = this.HitTest(mea.X, mea.Y);
            }
            catch
            {
                return;
            }

            if (hitTest.Type == HitTestType.Cell
                && hitTest.Row > -1
                && hitTest.Column > -1)
            {
                DataGridTableStyle tableStyle = this.TableStyle;
                var column = tableStyle.GridColumnStyles[hitTest.Column];

                if (column != null && column is IClickableDataGridColumn)
                {
                    ((IClickableDataGridColumn)column).HandleMouseDown(hitTest.Row, mea);

                    var cellBounds = this.GetCellBounds(hitTest.Row, hitTest.Column);
                    this.Invalidate(cellBounds);
                    this.Update();
                }

                var curCell = CurrentCell;
                if (CurrentCell.ColumnNumber == hitTest.Column
                    && CurrentCell.RowNumber == hitTest.Row)
                {
                    Edit();
                }
            }
        }

        //listen to mouse clicks to allow IClickableDataGridColumns to handle Mouse click events
        protected override void OnMouseUp(MouseEventArgs mea)
        {
            base.OnMouseUp(mea);

            DataGrid.HitTestInfo hitTest;
            try
            {
                hitTest = this.HitTest(mea.X, mea.Y);
            }
            catch
            {
                return;
            }

            if (hitTest.Column > -1
                && hitTest.Row > -1
                && hitTest.Type == HitTestType.Cell)
            {
                DataGridTableStyle tableStyle = this.TableStyle;
                var column = tableStyle.GridColumnStyles[hitTest.Column] as IClickableDataGridColumn;

                if (column != null)                                                         //check that column is clickable, if so intercept click
                {                                                                           //dont call base.OnmouseUp if clickable because we dont want to Select the cell, just click it
                    column.HandleMouseClick(hitTest.Row);

                    var cellBounds = this.GetCellBounds(hitTest.Row, hitTest.Column);
                    this.Invalidate(cellBounds);
                    this.Update();
                }
            }
        }

        protected virtual void OnReadOnlyChanged()
        {
            if (this.ReadOnly)
            {
                EndEdit();
            }
            else
            {
                this.Edit();
            }
        }

        protected void UpdateEditCell()
        {
            if (EditColumn != null)
            {
                Rectangle bounds = Rectangle.Empty;
                bool isvisable = false;
                try
                {
                    bounds = this.GetCellBounds(this.EditColumn.EditRow, this.EditColumn.ColumnOrdinal);
                    isvisable = this.GridRec.IntersectsWith(bounds);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Write(e.ToString());
                }
                EditColumn.UpdateEditCell(bounds, isvisable);
            }
        }

        private void DataSource_MetaDataChanged(object sender, EventArgs e)
        {
            try
            {
                this.EndEdit();
                this.CurrencyManager.EndCurrentEdit();
            }
            catch
            {
                /* do nothing */
            }
        }
        internal void OnEditControlLostFocus()
        {
            if (!this.Focused)
            {
                EndEdit();
            }
        }

        private void OnGridScroled(Object sender, EventArgs e)
        {
            _isScrolling = true;
            try
            {
                this.UpdateEditCell();
            }
            finally
            {
                _isScrolling = false;
            }
        }

        void OnSIPDisplayed(object sender, EventArgs e)
        {
            this.EnsureRowVisible(this.CurrentRowIndex);
        }

        private void UnwireDataSource()
        {
            this.CurrencyManager.MetaDataChanged -= this.DataSource_MetaDataChanged;
        }

        private void UnwireGridEvents()
        {
            this.VertScrollBar.ValueChanged -= this.OnGridScroled;
            this.HorizScrollBar.ValueChanged -= this.OnGridScroled;
        }

        private void WireDataSource()
        {
            this.CurrencyManager.MetaDataChanged += this.DataSource_MetaDataChanged;
        }

        private void WireGridEvents()
        {
            this.VertScrollBar.ValueChanged += new EventHandler(this.OnGridScroled);
            this.HorizScrollBar.ValueChanged += new EventHandler(this.OnGridScroled);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.UnwireGridEvents();
                if (this.ErrorBrush != null)
                {
                    this.ErrorBrush.Dispose();
                    this.ErrorBrush = null;
                }
                if (_altTextBrush != null)
                {
                    _altTextBrush.Dispose();
                    _altTextBrush = null;
                }
                if (this._forePen != null)
                {
                    this._forePen.Dispose();
                }
            }
        }

        #region ISupportInitialize Members

        //HACK Code generator automaticly puts in calls to
        //theses methods and can't be fixed, so these methods
        //must be included but do nothing.
        public void BeginInit()
        { /*do nothing*/ }

        public void EndInit()
        { /*do nothing*/ }

        #endregion ISupportInitialize Members

        #region IKeyPressProcssor Members

        public bool ProcessBackTabKey()
        {
            return this.SelectNextCell(false);
        }

        public bool ProcessDialogKey(Keys keyVal)
        {
            switch (keyVal)
            {
                case (Keys.Tab):
                    {
                        return this.ProcessTabKey();
                    }
                case ((Keys.Left)):
                case ((Keys.Control | Keys.Left)):
                    {
                        return this.MoveSeclection(Direction.Left);
                    }
                case ((Keys.Control | Keys.Down)):
                    {
                        //ClearInputMessageQueue();
                        return this.MoveSeclection(Direction.Down);
                    }
                case ((Keys.Down)):
                    {
                        return this.MoveSeclection(Direction.Down);
                    }

                case ((Keys.Control | Keys.Up)):
                    {
                        //ClearInputMessageQueue();
                        return this.MoveSeclection(Direction.Up);
                    }
                case ((Keys.Up)):
                    {
                        return this.MoveSeclection(Direction.Up);
                    }
                case ((Keys.Right)):
                case ((Keys.Control | Keys.Right)):
                    {
                        return this.MoveSeclection(Direction.Right);
                    }
            }

            //we dont know how to handle the key press
            //so, lets see if our parent want to
            IKeyPressProcessor p = this.Parent as IKeyPressProcessor;
            if (p == null) { return false; }
            return p.ProcessDialogKey(keyVal);
        }

        public bool ProcessKeyPress(Keys keyVal)
        {
            switch (keyVal)
            {
                case (Keys.Tab):
                case ((Keys.Left)):
                case ((Keys.Down)):
                case ((Keys.Up)):
                case ((Keys.Right)):
                case ((Keys.Control | Keys.Left)):
                case ((Keys.Control | Keys.Down)):
                case ((Keys.Control | Keys.Up)):
                case ((Keys.Control | Keys.Right)):
                    {
                        return this.ProcessDialogKey(keyVal);
                    }
            }
            //we dont know how to handle the key press
            //so, lets see if our parent want to
            IKeyPressProcessor p = this.Parent as IKeyPressProcessor;
            if (p == null) { return false; }
            return p.ProcessKeyPress(keyVal);
        }

        public bool ProcessReturnKey()
        {
            IClickableDataGridColumn col = this.CurrentCollumn as IClickableDataGridColumn;
            if (col != null)
            {
                col.HandleMouseClick(this.CurrentRowIndex);
                return true;
            }
            return false;
        }

        public bool ProcessTabKey()
        {
            return this.SelectNextCell(true);
        }

        #endregion IKeyPressProcssor Members
    }

    public class EditableDataGridCellEventArgs
    {
        public DataGridColumnStyle Column { get; set; }

        public int RowIndex { get; set; }
    }

    public class EditableDataGridCellValidatingEventArgs
    {
        public bool Cancel { get; set; }

        public DataGridColumnStyle Column { get; set; }

        public int RowIndex { get; set; }

        public object Value { get; set; }
    }
}