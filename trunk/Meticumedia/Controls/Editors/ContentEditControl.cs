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
using Microsoft.VisualBasic;

namespace Meticumedia
{
    /// <summary>
    /// Control for editing properties of a movie instance
    /// </summary>
    public partial class ContentEditControl : UserControl
    {
        #region Properties

        /// <summary>
        /// Content that is being editied
        /// </summary>
        public Content Content 
        {
            get { return content; }
            set 
            { 
                content = value;
                if (value != null)
                    DisplayContent(false);
                if (content is TvShow)
                    dvdOrderOnLoad = ((TvShow)value).DvdEpisodeOrder;
            }
        }

        public bool DvdOrderChange { get { return content is TvShow && ((TvShow)content).DvdEpisodeOrder != dvdOrderOnLoad; } }

        /// <summary>
        /// Type of content being edited.
        /// </summary>
        public ContentType ContentType
        {
            get { return cntrlSearch.ContentType; }
            set { cntrlSearch.ContentType = value; }
        }


        public string SearchEntry
        {
            set { cntrlSearch.SearchString = value; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Indicates that movie has changed
        /// </summary>
        public event EventHandler<EventArgs> ContentChanged;

        /// <summary>
        /// Triggers MovieChanged event
        /// </summary>
        protected void OnContentChanged()
        {
            if (ContentChanged != null && !disableContentChangedEvent)
                ContentChanged(this, new EventArgs());
        }

        #endregion

        #region Variables

        private bool dvdOrderOnLoad = false;

        /// <summary>
        /// The content instance being edited.
        /// </summary>
        private Content content = new Content();

        /// <summary>
        /// Flag indicating that movie changed event should be disabled.
        /// </summary>
        private bool disableContentChangedEvent = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ContentEditControl(ContentType type) : this()
        {
            this.ContentType = type;
        }

        /// <summary>
        /// 
        /// </summary>
        public ContentEditControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates content properties from online database
        /// </summary>
        public void UpdateContentInfo()
        {
            this.content.UpdateInfoFromDatabase();
            DisplayContent(false);
        }

        /// <summary>
        /// Display content properties in appropriate controls
        /// </summary>
        private void DisplayContent(bool forceProps)
        {
            disableContentChangedEvent = true;

            // Set which group box to show
            gbOnlineSearch.Visible = content.Id <= 0 && !forceProps;
            gbProperties.Visible = content.Id > 0 || forceProps;

            // Build search string for movie search: use movie name, if empty use folder name
            string searchString = txtName.Text;
            if (string.IsNullOrEmpty(searchString))
            {
                string[] dirs = content.Path.Split('\\');
                searchString = (dirs[dirs.Length - 1]);
            }
            cntrlSearch.SearchString = searchString;
            

            // Set form elements to movie properties
            txtName.Text = this.content.Name;
            numYear.Value = this.content.Date.Year;
            numId.Value = this.content.Id;
            lbGenres.Items.Clear();
            if (this.content.Genres != null)
                foreach (string genre in this.content.Genres)
                    lbGenres.Items.Add(genre);
            disableContentChangedEvent = false;
            chkDoRenaming.Checked = content.DoRenaming;

            if (this.ContentType == Meticumedia.ContentType.TvShow)
            {
                chkDoMissing.Visible = true;
                chkDoMissing.Checked = ((TvShow)content).DoMissingCheck;
                chkIncludeInSchedule.Visible = true;
                chkIncludeInSchedule.Checked = ((TvShow)content).IncludeInSchedule;
                chkDvdOrder.Visible = true;
                chkDvdOrder.Enabled = ((TvShow)content).DataBase != TvDataBaseSelection.TvRage;
                chkDvdOrder.Checked = chkDvdOrder.Enabled && ((TvShow)content).DvdEpisodeOrder;
                cmdDbSel.Items.Clear();
                cmdDbSel.Items.Add(((TvShow)content).DataBase);
                cmdDbSel.SelectedIndex = 0;
                gbAltMatchNames.Visible = true;

                lbAltNames.Items.Clear();
                TvShow show = (TvShow)content;
                if (show.AlternativeNameMatches != null)
                    foreach (string altName in show.AlternativeNameMatches)
                        lbAltNames.Items.Add(altName);

                cntrlSearch.DatabaseSelection = (int)show.DataBase;
            }
            else
            {
                cmdDbSel.Items.Clear();
                cmdDbSel.Items.Add("TheMovieDb");
                cmdDbSel.SelectedIndex = 0;
            }

            // Trigger movie changes event
            OnContentChanged();
        }

        #endregion

        #region Form Event Handlers
       
        /// <summary>
        /// Changes to ID are mirror to movie instances.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numId_ValueChanged(object sender, EventArgs e)
        {
            if (numId.Value == 0)
                numId.BackColor = Color.Red;
            else
                numId.BackColor = Color.White;
            OnContentChanged();
        }

        /// <summary>
        /// Changes to name text box are mirrored on movie instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtName_TextChanged(object sender, EventArgs e)
        {
            this.content.Name = txtName.Text;
            OnContentChanged();
        }

        /// <summary>
        /// Changes to year num are mirrored to movie instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numYear_ValueChanged(object sender, EventArgs e)
        {
            this.content.Date = new DateTime((int)numYear.Value, this.content.Date.Month, this.content.Date.Day);
            OnContentChanged();
        }

        private void chkDoMissing_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ContentType == ContentType.TvShow)
            {
                ((TvShow)content).DoMissingCheck = chkDoMissing.Checked;
                OnContentChanged();
            }
        }

