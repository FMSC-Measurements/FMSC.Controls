using System;
using System.Collections;
using System.Windows.Forms;

namespace FMSC.Controls.Mobile
{
    public partial class ComboBoxRedux : ComboBox
    {
        public ComboBoxRedux()
        {
            this.Validated += new EventHandler(this.HandleValidated);
            this.Validating += new System.ComponentModel.CancelEventHandler(HandleValidating);
        }

        public bool DroppedDown
        {
            get
            {
                IntPtr hndl = this.Handle;
                if (hndl == IntPtr.Zero) { return false; } //handle has not been created
                return FMSC.Controls.Win32.SendMessage(hndl, FMSC.Controls.Win32.CB_GETDROPPEDSTATE, 1, 0) != 0;
            }
            set
            {
                FMSC.Controls.Win32.SendMessage(this.Handle, FMSC.Controls.Win32.CB_SHOWDROPDOWN, value ? 1 : 0, 0);
            }
        }

        public new object SelectedItem
        {
            get
            {
                string itemText = this.Text;
                if (String.IsNullOrEmpty(itemText))
                {
                    return null;
                }
                else
                {
                    object value;
                    if (this.GetValueFromItemText(itemText, out value))
                    {
                        return value;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            set
            {
                base.SelectedItem = value;
            }
        }

#if NetCF_20

        /// <summary>
        /// Gets and Sets the location of the cursor in the textbox portion of the combobox
        /// </summary>
        public int SelectionStart
        {
            get
            {
                IntPtr hndl = this.Handle;
                int wpram = 0;
                int returnval;
                if (hndl == IntPtr.Zero) { return 0; }
                {
                    returnval = FMSC.Controls.Win32.SendMessage(hndl, 320, wpram, 0);// close, first few bits of return is the actual index perhaps it has something to do with textbox max size of ?? 32766 ( look it up!)
                    returnval = returnval & 0xFFFF;         //use 16 bit mask to get actual value
                }
                return returnval;
            }

            set
            {
                this.Select(value, 0);
            }
        }

#endif

        public int FindItem(string s, bool ignoreCase)
        {
            IList items = (IList)this.Items;
            for (int i = 0; i < items.Count; i++)
            {
                if (String.Compare(this.GetItemText(items[i]), s, ignoreCase) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool GetValueFromItemText(String displayValue, out object itemValue)
        {
            itemValue = null;
            int index = this.FindItem(displayValue, true);
            if (index == -1) { return false; }
            object item = base.Items[index];
            if (!String.IsNullOrEmpty(this.ValueMember))
            {
                itemValue = base.FilterItemOnProperty(item, this.ValueMember);
            }
            else
            {
                itemValue = item;
            }
            return true;
        }

        private void HandleValidated(object sender, EventArgs e)
        {
            this.OnValidated(e);
        }

        void HandleValidating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.OnValidating(e);
        }

        protected virtual void OnValidated(EventArgs e)
        {
            //do nothing
        }

        protected virtual void OnValidating(EventArgs e)
        {
            //do nothing
        }

#if NetCF_20

        /// <summary>
        /// Programaticly select text in the textbox portion of the combobox
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        public void Select(int start, int length)
        {
            if (start < 0)
            {
                throw new ArgumentException("InvalidArgument", "start");
            }
            else if (length < 0)
            {
                throw new ArgumentException("InvalidArgument", "length");
            }
            else
            {
                IntPtr hndl = this.Handle;
                if (hndl != IntPtr.Zero)
                {
                    int high = start + length;
                    FMSC.Controls.Win32.SendMessage(hndl, FMSC.Controls.Win32.CB_SETEDITSEL, 0, FMSC.Controls.Win32.MAKELPARAM(start, high).ToInt32());
                }
            }
        }

#endif
    }
}