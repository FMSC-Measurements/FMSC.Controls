using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Design;

namespace FMSC.Controls
{
    public delegate void SelectedValueChangedEventHandler(object sender, EventArgs e, object selectedValue);

    public delegate void SelectionAddedEventHandler(object sender, object item, int index);

    public delegate void SelectionRemovedEventHandler(object sender, object item);

    public class ItemMovedEventArgs : EventArgs
    {
        [Obsolete]
        public Direction Direction { get; set; }

        public object Item { get; set; }
        public int NewIndex { get; set; }
        public int PreviousIndex { get; set; }
    }

    [LookupBindingProperties("DataSource", "DisplayMember","ValueMember", "SelectedValue")]
    public partial class OrderableAddRemoveWidget : UserControl
    {
        public OrderableAddRemoveWidget()
        {
            InitializeComponent();
            _SelectedListBox.SelectedValueChanged += new EventHandler(OnSelectedValueChanged);
            _AvailableListBox.SelectedValueChanged += new EventHandler(OnSelectedValueChanged);
        }




        #region events
        [Browsable(true)]
        public event EventHandler<ItemMovedEventArgs> SelectionMoved;

        [Browsable(true)]
        public event SelectionAddedEventHandler SelectionAdded;

        [Browsable(true)]
        public event SelectionRemovedEventHandler SelectionRemoved;

        [Browsable(true)]
        public event SelectedValueChangedEventHandler SelectedValueChanged;
        #endregion events

        private CurrencyManager _CurrencyManager;
        private CurrencyManager _selectedItemsCurrencyManager;

        [Category("Data")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(null)]
        [AttributeProvider(typeof(IListSource))]
        public Object DataSource
        {
            get
            {
                return _AvailableListBox.DataSource;
            }
            set
            {
                OnAvalibleDataSourceChanging();
                _AvailableListBox.DataSource = value;
                OnAvalibleDataSourceChanged();
            }
        }

        private void OnAvalibleDataSourceChanging()
        {
            if(_CurrencyManager != null)
            {
                _CurrencyManager.ListChanged -= _CurrencyManager_ListChanged;
            }
        }

        private void OnAvalibleDataSourceChanged()
        {
            if (DataSource != null)
            {
                _CurrencyManager = _AvailableListBox.BindingContext[DataSource] as CurrencyManager;
                _CurrencyManager.ListChanged += _CurrencyManager_ListChanged;
                _CurrencyManager_ListChanged(null, null);
            }
            

        }

        public object SelectedItemsDataSource
        {
            get
            {
                return _SelectedListBox.DataSource;
            }

            set
            {
                OnSelectedDataSourceChanging();
                _SelectedListBox.DataSource = value;
                OnSelectedDataSourceChanged();
            }
        }

        private void OnSelectedDataSourceChanging()
        {
            if(_selectedItemsCurrencyManager != null)
            {
                _selectedItemsCurrencyManager.ListChanged -= _selectedItemsCurrencyManager_ListChanged;
            }
        }

        private void OnSelectedDataSourceChanged()
        {
            if(SelectedItemsDataSource != null)
            {
                _selectedItemsCurrencyManager = _SelectedListBox.BindingContext[SelectedItemsDataSource] as CurrencyManager;
                _selectedItemsCurrencyManager.ListChanged += _selectedItemsCurrencyManager_ListChanged;
                _selectedItemsCurrencyManager_ListChanged(null, null);
            }
        }

        private void _CurrencyManager_ListChanged(object sender, ListChangedEventArgs e)
        {
            var avalibleCount = _CurrencyManager?.Count ?? 0;

            AddAllButton.Enabled = AddButton.Enabled = avalibleCount > 1;
        }

        private void _selectedItemsCurrencyManager_ListChanged(object sender, ListChangedEventArgs e)
        {
            var selectedCount = _selectedItemsCurrencyManager?.Count ?? 0;

            DownButton.Enabled = UpButton.Enabled = selectedCount > 2;
            RemoveAllButton.Enabled = RemoveButton.Enabled = selectedCount > 1;
        }


        private string _displayMember;
        [Category("Data")]
        [TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DefaultValue("")]
        public string DisplayMember
        {
            get { return _displayMember; }
            set 
            { 
                _displayMember = value;
                _SelectedListBox.DisplayMember = value;
                _AvailableListBox.DisplayMember = value;
            }
        }

