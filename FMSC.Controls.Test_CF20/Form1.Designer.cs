namespace testBox
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.editableDataGrid1 = new FMSC.Controls.EditableDataGrid();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridTableStyle1 = new System.Windows.Forms.DataGridTableStyle();
            this.dataGridTableStyle2 = new System.Windows.Forms.DataGridTableStyle();
            this.dataGridTableStyle3 = new System.Windows.Forms.DataGridTableStyle();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.comboBoxRedux1 = new FMSC.Controls.Mobile.ComboBoxRedux();
            this.buttonPanel1 = new FMSC.Controls.Mobile.ButtonPanel();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.editableDataGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataSource = typeof(testBox.something);
            // 
            // editableDataGrid1
            // 
            this.editableDataGrid1.BackColor = System.Drawing.SystemColors.Highlight;
            this.editableDataGrid1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.editableDataGrid1.DataSource = this.bindingSource1;
            this.editableDataGrid1.ErrorColor = System.Drawing.Color.Red;
            this.editableDataGrid1.Location = new System.Drawing.Point(0, 134);
            this.editableDataGrid1.Name = "editableDataGrid1";
            this.editableDataGrid1.SelectionForeColor = System.Drawing.SystemColors.Info;
            this.editableDataGrid1.Size = new System.Drawing.Size(240, 131);
            this.editableDataGrid1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 20);
            this.label1.Text = "label1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // dataGridTableStyle1
            // 
            this.dataGridTableStyle1.MappingName = "something";
            // 
            // dataGridTableStyle2
            // 
            this.dataGridTableStyle2.MappingName = "something";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 33);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(49, 20);
            this.button1.TabIndex = 1;
            this.button1.Text = "OFD";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(165, 10);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(72, 20);
            this.button2.TabIndex = 3;
            this.button2.Text = "Do Stuff";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // comboBoxRedux1
            // 
            this.comboBoxRedux1.Location = new System.Drawing.Point(64, 83);
            this.comboBoxRedux1.Name = "comboBoxRedux1";
            this.comboBoxRedux1.Size = new System.Drawing.Size(100, 25);
            this.comboBoxRedux1.TabIndex = 5;
            this.comboBoxRedux1.Text = "comboBoxRedux1";
            // 
            // buttonPanel1
            // 
            this.buttonPanel1.Location = new System.Drawing.Point(165, 37);
            this.buttonPanel1.Name = "buttonPanel1";
            this.buttonPanel1.Size = new System.Drawing.Size(737, 20);
            this.buttonPanel1.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(949, 643);
            this.Controls.Add(this.buttonPanel1);
            this.Controls.Add(this.comboBoxRedux1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.editableDataGrid1);
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.editableDataGrid1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private FMSC.Controls.EditableDataGrid editableDataGrid1;
        private Microsoft.WindowsCE.Forms.InputPanel inputPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.DataGridTableStyle dataGridTableStyle1;
        private System.Windows.Forms.DataGridTableStyle dataGridTableStyle2;
        private System.Windows.Forms.DataGridTableStyle dataGridTableStyle3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private FMSC.Controls.Mobile.ComboBoxRedux comboBoxRedux1;
        private FMSC.Controls.Mobile.ButtonPanel buttonPanel1;
    }
}

