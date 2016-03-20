using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Meticumedia.Classes;
using Meticumedia.Windows;
using Meticumedia.WPF;

namespace Meticumedia.Controls
{
    public class ScanControlViewModel : OrgItemDisplayViewModel
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
        private List<OrgItem> queuedItems = new List<OrgItem>();

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
        /// Indicated scan is currently running
        /// </summary>
        private bool scanRunning = false;

        /// <summary>
        /// Flag that scan has been cancelled
        /// </summary>
        private bool scanCancelled = false;        

        #endregion

        #region Commands

        private ICommand runCommand;
        public ICommand RunCommand
        {
            get
            {
                if (runCommand == null)
                {
                    runCommand = new RelayCommand(
                        param => this.Run(),
                        param => this.CanDoRunCommand()
                    );
                }
                return runCommand;
            }
        }

        private bool CanDoRunCommand()
        {
            return true;
        }

        private ICommand queueItemsCommand;
        public ICommand QueueItemsCommand
        {
            get
            {
                if (queueItemsCommand == null)
                {
                    queueItemsCommand = new RelayCommand(
                        param => this.QueueItems(),
                        param => this.CanDoQueueItemsCommand()
                    );
                }
                return queueItemsCommand;
            }
        }

        private bool CanDoQueueItemsCommand()
        {
            return true;
        }

        private ICommand editSelectedItemCommand;
        public ICommand EditSelectedItemCommand
        {
            get
            {
                if (editSelectedItemCommand == null)
                {
                    editSelectedItemCommand = new RelayCommand(
                        param => this.EditSelectedItem(),
                        param => this.CanDoEditSelectedItemCommand()
                    );
                }
                return editSelectedItemCommand;
            }
        }

        private bool CanDoEditSelectedItemCommand()
        {
            return this.SelectedOrgItems != null && this.SelectedOrgItems.Count == 1;
        }

        private ICommand setSelectedItemToDeleteCommand;
        public ICommand SetSelectedItemToDeleteCommand
        {
            get
            {
                if (setSelectedItemToDeleteCommand == null)
                {
                    setSelectedItemToDeleteCommand = new RelayCommand(
                        param => this.SetSelectedItemToDelete()
                    );
                }
                return setSelectedItemToDeleteCommand;
            }
        }

        private ICommand ignoreItemsCommand;
        public ICommand IgnoreItemsCommand
        {
            get
            {
                if (ignoreItemsCommand == null)
                {
                    ignoreItemsCommand = new RelayCommand(
                        param => this.IgnoreItems()
                    );
                }
                return ignoreItemsCommand;
            }
        }

        private ICommand unignoreItemsCommand;
        public ICommand UnignoreItemsCommand
        {
            get
            {
                if (unignoreItemsCommand == null)
                {
                    unignoreItemsCommand = new RelayCommand(
                        param => this.UnignoreItems()
                    );
                }
                return unignoreItemsCommand;
            }
        }

        private ICommand locateEpisodeCommand;
        public ICommand LocateEpisodeCommand
        {
            get
            {
                if (locateEpisodeCommand == null)
                {
                    locateEpisodeCommand = new RelayCommand(
                        param => this.LocateEpisode()
                    );
                }
                return locateEpisodeCommand;
            }
        }

        private ICommand ignoreEpisodeCommand;
        public ICommand IgnoreEpisodeCommand
        {
            get
            {
                if (ignoreEpisodeCommand == null)
                {
                    ignoreEpisodeCommand = new RelayCommand(
                        param => this.IgnoreEpisode()
                    );
                }
                return ignoreEpisodeCommand;
            }
        }

        private ICommand ignoreSeasonCommand;
        public ICommand IgnoreSeasonCommand
        {
            get
            {
                if (ignoreSeasonCommand == null)
                {
                    ignoreSeasonCommand = new RelayCommand(
                        param => this.IgnoreSeason()
                    );
                }
                return ignoreSeasonCommand;
            }
        }

