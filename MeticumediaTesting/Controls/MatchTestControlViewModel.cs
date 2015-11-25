using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Meticumedia.WPF;
using System.Windows.Input;
using Meticumedia.Classes;
using System.ComponentModel;

namespace MeticumediaTesting
{
    public class MatchTestWindowViewModel : ViewModel
    {
        #region Properties

        public string MatchString
        {
            get
            {
                return matchString;
            }
            set
            {
                matchString = value;
                OnPropertyChanged(this, "MatchString");
            }
        }
        private string matchString = string.Empty;

        public ObservableCollection<string> MatchProcessing { get; set; }

        public ObservableCollection<OrgPath> ScanDirPaths { get; set; }

        public OrgPath SelectedScanDirPath
        {
            get
            {
                return selectedScanDirPath;
            }
            set
            {
                selectedScanDirPath = value;
                OnPropertyChanged(this, "SelectedScanDirPath");
            }
        }
        private OrgPath selectedScanDirPath;

        #endregion

        #region Commands

        private ICommand runCustomMatchCommand;
        public ICommand RunCustomMatchCommand
        {
            get
            {
                if (runCustomMatchCommand == null)
                {
                    runCustomMatchCommand = new RelayCommand(
                        param => this.RunCustomMatch()
                    );
                }
                return runCustomMatchCommand;
            }
        }

        private ICommand ruScanDirMatchCommand;
        public ICommand RuScanDirMatchCommand
        {
            get
            {
                if (ruScanDirMatchCommand == null)
                {
                    ruScanDirMatchCommand = new RelayCommand(
                        param => this.RunScanDirMatch()
                    );
                }
                return ruScanDirMatchCommand;
            }
        }

        #endregion

        #region Variables

        DirectoryScan scan = new DirectoryScan(false);

        private BackgroundWorker worker;

        #endregion

        #region Constructor

        public MatchTestWindowViewModel()
        {
            this.MatchProcessing = new ObservableCollection<string>();
            ContentSearch.DebugNotification += Search_DebugNotification;
            this.ScanDirPaths = new ObservableCollection<OrgPath>();
            UpdateScanDirPaths();
            Settings.SettingsModified += Settings_SettingsModified;

            scan.DebugNotification += Search_DebugNotification;

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
        }

        #endregion

        #region Methods

        void Settings_SettingsModified(object sender, EventArgs e)
        {
            UpdateScanDirPaths();
        }

        private void UpdateScanDirPaths()
        {
            this.ScanDirPaths.Clear();
            List<OrgItem> autoMoves;
            List<OrgPath> paths = scan.GetFolderFiles(Settings.ScanDirectories.ToList(), true, true, out autoMoves);
            foreach (OrgPath path in paths)
            {
                FileCategory fileCat = FileHelper.CategorizeFile(path, path.Path);
                if (fileCat == FileCategory.TvVideo || fileCat == FileCategory.MovieVideo)
                    this.ScanDirPaths.Add(path);
            }

            if (this.ScanDirPaths.Count > 0)
                this.SelectedScanDirPath = this.ScanDirPaths[0];
        }

        void Search_DebugNotification(object sender, DebugNotificationArgs e)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                this.MatchProcessing.Add(e.Notification);
            });
        }

        private void RunCustomMatch()
        {
            worker.RunWorkerAsync(true);
        }

        private void RunScanDirMatch()
        {
            worker.RunWorkerAsync(false);
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool custom = (bool)e.Argument;

            OrgPath path;
            if (custom)
            {
                string matchTo = this.MatchString;
                if (!this.MatchString.Contains('.'))
                    matchTo += ".avi";

                path = new OrgPath(matchTo, false, true, new OrgFolder());
            }
            else
                path = this.SelectedScanDirPath;

            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                this.MatchProcessing.Clear();
            });

            bool fromLog;
            OrgItem item = scan.ProcessPath(path, false, false, false, false, 0, false, out fromLog);

            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                this.MatchProcessing.Add("Final result: " + item.ToString());
            });
        }

        #endregion
    }
}
