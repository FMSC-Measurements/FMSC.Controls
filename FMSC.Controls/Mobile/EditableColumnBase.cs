//TODO add support for IDataErrorProvider 

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FMSC.Controls 
{
    public class CellEditEventArgs 
    {
        public CellEditEventArgs(int row, int col)
        {
            this.Row = row;
            this.Column = col;
        }
        public int Row { get; private set; }
        public int Column { get; private set; }
    }

    public delegate void CellEditEventHandler(EditableColumnBase sender, CellEditEventArgs e);

    // We'll inherit from DataGridTextBoxColumn, not from DataGridColumnStyle to ensure desktop compatibility.
    // Since some abstract methods are not availible on NETCF's DataGridColumnStyle, it's not possible to override them.
    // Thus attempt to run this code on desktop would fail as these abstract methods won't have implementation at runtime.
    public abstract partial class EditableColumnBase : CustomColumnBase
    {
        #region Statics

        private readonly static int ERROR_ICON_WIDTH = 10;

        #endregion

        #region Privates
        

        


        //private Color _alternatingBackColor = SystemColors.Window;                    // Back color for odd numbered rows
        //private SolidBrush _alternatingBrush = null;                                  // Brush to use for odd numbered rows
        
        #endregion

        #region Events 
        //public event EditableDataGridCellValidatingEventArgs Validating;
        //public event EventHandler TextChanged;
        public event CellEditEventHandler CellEditBeginning;
        //public event CellEditEventHandler CellEditEnding;
        #endregion

        #region Fields
        protected bool _isEditing;
        protected Control _hostedControl = null;                                        // Column's hosted control (e.g. TextBox).
        protected Rectangle _bounds = Rectangle.Empty;                                  // Last known bounds of hosted control.
        private bool _readOnly = false;                                                 // ReadOnly state
        //protected int _row;
        protected System.Reflection.MethodInfo _parseMethod;
        protected object _orgValue;

        #endregion

        #region Protected Properties
        
        protected abstract bool CanShowHostedControlIfReadOnly { get; }                 // Sub-Class can decide if host control can be show in read only mode. Use this if host control has its own read only property 

        #endregion

        #region Public properties




        // Gets hosted control.
        public virtual Control HostedControl
        {
            get
            {
                if ((null == this._hostedControl) && (this.Owner != null))              // If not created and have owner...
                {                                                                       
                    this._hostedControl = this.CreateHostedControl();                   // Create hosted control.
                    
                    //this._hostedControl.Validating += new CancelEventHandler(this.OnValidating);
                    //this._hostedControl.TextChanged += new EventHandler(this.OnTextChanged);

                    this._hostedControl.Visible = false;                                // Hide it.
                    this._hostedControl.Name = this.HeaderText;
                    this._hostedControl.Font = this.Owner.Font;                         // Set up control's font to match grid's font.
                    this._hostedControl.BackColor = this.Owner.SelectionBackColor;      //make sure our control has the same back color as the data grid
                    this._hostedControl.ForeColor = this.Owner.SelectionForeColor;

                    this._hostedControl.LostFocus += new EventHandler(_hostedControl_LostFocus);
                    this._hostedControl.Parent = this.Owner;

                    //_hostControlBinding = new Binding(this.GetBoundPropertyName(), Owner.DataSource, this.MappingName, true, DataSourceUpdateMode.OnValidation, this.NullValue, this.Format, this.FormatInfo);
                    //_hostControlBinding.Parse += new ConvertEventHandler(binding_Parse);
                    //this._hostedControl.DataBindings.Add(_hostControlBinding);
                    switch (Type.GetTypeCode(this.PropertyDescriptor.PropertyType))
                    {
                        case TypeCode.Byte:
                        case TypeCode.Decimal:
                        case TypeCode.Double:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.SByte:
                        case TypeCode.Single:
                        case TypeCode.UInt16:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                            {
                                Microsoft.WindowsCE.Forms.InputModeEditor.SetInputMode(_hostedControl, Microsoft.WindowsCE.Forms.InputMode.Numeric);
                                break;
                            } 
                    }
                    

                    
                                                                                        // Set up data binding so contol would get data from data source.

                    // Now we need to hook into grid's horisontal scroll event so we could move hosted control as user scrolls.
                    // To do so we'll look for HScrollBar control owned by the groid. We'll grab the first one we found.

                    //HScrollBar horisonal = null;// Assume no ScrollBar found.
                    //VScrollBar vert = null;
                        
                    //foreach (Control c in this.Owner.Controls)                          // For each controls owned by grid...
                    //{
                    //    if (horisonal == null && (horisonal = c as HScrollBar) != null)                      // See if it's HScrollBar
                    //    {
                    //        horisonal.ValueChanged += new EventHandler(gridScrolled);
                    //                                                                    // Got it. Hook into ValueChanged event.
                    //        continue;                                                    // We're done. Terminate.
                    //    }
                    //    if (vert == null && (vert = c as VScrollBar) != null)
                    //    {
                    //        vert.ValueChanged += new EventHandler(gridScrolled);

                    //        continue;
                    //    }

                    //}
                }


                return _hostedControl;
            }
        }

        public override PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return base.PropertyDescriptor;
            }
            set
            {

                base.PropertyDescriptor = value;
                if (this.PropertyDescriptor != null && this.PropertyDescriptor.PropertyType != typeof(object))
                {
                    this._parseMethod = this.PropertyDescriptor.PropertyType.GetMethod("Parse", new Type[] { typeof(string), typeof(IFormatProvider) });
                }
                else
                {
                    this._parseMethod = null;
                }
            }
        }

        void binding_Parse(object sender, ConvertEventArgs e)
        {
            if (e.Value is string && string.IsNullOrEmpty((string)e.Value))
            {
                if (e.DesiredType.IsValueType)
                {
                    object defaultValue = Activator.CreateInstance(e.DesiredType);
                    e.Value = defaultValue;
                }
            }
        }

        void _hostedControl_LostFocus(object sender, EventArgs e)
        {
            
            if (!this.Owner.Focused)
            {
                this.Owner.EndEdit();
                //this.CommitEdit();
                //this.EndEdit();
            }
        }

        

        public override bool ReadOnly
        {
            get
            {
                return this._readOnly;
            }
            set
            {
                if (this._readOnly != value)                                    // New value?
                {
                    this._readOnly = value;                                     // Yes, store it.
                    this.Invalidate();                                          // Update grid.
                }
            }
        }

        public int EditRow { get; private set; }

        protected virtual void OnReadOnlyChanged()
        {

        }

        public new EditableDataGrid Owner
        {
            get
            {
                return base.Owner as EditableDataGrid;
            }
        }


        #endregion

        //protected void OnValidating(object sender, CancelEventArgs e)
        //{
        //    if (Validating != null)
        //    {
        //        Validating(sender, e);
        //    }

        //}

        //protected void OnTextChanged(object sender, EventArgs e)
        //{
        //    if (this.TextChanged != null)
        //    {
        //        this.TextChanged(sender, e);
        //    }
        //}



        protected void OnCellEditBeginning(CellEditEventArgs e)
        {
            if (CellEditBeginning != null)
            {
                CellEditBeginning(this, e);
            }
        }

        protected void HandleCellLostFocus(Object sender, EventArgs e)
        {

        }

        //protected void OnCellEditEnding(CellEditEventArgs e)
        //{
        //    if(CellEditEnding != null)
        //    {
        //        CellEditEnding(this, e);
        //    }
        //}


        #region Methids to be overriden

        // Returns name of the property on hosted control we're going to bind to, e.g. "Text" on TextBox.
        //protected abstract string GetBoundPropertyName();

        // Creates hosted control and sets it's properties as needed.
        protected abstract Control CreateHostedControl();

        //updates hosted control
        protected abstract void UpdateHostedControl(object cellValue);

        //gets the hosted controls current value
        protected abstract object GetHostControlValue();
        

        #endregion

        #region virtual methods 
        internal virtual void Edit(CurrencyManager source, int row, int column)
        {
            

            if (!_isEditing) //if cell is just entering edit 
            {
                CellEditEventArgs e = new CellEditEventArgs(row, column);
                OnCellEditBeginning(e);
                object cellValue = base.GetCellValue(source, row);
                _orgValue = cellValue;
                UpdateHostedControl(cellValue);
                this.HostedControl.Focus();
            }
            _isEditing = true;

            this.EditRow = row;
        }

        internal virtual void UpdateEditCell(Rectangle bounds, bool isVisable)
        {
            isVisable = isVisable && (!this.ReadOnly || this.CanShowHostedControlIfReadOnly);
            if (isVisable)
            {
                this._bounds = GetPreferedBounds(bounds);
                HostedControl.Bounds = _bounds;
                HostedControl.Visible = true;
                //HostedControl.Focus();//for combobox calling focus causes controll to lose focus if already focused                                
            }
            else
            {
                HostedControl.Bounds = Rectangle.Empty;//hides control without affecting focus control
            }
            //this.HostedControl.Refresh();
            //this.HostedControl.Invalidate();
            //this.Owner.Invalidate(bounds);
            
        }

        internal virtual object ConvertValueFromText(String text)
        {
            object value;
            Type propType = this.PropertyDescriptor.PropertyType;
           
            try
            {
                if (!String.IsNullOrEmpty(base.Format) && this._parseMethod != null && this.FormatInfo != null)
                {
                    value = this._parseMethod.Invoke(null, new object[] { text, this.FormatInfo });
                }
                else
                {
                    if (string.IsNullOrEmpty(text))
                    {
                        if (propType.IsValueType)
                        {
                            value = Activator.CreateInstance(propType);
                        }
                        else
                        {
                            value = null;
                        }
                        return value;
                    }
                    value = Convert.ChangeType(text, propType, this.FormatInfo);
                }
            }
            catch
            {
                value = null;
            }
            return value;
        }


        internal virtual void EndEdit()
        {
            _isEditing = false;

            //this.HostedControl.Visible = false;
            Rectangle bounds = this.HostedControl.Bounds;
            this.HostedControl.Bounds = Rectangle.Empty;
            //this.Owner.Invalidate(bounds);
            //this.Owner.UnSelect(this.EditRow);
            this.EditRow = -1; 
        }

        internal virtual void CommitEdit()
        {
            if (this.ReadOnly) { return; }
            
            try
            {
                System.Diagnostics.Debug.Assert(this.EditRow != -1);
                object value = GetHostControlValue();
                bool hasChanges = !CompareValues(_orgValue, value);
                if (hasChanges)
                {
                    bool cancel = NotifyCellValidating(value);
                    if (cancel == true) { return; }

                    
                    base.SetCellValue(this.EditRow, value);
                    this.NotifyCellValueChanged();
                    _orgValue = value;
                }
            }
            catch
            {
                RevertEdit();
            }
        }

        internal virtual bool ConceadeFocus()
        {
            bool isFocused = this.HostedControl.Focused;
            if (isFocused && this.Owner != null)
            {
                this.Owner.Focus();
                return true;
            }
            return false;
        }

        internal virtual void RevertEdit()
        {
            this.UpdateHostedControl(_orgValue);
        }

        internal virtual void AbortEdit()
        {
            this.RevertEdit();
            this.EndEdit();
            this.ConceadeFocus();
        }

        

        protected virtual bool NotifyCellValidating(object value)
        {
            EditableDataGridCellValidatingEventArgs e = new EditableDataGridCellValidatingEventArgs()
            {
                Column = this,
                RowIndex = this.EditRow,
                Value = value,

            };
            //if (this.Validating != null)
            //{
            //    this.Validating(this, e);
            //}
            this.Owner.OnCellValidatingInternal(e);
            return e.Cancel;
        }

        protected virtual void NotifyCellValueChanged()
        {
            
            EditableDataGridCellEventArgs e = new EditableDataGridCellEventArgs()
            {
                Column = this,
                RowIndex = this.EditRow
            };
            this.Owner.OnCellValueChangedInternal(e);
        }
        
        protected virtual Rectangle GetPreferedBounds(Rectangle rc)
        {            
            return rc;
        }

        protected virtual void DestroyHostedControl()
        {
            if (_hostedControl != null)
            {
                this.Owner.Controls.Remove(_hostedControl);
                _hostedControl.Dispose();
                _hostedControl = null;
            }
        }

       

        #endregion

        protected static bool CompareValues(object obj1, object obj2)
        {
            bool objEquals = !(obj2 == null && obj1 != null || obj2 != null && obj1 == null || obj2 != null && !obj1.Equals(obj2) || obj1 != null && !obj2.Equals(obj1));
            //bool compEquals = false;
            //if (obj1 is IComparable)
            //{
            //    compEquals = ((IComparable)obj1).CompareTo(obj2) == 0;
            //}
            //if (!compEquals && obj2 is IComparable)
            //{
            //    compEquals = ((IComparable)obj2).CompareTo(obj1) == 0; 
            //}
            return objEquals;
        }

        #region Protected methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._hostedControl != null)
                {
                    this._hostedControl.Dispose();
                    this._hostedControl = null;
                }
            }

            base.Dispose(disposing);
        }

        
        protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
        {
            
            RectangleF textBounds;                                              // Bounds of text 
            Object cellData;                                                    // Object to show in the cell 
            Object rowData = source.List[rowNum];
            
            cellData = this.PropertyDescriptor.GetValue(source.List[rowNum]);   // Get data for this cell from data source.
            String cellText = FormatText(cellData);

            DrawBackground(g, bounds, rowNum, backBrush);
            IDataErrorInfo errorInfo = rowData as IDataErrorInfo;
            if (Owner is EditableDataGrid &&                                    //Draw ErrorInfo if Data Grid is compatable
                ((EditableDataGrid)Owner).ErrorColor != null &&
                errorInfo != null &&
                string.IsNullOrEmpty(errorInfo[this.MappingName]) == false )
            {
                Brush errorBrush = (Owner as EditableDataGrid).ErrorBrush;
                DrawCellError(g, bounds, errorBrush, foreBrush, errorInfo[MappingName]);
            }

            bounds.Inflate(-2, -2);                                             // Shrink cell by couple pixels for text.

            textBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                                                                                // Set text bounds.
            
            g.DrawString(cellText, this.Owner.Font, foreBrush, textBounds, this.StringFormat);
                                                                                // Render contents 
        
        }

       

        protected virtual void DrawCellError(Graphics g, Rectangle cellBounds, Brush errorBrush, Brush textBrush, String errorMessage)
        {
            Rectangle errorIconBounds = new Rectangle(cellBounds.Right - ERROR_ICON_WIDTH, cellBounds.Top, ERROR_ICON_WIDTH, cellBounds.Height);
            g.FillRectangle(errorBrush, errorIconBounds);
            g.DrawString(" !", Owner.Font, textBrush, errorIconBounds);
        }

      

        protected virtual void DrawBackground(Graphics g, Rectangle bounds, int rowNum, Brush backBrush)
        {
            Brush background = backBrush;                                       // Use default brush by... hmm... default.

            if ((Owner is EditableDataGrid) &&                                   //owner is a editable datagrid
                (((EditableDataGrid)Owner).AlternatingBackBrush != null) &&         //has a alternating brush
                ((rowNum & 1) != 0) &&                                          //row number is odd
                !Owner.IsSelected(rowNum))                                      //and row is not selected
            {
                background = (Owner as EditableDataGrid).AlternatingBackBrush;
            }

            g.FillRectangle(background, bounds);                                // Draw cell background
        }

        #endregion

    }
}
