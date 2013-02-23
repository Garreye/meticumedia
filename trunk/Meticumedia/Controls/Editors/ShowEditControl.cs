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

namespace Meticumedia.Controls
{
    /// <summary>
    /// Editor for TV show properties.
    /// </summary>
    public partial class ShowEditControl : UserControl
    {
        #region Properties

        /// <summary>
        /// Show that is being editied
        /// </summary>
        public TvShow TvShow
        {
            get { return show; }
            set
            {
                show = value;
                if (value != null)
                    DisplayShow(false);
            }
        }


        #endregion
        
        #region Events

        /// <summary>
        /// Indicates that show has changed
        /// </summary>
        public event EventHandler<EventArgs> ShowChanged;

        /// <summary>
        /// Triggers ShowChanged event
        /// </summary>
        protected void OnShowChanged()
        {
            if (ShowChanged != null && !disableShowChangedEvent)
                ShowChanged(this, new EventArgs());
        }

        #endregion

        #region Variables

        /// <summary>
        /// The movie instance being edited.
        /// </summary>
        private TvShow show = new TvShow();

        /// <summary>
        /// Disables ShowChanged event from triggering when true
        /// </summary>
        private bool disableShowChangedEvent = false;

        /// <summary>
        /// Disables updating show from control - to allow control to be loaded from show
        /// </summary>
        private bool disableUpdating = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with show instance to be edited.
        /// </summary>
        /// <param name="show"></param>
        public ShowEditControl(TvShow show) : this()
        {
            this.show = show;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ShowEditControl()
        {
            InitializeComponent();
        }


        #endregion

        #region Methods

        /// <summary>
        /// Displays properties of show to be edited in control
        /// </summary>
        private void DisplayShow(bool forceProps)
        {
            // Set which group box to show based on whether show ID is valid
            gbOnlineSearch.Visible = this.show.Id <= 0 && !forceProps;
            gbProperties.Visible = this.show.Id > 0 || forceProps;

            // Build search string for movie search: use movie name, if empty use folder name
            string searchString = txtName.Text;
            if (string.IsNullOrEmpty(searchString))
            {
                string[] dirs = show.Path.Split('\\');
                searchString = (dirs[dirs.Length - 1]);
            }
            cntrlSearch.SearchString = searchString;

            // Disable show changed event from triggering (from form event handler)
            disableShowChangedEvent = true;
            disableUpdating = true;

            // Set form elements to movie properties
            txtName.Text = this.show.Name;
            txtDescr.Text = this.show.Overview;
            numYear.Value = this.show.Date.Year;
            numId.Value = this.show.Id;
            chkInlcudeScan.Checked = this.show.IncludeInScan;
            chkDoRenaming.Checked = this.show.DoRenaming;
            chkDoMissing.Checked = this.show.DoMissingCheck;

            // Re-enabled show changed event
            disableShowChangedEvent = false;
            disableUpdating = false;

            // Trigger a single show changed event
            OnShowChanged();
        }

        /// <summary>
        /// Updates show from control
        /// </summary>
        private void UpdateShow()
        {
            if (disableUpdating)
                return;

            this.show.Name = txtName.Text;
            this.show.Overview = txtDescr.Text;
            this.show.Date = new DateTime((int)numYear.Value, this.show.Date.Month, this.show.Date.Day);
            chkDoRenaming.Enabled = chkInlcudeScan.Checked;
            chkDoMissing.Enabled = chkInlcudeScan.Checked;
            this.show.DoRenaming = chkDoRenaming.Checked && chkDoRenaming.Enabled;
            this.show.DoMissingCheck = chkDoMissing.Checked && chkDoMissing.Enabled;
        }

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// When show is selected from search results it's properties are copied to show.
        /// </summary>
        void cntrlSearch_SearchResultsSelected(object sender, SearchControl.SearchResultsSelectedArgs e)
        {
            if (!e.CustomSelected)
            {
                // Update the show
                disableUpdating = true;
                this.show.Name = cntrlSearch.Results.Name;
                this.show.Id = cntrlSearch.Results.Id;
                this.show.Overview = cntrlSearch.Results.Overview;
                this.show.Date = cntrlSearch.Results.Date;
                this.show.Genres = cntrlSearch.Results.Genres;
                TvDatabaseHelper.FullShowSeasonsUpdate(this.show);
                disableUpdating = false;
            }

            // Update form
            DisplayShow(e.CustomSelected);
        }

        /// <summary>
        /// Changes to do renaming check box are mirrored to show instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkDoRenaming_CheckedChanged(object sender, EventArgs e)
        {
            UpdateShow();
            OnShowChanged();
        }

        /// <summary>
        /// ID num box is colored red when ID is invalid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numId_ValueChanged(object sender, EventArgs e)
        {
            if (numId.Value == 0)
                numId.BackColor = Color.Red;
            else
                numId.BackColor = Color.White;
            OnShowChanged();
        }

        /// <summary>
        /// Changes to name text box are mirrored on show instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtName_TextChanged(object sender, EventArgs e)
        {
            UpdateShow();
            OnShowChanged();
        }

        /// <summary>
        /// Changes to description text box are mirrored to show instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDescr_TextChanged(object sender, EventArgs e)
        {
            UpdateShow();
            OnShowChanged();
        }

        /// <summary>
        /// Changes to year num are mirrored to show instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numYear_ValueChanged(object sender, EventArgs e)
        {
            UpdateShow();
            OnShowChanged();
        }

        /// <summary>
        /// Change to include in scan are mirrored to show instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkInlcudeScan_CheckedChanged(object sender, EventArgs e)
        {
            UpdateShow();
            OnShowChanged();
        }

        /// <summary>
        /// Change to do missing checkbox are mirrored to show instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkDoMissing_CheckedChanged(object sender, EventArgs e)
        {
            UpdateShow();
            OnShowChanged();
        }

        /// <summary>
        /// Switch to database search view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDbSearch_Click(object sender, EventArgs e)
        {
            gbOnlineSearch.Visible = true;
            gbProperties.Visible = false;
        }

        
        #endregion



    }
}
