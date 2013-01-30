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
    /// Form for editing TV show properties
    /// </summary>
    public partial class ShowEditForm : Form
    {
        #region Properties

        /// <summary>
        /// Resulting movie from editor form. Null when form cancelled.
        /// </summary>
        public TvShow Results { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with TvShow instance to edit
        /// </summary>
        /// <param name="show"></param>
        public ShowEditForm(TvShow show)
        {
            InitializeComponent();

            // Set show on control
            cntrlShowEdit.TvShow = new TvShow(show);

            // Clear results
            this.Results = null;
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Ok button sets results and closes form.
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Results = cntrlShowEdit.TvShow;
            this.Close();
        }

        /// <summary>
        /// Cancel button closes form (leaving results cleared)
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
