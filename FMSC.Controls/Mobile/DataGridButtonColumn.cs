using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FMSC.Controls
{
    public delegate void ButtonCellClickEventHandler(ButtonCellClickEventArgs e);

    public class ButtonCellClickEventArgs 
    {
        public object CellValue{ get; set; }

        public DataGrid DataGrid { get; set; }

        public int RowNumber { get; set; }

    }

    public class DataGridButtonColumn : CustomColumnBase, IClickableDataGridColumn
    {
        //protected class LayoutData
        //{
        //    internal int rowNum;
        //    internal Rectangle textBounds;
        //    internal Rectangle cellBounds;
        //    internal Rectangle imageBounds;
        //    internal bool alignToRight;
        //    internal Image cellImage;
        //    internal String cellText;
        //    internal Font font;


        //}

        //protected class ColorData
        //{
        //    internal Color buttonFace;
        //    internal Color buttonShadow;
        //    internal Color buttonShadowDark;
        //    internal Color constrastButtonShadow;
        //    internal Color windowText;
        //    internal Color highlight;
        //    internal Color lowHighlight;
        //    internal Color lowButtonFace;
        //    internal Color windowFrame;
        //    internal Color backColor;
        //    internal Color foreColor;
        //    internal Brush foreBrush;
        //    internal Brush backBrush;
        //    //internal Graphics graphics;
        //    internal bool highContrast;
        //    internal bool enabled;
        //    internal bool disabledTextDim = false;

        //    public float GetBrightness(Color color)
        //    {
        //        float num1 = (float) color.R / (float) byte.MaxValue;
        //        float num2 = (float) color.G / (float) byte.MaxValue;
        //        float num3 = (float) color.B / (float) byte.MaxValue;
        //        float num4 = num1;
        //        float num5 = num1;
        //        if ((double) num2 > (double) num4)
        //            num4 = num2;
        //        if ((double) num3 > (double) num4)
        //            num4 = num3;
        //        if ((double) num2 < (double) num5)
        //            num5 = num2;
        //        if ((double) num3 < (double) num5)
        //            num5 = num3;
        //        return (float) (((double) num4 + (double) num5) / 2.0);
        //    }  


        //    internal ColorData(Color foreColor, Color backColor, bool enabled, bool highContrast)
        //    {
        //        //this.options = options;
        //        this.foreColor = foreColor;
        //        this.backColor = backColor;
        //        this.enabled = enabled;
        //        this.highContrast = highContrast;
        //        this.Calculate();
        //    }

        //    internal static int Adjust255(float percentage, int value)
        //      {
        //        int adjustedVal = (int) ((double) percentage * (double) value);
        //        return (adjustedVal > 255) ? 225 : adjustedVal;
        //      }

        //    internal ColorData Calculate()
        //    {
        //        this.buttonFace = backColor;
        //        if (this.backColor == SystemColors.Control) //if using system colors
        //        {
        //            this.buttonShadow = SystemColors.ControlDark;
        //            this.buttonShadowDark = SystemColors.ControlDarkDark;
        //            this.highlight = SystemColors.ControlLightLight;
        //        }
        //        else if (!this.highContrast)                //else if not using high contrast
        //        {
        //          this.buttonShadow = Dark(this.backColor);
        //          this.buttonShadowDark = DarkDark(this.backColor);
        //          this.highlight = LightLight(this.backColor);
        //        }
        //        else
        //        {
        //          this.buttonShadow = Dark(this.backColor);     //else using high contrast
        //          this.buttonShadowDark = LightLight(this.backColor);
        //          this.highlight = LightLight(this.backColor);
        //        }

        //        float percentage1 = 0.9f;
        //        if ((double) this.GetBrightness(this.buttonFace) < 0.5) //if button face is darkish make it brighter when clicked
        //          percentage1 = 1.2f;
        //        this.lowButtonFace = Color.FromArgb(ColorData.Adjust255(percentage1, (int)this.buttonFace.R), ColorData.Adjust255(percentage1, (int)this.buttonFace.G), ColorData.Adjust255(percentage1, (int)this.buttonFace.B));

        //        float percentage2 = 0.9f;
        //        if ((double) this.GetBrightness(this.highlight) < 0.5)
        //          percentage2 = 1.2f;
        //        this.lowHighlight = Color.FromArgb(ColorData.Adjust255(percentage2, (int)this.highlight.R), ColorData.Adjust255(percentage2, (int)this.highlight.G), ColorData.Adjust255(percentage2, (int)this.highlight.B));

        //        if (this.highContrast && this.backColor != SystemColors.Control)
        //          this.highlight = this.lowHighlight;

        //        this.windowFrame = this.foreColor;                     

        //        this.constrastButtonShadow = (double) this.GetBrightness(this.buttonFace) >= 0.5 ? this.buttonShadow : this.lowHighlight;

        //        this.windowText = this.enabled || !this.disabledTextDim ? this.windowFrame : this.buttonShadow;

        //        //this.buttonFace = this.GetNearestColor(g, this.buttonFace);
        //        //this.buttonShadow = this.GetNearestColor(g, this.buttonShadow);
        //        //this.buttonShadowDark = this.GetNearestColor(g, this.buttonShadowDark);
        //        //this.constrastButtonShadow = this.GetNearestColor(g, this.constrastButtonShadow);
        //        //this.windowText = this.GetNearestColor(g, this.windowText);
        //        //this.highlight = this.GetNearestColor(g, this.highlight);
        //        //this.lowHighlight = this.GetNearestColor(g, this.lowHighlight);
        //        //this.lowButtonFace = this.graphics.GetNearestColor(g, this.lowButtonFace);
        //        //this.windowFrame = this.GetNearestColor(g, this.windowFrame);
        //        return this;
        //    }

           



        //    public static Color Dark(Color baseColor)
        //    {
        //      return new HLSColor(baseColor).Darker(0.5f);
        //    }

        //    public static Color DarkDark(Color baseColor)
        //    {
        //      return new HLSColor(baseColor).Darker(1f);
        //    }

        //    public static Color Light(Color baseColor)
        //    {
        //      return new HLSColor(baseColor).Lighter(0.5f);
        //    }

        //    public static Color LightLight(Color baseColor)
        //    {
               
        //      return new HLSColor(baseColor).Lighter(1f);
        //    }
        //}


        private bool _mouseDown = false;
        private int _mouseRowNum = -1; 

        //private int _columnOrdinal = -1; 

        private PropertyDescriptor _defaultPropertyDescriptor;
        protected PropertyDescriptor DefaultPropertyDescriptor
        {
            get
            {
                if (_defaultPropertyDescriptor == null)
                {
                    _defaultPropertyDescriptor = CreateDefaultPropertyDescriptor();
                }
                return _defaultPropertyDescriptor;
            }
        }

        private Pen _boarderPen; 
        protected Pen BoarderPen
        {
            get
            {
                if (_boarderPen == null && this.Owner != null)
                {
                    this._boarderPen = new Pen(this.Owner.ForeColor);
                }
                return _boarderPen; 
            }

        }


        //protected ColorData Colors { get; set; }

        //private static FieldInfo ownerAccessor;      

        public event ButtonCellClickEventHandler Click ;

        public String Text { get; set; }

        //private StringFormat _stringFormat = null;  

        //public StringFormat StringFormat
        //{
        //    get
        //    {
        //        if (null == _stringFormat)                                              // No format yet?
        //        {
        //            _stringFormat = new StringFormat();                                 // Create one.

        //            this.Alignment = HorizontalAlignment.Left;                          // And set default aligment.
        //        }

        //        return _stringFormat;                                                   // Return our format
        //    }

        //}

        

        public override PropertyDescriptor PropertyDescriptor
        {
            get
            {
                if (UseCellColumnTextForCellValue || string.IsNullOrEmpty(this.MappingName))
                {
                    return null;
                }
                else
                {
                    return base.PropertyDescriptor;
                }
            }
            set
            {
                base.PropertyDescriptor = value;
            }
        }


        public bool UseCellColumnTextForCellValue { get; set; }        

        public DataGridButtonColumn()
        {
          
        }


        //protected LayoutData DoCellLayout(Graphics g, Rectangle cellBounds, int rowNum, bool alignToRight)
        //{
            
        //    LayoutData layoutData = new LayoutData();
        //    layoutData.cellBounds = cellBounds;// face, deflate for border
        //    layoutData.rowNum = rowNum; 
        //    layoutData.alignToRight = alignToRight;
            
        //    layoutData.imageBounds = new Rectangle(layoutData.cellBounds.X, layoutData.cellBounds.Y, layoutData.cellBounds.Width, layoutData.cellBounds.Height);
        //    layoutData.imageBounds.Inflate(-2, -2);

        //    layoutData.font = Owner.Font;

        //    layoutData.cellText = getCellText(rowNum);
        //    //layoutData.textBounds = new Rectangle(layoutData.cellBounds.X, layoutData.cellBounds.Y, layoutData.cellBounds.Width, layoutData.cellBounds.Height);
        //    Size textSize = g.MeasureString(layoutData.cellText, layoutData.font).ToSize();
        //    ContentAlignment alignment = ((StringFormat.Alignment & StringAlignment.Near) != (StringAlignment) 0) ? ContentAlignment.TopLeft : 
        //        ((StringFormat.Alignment & StringAlignment.Far) != (StringAlignment) 0) ? ContentAlignment.TopRight : ContentAlignment.TopCenter;
        //    layoutData.textBounds = CustomColumnBase.AlignContentWithinCell(textSize, layoutData.cellBounds, alignment);
        //    //layoutData.textBounds.Offset(10, 0);


        //    layoutData.cellImage = getCellImage(rowNum);
        //    return layoutData;
        //}

        //protected Image getCellImage(int rowNum)
        //{
        //    //TODO
        //    return null;
        //}

        
        protected bool IsCellEnabled(int rowNumber)
        {
            //TODO
            return true;
        }



        protected override void  Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) { return; }//do not draw if not visable
            DataGrid dg = this.Owner;
            if (dg == null) { return; }

            g.FillRectangle(backBrush, bounds);
            this.PaintBoarder(g, bounds, rowNum);
            
            object obj = this.GetCellValue(rowNum);
            string cellText;
            if(!String.IsNullOrEmpty(this.Format) && obj is IFormattable)
            {
                cellText = ((IFormattable)obj).ToString(this.Format,System.Globalization.CultureInfo.CurrentCulture);
            }
            else
            {
                cellText = this.GetCellValue(rowNum).ToString();
            }

            bounds.Inflate(-3, -2);
            RectangleF textBounds = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            g.DrawString(cellText, dg.Font, foreBrush, textBounds);
        }


        private void PaintBoarder(Graphics g, Rectangle bounds, int rowNum)
        {
            System.Diagnostics.Debug.Assert(!(this._mouseDown && ( this._mouseRowNum == -1)));

            EditableDataGrid grid = this.Owner as EditableDataGrid;
            if (grid == null) { return; }
            Pen boarderPen = grid.ForePen; 

            bool isClicked = (this._mouseDown && (rowNum == this._mouseRowNum));
            if (isClicked)
            {
                this.DrawBorderClicked(g, bounds, boarderPen);
            }
            else
            {
                this.DrawBorderNormal(g, bounds, boarderPen);               
            }
        }


        //protected string getCellText(int rowNum)
        //{
        //    String cellText;
        //    if(!UseCellColumnTextForCellValue)
        //    {
        //        cellText = GetCellValue(rowNum).ToString();
        //    }
        //    else
        //    {
        //        cellText = this.Text;
        //    }
        //    return cellText;
        //}

        public override object GetCellValue(int rowNum)
        {            
            if (string.IsNullOrEmpty(MappingName) || UseCellColumnTextForCellValue)
            {
                return this.Text;
            }
            else
            {
                CurrencyManager source = Owner.BindingContext[this.Owner.DataSource] as CurrencyManager;
                return this.PropertyDescriptor.GetValue(source.List[rowNum]);
            }
        }

        private void DrawBorderClicked(Graphics g, Rectangle bounds, Pen borderPen)
        {

            System.Diagnostics.Debug.Assert(borderPen != null);
            //top
            g.DrawLine(borderPen, bounds.X, bounds.Y + 1, bounds.X + bounds.Width - 1, bounds.Y +1);
            //left
            g.DrawLine(borderPen, bounds.X +1, bounds.Y, bounds.X+1, bounds.Y + bounds.Height - 1);
            ////botom
            //g.DrawLine(borderPen, bounds.X, bounds.Y + bounds.Height - 1, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
            ////right
            //g.DrawLine(borderPen, bounds.X + bounds.Width - 1, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
        }

        private void DrawBorderNormal(Graphics g, Rectangle bounds, Pen borderPen)
        {
            System.Diagnostics.Debug.Assert(borderPen != null);
            ////draw top border shadowed
            //g.DrawLine(borderPen, bounds.X, bounds.Y + 1, bounds.X + bounds.Width - 1, bounds.Y + 1);
            ////draw left border shadowed
            //g.DrawLine(borderPen, bounds.X + 1, bounds.Y, bounds.X + 1, bounds.Y + bounds.Height - 1);
            //draw bottom border, highlighted
            g.DrawLine(borderPen, bounds.X, bounds.Y + bounds.Height - 1, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
            //draw right border, highlighted
            g.DrawLine(borderPen, bounds.X + bounds.Width - 1, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);



        }

        

        protected PropertyDescriptor CreateDefaultPropertyDescriptor()
        {
            PropertyDescriptor pd = TypeDescriptor.GetProperties(typeof(DataGridButtonColumn)).Find("Text", false);
            return pd;
        }


        //#region IClickableDataGridColumn Members

        public void HandleMouseDown(int rowNum, System.Windows.Forms.MouseEventArgs mea)
        {
            this._mouseDown = true;
            this._mouseRowNum = rowNum;
            this.Owner.Invalidate();

            //Graphics g = Owner.CreateGraphics();
            //try
            //{
            //    Rectangle cellBounds = this.Owner.GetCellBounds(rowNum, this.ColumnOrdinal);
            //    LayoutData layoutData = DoCellLayout(g, cellBounds, rowNum, true);
            //    this.PaintDown(g, Colors, layoutData);
            //}
            //catch
            //{ }//do nothing
        }

        public void HandleMouseUp(int rowNum, System.Windows.Forms.MouseEventArgs mea)
        {
            if (_mouseDown)
            {
                this._mouseDown = false;
                this._mouseRowNum = -1;
                this.Owner.Invalidate();
            }
        }


        public void HandleMouseClick(int rowNum)
        {
            if (_mouseDown)
            {
                this._mouseDown = false;
                this._mouseRowNum = -1;
                this.Owner.Invalidate();
            }

            if (this.Click != null)
            {
                this.Click(new ButtonCellClickEventArgs()
                {
                    CellValue = this.GetCellValue(rowNum),
                    DataGrid = this.Owner,
                    RowNumber = rowNum
                });
            }
        }
    }
}

