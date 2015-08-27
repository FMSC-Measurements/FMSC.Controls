using System;
using System.Windows.Forms;

namespace FMSC.Controls.Mobile
{
    public partial class ComboBoxRedux : ComboBox//, IKeyPressProcessor
    {
        /// <summary>
        /// Programaticly select text in the textbox portion of the combobox
        /// </summary>
        /// <param name="start"></param>
        /// <param name="length"></param>
        public void Select(int start, int length)
        {
            if (start < 0)
            {
                throw new ArgumentException("InvalidArgument","start");
            }
            else if (length < 0)
            {
                throw new ArgumentException("InvalidArgument","length");
            }
            else
            {
                IntPtr hndl = this.Handle;
                if(hndl != IntPtr.Zero)
                {
                    int high = start + length;
                    FMSC.Controls.Win32.SendMessage(hndl, FMSC.Controls.Win32.CB_SETEDITSEL, 0, FMSC.Controls.Win32.MAKELPARAM(start, high).ToInt32());
                }
            }
        }

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
    }
}
