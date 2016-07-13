using System;
using System.Drawing;
using System.Windows.Forms;

namespace FMSC.Controls
{
    public class ButtonPanel : UserControl
    {
        private ColorData _colors;
        private Font _font = new Font(String.Empty, 9, FontStyle.Regular);
        private Image _image;
        private bool _mouseDown = false;
        private ContentAlignment _textAllign = ContentAlignment.TopCenter;

        //deimplement panel autoScroll feature
        public override bool AutoScroll
        {
            get
            {
                return false;
            }
            set
            {
                throw new NotImplementedException("AutoScroll");
            }
        }

        public Image BackgroundImage
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }

        public ImageLayout ButtonImageLayout { get; set; }

        public override Font Font
        {
            get
            {
                return _font;
            }
            set
            {
                _font = value;
            }
        }

        public int ImageIndex { get; set; }

        public ImageList ImageList { get; set; }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        public ContentAlignment TextAllign
        {
            get { return _textAllign; }
            set { _textAllign = value; }
        }

        protected ColorData Colors
        {
            get
            {
                if (_colors == null)
                {
                    _colors = new ColorData(ForeColor, BackColor, true, false);
                }
                return _colors;
            }
        }

        internal static Rectangle CalculateBackgroundImageRectangle(Rectangle bounds, Image backgroundImage, ImageLayout imageLayout)
        {
            Rectangle rectangle = bounds;
            if (backgroundImage != null)
            {
                switch (imageLayout)
                {
                    case ImageLayout.None:
                        rectangle.Size = backgroundImage.Size;
                        break;
                    case ImageLayout.Center:
                        rectangle.Size = backgroundImage.Size;
                        Size size1 = bounds.Size;
                        if (size1.Width > rectangle.Width)
                            rectangle.X = (size1.Width - rectangle.Width) / 2;
                        if (size1.Height > rectangle.Height)
                        {
                            rectangle.Y = (size1.Height - rectangle.Height) / 2;
                            break;
                        }
                        else
                            break;
                    case ImageLayout.Stretch:
                        rectangle.Size = bounds.Size;
                        break;
                    case ImageLayout.Zoom:
                        Size size2 = backgroundImage.Size;
                        float num1 = (float)bounds.Width / (float)size2.Width;
                        float num2 = (float)bounds.Height / (float)size2.Height;
                        if ((double)num1 < (double)num2)
                        {
                            rectangle.Width = bounds.Width;
                            rectangle.Height = (int)((double)size2.Height * (double)num1 + 0.5);
                            if (bounds.Y >= 0)
                            {
                                rectangle.Y = (bounds.Height - rectangle.Height) / 2;
                                break;
                            }
                            else
                                break;
                        }
                        else
                        {
                            rectangle.Height = bounds.Height;
                            rectangle.Width = (int)((double)size2.Width * (double)num2 + 0.5);
                            if (bounds.X >= 0)
                            {
                                rectangle.X = (bounds.Width - rectangle.Width) / 2;
                                break;
                            }
                            else
                                break;
                        }
                }
            }
            return rectangle;
        }

