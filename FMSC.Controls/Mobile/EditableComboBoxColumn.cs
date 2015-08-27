using System;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;


namespace FMSC.Controls
{
    public class EditableComboBoxColumn : EditableColumnBase
    {
        #region Fields 
        private string _displayMember;
        private string _valueMember;
        private object _dataSource;
        #endregion

        #region Events
        public event EventHandler SelectedValueChanged;
        //public event EventHandler SelectedIndexChanged;
        #endregion

        #region Properties
        public object DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                if(_dataSource == value) { return; }
                _dataSource = value;
                if (_hostedControl != null)
                {
                    try
                    {
                        //((EditableDataGridComboBox)EditComboBox).SetDatasource(_dataSource);
                        //EditComboBox.DataSource = _dataSource;
                        if (value is IList)
                        {
                            EditComboBox.SetComboBoxItems((IList)value);
                        }
                        else
                        {
                            EditComboBox.SetComboBoxItems(new Object[] { value });
                        }
                    }
                    catch
                    {
                        _dataSource = null;
                    }
                }
            }
        }

        public string DisplayMember
        {
            get
            {
                return _displayMember;
            }
            set
            {
                if (_displayMember == value) { return; }
                _displayMember = value;
                if (base._hostedControl != null && EditComboBox != null)
                {
                    try
                    {
                        EditComboBox.DisplayMember = _displayMember;
                    }
                    catch
                    {
                        _displayMember = null;
                    }
                }
            }
        }

        public string ValueMember
        {
            get
            {
                return _valueMember;
            }
            set
            {
                if (_valueMember == value) { return; }
                _valueMember = value;
                if (base._hostedControl != null && this.EditComboBox != null)
                {
                    try
                    {
                        EditComboBox.ValueMember = _valueMember;
                    }
                    catch
                    {
                        _valueMember = null;
                    }
                }
            }

        }

        protected override bool CanShowHostedControlIfReadOnly
        {
            get { return false; }
        }

        public EditableDataGridComboBox EditComboBox
        {
            get
            {
                return this.HostedControl as EditableDataGridComboBox;
            }
        }
        #endregion

        //internal override void Edit(CurrencyManager source, System.Drawing.Rectangle bounds, int row, int column, bool isVisable)
        //{
        //    base.Edit(source, bounds, row, column, isVisable);
        //    if (this.HostedControl.Visible)
        //    {
        //        ((MyComboBox)EditComboBox).StartEdit();
        //    }
        //}

        //internal override void EndEdit()
        //{
        //    this._inEdit = false;
        //    ((MyComboBox)EditComboBox).EndEdit();
        //    base.EndEdit();
            
        //}

        internal override void CommitEdit()
        {
            this.EditComboBox.DroppedDown = false;
            base.CommitEdit();
            

            //object selValue = this.EditComboBox.SelectedValue;
            //try
            //{
            //    base.SetCellValue(_row, selValue);

            //    if (selValue == null && _preValue != null || selValue!= null && _preValue == null || selValue!= null && !_preValue.Equals(selValue))
            //    {
            //        this.OnSelectedValueChanged(new EventArgs());
            //    }
            //    _preValue = selValue;
            //}
            //catch
            //{
            //    this.UpdateHostedControl(_preValue);
            //}
        }



        protected override void UpdateHostedControl(object cellValue)
        {
            //this.EditComboBox.SelectedItem = cellValue;
            //this.EditComboBox.Text = null;
            //this.EditComboBox.Text = this.FormatText(cellValue);
            this.EditComboBox.Text = null;
            this.EditComboBox.Text = this.EditComboBox.GetItemText(cellValue);
        }


        protected override object GetHostControlValue()
        {
            string cellText = this.EditComboBox.Text;
            if (String.IsNullOrEmpty(cellText))
            {
                return null;
            }
            object value;
            if (this.EditComboBox.GetValueFromItemText(cellText,out value))//if the cell text matches a combobox item, get its value
            {
                return value;
            }
            else //otherwise try to convert the cell text 
            {
                return base.ConvertValueFromText(cellText);
            }

            //object value = this.EditComboBox.SelectedValue;
            ////object selItem = this.EditComboBox.SelectedItem;
            ////if (this.EditComboBox.GetItemText(selItem) != this.EditComboBox.Text)
            ////{
            ////    if (string.IsNullOrEmpty(this.EditComboBox.Text))
            ////    {
            ////        value = null;
            ////    }
            ////    else
            ////    {
            ////        value = base.ConvertValueFromText(this.EditComboBox.Text);
            ////    }
            ////}
            //return value;
        }



        internal void OnSelectedValueChanged(EventArgs e)
        {
            if (this.SelectedValueChanged != null)
            {
                this.SelectedValueChanged(this, e);
            }
        }



        protected override Control CreateHostedControl()
        {
            ComboBox box = new EditableDataGridComboBox(this);
            box.DataSource = _dataSource;
            box.DisplayMember = _displayMember;
            box.ValueMember = _valueMember;
            box.DropDownStyle = ComboBoxStyle.DropDown;
            return box;
        }


#if NET_CF
        public class EditableDataGridComboBox : FMSC.Controls.Mobile.ComboBoxRedux, IKeyPressProcessor
#else
        public class EditableDataGridComboBox : ComboBox
