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
using System.Text.RegularExpressions;

namespace Meticumedia
{
    /// <summary>
    /// Control to handle scanning.
    /// </summary>
    public partial class ScanControl : UserControl
    {
        #region Variables

        /// <summary>
        /// Instance for running directory scan.
        /// </summary>
        private DirectoryScan directoryScan = new DirectoryScan(false);

        /// <summary>
        /// Instance for running TV missing check
        /// </summary>
        private TvMissingScan tvMissingScan = new TvMissingScan(false);

        /// <summary>
        /// Instance for running TV rename check
        /// </summary>
        private TvRenameScan tvRenameScan = new TvRenameScan(false);

        /// <summary>
        /// Instance for running TV folder scan
        /// </summary>
        private TvFolderScan tvFolderScan = new TvFolderScan(false);
        
        /// <summary>
        /// Instance for running movie folder scan
        /// </summary>
        private MovieFolderScan movieFolderScan = new MovieFolderScan(false);

        /// <summary>
        /// Items that are currently in the queue
        /// </summary>
        private List<OrgItem> queuedItems;

        /// <summary>
        /// Last run scan type.
        /// </summary>
        private ScanType lastRunScan = ScanType.Directory;

        /// <summary>
        /// Currently run scan type.
        /// </summary>
        private ScanType currentScan = ScanType.Directory;

        /// <summary>
        /// Worker for processing scan.
        /// </summary>
        private BackgroundWorker scanWorker;

        /// <summary>
        /// Flag indicating that a scan in process was cancel and a new scan should be done once thread has been stopped.
        /// </summary>
        private bool rescanRequired = false;

        /// <summary>
        /// Context menu for scan list
        /// </summary>
        private ContextMenu contextMenu = new ContextMenu();

        /// <summary>
        /// Indicated scan is currently running
        /// </summary>
        private bool scanRunning = false;

        /// <summary>
        /// Flag that scan has been cancelled
        /// </summary>
        private bool scanCancelled = false;

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

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ScanControl()
        {
            InitializeComponent();

            // Initialize queue items list
            this.queuedItems = new List<OrgItem>();
            lvResults.SetColumns(OrgItemColumnSetup.DirectoryScan);

            // Setup scan worker
            scanWorker = new BackgroundWorker();
            scanWorker.WorkerSupportsCancellation = true;
            scanWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(scanWorker_RunWorkerCompleted);
            scanWorker.DoWork += new DoWorkEventHandler(scanWorker_DoWork);

            // Register to scan helper progress change (for tracking prgress of running scan)
            directoryScan.ItemsInitialized += scan_ItemsInitialized;

            // Setup context menu
            lvResults.ContextMenu = contextMenu;
            contextMenu.Popup += new EventHandler(contextMenu_Popup);

            // Setup scan type combo box
            foreach (ScanType type in Enum.GetValues(typeof(ScanType)))
                cmbScanType.Items.Add(type);
            cmbScanType.FormattingEnabled = true;
            cmbScanType.Format += cmbScanType_Format;
            cmbScanType.SelectedIndex = 0;
        }

        private void LinkProgressEvents()
        {
            directoryScan.ProgressChange += new EventHandler<ProgressChangedEventArgs>(scan_ScanProgressChange);
            tvMissingScan.ProgressChange += new EventHandler<ProgressChangedEventArgs>(scan_ScanProgressChange);
            tvRenameScan.ProgressChange += new EventHandler<ProgressChangedEventArgs>(scan_ScanProgressChange);
            tvFolderScan.ProgressChange += new EventHandler<ProgressChangedEventArgs>(scan_ScanProgressChange);
            movieFolderScan.ProgressChange += new EventHandler<ProgressChangedEventArgs>(scan_ScanProgressChange);
        }

        private void UnlinkProgressEvents()
        {
            directoryScan.ProgressChange -= new EventHandler<ProgressChangedEventArgs>(scan_ScanProgressChange);
            tvMissingScan.ProgressChange -= new EventHandler<ProgressChangedEventArgs>(scan_ScanProgressChange);
            tvRenameScan.ProgressChange -= new EventHandler<ProgressChangedEventArgs>(scan_ScanProgressChange);
            tvFolderScan.ProgressChange -= new EventHandler<ProgressChangedEventArgs>(scan_ScanProgressChange);
            movieFolderScan.ProgressChange -= new EventHandler<ProgressChangedEventArgs>(scan_ScanProgressChange);
        }


