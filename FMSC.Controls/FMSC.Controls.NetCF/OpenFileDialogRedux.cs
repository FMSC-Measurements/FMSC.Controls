
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
        private OpenFileDialogReduxControl _dialogInstance;
        #endregion


        #region ********* Ctor **********
        public OpenFileDialogRedux()
        {
            _dialogInstance = new OpenFileDialogReduxControl();
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


    
}