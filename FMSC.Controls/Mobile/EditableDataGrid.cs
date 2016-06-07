using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;

namespace FMSC.Controls 
{

    //public enum DataGridAutoSizeMode
    //{
    //    NotSet = 0,
    //    None = 1,
    //    ColumnHeader = 2,
    //    AllCellsExceptHeader = 4,
    //    AllCells = 6,
    //    DisplayedCellsExceptHeader = 8,
    //    DisplayedCells = 10,
    //    Fill = 16,
    //}

    public class EditableDataGridCellValidatingEventArgs
    {
        public bool Cancel { get; set; }
        public DataGridColumnStyle Column { get; set; }
        public int RowIndex { get; set; }
        public object Value { get; set; }
    }

    public class EditableDataGridCellEventArgs
    {
        public DataGridColumnStyle Column { get; set; }
        public int RowIndex { get; set; }
    }


    public delegate void EditableDataGridCellValidatingEventHandler(Object sender, EditableDataGridCellValidatingEventArgs e);
    public delegate void EditableDataGridCellValueChangedEventHandler(Object sender, EditableDataGridCellEventArgs e);

    public partial class EditableDataGrid : DataGrid, System.ComponentModel.ISupportInitialize, IKeyPressProcessor
    {
        #region events
        //public event EditableDataGridRowAddedEventHandler NewRowAdded;
        public event EditableDataGridCellValidatingEventHandler CellValidating;
        public event EditableDataGridCellValueChangedEventHandler CellValueChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ItemErrorChanged;
        #endregion

        #region static fields

        private static FieldInfo _firstRowVisableAccessor;
        private static FieldInfo _tableStyleAccessor;
        

        #if !NET_CF
        private static  FieldInfo _alternatingBackBrushAccessor;
        #endif 

        #endregion 

        private SolidBrush _altTextBrush;
        private Color _errorColor;
        private bool _readOnly;
        private bool _isScrolling;
        private int _homeColumnIndex = 0;
        private EditableColumnBase _editColumn = null;
        



        #region reflected properties 

#if !NET_CF
        internal SolidBrush AlternatingBackBrush { get { return _alternatingBackBrushAccessor.GetValue(this) as SolidBrush; } }
#endif

        internal int FirstVisibleRow { get { return (int)_firstRowVisableAccessor.GetValue(this); } }

        public DataGridTableStyle TableStyle
        {
            get
            {
                return (DataGridTableStyle)_tableStyleAccessor.GetValue(this);

            }
        }

        #endregion 


        #region Public Properties
        public bool AllowUserToAddRows { get; set; }

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

#if !NET_CF
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
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


