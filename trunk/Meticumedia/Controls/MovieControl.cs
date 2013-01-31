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
using System.IO;
using System.Threading;

namespace Meticumedia
{
    /// <summary>
    /// Control for displaying and updating movies.
    /// </summary>
    public partial class MovieControl : UserControl
    {
        #region Variables

        /// <summary>
        /// Worker for updating movie folders.
        /// </summary>
        BackgroundWorker movieUpdater;

        /// <summary>
        /// Flag indicating that update of movies was cancelled and needs to be restarted once cancellation is complete
        /// </summary>
        private bool reUpdateRequired = false;

        /// <summary>
        /// Flag indicating that initialization of movies is required when control is loaded
        /// </summary>
        private bool initRequiredOnLoad = false;

        /// <summary>
        /// Flag indicating that an update of movie folder is required when control is loaded
        /// </summary>
        private bool folderUpdateequiredOnLoad = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Defaul constructor
        /// </summary>
        public MovieControl()
        {
            InitializeComponent();

            // Add load event handler
            this.Load += new EventHandler(ShowsControl_Load);

            // Initialize sort order for movies
            Movie.AscendingSort = true;

            // Start build of genres combo options
            cmbGenre.Items.Add("All Genres");
            cmbGenre.SelectedIndex = 0;

            // Set legend color
            lbInvalid.Color = Color.LightCoral;

            // Display progress bar to show loading progress
            ShowProgressBar();

            // Start genre build
            TheMovieDbHelper.BuildGenres();
            TheMovieDbHelper.GenresChanged += new EventHandler(TheMovieDbHelper_GenreChange);

            // Register progress from movie updating
            Organization.MovieLoadProgressChange += new EventHandler<ProgressChangedEventArgs>(Organization_MovieLoadProgressChange);
            Organization.MovieLoadComplete += new EventHandler(Organization_MovieLoadComplete);
            Organization.MovieFolderUpdateProgressChange += new EventHandler<Organization.OrgProgressChangesEventArgs>(TheMovieDbHelper_FindMovieProgressChange);

            // Setup movie update worker
            movieUpdater = new BackgroundWorker();
            movieUpdater.DoWork += new DoWorkEventHandler(movieUpdater_DoWork);
            movieUpdater.RunWorkerCompleted += new RunWorkerCompletedEventHandler(movieUpdater_RunWorkerCompleted);

            // Setup listview
            lvMovieDirectory.DisplayGenres = true;
            lvMovieDirectory.ItemToEdit += new EventHandler(lvMovieDirectory_ItemToEdit);
            lvMovieDirectory.SaveContentsRequired += new EventHandler(lvMovieDirectory_SaveRequired);
            lvMovieDirectory.UpdateContentsRequired += new EventHandler(lvMovieDirectory_UpdateContentsRequired);
        }

        #endregion

        #region Updating

        /// <summary>
        /// Update Genres in list based on what genres are actually in 
        /// the movies displayed
        /// </summary>
        private void UpdateGenres()
        {
            if (cmbFolders.SelectedItem == null)
                return;

            string selectedGenre = string.Empty;
            if (cmbGenre.SelectedItem != null)
                selectedGenre = cmbGenre.SelectedItem.ToString();

            // Add all available genres
            cmbGenre.Items.Clear();
            cmbGenre.Items.Add("All Genres");
            foreach (string genre in TheMovieDbHelper.GetAvailableGenres(lvMovieDirectory.Contents))
            {
                int item = cmbGenre.Items.Add(genre);

                // Reselect genre
                if (selectedGenre == genre)
                    cmbGenre.SelectedIndex = item;
            }

            if (cmbGenre.SelectedIndex < 0)
                cmbGenre.SelectedIndex = 0;
        }