#endif
        {

            public EditableDataGridComboBox(EditableComboBoxColumn owner)
                : base()
            {
                
            }

            //internal void SetDatasource(object dataSource)
            //{
            //    int index = -1;
            //    IList list = (IList)dataSource;
            //    for (int i = 0; i < list.Count; i++)
            //    {
            //        if (this.GetItemText(list[i]) == _casheItemText)
            //        {
            //            index = i;
            //            break;
            //        }
            //    }
            //    this.DataSource = dataSource;
            //    this.SelectedIndex = index;
            //}
            //protected override void OnDataSourceChanged(EventArgs e)
            //{
            //    base.OnDataSourceChanged(e);
            //    int index = FindItem(_casheItemText, true);
            //    this.SelectedIndex = index;
            //}

            //protected int FindItem(string s, bool ignoreCase)
            //{
            //    IList items = (IList)this.Items;
            //    for (int i = 0; i < items.Count; i++)
            //    {
            //        if (String.Compare(this.GetItemText(items[i]), s, ignoreCase) == 0)
            //        {
            //            return i;
            //        }
            //    }
            //    return -1;
            //}

            //protected override void OnTextChanged(EventArgs e)
            //{
            //    base.OnTextChanged(e);
            //    if (this.GetItemText(this.SelectedItem) != this.Text)
            //    {
            //        this.SelectedIndex = this.FindItem(this.Text, true);
            //    }
            //}

            //public override int SelectedIndex
            //{
            //    get
            //    {
            //        return base.SelectedIndex;
            //    }
            //    set
            //    {
            //        if (this.SelectedIndex == value) { return; }
            //       base.SelectedIndex = value;
            //    }
            //}

            //public override string Text
            //{
            //    get
            //    {
            //        return base.Text;
            //    }
            //    set
            //    {

            //        base.Text = value;
            //        if (this.GetItemText(this.SelectedItem) != value)
            //        {
            //            this.SelectedIndex = this.FindItem(value, true);
            //        }
            //    }
            //}



            public new object SelectedItem
            {
                get
                {
                    string itemText = this.Text;
                    int i = this.FindItem(itemText, true);
                    if (i < 0)
                    {
                        return null;
                    }
                    else
                    {
                        return this.Items[i];
                    }
                }
                set
                {
                    base.SelectedItem = value;
                }

            }

            public object GetValueFromItemText(String displayValue)
            {
                int index = this.FindItem(displayValue, true);
                if (index == -1) { return null; }
                object item = base.Items[index];
                object itemValue = base.FilterItemOnProperty(item, this.ValueMember);
                if (itemValue != null)
                {
                    item = itemValue;
                }
                return item;
            }

            //public String GetItemText(object obj)
            //{
            //    base.getI

            //}


            public void SetComboBoxItems(System.Collections.IList list)
            {
                //String  selectedText = this.Text; 
                this.SetItemsCore(list);
                
                //for (int i = 0; i < this.Items.Count; i++)
                //{
                //    if (selectedText == this.GetItemText(this.Items[i]))
                //    {
                //        this.SelectedIndex = i;
                //        return;
                //    }
                //}
            }




            


            protected override void OnGotFocus(EventArgs e)
            {
                _keyCounter = 0;
                base.OnGotFocus(e);
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                e.Handled = this.ProcessKeyPress(e.KeyData);
                if (e.Handled == false)
                {
                    base.OnKeyDown(e);
                }

            }

            #region IkeyPressProcessor members

            //private bool _keyCounter2 = false;
            private int _keyCounter = 0;//keeps track of the number of times the user hits enter on the control after it gains focus. 
            //
            public virtual bool ProcessReturnKey()
            {
                
                if (this.DroppedDown == true)
                {
                    _keyCounter++; 
                    EditableDataGrid dg = this.Parent as EditableDataGrid;

                    if (DeviceInfo.DevicePlatform == PlatformType.WinCE
                        && (dg != null && _keyCounter == 3))
                    {
                        dg.MoveSeclection(Direction.Right);
                        return true;
                    }

                    else if (dg != null && _keyCounter == 2)
                    {
                        dg.MoveSeclection(Direction.Right);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    _keyCounter = 0;

                    IKeyPressProcessor p = this.Parent as IKeyPressProcessor;
                    if (p == null) { return false; }
                    return p.ProcessReturnKey();
                }
            }

            public virtual bool ProcessTabKey()
            {
                IKeyPressProcessor p = this.Parent as IKeyPressProcessor;
                if (p == null) { return false; }
                return p.ProcessTabKey();
            }

            public virtual bool ProcessBackTabKey()
            {
                IKeyPressProcessor p = this.Parent as IKeyPressProcessor;
                if (p == null) { return false; }
                return p.ProcessBackTabKey();
            }

#if NET_CF
        public virtual bool ProcessDialogKey(Keys keyVal)
#else 
        public new bool ProcessDialogKey(Keys keyVal)
#endif
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
                            if (this.DroppedDown == false)
                            {
                                EditableDataGrid dg = this.Parent as EditableDataGrid;
                                if (dg != null)
                                {
                                    return dg.MoveSeclection(Direction.Up);
                                }
                            }
                            break;
                        }
                    case (Keys.Down):
                        {
                            if (this.DroppedDown == false)
                            {
                                EditableDataGrid dg = this.Parent as EditableDataGrid;
                                if (dg != null)
                                {
                                    return dg.MoveSeclection(Direction.Down);
                                }
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

            public virtual bool ProcessKeyPress(Keys keyVal)
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
                    case (Keys.Enter):
                        {
                            if (DeviceInfo.DevicePlatform == PlatformType.WinCE)
                            {
                                this.DroppedDown = true;
                                return true;
                            }
                            else
                            {
                                return false;
                            }

                        }
                    default:
                        {
                            IKeyPressProcessor p = this.Parent as IKeyPressProcessor;
                            if (p == null) { return false; }
                            return p.ProcessKeyPress(keyVal);
                        }
                }
            }
            #endregion
        }
    }
}