        private void chkIncludeInSchedule_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ContentType == ContentType.TvShow)
            {
                ((TvShow)content).IncludeInSchedule = chkIncludeInSchedule.Checked;
                OnContentChanged();
            }
        }

        private void chkDoRenaming_CheckedChanged(object sender, EventArgs e)
        {
            this.content.DoRenaming = chkDoRenaming.Checked;
            OnContentChanged();
        }

        private void chkDvdOrder_CheckedChanged(object sender, EventArgs e)
        {
            ((TvShow)this.content).DvdEpisodeOrder = chkDvdOrder.Checked;
            OnContentChanged();
        }

        /// <summary>
        /// Add genre button open a selection form for selecting a genre to be added.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddGenre_Click(object sender, EventArgs e)
        {
            // Create array of genre string for selection
            string[] genres = new string[Organization.AllMovieGenres.Count];
            for (int i = 0; i < Organization.AllMovieGenres.Count; i++)
            {
                // Don't list genres already added
                if (content.Genres.Contains(Organization.AllMovieGenres[i]))
                    continue;

                // Add genre name
                genres[i] = Organization.AllMovieGenres[i];
            }

            // Show selection form
            SelectionForm selForm = new SelectionForm("Add Genre", genres);
            selForm.ShowDialog();

            // Check results and match to a genre
            if (!string.IsNullOrEmpty(selForm.Results))
                foreach (string genre in Organization.AllMovieGenres)
                    if (genre == selForm.Results)
                    {
                        this.content.Genres.Add(genre);
                        break;
                    }

            // Refresh display
            DisplayContent(false);
        }

        /// <summary>
        /// Remove genre button removes selected genre from movie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemoveGenre_Click(object sender, EventArgs e)
        {
            // Check selection
            if (lbGenres.SelectedIndices.Count == 0)
                return;

            // Remove genre
            string genreToRemove = lbGenres.SelectedItem.ToString();
            foreach (string genre in this.content.Genres)
                if (genre == genreToRemove)
                {
                    this.content.Genres.Remove(genre);
                    break;
                }

            // Refresh display
            DisplayContent(false);
        }

        private void btnAddMatch_Click(object sender, EventArgs e)
        {
            string newName = Interaction.InputBox("Enter Name");
            if (!string.IsNullOrWhiteSpace(newName))
                ((TvShow)this.content).AlternativeNameMatches.Add(newName);

            // Refresh display
            DisplayContent(false);
        }

        private void btnRemoveMatch_Click(object sender, EventArgs e)
        {

        }

        // <summary>
        /// Switch to database search view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDbSearch_Click(object sender, EventArgs e)
        {
            gbOnlineSearch.Visible = true;
            gbProperties.Visible = false;
        }

        /// <summary>
        /// When content is selected from search results it's properties are copied to show.
        /// </summary>
        private void cntrlSearch_SearchResultsSelected(object sender, SearchControl.SearchResultsSelectedArgs e)
        {
            if (!e.CustomSelected)
            {
                // Update the movie
                if (cntrlSearch.Results != null)
                {
                    switch (this.ContentType)
                    {
                        case ContentType.Movie:
                            ((Movie)this.content).Clone((Movie)cntrlSearch.Results);
                            break;
                        case ContentType.TvShow:
                            ((TvShow)this.content).Clone((TvShow)cntrlSearch.Results);
                            break;
                    }                    
                    this.content.UpdateInfoFromDatabase();
                    this.content.Path = this.content.BuildFolderPath();
                }
            }

            // Update form
            DisplayContent(e.CustomSelected);
        }

        #endregion


    }
}
