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
        /// Items currently displayed in listview. May differ from all items due to ignored/hidden items.
        /// </summary>
        private List<OrgItem> displayItems = new List<OrgItem>();

        /// <summary>
        /// List of organization items resulting from last run scan
        /// </summary>
        private List<OrgItem> scanResults = new List<OrgItem>();

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
        /// Set of column contained in the scan listview
        /// </summary>
        private Dictionary<OrgColumnType, OrgItemColumn> scanColumns;

        /// <summary>
        /// Last column clicked in scan listview
        /// </summary>
        private OrgColumnType lastClickedColumn = 0;

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
        /// Column type listview is currently sorted on
        /// </summary>
        private OrgColumnType sortType = OrgColumnType.Source_Folder;

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

            // Setup scan worker
            scanWorker = new BackgroundWorker();
            scanWorker.WorkerSupportsCancellation = true;
            scanWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(scanWorker_RunWorkerCompleted);
            scanWorker.DoWork += new DoWorkEventHandler(scanWorker_DoWork);

            // Register to scan helper progress change (for tracking prgress of running scan)
            directoryScan.ProgressChange += new EventHandler<ProgressChangedEventArgs>(ScanHelper_ScanProgressChange);
            tvMissingScan.ProgressChange += new EventHandler<ProgressChangedEventArgs>(ScanHelper_ScanProgressChange);
            tvRenameScan.ProgressChange += new EventHandler<ProgressChangedEventArgs>(ScanHelper_ScanProgressChange);
            tvFolderScan.ProgressChange += new EventHandler<ProgressChangedEventArgs>(ScanHelper_ScanProgressChange);
            movieFolderScan.ProgressChange += new EventHandler<ProgressChangedEventArgs>(ScanHelper_ScanProgressChange);

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
            for (int i = 0; i < lvResults.SelectedIndices.Count; i++)
            {
                
                OrgItem currItem = displayItems[lvResults.SelectedIndices[i]];
                if (currItem.Action != OrgAction.AlreadyExists)
                    allAlreadyExists = false;
                if (currItem.Action == OrgAction.Copy || currItem.Action == OrgAction.Move)
                {
                    if (currItem.Category != FileHelper.FileCategory.NonTvVideo)
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

            if (lvResults.SelectedItems.Count == 1 && displayItems[lvResults.SelectedIndices[0]].Status != OrgStatus.Missing)
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
            for (int i = 0; i < lvResults.SelectedIndices.Count; i++)
            {
                // Get current item
                OrgItem currItem = displayItems[lvResults.SelectedIndices[i]];

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
            DisplayResults();

        }

        /// <summary>
        /// Handles folder change action from context menu
        /// </summary>
        private void HandleFolderChange(object sender, EventArgs e)
        {
            string destinationFolder = ((MenuItem)sender).Text;

            for (int i = 0; i < lvResults.SelectedIndices.Count; i++)
            {
                OrgItem currItem = displayItems[lvResults.SelectedIndices[i]];

                if (currItem.Category == FileHelper.FileCategory.NonTvVideo && currItem.Movie != null)
                {
                    currItem.Movie.RootFolder = destinationFolder;
                    currItem.DestinationPath = currItem.Movie.BuildFilePath(currItem.SourcePath);
                }
            }

            // Update display
            DisplayResults();
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
            for (int i = lvResults.SelectedIndices.Count - 1; i >= 0; i--)
            {
                OrgItem selectedResult = displayItems[lvResults.SelectedIndices[i]];

                foreach (OrgFolder scanDir in Settings.ScanDirectories)
                    if (scanDir.FolderPath == selectedResult.ScanDirectory.FolderPath)
                        scanDir.AddIgnoreFile(selectedResult.SourcePath);

                selectedResult.Category = FileHelper.FileCategory.Ignored;
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
            for (int i = lvResults.SelectedIndices.Count - 1; i >= 0; i--)
            {
                OrgItem selectedResult = displayItems[lvResults.SelectedIndices[i]];

                foreach (OrgFolder scanDir in Settings.ScanDirectories)
                    if (scanDir.FolderPath == selectedResult.ScanDirectory.FolderPath)
                        if (scanDir.RemoveIgnoreFile(selectedResult.SourcePath))
                            selectedResult.Category = FileHelper.FileCategory.Unknown;
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
            OrgItem selectedResult = displayItems[lvResults.SelectedIndices[0]];

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
            for (int i = lvResults.SelectedIndices.Count - 1; i >= 0; i--)
            {
                OrgItem selectedResult = displayItems[lvResults.SelectedIndices[i]];
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

            for (int i = lvResults.Items.Count - 1; i >= 0; i--)
                foreach (TvShow show in showsToBeSet)
                    if (displayItems[i].TvEpisode != null && displayItems[i].TvEpisode.Show == show.Name)
                    {
                        displayItems[i].Category = FileHelper.FileCategory.Ignored;
                        break;
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
            for (int i = lvResults.SelectedIndices.Count - 1; i >= 0; i--)
            {
                OrgItem selectedResult = displayItems[lvResults.SelectedIndices[i]];
                if (selectedResult.TvEpisode == null)
                    return;

                selectedResult.TvEpisode.Ignored = true;
                selectedResult.Category = FileHelper.FileCategory.Ignored;

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
            for (int i = lvResults.SelectedIndices.Count - 1; i >= 0; i--)
            {
                // Make sure it's a tv item
                OrgItem selectedResult = displayItems[lvResults.SelectedIndices[i]];
                if (selectedResult.TvEpisode == null)
                    return;

                // Set show's season to ignored
                TvShow show = selectedResult.TvEpisode.GetShow();
                show.Seasons[selectedResult.TvEpisode.Season].Ignored = true;

                // Save season
                KeyValuePair<TvShow, int> showSeason = new KeyValuePair<TvShow, int>(show, selectedResult.TvEpisode.Season);
                if (!setShowSeasons.Contains(showSeason))
                    setShowSeasons.Add(showSeason);
            }

            // Ignore all season episodes
            for (int i = lvResults.Items.Count - 1; i >= 0; i--)
            {
                // Make sure it's a tv item
                OrgItem currResult = displayItems[i];
                if (currResult.TvEpisode == null)
                    return;

                // Get season/show
                KeyValuePair<TvShow, int> showSeason = new KeyValuePair<TvShow, int>(currResult.TvEpisode.GetShow(), currResult.TvEpisode.Season);
                if (setShowSeasons.Contains(showSeason))
                    currResult.Category = FileHelper.FileCategory.Ignored;
                    
            }
                

            Organization.Shows.Save();
            DisplayResults();
        }

        #endregion

        #region Events Handling

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
            // Setup columns based on scan type to run
            OrgItemListHelper.OrgColumnSetup colSetup;
            ScanType scanType = (ScanType)cmbScanType.SelectedItem;
            switch (scanType)
            {
                case ScanType.Directory:
                    colSetup = OrgItemListHelper.OrgColumnSetup.DirectoryScan;
                    break;
                case ScanType.TvMissing:
                case ScanType.TvRename:
                    colSetup = OrgItemListHelper.OrgColumnSetup.MissingCheck;
                    break;
                default:
                    colSetup = OrgItemListHelper.OrgColumnSetup.RootFolderCheck;
                    break;
            }
            scanColumns = OrgItemListHelper.SetOrgItemListColumns(colSetup, lvResults);
            sortType = OrgColumnType.Source_Folder;

            // Run scan
            DoScan();
        }

        /// <summary>
        /// Handles check change on scan listview item
        /// </summary>
        private void lvResults_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            // Prevent checking on items with no action
            if (e.Item.SubItems.Count > 1)
            {
                OrgAction action = displayItems[e.Item.Index].Action;
                if (action == OrgAction.None || action == OrgAction.Queued || action == OrgAction.AlreadyExists || action == OrgAction.NoRootFolder)
                    e.Item.Checked = false;
            }

            // Mirror check state to scan results list
            lock (displayItems)
                displayItems[e.Item.Index].Check = e.Item.Checked ? CheckState.Checked : CheckState.Unchecked;

            // De-register check change events (to prevent this method being called again)
            chkMoveCopy.CheckedChanged -= new System.EventHandler(this.chkMoveCopy_CheckedChanged);
            chkRenameDelete.CheckedChanged -= new System.EventHandler(this.chkRenameDelete_CheckedChanged);

            // Set combined check boxes
            chkMoveCopy.CheckState = GetCheckState(new OrgAction[] { OrgAction.Copy, OrgAction.Move });
            if (lastRunScan == ScanType.Directory)
                chkRenameDelete.CheckState = GetCheckState(OrgAction.Delete);
            else
                chkRenameDelete.CheckState = GetCheckState(OrgAction.Rename);

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
            btnQueueSelected.Enabled = itemsChecked;
        }

        /// <summary>
        /// Set all move/copy check items when global check is changed
        /// </summary>
        private void chkMoveCopy_CheckedChanged(object sender, EventArgs e)
        {
            SetCheckItems(new OrgAction[] { OrgAction.Copy, OrgAction.Move }, chkMoveCopy.Checked);
        }

        /// <summary>
        /// Set all rename item when global check is changes
        /// </summary>
        private void chkRenameDelete_CheckedChanged(object sender, EventArgs e)
        {
            switch (lastRunScan)
            {
                case ScanType.Directory:
                    SetCheckItems(OrgAction.Delete, chkRenameDelete.Checked);
                    break;
                case ScanType.TvMissing:
                    SetCheckItems(OrgAction.Rename, chkRenameDelete.Checked);
                    break;
                case ScanType.MovieFolder:
                    SetCheckItems(OrgAction.Rename, chkRenameDelete.Checked);
                    break;
            }
        }
        
        /// <summary>
        /// Set scan item check to for all items with a specific action
        /// </summary>
        /// <param name="action">Action type for item to set check</param>
        /// <param name="check">Value to set checked to</param>
        private void SetCheckItems(OrgAction action, bool check)
        {
            SetCheckItems(new OrgAction[] { action }, check);
        }

        /// <summary>
        /// Set scan item check to for all items with a specific actions
        /// </summary>
        /// <param name="action">Action types for item to set check</param>
        /// <param name="check">Value to set checked to</param>
        private void SetCheckItems(OrgAction[] actions, bool check)
        {
            // Get column index for action
            int actionColIndex = scanColumns[OrgColumnType.Action].Header.Index;
            if (actionColIndex == -1)
                return;
            
            // Set check state for all items that match any of the action types
            for(int i=0;i<lvResults.Items.Count;i++)
                foreach (OrgAction action in actions)
                    if (lvResults.Items[i].SubItems.Count > 1 && lvResults.Items[i].SubItems[actionColIndex].Text == action.ToString())
                    {
                        lvResults.Items[i].Checked = check;
                        lock (displayItems)
                            displayItems[i].Check = check ? CheckState.Checked : CheckState.Unchecked;
                    }
        }

        /// <summary>
        /// Get combined check state of all item of set of action types
        /// </summary>
        /// <param name="actions">Set of actions types to get check state from</param>
        /// <returns>Combined check state of all item match any of the specified actions</returns>
        private CheckState GetCheckState(OrgAction[] actions)
        {
            if (scanColumns == null)
                return CheckState.Unchecked;

            // Get column with action type in it
            int actionColIndex = scanColumns[OrgColumnType.Action].Header.Index;
            if (actionColIndex == -1)
                return CheckState.Unchecked;

            // Set booleans indicating if there are any checked/unchecked item it listview
            bool check = false;
            bool notCheck = false;
            foreach (ListViewItem item in lvResults.Items)
                foreach (OrgAction action in actions)
                    if (item.SubItems.Count > 1 && item.SubItems[actionColIndex].Text == action.ToString())
                    {
                        if (item.Checked)
                            check = true;
                        else
                            notCheck = true;
                    };

            // Return combined check state
            if (check && !notCheck)
                return CheckState.Checked;
            else if (check && notCheck)
                return CheckState.Indeterminate;
            else
                return CheckState.Unchecked;
        }

        /// <summary>
        /// Get combined check state of all item of specific action types
        /// </summary>
        /// <param name="action">Actions type to get check state from</param>
        /// <returns>Combined check state of all item match the specified action</returns>
        private CheckState GetCheckState(OrgAction action)
        {
            return GetCheckState(new OrgAction[] { action });
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
            OrgItem selectedResult = displayItems[lvResults.SelectedIndices[0]];
            if (selectedResult.Action == OrgAction.Queued)
                return;

            OrgItemEditor sam = new OrgItemEditor(selectedResult);
            sam.ShowDialog();

            // Apply results if valid
            if (sam.Result != null)
            {
                lock (displayItems)
                {
                    displayItems[lvResults.SelectedIndices[0]].UpdateInfo(sam.Result);
                    if (sam.Result.Action != OrgAction.None)
                        displayItems[lvResults.SelectedIndices[0]].Check = CheckState.Checked;

                    // Automatically apply the movie to unknown items with similar names
                    if (lastRunScan == ScanType.Directory && sam.Result.Movie != null && (sam.Result.Action == OrgAction.Move || sam.Result.Action == OrgAction.Copy))
                    {
                        foreach (OrgItem item in scanResults)
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
            return displayItems[lvResults.SelectedIndices[0]].Status;
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
            return displayItems[lvResults.SelectedIndices[0]].Action;
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
            lock (displayItems)
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
        /// Clicking on a listview column cause items to be sorted by the column type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvResults_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Find the column type to sort by
            sortType = 0;
            foreach (OrgColumnType colType in Enum.GetValues(typeof(OrgColumnType)))
                if (scanColumns.ContainsKey(colType) && scanColumns[colType].Header.Index == e.Column)
                {
                    sortType = colType;
                    break;
                }

            // Set order
            if (lastClickedColumn == 0 || lastClickedColumn != sortType)
                sortAcending = true;
            else
                sortAcending = !sortAcending;
            lastClickedColumn = sortType;

            // Sort items and re-display
            DisplayResults();
        }

        /// <summary>
        /// Refresh of listview is triggered when ignored filter check box is changed
        /// </summary>
        private void chkFilter_CheckedChanged(object sender, EventArgs e)
        {
            DisplayResults();
        }

        #endregion

        #region Updating

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

        private DirectoryScan directoryScan = new DirectoryScan(false);
        private TvMissingScan tvMissingScan = new TvMissingScan(false);
        private TvRenameScan tvRenameScan = new TvRenameScan(false);
        private TvFolderScan tvFolderScan = new TvFolderScan(false);
        private MovieFolderScan movieFolderScan = new MovieFolderScan(false);

        /// <summary>
        /// Work event for scanning worker.
        /// </summary>
        private void scanWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Cast arguments to object array
            object[] args = (object[])e.Argument;

            // Perform appropriate scan
            ScanType scanType = (ScanType)args[0];
            switch (scanType)
            {
                case ScanType.Directory:
                    scanResults = directoryScan.RunScan((List<OrgFolder>)args[1], queuedItems, false, false);
                    break;
                case ScanType.TvMissing:
                    scanResults = tvMissingScan.RunScan((List<Content>)args[1], queuedItems);
                    break;
                case ScanType.TvRename:
                    scanResults = tvRenameScan.RunScan((List<Content>)args[1], queuedItems);
                    break;
                case ScanType.TvFolder:
                    scanResults = tvFolderScan.RunScan((List<ContentRootFolder>)args[1], queuedItems);
                    break;
                case ScanType.MovieFolder:
                    scanResults = movieFolderScan.RunScan((List<ContentRootFolder>)args[1], queuedItems);
                    break;
                default:
                    throw new Exception("Unknown scan type!");
            }
        }

        /// <summary>
        /// Scan progress is shown in progress bar
        /// </summary>
        private void ScanHelper_ScanProgressChange(object sender, ProgressChangedEventArgs e)
        {
            // Get process
            ScanProcess process = (ScanProcess)sender;
            string info = (string)e.UserState;
            
            // Get progress bar message based on scan type
            string msg = process.Description();
            if (e.ProgressPercentage == 100)
                msg += " - Complete";
            else if (process != ScanProcess.FileCollect)
                msg += " - Processing File '" + Path.GetFileName(info) + "'";

            this.Invoke((MethodInvoker)delegate
            {
                // Set message and value
                pbScanProgress.Message = msg;
                pbScanProgress.Value = e.ProgressPercentage;
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
            else
            {
                // Display actions in listview
                lastClickedColumn = 0;
                this.Invoke((MethodInvoker)delegate
                {
                    pbScanProgress.Value = 100;
                    Application.DoEvents(); // TODO: is this needed? Seems hacky!
                    DisplayResults();
                });
            }
        }

        /// <summary>
        /// Whether to sort ascending (or descending if false)
        /// </summary>
        private bool sortAcending = true;

        /// <summary>
        /// Perform scan
        /// </summary>
        private void DoScan()
        {
            // Clear results
            lvResults.Items.Clear();
            pbScanProgress.Value = 0;
            
            sortAcending = true;
            
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
                    sortType = OrgColumnType.Source_Folder;
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
                    sortType = OrgColumnType.Show;
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
                    sortType = OrgColumnType.Source_Folder;
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
                    sortType = OrgColumnType.Source_Folder;
                    break;
                default:
                    throw new Exception("Unknown scan type");
            }

            // Start worker if not busy, otherwise cancel current scan and set rescan required flag
            if (!scanWorker.IsBusy)
            {
                currentScan = scanType;
                scanWorker.RunWorkerAsync(new object[] { scanType, list });
                lastRunScan = scanType;
            }
            else
            {
                rescanRequired = true;
                directoryScan.CancelScan();
                tvMissingScan.CancelScan();
                tvRenameScan.CancelScan();
                tvFolderScan.CancelScan();
                movieFolderScan.CancelScan();
            }

        }

        /// <summary>
        /// Displays scan results in listview
        /// </summary>
        private void DisplayResults()
        {
            // Update selection
            OrgItem selItem = null;
            int selIndex = -1;
            if (lvResults.SelectedIndices.Count > 0)
            {
                selItem = displayItems[lvResults.SelectedIndices[0]];
                selIndex = lvResults.SelectedIndices[0];
            }
            
            // Filter and sort items for display
            lock (displayItems)
            {
                displayItems = FilterResults(scanResults);
                OrgItem.AscendingSort = sortAcending;
                OrgItem.Sort(displayItems, sortType);
            }

            // De-register listview events
            lvResults.SelectedIndexChanged -= new System.EventHandler(this.lvResults_SelectedIndexChanged);
            lvResults.ItemChecked -= new System.Windows.Forms.ItemCheckedEventHandler(this.lvResults_ItemChecked);

            lock (displayItems)
                OrgItemListHelper.DisplayOrgItemInList(displayItems, lvResults, scanColumns);

            // Re-select
            bool reselected = false;
            for (int i = 0; i < displayItems.Count; i++)
                if (displayItems[i] == selItem)
                {
                    lvResults.Items[i].Selected = true;
                    lvResults.Items[i].EnsureVisible();
                    reselected = true;
                    break;
                }
            if(!reselected)
            {
                if (selIndex > lvResults.Items.Count - 1)
                    selIndex = lvResults.Items.Count - 1;
                if (selIndex >= 0)
                {
                    lvResults.Items[selIndex].Selected = true;
                    lvResults.Items[selIndex].EnsureVisible();
                }
            }

            // Re-register listview events
            lvResults.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvResults_ItemChecked);
            lvResults.SelectedIndexChanged += new System.EventHandler(this.lvResults_SelectedIndexChanged);

            if (lvResults.Items.Count > 0)
            {
                lvResults_ItemChecked(null, new ItemCheckedEventArgs(lvResults.Items[0]));
                lvResults_SelectedIndexChanged(null, new ItemCheckedEventArgs(lvResults.Items[0]));
            }
            
            // Reset check selection
            chkMoveCopy.Text = lastRunScan == ScanType.TvMissing ? "Found" : "Move/Copy";
            chkRenameDelete.Text = lastRunScan == ScanType.Directory ? "Delete" : "Organization";
            chkMoveCopy.CheckState = GetCheckState(new OrgAction[] { OrgAction.Copy, OrgAction.Move });
            chkRenameDelete.CheckState = GetCheckState(new OrgAction[] { lastRunScan == ScanType.Directory ? OrgAction.Delete : OrgAction.Rename });
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
                    default:
                        throw new Exception("Bad filter type");
                }

                switch (result.Category)
                {
                    case FileHelper.FileCategory.TvVideo:
                        if (!chkTvCatFilter.Checked)
                            continue;
                        break;
                    case FileHelper.FileCategory.NonTvVideo:
                        if (!chkNonTvCatFilter.Checked)
                            continue;
                        break;
                    case FileHelper.FileCategory.Ignored:
                        if (!chkIgnoreCatFilter.Checked)
                            continue;
                        break;
                    case FileHelper.FileCategory.Custom:
                        if (!chkCustomCatFilter.Checked)
                            continue;
                        break;
                    case FileHelper.FileCategory.Unknown:
                        if (!chkUnknownCatFilter.Checked)
                            continue;
                        break;
                    case FileHelper.FileCategory.Trash:
                        if (!chkTrashCatFilter.Checked)
                            continue;
                        break;
                    case FileHelper.FileCategory.Folder:
                        if (!chkFolderCatFilter.Checked)
                            continue;
                        break;

                }
                filteredResults.Add(result);
            }

            return filteredResults;
        }

        #endregion

        private void cmbScanType_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateScanSelection();
        }
    }
}
