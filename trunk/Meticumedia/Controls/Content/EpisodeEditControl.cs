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

        public bool NumbersEnabled
        {
            set
            {
                numSeason.Enabled = value;
                numEpisode.Enabled = value;
                lblSeason.Enabled = value;
                lblEpisode.Enabled = value;
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// The movie instance being edited.
        /// </summary>
        private TvEpisode episode = new TvEpisode();

        private bool disableEvents = false;

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
            disableEvents = true;

            // Set form elements to movie properties
            txtName.Text = this.episode.Name;
            chkIgnored.Checked = this.episode.Ignored;
            if (this.episode.Number >= 0)
                numEpisode.Value = this.episode.Number;
            else
                numEpisode.Value = 0;
            if (this.episode.Season >= 0)
                numSeason.Value = this.episode.Season;
            else
                numEpisode.Value = 0;

            numYear.Value = this.episode.AirDate.Year;
            numMonth.Value = this.episode.AirDate.Month;
            numDay.Value = this.episode.AirDate.Day;

            txtOverview.Text = this.episode.Overview;

            chkDisableDatabase.Checked = this.episode.PreventDatabaseUpdates;

            disableEvents = false;
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
            if (!disableEvents)
                this.episode.UserDefinedName = txtName.Text;
        }

        /// <summary>
        /// Changes to ignored checkbox are mirror to episode instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkIgnored_CheckedChanged(object sender, EventArgs e)
        {
            if (!disableEvents)
                this.episode.Ignored = chkIgnored.Checked;
        }

        private void numSeason_ValueChanged(object sender, EventArgs e)
        {
            if (!disableEvents)
                this.episode.Season = (int)numSeason.Value;
        }

        private void numEpisode_ValueChanged(object sender, EventArgs e)
        {
            if (!disableEvents)
                this.episode.UserDefinedNumber = (int)numEpisode.Value;
        }

        private void numDate_ValueChanged(object sender, EventArgs e)
        {
            if (!disableEvents)
                this.episode.AirDate = new DateTime((int)numYear.Value, (int)numMonth.Value, (int)numDay.Value);
        }

        private void txtOverview_TextChanged(object sender, EventArgs e)
        {
            if (!disableEvents)
                this.episode.Overview = txtOverview.Text;
        }

        private void chkDisableDatabase_CheckedChanged(object sender, EventArgs e)
        {
            if (!disableEvents)
                this.episode.PreventDatabaseUpdates = chkDisableDatabase.Checked;
        }

        #endregion
    }
}
