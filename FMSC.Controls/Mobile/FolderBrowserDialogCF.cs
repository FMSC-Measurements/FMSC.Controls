using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;

namespace FMSC.Controls
{
    public partial class FolderBrowserDialogCF : Form
    {
        private string _FolderPath;
        private List<string> _FolderList;

        public string Folder { get { return _FolderPath; } }

        public string Title { set { this.Text = value; } } 

        public FolderBrowserDialogCF(string StartFolder)
        {
            this.DialogResult = DialogResult.Cancel;

            InitializeComponent();

            _FolderList = new List<string>();

            if (StartFolder != null && Directory.Exists(StartFolder))
            {
                _FolderPath = StartFolder;
            }
            else
            {
                _FolderPath = "\\";// Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            }

            LoadFolders(_FolderPath);
        }

        private void LoadFolders(string dir)
        {
            try
            {
                if (Directory.Exists(dir))
                {
                    DirectoryInfo curDir = new DirectoryInfo(dir);

                    _FolderList.Clear();
                    lstFolders.Items.Clear();

                    try
                    {
                        foreach (DirectoryInfo di in curDir.GetDirectories())
                        {
                            _FolderList.Add(di.FullName);
                            lstFolders.Items.Add(di.Name);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Unable to load subfolders");
                    }

                    if (curDir != null && curDir.Parent != null)
                    {
                        _FolderList.Add(curDir.Parent.FullName);
                        lstFolders.Items.Add("Go to Parent");
                    }

                    txtFolder.Text = curDir.FullName;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MessageBox.Show("error loading directory: " + dir); 
            }
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lstFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_FolderList.Count > 0)
            {
                if (lstFolders.SelectedIndex > -1 && lstFolders.SelectedIndex < _FolderList.Count)
                {
                    _FolderPath = _FolderList[lstFolders.SelectedIndex];
                    LoadFolders(_FolderPath);
                }
            }
        }
    }
}