        /// <summary>
        /// Updates movies from Organization based on selected folder.
        /// </summary>
        public void UpdateMovies(bool newOnly)
        {
            // Get currently selected folder
            string currentSel = string.Empty;
            if (lvMovieDirectory.SelectedItems.Count > 0)
                currentSel = lvMovieDirectory.SelectedItems[0].ToString();

            string folderName = "All";
            if (cmbFolders.SelectedItem != null)
                folderName = cmbFolders.SelectedItem.ToString();
            string genre = "All";
            if(cmbGenre.SelectedItem != null)
                genre = cmbGenre.SelectedItem.ToString();

            lvMovieDirectory.Contents = Organization.GetMoviesFromRootFolders(folderName, genre, chkYearFilter.Checked, (int)numMinYear.Value, (int)numMaxYear.Value, txtNameFilter.Text);
            if (!newOnly)
                lvMovieDirectory.SortContents(lvMovieDirectory.lastSortColumn, true);
            else
                lvMovieDirectory.DisplayContent(true);
            
        }

        /// <summary>
        /// Updates folders that are displayed in combo box. If handle for
        /// control is not created yet (update was requested right at start-up)
        /// then flag is set indicating update needs to be done on load.
        /// </summary>
        public void UpdateFolders()
        {
            // Check that handle is created
            if (!this.IsHandleCreated)
            {
                folderUpdateequiredOnLoad = true;
                return;
            }

            // Update movies for display
            this.Invoke((MethodInvoker)delegate
            {
                DoUpdateFolders();
            });
        }

        /// <summary>
        /// Updates folders that are displayed in combo box.
        /// Attempts to maintain selection.
        /// </summary>
        private void DoUpdateFolders()
        {            
            // Get currently selected folder
            string selected = string.Empty;
            if (cmbFolders.SelectedItem != null)
                selected = cmbFolders.SelectedItem.ToString();
            
            // Set default index to select
            int indexToSelect = 0;

            // Clear items and add all option
            cmbFolders.Items.Clear();
            cmbFolders.Items.Add("All Movie Folders");

            // Add each movie folder from settings
            foreach (ContentRootFolder folder in Settings.MovieFolders)
            {
                int index = cmbFolders.Items.Add(folder);

                // If this folder was selected than set its index to be selected again
                if (folder.ToString() == selected)
                    indexToSelect = index;
            }

            // Set selected index
            cmbFolders.SelectedIndex = indexToSelect;
        }

        /// <summary>
        /// Run background worker that updates movies found in selected folder.
        /// </summary>
        public void UpdateMoviesInFolders()
        {
            // Show progress bar
            pbUpdating.Value = 0;

            btnForceRefresh.Enabled = false;

            if (movieUpdater.IsBusy)
            {
                reUpdateRequired = true;
                Organization.CancelTvUpdating();
            }
            else 
            {
                ShowProgressBar();
                if (cmbFolders.SelectedItem != null)
                    movieUpdater.RunWorkerAsync(cmbFolders.SelectedItem.ToString());
                else
                    movieUpdater.RunWorkerAsync("All");
            }
        }

        /// <summary>
        /// Flag indicating if progress bar is currently shown.
        /// </summary>
        private bool progressBarShown = false;

        /// <summary>
        /// Shows progress bar. List need to be moved out of the way.
        /// </summary>
        private void ShowProgressBar()
        {
            if (!progressBarShown)
            {
                lvMovieDirectory.Top += 30;
                lvMovieDirectory.Height -= 30;
                progressBarShown = true;
            }
        }

        /// <summary>
        /// Hide progress bar, list is moved up to hide it.
        /// </summary>
        private void HideProgressBar()
        {
            if (progressBarShown)
            {
                lvMovieDirectory.Top -= 30;
                lvMovieDirectory.Height += 30;
                progressBarShown = false;
            }
        }

        /// <summary>
        /// Asynchronous work for updating movies found in movie folders.
        /// </summary>
        private void movieUpdater_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get movie folders to update from arguments
            string folder = (string)e.Argument;

            // Update each selected movie folder
            foreach (ContentRootFolder movieFldr in Settings.MovieFolders)
                if (folder.StartsWith("All") || movieFldr.FullPath == folder)
                    if (!Organization.UpdateMovieFolder(movieFldr))
                        break;
        }

