using System.Windows.Forms;

namespace FMSC.Controls
{
    public class KeyPressDispatcher : IKeyPressProcessor
    {

        protected Control _topControl;
        protected Control _lastFocusedControlCashe;

        public KeyPressDispatcher(Control topControl)
        {
            this._topControl = topControl;
        }
        

        public Control GetFocusedControl()
        {
            //insted of doing another search, lets see if the last focused control is still focused 
            if (_lastFocusedControlCashe != null && _lastFocusedControlCashe.Focused == true) { return _lastFocusedControlCashe; }
            //ok, lets see if any of its parrents children got focus, that is a likly outcome 
            if (_lastFocusedControlCashe != null && _lastFocusedControlCashe.Parent != null)
            {
                _lastFocusedControlCashe = GetFocusedControl(_lastFocusedControlCashe.Parent);
                if (_lastFocusedControlCashe != null && _lastFocusedControlCashe.Focused == true) { return _lastFocusedControlCashe; }
            }
            //nope, so lets start from the top
            _lastFocusedControlCashe = GetFocusedControl(_topControl);
            return _lastFocusedControlCashe;
        }


        protected Control GetFocusedControl(Control topControl)
        {
            foreach (Control childControl in topControl.Controls)
            {
                if (childControl.Focused)
                {
                    return childControl;
                }
            }

            foreach (Control childControl in topControl.Controls)
            {
                Control maybeFocusedControl = GetFocusedControl(childControl);
                if (maybeFocusedControl != null)
                {
                    return maybeFocusedControl;
                }
            }

            return null;
        }

        #region IKeyPressProcessor Members



        public bool ProcessDialogKey(Keys keyVal)
        {
            IKeyPressProcessor kpp = GetFocusedControl() as IKeyPressProcessor;
            if (kpp != null)
            {
                return kpp.ProcessDialogKey(keyVal);
            }
            return false;
        }

        public bool ProcessTabKey()
        {
            IKeyPressProcessor kpp = GetFocusedControl() as IKeyPressProcessor;
            if (kpp != null)
            {
                return kpp.ProcessTabKey();
            }
            return false;
        }

        public bool ProcessReturnKey()
        {
            IKeyPressProcessor kpp = GetFocusedControl() as IKeyPressProcessor;
            if (kpp != null)
            {
                return kpp.ProcessReturnKey();
            }
            return false;
        }


        public bool ProcessKeyPress(Keys keyVal)
        {
            IKeyPressProcessor kpp = GetFocusedControl() as IKeyPressProcessor;
            if (kpp != null)
            {
                return kpp.ProcessKeyPress(keyVal);
            }
            return false;
        }

        

        public bool ProcessBackTabKey()
        {
            IKeyPressProcessor kpp = GetFocusedControl() as IKeyPressProcessor;
            if (kpp != null)
            {
                return kpp.ProcessBackTabKey();
            }
            return false;
        }
        #endregion
    }
}
