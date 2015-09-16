namespace testBox
{
    partial class TestBeep
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.exclamationBTN = new System.Windows.Forms.Button();
            this.handBtn = new System.Windows.Forms.Button();
            this.questionBtn = new System.Windows.Forms.Button();
            this.asteriskBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // exclamationBTN
            // 
            this.exclamationBTN.Dock = System.Windows.Forms.DockStyle.Top;
            this.exclamationBTN.Location = new System.Drawing.Point(0, 0);
            this.exclamationBTN.Name = "exclamationBTN";
            this.exclamationBTN.Size = new System.Drawing.Size(240, 20);
            this.exclamationBTN.TabIndex = 0;
            this.exclamationBTN.Text = "MB_ICONEXCLAMATION";
            this.exclamationBTN.Click += new System.EventHandler(this.exclamationBTN_Click);
            // 
            // handBtn
            // 
            this.handBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.handBtn.Location = new System.Drawing.Point(0, 20);
            this.handBtn.Name = "handBtn";
            this.handBtn.Size = new System.Drawing.Size(240, 20);
            this.handBtn.TabIndex = 1;
            this.handBtn.Text = "MB_ICONHAND";
            this.handBtn.Click += new System.EventHandler(this.handBtn_Click);
            // 
            // questionBtn
            // 
            this.questionBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.questionBtn.Location = new System.Drawing.Point(0, 40);
            this.questionBtn.Name = "questionBtn";
            this.questionBtn.Size = new System.Drawing.Size(240, 20);
            this.questionBtn.TabIndex = 2;
            this.questionBtn.Text = "MB_ICONQUESTION";
            this.questionBtn.Click += new System.EventHandler(this.questionBtn_Click);
            // 
            // asteriskBtn
            // 
            this.asteriskBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.asteriskBtn.Location = new System.Drawing.Point(0, 60);
            this.asteriskBtn.Name = "asteriskBtn";
            this.asteriskBtn.Size = new System.Drawing.Size(240, 20);
            this.asteriskBtn.TabIndex = 3;
            this.asteriskBtn.Text = "MB_ICONASTERISK";
            this.asteriskBtn.Click += new System.EventHandler(this.asteriskBtn_Click);
            // 
            // TestBeep
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(240, 268);
            this.Controls.Add(this.asteriskBtn);
            this.Controls.Add(this.questionBtn);
            this.Controls.Add(this.handBtn);
            this.Controls.Add(this.exclamationBTN);
            this.Menu = this.mainMenu1;
            this.Name = "TestBeep";
            this.Text = "TestBeep";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button exclamationBTN;
        private System.Windows.Forms.Button handBtn;
        private System.Windows.Forms.Button questionBtn;
        private System.Windows.Forms.Button asteriskBtn;
    }
}