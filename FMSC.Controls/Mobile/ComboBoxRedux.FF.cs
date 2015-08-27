using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FMSC.Controls.Mobile
{
    public partial class ComboBoxRedux : ComboBox//, IKeyPressProcessor
    {

        //#region IKeyPressProcssor Members

#if FullFramework
        protected override bool  ProcessDialogKey(Keys keyData)
#elif Mobil
        protected virtual bool ProcessDialogKey(Keys keyVal)
#endif
        {
            switch (keyVal)
            {
                case (Keys.Left):
                    {
                        if (this.SelectionStart == 0)
                        {
                            this.Parent.SelectNextControl(this, false, true, true, true);
                        }
                        break;
                    }
                case (Keys.Right):
                    {
                        if (this.SelectionStart == this.Text.Length)
                        {
                            this.Parent.SelectNextControl(this, true, true, true, true);
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

        protected virtual bool ProcessTabKey()
        {
            IKeyPressProcessor p = this.Parent as IKeyPressProcessor;
            if (p == null) { return false; }
            return p.ProcessTabKey();
        }

        protected virtual bool ProcessBackTabKey()
        {
            IKeyPressProcessor p = this.Parent as IKeyPressProcessor;
            if (p == null) { return false; }
            return p.ProcessBackTabKey();
        }

        protected virtual bool ProcessReturnKey()
        {
            IKeyPressProcessor p = this.Parent as IKeyPressProcessor;
            if (p == null) { return false; }
            return p.ProcessReturnKey();
        }

        protected virtual bool ProcessKeyPress(Keys keyVal)
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

        //#endregion

    }
}
