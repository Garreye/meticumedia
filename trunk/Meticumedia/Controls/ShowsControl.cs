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
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Meticumedia
{
    public partial class ShowsControl : UserControl
    {
        #region Episode Filter Class

        /// <summary>
        /// Defines filter that can be applied to TV episodes.
        /// </summary>
        private class EpisodeFilter
        {
            #region Properties

            /// <summary>
            /// Type of filters that can be applies to episodes.
            /// </summary>
            public enum FilterType { All, Missing, Unaired, Season };

            /// <summary>
            /// The type of episode filter being used.
            /// </summary>
            public FilterType Type { get; set; }

            /// <summary>
            /// The season number used when Type is season filter.
            /// </summary>
            public int Season { get; set; }

            #endregion

            #region Constructor

            /// <summary>
            /// Constructor with known properties
            /// </summary>
            /// <param name="type"></param>
            /// <param name="season"></param>
            public EpisodeFilter(FilterType type, int season)
            {
                this.Type = type;
                this.Season = season;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Runs an episode through filter.
            /// </summary>
            /// <param name="ep">The episode to filter</param>
            /// <returns>True if the episode makes it through filter</returns>
            public bool FilterEpisode(TvEpisode ep)
            {
                switch (this.Type)
                {
                    case EpisodeFilter.FilterType.All:
                        return true;
                    case EpisodeFilter.FilterType.Missing:
                        if ((ep.Missing == TvEpisode.MissingStatus.Missing || ep.Missing == TvEpisode.MissingStatus.InScanDirectory) && ep.Aired)
                            return true;
                        break;
                    case EpisodeFilter.FilterType.Season:
                        if (ep.Season == this.Season)
                            return true;
                        break;
                    case EpisodeFilter.FilterType.Unaired:
                        if (!ep.Aired)
                            return true;
                        break;
                    default:
                        throw new Exception("Unknown filter type!");
                }

                return false;
            }

            /// <summary>
            /// Return string for filter.
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                switch (this.Type)
                {
                    case FilterType.All:
                        return "All Episodes";
                    case FilterType.Missing:
                        return "Missing Episodes";
                    case FilterType.Season:
                        return "Season " + this.Season;
                    case FilterType.Unaired:
                        return "Unaired";
                    default:
                        throw new Exception("Unknown type");
                }
            }

            /// <summary>
            /// Equals check for this filter and another filter.
            /// Uses ToString to compare.
            /// </summary>
            /// <param name="obj">EpisodeFilter to compare to</param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                // False for null/invalid objects
                if (obj == null || !(obj is EpisodeFilter))
                    return false;

                // Case object to episode
                EpisodeFilter epFilter = (EpisodeFilter)obj;

                // Compare is on season and episode number only (show name may not be set yet)
                return epFilter.ToString() == this.ToString();
            }

            /// <summary>
            /// Overrides to prevent warning that Equals is overriden but no GetHashCode.
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            /// <summary>
            /// Build array of possible episode filters that can be used for a TvShow.
            /// </summary>
            /// <param name="show">The show to build filters for</param>
            /// <param name="displayIgnored">Whether to add ignored season filters</param>
            /// <returns></returns>
            public static List<EpisodeFilter> BuildFilters(TvShow show, bool displayIgnored)
            {
                List<EpisodeFilter> filters = new List<EpisodeFilter>();

                filters.Add(new EpisodeFilter(FilterType.All, 0));
                filters.Add(new EpisodeFilter(FilterType.Missing, 0));
                filters.Add(new EpisodeFilter(FilterType.Unaired, 0));
                foreach(TvSeason season in show.Seasons)
                    if (!season.Ignored || displayIgnored)
                        filters.Add(new EpisodeFilter(FilterType.Season, season.Number));

                return filters;
            }

            #endregion
        }

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

        /// <summary>
        /// Indicates that a TV shows have changed.
        /// </summary>
        public event EventHandler ShowsChanged;

        /// <summary>
        /// Triggers ShowsChanged event
        /// </summary>
        protected void OnShowsChange()
        {
            //UpdateShow(false);
            if (ShowsChanged != null)
                ShowsChanged(this, new EventArgs());
        }  

        #endregion

        #region Variables

        /// <summary>
        /// Currently selected TV show.
        /// </summary>
        private TvShow SelectedShow = null;

        /// <summary>
        /// List of currently displayed episodes
        /// </summary>
        private List<TvEpisode> displayedEpisodes = new List<TvEpisode>();

        /// <summary>
        /// Context menu for episodes list
        /// </summary>
        private ContextMenu episodeContextMenu = new ContextMenu(); 

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ShowsControl()
        {
            InitializeComponent();

            // Set legend colours and register to changes
            SetLegend();
            TvEpisode.BackColourChanged += new EventHandler(TvEpisode_BackColourChanged);

            // Initialize sort order for episodes
            TvShow.AscendingSort = true;

            // Show progress bar
            ShowProgressBar();
            
            // Register folder updating progress, for display in progress bar
            Organization.TvShowLoadProgressChange += new EventHandler<ProgressChangedEventArgs>(Organization_TvShowLoadProgressChange);
            Organization.TvShowLoadComplete += new EventHandler(Organization_TvShowLoadComplete);
            Organization.TvFolderUpdateProgressChange += new EventHandler<Organization.OrgProgressChangesEventArgs>(TheTvDbHelper_FolderUpdateProgressChange);

            // Initialize show update worker
            showUpdater = new BackgroundWorker();
            showUpdater.DoWork += new DoWorkEventHandler(showUpdater_DoWork);
            showUpdater.RunWorkerCompleted += new RunWorkerCompletedEventHandler(showUpdater_RunWorkerCompleted);

            // Setup shows list
            lvShows.DisplayGenres = false;
            lvShows.ItemToEdit += new EventHandler(lvShows_ItemToEdit);
            lvShows.SaveContentsRequired += new EventHandler(lvShows_SaveContentsRequired);
            lvShows.UpdateContentsRequired += new EventHandler(lvShows_UpdateContentsRequired);
            
            // Setup episode context
            lvEpisodes.ContextMenu = episodeContextMenu;
            episodeContextMenu.Popup += new EventHandler(episodeContextMenu_Popup);

            this.Load += new EventHandler(ShowsControl_Load);

            ScanHelper.TvScanItemsUpdate += ScanHelper_TvScanItemsUpdate;
        }

        void ScanHelper_TvScanItemsUpdate(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
                return;
            
            this.Invoke((MethodInvoker)delegate
            {
                DisplayShow(true);
            });
        }
        
        #endregion

        #region Context Menu

        /// <summary>
        /// Context menu for episodes listview is built on pop-up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void episodeContextMenu_Popup(object sender, EventArgs e)
        {
            BuildContextMenu();
        }

            /// <summary>
        /// Builds context menu based on what is currently selected
        /// </summary>
        private void BuildContextMenu()
        {
            episodeContextMenu.MenuItems.Clear();

            // Get selected episodes
            List<TvEpisode> selEpisodes;
            if (!GetSelectedEpisodes(out selEpisodes))
                return;

            episodeContextMenu.MenuItems.Add("Play", new EventHandler(HandlePlay));
            episodeContextMenu.MenuItems.Add("Edit", new EventHandler(HandleEdit));

            if (selEpisodes.Count == 1 && selEpisodes[0].Missing == TvEpisode.MissingStatus.Missing)
            {
                episodeContextMenu.MenuItems.Add("Locate and Move", new EventHandler(HandleLocate));
                episodeContextMenu.MenuItems.Add("Locate and Copy", new EventHandler(HandleLocate));
            }

            bool allIgnored = true;
            bool allUnignored = true;
            foreach (TvEpisode ep in selEpisodes)
                if (ep.Ignored)
                    allUnignored = false;
                else
                    allIgnored = false;

            if ((allUnignored && !allIgnored) || (!allIgnored && !allUnignored))
                episodeContextMenu.MenuItems.Add("Ignore", new EventHandler(HandleIgnore));
            if ((allIgnored && !allUnignored) || (!allIgnored && !allUnignored))
                episodeContextMenu.MenuItems.Add("Unignore", new EventHandler(HandleUnignore));            

            episodeContextMenu.MenuItems.Add("Delete", new EventHandler(HandleDelete));

            //bool allWatched = true;
            //bool allUnwatched = true;
            //foreach (TvEpisode ep in selEpisodes)
            //    if (ep.Watched)
            //        allUnwatched = false;
            //    else
            //        allWatched = false;

            //if ((allUnwatched && !allWatched) || (!allWatched && !allUnwatched))
            //    episodeContextMenu.MenuItems.Add("Mark as watched", new EventHandler(HandleMarkAsWatch));
            //if ((allWatched && !allUnwatched) || (!allWatched && !allUnwatched))
            //    episodeContextMenu.MenuItems.Add("Unmark as watched", new EventHandler(HandleUnmarkAsWatch));


        }

        /// <summary>
        /// Handles play selection from context menu
        /// </summary>
        private void HandlePlay(object sender, EventArgs e)
        {
            PlaySelectedEpisode();
        }

        /// <summary>
        /// Handles edit selection from context menu
        /// </summary>
        private void HandleEdit(object sender, EventArgs e)
        {
            TvEpisode selEp;
            if (GetSelectedEpisode(out selEp))
            {
                EpisodeEditorForm eef = new EpisodeEditorForm(selEp);

                if (eef.ShowDialog() == DialogResult.OK)
                {
                    selEp.UpdateInfo(eef.Episode);
                    Organization.SaveShows();
                    DisplayEpisodesList();
                }
            }
        }

        /// <summary>
        /// Handles locate selection from context menu
        /// </summary>
        private void HandleLocate(object sender, EventArgs e)
        {
            // Get selected episode
            List<TvEpisode> selEpisodes;
            if (!GetSelectedEpisodes(out selEpisodes))
                return;
            
            // Locate
            List<OrgItem> items;
            if (selEpisodes[0].UserLocate(true, ((MenuItem)sender).Text.Contains("Move"), out items))
                OnItemsToQueue(items);
        }

        /// <summary>
        /// Handles ignore selection from context menu
        /// </summary>
        private void HandleIgnore(object sender, EventArgs e)
        {
            SetIgnoreOnSelected(true);
            DisplayEpisodesList();
        }

        /// <summary>
        /// Handles unignore selection from context menu
        /// </summary>
        private void HandleUnignore(object sender, EventArgs e)
        {
            SetIgnoreOnSelected(false);
            DisplayEpisodesList();
        }

        /// <summary>
        /// Handles delete selection from context menu
        /// </summary>
        private void HandleDelete(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you want to delete the selected episode? This operation cannot be undone", "Sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Get selected episode
                List<TvEpisode> selEpisodes;
                if (!GetSelectedEpisodes(out selEpisodes))
                    return;

                List<OrgItem> items = new List<OrgItem>();
                foreach (TvEpisode ep in selEpisodes)
                    if (File.Exists(ep.File.FilePath))
                        items.Add(new OrgItem(OrgAction.Delete, ep.File.FilePath, FileHelper.FileCategory.Trash, null));
                OnItemsToQueue(items);
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handler for control load event
        /// </summary>
        void ShowsControl_Load(object sender, EventArgs e)
        {
            if (updateRequiredOnLoad)
            {
                OnShowsChange();
                UpdateFolders();
            }
        }

        /// <summary>
        /// Perform update of shows on update required event from content listview
        /// </summary>
        void lvShows_UpdateContentsRequired(object sender, EventArgs e)
        {
            UpdateShows(false);
        }

        /// <summary>
        /// Perform save of shows on update required event from content listview
        /// </summary>
        void lvShows_SaveContentsRequired(object sender, EventArgs e)
        {
            Organization.SaveShows();
        }

        /// <summary>
        /// Opens show edit on edit required event from content listview
        /// </summary>
        void lvShows_ItemToEdit(object sender, EventArgs e)
        {
            EditShow();
        }

        /// <summary>
        /// Updates displayed legend colours when they are changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TvEpisode_BackColourChanged(object sender, EventArgs e)
        {
            SetLegend();
        }

        /// <summary>
        /// Sets legend colors to episode colors.
        /// </summary>
        private void SetLegend()
        {
            lbMissing.Color = TvEpisode.MissingBackColor;
            lbScanDir.Color = TvEpisode.InScanDirectoryColor;
            lbIgnored.Color = TvEpisode.IgnoredBackColor;
            lbUnaired.Color = TvEpisode.UnairedBackColor;
            lbInvalid.Color = Color.LightCoral;
        }

        /// <summary>
        /// Handle change of episode filter; re-displayed episodes that match filter.
        /// </summary>
        private void cmbEpFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayEpisodesList();
        }

        /// <summary>
        /// Handle change of display ignored check box
        /// </summary>
        private void chkDisplayIgnored_CheckedChanged(object sender, EventArgs e)
        {
            lbIgnored.Visible = chkDisplayIgnored.Checked;
            btnUnignore.Visible = chkDisplayIgnored.Checked;
            DisplayShow(true);
            DisplayEpisodesList();
        }

        /// <summary>
        /// Set ignore property to true on selected episode when button is clicked.
        /// </summary>
        private void btnIgnore_Click(object sender, EventArgs e)
        {
            SetIgnoreOnSelected(true);
            DisplayEpisodesList();
        }

        /// <summary>
        /// Set ignore property to false on selected episode when button is clicked.
        /// </summary>
        private void btnUnignore_Click(object sender, EventArgs e)
        {
            SetIgnoreOnSelected(false);
            DisplayEpisodesList();
        }

        /// <summary>
        /// Double clicking on episode in list plays the episode.
        /// </summary>
        private void lvEpisodes_DoubleClick(object sender, EventArgs e)
        {
            PlaySelectedEpisode();
        }

        /// <summary>
        /// Play episode that is selected
        /// </summary>
        private void PlaySelectedEpisode()
        {
            if (lvEpisodes.SelectedItems.Count == 0)
                return;

            ListViewItem item = lvEpisodes.SelectedItems[0];

            int seasonNumber = int.Parse(item.Text);
            int episodeNumber = int.Parse(item.SubItems[1].Text);

            TvEpisode episode;
            if (SelectedShow.FindEpisode(seasonNumber, episodeNumber, out episode))
                episode.PlayEpisodeFile();
        }

        /// <summary>
        /// Handles change of selection on shows list; sets selected and displays episodes from show.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvShows_SelectedIndexChanged(object sender, EventArgs e)
        {
            Content selShow;
            lvShows.GetSeletectedContent(out selShow);
            SelectedShow = selShow as TvShow;

            // Display show
            DisplayShow(false);
        }

        /// <summary>
        /// The show editor is opened when the edit button is opened.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEditShow_Click(object sender, EventArgs e)
        {
            EditShow();
        }

        /// <summary>
        /// Triggers update of shows when force refresh button is clicked
        /// </summary>
        private void btnForceRefresh_Click(object sender, EventArgs e)
        {
            UpdateShowsInFolders(false);
        }

        private void cmbFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateShows(false);
        }

        private void txtNameFilter_TextChanged(object sender, EventArgs e)
        {
            UpdateShows(false);
            txtNameFilter.Select();
        }

        #endregion

        #region Shows List

        /// <summary>
        /// Updates TV folders available for selection in combo box.
        /// </summary>
        public void UpdateFolders()
        {
            // Get currently selected folder
            string selected = string.Empty;
            if (cmbFolders.SelectedItem != null)
                selected = cmbFolders.SelectedItem.ToString();

            // Set default index to select
            int indexToSelect = 0;

            // Clear items and add all option
            cmbFolders.Items.Clear();
            cmbFolders.Items.Add("All Show Folders");

            // Add each movie folder from settings
            foreach (ContentRootFolder folder in Settings.TvFolders)
            {
                int index = cmbFolders.Items.Add(folder);

                // If this folder was selected than set its index to be selected again
                if (folder.ToString() == selected)
                    indexToSelect = index;
            }

            // Set selected index
            cmbFolders.SelectedIndex = indexToSelect;

            // Display shows
            lvShows.Contents = Organization.GetShowsFromRootFolders(cmbFolders.SelectedItem.ToString(), txtNameFilter.Text);
            lvShows.DisplayContent(false);

            // Start updating the movies that are found in select folder
            UpdateShowsInFolders(!fullUpdateCompleted);    
        }

        /// <summary>
        /// Worker for updating shows in selected folder(s)
        /// </summary>
        private BackgroundWorker showUpdater;

        /// <summary>
        /// Flag indicatating update was cancelled and need to be restarted when cancellation is complete
        /// </summary>
        private bool reupdateRequired = false;

        /// <summary>
        /// Idicates that a full update from online database has been complete. 
        /// Once set updates will be local update checks only.
        /// </summary>
        private bool fullUpdateCompleted = false;

        /// <summary>
        /// Updates shows found in the selected TV folder(s)
        /// </summary>
        public void UpdateShowsInFolders(bool full)
        {
            pbUpdating.Value = 0;
            pbUpdating.Message = "Updating...";

            if (full) 
                fullUpdateCompleted = false;

            // Disable refresh button
            btnForceRefresh.Enabled = false;

            // Create worker to run shows folder updatU
            if (showUpdater.IsBusy)
            {
                reupdateRequired = true;
                Organization.CancelTvUpdating();
            }
            else
            {
                ShowProgressBar();
                if (cmbFolders.SelectedItem != null)
                    showUpdater.RunWorkerAsync(cmbFolders.SelectedItem.ToString());
            }
        }

        /// <summary>
        /// Indicate whether progress bar is currently shown
        /// </summary>
        private bool progressBarShown = false;

        /// <summary>
        /// Shrinks listview so that progress bar is visile.
        /// </summary>
        private void ShowProgressBar()
        {
            if (!progressBarShown)
            {
                lvShows.Height -= 30;
                lvShows.Top += 30;
                progressBarShown = true;
            }
        }

        /// <summary>
        /// Expands listview so that progress bar is hidden
        /// </summary>
        private void HideProgressBar()
        {
            if (progressBarShown)
            {
                lvShows.Height += 30;
                lvShows.Top -= 30;
                progressBarShown = false;
            }
        }

        /// <summary>
        /// Asynchronous work for updating shows found in TV folders.
        /// </summary>
        private void showUpdater_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get movie folders to update from arguments
            string folder = (string)e.Argument;

            // Update each selected movie folder
            Organization.UpdateTvRootFolders(folder, fullUpdateCompleted);
        }

        /// <summary>
        /// Loading progress changes are shown in progress bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Organization_TvShowLoadProgressChange(object sender, ProgressChangedEventArgs e)
        {
            if (pbUpdating.Value != e.ProgressPercentage)
            {
                if (this.InvokeRequired)
                    this.Invoke((MethodInvoker)delegate
                    {
                        UpdateProgress(e.ProgressPercentage, "Loading TV Shows");
                    });
                else
                    UpdateProgress(e.ProgressPercentage, "Loading TV Shows");
            }
        }

        /// <summary>
        /// Update info on progress bar
        /// </summary>
        /// <param name="percent">Percent value to set progress bar to</param>
        /// <param name="msg">Message to display on progress bar</param>
        private void UpdateProgress(int percent, string msg)
        {
            pbUpdating.Message = msg;
            pbUpdating.Value = percent;
        }

        /// <summary>
        /// Flag indicating shows update is required when control is loaded.
        /// Set if update was attempted before handle for control was created.
        /// </summary>
        private bool updateRequiredOnLoad = false;
        
        /// <summary>
        /// Handler for control load event
        /// </summary>
        private void Organization_TvShowLoadComplete(object sender, EventArgs e)
        {
            if (!this.IsHandleCreated)
            {
                updateRequiredOnLoad = true;
                return;
            }

            this.Invoke((MethodInvoker)delegate
            {
                OnShowsChange();
                UpdateFolders();
            });
        }

        /// <summary>
        /// Folder updating progress is shown in progress bar
        /// </summary>
        private void TheTvDbHelper_FolderUpdateProgressChange(object sender, Organization.OrgProgressChangesEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                UpdateProgress(e.ProgressPercentage, (string)e.UserState);
                if (e.NewItem)
                {
                    UpdateShows(true);
                }

            });

        }

        /// <summary>
        /// Called when show folder updating is completed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showUpdater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (reupdateRequired)
            {
                reupdateRequired = false;
                showUpdater.RunWorkerAsync(cmbFolders.SelectedItem.ToString());
            }
            else
            {
                if (true) // TODO: setting for this!
                {
                    lock (Organization.Shows)
                        foreach (TvShow show in Organization.Shows)
                            foreach (TvSeason season in show.Seasons)
                                if (season.Number == 0)
                                    foreach (TvEpisode ep in season.Episodes)
                                        ep.Ignored = true;
                    Organization.SaveShows();
                }

                // Update shows
                if (this.InvokeRequired)
                    this.Invoke((MethodInvoker)delegate
                    {
                        UpdateShows(false);
                        btnForceRefresh.Enabled = true;
                        OnShowsChange();
                        HideProgressBar();

                    });
                else
                {
                    UpdateShows(false);
                    btnForceRefresh.Enabled = true;
                    OnShowsChange();
                    HideProgressBar();
                }
                fullUpdateCompleted = true;
            }
        }
        
        /// <summary>
        /// Update episode listview is episode that has changed is from the currently selected show
        /// </summary>
        /// <param name="ep">Episode that is triggering update</param>
        public void UpdateShowsIfNecessary(TvEpisode ep)
        {
            if (SelectedShow != null && SelectedShow.Name == ep.Show)
                DisplayShow(true);
        }       

        /// <summary>
        /// Update shows in listview based on selected content folder.
        /// </summary>
        public void UpdateShows(bool newOnly)
        {
            // Stored the currently selected show
            string selectedShowName = string.Empty;
            if (SelectedShow != null)
                selectedShowName = SelectedShow.Name;

            // Get currently selected folder
            string folderName = "All";
            if (cmbFolders.SelectedItem != null)
                folderName = cmbFolders.SelectedItem.ToString();

            // Get shows for selected folder and sort them
            lvShows.Contents = Organization.GetShowsFromRootFolders(folderName, txtNameFilter.Text);
            if (newOnly)
                lvShows.DisplayContent(newOnly);
            else
            {
                lvShows.SortContents(lvShows.lastSortColumn, true);

                // Reselect the show that was selected before update
                if (!string.IsNullOrEmpty(selectedShowName))
                    for (int i = 0; i < lvShows.Contents.Count; i++)
                        if (lvShows.Contents[i].Name == selectedShowName)
                        {
                            lvShows.SelectedIndexChanged -= new EventHandler(this.lvShows_SelectedIndexChanged);
                            lvShows.Items[i].Selected = true;
                            lvShows.Items[i].EnsureVisible();
                            lvShows.SelectedIndexChanged += new EventHandler(this.lvShows_SelectedIndexChanged);
                            break;
                        }
            }
        }

        /// <summary>
        /// Open show editor for editing show and updating show
        /// information when closed (if needed).
        /// </summary>
        private void EditShow()
        {
            // Check that selected show is valid
            if (SelectedShow == null)
                return;

            // Open Movie editor
            ShowEditForm sef = new ShowEditForm(SelectedShow);
            sef.ShowDialog();

            // Update show if results are valid
            if (sef.Results != null)
            {
                SelectedShow.Clone(sef.Results);
                Organization.SaveShows();
                UpdateShows(false);
                OnShowsChange();
            }
        }

        #endregion

        #region Selected Show Dsiplay

        /// <summary>
        /// Displays show that is selected in show list.
        /// </summary>
        /// <param name="maintainSelection"></param>
        private void DisplayShow(bool maintainSelection)
        {
            // Check that a show is selected
            if (SelectedShow == null)
                return;

            // Set episodes text
            gbEpisodes.Text = "Episodes of '" + SelectedShow.Name + "'";

            // Save current filter
            EpisodeFilter currentFilterSel = (EpisodeFilter)cmbEpFilter.SelectedItem;

            // Buil episode filters
            cmbEpFilter.Items.Clear();
            List<EpisodeFilter> epFilters = EpisodeFilter.BuildFilters(SelectedShow, chkDisplayIgnored.Checked);
            foreach (EpisodeFilter filter in epFilters)
                cmbEpFilter.Items.Add(filter);
            
            // Set filter selection back to what it was befor building if needed
            if (maintainSelection)
            {
                bool set = false;
                for (int i = 0; i < cmbEpFilter.Items.Count; i++)
                    if (((EpisodeFilter)cmbEpFilter.Items[i]).Equals(currentFilterSel))
                    {
                        cmbEpFilter.SelectedIndex = i;
                        set = true;
                        break;
                    }

                if (!set)
                    cmbEpFilter.SelectedIndex = 0;
            }
            else
                cmbEpFilter.SelectedIndex = 0;

            // Display episodes for show in episodes listview
            DisplayEpisodesList();

        }

        /// <summary>
        /// Adds episodes from the selected show to the episodes list
        /// based on the episode filter that is applied.
        /// </summary>
        private void DisplayEpisodesList()
        {
            // Check that filter is valid
            if (cmbEpFilter.SelectedIndex == -1)
                return;

            // Get filter
            EpisodeFilter epFilter = (EpisodeFilter)cmbEpFilter.SelectedItem;
            
            // Clear list
            lvEpisodes.Items.Clear();
            displayedEpisodes.Clear();

            // Loop through each episode of each season and
            foreach(TvSeason season in SelectedShow.Seasons)
                foreach (TvEpisode ep in season.Episodes)
                    // Add episodes that make it through the filter
                    if (epFilter.FilterEpisode(ep))
                        AddEpisodeToList(ep);
            
        }

        /// <summary>
        /// Adds a single TV episode to episodes list.
        /// </summary>
        /// <param name="ep"></param>
        private void AddEpisodeToList(TvEpisode ep)
        {
            if (!ep.Ignored || chkDisplayIgnored.Checked)
            {
                ListViewItem item = lvEpisodes.Items.Add(ep.Season.ToString());
                item.SubItems.Add(ep.Number.ToString());
                item.SubItems.Add(ep.Name);
                item.SubItems.Add(String.Format("{0:MMM dd yyyy}", ep.AirDate));
                item.SubItems.Add(ep.Overview);
                item.BackColor = ep.GetBackColor();
                displayedEpisodes.Add(ep);
            }
        }

        /// <summary>
        /// Set ignore property on episodes in episodes list 
        /// that are selected.
        /// </summary>
        /// <param name="ignore">The value to set the ignore property to</param>
        private void SetIgnoreOnSelected(bool ignore)
        {
            List<TvEpisode> selEps;
            if(!GetSelectedEpisodes(out selEps))
                return;
            
            foreach(TvEpisode ep in selEps)
                ep.Ignored = ignore;

            Organization.SaveShows();
        }

        /// <summary>
        /// Get episodes objects that are selected from list view
        /// </summary>
        private bool GetSelectedEpisodes(out List<TvEpisode> selEps)
        {
            selEps = new List<TvEpisode>();
            foreach (int index in lvEpisodes.SelectedIndices)
                selEps.Add(displayedEpisodes[index]);

            return selEps.Count > 0;
        }

        /// <summary>
        /// Get episode object that is firstly selected from listview
        /// </summary>
        private bool GetSelectedEpisode(out TvEpisode selEp)
        {
            List<TvEpisode> selEps;
            if (GetSelectedEpisodes(out selEps))
            {
                selEp = selEps[0];
                return true;
            }

            selEp = null;
            return false;
        }

        #endregion


    }
}