        private string _ValueMember;
        [Category("Data")]
        [DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string ValueMember
        {
            get
            {
                return _ValueMember;
            }
            set
            {
                _ValueMember = value;
                _SelectedListBox.ValueMember = value;
                _AvailableListBox.ValueMember = value;
            }
        }

        //public object SelectedValue
        //{
        //    get
        //    {
        //        return null;
        //    }
        //    set
        //    {
        //    }
        //}

        private void AddAllButton_Click(object sender, EventArgs e)
        {
            IList list = _CurrencyManager.List;
            IList selList = _selectedItemsCurrencyManager.List;
            foreach (object obj in list)
            {
                selList.Add(obj);
                OnSelectionAdded(this, obj, selList.Count - 1);
            }
            list.Clear();
            RefreshBindings();
        }

        private void RemoveAllButton_Click(object sender, EventArgs e)
        {
            IList selList = _selectedItemsCurrencyManager.List;
            IList list = _CurrencyManager.List;
            foreach (object obj in selList)
            {
                list.Add(obj);
                OnSelectionRemoved(this, obj);
            }
            selList.Clear();
            RefreshBindings();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            //object obj = this._CurrencyManager.Current;
            IList selList = _selectedItemsCurrencyManager.List;
            IList list = _CurrencyManager.List;

            object[] items = new object[_AvailableListBox.SelectedItems.Count];
            _AvailableListBox.SelectedItems.CopyTo(items, 0);

            foreach (object obj in items)
            {
                selList.Add(obj);
                list.Remove(obj);//throws if avalible is a fixed size
                OnSelectionAdded(this, obj, selList.Count - 1);
            }
            RefreshBindings();
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            //object obj = this._selectedItemsCurrencyManager.Current;
            IList list = _CurrencyManager.List;
            IList selList = _selectedItemsCurrencyManager.List;

            object[] items = new object[_SelectedListBox.SelectedItems.Count];
            _SelectedListBox.SelectedItems.CopyTo(items, 0);

            foreach (object obj in items)
            {
                list.Add(obj);
                selList.Remove(obj);
                OnSelectionRemoved(this, obj);
            }
            RefreshBindings();
        }

        private void UpButton_Click(object sender, EventArgs e)
        {
            if (_selectedItemsCurrencyManager.Count > 1)
            {
                object item = _selectedItemsCurrencyManager.Current;
                int index = _selectedItemsCurrencyManager.Position;
                if (index > 0)
                {
                    int newIndex = index - 1;
                    _SelectedListBox.BeginUpdate();
                    Swap(_selectedItemsCurrencyManager.List, index, newIndex);
                    RefreshBindings();
                    _SelectedListBox.EndUpdate();
                    _SelectedListBox.ClearSelected();
                    _SelectedListBox.SelectedIndex = newIndex;
                    OnSelectionMoved(new ItemMovedEventArgs
                    {
                        Direction = Direction.Up,
                        Item = item,
                        NewIndex = newIndex,
                        PreviousIndex = index
                    });
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Nothing selected, list is empty.");
            }
        }

        private void DownButton_Click(object sender, EventArgs e)
        {
            if(_selectedItemsCurrencyManager.Count > 1)
            {
                IList list = _selectedItemsCurrencyManager.List;

                object item = _selectedItemsCurrencyManager.Current;
                int index = _selectedItemsCurrencyManager.Position;
                if (index < list.Count - 1)
                {
                    int newIndex = index + 1;
                    _SelectedListBox.BeginUpdate();
                    Swap(list, index, newIndex);
                    RefreshBindings();
                    _SelectedListBox.EndUpdate();
                    _SelectedListBox.ClearSelected();
                    _SelectedListBox.SelectedIndex = newIndex;


                    OnSelectionMoved(new ItemMovedEventArgs
                    {
                        Direction = Direction.Down,
                        Item = item,
                        NewIndex = newIndex,
                        PreviousIndex = index
                    });
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Nothing selected, list is empty.");
            }
            
        }

        protected void RefreshBindings()
        {
            _CurrencyManager.Refresh();
            _selectedItemsCurrencyManager.Refresh();
        }

        protected void OnSelectionMoved(ItemMovedEventArgs e)
        {
            SelectionMoved?.Invoke(this, e);
        }

         protected void OnSelectionAdded(object sender, object item, int index)
        {
            SelectionAdded?.Invoke(this, item, index);
        }

         void OnSelectedValueChanged(object sender, EventArgs e)
         {
            var selectedValueChanged = SelectedValueChanged;
             if (selectedValueChanged != null)
             {
                 ListControl control = (ListControl)sender;
                 object value = control.SelectedValue;
                selectedValueChanged(sender, e, value);
             }
         }

        protected void OnSelectionRemoved(object sender, object item)
        {
            SelectionRemoved?.Invoke(this, item);
        }

        private static void Swap(IList list, int i, int j)
        {
            object temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        

    }

    
    
}
