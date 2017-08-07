namespace FMSC.Controls.Test
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.orderableAddRemoveWidget1 = new FMSC.Controls.OrderableAddRemoveWidget();
            this.SuspendLayout();
            // 
            // orderableAddRemoveWidget1
            // 
            this.orderableAddRemoveWidget1.DisplayMember = null;
            this.orderableAddRemoveWidget1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.orderableAddRemoveWidget1.Location = new System.Drawing.Point(0, 0);
            this.orderableAddRemoveWidget1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.orderableAddRemoveWidget1.MinimumSize = new System.Drawing.Size(0, 183);
            this.orderableAddRemoveWidget1.Name = "orderableAddRemoveWidget1";
            this.orderableAddRemoveWidget1.SelectedItemsDataSource = null;
            this.orderableAddRemoveWidget1.Size = new System.Drawing.Size(570, 415);
            this.orderableAddRemoveWidget1.TabIndex = 0;
            this.orderableAddRemoveWidget1.ValueMember = null;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(570, 415);
            this.Controls.Add(this.orderableAddRemoveWidget1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private OrderableAddRemoveWidget orderableAddRemoveWidget1;
    }
}

