using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Meticumedia.Classes;

namespace Meticumedia.Controls
{
    /// <summary>
    /// Interaction logic for ScanControl.xaml
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
        /// All scans
        /// </summary>
        private Dictionary<ScanType, Scan> scans;

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

        public ScanControl()
        {
            InitializeComponent();

            Settings.SettingsModified += Settings_SettingsModified;

            // Setup scan worker
            scanWorker = new BackgroundWorker();
            scanWorker.WorkerSupportsCancellation = true;
            scanWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(scanWorker_RunWorkerCompleted);
            scanWorker.DoWork += new DoWorkEventHandler(scanWorker_DoWork);

            // Register to scan helper progress change (for tracking progress of running scan)
            directoryScan.ItemsInitialized += directoryScan_ItemsInitialized;
            
            // Build scans list
            scans = new Dictionary<ScanType, Scan>();
            scans.Add(ScanType.Directory, directoryScan);
            scans.Add(ScanType.MovieFolder, movieFolderScan);
            scans.Add(ScanType.TvFolder, tvFolderScan);
            scans.Add(ScanType.TvMissing, tvMissingScan);
            scans.Add(ScanType.TvRename, tvRenameScan);

            // Register progress changes for scan
            foreach(Scan scan in scans.Values)
                scan.ProgressChange += scan_ProgressChange;

            // Setup context menu
            lbResults.ContextMenu = contextMenu;
            contextMenu.Opened += contextMenu_Opened;

            cmbScanType.SelectedIndex = 0;
        }



        private void contextMenu_Opened(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
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
                    return;
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

            this.Dispatcher.Invoke((Action)delegate
            {
                lbResults.ItemsSource = scanResults;
            });
        }

        /// <summary>
        /// Scan progress is shown in progress bar. Results are shown in listview as they are processed.
        /// </summary>
        private void scan_ProgressChange(object sender, ProgressChangedEventArgs e)
        {
            if (!scanRunning)
                UpdateProgress(0, string.Empty);

            // Get process
            ScanProcess process = (ScanProcess)sender;
            string info = (string)e.UserState;

            // Get progress bar message based on scan type
            string msg = process.Description();
            if (e.ProgressPercentage == 100)
                msg += " - Complete";
            else if (process != ScanProcess.FileCollect && process != ScanProcess.Directory)
                msg += " - Processing File '" + System.IO.Path.GetFileName(info) + "'";

            UpdateProgress(e.ProgressPercentage, msg);
            DisplayResults(true, false, true);
        }

        /// <summary>
        /// Scan worker complete handler
        /// </summary>
        private void scanWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //lvResults.ColumnSortEnable = false;