        public int HomeColumnIndex
        {
            get { return _homeColumnIndex; }
            set { _homeColumnIndex = (value > 0) ? value : 0; }
        }

#if !NET_CF
        [Browsable(false)]
#endif
        public string HomeColumn
        {
            get { return TableStyle.GridColumnStyles[HomeColumnIndex].MappingName; }
            set
            {
                DataGridColumnStyle colSty = TableStyle.GridColumnStyles[value];
                HomeColumnIndex = TableStyle.GridColumnStyles.IndexOf(colSty);
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

        public int RowCount
        {
            get
            {
                return this.CurrencyManager.Count;
            }
        }

        #endregion

        internal bool IsEditing { get; set; }
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

        private Pen _forePen; 
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


        internal SolidBrush ErrorBrush { get;  set; }

        
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

        internal CurrencyManager CurrencyManager
        {
            get
            {    
#if NET_CF
                return (CurrencyManager)_listManagerAccessor.GetValue(this);
#else
                return this.BindingContext[this.DataSource] as CurrencyManager;
#endif
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
                dataSourceChanged = true;

                if(this.CurrencyManager != null)
                {
                    this.WireDataSource();
                }
            }
        }



        
        #region Ctor

        //this is the static constructor, where we initialize our static variables
        static EditableDataGrid()
        {
            #if NET_CF
            MobileOnlyStaticInit();
            #else
            _firstRowVisableAccessor = typeof(DataGrid).GetField("firstVisableRow", BindingFlags.NonPublic | BindingFlags.Instance);
            _alternatingBackBrushAccessor = typeof(DataGrid).GetField("alternatingBackBrush", BindingFlags.NonPublic | BindingFlags.Instance);
            _tableStyleAccessor = typeof(DataGrid).GetField("myGridTable", BindingFlags.NonPublic | BindingFlags.Instance);
            #endif

        }

        public EditableDataGrid()
            : base()
        {
            this.WireGridEvents();
        }
        #endregion

        //protected override void OnBindingContextChanged(EventArgs e)
        //{
        //    base.OnBindingContextChanged(e);

        //}

        

        protected virtual void OnReadOnlyChanged()
        {
            if (this.ReadOnly)
            {
                this.IsEditing = false;
                this.InternalEndEdit();
            }
            else
            {
                this.Edit();
            }
            //this.Invalidate();
        }


        //public void CancelAddRow()
        //{
        //    this.CurrencyManager.CancelCurrentEdit();
        //}

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
            if (col == null) { return false; }
            if (col.Width <= 0) { return false; }
            if (col.PropertyDescriptor == null) { return false; }
            return true;
        }

        public void AddRow()
        {
            if (this.CurrencyManager == null || this.TableStyle == null) { return; }
            this.CurrencyManager.AddNew();
            this.Invalidate();            
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

        public void MoveFirstEmptyCell()
        {
            if(this.TableStyle == null) { return; }
            object rowData = this.CurrencyManager.List[this.CurrentRowIndex];
            for(int i = 0; i < this.TableStyle.GridColumnStyles.Count; i++)
            {
                DataGridColumnStyle col = this.TableStyle.GridColumnStyles[i];
                if(!this.IsColumnDisplayable(col)) {continue; }
                object cellValue = col.PropertyDescriptor.GetValue(rowData);
                if (cellValue == null)
                {
                    this.CurrentCell = new DataGridCell(this.CurrentRowIndex, i);
                    return;
                }
                Type t = cellValue.GetType();
                if (cellValue is String && String.IsNullOrEmpty(cellValue as String))
                {
                    this.CurrentCell = new DataGridCell(this.CurrentRowIndex, i);
                    return;
                }
                else if (t.IsValueType && object.Equals(cellValue, Activator.CreateInstance(t)))//if value is a value type(int, double, bool ....) compare cellValue to type's default vaule
                {
                    this.CurrentCell = new DataGridCell(this.CurrentRowIndex, i);
                    return;
                }
            }

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

            ensureCurrentCellFocused();// ? finally
            return success;
        }

        public bool SelectNextRow()
        {
            int homeCol = (HomeColumnIndex < this.ColumnCount) ? HomeColumnIndex : 0;

            if ((this.CurrentRowIndex < this.RowCount - 1))                                    //if we aren't at the end OR
            {
                this.CurrentCell = new DataGridCell(this.CurrentRowIndex + 1, this.CurrentColumnIndex); //select next cell in the next row
            }
            else if (this.CurrentRowIndex == this.RowCount - 1 && this.UserAddRow())            //we are at the end and can make a new row
            {
                this.CurrentCell = new DataGridCell(this.CurrentRowIndex, homeCol);     //select next cell in the next row OR
            }
            else                                                                                //we are at the end and can't make a new row
            {
                return false;
            }
            ensureCurrentCellFocused();
            return true;
        }

        protected void ensureCurrentCellFocused()
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

        public bool SelectNextCell(bool forward)
        {
            if (forward && this.CurrentColumnIndex >= this.ColumnCount - 1)                         // are we going forward from the last column
            {
                return SelectNextRow();                                                             //go to next row
            }
            else if (!forward && this.CurrentColumnIndex < 1 && this.CurrentRowIndex > 0)           //are we back from the first column
            {
                this.CurrentCell = new DataGridCell(this.CurrentRowIndex - 1, this.ColumnCount - 1);//go to previous row
                this.ensureCurrentCellFocused();
            }
            else                                                                                    //we aren't on a end column
            {
                this.MoveSeclection((forward) ? Direction.Right: Direction.Left);                             //move cell (left or right)
            }

            
            return true;
        }




        private void UnwireDataSource()
        {
            this.CurrencyManager.MetaDataChanged -= this.DataSource_MetaDataChanged;

            if (IsDataSourceINotifyDataErrorCompatible())
            {
                UnwireDataSourceBinding();
            }
        }

        private void WireDataSource()
        {
            this.CurrencyManager.MetaDataChanged += this.DataSource_MetaDataChanged; 

            if (IsDataSourceINotifyDataErrorCompatible())
            {
                WireDataSourceBinding();
            }
        }

        private void WireDataSourceBinding()
        {
            IBindingList ibList = (IBindingList)this.DataSource;

            ibList.ListChanged += new ListChangedEventHandler(BindingListChanged);

            foreach (INotifyDataErrorInfo indei in ibList)
            {
                indei.ErrorsChanged -= new EventHandler<DataErrorsChangedEventArgs>(OnItemErrorChanged);
            }
        }

        private void UnwireDataSourceBinding()
        {
            IBindingList ibList = (IBindingList)this.DataSource;

            ibList.ListChanged -= new ListChangedEventHandler(BindingListChanged);

            foreach (INotifyDataErrorInfo indei in ibList)
            {
                indei.ErrorsChanged += new EventHandler<DataErrorsChangedEventArgs>(OnItemErrorChanged);
            }
        }

        private bool dataSourceChanged, dataSourceIsCompatible;
        private bool IsDataSourceINotifyDataErrorCompatible()
        {
            if (!dataSourceChanged)
                return dataSourceIsCompatible;

            dataSourceIsCompatible = false;

            if (this.DataSource != null && DataSource is IBindingList)
            {
                IBindingList ibList = (IBindingList)this.DataSource;

                Type itemType = ibList.GetType().GetProperty("Item").PropertyType;

                if (itemType.IsAssignableFrom(typeof(INotifyDataErrorInfo)))
                {
                    dataSourceIsCompatible = true;
                }
            }

            dataSourceChanged = false;
            return dataSourceIsCompatible;
        }

        void BindingListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    ((INotifyDataErrorInfo)sender).ErrorsChanged += OnItemErrorChanged;
                    break;
                case ListChangedType.ItemDeleted:
                    ((INotifyDataErrorInfo)sender).ErrorsChanged -= OnItemErrorChanged;
                    break;
                case ListChangedType.Reset:
                    UnwireDataSourceBinding();
                    WireDataSourceBinding();
                    break;
            }
        }