        void scan_ItemsInitialized(object sender, Scan.ItemsInitializedArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                lvResults.ItemChecked -= new ItemCheckedEventHandler(lvResults_ItemChecked);
                lvResults.SetOrgItems(e.Items);
                lvResults.ItemChecked += new ItemCheckedEventHandler(lvResults_ItemChecked);
                DisplayResults();
            });
        }

        void cmbScanType_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = ((ScanType)e.Value).Description();
        }

        #endregion

        #region Context Menu

        /// <summary>
        /// Build context menu for listview when it pops up
        /// </summary>
        private void contextMenu_Popup(object sender, EventArgs e)
        {
            BuildContextMenu();
        }

        /// <summary>
        /// Build context menu for queue listview based on what is selected
        /// </summary>
        private void BuildContextMenu()
        {
            // Clear item
            contextMenu.MenuItems.Clear();

            // Check that item is selected
            if (lvResults.SelectedItems.Count == 0)
                return;

            // Check if all selected item are consistent
            bool allAlreadyExists = true;
            bool allValidMovies = true;
            List<OrgItem> selItems = lvResults.GetSelectedOrgItems();
            for (int i = 0; i < selItems.Count; i++)
            {
                if (selItems[i].Action != OrgAction.AlreadyExists)
                    allAlreadyExists = false;
                if (selItems[i].Action == OrgAction.Copy || selItems[i].Action == OrgAction.Move)
                {
                    if (selItems[i].Category != FileCategory.NonTvVideo)
                        allValidMovies = false;
                }
                else
                {
                    allValidMovies = false;
                }
            }

            // Add change folder
            if (allValidMovies)
            {
                MenuItem item = contextMenu.MenuItems.Add("Set Destination Movie Folder");

                List<string> folderOptions = new List<string>();
                List<ContentRootFolder> folders = Settings.MovieFolders;
                foreach (ContentRootFolder folder in folders)
                    AddFolderPaths(folderOptions, folder);

                foreach (string option in folderOptions)
                    item.MenuItems.Add(option, HandleFolderChange);
            }

            // Create options
            if (allAlreadyExists)
                contextMenu.MenuItems.Add("Set action to replace existing", new EventHandler(HandleReplace));

            contextMenu.MenuItems.Add("Add Checked to Queue", new EventHandler(HandleQueue));
            if (lastRunScan == ScanType.Directory)
            {
                contextMenu.MenuItems.Add("Ignore Item(s)", new EventHandler(HandleIgnore));
                contextMenu.MenuItems.Add("Unignore Item(s)", new EventHandler(HandleUnignore));
            }

            if (selItems.Count == 1 && selItems[0].Status != OrgStatus.Missing)
                contextMenu.MenuItems.Add("Edit Action", new EventHandler(HandleEdit));

            if (lastRunScan == ScanType.TvMissing)
            {
                if (lvResults.SelectedItems.Count == 1)
                    contextMenu.MenuItems.Add("Locate Episode", new EventHandler(HandleLocateTv));
                MenuItem ignoreItem = contextMenu.MenuItems.Add("Ignore");

                ignoreItem.MenuItems.Add("Episode(s)", new EventHandler(HandleIgnoreTv));
                ignoreItem.MenuItems.Add("Season(s)", new EventHandler(HandleIgnoreTvSeason));
                ignoreItem.MenuItems.Add("Show(s) (disables include in scan)", new EventHandler(HandleTurnOffIncludeInScan));
            }
        }

        /// <summary>
        /// Recursively add content folder and its sub-content folders to list of folder paths
        /// </summary>
        /// <param name="folderStrings">list of folder paths being built</param>
        /// <param name="folder">current folder to be added to list</param>
        private void AddFolderPaths(List<string> folderStrings, ContentRootFolder folder)
        {
            folderStrings.Add(folder.FullPath);

            foreach (ContentRootFolder subFolder in folder.ChildFolders)
                AddFolderPaths(folderStrings, subFolder);
        }

        /// <summary>
        /// Handles updating action for replacing destination from context menu selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleReplace(object sender, EventArgs e)
        {
            // Change action for each selected item
            foreach (OrgListItem selItem in lvResults.SelectedItems)
            {
                OrgItem currItem = selItem.OrgItem;

                // Check that action is already exists
                if (currItem.Action == OrgAction.AlreadyExists)
                {
                    // Get scan directory object item belongs to
                    foreach (OrgFolder dir in Settings.ScanDirectories)
                        if (dir.FolderPath == currItem.ScanDirectory.FolderPath)
                        {
                            // Set action based on scan directory
                            currItem.Action = dir.CopyFrom ? OrgAction.Copy : OrgAction.Move;
                            currItem.Replace = true;
                            currItem.Check = CheckState.Checked;
                            break;
                        }
                }
            }

            // Update display
            DisplayResults(true);

        }

        /// <summary>
        /// Handles folder change action from context menu
        /// </summary>
        private void HandleFolderChange(object sender, EventArgs e)
        {
            string destinationFolder = ((MenuItem)sender).Text;

            foreach (OrgListItem selItem in lvResults.SelectedItems)
            {
                OrgItem currItem = selItem.OrgItem;

                if (currItem.Category == FileCategory.NonTvVideo && currItem.Movie != null)
                {
                    currItem.Movie.RootFolder = destinationFolder;
                    currItem.DestinationPath = currItem.Movie.BuildFilePath(currItem.SourcePath);
                }
            }

            // Update display
            DisplayResults(true);
        }

        /// <summary>
        /// Handles queue items selection from context menu
        /// </summary>
        private void HandleQueue(object sender, EventArgs e)
        {
            QueueCheckItems();
        }

        /// <summary>
        /// Handles ignore item selection from context menu
        /// </summary>
        private void HandleIgnore(object sender, EventArgs e)
        {
            // Ensure items are selected
            if (lvResults.SelectedIndices.Count == 0)
                return;

            // Get selected items
            foreach (OrgListItem selItem in lvResults.SelectedItems)
            {
                OrgItem selectedResult = selItem.OrgItem;

                foreach (OrgFolder scanDir in Settings.ScanDirectories)
                    if (scanDir.FolderPath == selectedResult.ScanDirectory.FolderPath)
                        scanDir.AddIgnoreFile(selectedResult.SourcePath);

                selectedResult.Category = FileCategory.Ignored;
                selectedResult.Action = OrgAction.None;
                selectedResult.DestinationPath = string.Empty;
                selectedResult.Check = CheckState.Unchecked;
            }
            Settings.Save();
            DisplayResults();
        }

        /// <summary>
        /// Handles unignore items selection from context menu
        /// </summary>
        private void HandleUnignore(object sender, EventArgs e)
        {
            // Ensure items are selected
            if (lvResults.SelectedIndices.Count == 0)
                return;

            // Get selected items
            foreach (OrgListItem selItem in lvResults.SelectedItems)
            {
                OrgItem selectedResult = selItem.OrgItem;

                foreach (OrgFolder scanDir in Settings.ScanDirectories)
                    if (scanDir.FolderPath == selectedResult.ScanDirectory.FolderPath)
                        if (scanDir.RemoveIgnoreFile(selectedResult.SourcePath))
                            selectedResult.Category = FileCategory.Unknown;
            }
            Settings.Save();
            DisplayResults();
        }

        /// <summary>
        /// Handles edit items selection from context menu
        /// </summary>
        private void HandleEdit(object sender, EventArgs e)
        {
            ModifyAction();
        }

        /// <summary>
        /// Handles locate tv item selection from context menu
        /// </summary>
        private void HandleLocateTv(object sender, EventArgs e)
        {
            // Ensure items are selected
            if (lvResults.SelectedIndices.Count == 0)
                return;

            // Open editor for selected item
            OrgItem selectedResult = lvResults.GetSelectedListItems()[0].OrgItem;

            // Locate
            List<OrgItem> items;
            if (selectedResult.TvEpisode.UserLocate(false, false, out items))
            {
                selectedResult.UpdateInfo(items[0]);
                DisplayResults();
            }
        }

        /// <summary>
        /// Handles turn off include in scan for show selection from context menu
        /// </summary>
        private void HandleTurnOffIncludeInScan(object sender, EventArgs e)
        {
            // Ensure items are selected
            if (lvResults.SelectedIndices.Count == 0)
                return;

            // Get selected shows
            List<TvShow> showsToBeSet = new List<TvShow>();
            foreach (OrgListItem selItem in lvResults.SelectedItems)
            {
                OrgItem selectedResult = selItem.OrgItem;
                if (selectedResult.TvEpisode == null)
                    return;

                TvShow show = selectedResult.TvEpisode.GetShow();
                if (show != null)
                {
                    show.DoRenaming = false;
                    show.DoMissingCheck = false;
                    if (!showsToBeSet.Contains(show))
                        showsToBeSet.Add(show);
                }
            }

            List<OrgItem> displayItems = lvResults.GetDisplayedOrgItems();
            foreach (OrgItem displayItem in displayItems)
            {
                foreach (TvShow show in showsToBeSet)
                    if (displayItem.TvEpisode != null && displayItem.TvEpisode.Show == show.Name)
                    {
                        displayItem.Category = FileCategory.Ignored;
                        break;
                    }
            }

            Organization.Shows.Save();
            DisplayResults();
        }

        /// <summary>
        /// Handles ignore tv items selection from context menu
        /// </summary>
        private void HandleIgnoreTv(object sender, EventArgs e)
        {
            // Ensure items are selected
            if (lvResults.SelectedIndices.Count == 0)
                return;

            // Get selected item
            foreach (OrgListItem selItem in lvResults.SelectedItems)
            {
                OrgItem selectedResult = selItem.OrgItem;
                if (selectedResult.TvEpisode == null)
                    return;

                selectedResult.TvEpisode.Ignored = true;
                selectedResult.Category = FileCategory.Ignored;

            }
            Organization.Shows.Save();
            DisplayResults();
        }

        /// <summary>
        /// Handles ignore tv items selection from context menu
        /// </summary>
        private void HandleIgnoreTvSeason(object sender, EventArgs e)
        {
            // Ensure items are selected
            if (lvResults.SelectedIndices.Count == 0)
                return;

            List<KeyValuePair<TvShow, int>> setShowSeasons = new List<KeyValuePair<TvShow, int>>();

            // Get selected item
            List<OrgItem> selItems = lvResults.GetSelectedOrgItems();
            for (int i = selItems.Count - 1; i >= 0; i--)
            {
                // Make sure it's a tv item
                if (selItems[i].TvEpisode == null)
                    return;

                // Set show's season to ignored
                TvShow show = selItems[i].TvEpisode.GetShow();
                show.Seasons[selItems[i].TvEpisode.Season].Ignored = true;

                // Save season
                KeyValuePair<TvShow, int> showSeason = new KeyValuePair<TvShow, int>(show, selItems[i].TvEpisode.Season);
                if (!setShowSeasons.Contains(showSeason))
                    setShowSeasons.Add(showSeason);
            }

            // Ignore all season episodes
            List<OrgItem> displayItems = lvResults.GetDisplayedOrgItems();
            for (int i = displayItems.Count - 1; i >= 0; i--)
            {
                // Make sure it's a tv item
                if (displayItems[i].TvEpisode == null)
                    return;

                // Get season/show
                KeyValuePair<TvShow, int> showSeason = new KeyValuePair<TvShow, int>(displayItems[i].TvEpisode.GetShow(), displayItems[i].TvEpisode.Season);
                if (setShowSeasons.Contains(showSeason))
                    displayItems[i].Category = FileCategory.Ignored;
                    
            } 

            Organization.Shows.Save();
            DisplayResults();
        }

        #endregion

        #region GUI Events Handling

        /// <summary>
        /// Tooltips setup on load
        /// </summary>
        private void ScanControl_Load(object sender, EventArgs e)
        {
            ToolTip ttFastScan = new ToolTip();
            ttFastScan.SetToolTip(chkFast, "Skips all items that require database searching.");
        }

        /// <summary>
        /// Edit list button for directory scan list opens settings to scan folders tab.
        /// </summary>
        private void btnEditScanDirs_Click(object sender, EventArgs e)
        {
            SettingsForm settingForm = new SettingsForm(0);
            settingForm.ShowDialog();
        }

        /// <summary>
        /// Edit list button for movie check opens settings to movie folders tab.
        /// </summary>
        private void btnEditMovieFolders_Click(object sender, EventArgs e)
        {
            SettingsForm settingForm = new SettingsForm(2);
            settingForm.ShowDialog();
        }

        /// <summary>
        /// Run button runs a scan.
        /// </summary>
        private void btnRun_Click(object sender, EventArgs e)
        {
            if (btnRun.Text == "Cancel")
            {
                CancelScan();
                return;
            }

            
            // Setup columns based on scan type to run
            OrgItemColumnSetup colSetup;
            ScanType scanType = (ScanType)cmbScanType.SelectedItem;
            switch (scanType)
            {
                case ScanType.Directory:
                    colSetup = OrgItemColumnSetup.DirectoryScan;
                    break;
                case ScanType.TvMissing:
                case ScanType.TvRename:
                    colSetup = OrgItemColumnSetup.MissingCheck;
                    break;
                default:
                    colSetup = OrgItemColumnSetup.RootFolderCheck;
                    break;
            }
            lvResults.SetColumns(colSetup);

            // Run scan
            ScanRunEnables(false);
            DoScan();
        }

        private void ScanRunEnables(bool enable)
        {
            gbActionFilter.Enabled = enable;
            gbCategoryFilter.Enabled = enable;
            cmbScanType.Enabled = enable;
            cmbScanSelection.Enabled = enable;

            if (enable)
                btnRun.Text = "Run";
            else
            {
                LinkProgressEvents();
                btnRun.Text = "Cancel";
            }
        }

        /// <summary>
        /// Handles check change on scan listview item
        /// </summary>
        private void lvResults_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            // Prevent checking on items with no action
            if (e != null)
            {
                List<OrgItem> displayItems = lvResults.GetDisplayedOrgItems();
                if (e.Item.SubItems.Count > 1)
                {
                    OrgAction action = displayItems[e.Item.Index].Action;
                    if (action == OrgAction.None || action == OrgAction.Queued || action == OrgAction.AlreadyExists || action == OrgAction.NoRootFolder)
                        e.Item.Checked = false;
                }

                // Mirror check state to scan results list
                displayItems[e.Item.Index].Check = e.Item.Checked ? CheckState.Checked : CheckState.Unchecked;
            }

            // De-register check change events (to prevent this method being called again)
            chkMoveCopy.CheckedChanged -= new System.EventHandler(this.chkMoveCopy_CheckedChanged);
            chkRenameDelete.CheckedChanged -= new System.EventHandler(this.chkRenameDelete_CheckedChanged);

            // Set combined check boxes
            chkMoveCopy.CheckState = lvResults.GetCheckState(new OrgAction[] { OrgAction.Copy, OrgAction.Move });
            if (lastRunScan == ScanType.Directory)
                chkRenameDelete.CheckState = lvResults.GetCheckState(OrgAction.Delete);
            else
                chkRenameDelete.CheckState = lvResults.GetCheckState(OrgAction.Rename);

            // Re-register check changes events
            chkMoveCopy.CheckedChanged += new System.EventHandler(this.chkMoveCopy_CheckedChanged);
            chkRenameDelete.CheckedChanged += new System.EventHandler(this.chkRenameDelete_CheckedChanged);

            // Set add to queue button enable
            bool itemsChecked = false;
            foreach(ListViewItem item in lvResults.Items)
                if (item.Checked)
                {
                    itemsChecked = true;
                    break;
                }
            btnQueueSelected.Enabled = itemsChecked && !scanRunning;
        }

        /// <summary>
        /// Set all move/copy check items when global check is changed
        /// </summary>
        private void chkMoveCopy_CheckedChanged(object sender, EventArgs e)
        {
            lvResults.SetCheckItems(new OrgAction[] { OrgAction.Copy, OrgAction.Move }, chkMoveCopy.Checked);
        }

        /// <summary>
        /// Set all rename item when global check is changes
        /// </summary>
        private void chkRenameDelete_CheckedChanged(object sender, EventArgs e)
        {
            switch (lastRunScan)
            {
                case ScanType.Directory:
                    lvResults.SetCheckItems(OrgAction.Delete, chkRenameDelete.Checked);
                    break;
                case ScanType.TvMissing:
                    lvResults.SetCheckItems(OrgAction.Rename, chkRenameDelete.Checked);
                    break;
                case ScanType.MovieFolder:
                    lvResults.SetCheckItems(OrgAction.Rename, chkRenameDelete.Checked);
                    break;
            }
        }

        /// <summary>
        /// Set modify action button enable when listview selection changes
        /// </summary>
        private void lvResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnModAction.Enabled = GetSelectedStatus() != OrgStatus.Missing && GetSelectedAction() != OrgAction.Queued;
        }

        /// <summary>
        /// Open action modifier form when modify action button is clicked
        /// </summary>
        private void btnModAction_Click(object sender, EventArgs e)
        {
            ModifyAction();
        }

        /// <summary>
        /// Open modify action form and applies changes to action
        /// </summary>
        private void ModifyAction()
        {
            // Ensure items are selected
            if (lvResults.SelectedIndices.Count == 0)
                return;

            // Open editor for selected item
            OrgItem selectedResult = lvResults.GetSelectedOrgItems()[0];
            if (selectedResult.Action == OrgAction.Queued)
                return;

            OrgItemEditor sam = new OrgItemEditor(selectedResult);
            sam.ShowDialog();

            // Apply results if valid
            if (sam.Result != null)
            {
                selectedResult.UpdateInfo(sam.Result);
                if (sam.Result.Action != OrgAction.None)
                    selectedResult.Check = CheckState.Checked;

                // Automatically apply the movie to unknown items with similar names
                if (lastRunScan == ScanType.Directory && sam.Result.Movie != null && (sam.Result.Action == OrgAction.Move || sam.Result.Action == OrgAction.Copy))
                {
                    List<OrgItem> displayItems = lvResults.GetDisplayedOrgItems();
                    foreach (OrgItem item in displayItems)
                    {
                        if (item.Action == OrgAction.None && item.Movie == null && item.SourcePath.Length == sam.Result.SourcePath.Length)
                        {
                            int diffCnt = 0;
                            for (int i = 0; i < item.SourcePath.Length; i++)
                                if (item.SourcePath[i] != sam.Result.SourcePath[i])
                                    diffCnt++;

                            if (diffCnt == 1)
                            {
                                item.Movie = sam.Result.Movie;
                                item.DestinationPath = item.Movie.BuildFilePath(item.SourcePath);
                                item.Action = sam.Result.Action;
                                item.Check = CheckState.Checked;
                            }
                        }
                    }
                }


                DisplayResults();
            }
        }

        /// <summary>
        /// Get organization status of selected item in listview
        /// </summary>
        /// <returns></returns>
        private OrgStatus GetSelectedStatus()
        {
            // Check that an item is selected
            if (lvResults.SelectedIndices.Count == 0)
                return OrgStatus.Missing;

            // Retun status for selected item
            return lvResults.GetSelectedOrgItems()[0].Status;
        }

        /// <summary>
        /// Gets action of selected item.
        /// </summary>
        /// <returns></returns>
        private OrgAction GetSelectedAction()
        {
            // Check that an item is selected
            if (lvResults.SelectedIndices.Count == 0)
                return OrgAction.Queued;

            // Retun status for selected item
            return lvResults.GetSelectedOrgItems()[0].Action;
        }

        /// <summary>
        /// Queue button moves check item to queue.
        /// </summary>
        private void btnQueueSelected_Click(object sender, EventArgs e)
        {
            QueueCheckItems();
        }

        /// <summary>
        /// Add checked item from listview to queue
        /// </summary>
        private void QueueCheckItems()
        {
            // Get checked items
            List<OrgItem> itemsToQueue = new List<OrgItem>();
            //lock (displayItems)
            List<OrgItem> displayItems = lvResults.GetDisplayedOrgItems();
            for (int i = 0; i < displayItems.Count; i++)
                if (displayItems[i].Check == CheckState.Checked)
                {
                    // Add item
                    itemsToQueue.Add(new OrgItem(displayItems[i]));

                    // Update item in scan list
                    displayItems[i].Action = OrgAction.Queued;
                    displayItems[i].Check = CheckState.Unchecked;
                }

            if (lastRunScan == ScanType.MovieFolder || lastRunScan == ScanType.TvMissing)
            {
                OrgItem.AscendingSort = true;
                itemsToQueue.Sort(OrgItem.CompareByNumber);
            }

            // Queue items
            OnItemsToQueue(itemsToQueue);

            // Refresh Results
            DisplayResults();
        }

        /// <summary>
        /// Double-clicking an item in listview open editor for it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvResults_DoubleClick(object sender, EventArgs e)
        {
            // Prevent check from inverting automatically 
            if (lvResults.SelectedItems.Count > 0)
                lvResults.SelectedItems[0].Checked = !lvResults.SelectedItems[0].Checked;

            ModifyAction();
        }

        /// <summary>
        /// Refresh of listview is triggered when ignored filter check box is changed
        /// </summary>
        private void chkFilter_CheckedChanged(object sender, EventArgs e)
        {
            // Action Filter
            lvResults.ActionFilter = OrgAction.NoRootFolder | OrgAction.TBD | OrgAction.Processing;;
            if (chkNoneFilter.Checked) 
                lvResults.ActionFilter |= OrgAction.None | OrgAction.AlreadyExists;
            if (chkMoveCopyFilter.Checked)
                lvResults.ActionFilter |= OrgAction.Move | OrgAction.Copy;
            if (chkRenameFilter.Checked)
                lvResults.ActionFilter |= OrgAction.Rename;
            if (chkDelFilter.Checked)
                lvResults.ActionFilter |= OrgAction.Delete;
            if (chkQueueFilter.Checked)
                lvResults.ActionFilter |= OrgAction.Queued;
            
            // Category Filter
            lvResults.CategoryFilter = FileCategory.Empty;
            if (chkTvCatFilter.Checked)
                lvResults.CategoryFilter |= FileCategory.TvVideo;
            if (chkNonTvCatFilter.Checked)
                lvResults.CategoryFilter |= FileCategory.NonTvVideo;
            if (chkCustomCatFilter.Checked)
                lvResults.CategoryFilter |= FileCategory.Custom;
            if (chkTrashCatFilter.Checked)
                lvResults.CategoryFilter |= FileCategory.Trash;
            if (chkUnknownCatFilter.Checked)
                lvResults.CategoryFilter |= FileCategory.Unknown;
            if (chkFolderCatFilter.Checked)
                lvResults.CategoryFilter |= FileCategory.Folder;
            if (chkIgnoreCatFilter.Checked)
                lvResults.CategoryFilter |= FileCategory.Ignored;

            DisplayResults();
        }

        private void cmbScanType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateScanSelection();
        }

        #endregion

        #region Updating

        /// <summary>
        /// Update scan selection combo box based on scan type that is selected.
        /// </summary>
        public void UpdateScanSelection()
        {
            // Save current selection
            string currSelection = string.Empty;
            if (cmbScanSelection.SelectedItem != null)
                currSelection = cmbScanSelection.SelectedItem.ToString();

            if(cmbScanType.SelectedItem == null)
                return;

            // Rebuild combo box items
            cmbScanSelection.Items.Clear();
            bool selected = false;
            ScanType scanType = (ScanType)cmbScanType.SelectedItem;
            chkFast.Visible = true;
            switch(scanType)
            {
                case ScanType.Directory:
                    cmbScanSelection.Items.Add("All Scan Directories");
                    foreach (OrgFolder folder in Settings.ScanDirectories)
                    {
                        // Add item
                        cmbScanSelection.Items.Add(folder.FolderPath);

                        // Check if item matches previous selection
                        if (folder.FolderPath == currSelection)
                        {
                            cmbScanSelection.SelectedIndex = cmbScanSelection.Items.Count - 1;
                            selected = true;
                        }
                    }
                    break;
                case ScanType.MovieFolder:
                    cmbScanSelection.Items.Add("All Movie Root Folders");
                    foreach (ContentRootFolder folder in Settings.MovieFolders)
                    {
                        // Add item
                        cmbScanSelection.Items.Add(folder);

                        // Check if item matches previous selection
                        if (folder.ToString() == currSelection)
                        {
                            cmbScanSelection.SelectedIndex = cmbScanSelection.Items.Count - 1;
                            selected = true;
                        }
                    }
                    break;
                case ScanType.TvFolder:
                    cmbScanSelection.Items.Add("All TV Root Folders");
                    foreach (ContentRootFolder folder in Settings.TvFolders)
                    {
                        // Add item
                        cmbScanSelection.Items.Add(folder);

                        // Check if item matches previous selection
                        if (folder.ToString() == currSelection)
                        {
                            cmbScanSelection.SelectedIndex = cmbScanSelection.Items.Count - 1;
                            selected = true;
                        }
                    }
                    break;
                case ScanType.TvMissing:
                case ScanType.TvRename:
                    cmbScanSelection.Items.Add("All Shows");
                    foreach (TvShow show in Organization.Shows.GetScannableContent(false))
                    {
                        // Add item
                        cmbScanSelection.Items.Add(show.Name);

                        // Check if item matches previous selection
                        if (show.Name == currSelection)
                        {
                            cmbScanSelection.SelectedIndex = cmbScanSelection.Items.Count - 1;
                            selected = true;
                        }
                    }
                    chkFast.Visible = false;
                    break;
        }

            // Select first item if previous selection was not set
            if (!selected)
                cmbScanSelection.SelectedIndex = 0;
        }

        /// <summary>
        /// Updates private list of items in queue
        /// </summary>
        /// <param name="queuedItems"></param>
        public void UpdateQueueItems(List<OrgItem> queuedItems)
        {
            this.queuedItems = queuedItems;
        }

        #endregion

        #region Scanning

        /// <summary>
        /// Work event for scanning worker.
        /// </summary>
        private void scanWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Cast arguments to object array
            object[] args = (object[])e.Argument;

            // Perform appropriate scan
            ScanType scanType = (ScanType)args[0];
            List<OrgItem> scanResults = new List<OrgItem>();
            switch (scanType)
            {
                case ScanType.Directory:
                    directoryScan.RunScan((List<OrgFolder>)args[1], queuedItems, false, false, (bool)args[2]);
                    break;
                case ScanType.TvMissing:
                    scanResults = tvMissingScan.RunScan((List<Content>)args[1], queuedItems, (bool)args[2]);
                    break;
                case ScanType.TvRename:
                    scanResults = tvRenameScan.RunScan((List<Content>)args[1], queuedItems);
                    break;
                case ScanType.TvFolder:
                    scanResults = tvFolderScan.RunScan((List<ContentRootFolder>)args[1], queuedItems, (bool)args[2]);
                    break;
                case ScanType.MovieFolder:
                    scanResults = movieFolderScan.RunScan((List<ContentRootFolder>)args[1], queuedItems);
                    break;
                default:
                    throw new Exception("Unknown scan type!");
            }

            this.Invoke((MethodInvoker)delegate
            {
                lvResults.ItemChecked -= new ItemCheckedEventHandler(lvResults_ItemChecked);
                if (scanType != ScanType.Directory)
                    lvResults.SetOrgItems(scanResults);
                lvResults_ItemChecked(null, null);
                lvResults.ItemChecked += new ItemCheckedEventHandler(lvResults_ItemChecked);
            });
        }

        /// <summary>
        /// Scan progress is shown in progress bar. Results are shown in listview as they are processed.
        /// </summary>
        private void scan_ScanProgressChange(object sender, ProgressChangedEventArgs e)
        {
            if (!scanRunning)
                this.Invoke((MethodInvoker)delegate
                {
                    pbScanProgress.Value = 0;
                    pbScanProgress.Message = string.Empty;
                });
            
            // Get process
            ScanProcess process = (ScanProcess)sender;
            string info = (string)e.UserState;
            
            // Get progress bar message based on scan type
            string msg = process.Description();
            if (e.ProgressPercentage == 100)
                msg += " - Complete";
            else if (process != ScanProcess.FileCollect && process != ScanProcess.Directory)
                msg += " - Processing File '" + Path.GetFileName(info) + "'";

            this.Invoke((MethodInvoker)delegate
            {
                // Set message and value
                pbScanProgress.Message = msg;
                if (e.ProgressPercentage > 100)
                    pbScanProgress.Value = 100;
                else
                    pbScanProgress.Value = e.ProgressPercentage;
                DisplayResults(true);
            });
        }

        /// <summary>
        /// Scan worker complete handler
        /// </summary>
        private void scanWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // If last scan was cancelled run new scan
            if (rescanRequired)
            {
                rescanRequired = false;
                DoScan();
            }
            else if (scanCancelled)
            {
                scanCancelled = false;
                UnlinkProgressEvents();
                this.Invoke((MethodInvoker)delegate
               {
                   List<OrgItem> items = lvResults.GetOrgItems();
                   for (int i = items.Count - 1; i >= 0; i--)
                   {
                       if (items[i].Action == OrgAction.Processing || items[i].Action == OrgAction.TBD)
                           lvResults.RemoveItemAt(i);
                   }
                   scanRunning = false;
                   DisplayResults();
                   ScanRunEnables(true);
                   pbScanProgress.Value = 0;
                   pbScanProgress.Message = string.Empty;
               });
            }
            else
            {
                // Display actions in listview
                //lastClickedColumn = 0;
                UnlinkProgressEvents();
                this.Invoke((MethodInvoker)delegate
                {
                    pbScanProgress.Value = 100;
                    Application.DoEvents(); // TODO: is this needed? Seems hacky!
                    scanRunning = false;

                    DisplayResults();
                    ScanRunEnables(true);
                });
            }
        }

        /// <summary>
        /// Performs scan
        /// </summary>
        private void DoScan()
        {
            scanRunning = true;
            
            // Clear results
            lvResults.Items.Clear();
            pbScanProgress.Value = 0;
            
            //sortAcending = true;
            bool fast = chkFast.Visible && chkFast.Checked;
            
            // Build scan parameters
            ScanType scanType = (ScanType)cmbScanType.SelectedItem;
            object list;
            switch (scanType)
            {
                case ScanType.Directory:
                    // Get folders from combo box
                    List<OrgFolder> scanDirs;
                    if (cmbScanSelection.SelectedIndex == 0)
                        scanDirs = Settings.ScanDirectories;
                    else
                    {
                        scanDirs = new List<OrgFolder>();
                        foreach (OrgFolder fld in Settings.ScanDirectories)
                            if (fld.FolderPath == cmbScanSelection.SelectedItem.ToString())
                            {
                                scanDirs.Add(fld);
                                break;
                            }
                    }
                    list = scanDirs;
                    // TODO
                    //sortType = OrgColumnType.Source_Folder;
                    break;

                case ScanType.TvRename:
                case ScanType.TvMissing:
                    // Get shows to check from combo box
                    List<Content> shows;
                    if (cmbScanSelection.SelectedIndex <= 0)
                        shows = Organization.Shows.GetScannableContent(true);
                    else
                    {
                        shows = new List<Content>();
                        shows.Add(Organization.Shows.GetScannableContent(true)[cmbScanSelection.SelectedIndex - 1]);
                    }
                    list = shows;
                    // TODO
                    //sortType = OrgColumnType.Show;
                    break;
                case ScanType.TvFolder:
                    // Get folders from combo box
                    List<ContentRootFolder> tvFolders;
                    if (cmbScanSelection.SelectedIndex == 0)
                        tvFolders = Settings.TvFolders;
                    else
                    {
                        tvFolders = new List<ContentRootFolder>();
                        foreach (ContentRootFolder fld in Settings.TvFolders)
                            if (fld.FullPath == cmbScanSelection.SelectedItem.ToString())
                            {
                                tvFolders.Add(fld);
                                break;
                            }
                    }
                    list = tvFolders;
                    // TODO
                    //sortType = OrgColumnType.Source_Folder;
                    break;
                case ScanType.MovieFolder:
                    // Get folders from combo box
                    List<ContentRootFolder> movieFolders;
                    if (cmbScanSelection.SelectedIndex == 0)
                        movieFolders = Settings.MovieFolders;
                    else
                    {
                        movieFolders = new List<ContentRootFolder>();
                        foreach (ContentRootFolder fld in Settings.MovieFolders)
                            if (fld.FullPath == cmbScanSelection.SelectedItem.ToString())
                            {
                                movieFolders.Add(fld);
                                break;
                            }
                    }
                    list = movieFolders;
                    // TODO
                    //sortType = OrgColumnType.Source_Folder; 
                    break;
                default:
                    throw new Exception("Unknown scan type");
            }

            // Start worker if not busy, otherwise cancel current scan and set rescan required flag
            if (!scanWorker.IsBusy)
            {
                currentScan = scanType;
                scanWorker.RunWorkerAsync(new object[] { scanType, list, fast });
                lastRunScan = scanType;
            }
            else
            {
                rescanRequired = true;
                CancelScan();
            }

            // Reset check selection
            chkMoveCopy.Text = lastRunScan == ScanType.TvMissing ? "Found" : "Move/Copy";
            chkRenameDelete.Text = lastRunScan == ScanType.Directory ? "Delete" : "Organization";
            chkMoveCopy.CheckState = lvResults.GetCheckState(new OrgAction[] { OrgAction.Copy, OrgAction.Move });
            chkRenameDelete.CheckState = lvResults.GetCheckState(new OrgAction[] { lastRunScan == ScanType.Directory ? OrgAction.Delete : OrgAction.Rename });
        }

        /// <summary>
        /// Cancels currently running scan.
        /// </summary>
        private void CancelScan()
        {
            scanCancelled = true;
            directoryScan.CancelScan();
            tvMissingScan.CancelScan();
            tvRenameScan.CancelScan();
            tvFolderScan.CancelScan();
            movieFolderScan.CancelScan();
            pbScanProgress.Value = 0;
            pbScanProgress.Message = string.Empty;
        }

        /// <summary>
        /// Displays scan results in listview
        /// </summary>
        private void DisplayResults(bool updating = false)
        {
            lvResults.ItemChecked -= new ItemCheckedEventHandler(lvResults_ItemChecked);

            lvResults.UpdateDisplay(5);

            lvResults.ItemChecked += new ItemCheckedEventHandler(lvResults_ItemChecked);
            // Update selection
            //OrgItem selItem = null;
            //int selIndex = -1;
            //if (lvResults.SelectedIndices.Count > 0)
            //{
            //    selItem = lvResults.DisplayedListItems[0].OrgItem;
            //    selIndex = lvResults.SelectedIndices[0];
            //}
            
            //// Filter and sort items for display
            //if (!updating)
            //    //lock (displayItems)
            //    {
            //        displayItems = FilterResults(scanResults);
            //        OrgItem.AscendingSort = sortAcending;
            //        OrgItem.Sort(displayItems, sortType);
            //    }

            //// De-register listview events
            //lvResults.SelectedIndexChanged -= new System.EventHandler(this.lvResults_SelectedIndexChanged);
            //lvResults.ItemChecked -= new System.Windows.Forms.ItemCheckedEventHandler(this.lvResults_ItemChecked);

            //lock (displayItems)
            //    OrgItemListHelper.DisplayOrgItemInList(displayItems, lvResults, scanColumns, new int[1] { -1 }, updating);
            
            //// Re-register listview events
            //lvResults.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvResults_ItemChecked);
            //lvResults.SelectedIndexChanged += new System.EventHandler(this.lvResults_SelectedIndexChanged);

            //if (lvResults.Items.Count > 0)
            //{
            //    lvResults_ItemChecked(null, new ItemCheckedEventArgs(lvResults.Items[0]));
            //    lvResults_SelectedIndexChanged(null, new ItemCheckedEventArgs(lvResults.Items[0]));
            //}
            
            
        }

        /// <summary>
        /// Filters items based on checkbox filters in control
        /// </summary>
        /// <param name="unfilteredResults">Unfiltered items to be filterd</param>
        /// <returns>Resulting filtered list</returns>
        private List<OrgItem> FilterResults(List<OrgItem> unfilteredResults)
        {
            List<OrgItem> filteredResults = new List<OrgItem>();

            foreach (OrgItem result in unfilteredResults)
            {
                switch (result.Action)
                {
                    case OrgAction.None:
                    case OrgAction.NoRootFolder:
                    case OrgAction.AlreadyExists:
                        if (!chkNoneFilter.Checked)
                            continue;
                        break;
                    case OrgAction.Move:
                    case OrgAction.Copy:
                        if (!chkMoveCopyFilter.Checked)
                            continue;
                        break;
                    case OrgAction.Rename:
                        if (!chkRenameFilter.Checked)
                            continue;
                        break;
                    case OrgAction.Delete:
                        if (!chkDelFilter.Checked)
                            continue;
                        break;
                    case OrgAction.Queued:
                        if (!chkQueueFilter.Checked)
                            continue;
                        break;
                    case OrgAction.TBD:
                    case OrgAction.Processing:
                        break;
                    default:
                        throw new Exception("Bad filter type");
                }

                switch (result.Category)
                {
                    case FileCategory.TvVideo:
                        if (!chkTvCatFilter.Checked)
                            continue;
                        break;
                    case FileCategory.NonTvVideo:
                        if (!chkNonTvCatFilter.Checked)
                            continue;
                        break;
                    case FileCategory.Ignored:
                        if (!chkIgnoreCatFilter.Checked)
                            continue;
                        break;
                    case FileCategory.Custom:
                        if (!chkCustomCatFilter.Checked)
                            continue;
                        break;
                    case FileCategory.Unknown:
                        if (!chkUnknownCatFilter.Checked)
                            continue;
                        break;
                    case FileCategory.Trash:
                        if (!chkTrashCatFilter.Checked)
                            continue;
                        break;
                    case FileCategory.Folder:
                        if (!chkFolderCatFilter.Checked)
                            continue;
                        break;

                }
                filteredResults.Add(result);
            }

            return filteredResults;
        }

        #endregion

    }
}
