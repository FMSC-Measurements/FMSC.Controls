
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Forms;


namespace FMSC.Controls {
    public class OpenFileDialogRedux : Component
    {
        #region ********** Fields **********
        private MyOpenFileDialogRedux _dialogInstance;
        #endregion


        #region ********* Ctor **********
        public OpenFileDialogRedux()
        {
            _dialogInstance = new MyOpenFileDialogRedux();
        }

        ~OpenFileDialogRedux()
        {
            this.Dispose(false);
        }
        #endregion


        #region ********** Public Properties **********
        public string Filter
        {
            get
            {
                return _dialogInstance.Filter;
            }
            set
            {
                _dialogInstance.Filter = value;
            }
        }
        public int FilterIndex
        {
            get { return _dialogInstance.FilterIndex; }
            set { _dialogInstance.FilterIndex = value; }
        }

        public string FileName
        {
            get { return _dialogInstance.FileName; }
            set { _dialogInstance.FileName = value; }
        }

        public string InitialDirectory
        {
            get { return _dialogInstance.InitialDirectory; }
            set { _dialogInstance.InitialDirectory = value; }
        }
        //TODO add support for RestoreDirectory
        //public bool RestoreDirectory { get; set; }
        public String SafeFileName
        {
            get { return _dialogInstance.SafeFileName; }
        }
        public FileInfo SelectedFile
        {
            get { return _dialogInstance.SelectedFile; }
            set { _dialogInstance.SelectedFile = value; }
        }
        public bool ShowHidden
        {
            get { return _dialogInstance.ShowHidden; }
            set { _dialogInstance.ShowHidden = value; }
        }
        #endregion


        #region ********** Public Methods **********
        public new void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public DialogResult ShowDialog()
        {
            return _dialogInstance.ShowDialog();
        }
        #endregion


        #region ********** Protected Methods **********
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            //this._dialogInstance.Dispose();