        /// <summary>
        /// When a new movies is found from updating add it to the listview
        /// </summary>
        private void TheMovieDbHelper_FindMovieProgressChange(object sender, Organization.OrgProgressChangesEventArgs e)
        {
            if (pbUpdating.Value != e.ProgressPercentage)
            {
                if (this.InvokeRequired)
                    this.Invoke((MethodInvoker)delegate
                    {
                        UpdataProgress(e.ProgressPercentage, (string)e.UserState);
                    });
                else
                    UpdataProgress(e.ProgressPercentage, (string)e.UserState);

                if (e.NewItem)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        UpdateMovies(true);
                        UpdateGenres();
                    });
                }
            }

        }

        /// <summary>
        /// Updates progress bar percent and message
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="msg"></param>
        private void UpdataProgress(int percent, string msg)
        {
            pbUpdating.Message = msg;
            pbUpdating.Value = percent;
        }
        
        /// <summary>
        /// Called when movie folder updating is completed. Refresh display of movies.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void movieUpdater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (reUpdateRequired)
            {
                reUpdateRequired = false;
                movieUpdater.RunWorkerAsync(cmbFolders.SelectedItem == null ? "All" : cmbFolders.SelectedItem.ToString());
            }
            else
            {
                if (!this.IsHandleCreated)
                {
                    initRequiredOnLoad = true;
                    return;
                }
                
                // Update movies for display
                this.BeginInvoke((MethodInvoker)delegate
                {
                    InitializeMovies();
                });
            }
        }

        /// <summary>
        /// Handler for control load event.
        /// </summary>
        void ShowsControl_Load(object sender, EventArgs e)
        {
            // Perform initialization of movie if required
            if (initRequiredOnLoad)
            {
                initRequiredOnLoad = false;    
                InitializeMovies();
            }

            // Perform update of movie folders if required
            if (folderUpdateequiredOnLoad)
            {
                folderUpdateequiredOnLoad = false;
                DoUpdateFolders();
            }

        }

        /// <summary>
        /// Initializes movies info in control
        /// </summary>
        private void InitializeMovies()
        {
            UpdateMovies(false);
            UpdateGenres();
            btnForceRefresh.Enabled = true;
            if (!movieUpdater.IsBusy)
                HideProgressBar();
        }

        /// <summary>
        /// Display progress during loading of movie data at startup.
        /// </summary>
        private void Organization_MovieLoadProgressChange(object sender, ProgressChangedEventArgs e)
        {
            if (!this.IsHandleCreated)
                return;

            if (pbUpdating.Value != e.ProgressPercentage)
            {
                if (this.InvokeRequired)
                    this.Invoke((MethodInvoker)delegate
                    {
                        UpdataProgress(e.ProgressPercentage, "Loading Movies" + (string)e.UserState);
                    });
                else
                    UpdataProgress(e.ProgressPercentage, "Loading Movies" + (string)e.UserState);
            }
        }

        /// <summary>
        /// Trigger update of movie once loading is complete at startup.
        /// </summary>
        private void Organization_MovieLoadComplete(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
                this.Invoke((MethodInvoker)delegate
                {
                    UpdateMovies(false);
                    
                    UpdateMoviesInFolders();
                });
            else
            {
                UpdateMovies(false);
                UpdateMoviesInFolders();
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Perform update of movie on update required event from content listview
        /// </summary>
        void lvMovieDirectory_UpdateContentsRequired(object sender, EventArgs e)
        {
            UpdateMoviesInFolders();
        }

        /// <summary>
        /// Perform save of movie on save required event from content listview
        /// </summary>
        void lvMovieDirectory_SaveRequired(object sender, EventArgs e)
        {
            Organization.SaveMovies();
        }

        /// <summary>
        /// Opens movie editor on edit required event from content listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lvMovieDirectory_ItemToEdit(object sender, EventArgs e)
        {
            EditMovie();
        }

        /// <summary>
        /// Rebuilds genres in combobox when build from database is complete.
        /// </summary>
        private void TheMovieDbHelper_GenreChange(object sender, EventArgs e)
        {
            UpdateGenres();
        }

        /// <summary>
        /// Updates displayed movies when selected movie folder is changed.
        /// </summary>
        private void cmbFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateMovies(false);
            UpdateGenres();
        }

        private string lastSelectedGenre = string.Empty;

        /// <summary>
        /// Updates displayed movies when selected genre is changed.
        /// </summary>
        private void cmbGenre_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGenre.SelectedIndex >= 0)
            {
                string selectedGenre = cmbGenre.SelectedItem.ToString();
                if (selectedGenre != lastSelectedGenre)
                    UpdateMovies(false);
                lastSelectedGenre = selectedGenre;
            }
        }

        /// <summary>
        /// Open movie editor when edit buton is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditMovie_Click(object sender, EventArgs e)
        {
            EditMovie();
        }

        /// <summary>
        /// Open editor for editing movie that is selected in listview
        /// </summary>
        private void EditMovie()
        {
            // Get the movie
            Movie selMovie;
            if (!GetSeletectedMovie(out selMovie))
                return;

            // Open Movie editor
            MovieEditorForm mef = new MovieEditorForm(selMovie);
            mef.ShowDialog();

            // Update movie
            if (mef.Results != null)
            {
                selMovie.UpdateInfo(mef.Results);
                Organization.SaveMovies();
                lvMovieDirectory.DisplayContent(false);
            }
        }

        /// <summary>
        /// Watched properties is toggle for selected show when watched button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWatchMod_Click(object sender, EventArgs e)
        {
            lvMovieDirectory.SetWatchedForSelected((Button)sender == btnWatched);
        }

        /// <summary>
        /// Gets the currently selected movies from the movie listview.
        /// </summary>
        /// <param name="movies">Resulting selected movies</param>
        /// <returns>Whether any selected movies were found</returns>
        private bool GetSeletectedMovies(out List<Content> movies)
        {
            return lvMovieDirectory.GetSeletectedContents(out movies);
        }

        /// <summary>
        /// Get first selected movie in movies listview.
        /// </summary>
        /// <param name="movie">Resulting selected movie</param>
        /// <returns>Whether a selected movie was found</returns>
        private bool GetSeletectedMovie(out Movie movie)
        {
            // Initialize selected movie
            movie = null;
            
            // Get all selected movies, if none found return false
            List<Content> selMovies;
            if (!GetSeletectedMovies(out selMovies))
                return false;

            // Set selected movie to first selected and return true
            movie = selMovies[0] as Movie;
            return true;
        }

        /// <summary>
        /// Force refresh updates movies that are found in selected movie folder(s)
        /// </summary>
        private void btnForceRefresh_Click(object sender, EventArgs e)
        {
            UpdateMoviesInFolders();
        }

        /// <summary>
        /// Edit list button open settings to movie folders tabs.
        /// </summary>
        private void btnEditDir_Click(object sender, EventArgs e)
        {
            SettingsForm settingForm = new SettingsForm(2);
            settingForm.ShowDialog();
        }

        /// <summary>
        /// Change of hide watched checkbox re-displays movies with watched
        /// shown/unshown toggled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkHideWatched_CheckedChanged(object sender, EventArgs e)
        {
            lvMovieDirectory.HideWatched = chkHideWatched.Checked;
        }

        private void chkYearFilter_CheckedChanged(object sender, EventArgs e)
        {
            UpdateMovies(false);
        }

        private void numMinYear_ValueChanged(object sender, EventArgs e)
        {
            UpdateMovies(false);
        }

        private void numMaxYear_ValueChanged(object sender, EventArgs e)
        {
            UpdateMovies(false);
        }

        private void txtNameFilter_TextChanged(object sender, EventArgs e)
        {
            UpdateMovies(false);
            txtNameFilter.Select();
        }

        #endregion
    }
}
