// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Meticumedia
{
    /// <summary>
    /// Form for selecting a value form a list of options.
    /// </summary>
    public partial class SelectionForm : Form
    {
        #region Properties

        /// <summary>
        /// Results from selection. Empty string when cancelled.
        /// </summary>
        public string Results { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for generic selection form.
        /// </summary>
        /// <param name="title">Title for form</param>
        /// <param name="options">List of options for selection</param>
        public SelectionForm(string title, string[] options)
        {
            InitializeComponent();

            // Setup display
            this.Text = title;
            foreach (string option in options)
                lbOptions.Items.Add(option);

            // Clear results
            this.Results = string.Empty;
        }

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// Listbox selection enables OK button
        /// </summary>
        private void lbOptions_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = lbOptions.SelectedIndices.Count > 0;
        }

        /// <summary>
        /// Double-click on an item in listbox selects it as the
        /// results and closes the form.
        /// </summary>
        private void lbOptions_DoubleClick(object sender, EventArgs e)
        {
            if (lbOptions.SelectedIndices.Count > 0)
            {
                this.Results = lbOptions.SelectedItem.ToString();
                this.Close();
            }
        }

        /// <summary>
        /// OK button sets current selection to results and closes form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Results = lbOptions.SelectedItem.ToString();
            this.Close();
        }
        
        /// <summary>
        /// Cancel closes form. Results will be empty.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
