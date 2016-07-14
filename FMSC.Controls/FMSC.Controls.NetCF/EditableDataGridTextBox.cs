using System.Windows.Forms;

namespace FMSC.Controls
{
    public class EditableDataGridTextBox : TextBox, IKeyPressProcessor
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