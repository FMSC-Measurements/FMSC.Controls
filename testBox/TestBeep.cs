using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FMSC.Controls;

namespace testBox
{
    public partial class TestBeep : Form
    {
        public TestBeep()
        {
            InitializeComponent();
        }

        private void exclamationBTN_Click(object sender, EventArgs e)
        {
            if(!Win32.MessageBeep(1))
            {
                Win32.MessageBeep(0xFFFFFFFF);
            }
        }

        private void handBtn_Click(object sender, EventArgs e)
        {
            if(!Win32.MessageBeep(2))
            {
                Win32.MessageBeep(2);
            }
        }

        private void questionBtn_Click(object sender, EventArgs e)
        {
            if(!Win32.MessageBeep(3))
            {
                Win32.MessageBeep(0xFFFFFFFF);
            }
        }

        private void asteriskBtn_Click(object sender, EventArgs e)
        {
            if(!Win32.MessageBeep(4))
            {
                Win32.MessageBeep(0xFFFFFFFF);
            }
        }
    }
}