using Bogus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FMSC.Controls.Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var words = new Faker().Lorem.Words(10).ToList();
            orderableAddRemoveWidget1.DataSource = words;
            orderableAddRemoveWidget1.SelectedItemsDataSource = new List<string>();
        }
    }
}