        internal static void DrawBackgroundImage(Graphics g, Image backgroundImage, Color backColor, ImageLayout backgroundImageLayout, Rectangle bounds, Rectangle clipRect, Point scrollOffset)
        {
            if (g == null)
                throw new ArgumentNullException("g");
            if (backgroundImageLayout == ImageLayout.Tile)
            {
                //throw new NotImplementedException("ImageLayout.Tile Not supported");
                using (TextureBrush textureBrush = new TextureBrush(backgroundImage))
                {
                    g.FillRectangle((Brush)textureBrush, bounds);
                }
            }
            else
            {
                Rectangle rectangle1 = CalculateBackgroundImageRectangle(bounds, backgroundImage, backgroundImageLayout);
                //if (rightToLeft == RightToLeft.Yes && backgroundImageLayout == ImageLayout.None)
                //    rectangle1.X += clipRect.Width - rectangle1.Width;
                //using (SolidBrush solidBrush = new SolidBrush(backColor))
                //    g.FillRectangle((Brush)solidBrush, clipRect);
                if (!clipRect.Contains(rectangle1))
                {
                    if (backgroundImageLayout == ImageLayout.Stretch || backgroundImageLayout == ImageLayout.Zoom)
                    {
                        rectangle1.Intersect(clipRect);
                        //g.DrawImage(backgroundImage, rectangle1);
                        g.DrawImage(backgroundImage, rectangle1, new Rectangle(0, 0, backgroundImage.Width, backgroundImage.Height), GraphicsUnit.Pixel);
                    }
                    else if (backgroundImageLayout == ImageLayout.None)
                    {
                        rectangle1.Offset(clipRect.Location.X, clipRect.Location.Y);
                        Rectangle destRect = rectangle1;
                        destRect.Intersect(clipRect);
                        Rectangle rectangle2 = new Rectangle(0, 0, destRect.Width, destRect.Height);//new Rectangle(Point.Empty, destRect.Size);
                        //g.DrawImage(backgroundImage, destRect, rectangle2.X, rectangle2.Y, rectangle2.Width, rectangle2.Height, GraphicsUnit.Pixel);
                        g.DrawImage(backgroundImage, destRect, rectangle2, GraphicsUnit.Pixel);
                    }
                    else
                    {
                        Rectangle destRect = rectangle1;
                        destRect.Intersect(clipRect);
                        Rectangle rectangle2 = new Rectangle(destRect.X - rectangle1.X, destRect.Y - rectangle1.Y, destRect.Width, destRect.Height);
                        g.DrawImage(backgroundImage, destRect, rectangle2, GraphicsUnit.Pixel);
                    }
                }
                else
                {
                    //ImageAttributes imageAttr = new ImageAttributes();
                    //imageAttr.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(backgroundImage, rectangle1, new Rectangle(0, 0, backgroundImage.Width, backgroundImage.Height), GraphicsUnit.Pixel);
                    //imageAttr.Dispose();
                }
            }
        }

        protected static Rectangle AlignContentWithinCell(Size alignThis, Rectangle withinThis, ContentAlignment align)
        {
            if ((align & ContentAlignment.TopRight) == ContentAlignment.TopRight)       //allign top right
                withinThis.X += withinThis.Width - alignThis.Width;                     //offset X position = orig X pos + dWidth
            else if ((align & ContentAlignment.TopCenter) == ContentAlignment.TopCenter)//allign top center
                withinThis.X += (withinThis.Width - alignThis.Width) / 2;               //offset X pos = orig X pos + (dWidth / 2)

            //if left allign we don't need to do anything

            withinThis.Width = alignThis.Width;
            return withinThis;
        }

        protected LayoutData DoCellLayout(Graphics g, Rectangle bounds)
        {
            LayoutData layoutData = new LayoutData();
            layoutData.cellBounds = bounds;
            layoutData.faceBounds = bounds;
            layoutData.faceBounds.Inflate(-2, -2);//deflate for boarders

            Size textSize = g.MeasureString(this.Text, this.Font).ToSize();
            layoutData.textBounds = layoutData.faceBounds;
            layoutData.textBounds.Inflate(-1, -1);//pad the text area
            layoutData.textBounds = AlignContentWithinCell(textSize, layoutData.textBounds, this.TextAllign);

            return layoutData;
        }

        protected void DrawBorder(Graphics g, Rectangle bounds, ColorData colors, bool raised)
        {
            if (raised)
            {
                this.DrawBorderRaised(g, ref bounds, colors);
            }
            else
            {
                this.DrawBorderNormal(g, ref bounds, colors);
            }
        }

        protected void DrawImage(Graphics graphics, ref Rectangle bounds, Image image, int xLoc, int yLoc)
        {
            DrawBackgroundImage(graphics, image, Colors.backColor, this.ButtonImageLayout, bounds, this.ClientRectangle, Point.Empty);
            //graphics.DrawImage(image, layout.imageBounds, new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            //graphics.DrawImage(image, xLoc, yLoc);
        }

        protected void DrawImageDisabled(Graphics graphics, ref Rectangle bounds, Image image, int xLoc, int yLoc)
        {
            DrawBackgroundImage(graphics, image, Colors.backColor, this.ButtonImageLayout, bounds, this.ClientRectangle, Point.Empty);
            //DrawImage(graphics, image, xLoc, yLoc);
        }

