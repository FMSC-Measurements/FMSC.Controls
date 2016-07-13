using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FMSC.Controls
{
    // This is our editable TextBox column.
    public class EditableTextBoxColumn : EditableColumnBase
    {
        #region Fields

        private int _maxTextLength = -1;
        private int _textWidth;

        #endregion Fields

        #region Properties

        protected override bool CanShowHostedControlIfReadOnly
        {
            get { return true; }
        }

        public int MaxTextLength
        {
            get { return this.TextBox.MaxLength; }
            set
            {
                if (base._hostedControl != null)
                {
                    this.TextBox.MaxLength = value;
                }
                this._maxTextLength = value;
            }
        }

        public bool MultiLine { get; set; }

#if NET_CF

        public virtual TextBox TextBox
#else
        public override TextBox TextBox
#endif
        {
            get
            {
                return this.HostedControl as TextBox;
            }
        }

        /// <summary>
        /// If set to true, data grid will cycle to the next column when the length of text in the text box == MaxTextLength
        /// </summary>
        public bool GoToNextColumnWhenTextCompleate { get; set; }

        #endregion Properties

        public EditableTextBoxColumn()
        { }

        #region override methods

        protected override void OnReadOnlyChanged()
        {
            base.OnReadOnlyChanged();
            this.TextBox.ReadOnly = base.ReadOnly;
        }

        protected override Rectangle GetPreferedBounds(Rectangle rc)
        {
            if (_textWidth > rc.Width)
            {
                if (this.MultiLine)
                {
                    rc.Height = rc.Height * ((_textWidth / rc.Width) + 1);
                }
                else
                {
                    rc.Width = _textWidth;
                }
            }
            return rc;
        }

        internal override void CommitEdit()
        {
            base.CommitEdit();
            //string text = this.TextBox.Text;
            //try
            //{
            //    object value = base.ConvertValueFromText(text);
            //    base.SetCellValue(base._row, value);
            //    base._preValue = value;
            //}
            //catch
            //{
            //    this.UpdateHostedControl(_preValue);
            //}
        }

        protected override object GetHostControlValue()
        {
            try
            {
                return base.ConvertValueFromText(this.TextBox.Text);
            }
            catch
            {
                return string.Empty;
            }
        }

        internal override void Edit(CurrencyManager source, int row, int col)
        {
            base.Edit(source, row, col);
            this.TextBox.SelectAll();
        }

        protected override void UpdateHostedControl(object cellValue)
        {
            this.TextBox.Text = base.FormatText(cellValue);
        }

        protected override Control CreateHostedControl()
        {
            TextBox box = new EditableDataGridTextBox();                                            // Our hosted control is a TextBox

            box.BorderStyle = BorderStyle.None;                                     // It has no border
            box.Multiline = this.MultiLine;
            box.ReadOnly = this.ReadOnly;

            box.AcceptsReturn = false;
            box.TextAlign = this.Alignment;                                         // Set up aligment.
            if (this._maxTextLength != -1) { box.MaxLength = _maxTextLength; }
            WireTextBoxEvents(box);

            return box;
        }

        protected override void DestroyHostedControl()
        {
            if (base._hostedControl != null)
            {
                UnwireTextBoxEvents(this.TextBox);
            }
            base.DestroyHostedControl();
        }

        #endregion override methods

        protected void WireTextBoxEvents(TextBox textbox)
        {
            textbox.TextChanged += new EventHandler(TextBox_TextChanged);
        }

        protected void UnwireTextBoxEvents(TextBox textBox)
        {
            textBox.TextChanged -= TextBox_TextChanged;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            EditableDataGrid dg = this.Owner as EditableDataGrid;
            if (dg != null)
            {
                _textWidth = Win32.MeasureTextWidth(dg, this.TextBox.Text);
                if (this.GoToNextColumnWhenTextCompleate &&
                  this.TextBox.TextLength == this.TextBox.MaxLength)
                {
                    dg.SelectNextCell(true);
                }
            }
            else
            {
                _textWidth = -1;
            }
        }

        private class EditableDataGridTextBox : TextBox, IKeyPressProcessor
        {
            protected override void OnKeyDown(KeyEventArgs e)
            {
                e.Handled = this.ProcessKeyPress(e.KeyData);
                if (e.Handled == false)
                {
                    base.OnKeyDown(e);
                }
            }

            #region IKeyPressProcssor Members

            public bool ProcessDialogKey(Keys keyVal)
            {
                switch (keyVal)
                {
                    case (Keys.Left):
                        {
                            if (this.SelectionStart == 0)
                            {
                                EditableDataGrid dg = this.Parent as EditableDataGrid;
                                if (dg != null)
                                {
                                    return dg.MoveSeclection(Direction.Left);
                                }
                            }
                            break;
                        }
                    case (Keys.Right):
                        {
                            if (this.SelectionStart == this.Text.Length)
                            {
                                EditableDataGrid dg = this.Parent as EditableDataGrid;
                                if (dg != null)
                                {
                                    return dg.MoveSeclection(Direction.Right);
                                }
                            }
                            break;
                        }
                    case (Keys.Up):
                        {
                            EditableDataGrid dg = this.Parent as EditableDataGrid;
                            if (dg != null)
                            {
                                return dg.MoveSeclection(Direction.Up);
                            }
                            break;
                        }
                    case (Keys.Down):
                        {
                            EditableDataGrid dg = this.Parent as EditableDataGrid;
                            if (dg != null)
                            {
                                return dg.MoveSeclection(Direction.Down);
                            }
                            break;
                        }
                    default:
                        {
                            IKeyPressProcessor p = this.Parent as IKeyPressProcessor;
                            if (p == null) { return false; }
                            return p.ProcessDialogKey(keyVal);
                        }
                }
                return false;
            }

            public bool ProcessTabKey()
            {
                IKeyPressProcessor p = this.Parent as IKeyPressProcessor;
                if (p == null) { return false; }
                return p.ProcessTabKey();
            }

            public bool ProcessBackTabKey()
            {
                IKeyPressProcessor p = this.Parent as IKeyPressProcessor;
                if (p == null) { return false; }
                return p.ProcessBackTabKey();
            }

            public bool ProcessReturnKey()
            {
                return this.ProcessTabKey();
            }

            public bool ProcessKeyPress(Keys keyVal)
            {
                switch (keyVal)
                {
                    case (Keys.Up):
                    case (Keys.Down):
                    case (Keys.Left):
                    case (Keys.Right):
                        {
                            return this.ProcessDialogKey(keyVal);
                        }
                    default:
                        {
                            IKeyPressProcessor p = this.Parent as IKeyPressProcessor;
                            if (p == null) { return false; }
                            return p.ProcessKeyPress(keyVal);
                        }
                }
            }

            #endregion IKeyPressProcssor Members
        }
    }
}