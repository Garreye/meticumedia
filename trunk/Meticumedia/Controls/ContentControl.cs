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
    public partial class ContentControl : UserControl
    {
        #region Variables

        /// <summary>
        /// Worker for updating root folders.
        /// </summary>
        BackgroundWorker rootFolderUpdater;

        /// <summary>
        /// Flag indicating that update of root folders was cancelled and needs to be restarted once cancellation is complete
        /// </summary>
        private bool reUpdateRequired = false;

        /// <summary>
        /// Flag indicating that root folder synchronization is required when control is loaded
        /// </summary>
        private bool rootFolderSyncOnLoad = false;

        /// <summary>
        /// Flag indicating that content synchronization is required when control is loaded
        /// </summary>
        private bool contentSyncOnLoad = false;

        /// <summary>
        /// Flag indicating that an update of root folders is required when control is loaded
        /// </summary>
        private bool folderUpdateRequiredOnLoad = false;

        /// <summary>
        /// Type of content displayed in control
        /// </summary>
        public ContentType ContentType { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Event indicating there are items to be sent to the queue
        /// </summary>
        public static event EventHandler<ItemsToQueueArgs> ItemsToQueue;

        /// <summary>
        /// Triggers ItemsToQueue event
        /// </summary>
        /// <param name="items"></param>
        protected static void OnItemsToQueue(List<OrgItem> items)
        {
            if (ItemsToQueue != null)
                ItemsToQueue(null, new ItemsToQueueArgs(items));
        }

        public event EventHandler<SelectionChangedArgs> SelectionChanged;

        protected void OnSelectionChanged(List<Content> items)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, new SelectionChangedArgs(items));
        }

        public class SelectionChangedArgs : EventArgs
        {
            /// <summary>
            /// List of organization item to be queued
            /// </summary>
            public List<Content> Selections { get; private set; }

            /// <summary>
            /// Constructor with item to be queue
            /// </summary>
            /// <param name="items">Items to be queue</param>
            public SelectionChangedArgs(List<Content> items)
            {
                this.Selections = items;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Defaul constructor
        /// </summary>
        public ContentControl(ContentType type)
        {
            this.ContentType = type;

            InitializeComponent();

            // Add load event handler
            this.Load += new EventHandler(Control_Load);

            // Start build of genres combo options
            cmbGenre.Items.Add("All Genres");
            cmbGenre.SelectedIndex = 0;

            // Set legend color
            lbInvalid.Color = Color.LightCoral;

            // Display progress bar to show loading progress
            ShowProgressBar();

            // Register progress from movie updating
            switch (this.ContentType)
            {
                case ContentType.Movie:
                    Organization.Movies.LoadProgressChange += new EventHandler<ProgressChangedEventArgs>(Organization_LoadProgressChange);
                    Organization.Movies.LoadComplete += new EventHandler(Organization_LoadComplete);
                    ContentRootFolder.UpdateProgressChange += new EventHandler<OrgProgressChangesEventArgs>(Organization_FolderUpdateProgressChange);
                    break;
                case ContentType.TvShow:
                    Organization.Shows.LoadProgressChange += new EventHandler<ProgressChangedEventArgs>(Organization_LoadProgressChange);
                    Organization.Shows.LoadComplete += new EventHandler(Organization_LoadComplete);
                    ContentRootFolder.UpdateProgressChange += new EventHandler<OrgProgressChangesEventArgs>(Organization_FolderUpdateProgressChange);
                    break;
                default:
                    throw new Exception("Unknown content type");
            }

            // Setup root folder update worker
            rootFolderUpdater = new BackgroundWorker();
            rootFolderUpdater.DoWork += new DoWorkEventHandler(rootFolderUpdater_DoWork);
            rootFolderUpdater.RunWorkerCompleted += new RunWorkerCompletedEventHandler(rootFolderUpdater_RunWorkerCompleted);

            // Setup listview
            lvContentFolders.ItemToEdit += new EventHandler(lvContentFolders_ItemToEdit);
            lvContentFolders.SaveContentsRequired += new EventHandler(lvContentFolders_SaveRequired);
            lvContentFolders.UpdateContentsRequired += new EventHandler(lvContentFolders_UpdateContentsRequired);
        }

        #endregion

        #region Updating

        /// <summary>
        /// Get root folder instances from combo box
        /// </summary>
        /// <returns>List of root folders that are selected</returns>
        public List<ContentRootFolder> GetSelectedRootFolders()
        {
            // Get all root folder
            List<ContentRootFolder> allRootFolders = Settings.GetAllRootFolders(this.ContentType);

            // Get selection string
            if (cmbFolders.SelectedItem == null)
                return allRootFolders;
            string selFolderStr = cmbFolders.SelectedItem.ToString();

            // Return all if selected
            if (selFolderStr.StartsWith("All"))
                return allRootFolders;

            // Return single select root folder as list
            List<ContentRootFolder> selFolders = new List<ContentRootFolder>();
            foreach (ContentRootFolder folder in allRootFolders)
                if (folder.FullPath == selFolderStr)
                {
                    selFolders.Add(folder);
                    break;
                }
            return selFolders;
        }

        public List<ContentRootFolder> GetFilteredRootFolders(out bool recursive)
        {
            List<ContentRootFolder> baseRootFolders = GetSelectedRootFolders();
            recursive = false;

            if (cmbRootFilter.Text == "Recursive")
            {
                recursive = true;
                return baseRootFolders;
            }

            string rootFolderPath = cmbRootFilter.Text.Replace("Non-recursive: ", "");
            ContentRootFolder filteredFolder;
            if (baseRootFolders.Count > 0 && GetMatchingRootFolder(rootFolderPath, baseRootFolders[0], out filteredFolder))
            {
                List<ContentRootFolder> filteredList = new List<ContentRootFolder>();
                filteredList.Add(filteredFolder);
                return filteredList;
            }

            return baseRootFolders;
        }

        private bool GetMatchingRootFolder(string path, ContentRootFolder baseFolder, out ContentRootFolder matchedFolder)
        {
            if (path == baseFolder.FullPath)
            {
                matchedFolder = baseFolder;
                return true;
            }
            else
                foreach(ContentRootFolder child in baseFolder.ChildFolders)
                {
                    if(GetMatchingRootFolder(path, child, out matchedFolder))
                        return true;
                }

            matchedFolder = null;
            return false;
        }

        /// <summary>
        /// Get genre instances from combo box
        /// </summary>
        /// <returns>List of genres that are selected</returns>
        public GenreCollection GetSelectedGenres()
        {
            // Get all genres
            GenreCollection allGenres = Organization.GetAllGenres(this.ContentType);

            // Get selection string
            if (cmbGenre.SelectedItem == null)
                return allGenres;
            string selGenreStr = cmbGenre.SelectedItem.ToString();

            // Return all if selected
            if (selGenreStr.StartsWith("All"))
                return allGenres;

            // Return single select genre as list
            GenreCollection genres = new GenreCollection(this.ContentType);
            foreach(string genre in allGenres)
                if (selGenreStr == genre)
                {
                    genres.Add(genre);
                    break;
                }
            return genres;
        }

        /// <summary>
        /// Update Genres in combo box based on what genres are actually in the content displayed
        /// </summary>
        private void UpdateGenresComboBox()
        {
            // Save current selection
            string selectedGenre = string.Empty;
            if (cmbGenre.SelectedItem != null)
                selectedGenre = cmbGenre.SelectedItem.ToString();

            // Get selected folder
            string selFolderStr = "All";
            if (cmbFolders.SelectedItem != null)
                selFolderStr = cmbFolders.SelectedItem.ToString();

            // Add all available genres
            GenreCollection genres = null;
            switch (ContentType)
            {
                case ContentType.Movie:
                    if (selFolderStr.StartsWith("All"))
                        genres = Organization.AllMovieGenres;
                    else
                        genres = GetSelectedRootFolders()[0].GetGenres();
                    break;
                case ContentType.TvShow:
                    if (selFolderStr.StartsWith("All"))
                        genres = Organization.AllTvGenres;
                    else
                        genres = GetSelectedRootFolders()[0].GetGenres();
                    break;
                default:
                    throw new Exception("Unknown content type");
            }

            // Add genres to combo box
            cmbGenre.Items.Clear();
            cmbGenre.Items.Add("All Genres");
            foreach (string genre in genres)
            {
                int item = cmbGenre.Items.Add(genre);

                // Reselect genre
                if (selectedGenre == genre)
                    cmbGenre.SelectedIndex = item;
            }

            // Set default selection if needed
            if (cmbGenre.SelectedIndex < 0)
                cmbGenre.SelectedIndex = 0;
        }

        /// <summary>
        /// Updates movies from Organization based on selected folder.
        /// </summary>
        public void UpdateContent(bool newOnly)
        {
            // Save current selection from listview
            string currentSel = string.Empty;
            if (lvContentFolders.SelectedItems.Count > 0)
                currentSel = lvContentFolders.SelectedItems[0].ToString();

            // Get selected folder
            string folderName = "All";
            if (cmbFolders.SelectedItem != null)
                folderName = cmbFolders.SelectedItem.ToString();

            // Get root folders
            bool recursive;
            List<ContentRootFolder> selRootFolders = GetFilteredRootFolders(out recursive);

            // Set contents for listview
            switch (ContentType)
            {
                case ContentType.Movie:
                    lvContentFolders.Contents = Organization.GetContentFromRootFolders(selRootFolders, recursive, GetSelectedGenres(), chkYearFilter.Checked, (int)numMinYear.Value, (int)numMaxYear.Value, txtNameFilter.Text);        
                    break;
                case ContentType.TvShow:
                    lvContentFolders.Contents = Organization.GetContentFromRootFolders(selRootFolders, recursive, GetSelectedGenres(), chkYearFilter.Checked, (int)numMinYear.Value, (int)numMaxYear.Value, txtNameFilter.Text);
                    break;
                default:
                    throw new Exception("Unknown content type");
            }
            
            if (!newOnly)
                lvContentFolders.SortContents(lvContentFolders.lastSortColumn, true);
            else
                lvContentFolders.DisplayContent(true);
            
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
                folderUpdateRequiredOnLoad = true;
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
            cmbFolders.Items.Add("All Root Folders");

            // Add each movie folder from settings
            foreach (ContentRootFolder folder in Settings.GetAllRootFolders(this.ContentType))
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
        /// Update root folder filters list
        /// </summary>
        private void UpdateRootFilters()
        {
            cmbRootFilter.Items.Clear();
            cmbRootFilter.Items.Add("Recursive");
            if (cmbFolders.SelectedIndex > 0)
                AddRootFolderFilterItems(GetSelectedRootFolders()[0]);
            cmbRootFilter.SelectedIndex = 0;
        }

        private void AddRootFolderFilterItems(ContentRootFolder rootFolder)
        {
            cmbRootFilter.Items.Add("Non-recursive: " + rootFolder.FullPath);
            foreach (ContentRootFolder child in rootFolder.ChildFolders)
                AddRootFolderFilterItems(child);
        }

        /// <summary>
        /// Run background worker that updates content found in selected folder.
        /// </summary>
        public void UpdateContentInFolders(bool full)
        {
            // Show progress bar
            pbUpdating.Value = 0;
            btnForceRefresh.Enabled = false;

            // If update is already running, cancel it
            if (rootFolderUpdater.IsBusy)
            {
                reUpdateRequired = true;
                Organization.CancelFolderUpdating(this.ContentType);
            }
            else 
            {
                ShowProgressBar();
                if (cmbFolders.SelectedItem != null)
                    rootFolderUpdater.RunWorkerAsync(cmbFolders.SelectedItem.ToString());
                else
                    rootFolderUpdater.RunWorkerAsync("All");
            }
        }

        /// <summary>
        /// TODO!
        /// </summary>
        /// <param name="content"></param>
        public void UpdateDisplayIfNecessary(string content)
        {
            //if (SelectedShow != null && SelectedShow.Name == ep.Show)
                //DisplayShow(true);
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
                lvContentFolders.Top += 30;
                lvContentFolders.Height -= 30;
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
                lvContentFolders.Top -= 30;
                lvContentFolders.Height += 30;
                progressBarShown = false;
            }
        }

        /// <summary>
        /// Asynchronous work for updating movies found in movie folders.
        /// </summary>
        private void rootFolderUpdater_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get root folders to update from arguments
            string folder = (string)e.Argument;

            // Update each selected movie folder
            List<ContentRootFolder> rootFolders = null;
            this.Invoke((MethodInvoker)delegate
            {
                rootFolders = GetSelectedRootFolders();
            });
            Organization.UpdateContentsFromRootFolders(rootFolders, false);
        }

        /// <summary>
        /// When a new movies is found from updating add it to the listview
        /// </summary>
        private void Organization_FolderUpdateProgressChange(object sender, OrgProgressChangesEventArgs e)
        {
            ContentRootFolder rootFolder = (ContentRootFolder)sender;
            if (rootFolder.ContentType != this.ContentType)
                return;

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
                        UpdateContent(true);
                        UpdateGenresComboBox();
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
        private void rootFolderUpdater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (reUpdateRequired)
            {
                reUpdateRequired = false;
                rootFolderUpdater.RunWorkerAsync(cmbFolders.SelectedItem == null ? "All" : cmbFolders.SelectedItem.ToString());
            }
            else
            {
                if (!this.IsHandleCreated)
                {
                    rootFolderSyncOnLoad = true;
                    return;
                }
                
                // Update movies for display
                this.BeginInvoke((MethodInvoker)delegate
                {
                    RootFolderUpdateSync();
                });
            }
        }

        /// <summary>
        /// Handler for control load event.
        /// </summary>
        private void Control_Load(object sender, EventArgs e)
        {
            // Perform initialization of movie if required
            if (rootFolderSyncOnLoad)
            {
                rootFolderSyncOnLoad = false;    
                RootFolderUpdateSync();
            }

            if (contentSyncOnLoad)
            {
                contentSyncOnLoad = false;
                ContentLoadSync();
            }

            // Perform update of movie folders if required
            if (folderUpdateRequiredOnLoad)
            {
                folderUpdateRequiredOnLoad = false;
                DoUpdateFolders();
            }

        }

        /// <summary>
        /// Initializes movies info in control
        /// </summary>
        private void RootFolderUpdateSync()
        {
            UpdateContent(false);
            UpdateGenresComboBox();
            btnForceRefresh.Enabled = true;
            if (!rootFolderUpdater.IsBusy)
                HideProgressBar();
        }

        private void ContentLoadSync()
        {
            UpdateGenresComboBox();

            switch (this.ContentType)
            {
                case ContentType.Movie:
                    lock (Organization.AllMovieGenres.AccessLock)
                        Organization.AllMovieGenres.GenresUpdated += Organization_GenresUpdated;
                    break;
                case ContentType.TvShow:
                    lock (Organization.AllTvGenres.AccessLock)
                        Organization.AllTvGenres.GenresUpdated += Organization_GenresUpdated;
                    break;
            }

            UpdateContent(false);
        }

        /// <summary>
        /// Display progress during loading of movie data at startup.
        /// </summary>
        private void Organization_LoadProgressChange(object sender, ProgressChangedEventArgs e)
        {
            if(((ContentCollection)sender).ContentType != this.ContentType)
                return;

            if (!this.IsHandleCreated)
                return;

            this.Invoke((MethodInvoker)delegate
            {
                UpdataProgress(e.ProgressPercentage, "Loading " + this.ContentType.ToString() + "s" + (string)e.UserState);
            });
            
        }

        /// <summary>
        /// Trigger update of movie once loading is complete at startup.
        /// </summary>
        private void Organization_LoadComplete(object sender, EventArgs e)
        {
            if (((ContentCollection)sender).ContentType != this.ContentType)
                return;

            if (!this.IsHandleCreated)
            {
                contentSyncOnLoad = true;
                UpdateContentInFolders(true);
                return;
            }

            this.Invoke((MethodInvoker)delegate
            {
                ContentLoadSync();
                UpdateContentInFolders(true);
            });
        }

        

        /// <summary>
        /// Update genres combo box when list is updated
        /// </summary>
        private void Organization_GenresUpdated(object sender, EventArgs e)
        {
            UpdateGenresComboBox();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Perform update of movie on update required event from content listview
        /// </summary>
        void lvContentFolders_UpdateContentsRequired(object sender, EventArgs e)
        {
            UpdateContentInFolders(false);
        }

        /// <summary>
        /// Perform save of movie on save required event from content listview
        /// </summary>
        void lvContentFolders_SaveRequired(object sender, EventArgs e)
        {
            Organization.Movies.Save();
        }

        /// <summary>
        /// Opens movie editor on edit required event from content listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lvContentFolders_ItemToEdit(object sender, EventArgs e)
        {
            EditContent();
        }

        /// <summary>
        /// Updates displayed movies when selected movie folder is changed.
        /// </summary>
        private void cmbFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateRootFilters();
            UpdateContent(false);
            UpdateGenresComboBox();
        }

        private void cmbRootFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateContent(false);
        }

        /// <summary>
        /// Last selected genre
        /// </summary>
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
                    UpdateContent(false);
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
            EditContent();
        }

        /// <summary>
        /// Open editor for editing content that is selected in listview
        /// </summary>
        private void EditContent()
        {
            // Get the movie
            Content selContent;
            if (!GetSeletectedContent(out selContent))
                return;

            // Open Movie editor
            ContentEditorForm editor = new ContentEditorForm(selContent);
            editor.ShowDialog();

            // Update movie
            if (editor.Results != null)
            {
                switch (this.ContentType)
                {
                    case ContentType.Movie:
                        ((Movie)selContent).Clone((Movie)editor.Results);
                        Organization.Movies.Save();
                        break;
                    case ContentType.TvShow:
                        ((TvShow)selContent).Clone((TvShow)editor.Results);
                        Organization.Shows.Save();
                        break;
                }
                
                
                lvContentFolders.DisplayContent(false);
            }
        }

        /// <summary>
        /// Watched properties is toggle for selected show when watched button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnWatchMod_Click(object sender, EventArgs e)
        {
            lvContentFolders.SetWatchedForSelected((Button)sender == btnWatched);
        }

        /// <summary>
        /// Gets the currently selected movies from the movie listview.
        /// </summary>
        /// <param name="movies">Resulting selected movies</param>
        /// <returns>Whether any selected movies were found</returns>
        private bool GetSeletectedContent(out List<Content> contents)
        {
            return lvContentFolders.GetSeletectedContents(out contents);
        }

        /// <summary>
        /// Get first selected movie in movies listview.
        /// </summary>
        /// <param name="content">Resulting selected movie</param>
        /// <returns>Whether a selected movie was found</returns>
        private bool GetSeletectedContent(out Content content)
        {
            // Initialize selected movie
            content = null;
            
            // Get all selected movies, if none found return false
            List<Content> selContent;
            if (!GetSeletectedContent(out selContent))
                return false;

            // Set selected movie to first selected and return true
            content = selContent[0];
            return true;
        }

        /// <summary>
        /// Force refresh updates movies that are found in selected movie folder(s)
        /// </summary>
        private void btnForceRefresh_Click(object sender, EventArgs e)
        {
            UpdateContentInFolders(false);
        }

        /// <summary>
        /// Edit list button open settings to movie folders tabs.
        /// </summary>
        private void btnEditDir_Click(object sender, EventArgs e)
        {
            SettingsForm settingForm = new SettingsForm(this.ContentType == Meticumedia.ContentType.Movie ? 3 : 1);
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
            lvContentFolders.HideWatched = chkHideWatched.Checked;
        }

        /// <summary>
        /// Year check box updates displayed items
        /// </summary>
        private void chkYearFilter_CheckedChanged(object sender, EventArgs e)
        {
            UpdateContent(false);
        }

        /// <summary>
        /// Min. year change updates displayed items
        /// </summary>
        private void numMinYear_ValueChanged(object sender, EventArgs e)
        {
            UpdateContent(false);
        }

        /// <summary>
        /// Max. year change updates displayed items
        /// </summary>
        private void numMaxYear_ValueChanged(object sender, EventArgs e)
        {
            UpdateContent(false);
        }

        /// <summary>
        /// Name filter updates displayed items
        /// </summary>
        private void txtNameFilter_TextChanged(object sender, EventArgs e)
        {
            UpdateContent(false);
            txtNameFilter.Select();
        }

        /// <summary>
        /// Listview selection change triggers event
        /// </summary>
        private void lvContentFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<Content> selections;
            if (GetSeletectedContent(out selections))
                OnSelectionChanged(selections);
        }

        #endregion


    }
}
