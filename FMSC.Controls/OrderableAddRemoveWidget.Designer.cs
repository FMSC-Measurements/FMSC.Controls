namespace FMSC.Controls
{
    partial class OrderableAddRemoveWidget
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.AddAllButton = new System.Windows.Forms.Button();
            this.AddButton = new System.Windows.Forms.Button();
            this.UpButton = new System.Windows.Forms.Button();
            this.DownButton = new System.Windows.Forms.Button();
            this.RemoveButton = new System.Windows.Forms.Button();
            this.RemoveAllButton = new System.Windows.Forms.Button();
            this._SelectedListBox = new System.Windows.Forms.ListBox();
            this._AvailableListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.AddAllButton, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.AddButton, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.UpButton, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.DownButton, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.RemoveButton, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.RemoveAllButton, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this._SelectedListBox, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this._AvailableListBox, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(338, 229);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // AddAllButton
            // 
            this.AddAllButton.AutoSize = true;
            this.AddAllButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddAllButton.Location = new System.Drawing.Point(141, 37);
            this.AddAllButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.AddAllButton.Name = "AddAllButton";
            this.AddAllButton.Size = new System.Drawing.Size(56, 24);
            this.AddAllButton.TabIndex = 0;
            this.AddAllButton.Text = ">>";
            this.AddAllButton.UseVisualStyleBackColor = true;
            this.AddAllButton.Click += new System.EventHandler(this.AddAllButton_Click);
            // 
            // AddButton
            // 
            this.AddButton.AutoSize = true;
            this.AddButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AddButton.Location = new System.Drawing.Point(141, 67);
            this.AddButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(56, 24);
            this.AddButton.TabIndex = 1;
            this.AddButton.Text = ">";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // UpButton
            // 
            this.UpButton.AutoSize = true;
            this.UpButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UpButton.Location = new System.Drawing.Point(141, 97);
            this.UpButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.UpButton.Name = "UpButton";
            this.UpButton.Size = new System.Drawing.Size(56, 24);
            this.UpButton.TabIndex = 2;
            this.UpButton.Text = "UP";
            this.UpButton.UseVisualStyleBackColor = true;
            this.UpButton.Click += new System.EventHandler(this.UpButton_Click);
            // 
            // DownButton
            // 
            this.DownButton.AutoSize = true;
            this.DownButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DownButton.Location = new System.Drawing.Point(141, 127);
            this.DownButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.DownButton.Name = "DownButton";
            this.DownButton.Size = new System.Drawing.Size(56, 24);
            this.DownButton.TabIndex = 3;
            this.DownButton.Text = "DOWN";
            this.DownButton.UseVisualStyleBackColor = true;
            this.DownButton.Click += new System.EventHandler(this.DownButton_Click);
            // 
            // RemoveButton
            // 
            this.RemoveButton.AutoSize = true;
            this.RemoveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RemoveButton.Location = new System.Drawing.Point(141, 157);
            this.RemoveButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(56, 24);
            this.RemoveButton.TabIndex = 4;
            this.RemoveButton.Text = "<";
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // RemoveAllButton
            // 
            this.RemoveAllButton.AutoSize = true;
            this.RemoveAllButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RemoveAllButton.Location = new System.Drawing.Point(141, 187);
            this.RemoveAllButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.RemoveAllButton.Name = "RemoveAllButton";
            this.RemoveAllButton.Size = new System.Drawing.Size(56, 24);
            this.RemoveAllButton.TabIndex = 5;
            this.RemoveAllButton.Text = "<<";
            this.RemoveAllButton.UseVisualStyleBackColor = true;
            this.RemoveAllButton.Click += new System.EventHandler(this.RemoveAllButton_Click);
            // 
            // _SelectedListBox
            // 
            this._SelectedListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._SelectedListBox.FormattingEnabled = true;
            this._SelectedListBox.IntegralHeight = false;
            this._SelectedListBox.Location = new System.Drawing.Point(201, 23);
            this._SelectedListBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this._SelectedListBox.Name = "_SelectedListBox";
            this.tableLayoutPanel1.SetRowSpan(this._SelectedListBox, 8);
            this._SelectedListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this._SelectedListBox.Size = new System.Drawing.Size(135, 203);
            this._SelectedListBox.TabIndex = 6;
            // 
            // _AvailableListBox
            // 
            this._AvailableListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._AvailableListBox.FormattingEnabled = true;
            this._AvailableListBox.IntegralHeight = false;
            this._AvailableListBox.Location = new System.Drawing.Point(2, 23);
            this._AvailableListBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this._AvailableListBox.Name = "_AvailableListBox";
            this.tableLayoutPanel1.SetRowSpan(this._AvailableListBox, 8);
            this._AvailableListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this._AvailableListBox.Size = new System.Drawing.Size(135, 203);
            this._AvailableListBox.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Available";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(202, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "Selected";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OrderableAddRemoveWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MinimumSize = new System.Drawing.Size(0, 183);
            this.Name = "OrderableAddRemoveWidget";
            this.Size = new System.Drawing.Size(338, 229);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button AddAllButton;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button UpButton;
        private System.Windows.Forms.Button DownButton;
        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.Button RemoveAllButton;
        private System.Windows.Forms.ListBox _SelectedListBox;
        private System.Windows.Forms.ListBox _AvailableListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