        private void OnItemErrorChanged(object sender, DataErrorsChangedEventArgs e)
        {
            Refresh();
            
            if (ItemErrorChanged != null)
            {
                ItemErrorChanged(sender, e);
            }
        }


        private void WireGridEvents()
        {
            this.VertScrollBar.ValueChanged += new EventHandler(this.OnGridScroled);
            this.HorizScrollBar.ValueChanged += new EventHandler(this.OnGridScroled);
        }

        private void UnwireGridEvents()
        {
            this.VertScrollBar.ValueChanged -= this.OnGridScroled;
            this.HorizScrollBar.ValueChanged -= this.OnGridScroled;
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

        internal void OnCellValidatingInternal(EditableDataGridCellValidatingEventArgs e)
        {
            this.OnCellValidating(e);
        }

        protected virtual void OnCellValidating(EditableDataGridCellValidatingEventArgs e)
        {
            if(CellValidating != null)
            {
                this.CellValidating(this, e);
            }
        }

        internal void OnCellValueChangedInternal(EditableDataGridCellEventArgs e)
        {
            this.OnCellValueChanged(e);
        }

        protected virtual void OnCellValueChanged(EditableDataGridCellEventArgs e)
        {
            if(this.CellValueChanged != null)
            {
                this.CellValueChanged(this, e);
            }
        }

        protected override void OnCurrentCellChanged(EventArgs e)
        {
            this.IsEditing = false;
            this.InternalEndEdit();
            base.OnCurrentCellChanged(e);//causes bound dataSource to update current
            this.Edit();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            
            base.OnLostFocus(e);
        }


        

        private void Edit()
        {
            this.InternalEndEdit();
            if (!this.ReadOnly)
            {
                
                this.EditColumn = this.CurrentCollumn as EditableColumnBase;
                if (this.EditColumn != null)
                {
                    try
                    {
                        this.IsEditing = true;
                        this.EditColumn.Edit(this.CurrencyManager, this.CurrentCell.RowNumber, this.CurrentCell.ColumnNumber);
                        this.UpdateEditCell();
                        return;
                    }
                    catch
                    { /*fall thru*/}
                }
                //reset editing state, if all else fails 
                this.IsEditing = false;
                this.EditColumn = null;
                this.InternalEndEdit();
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
                catch(Exception e)
                {
                    System.Diagnostics.Debug.Write(e.ToString());
                }
                this.EditColumn.UpdateEditCell(bounds, isvisable);
            }

        }


        //protected void AbortEdit()
        //{

        //}

        public void EndEdit()
        {
            this.IsEditing = false;
            this.InternalEndEdit();
            
        }

        private void InternalEndEdit()
        {
            
            if (EditColumn != null)
            {
                EditColumn.CommitEdit();
                if (IsEditing) { return; }
                EditColumn.EndEdit();
                this.EditColumn = null;
            }

            //DataGrid recieves back focus only if it is Ending the edit
            //that ways if the column loses focus to something other than the dataGrid,
            //the dataGrid isn't fighting for focus
            if (!_isScrolling)
            {
                this.Focus();
            }
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
            
            //Point point = this.PointToClient(new Point(mea.X, mea.Y));
            DataGrid.HitTestInfo hitTest;
            try
            {
                hitTest = this.HitTest(mea.X, mea.Y);
            }
            catch
            {
                return;
            }

            IClickableDataGridColumn column = null;
            if (hitTest.Row >= 0)
            {
                DataGridTableStyle tableStyle = this.TableStyle;
                column = tableStyle.GridColumnStyles[hitTest.Column] as IClickableDataGridColumn;
            }

            
            if (column != null)
            {
                column.HandleMouseDown(hitTest.Row, mea);

                var cellBounds = this.GetCellBounds(hitTest.Row, hitTest.Column);
                this.Invalidate(cellBounds);
                this.Update();
            }
            //else
            //{
            //    base.OnMouseDown(mea);
            //}
            base.OnMouseDown(mea);

        }


        //listen to mouse clicks to allow IClickableDataGridColumns to handle Mouse click events
        protected override void OnMouseUp(MouseEventArgs mea)
        {
            IClickableDataGridColumn column = null;
            DataGrid.HitTestInfo hitTest;
            try
            {
                hitTest = this.HitTest(mea.X, mea.Y);
            }
            catch
            {
                return;
            }

            if (hitTest.Row >= 0)                                                        //check click is in row, not header or invalid 
            {
                DataGridTableStyle tableStyle = this.TableStyle;
                column = tableStyle.GridColumnStyles[hitTest.Column] as IClickableDataGridColumn;
            }
            if (column != null)                                                         //check that column is clickable, if so intercept click
            {                                                                           //dont call base.OnmouseUp if clickable because we dont want to Select the cell, just click it
                column.HandleMouseClick(hitTest.Row);

                var cellBounds = this.GetCellBounds(hitTest.Row, hitTest.Column);
                this.Invalidate(cellBounds);
                this.Update();
            }
            //else
            //{
            //    base.OnMouseUp(mea);
            //}
            base.OnMouseUp(mea);
        }




        #region IKeyPressProcssor Members

#if NET_CF
        public bool ProcessDialogKey(Keys keyVal)
#else
            //hide original member if not net cf
        public new bool ProcessDialogKey(Keys keyVal)        
#endif
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

        public bool ProcessTabKey()
        {
            return this.SelectNextCell(true);
        }

        public bool ProcessBackTabKey()
        {
            return this.SelectNextCell(false);
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

        #endregion

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
    }

    


    //public class EditableDataGridRowAddedEventArgs
    //{
    //    public Object NewRow;
    //    public int RowIndex;
    //}

    //public delegate void EditableDataGridRowAddedEventHandler(Object Sender, EditableDataGridRowAddedEventArgs e);
}