            // If last scan was cancelled run new scan
            if (rescanRequired)
            {
                rescanRequired = false;
                DoScan();
            }
            else if (scanCancelled)
            {
                scanCancelled = false;
                //UnlinkProgressEvents();
                //this.Invoke((MethodInvoker)delegate
                //{
                //    List<OrgItem> items = lvResults.GetOrgItems();
                //    for (int i = items.Count - 1; i >= 0; i--)
                //    {
                //        if (items[i].Action == OrgAction.Processing || items[i].Action == OrgAction.TBD)
                //            lvResults.RemoveItemAt(i);
                //    }
                //    scanRunning = false;
                //    DisplayResults(true, true, false);
                //    ScanRunEnables(true);
                //    pbScanProgress.Value = 0;
                //    pbScanProgress.Message = string.Empty;
                //});
            }
            else
            {
                // Display actions in listview
                ////lastClickedColumn = 0;
                //UnlinkProgressEvents();
                //this.Invoke((MethodInvoker)delegate
                //{
                //    pbScanProgress.Value = 100;
                //    Application.DoEvents(); // TODO: is this needed? Seems hacky!
                //    scanRunning = false;

                //    DisplayResults(true, true, false);
                //    lvResults_ItemChecked(null, null);
                //    ScanRunEnables(true);
                //});
            }
        }

        /// <summary>
        /// Performs scan
        /// </summary>
        private void DoScan()
        {
            scanRunning = true;

            // Clear results
            UpdateProgress(0, "Scan Started");

            //sortAcending = true;
            bool fast = chkFastScan.Visibility == System.Windows.Visibility.Visible && chkFastScan.IsChecked == true;

            // Build scan parameters
            ScanType scanType = (ScanType)cmbScanType.SelectedItem;
            object list;
            switch (scanType)
            {
                case ScanType.Directory:
                    // Get folders from combo box
                    List<OrgFolder> scanDirs;
                    if (cmbScanSelection.SelectedIndex <= 0)
                        scanDirs = Settings.ScanDirectories.ToList();
                    else
                    {
                        scanDirs = new List<OrgFolder>();
                        foreach (OrgFolder fld in Settings.ScanDirectories)
                            if (cmbScanSelection.SelectedItem != null && fld.FolderPath == cmbScanSelection.SelectedItem.ToString())
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
                    if (cmbScanSelection.SelectedIndex <= 0)
                        tvFolders = Settings.TvFolders.ToList();
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
                    if (cmbScanSelection.SelectedIndex <= 0)
                        movieFolders = Settings.MovieFolders.ToList();
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

            // TODO
            // Reset check selection
            //chkMoveCopy.Text = lastRunScan == ScanType.TvMissing ? "Found" : "Move/Copy";
            //chkRenameDelete.Text = lastRunScan == ScanType.Directory ? "Delete" : "Organization";
            //chkMoveCopy.CheckState = lvResults.GetCheckState(new OrgAction[] { OrgAction.Copy, OrgAction.Move });
            //chkRenameDelete.CheckState = lvResults.GetCheckState(new OrgAction[] { lastRunScan == ScanType.Directory ? OrgAction.Delete : OrgAction.Rename });
        }

        /// <summary>
        /// Cancels currently running scan.
        /// </summary>
        private void CancelScan()
        {
            scanCancelled = true;
            foreach (Scan scan in scans.Values)
                scan.CancelScan();
            UpdateProgress(0, string.Empty);
        }

        /// <summary>
        /// Displays scan results in listview
        /// </summary>
        private void DisplayResults(bool noCheckEvents, bool forceSingleCheckEvent, bool limitUpdate)
        {
            //if (noCheckEvents)
            //    lvResults.ItemChecked -= new ItemCheckedEventHandler(lvResults_ItemChecked);

            //lvResults.UpdateDisplay(limitUpdate ? 2 : 0);

            //if (forceSingleCheckEvent)
            //    lvResults_ItemChecked(null, null);

            //if (noCheckEvents)
            //    lvResults.ItemChecked += new ItemCheckedEventHandler(lvResults_ItemChecked);
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
                // Action filter
                switch (result.Action)
                {
                    case OrgAction.None:
                    case OrgAction.NoRootFolder:
                    case OrgAction.AlreadyExists:
                        if (!chkNoneFilter.IsChecked == true)
                            continue;
                        break;
                    case OrgAction.Move:
                    case OrgAction.Copy:
                        if (!chkMoveCopyFilter.IsChecked == true)
                            continue;
                        break;
                    case OrgAction.Rename:
                        if (!chkRenameFilter.IsChecked == true)
                            continue;
                        break;
                    case OrgAction.Delete:
                        if (!chkDelFilter.IsChecked == true)
                            continue;
                        break;
                    case OrgAction.Queued:
                        if (!chkQueueFilter.IsChecked == true)
                            continue;
                        break;
                    case OrgAction.TBD:
                    case OrgAction.Processing:
                        break;
                    default:
                        throw new Exception("Bad filter type");
                }

                // Category filter
                switch (result.Category)
                {
                    case FileCategory.TvVideo:
                        if (!chkTvCatFilter.IsChecked == true)
                            continue;
                        break;
                    case FileCategory.NonTvVideo:
                        if (!chkNonTvCatFilter.IsChecked == true)
                            continue;
                        break;
                    case FileCategory.Ignored:
                        if (!chkIgnoreCatFilter.IsChecked == true)
                            continue;
                        break;
                    case FileCategory.Custom:
                        if (!chkCustomCatFilter.IsChecked == true)
                            continue;
                        break;
                    case FileCategory.Unknown:
                        if (!chkUnknownCatFilter.IsChecked == true)
                            continue;
                        break;
                    case FileCategory.Trash:
                        if (!chkTrashCatFilter.IsChecked == true)
                            continue;
                        break;
                    case FileCategory.Folder:
                        if (!chkFolderCatFilter.IsChecked == true)
                            continue;
                        break;

                }
                filteredResults.Add(result);
            }

            return filteredResults;
        }

        /// <summary>
        /// Updates progress bar percent and message
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="msg"></param>
        private void UpdateProgress(int percent, string msg)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                tbPbText.Text = msg;
                pbUpdating.Value = percent;

            });
        }

        /// <summary>
        /// Update scan selection combo box based on scan type that is selected.
        /// </summary>
        public void UpdateScanSelection()
        {
            // Save current selection
            string currSelection = string.Empty;
            if (cmbScanSelection.SelectedItem != null)
                currSelection = cmbScanSelection.SelectedItem.ToString();

            if (cmbScanType.SelectedItem == null)
                return;

            // Rebuild combo box items
            cmbScanSelection.Items.Clear();
            bool selected = false;
            ScanType scanType = (ScanType)cmbScanType.SelectedItem;
            chkFastScan.Visibility = System.Windows.Visibility.Visible;
            switch (scanType)
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
                    chkFastScan.Visibility = System.Windows.Visibility.Hidden;
                    break;
            }

            // Select first item if previous selection was not set
            if (!selected)
                cmbScanSelection.SelectedIndex = 0;
        }

        #endregion

        void directoryScan_ItemsInitialized(object sender, Scan.ItemsInitializedArgs e)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                lbResults.ItemsSource = directoryScan.Items;
            });
        }

        void Settings_SettingsModified(object sender, EventArgs e)
        {
            UpdateScanSelection();
        }

        private void cmbScanType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateScanSelection();
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            DoScan();
        }
    }
}