        private ICommand ignoreShowCommand;
        public ICommand IgnoreShowCommand
        {
            get
            {
                if (ignoreShowCommand == null)
                {
                    ignoreShowCommand = new RelayCommand(
                        param => this.IgnoreShow()
                    );
                }
                return ignoreShowCommand;
            }
        }

        private ICommand setReplaceExistingCommand;
        public ICommand SetReplaceExistingCommand
        {
            get
            {
                if (setReplaceExistingCommand == null)
                {
                    setReplaceExistingCommand = new RelayCommand(
                        param => this.SetReplaceExisting()
                    );
                }
                return setReplaceExistingCommand;
            }
        }
        
        #endregion

        #region Properties

        #region General

        public string RunButtonText
        {
            get
            {
                return runButtonText;
            }
            set
            {
                runButtonText = value;
                OnPropertyChanged(this, "RunButtonText");
            }
        }
        private string runButtonText = "Run";

        public bool Fast
        {
            get
            {
                return fast;
            }
            set
            {
                fast = value;
                OnPropertyChanged(this, "Fast");
            }
        }
        private bool fast = false;

        public bool ReuseResults
        {
            get
            {
                return reuseResults;
            }
            set
            {
                reuseResults = value;
                OnPropertyChanged(this, "ReuseResults");
            }
        }
        private bool reuseResults = true;

        public ScanType RunType
        {
            get
            {
                return runType;
            }
            set
            {
                runType = value;
                UpdateRunSelectionsSafe();
                OnPropertyChanged(this, "RunType");
            }
        }
        private ScanType runType = ScanType.Directory;

        public ObservableCollection<object> RunSelections
        {
            get;
            private set;
        }

        public object SelectedRunSelection
        {
            get
            {
                return selectedRunSelection;
            }
            set
            {
                selectedRunSelection = value;

                OnPropertyChanged(this, "SelectedRunSelection");
            }
        }
        private object selectedRunSelection = new object();

        public bool MoveCopyEnables
        {
            get
            {
                return moveCopyEnables;
            }
            set
            {
                moveCopyEnables = value;
                SetEnables(OrgAction.Move | OrgAction.Copy, value);
                OnPropertyChanged(this, "MoveCopyEnables");
            }
        }
        private bool moveCopyEnables = true;

        public bool DeleteEnables
        {
            get
            {
                return deleteEnables;
            }
            set
            {
                deleteEnables = value;
                SetEnables(OrgAction.Delete, value);
                OnPropertyChanged(this, "DeleteEnables");
            }
        }
        private bool deleteEnables = true;

        public ObservableCollection<DataGridColumn> GridColumns { get; set; }

        public bool ListView
        {
            get
            {
                return Settings.GuiControl.ScanListView;
            }
            set
            {
                Settings.GuiControl.ScanListView = value;
                Settings.Save(false);
                OnPropertyChanged(this, "ListView");
                OnPropertyChanged(this, "GridView");
            }
        }

        public bool GridView
        {
            get
            {
                return !Settings.GuiControl.ScanListView;
            }
        }

        #endregion

        #region Context Menu Related

        public ObservableCollection<MenuItem> MovieFolderItems { get; set; }

