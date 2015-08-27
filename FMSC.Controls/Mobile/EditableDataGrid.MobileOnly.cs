using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.WindowsCE.Forms;

namespace FMSC.Controls
{
    public partial class EditableDataGrid
    {
        private static FieldInfo _vertScrollBarAccessor;
        private static FieldInfo _horzScrollBarAccessor;
        
        //private static FieldInfo _currencyManagerAccessor;
        //private static FieldInfo _propertyDiscriptorCollectionAccessor;
        private static FieldInfo _gridRecAccessor;
        private static FieldInfo _gridRendererAccessor;
        private static FieldInfo _rowHeightAccessor;
        private static FieldInfo _listManagerAccessor; 
        
        
        private Color _alternatingBackColor;

        private InputPanel _sip;
        public ScrollBar VertScrollBar { get { return (ScrollBar)_vertScrollBarAccessor.GetValue(this); } }
        public ScrollBar HorizScrollBar { get { return (ScrollBar)_horzScrollBarAccessor.GetValue(this); } }

        private static void MobileOnlyStaticInit()
        {
            _firstRowVisableAccessor = typeof(EditableDataGrid).GetField("m_irowVisibleFirst", BindingFlags.NonPublic | BindingFlags.Instance);
            _horzScrollBarAccessor = typeof(EditableDataGrid).GetField("m_sbHorz", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            _vertScrollBarAccessor = typeof(EditableDataGrid).GetField("m_sbVert", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            _tableStyleAccessor = typeof(EditableDataGrid).GetField("m_tabstyActive", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            _rowHeightAccessor = typeof(EditableDataGrid).GetField("m_cyRow", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            _gridRendererAccessor = typeof(EditableDataGrid).GetField("m_renderer", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            _listManagerAccessor = typeof(EditableDataGrid).GetField("m_cmData", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            Type gridRenderer = (_gridRendererAccessor != null) ? _gridRendererAccessor.FieldType : null;
            if (gridRenderer != null)
            {
                _gridRecAccessor = gridRenderer.GetField("m_rcGrid", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            }
        }


        /// <summary>
        /// Gets the area of the data grid where rows can be displayed
        /// </summary>
        public Rectangle GridRec
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

        /// <summary>
        /// gets the internal Row Height
        /// </summary>
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

        internal SolidBrush AlternatingBackBrush { get; private set; }

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

        

        //protected override void OnPaint(PaintEventArgs pea)
        //{
        //    base.OnPaint(pea);
        //    if (this.AllowUserToAddRows == false) { return; }
        //    Rectangle gRec = this.GridRec;
            
        //    if(gRec != Rectangle.Empty)
        //    {
        //        if (base.VisibleRowCount > 0)
        //        {
        //            int hRow = this.RowHeight + 2;
        //            int hDif = base.VisibleRowCount * hRow;
        //            if (hDif < gRec.Height)
        //            {
        //                gRec = new Rectangle(gRec.X, gRec.Y + hDif, gRec.Width, gRec.Height - hDif);
        //            }
        //            else
        //            {
        //                return;
        //            }
        //        }
        //        using (SolidBrush foreBrush = new SolidBrush(this.HeaderForeColor))
        //        {
        //            //pea.Graphics.FillRectangle(foreBrush, gRec);                    
        //            pea.Graphics.DrawString("Click Here To Add new Row", this.Font, this.AltTextBrush, gRec);
        //        }
                
        //    }
        
        //}




        void OnSIPDisplayed(object sender, EventArgs e)
        {
            this.EnsureRowVisible(this.CurrentRowIndex);
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

        //HACK Code generator automaticly puts in calls to 
        //theses methods and can't be fixed, so these methods
        //must be included but do nothing.
        #region ISupportInitialize Members

        public void BeginInit()
        {

        }

        public void EndInit()
        {

        }

        #endregion
    }
}
