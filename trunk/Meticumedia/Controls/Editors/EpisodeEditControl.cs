// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Meticumedia
{
    /// <summary>
    /// Control for editing properties of a TV episode.
    /// </summary>
    public partial class EpisodeEditControl : UserControl
    {
        #region Properties

        /// <summary>
        /// Show that is being editied
        /// </summary>
        public TvEpisode Episode
        {
            get { return episode; }
            set 
            { 
                episode = new TvEpisode(value);
                DisplayEpisode();
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// The movie instance being edited.
        /// </summary>
        private TvEpisode episode = new TvEpisode();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public EpisodeEditControl()
        {
            InitializeComponent();
            this.episode = new TvEpisode();
            DisplayEpisode();
            txtName.Focus();
            txtName.SelectAll();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Display TV episode's properties into control
        /// </summary>
        private void DisplayEpisode()
        {
            // Set form elements to movie properties
            txtName.Text = this.episode.Name;
            chkIgnored.Checked = this.episode.Ignored;
        }

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// Changes to name text box are mirrored on episode instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtName_TextChanged(object sender, EventArgs e)
        {
            this.episode.Name = txtName.Text;
        }

        /// <summary>
        /// Changes to ignored checkbox are mirror to episode instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkIgnored_CheckedChanged(object sender, EventArgs e)
        {
            this.episode.Ignored = chkIgnored.Checked;
        }

        #endregion 
    }
}