        protected override void OnClick(EventArgs e)
        {
            if (_mouseDown)
            {
                _mouseDown = false;
                this.Invalidate();
                base.OnClick(e);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            this.Invalidate();
            base.OnGotFocus(e);
        }

        //public event EventHandler Click;
        protected override void OnMouseDown(MouseEventArgs e)
        {
            //here we have to set a mouseDown flag because
            //otherwise we wont know whether to draw the button up or down
            if (e.Button == MouseButtons.Left)
            {
                _mouseDown = true;

                if (!this.Focused)
                {
                    this.Focus();//give ourself focuse be cause we are just a panel
                }
                else
                {
                    this.Invalidate();
                }
            }
            base.OnMouseDown(e);
        }

        //private static FieldInfo ownerAccessor;
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_mouseDown)
            {
                _mouseDown = false;
                this.Invalidate();
                base.OnClick(EventArgs.Empty);
                base.OnMouseUp(e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            LayoutData layoutData = DoCellLayout(e.Graphics, this.ClientRectangle);

            using (Brush background = new SolidBrush(Colors.backColor))
            {
                this.PaintButtonBackground(e.Graphics, background, layoutData);
            }

            this.DrawButton(e.Graphics, Colors, layoutData, !this._mouseDown);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnTextChanged(e);
        }

        protected void PaintButtonBackground(Graphics g, Brush background, LayoutData layout)
        {
            g.FillRectangle(background, layout.cellBounds);
        }

        protected void PaintField(Graphics g, string text, LayoutData layout, ColorData colors, bool drawFocus)
        {
            using (Brush foreBrush = new SolidBrush(colors.foreColor))
            {
                g.DrawString(text, this.Font, foreBrush, layout.textBounds);
            }
        }

        //TODO clean up stale code
        private void DrawBorderNormal(Graphics g, ref Rectangle bounds, ColorData colors)
        {
            Pen pen = new Pen(colors.buttonShadowDark, 2);
            //draw top border shadowed
            g.DrawLine(pen, bounds.X, bounds.Y + 1, bounds.X + bounds.Width - 1, bounds.Y + 1);
            //draw left border shadowed
            g.DrawLine(pen, bounds.X + 1, bounds.Y, bounds.X + 1, bounds.Y + bounds.Height - 1);
            pen.Color = colors.highlight;
            //draw bottom border, highlighted
            g.DrawLine(pen, bounds.X, bounds.Y + bounds.Height - 1, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
            //draw right border, highlighted
            g.DrawLine(pen, bounds.X + bounds.Width - 1, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
            //pen.Color = colors.buttonFace;
            //g.DrawLine(pen, bounds.X + 1, bounds.Y + 1, bounds.X + bounds.Width - 2, bounds.Y + 1);
            //g.DrawLine(pen, bounds.X + 1, bounds.Y + 1, bounds.X + 1, bounds.Y + bounds.Height - 2);
            ////int num1 = (int) colors.buttonFace.ToKnownColor();
            ////int num2 = (int) SystemColors.Control.ToKnownColor();
            //pen.Color = colors.buttonFace != SystemColors.Control ? colors.buttonFace : SystemColors.ControlLight;
            //g.DrawLine(pen, bounds.X + 1, bounds.Y + bounds.Height - 2, bounds.X + bounds.Width - 2, bounds.Y + bounds.Height - 2);
            //g.DrawLine(pen, bounds.X + bounds.Width - 2, bounds.Y + 1, bounds.X + bounds.Width - 2, bounds.Y + bounds.Height - 2);
            pen.Dispose();
        }

        private void DrawBorderRaised(Graphics g, ref Rectangle bounds, ColorData colors)
        {
            bool flag = colors.buttonFace == SystemColors.Control;
            Pen pen = flag ? new Pen(SystemColors.ControlLightLight, 2) : new Pen(colors.highlight, 2);
            //top
            g.DrawLine(pen, bounds.X, bounds.Y + 1, bounds.X + bounds.Width - 1, bounds.Y + 1);
            //left
            g.DrawLine(pen, bounds.X + 1, bounds.Y, bounds.X + 1, bounds.Y + bounds.Height - 1);
            if (flag)
            {
                pen.Color = SystemColors.ControlDarkDark;
            }
            else
            {
                pen.Color = colors.buttonShadowDark;
            }
            //botom
            g.DrawLine(pen, bounds.X, bounds.Y + bounds.Height - 1, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
            //right
            g.DrawLine(pen, bounds.X + bounds.Width - 1, bounds.Y, bounds.X + bounds.Width - 1, bounds.Y + bounds.Height - 1);
            //if (flag)
            //{
            //    pen.Color = !colors.highContrast ? SystemColors.Control : SystemColors.ControlLight;
            //}
            //else
            //{
            //    pen.Color = colors.buttonFace;
            //}
            //g.DrawLine(pen, bounds.X + 1, bounds.Y + 1, bounds.X + bounds.Width - 2, bounds.Y + 1);
            //g.DrawLine(pen, bounds.X + 1, bounds.Y + 1, bounds.X + 1, bounds.Y + bounds.Height - 2);
            //if (flag)
            //{
            //    pen.Color = SystemColors.ControlDark;
            //}
            //else
            //{
            //    pen.Color = colors.buttonShadow;
            //}
            //g.DrawLine(pen, bounds.X + 1, bounds.Y + bounds.Height - 2, bounds.X + bounds.Width - 2, bounds.Y + bounds.Height - 2);
            //g.DrawLine(pen, bounds.X + bounds.Width - 2, bounds.Y + 1, bounds.X + bounds.Width - 2, bounds.Y + bounds.Height - 2);
            //if (flag)
            //{
            //    return;
            //}
            pen.Dispose();
        }

        private void DrawButton(Graphics g, ColorData colorData, LayoutData layout, bool up)
        {
            Image img = this.GetImage();
            if (img != null)
            {
                if (this.Enabled)
                {
                    this.DrawImage(g, ref layout.faceBounds, img, layout.faceBounds.X, layout.faceBounds.Y);
                }
                else
                {
                    this.DrawImageDisabled(g, ref layout.faceBounds, img, layout.faceBounds.X, layout.faceBounds.Y);
                }
            }

            string text = this.Text;
            //MatchCollection matches = Regex.Matches(this.Text, @"<\w*>", RegexOptions.Compiled);
            //if (matches.Count > 0)
            //{
            //    Rectangle[] rectangles = new Rectangle[matches.Count];
            //    int recW = layout.textBounds.Width / matches.Count;
            //    for (int i = 0; i < matches.Count; i++)
            //    {
            //        Match m = matches[i];
            //        rectangles[i] = new Rectangle(recW * i + layout.textBounds.X, layout.textBounds.Y, recW, layout.textBounds.Height);
            //        string val = m.Value.Trim().Trim('<', '>');

            //        Image mImg = GetImage(Convert.ToInt32(val));
            //        this.DrawImage(g, ref rectangles[i], mImg, rectangles[i].X, rectangles[i].Y);
            //        text = text.Replace(m.Value, string.Empty);
            //    }
            //}

            this.PaintField(g, text, layout, colorData, true);

            this.DrawBorder(g, layout.cellBounds, colorData, up);
            //else
            //  ControlPaint.DrawBorder(graphics, clientRectangle, colors.buttonShadow, ButtonBorderStyle.Solid);
        }

        private Image GetImage(int imgIndex)
        {
            if (ImageList != null &&
                    (imgIndex >= 0 && imgIndex < ImageList.Images.Count))
            {
                return ImageList.Images[imgIndex];
            }
            return _image;
        }

        private Image GetImage()
        {
            return GetImage(this.ImageIndex);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            //
            // ButtonPanel
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Name = "ButtonPanel";
            this.Size = new System.Drawing.Size(75, 20);
            this.ResumeLayout(false);
        }

        protected struct LayoutData
        {
            internal Rectangle cellBounds;
            internal Rectangle faceBounds;
            internal Rectangle textBounds;
        }

        protected class ColorData
        {
            internal Color backColor;
            internal Color buttonFace;
            internal Color buttonShadow;
            internal Color buttonShadowDark;
            internal Color constrastButtonShadow;
            internal bool disabledTextDim = false;
            internal bool enabled;
            internal Color foreColor;

            //internal Brush foreBrush;
            //internal Brush backBrush;
            //internal Graphics graphics;
            internal bool highContrast;

            internal Color highlight;
            internal Color lowButtonFace;
            internal Color lowHighlight;
            internal Color windowFrame;
            internal Color windowText;

            internal ColorData(Color foreColor, Color backColor, bool enabled, bool highContrast)
            {
                //this.options = options;
                this.foreColor = foreColor;
                this.backColor = backColor;
                this.enabled = enabled;
                this.highContrast = highContrast;
                this.Calculate();
            }

            public static Color Dark(Color baseColor, float percOfDarkDark)
            {
                return new HLSColor(baseColor).Darker(percOfDarkDark);
            }

            public static Color Dark(Color baseColor)
            {
                return new HLSColor(baseColor).Darker(0.5f);
            }

            public static Color DarkDark(Color baseColor)
            {
                return new HLSColor(baseColor).Darker(1f);
            }

            public static Color Light(Color baseColor)
            {
                return new HLSColor(baseColor).Lighter(0.5f);
            }

            public static Color LightLight(Color baseColor)
            {
                return new HLSColor(baseColor).Lighter(1f);
            }

            public float GetBrightness(Color color)
            {
                float num1 = (float)color.R / (float)byte.MaxValue;
                float num2 = (float)color.G / (float)byte.MaxValue;
                float num3 = (float)color.B / (float)byte.MaxValue;
                float num4 = num1;
                float num5 = num1;
                if ((double)num2 > (double)num4)
                    num4 = num2;
                if ((double)num3 > (double)num4)
                    num4 = num3;
                if ((double)num2 < (double)num5)
                    num5 = num2;
                if ((double)num3 < (double)num5)
                    num5 = num3;
                return (float)(((double)num4 + (double)num5) / 2.0);
            }

            internal static int Adjust255(float percentage, int value)
            {
                int adjustedVal = (int)((double)percentage * (double)value);
                return (adjustedVal > 255) ? 225 : adjustedVal;
            }

            internal ColorData Calculate()
            {
                this.buttonFace = backColor;
                if (this.backColor == SystemColors.Control) //if using system colors
                {
                    this.buttonShadow = SystemColors.ControlDark;
                    this.buttonShadowDark = SystemColors.ControlDarkDark;
                    this.highlight = SystemColors.ControlLightLight;
                }
                else if (!this.highContrast)                //else if not using high contrast
                {
                    this.buttonShadow = Dark(this.backColor);
                    this.buttonShadowDark = DarkDark(this.backColor);
                    this.highlight = LightLight(this.backColor);
                }
                else
                {
                    this.buttonShadow = Dark(this.backColor);     //else using high contrast
                    this.buttonShadowDark = LightLight(this.backColor);
                    this.highlight = LightLight(this.backColor);
                }

                float percentage1 = 0.9f;
                if ((double)this.GetBrightness(this.buttonFace) < 0.5) //if button face is darkish make it brighter when clicked
                    percentage1 = 1.2f;
                this.lowButtonFace = Color.FromArgb(ColorData.Adjust255(percentage1, (int)this.buttonFace.R), ColorData.Adjust255(percentage1, (int)this.buttonFace.G), ColorData.Adjust255(percentage1, (int)this.buttonFace.B));

                float percentage2 = 0.9f;
                if ((double)this.GetBrightness(this.highlight) < 0.5)
                    percentage2 = 1.2f;
                this.lowHighlight = Color.FromArgb(ColorData.Adjust255(percentage2, (int)this.highlight.R), ColorData.Adjust255(percentage2, (int)this.highlight.G), ColorData.Adjust255(percentage2, (int)this.highlight.B));

                if (this.highContrast && this.backColor != SystemColors.Control)
                    this.highlight = this.lowHighlight;

                this.windowFrame = this.foreColor;

                this.constrastButtonShadow = (double)this.GetBrightness(this.buttonFace) >= 0.5 ? this.buttonShadow : this.lowHighlight;

                this.windowText = this.enabled || !this.disabledTextDim ? this.windowFrame : this.buttonShadow;

                //this.buttonFace = this.GetNearestColor(g, this.buttonFace);
                //this.buttonShadow = this.GetNearestColor(g, this.buttonShadow);
                //this.buttonShadowDark = this.GetNearestColor(g, this.buttonShadowDark);
                //this.constrastButtonShadow = this.GetNearestColor(g, this.constrastButtonShadow);
                //this.windowText = this.GetNearestColor(g, this.windowText);
                //this.highlight = this.GetNearestColor(g, this.highlight);
                //this.lowHighlight = this.GetNearestColor(g, this.lowHighlight);
                //this.lowButtonFace = this.graphics.GetNearestColor(g, this.lowButtonFace);
                //this.windowFrame = this.GetNearestColor(g, this.windowFrame);
                return this;
            }
        }
    }
}