        public Visibility SingleDirScanSelectionVisibility
        {
            get
            {
                return this.lastRunScan == ScanType.Directory && this.SelectedOrgItems != null && this.SelectedOrgItems.Count == 1 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility AnySelectionVisibility
        {
            get
            {
                return this.SelectedOrgItems != null && this.SelectedOrgItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility DirectoryScanAnySelectedResultsVisibility
        {
            get
            {
                return this.lastRunScan == ScanType.Directory && this.SelectedOrgItems != null && this.SelectedOrgItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility TvMissingScanAnySelectedResultsVisibility
        {
            get
            {
                return this.lastRunScan == ScanType.TvMissing && this.SelectedOrgItems != null && this.SelectedOrgItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility TvMissingScanSingleMissingSelectedResultsVisibility
        {
            get
            {
                return this.lastRunScan == ScanType.TvMissing && this.SelectedOrgItems != null && this.SelectedOrgItems.Count == 1 && (this.SelectedOrgItems[0] as OrgItem).Status == OrgStatus.Missing ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility AllMoviesSelectedResultsVisibility
        {
            get
            {                
                if (this.SelectedOrgItems == null || this.SelectedOrgItems.Count == 0)
                    return Visibility.Collapsed;

                foreach (object obj in this.SelectedOrgItems)
                {
                    OrgItem item = obj as OrgItem;
                    if (item.Category != FileCategory.MovieVideo || (item.Action != OrgAction.Move && item.Action != OrgAction.Copy))
                        return Visibility.Collapsed;
                }

                return Visibility.Visible;
            }
        }

        public Visibility AllAlreadyExistSelectedResultsVisibility
        {
            get
            {
                if (this.SelectedOrgItems == null || this.SelectedOrgItems.Count == 0)
                    return Visibility.Collapsed;

                foreach (object obj in this.SelectedOrgItems)
                {
                    OrgItem item = obj as OrgItem;
                    if (item.Action != OrgAction.AlreadyExists)
                        return Visibility.Collapsed;
                }

                return Visibility.Visible;
            }
        }
        
        #endregion

        #endregion

        #region Constructor

        public ScanControlViewModel() : base()
        {
            this.GridColumns = new ObservableCollection<DataGridColumn>();

            this.RunSelections = new ObservableCollection<object>();
            UpdateRunSelections();

            // Setup scan worker
            scanWorker = new BackgroundWorker();
            scanWorker.WorkerSupportsCancellation = true;
            scanWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(scanWorker_RunWorkerCompleted);
            scanWorker.DoWork += new DoWorkEventHandler(scanWorker_DoWork);

            // Register settings modification
            Settings.SettingsModified += Settings_SettingsModified;

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
            foreach (Scan scan in scans.Values)
                scan.ProgressChange += scan_ProgressChange;

            QueueControlViewModel.QueueItemsChanged += QueueControlViewModel_QueueItemsChanged;

            this.MovieFolderItems = new ObservableCollection<MenuItem>();
        }

        #endregion

        #region Event Handlers

        protected override void SelectedOrgItemsChange()
        {
            OnPropertyChanged(this, "SingleDirScanSelectionVisibility");
            OnPropertyChanged(this, "AnySelectionVisibility");
            OnPropertyChanged(this, "DirectoryScanAnySelectedResultsVisibility");
            OnPropertyChanged(this, "TvMissingScanAnySelectedResultsVisibility");
            OnPropertyChanged(this, "TvMissingScanSingleMissingSelectedResultsVisibility");
            OnPropertyChanged(this, "AllMoviesSelectedResultsVisibility");
            OnPropertyChanged(this, "AllAlreadyExistSelectedResultsVisibility");

            UpdateMovieFolderItems();
        }

        void QueueControlViewModel_QueueItemsChanged(object sender, QueueControlViewModel.QueueItemsChangedArgs e)
        {
            this.queuedItems = e.QueueItems;
        }

        void Settings_SettingsModified(object sender, EventArgs e)
        {
            OnPropertyChanged(this, "ListView");
            OnPropertyChanged(this, "GridView");
            UpdateRunSelectionsSafe();
            UpdateMovieFolderItems();
        }

        void directoryScan_ItemsInitialized(object sender, Scan.ItemsInitializedArgs e)
        {
            if (App.Current.Dispatcher.CheckAccess())
            {
                this.OrgItems.Clear();
                foreach (OrgItem item in e.Items)
                    this.OrgItems.Add(item);
            }
            else
                App.Current.Dispatcher.BeginInvoke((Action)delegate
                {
                    this.OrgItems.Clear();
                    foreach (OrgItem item in e.Items)
                        this.OrgItems.Add(item);
                });
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
                    directoryScan.RunScan((List<OrgFolder>)args[1], queuedItems, false, false, (bool)args[2], (bool)args[3]);
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

            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                foreach (OrgItem item in scanResults)
                    this.OrgItems.Add(item);
            });

        }

        /// <summary>
        /// Scan progress is shown in progress bar. Results are shown in listview as they are processed.
        /// </summary>
        private void scan_ProgressChange(object sender, ProgressChangedEventArgs e)
        {
            if (!scanRunning)
            {
                this.Progress = 0;
                this.ProgressMessage = string.Empty;
                return;
            }

            // Get process
            ScanProcess process = (ScanProcess)sender;
            string info = (string)e.UserState;

            // Get progress bar message based on scan type
            string msg = process.Description();
            if (e.ProgressPercentage == 100)
                msg += " - Complete";
            else if (process != ScanProcess.FileCollect && process != ScanProcess.Directory)
                msg += " - Processing File '" + System.IO.Path.GetFileName(info) + "'";

            UpdateProgressSafe(e.ProgressPercentage, msg);
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
                Run();
            }
            else
            {
                if (scanCancelled)
                    scanCancelled = false;
                RefreshResultsSafe(false);
                scanRunning = false;

                App.Current.Dispatcher.BeginInvoke((Action)delegate
                {
                    this.RunButtonText = "Run";
                });
            }
        }

        /// <summary>
        /// Performs scan
        /// </summary>
        private void Run()
        {
            if (!scanRunning)
            {
                this.RunButtonText = "Stop";
                this.OrgItems.Clear();
            }
            else
            {
                CancelScan();
                return;
            }
            
            scanRunning = true;

            // Clear results
            this.Progress = 0;
            this.ProgressMessage = "Scan Started";

            SetupColumns();

            // Build scan parameters
            object list;
            switch (this.RunType)
            {
                case ScanType.Directory:
                    // Get folders from combo box
                    List<OrgFolder> scanDirs;
                    if (this.SelectedRunSelection == OrgFolder.AllFolders)
                        scanDirs = Settings.ScanDirectories.ToList();
                    else
                    {
                        scanDirs = new List<OrgFolder>() { (OrgFolder)this.SelectedRunSelection };
                    }
                    list = scanDirs;
                    break;

                case ScanType.TvRename:
                case ScanType.TvMissing:
                    List<Content> shows;
                    if (this.SelectedRunSelection == TvShow.AllShows)
                        shows = Organization.Shows.GetScannableContent(true, this.RunType);
                    else
                    {
                        shows = new List<Content>();
                        shows.Add((TvShow)this.SelectedRunSelection);
                    }
                    list = shows;
                    break;
                case ScanType.TvFolder:
                    List<ContentRootFolder> tvFolders;
                    if (this.SelectedRunSelection == ContentRootFolder.AllTvFolders)
                        tvFolders = Settings.TvFolders.ToList();
                    else
                        tvFolders = new List<ContentRootFolder>() { (ContentRootFolder)this.SelectedRunSelection };
                    list = tvFolders;
                    break;
                case ScanType.MovieFolder:
                    List<ContentRootFolder> movieFolders;
                    if (this.SelectedRunSelection == ContentRootFolder.AllMoviesFolders)
                        movieFolders = Settings.MovieFolders.ToList();
                    else
                        movieFolders = new List<ContentRootFolder>() { (ContentRootFolder)this.SelectedRunSelection };
                    list = movieFolders;
                    break;
                default:
                    throw new Exception("Unknown scan type");
            }

            // Start worker if not busy, otherwise cancel current scan and set rescan required flag
            if (!scanWorker.IsBusy)
            {
                currentScan = this.RunType;
                scanWorker.RunWorkerAsync(new object[] { currentScan, list, this.Fast, this.ReuseResults });
                lastRunScan = currentScan;
            }
            else
            {
                rescanRequired = true;
                CancelScan();
            }
        }

        /// <summary>
        /// Cancels currently running scan.
        /// </summary>
        private void CancelScan()
        {
            scanCancelled = true;
            foreach (Scan scan in scans.Values)
                scan.CancelScan();

            this.Progress = 0;
            this.ProgressMessage = string.Empty;

            for (int i = OrgItems.Count - 1; i >= 0; i--)
                if (OrgItems[i].Action == OrgAction.TBD || OrgItems[i].Action == OrgAction.Processing)
                    OrgItems.RemoveAt(i);
        }


        private void UpdateRunSelectionsSafe()
        {
            if (App.Current.Dispatcher.CheckAccess())
                UpdateRunSelections();
            else
                App.Current.Dispatcher.BeginInvoke((Action)delegate
                {
                    UpdateRunSelections();
                });
        }

        /// <summary>
        /// Update scan selection combo box based on scan type that is selected.
        /// </summary>
        private void UpdateRunSelections()
        {
            this.RunSelections.Clear();
            switch (runType)
            {
                case ScanType.Directory:
                    this.RunSelections.Add(OrgFolder.AllFolders);
                    foreach (OrgFolder folder in Settings.ScanDirectories)
                        this.RunSelections.Add(folder);
                    break;
                case ScanType.TvMissing:
                case ScanType.TvRename:
                    this.RunSelections.Add(TvShow.AllShows);
                    foreach (TvShow show in Organization.Shows)
                        this.RunSelections.Add(show);
                    break;
                case ScanType.TvFolder:
                    this.RunSelections.Add(ContentRootFolder.AllTvFolders);
                    foreach (ContentRootFolder folder in Settings.TvFolders)
                        this.RunSelections.Add(folder);
                    break;
                case ScanType.MovieFolder:
                    this.RunSelections.Add(ContentRootFolder.AllMoviesFolders);
                    foreach (ContentRootFolder folder in Settings.MovieFolders)
                        this.RunSelections.Add(folder);
                    break;
                default:
                    throw new Exception("Unknown scan type");
            }

            if (!this.RunSelections.Contains(this.SelectedRunSelection))
                this.SelectedRunSelection = this.RunSelections[0];
            
        }

        #endregion

        #region Methods

        private void UpdateMovieFolderItems()
        {
            this.MovieFolderItems.Clear();
            foreach (ContentRootFolder folder in Settings.MovieFolders)
                UpdateMovieFolderItem(folder);
        }

        private void UpdateMovieFolderItem(ContentRootFolder folder)
        {
            MenuItem item = new MenuItem();
            item.Header = folder.FullPath;
            item.Command = new RelayCommand(param => this.SetMovieFolder(folder.FullPath));
            this.MovieFolderItems.Add(item);

            foreach (ContentRootFolder subFolder in folder.ChildFolders)
                UpdateMovieFolderItem(subFolder);
        }

        private void QueueItems()
        {
            List<OrgItem> toQueue = new List<OrgItem>();
            for (int i = 0; i < this.OrgItems.Count; i++)
            {
                if(!FilterItem(this.OrgItems[i]) || !this.OrgItems[i].Enable)
                    continue;
                toQueue.Add(new OrgItem(this.OrgItems[i]));

                this.OrgItems[i].Action = OrgAction.Queued;
                this.OrgItems[i].Enable = false;
            }
            OnItemsToQueue(toQueue);
        }

        private void SetEnables(OrgAction action, bool enable)
        {
            for (int i = 0; i < this.OrgItems.Count; i++)
                if ((this.OrgItems[i].Action & action) > 0)
                {
                    this.OrgItems[i].Enable = enable;
                }
            RefreshResultsSafe(false);
        }

        private void EditSelectedItem()
        {
            OrgItem selItem = (this.SelectedOrgItems[0] as OrgItem);
            OrgItemEditorWindow editor = new OrgItemEditorWindow(selItem);
            editor.ShowDialog();

            if (editor.Results != null)
                selItem.Clone(editor.Results);
            else
                return;

            if (selItem.Action != OrgAction.None)
                selItem.Enable = true;
            
            // Automatically apply the movie to unknown items with similar names
            if (lastRunScan == ScanType.Directory && selItem.Category == FileCategory.MovieVideo && (selItem.Action == OrgAction.Move || selItem.Action == OrgAction.Copy))
            {
                foreach (OrgItem item in this.OrgItems)
                {
                    if(FileHelper.PathsVerySimilar(item.SourcePath, selItem.SourcePath))
                    {
                        item.Movie = selItem.Movie;
                        item.DestinationPath = item.Movie.BuildFilePath(item.SourcePath);
                        item.Action = selItem.Action;
                        item.Enable = true;
                    }
                }
            }
        }

        private void SetSelectedItemToDelete()
        {
            foreach (OrgItem item in this.SelectedOrgItems)
            {
                item.Replace = true;
                item.Action = OrgAction.Delete;
                item.Category = FileCategory.Trash;
                item.Enable = true;
                item.BuildDestination();
            }
        }

        private void IgnoreItems()
        {
            foreach (OrgItem item in this.SelectedOrgItems)
            {
                item.Enable = false;
                item.Action = OrgAction.None;
                item.Category = FileCategory.Ignored;

                foreach (OrgFolder scanDir in Settings.ScanDirectories)
                    if (scanDir.FolderPath == item.ScanDirectory.FolderPath)
                        scanDir.AddIgnoreFile(item.SourcePath);
            }
            Settings.Save();
        }

        private void UnignoreItems()
        {
            foreach (OrgItem item in this.SelectedOrgItems)
            {
                foreach (OrgFolder scanDir in Settings.ScanDirectories)
                    if (scanDir.FolderPath == item.ScanDirectory.FolderPath)
                        if (scanDir.RemoveIgnoreFile(item.SourcePath))
                            item.Category = FileCategory.Unknown;
            }
            Settings.Save();
        }

        private void LocateEpisode()
        {
            foreach (OrgItem item in this.SelectedOrgItems)
            {
                List<OrgItem> items;
                if (item.TvEpisode.UserLocate(false, false, out items))
                {
                    item.Clone(items[0]);
                    item.Enable = true;
                }
            }
        }

        private void IgnoreEpisode()
        {
            foreach (OrgItem item in this.SelectedOrgItems)
            {
                item.Enable = false;
                item.TvEpisode.Ignored = true;
                item.Category = FileCategory.Ignored;
            }
            Organization.Shows.Save();
        }

        private void IgnoreSeason()
        {
            // Get show seasons to ignore
            List<Tuple<TvShow, int>> setShowSeasons = new List<Tuple<TvShow, int>>();
            foreach (OrgItem item in this.SelectedOrgItems)
            {
                TvShow show = item.TvEpisode.Show;
                int season = item.TvEpisode.Season;
                
                Tuple<TvShow, int> showSeason = new Tuple<TvShow, int>(show, season);
                if (!setShowSeasons.Contains(showSeason))
                {
                    setShowSeasons.Add(showSeason);
                    foreach (TvEpisode ep in show.Episodes)
                        if (ep.Season == season)
                            ep.Ignored = true;
                }
            }

            for(int i=this.OrgItems.Count-1;i>=0;i--)
            {
                TvShow show = this.OrgItems[i].TvEpisode.Show;
                int season = this.OrgItems[i].TvEpisode.Season;

                if(setShowSeasons.Contains(new Tuple<TvShow,int>(show, season)))
                    this.OrgItems.RemoveAt(i);
            }
            Organization.Shows.Save();
        }

        private void IgnoreShow()
        {
            List<TvShow> shows = new List<TvShow>();
            foreach (OrgItem item in this.SelectedOrgItems)
            {
                TvShow show = item.TvEpisode.Show;
                if (!shows.Contains(show))
                {
                    shows.Add(show);
                    show.DoRenaming = false;
                }
            }
        }

        private void SetReplaceExisting()
        {
            foreach (OrgItem item in this.SelectedOrgItems)
            {
                foreach (OrgFolder dir in Settings.ScanDirectories)
                    if (dir.FolderPath == item.ScanDirectory.FolderPath)
                    {
                        // Set action based on scan directory
                        item.Action = dir.CopyFrom ? OrgAction.Copy : OrgAction.Move;
                        item.Replace = true;
                        item.Enable = true;
                        break;
                    }
            }
        }

        private void SetMovieFolder(string path)
        {
            foreach (OrgItem item in this.SelectedOrgItems)
            {
                item.Movie.RootFolder = path;
                item.BuildDestination();

                foreach (OrgItem otherItem in this.OrgItems)
                    if (otherItem.Movie.DatabaseName == item.Movie.DatabaseName)
                    {
                        otherItem.Movie.RootFolder = path;
                        otherItem.BuildDestination();
                    }
            }
        }

        #endregion

        #region Columns

        private void SetupColumns()
        {
            this.GridColumns.Clear();

            EnumDescriptionConverter enumConv = new EnumDescriptionConverter();

            AddCheckBoxColumn("Enable", "Enable", "CanEnable");
            AddTextColumn("Action", "Action", enumConv);
            AddTextColumn("Source Folder", "SourcePathDirectory");
            AddTextColumn("Source File", "SourcePathFileName");            
            if (this.RunType == ScanType.Directory || this.RunType == ScanType.TvMissing)
                AddTextColumn("Category", "Category", enumConv);

            if (this.RunType == ScanType.TvMissing)
            {
                AddTextColumn("Status", "Status");
                AddTextColumn("Show", "TvEpisode.Show.DisplayName");
                AddTextColumn("Season", "TvEpisode.Season");
                AddTextColumn("Episode", "TvEpisode.DisplayNumber");
            }

            AddTextColumn("Destination Folder", "DestinationPathDirectory");
            AddTextColumn("Destination File", "DestinationPathFileName");
        }

        private void AddTextColumn(string header, string bindingPath, IValueConverter converter = null, string format = "")
        {
            // Create column
            DataGridTextColumn col = new DataGridTextColumn();
            col.IsReadOnly = true;

            col.Header = header;
            col.MinWidth = 60;


            // Setup binding
            Binding binding = new Binding(bindingPath)
            {
                Mode = BindingMode.OneWay,
            };
            if (!string.IsNullOrEmpty(format))
                binding.StringFormat = format;
            if (converter != null)
                binding.Converter = converter;

            col.Binding = binding;
            this.GridColumns.Add(col);
        }

        private void AddCheckBoxColumn(string header, string bindingPath, string enableBindingPath = "", IValueConverter converter = null)
        {
            // Create column
            DataGridTemplateColumn col = new DataGridTemplateColumn();
            col.Header = header;
            col.MinWidth = 60;

            FrameworkElementFactory checkFactory = new FrameworkElementFactory(typeof(CheckBox));
            Binding binding = new Binding(bindingPath)
            {
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            Binding enableBinding = new Binding(enableBindingPath);
            if (converter != null)
                binding.Converter = converter;
            checkFactory.SetValue(CheckBox.IsCheckedProperty, binding);
            checkFactory.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            checkFactory.SetValue(CheckBox.IsEnabledProperty, enableBinding);


            DataTemplate cellTemp = new DataTemplate();
            cellTemp.VisualTree = checkFactory;
            col.CellTemplate = cellTemp;

            this.GridColumns.Add(col);
        }

        #endregion
    }
}
