using System;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;

namespace FMSC.Controls.Mobile
{
    public partial class ComboBoxRedux : ComboBox
    {
        public ComboBoxRedux()
        {
            this.Validated += new EventHandler(this.HandleValidated);
            this.Validating += new System.ComponentModel.CancelEventHandler(HandleValidating);
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
            if (index == -1 ) { return false; }
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


        
    }
    
}
