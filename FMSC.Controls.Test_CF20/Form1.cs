using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using FMSC.Controls;

namespace testBox
{
    public partial class Form1 : CustomForm
    {
        EditableComboBoxColumn comboBoxCol;
        EditableTextBoxColumn textBoxCol;
        EditableDateTimePickerColumn dateTimeCol;
        EditableUpDownColumn upDwnCol;
        EditableTextBoxColumn readOnlyTextBoxCol;

        public Form1()
        {
            InitializeComponent();
            // tell our form that we want to intercept all key press events in our form.Otherwise our KeyPress event method will only get events when the form class has focus.
            //this.KeyPreview = true;

            //give our dataGrid our SIP, so that I can listen and react to events on our SIP
            editableDataGrid1.SIP = inputPanel1;

            editableDataGrid1.CellValidating += new EditableDataGridCellValidatingEventHandler(editableDataGrid1_CellValidating);
            editableDataGrid1.CellValueChanged += new EditableDataGridCellValueChangedEventHandler(editableDataGrid1_CellValueChanged);

            buttonPanel1.Text = "something";

            ////allow new rows on datagrid
            //editableDataGrid1.AllowNewRow = true;

            //set up columns for our dataGrid
            DataGridTableStyle ts = new DataGridTableStyle() { MappingName = "something" };
            comboBoxCol = new EditableComboBoxColumn() { MappingName = "num1" };
            textBoxCol = new EditableTextBoxColumn() { MappingName = "word", MaxTextLength = 12, GoToNextColumnWhenTextCompleate = true };
            dateTimeCol = new EditableDateTimePickerColumn() { MappingName = "date" };
            upDwnCol = new EditableUpDownColumn() { MappingName = "upDwn" };
            readOnlyTextBoxCol = new EditableTextBoxColumn() { MappingName = "word2", ReadOnly = true };
            ts.GridColumnStyles.Add(comboBoxCol);
            ts.GridColumnStyles.Add(textBoxCol);
            ts.GridColumnStyles.Add(dateTimeCol);
            ts.GridColumnStyles.Add(upDwnCol);
            ts.GridColumnStyles.Add(readOnlyTextBoxCol);

            //set up our button column and give it a event handler for button clicks
            //note the mapping name for the buttonColumn must be provided and a valid property to bind to
            //this is because the dataGrid only displays columns that have a valid MappingName
            //I've created a work around that should work for Full Framework, and will probably be able to make a workaround for Mobile
            //, but until then provide a mapping name.
            //For DAL DataObjects you could use the Tag property as a mapping name, since it isn't reserved for any propuse, and is intended for special cases like this.
            DataGridButtonColumn buttonCol = new DataGridButtonColumn() { MappingName = "button", UseCellColumnTextForCellValue = true, Text = "click me" };
            buttonCol.Click += new ButtonCellClickEventHandler(buttonCol_Click);
            ts.GridColumnStyles.Add(buttonCol);

            editableDataGrid1.TableStyles.Add(ts);

            //create some dummy data to test the grid
            List<something> things = new List<something>();
            for (int i = 0; i < 100; i++)
            {
                things.Add(new something() { num1 = i, word = "word", button = "click me" });
            }
            bindingSource1.DataSource = things;
            //editableDataGrid1.DataSource = things;
        }

        void editableDataGrid1_CellValueChanged(object sender, EditableDataGridCellEventArgs e)
        {
        }

        void editableDataGrid1_CellValidating(object sender, EditableDataGridCellValidatingEventArgs e)
        {
            editableDataGrid1.Focus();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.comboBoxCol.DataSource = new int[] { 101, 102, 103 };
        }

        void buttonCol_Click(ButtonCellClickEventArgs e)
        {
            label1.Text = e.RowNumber.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialogRedux ofd = new OpenFileDialogRedux();
            //ofd.Filter = "text .txt|*.txt|executable .exe|*.exe";
            ofd.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.comboBoxRedux1.DroppedDown = true;
            //Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders\");
            ////Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedPidlMRULegacy");
            //String[] subkeys = key.GetSubKeyNames();
            //String[] values = key.GetValueNames();
            //AppDomain.CurrentDomain.FriendlyName
        }
    }

    public class something : IDataErrorInfo
    {
        public int num1 { get; set; }

        public string word { get; set; }

        public string word2 { get; set; }

        public string button { get; set; }

        public DateTime date { get; set; }

        public int upDwn { get; set; }

        #region IDataErrorInfo Members

        public string Error
        {
            get { return "error"; }
        }

        public string this[string columnName]
        {
            get { return columnName + " error"; }
        }

        #endregion IDataErrorInfo Members
    }
}