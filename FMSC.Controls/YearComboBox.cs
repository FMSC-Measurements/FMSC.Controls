using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FMSC.Controls
{
    public class YearComboBox : ComboBox
    {
        private bool isCurrentYearInRange
        {
            get
            {
                return IsYearInRange(DateTime.Today.Year);
            }
        }

        private bool IsYearInRange(int year)
        {
            return (_endYear > year) && (startYear < year);
        }
        

        private int _endYear = 2099;
        public int EndYear
        {
            get
            {
                return _endYear;
            }
            set
            {
                if (value < startYear) { return; }
                _endYear = value;

                resetDateRange();
            }
        }

        private int startYear = 1900;
        public int StartYear
        {
            get
            {
                return startYear;
            }
            set
            {
                if (value > _endYear) { return; }
                startYear = value;

                resetDateRange();
            }
        }

        private void resetDateRange()
        {
            if (!this.DesignMode)
            {
                this.DataSource = Enumerable.Range(startYear, _endYear - startYear).ToArray();
            }
        }


        public YearComboBox() : base()
        {

        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (isCurrentYearInRange && (String.IsNullOrEmpty(this.Text) || this.SelectedText.Trim() == "0"))
            {
                this.Text = DateTime.Today.Year.ToString();
            }
        }



        protected override void OnTextChanged(EventArgs e)
        {
            try
            {
                int year = Int32.Parse(this.Text);
                if (year <= 0 || !this.IsYearInRange(year))
                {
                    this.Text = DateTime.Today.Year.ToString();
                }
            }
            catch
            {
                //do nothing
            }
            base.OnTextChanged(e);

        }


        //protected override void OnDropDown(EventArgs e)
        //{
        //    base.OnDropDown(e);
        //    if (isCurrentYearInRange && this.SelectedIndex <= 0)
        //    {
        //        this.SelectedIndex = DateTime.Today.Year - startYear;
        //    }
        //}
    }
}
