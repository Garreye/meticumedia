using System;
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
    public class ScanControlViewModel : ProgressViewModel
    {
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
        /// Indicated scan is currently running
        /// </summary>
        private bool scanRunning = false;

        /// <summary>
        /// Flag that scan has been cancelled
        /// </summary>
        private bool scanCancelled = false;

        private DataGrid grid;

        private DateTime lastRefresh = DateTime.Now;

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
            return this.SelectedResultItem != null;
        }

        #endregion

        #region Properties

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

        public ObservableCollection<OrgItem> ScanResults
        {
            get;
            private set;
        }

        public ICollectionView ScanResultsCollection { get; set; }

        #region Action Filter

        public bool NoneActionFilter
        {
            get
            {
                return (int)(actionFilter & OrgAction.None) > 0;
            }
            set
            {
                if (value)
                    this.ActionFilter = actionFilter | OrgAction.None;
                else
                    this.ActionFilter = actionFilter & ~OrgAction.None;
            }
        }

        public bool MoveCopyActionFilter
        {
            get
            {
                return (int)(actionFilter & (OrgAction.Move | OrgAction.Copy)) > 0;
            }
            set
            {
                if (value)
                    this.ActionFilter = actionFilter | (OrgAction.Move | OrgAction.Copy);
                else
                    this.ActionFilter = actionFilter & ~(OrgAction.Move | OrgAction.Copy);
            }
        }

        public bool RenameActionFilter
        {
            get
            {
                return (int)(actionFilter & OrgAction.Rename) > 0;
            }
            set
            {
                if (value)
                    this.ActionFilter = actionFilter | OrgAction.Rename;
                else
                    this.ActionFilter = actionFilter & ~OrgAction.Rename;
            }
        }

        public bool DeleteActionFilter
        {
            get
            {
                return (int)(actionFilter & OrgAction.Delete) > 0;
            }
            set
            {
                if (value)
                    this.ActionFilter = actionFilter | OrgAction.Delete;
                else
                    this.ActionFilter = actionFilter & ~OrgAction.Delete;
            }
        }

        public bool QueuedActionFilter
        {
            get
            {
                return (int)(actionFilter & OrgAction.Queued) > 0;
            }
            set
            {
                if (value)
                    this.ActionFilter = actionFilter | OrgAction.Queued;
                else
                    this.ActionFilter = actionFilter & ~OrgAction.Queued;
            }
        }

        public bool TbdActionFilter
        {
            get
            {
                return (int)(actionFilter & OrgAction.TBD) > 0;
            }
            set
            {
                if (value)
                    this.ActionFilter = actionFilter | OrgAction.TBD | OrgAction.Processing;
                else
                    this.ActionFilter = actionFilter & ~(OrgAction.TBD | OrgAction.Processing);
            }
        }

        public OrgAction ActionFilter
        {
            get
            {
                return actionFilter;
            }
            set
            {
                actionFilter = value;
                OnPropertyChanged(this, "ActionFilter");
                OnPropertyChanged(this, "NoneActionFilter");
                OnPropertyChanged(this, "MoveCopyActionFilter");
                OnPropertyChanged(this, "RenameActionFilter");
                OnPropertyChanged(this, "DeleteActionFilter");
                OnPropertyChanged(this, "QueuedActionFilter");
                OnPropertyChanged(this, "TbdActionFilter");
                RefreshResultsSafe(false);
            }
        }
        private OrgAction actionFilter = OrgAction.All;


        #endregion

        #region Category Filter

        public bool TvVideoCategoryFilter
        {
            get
            {
                return (int)(categoryFilter & FileCategory.TvVideo) > 0;
            }
            set
            {
                if (value)
                    this.CategoryFilter = categoryFilter | FileCategory.TvVideo;
                else
                    this.CategoryFilter = categoryFilter & ~FileCategory.TvVideo;
            }
        }

        public bool MovieVideoCategoryFilter
        {
            get
            {
                return (int)(categoryFilter & FileCategory.MovieVideo) > 0;
            }
            set
            {
                if (value)
                    this.CategoryFilter = categoryFilter | FileCategory.MovieVideo;
                else
                    this.CategoryFilter = categoryFilter & ~FileCategory.MovieVideo;
            }
        }

        public bool CustomCategoryFilter
        {
            get
            {
                return (int)(categoryFilter & FileCategory.Custom) > 0;
            }
            set
            {
                if (value)
                    this.CategoryFilter = categoryFilter | FileCategory.Custom;
                else
                    this.CategoryFilter = categoryFilter & ~FileCategory.Custom;
            }
        }

        public bool TrashCategoryFilter
        {
            get
            {
                return (int)(categoryFilter & FileCategory.Trash) > 0;
            }
            set
            {
                if (value)
                    this.CategoryFilter = categoryFilter | FileCategory.Trash;
                else
                    this.CategoryFilter = categoryFilter & ~FileCategory.Trash;
            }
        }

        public bool UnknownCategoryFilter
        {
            get
            {
                return (int)(categoryFilter & FileCategory.Unknown) > 0;
            }
            set
            {
                if (value)
                    this.CategoryFilter = categoryFilter | FileCategory.Unknown;
                else
                    this.CategoryFilter = categoryFilter & ~FileCategory.Unknown;
            }
        }

        public bool FolderCategoryFilter
        {
            get
            {
                return (int)(categoryFilter & FileCategory.Folder) > 0;
            }
            set
            {
                if (value)
                    this.CategoryFilter = categoryFilter | FileCategory.Folder;
                else
                    this.CategoryFilter = categoryFilter & ~FileCategory.Folder;
            }
        }

        public bool IgnoredCategoryFilter
        {
            get
            {
                return (int)(categoryFilter & FileCategory.Ignored) > 0;
            }
            set
            {
                if (value)
                    this.CategoryFilter = categoryFilter | FileCategory.Ignored;
                else
                    this.CategoryFilter = categoryFilter & ~FileCategory.Ignored;
            }
        }

        public FileCategory CategoryFilter
        {
            get
            {
                return categoryFilter;
            }
            set
            {
                categoryFilter = value;
                OnPropertyChanged(this, "CategoryFilter");
                OnPropertyChanged(this, "TvVideoCategoryFilter");
                OnPropertyChanged(this, "MovieVideoCategoryFilter");
                OnPropertyChanged(this, "CustomCategoryFilter");
                OnPropertyChanged(this, "TrashCategoryFilter");
                OnPropertyChanged(this, "UnknownCategoryFilter");
                OnPropertyChanged(this, "FolderCategoryFilter");
                OnPropertyChanged(this, "IgnoredCategoryFilter");
                RefreshResultsSafe(false);
            }
        }
        private FileCategory categoryFilter = FileCategory.All;

        #endregion

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

        public OrgItem SelectedResultItem
        {
            get
            {
                return selectedResultItem;
            }
            set
            {
                selectedResultItem = value;
                OnPropertyChanged(this, "SelectedResultItem");
            }
        }
        private OrgItem selectedResultItem;

        #endregion

        #region Constructor

        public ScanControlViewModel(DataGrid grid)
        {
            this.grid = grid;
            
            this.RunSelections = new ObservableCollection<object>();
            UpdateRunSelections();

            this.ScanResults = new ObservableCollection<OrgItem>();
            CollectionViewSource scanResultsViewSource = new CollectionViewSource() { Source = ScanResults };
            this.ScanResultsCollection = scanResultsViewSource.View;
            this.ScanResultsCollection.Filter = new Predicate<object>(FilterItem);
            
            // Set properties to trigger live updating
            ICollectionViewLiveShaping liveCollection = this.ScanResultsCollection as ICollectionViewLiveShaping;
            liveCollection.LiveFilteringProperties.Add("Category");
            liveCollection.LiveFilteringProperties.Add("Action");
            liveCollection.IsLiveFiltering = true;

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
        }



        #endregion

        #region Event Handlers

        void QueueControlViewModel_QueueItemsChanged(object sender, QueueControlViewModel.QueueItemsChangedArgs e)
        {
            this.queuedItems = e.QueueItems;
        }

        void Settings_SettingsModified(object sender, EventArgs e)
        {
            UpdateRunSelectionsSafe();
        }

        void directoryScan_ItemsInitialized(object sender, Scan.ItemsInitializedArgs e)
        {
            if (App.Current.Dispatcher.CheckAccess())
            {
                this.ScanResults.Clear();
                foreach (OrgItem item in e.Items)
                    this.ScanResults.Add(item);
            }
            else
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    this.ScanResults.Clear();
                    foreach (OrgItem item in e.Items)
                        this.ScanResults.Add(item);
                });
        }


        private bool FilterItem(object obj)
        {
            OrgItem item = obj as OrgItem;
            
            // Action filter
            if ((int)(item.Action & this.ActionFilter) == 0)
                return false;

            // Category filter
            if ((int)(item.Category & this.CategoryFilter) == 0)
                return false;

             return true;
        }

        #endregion

        #region Scanning

        private void RefreshResultsSafe(bool limitRate)
        {
            if (limitRate && (DateTime.Now - lastRefresh).TotalSeconds < 5)
                return;

            lastRefresh = DateTime.Now;

            try
            {
                if (App.Current.Dispatcher.CheckAccess())
                {
                    grid.CancelEdit();
                    this.ScanResultsCollection.Refresh();

                }
                else
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        this.ScanResultsCollection.Refresh();
                    });
            }
            catch { }
        }

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

            this.ScanResults.Clear();
            foreach (OrgItem item in scanResults)
                this.ScanResults.Add(item);
            OnPropertyChanged(this, "ScanResults");

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
            }
        }

        /// <summary>
        /// Performs scan
        /// </summary>
        private void Run()
        {
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
                        shows = Organization.Shows.GetScannableContent(true);
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
                scanWorker.RunWorkerAsync(new object[] { currentScan, list, fast });
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
        }


        private void UpdateRunSelectionsSafe()
        {
            if (App.Current.Dispatcher.CheckAccess())
                UpdateRunSelections();
            else
                App.Current.Dispatcher.Invoke((Action)delegate
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

        private void QueueItems()
        {
            List<OrgItem> toQueue = new List<OrgItem>();
            for (int i = 0; i < this.ScanResults.Count; i++)
            {
                if(!FilterItem(this.ScanResults[i]) || !this.ScanResults[i].Enable)
                    continue;
                toQueue.Add(new OrgItem(this.ScanResults[i]));

                this.ScanResults[i].Action = OrgAction.Queued;
                this.ScanResults[i].Enable = false;
            }
            OnItemsToQueue(toQueue);
        }

        private void SetEnables(OrgAction action, bool enable)
        {
            for (int i = 0; i < this.ScanResults.Count; i++)
                if ((this.ScanResults[i].Action & action) > 0)
                {
                    this.ScanResults[i].Enable = enable;
                }
            RefreshResultsSafe(false);
        }

        private void EditSelectedItem()
        {
            OrgItemEditorWindow editor = new OrgItemEditorWindow(this.SelectedResultItem);
            editor.ShowDialog();
        }

        #region Columns

        private void SetupColumns()
        {
            this.grid.Columns.Clear();

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
                AddTextColumn("Show", "Show");
                AddTextColumn("Season", "TvEpisode.Season");
                AddTextColumn("Episode", "TvEpisode.Number");
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
            this.grid.Columns.Add(col);
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

            this.grid.Columns.Add(col);
        }

        #endregion
    }
}
