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
    /// Form for searching for content from online database.
    /// </summary>
    public partial class SearchForm : Form
    {
        #region Properties

        /// <summary>
        /// Resulting content from search from online database.
        /// </summary>
        public Content Results { get { return searchControl1.Results; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with preset search string
        /// </summary>
        /// <param name="searchString"></param>
        public SearchForm(string searchString, bool show)
            : this(searchString, show, false)
        { }

        /// <summary>
        /// Constructor with known properties
        /// </summary>
        /// <param name="searchString">Search string</param>
        /// <param name="show">Whether search is for tv show (or movie if false)</param>
        /// <param name="showMatchButton">Whether to show match button - for debugging only</param>
        public SearchForm(string searchString, bool show, bool showMatchButton)
        {
            InitializeComponent();

            // Init wordhelper
            WordHelper.Initialize();

            // Set search string text
            searchControl1.IsShow = show;
            searchControl1.MatchVisible = showMatchButton;
        }

        #endregion

        #region Form Event Handlers
        
        /// <summary>
        /// OK button click sets results to selected item and closes form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (this.Results != null)
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }


        /// <summary>
        /// Cancel button closes form. Results will be null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        #endregion

    }
}