            if (disposing)
            {
                this._dialogInstance = null;
            }
        }
        #endregion
    }


    public class MyOpenFileDialogRedux : Form
    {
        #region ********* internal classes *********
        public class CustomPlacesCollection : ICollection<DirectoryInfo>
        {
            public CustomPlacesCollection()
            {
                this.Items = new List<DirectoryInfo>();
            }

            protected List<DirectoryInfo> Items { get; set; }

            public void Add(String path)
            {
                this.Items.Add(new DirectoryInfo(path));
            }

            public void Add(DirectoryInfo item)
            {
                this.Items.Add(item);
            }

            #region ICollection<DirectoryInfo> Members



            void ICollection<DirectoryInfo>.Clear()
            {
                this.Items.Clear();
            }

            bool ICollection<DirectoryInfo>.Contains(DirectoryInfo item)
            {
                return this.Items.Contains(item);
            }

            void ICollection<DirectoryInfo>.CopyTo(DirectoryInfo[] array, int arrayIndex)
            {
                this.Items.CopyTo(array, arrayIndex);
            }

            int ICollection<DirectoryInfo>.Count
            {
                get { return Items.Count; }
            }

            bool ICollection<DirectoryInfo>.IsReadOnly
            {
                get { return false; }
            }

            bool ICollection<DirectoryInfo>.Remove(DirectoryInfo item)
            {
                return this.Items.Remove(item);
            }

            #endregion

            #region IEnumerable<DirectoryInfo> Members

            IEnumerator<DirectoryInfo> IEnumerable<DirectoryInfo>.GetEnumerator()
            {
                return this.Items.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.Items.GetEnumerator();
            }

            #endregion
        }

        protected enum FileIcon : int {
            File,
            Directory,
            StorageCard,
            Up
        }

        protected struct FilterItem
        {
            public FilterItem(string friendlyStr, string filterStr)
            {
                FriendlyStr = friendlyStr;
                FilterStr = filterStr;
            }
            public string FriendlyStr;
            public string FilterStr;
        }
        #endregion

        #region ********** Fields **********
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mnuMain;
        private IntPtr hImgSmall; //handel to system icon image list
        private bool changingDir = false;
        protected DirectoryInfo _currentDirectory;
        private string _filter ;
        private FilterItem[] _filters;
        private CustomPlacesCollection _customPlaces = new CustomPlacesCollection();
        //protected BindingList<DirectoryInfo> _parentDirList;

        private System.Windows.Forms.ComboBox ddVisitedLocations;
        private System.Windows.Forms.ListView lstFiles;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.MenuItem itmCancel;
        private System.Windows.Forms.MenuItem itmOk;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonUP;
        private System.Windows.Forms.Panel panel3;
        private Label label;
        private System.Windows.Forms.ComboBox cbType;
        #endregion

        #region ********** Public Properties **********

        public CustomPlacesCollection CustomPlaces { get { return this._customPlaces; } }

        public string Filter
        {
            get{ return _filter; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    cbType.Items.Clear();       //clear filter combobox
                    String[] filters = value.Split('|');
                    if (filters.Length == 0 || filters.Length % 2 != 0)
                    {
                        throw new ArgumentException("Invalid Filter", "Filter");
                    }

                    _filters = new FilterItem[filters.Length / 2];
                    for (int i = 0; i < filters.Length; i += 2)
                    {
                        _filters[i / 2] = new FilterItem(filters[i], filters[i + 1]);
                        cbType.Items.Add(filters[i]);       //add filter to type combo box
                        
                    }
                    cbType.SelectedIndex = 0;
                    _filter = value;
                }
                else
                {
                    _filter = null;
                }
            }    
        }

        public int FilterIndex{ get; set; }

        public string FileName 
        { 
            get
            {
                return SelectedFile.FullName;
            }
            set
            {
                //throw new NotImplementedException("FMSC.Controls.OpenFileDialog.Set_FileName not implemented use Set_SelectedFile instead");
                if(!String.IsNullOrEmpty(value) && value != SelectedFile.FullName)
                {
                    SelectedFile = new FileInfo(value);
                }
            }
        }
        //public string[] FileNames 
        //{ 
        //    get
        //    {
        //        String[] array = new String[SelectedFiles.Length];
        //        for(int i = 0; i < SelectedFiles.Length; i++)
        //        {
        //            array[i] = SelectedFiles[i].FullName;
        //        }
        //        return array;
        //    }
        //    set
        //    {
        //        throw new NotImplementedException("FMSC.Controls.OpenFileDialog.Set_FileNames not implemented use Set_SelectedFiles instead");
        //    }
        //}

        public string InitialDirectory { get; set; }

        //TODO add support for RestoreDirectory
        //public bool RestoreDirectory { get; set; }

        public String SafeFileName
        {
            get
            {
                return SelectedFile.Name;
            }
        }

        //public String[] SafeFileNames 
        //{ 
        //    get
        //    {
        //        String[] array = new String[SelectedFiles.Length];
        //        for(int i = 0; i < SelectedFiles.Length; i++)
        //        {
        //            array[i] = SelectedFiles[i].Name;
        //        }
        //        return array;
        //    }
        //} 

        public FileInfo SelectedFile { get; set; }
        //public FileInfo[] SelectedFiles { get; set; }

        public bool ShowHidden { get; set; }

        


        /// <summary>
        /// Gets or sets the full path (including directory) of theselected file.
        /// </summary>
        /// <value>The path of the selected file.</value>
        //public string SelectedFile {
        //    get {
        //        return Path.Combine(currentDirectory, selectedFileName); 
        //    }
        //    set { 
        //        FileInfo file = new FileInfo(value);
        //        selectedFileName = file.Name;
        //        currentDirectory = file.DirectoryName;
        //        txtFileName.Text = value;
        //    }
            
        //}
        /// <summary>
        /// Gets or sets the name of the selected file.
        /// </summary>
        /// <value>The name of the selected file.</value>
        //public string SelectedFileName {
        //    get { return selectedFileName; }
        //    set { selectedFileName = value; }
        //}

        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets or sets the currently selected path.
        /// </summary>
        /// <value>The current path.</value>
        protected DirectoryInfo CurrentDirectory {
            get { return _currentDirectory; }
            set { _currentDirectory = value; }
        }

        protected bool IsVisable(Win32.SHFILEINFO fsi)
        {
            int SFGAO_HIDDEN = 0x00080000; // see file atrubutes @ http://msdn.microsoft.com/en-us/library/windows/desktop/bb762589(v=vs.85).aspx
            return ShowHidden || ((fsi.dwAttributes & SFGAO_HIDDEN) != SFGAO_HIDDEN);
        }

        protected bool IsVisable(FileInfo fsi)
        {
            return ShowHidden || ((fsi.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden);
        }




        //protected String GetLastVisitedMRU()
        //{
            
            //Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedPidlMRULegacy");
            //key.GetValue(...)
            //AppDomain.CurrentDomain.FriendlyName
        //}

        //protected void SetLastVisitedMRU(DirectoryInfo dir)
        //{
            //
            //Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\LastVisitedPidlMRULegacy");
            //key.SetValue
            //AppDomain.CurrentDomain.FriendlyName
        //}



        private void InitializeListViewImageList()
        {
            Win32.SHFILEINFO shinfo = new Win32.SHFILEINFO();       //initialize a SHFILEINFO struct  
            hImgSmall = Win32.SHGetFileInfo("\\", 0, ref shinfo,    //call native method to retrieve system image list
                (uint)System.Runtime.InteropServices.Marshal.SizeOf(shinfo),    //provide size of SHFILEINFO struct
                (uint)Win32.SHGetFileInfoFlags.SHGFI_ICON | (uint)Win32.SHGetFileInfoFlags.SHGFI_SMALLICON);          //tell it we want the small image list;

            //instruct List View to not delete the image list when it closes, otherwise bad things could happen
            int currentLVStyle = Win32.GetWindowLong(lstFiles.Handle, Win32.GWL_STYLE);
            Win32.SetWindowLong(lstFiles.Handle, Win32.GWL_STYLE, currentLVStyle | Win32.LVS_SHAREIMAGELISTS);

            Win32.SendMessage(lstFiles.Handle, Win32.LVM_SETIMAGELIST, 1, hImgSmall); //attatch image list to list view
        }

        

        /// <summary>
        /// Initializes a new <see cref="OpenFileDialog"/> to show the root of the file system.
        /// </summary>
        public MyOpenFileDialogRedux() {
            this.InitializeComponent();

            this.CustomPlaces.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Personal)));
            this.CustomPlaces.Add(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)));

            if(DeviceInfo.DevicePlatform == PlatformType.WinCE)
            {
                this.WindowState = FormWindowState.Maximized;
            }

            this.InitialDirectory = @"\";
            //this._fileList = new BindingList<FileListItem>();
            //this._parentDirList = new BindingList<DirectoryInfo>();

            
            this.InitializeListViewImageList();



            //this.ddVisitedLocations.DataSource = typeof(DirectoryInfo);
            this.ddVisitedLocations.DisplayMember = "Name";
         

            this.itmCancel.Click += delegate(object sender, EventArgs e) {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            this.itmOk.Click += delegate(object sender, EventArgs e) {
                if (ProcessSelection())
                {
                    DialogResult = DialogResult.OK;
                    this.Close();
                }

            };

            this.ddVisitedLocations.SelectedValueChanged += delegate(object sender, EventArgs e) {
                if (!changingDir && ddVisitedLocations.SelectedItem != null)
                //if(ddVisitedLocations.SelectedItem != null)
                {
                    ShowDirectory(ddVisitedLocations.SelectedItem as DirectoryInfo);
                }
            };
            
            this.lstFiles.ItemActivate += delegate(object sender, EventArgs e) 
            {
                if (ProcessSelection())
                {
                    DialogResult = DialogResult.OK;
                    this.Close();
                }

            };

            this.lstFiles.SelectedIndexChanged += delegate(object sender, EventArgs e) {
                if(lstFiles.SelectedIndices.Count > 0) {
                    txtFileName.Text = lstFiles.Items[lstFiles.SelectedIndices[0]].Text;
                }
            };

            this.txtFileName.KeyDown += delegate(object sender, KeyEventArgs e) {
                if((e.KeyCode == Keys.Return || e.KeyCode == Keys.Enter) && !string.IsNullOrEmpty(txtFileName.Text)) {
                    string text = txtFileName.Text.ToLower(CultureInfo.InvariantCulture);
                    for(int i = 0; i < lstFiles.Items.Count; i++) {
                        if(lstFiles.Items[i].Text == text) {
                            lstFiles.Items[i].Selected = true;
                            if (ProcessSelection())
                            {
                                DialogResult = DialogResult.OK;
                                this.Close();
                            }
                        }
                    }
                }
            };
            //ShowDirectory(CurrentDirectory);
        }

        /// <summary>
        /// Initializes a new <see cref="OpenFileDialog"/> which shows the specified directory and file.
        /// </summary>
        /// <param name="currentPath">The current path.</param>
        /// <param name="selectedFile">The selected file.</param>
        //public OpenFileDialogRedux(string currentPath, string selectedFile)
        //    : this() {
        //    this.CurrentDirectory = currentPath;
        //    this.SelectedFile = selectedFile;
        //    this.txtFileName.Text = selectedFile;
        //}

        protected Win32.SHFILEINFO GetFileInfo(String path)
        {
            Win32.SHFILEINFO shinfo = new Win32.SHFILEINFO();       //initialize a SHFILEINFO struct  
            Win32.SHGetFileInfo(path, 0, ref shinfo,                //call native method to retrieve system image list
                (uint)System.Runtime.InteropServices.Marshal.SizeOf(shinfo),    //provide size of SHFILEINFO struct
                (uint)Win32.SHGetFileInfoFlags.SHGFI_ICON | (uint)Win32.SHGetFileInfoFlags.SHGFI_SMALLICON);          //tell it we want the small image list;

            return shinfo;
        }

        protected string GetDirName(string path)
        {
            
            return path.Substring(path.LastIndexOf('\\') + 1);
        }

        /// <summary>
        /// Updates the file name text box with the current file name, and displays the current directory in the list view.
        /// </summary>
        /// <param name="directoryPath">The directory path to show.</param>
        public void ShowDirectory(DirectoryInfo directoryPath) {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                changingDir = true;
                lstFiles.SuspendLayout();
                lstFiles.BeginUpdate();
                lstFiles.Items.Clear();                                                      //clear file list

                

                CurrentDirectory = directoryPath;
                if (CurrentDirectory.Parent == null)
                {
                    buttonUP.Enabled = false;
                }
                else
                {
                    buttonUP.Enabled = true;
                }
                //DirectoryInfo parentDir = CurrentDirectory.Parent;
                //if (parentDir != null)                                                   //if currentDirectory has a parrent directory
                //{
                //    ListViewItem itm = new ListViewItem("..");
                //    itm.Tag = parentDir;
                //    //itm.ImageIndex = (int)FileIcon.Up;
                //    lstFiles.Items.Add(itm);

                //    //FileListItem item = new FileListItem(parentDir);
                //    //item.Icon = (int)FileIcon.Up;
                //    //_itemList.Add(item);
                //}
                Win32.SHFILEINFO[] directories = GetSubDirectories(CurrentDirectory);
                foreach (Win32.SHFILEINFO dir in directories)
                {
                    if (!IsVisable(dir)) { continue; } //skip if not visable
                    ListViewItem item = new ListViewItem(GetDirName(dir.szDisplayName )); 
                    item.Tag = dir;
                    item.ImageIndex = dir.iIcon.ToInt32();
                    
                    //if ((dir.Attributes & (FileAttributes.Directory | FileAttributes.Temporary)) == (FileAttributes.Directory | FileAttributes.Temporary))
                    //{
                    //    //item.ImageIndex = (int)FileIcon.StorageCard;
                    //}
                    //else
                    //{
                    //    //item.ImageIndex = (int)FileIcon.Directory;
                    //}
                    lstFiles.Items.Add(item);
                }

                Win32.SHFILEINFO[] files = GetFilteredFiles(CurrentDirectory);
                foreach (Win32.SHFILEINFO file in files)
                {
                    if (!IsVisable(file)) { continue; }
                    ListViewItem item = new ListViewItem(Path.GetFileName(file.szDisplayName));
                    item.Tag = file;
                    item.ImageIndex = file.iIcon.ToInt32();
                    lstFiles.Items.Add(item);
                }

                UpdatePathComboBox();
                lstFiles.ResumeLayout();
                lstFiles.EndUpdate();

                txtFileName.Text = string.Empty;
                SelectedFile = null;
                changingDir = false;
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private Win32.SHFILEINFO[] GetSubDirectories(DirectoryInfo currentDir)
        {
            try
            {
                String[] directories = Directory.GetDirectories(currentDir.FullName);        //sort our list of directories
                Array.Sort(directories);
                //Array.Sort(directories, delegate(DirectoryInfo item1, DirectoryInfo item2)
                //{
                //    return String.Compare(item1.Name, item2.Name, true, CultureInfo.InvariantCulture);
                //});

                Win32.SHFILEINFO[] dirInfo = new Win32.SHFILEINFO[directories.Length];
                for (int i = 0; i < directories.Length; i++)
                {
                    dirInfo[i] = GetFileInfo(directories[i]);
                    dirInfo[i].szDisplayName = directories[i];
                }

                return dirInfo;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MessageBox.Show("Unable to load sub-directories. Check the drive for errors.");
                return new Win32.SHFILEINFO[0];//return empty array
            }

        }

        private Win32.SHFILEINFO[] GetFilteredFiles(DirectoryInfo currentDir)
        {
            List<String> files = new List<String>();
            if (_filters != null && _filters.Length > 0)        //we have filters
            {
                foreach (string filter in _filters[FilterIndex].FilterStr.Split(';'))
                {
                    files.AddRange(Directory.GetFiles(currentDir.FullName, filter));
                }
            }
            else                                                // filter hasn't been set 
            {
                files.AddRange(Directory.GetFiles(currentDir.FullName));
            }
            files.Sort();
            //files.Sort(delegate(FileInfo item1, FileInfo item2) //sort the files
            //{
            //    return String.Compare(item1.Name, item2.Name, true, CultureInfo.InvariantCulture);
            //});

            Win32.SHFILEINFO[] fileInfo = new Win32.SHFILEINFO[files.Count];
            
            for( int i = 0; i < files.Count; i++)
            {
                fileInfo[i] = this.GetFileInfo(files[i]);
                fileInfo[i].szDisplayName = files[i];
            }

            return fileInfo;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (this.SelectedFile != null)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                if (!string.IsNullOrEmpty(this.txtFileName.Text) && Path.HasExtension(this.txtFileName.Text))
                {
                    this.SelectedFile = new FileInfo(this.CurrentDirectory.FullName + "\\" + this.txtFileName.Text);
                    this.DialogResult = DialogResult.OK;
                }
                else
                {
                    this.DialogResult = DialogResult.Cancel;
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ShowDirectory(new DirectoryInfo(InitialDirectory));
        }

        /// <summary>
        /// Processes a selection in the list view and either displays a directory or closes the dialog..
        /// </summary>
        private bool ProcessSelection() 
        {
            ListViewItem selectedItem = null;
            if(lstFiles.SelectedIndices.Count > 0 && lstFiles.Focused) // Item was selected in the file list
            { 
                selectedItem = lstFiles.Items[lstFiles.SelectedIndices[0]] as ListViewItem;
                String path = ((Win32.SHFILEINFO)selectedItem.Tag).szDisplayName;  
                var file = new FileInfo(path);
                if ((file.Attributes & FileAttributes.Directory) == FileAttributes.Directory) 
                {
                    ShowDirectory(new DirectoryInfo(path));
                    return false; 
                }

                SelectedFile = new FileInfo(path);
                //return ValidateSelection();
            } 
            else // Name was entered into the text box
            {
                String fullPath;
                if (Path.IsPathRooted(txtFileName.Text))
                {
                    fullPath = txtFileName.Text;
                }
                else
                {
                    fullPath = String.Format("%s\\%s", CurrentDirectory.FullName, txtFileName.Text);
                }
                if (System.IO.Path.HasExtension(fullPath))
                {
                    SelectedFile = new FileInfo(fullPath);
                    //for (int i = 0; i < lstFiles.Items.Count; i++)
                    //{
                    //    if (lstFiles.Items[i].Text == SelectedFile.Name)
                    //    {
                    //        lstFiles.Items[i].Selected = true;
                    //        lstFiles.Items.
                    //        selectedItem = lstFiles.Items[i];
                    //        break;
                    //    }
                    //}
                }
            }
            return ValidateSelection();
            //if(selectedItem == null) {
            //    if(ValidateFile()) {
            //        this.DialogResult = DialogResult.OK;                    //?
            //    }
            //} else {
            //    switch(selectedItem.ImageIndex) {
            //        case (int)FileIcon.File:
            //            SelectedFileName = txtFileName.Text;
            //            if(ValidateFile()) {
            //                this.DialogResult = DialogResult.OK;
            //            }
            //            break;
            //        case (int)FileIcon.Directory:
            //        case (int)FileIcon.StorageCard:
            //        case (int)FileIcon.Up:
            //            ShowDirectory(selectedItem.Tag.ToString());
            //            break;
            //    }
            //}
        }
                                                                                                                                                         
        /// <summary>
        /// Updates the path combo box to show all parents of the current directory.
        /// </summary>
        private void UpdatePathComboBox() 
        {
            ddVisitedLocations.BeginUpdate();
            ddVisitedLocations.Items.Clear();
            ddVisitedLocations.Items.Add(CurrentDirectory);
            
            DirectoryInfo dir = CurrentDirectory;
            while (dir.Parent != null)
            {
                ddVisitedLocations.Items.Add(dir.Parent);
                dir = dir.Parent;
            }
            foreach (DirectoryInfo d in this.CustomPlaces)
            {
                ddVisitedLocations.Items.Add(d);
            }

            ddVisitedLocations.SelectedIndex = 0;
            ddVisitedLocations.EndUpdate();
        }

        /// <summary>
        /// Validates the selected file, more validation can be added if needed.
        /// </summary>
        /// <returns></returns>
        private bool ValidateSelection() {

            if(SelectedFile == null || SelectedFile.Exists == false || !IsVisable(SelectedFile) ) 
            {
                txtFileName.SelectAll();
                txtFileName.Focus();
                return false;
            }
            return true;
        }

        private void buttonUP_Click(object sender, EventArgs e)
        {
            if (CurrentDirectory == null || CurrentDirectory.Parent == null)
            { return; }

            ShowDirectory(CurrentDirectory.Parent);
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.FilterIndex = cbType.SelectedIndex;
                this.ShowDirectory(CurrentDirectory);
            }
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.label = new System.Windows.Forms.Label();
            this.mnuMain = new System.Windows.Forms.MainMenu();
            this.itmCancel = new System.Windows.Forms.MenuItem();
            this.itmOk = new System.Windows.Forms.MenuItem();
            this.ddVisitedLocations = new System.Windows.Forms.ComboBox();
            this.lstFiles = new System.Windows.Forms.ListView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.lblFileName = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonUP = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.Dock = System.Windows.Forms.DockStyle.Left;
            this.label.Location = new System.Drawing.Point(0, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(100, 22);
            this.label.Text = "Type";
            // 
            // mnuMain
            // 
            this.mnuMain.MenuItems.Add(this.itmCancel);
            this.mnuMain.MenuItems.Add(this.itmOk);
            // 
            // itmCancel
            // 
            this.itmCancel.Text = "Cancel";
            // 
            // itmOk
            // 
            this.itmOk.Text = "OK";
            // 
            // ddVisitedLocations
            // 
            this.ddVisitedLocations.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ddVisitedLocations.Location = new System.Drawing.Point(72, 0);
            this.ddVisitedLocations.Name = "ddVisitedLocations";
            this.ddVisitedLocations.Size = new System.Drawing.Size(168, 22);
            this.ddVisitedLocations.TabIndex = 0;
            // 
            // lstFiles
            // 
            this.lstFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstFiles.FullRowSelect = true;
            this.lstFiles.Location = new System.Drawing.Point(0, 44);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(240, 181);
            this.lstFiles.TabIndex = 1;
            this.lstFiles.View = System.Windows.Forms.View.List;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtFileName);
            this.panel1.Controls.Add(this.lblFileName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 225);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(240, 43);
            // 
            // txtFileName
            // 
            this.txtFileName.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtFileName.Location = new System.Drawing.Point(0, 22);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(240, 21);
            this.txtFileName.TabIndex = 1;
            // 
            // lblFileName
            // 
            this.lblFileName.Location = new System.Drawing.Point(3, 3);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(100, 20);
            this.lblFileName.Text = "File Name:";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ddVisitedLocations);
            this.panel2.Controls.Add(this.buttonUP);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(240, 22);
            // 
            // buttonUP
            // 
            this.buttonUP.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonUP.Location = new System.Drawing.Point(0, 0);
            this.buttonUP.Name = "buttonUP";
            this.buttonUP.Size = new System.Drawing.Size(72, 22);
            this.buttonUP.TabIndex = 0;
            this.buttonUP.Text = "UP";
            this.buttonUP.Click += new System.EventHandler(this.buttonUP_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.cbType);
            this.panel3.Controls.Add(this.label);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 22);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(240, 22);
            // 
            // cbType
            // 
            this.cbType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbType.Location = new System.Drawing.Point(100, 0);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(140, 22);
            this.cbType.TabIndex = 1;
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
            // 
            // MyOpenFileDialogRedux
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.lstFiles);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Menu = this.mnuMain;
            this.Name = "MyOpenFileDialogRedux";
            this.Text = "Open File";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}