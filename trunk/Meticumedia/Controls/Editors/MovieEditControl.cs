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
    /// Control for editing properties of a movie instance
    /// </summary>
    public partial class MovieEditControl : UserControl
    {
        #region Properties

        /// <summary>
        /// Movie that is being editied
        /// </summary>
        public Movie Movie 
        {
            get { return movie; }
            set 
            { 
                movie = value;
                if(value != null)
                DisplayMovie(false);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Indicates that movie has changed
        /// </summary>
        public event EventHandler<EventArgs> MovieChanged;

        /// <summary>
        /// Triggers MovieChanged event
        /// </summary>
        protected void OnMovieChanged()
        {
            if (MovieChanged != null && !disableMovieChangedEvent)
                MovieChanged(this, new EventArgs());
        }

        #endregion

        #region Variables

        /// <summary>
        /// The movie instance being edited.
        /// </summary>
        private Movie movie = new Movie();

        /// <summary>
        /// Flag indicating that movie changed event should be disabled.
        /// </summary>
        private bool disableMovieChangedEvent = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MovieEditControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates movie properties from online database
        /// </summary>
        public void UpdateMovieInfo()
        {
            // 
            this.movie = TheMovieDbHelper.UpdateMovieInfo(this.movie);
            DisplayMovie(false);
        }

        /// <summary>
        /// Display movie properties in appropriate controls
        /// </summary>
        private void DisplayMovie(bool forceProps)
        {
            disableMovieChangedEvent = true;

            // Set which group box to show
            gbOnlineSearch.Visible = movie.Id <= 0 && !forceProps;
            gbProperties.Visible = movie.Id > 0 || forceProps;

            // Build search string for movie search: use movie name, if empty use folder name
            string searchString = txtName.Text;
            if (string.IsNullOrEmpty(searchString))
            {
                string[] dirs = movie.Path.Split('\\');
                searchString = (dirs[dirs.Length - 1]);
            }
            cntrlSearch.SearchString = searchString;

            // Set form elements to movie properties
            txtName.Text = this.movie.Name;
            numYear.Value = this.movie.Date.Year;
            numId.Value = this.movie.Id;
            lbGenres.Items.Clear();
            if (this.movie.Genres != null)
                foreach (string genre in this.movie.Genres)
                    lbGenres.Items.Add(genre);

            disableMovieChangedEvent = false;

            // Trigger movie changes event
            OnMovieChanged();
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
            OnMovieChanged();
        }

        /// <summary>
        /// Changes to name text box are mirrored on movie instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtName_TextChanged(object sender, EventArgs e)
        {
            this.movie.Name = txtName.Text;
            OnMovieChanged();
        }

        /// <summary>
        /// Changes to year num are mirrored to movie instance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numYear_ValueChanged(object sender, EventArgs e)
        {
            this.movie.Date = new DateTime((int)numYear.Value, this.movie.Date.Month, this.movie.Date.Day);
            OnMovieChanged();
        }

        /// <summary>
        /// Add genre button open a selection form for selecting a genre to be added.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddGenre_Click(object sender, EventArgs e)
        {
            // Create array of genre string for selection
            string[] genres = new string[TheMovieDbHelper.AllGenres.Count];
            for (int i = 0; i < TheMovieDbHelper.AllGenres.Count; i++)
            {
                // Don't add genre's already in the movies list
                bool alreadyAdded = false;
                if (this.movie.Genres != null)
                    foreach (string movieGenre in this.movie.Genres)
                        if (TheMovieDbHelper.AllGenres[i] == this.movie.Name)
                        {
                            alreadyAdded = true;
                            break;
                        }

                if (alreadyAdded)
                    continue;

                // Add genre name
                genres[i] = TheMovieDbHelper.AllGenres[i];
            }

            // Show selection form
            SelectionForm selForm = new SelectionForm("Add Genre", genres);
            selForm.ShowDialog();

            // Check results and match to a genre
            if (!string.IsNullOrEmpty(selForm.Results))
                foreach (string genre in TheMovieDbHelper.AllGenres)
                    if (genre == selForm.Results)
                    {
                        this.movie.Genres.Add(genre);
                        break;
                    }

            // Refresh display
            DisplayMovie(false);
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
            foreach (string genre in this.movie.Genres)
                if (genre == genreToRemove)
                {
                    this.movie.Genres.Remove(genre);
                    break;
                }

            // Refresh display
            DisplayMovie(false);
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
        /// When show is selected from search results it's properties are copied to show.
        /// </summary>
        private void cntrlSearch_SearchResultsSelected(object sender, SearchControl.SearchResultsSelectedArgs e)
        {
            if (!e.CustomSelected)
            {
                // Update the movie
                if (cntrlSearch.Results != null)
                {
                    this.movie.UpdateInfo((Movie)cntrlSearch.Results);
                    this.movie = TheMovieDbHelper.UpdateMovieInfo(this.movie);
                    this.movie.Path = this.movie.BuildFolderPath();
                }
            }

            // Update form
            DisplayMovie(e.CustomSelected);
        }

        #endregion




    }
}
