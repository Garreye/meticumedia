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
    /// Form for editing properties of a Movie object.
    /// </summary>
    public partial class ContentEditorForm : Form
    {
        #region Properties

        /// <summary>
        /// Resulting movie from editor form. Null when form cancelled.
        /// </summary>
        public Content Results { get; private set; }

        public bool DvdOrderChange { get { return cntrlContent.DvdOrderChange; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with content to be edited passed in.
        /// </summary>
        /// <param name="movie"></param>
        public ContentEditorForm(Content content)
        {
            InitializeComponent();

            // Clone content (so that changes here won't affect original unintentionally)
            if (content is Movie)
            {
                cntrlContent.ContentType = ContentType.Movie;
                cntrlContent.Content = new Movie((Movie)content);
            }
            else
            {
                cntrlContent.ContentType = ContentType.TvShow;
                cntrlContent.Content = new TvShow((TvShow)content);
            }

            SetUpdateButtonEnable();
            cntrlContent.ContentChanged += new EventHandler<EventArgs>(cntrlContent_ContentChanged);

            // Clear results
            this.Results = null;
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Enable update button if movie ID is valid.
        /// </summary>
        private void SetUpdateButtonEnable()
        {
            // Enable update button for valid IDs only
            btnUpdateInfo.Enabled = cntrlContent.Content.Id != 0;
        }

        /// <summary>
        /// Movie changed on movie editor control cause update button enable to be updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cntrlContent_ContentChanged(object sender, EventArgs e)
        {
            SetUpdateButtonEnable();
        }

        /// <summary>
        /// Update movie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateInfo_Click(object sender, EventArgs e)
        {
            cntrlContent.UpdateContentInfo();
        }

        /// <summary>
        /// When OK button click the results are set and form is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Results = cntrlContent.Content;
            this.Close();
        }

        /// <summary>
        /// Cancel button simply closes the form. (Results will be null)
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
