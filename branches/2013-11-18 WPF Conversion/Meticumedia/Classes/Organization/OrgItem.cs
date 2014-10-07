// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Xml;
using Ookii.Dialogs.Wpf;
using System.Windows;
using System.Windows.Media;

namespace Meticumedia.Classes
{
    /// <summary>
    /// An organization item. Stores information about a single organization of a path and handles the action related to organization.
    /// </summary>
    public class OrgItem : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #region Static Properties

        /// <summary>
        /// Global pause for all items.
        /// </summary>
        public static bool QueuePaused { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// The status of the item for a TV rename/missing check
        /// </summary>
        public OrgStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }
        private OrgStatus status = OrgStatus.Missing;

        /// <summary>
        /// The action to be performed on the file
        /// </summary>
        public OrgAction Action
            {
            get
            {
                return action;
            }
            set
            {
                action = value;
                OnPropertyChanged("Action");
                OnPropertyChanged("ActionColor");
                OnPropertyChanged("CanEnable");
            }
        }
        private OrgAction action = OrgAction.None;

        public SolidColorBrush ActionColor
        {
            get
            {
                return new SolidColorBrush(GetActionColor(action));
            }
        }

        /// <summary>
        /// Replacing of destination files has been confirmed by user
        /// </summary>
        public bool Replace
            {
            get
            {
                return replace;
            }
            set
            {
                replace = value;
                OnPropertyChanged("Replace");
            }
        }
        private bool replace = false;

        /// <summary>
        /// Notes to be displayed to user about item
        /// </summary>
        public string Notes
            {
            get
            {
                return notes;
            }
            set
            {
                notes = value;
                OnPropertyChanged("Notes");
            }
        }
        private string notes = string.Empty;

        /// <summary>
        /// Path of the source file
        /// </summary>
        public string SourcePath
        {
            get
            {
                return sourcePath;
            }
            set
            {
                sourcePath = value;
                OnPropertyChanged("SourcePath");
                OnPropertyChanged("SourcePathFileName");
                OnPropertyChanged("SourcePathDirectory");
            }
        }
        public string sourcePath = string.Empty;

        /// <summary>
        /// File name of source file
        /// </summary>
        public string SourcePathFileName
        {
            get
            {
                return string.IsNullOrEmpty(sourcePath) ? "" : Path.GetFileName(sourcePath);
            }
        }

        /// <summary>
        /// Directory of source file
        /// </summary>
        public string SourcePathDirectory
        {
            get
            {
                return string.IsNullOrEmpty(sourcePath) ? "" : Path.GetDirectoryName(sourcePath);
            }
        }

        /// <summary>
        /// Path where the source file will be sent to for a copy/move action.
        /// </summary>
        public string DestinationPath
        {
            get
            {
                return destinationPath;
            }
            set
            {
                destinationPath = value;
                OnPropertyChanged("DestinationPath");
                OnPropertyChanged("DestinationPathFileName");
                OnPropertyChanged("DestinationPathDirectory");
            }
        }
        private string destinationPath = string.Empty;

        /// <summary>
        /// File name of destination file
        /// </summary>
        public string DestinationPathFileName
        {
            get
            {
                return string.IsNullOrEmpty(destinationPath) ? "" : Path.GetFileName(destinationPath);
            }
        }

        /// <summary>
        /// Directory of destination file
        /// </summary>
        public string DestinationPathDirectory
        {
            get
            {
                return string.IsNullOrEmpty(destinationPath) ? "" : Path.GetDirectoryName(destinationPath);
            }
        }

        /// <summary>
        /// TV epsiode that the file is associated with.
        /// </summary>
        public TvEpisode TvEpisode
            {
            get
            {
                return tvEpisode;
            }
            set
            {
                tvEpisode = value;
                OnPropertyChanged("TvEpisode");
            }
        }
        private TvEpisode tvEpisode;

        /// <summary>
        /// 2nd TV episode that the file is assocatied with. Used for multi-episode files only.
        /// </summary>
        public TvEpisode TvEpisode2
        {
            get
            {
                return tvEpisode2;
            }
            set
            {
                tvEpisode2 = value;
                OnPropertyChanged("TvEpisode2");
            }
        }
        private TvEpisode tvEpisode2;

        /// <summary>
        /// Whether item is for multiple TV episodes
        /// </summary>
        public bool MultiEpisode
        {
            get
            {
                return multiEpisode;
            }
            set
            {
                multiEpisode = value;
                OnPropertyChanged("MultiEpisode");
            }
        }
        private bool multiEpisode = false;

        /// <summary>
        /// Movie that file is associated with.
        /// </summary>
        public Movie Movie
            {
            get
            {
                return movie;
            }
            set
            {
                movie = value;
                OnPropertyChanged("Movie");
            }
        }
        private Movie movie;

        /// <summary>
        /// Tv Show
        /// </summary>
        public TvShow Show
        {
            get
            {
                return show;
            }
            set
            {
                show = value;
                OnPropertyChanged("Show");
            }
        }
        private TvShow show;

        /// <summary>
        /// Flag indicating TV episode(s) associated with item are for a newly found show.
        /// </summary>
        public TvShow NewShow
            {
            get
            {
                return newShow;
            }
            set
            {
                newShow = value;
                OnPropertyChanged("NewShow");
            }
        }
        private TvShow newShow;

        /// <summary>
        /// Scan Directory where source comes from
        /// </summary>
        public OrgFolder ScanDirectory
            {
            get
            {
                return scanDirectory;
            }
            set
            {
                scanDirectory = value;
                OnPropertyChanged("ScanDirectory");
            }
        }
        private OrgFolder scanDirectory;

        /// <summary>
        /// The file categarization.
        /// </summary>
        public FileCategory Category
            {
            get
            {
                return category;
            }
            set
            {
                category = value;
                OnPropertyChanged("Category");
            }
        }
        private FileCategory category;

        /// <summary>
        /// Enable for the item in the list of item to be organized.
        /// </summary>
        public bool Enable
        {
            get
            {
                return enable;
            }
            set
            {
                enable = value;
                OnPropertyChanged("Enable");
            }
        }
        private bool enable;

        public bool CanEnable
        {
            get
            {
                switch (this.Action)
                {
                    case OrgAction.Move:
                    case OrgAction.Copy:
                    case OrgAction.Rename:
                    case OrgAction.Delete:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Action progress percentage.
        /// </summary>
        public int Progress
        {
            get
            {
                return progress;
            }
            set
            {
                progress = value;
                OnPropertyChanged("Progress");
                OnPropertyChanged("QueueStatusString");
            }
        }
        private int progress;

        /// <summary>
        /// Item paused
        /// </summary>
        public OrgQueueStatus QueueStatus
            {
            get
            {
                return queueStatus;
            }
            set
            {
                queueStatus = value;
                OnPropertyChanged("QueueStatus");
                OnPropertyChanged("QueueStatusString");
            }
        }
        private OrgQueueStatus queueStatus = OrgQueueStatus.Enabled;

        public string QueueStatusString
        {
            get
            {
                string status = string.Empty;
                switch (this.QueueStatus)
                {
                    case OrgQueueStatus.Enabled:

                        if (this.Progress > 0)
                            status =  this.Progress.ToString() + "%";
                        else
                            status = "Queued";
                        break;
                    case OrgQueueStatus.Paused:
                        status = "Paused";
                        if (this.Progress > 0)
                            status += " - " + this.Progress.ToString() + "%";
                        break;
                    case OrgQueueStatus.Failed:
                        status = "Failed";
                        break;
                    case OrgQueueStatus.Completed:
                        status = "Completed";
                        break;
                    case OrgQueueStatus.Cancelled:
                        status = "Cancelled";
                        break;
                }

                return status;
            }
        }

        /// <summary>
        /// State of whether the action has been completed or not. Used for queuing.
        /// </summary>
        public bool ActionComplete
            {
            get
            {
                return actionComplete;
            }
            set
            {
                actionComplete = value;
                OnPropertyChanged("ActionComplete");
            }
        }
        private bool actionComplete = false;

        /// <summary>
        /// State of whether the action was finished due to completing action or cancellation (user chose not to override destination).
        /// </summary>
        public bool ActionSucess
            {
            get
            {
                return actionSucess;
            }
            set
            {
                actionSucess = value;
                OnPropertyChanged("ActionSucess");
            }
        }
        private bool actionSucess = false;

        /// <summary>
        /// Date/time when the action was performed. Only valid is ActionComplete is true.
        /// </summary>
        public DateTime ActionTime
            {
            get
            {
                return actionTime;
            }
            set
            {
                actionTime = value;
                OnPropertyChanged("ActionTime");
            }
        }
        private DateTime actionTime;

        /// <summary>
        /// Item number in scan. To be processed in same order as scanned to prevent conflicts.
        /// </summary>
        public int Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
                OnPropertyChanged("Number");
            }
        }
        private int number = -1;

        #endregion

        #region Variables

        /// <summary>
        /// Buffer size for file transfers
        /// </summary>
        private static readonly int BUFFER_SIZE = 65536; // 64 MB

        /// <summary>
        /// Whether or not item action has been started.
        /// </summary>
        private bool actionStarted = false;

        /// <summary>
        /// Whether or not action is currently running.
        /// </summary>
        private bool actionRunning = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for directory scan for file matched to TV episode.
        /// </summary>
        /// <param name="action">action to be performed</param>
        /// <param name="file">source path</param>
        /// <param name="destination">destination path</param>
        /// <param name="episode">TV episode for file</param>
        /// <param name="episode2">2nd Tv epsidoe for file</param>
        /// <param name="category">file category</param>
        public OrgItem(OrgAction action, string file, string destination, TvEpisode episode, TvEpisode episode2, FileCategory category, OrgFolder scanDir, TvShow newShow)
        {
            this.Status = OrgStatus.Found;
            this.Action = action;
            this.SourcePath = file;
            this.DestinationPath = destination;
            this.TvEpisode = episode;
            this.Category = category;
            this.TvEpisode2 = episode2;
            this.Enable = false;
            this.ScanDirectory = scanDir;
            this.NewShow = newShow;
            this.Replace = false;
            this.Number = 0;
        }

        /// <summary>
        /// Constructor for TV rename/missing check where file was found.
        /// </summary>
        /// <param name="status">Status of rename/missing</param>
        /// <param name="action">action to be performed</param>
        /// <param name="file">source path</param>
        /// <param name="destination">destination path</param>
        /// <param name="episode">TV episode for file</param>
        /// <param name="episode2">2nd Tv epsidoe for file</param>
        /// <param name="category">file category</param>
        public OrgItem(OrgStatus status, OrgAction action, string file, string destination, TvEpisode episode, TvEpisode episode2, FileCategory category, OrgFolder scanDir)
            : this(action, file, destination, episode, episode2, category, scanDir, null)
        {
            this.Status = status;
            this.Progress = 0;
        }

        /// <summary>
        /// Constructor for TV rename/missing check where file was not found.
        /// </summary>
        /// <param name="status">Status of rename/missing</param>
        /// <param name="action">action to be performed</param>
        /// <param name="episode">TV episode for file</param>
        /// <param name="episode2">2nd Tv epsidoe for file</param>
        /// <param name="category">file category</param>
        public OrgItem(OrgStatus status, OrgAction action, TvEpisode episode, TvEpisode episode2, FileCategory category, OrgFolder scanDir)
        {
            this.Status = status;
            this.Progress = 0;
            this.Action = action;
            this.SourcePath = string.Empty;
            if (action == OrgAction.Delete)
                this.DestinationPath = FileHelper.DELETE_DIRECTORY;
            else
                this.DestinationPath = string.Empty;
            this.TvEpisode = episode;
            this.TvEpisode2 = episode2;
            this.Category = category;
            this.Enable = false;
            this.ScanDirectory = scanDir;
            this.NewShow = null;
            this.Replace = false;
            this.Number = 0;
        }

        /// <summary>
        /// Constructor for directory scan for file that is unknown.
        /// </summary>
        /// <param name="action">action to be performed</param>
        /// <param name="file">source path</param>
        /// <param name="category">file category</param>
        public OrgItem(OrgAction action, string file, FileCategory category, OrgFolder scanDir)
        {
            this.Status = OrgStatus.Found;
            this.Progress = 0;
            this.Action = action;
            this.SourcePath = file;
            if (action == OrgAction.Delete)
                this.DestinationPath = FileHelper.DELETE_DIRECTORY;
            else
                this.DestinationPath = string.Empty;
            this.TvEpisode = null;
            this.Category = category;
            this.Enable = false;
            this.ScanDirectory = scanDir;
            this.NewShow = null;
            this.Replace = false;
            this.Number = 0;
        }

        /// <summary>
        /// Constructor for movie item.
        /// </summary>
        /// <param name="action">action to be performed</param>
        /// <param name="sourceFile">the source path</param>
        /// <param name="category">file's category</param>
        /// <param name="movie">Movie object related to file</param>
        /// <param name="destination">destination path</param>
        /// <param name="scanDir">path to content folder of movie</param>
        public OrgItem(OrgAction action, string sourceFile, FileCategory category, Movie movie, string destination, OrgFolder scanDir)
        {
            this.Status = OrgStatus.Found;
            this.Progress = 0;
            this.Action = action;
            this.SourcePath = sourceFile;
            this.Movie = movie;
            this.ScanDirectory = scanDir;
            this.DestinationPath = destination;
            this.TvEpisode = null;
            this.Category = category;
            this.Enable = false;
            this.NewShow = null;
            this.Replace = false;
            this.Number = 0;
        }

        /// <summary>
        /// Constructor for copying item.
        /// </summary>
        /// <param name="item">item to be copied</param>
        public OrgItem(OrgItem item)
        {
            UpdateInfo(item);
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public OrgItem()
        {
            this.Status = OrgStatus.Found;
            this.Replace = false;
        }

        /// <summary>
        /// Update current update from another item's data
        /// </summary>
        /// <param name="item">item to be copied</param>
        public void UpdateInfo(OrgItem item)
        {
            this.Status = item.Status;
            this.Progress = 0;
            this.Action = item.Action;
            this.SourcePath = item.SourcePath;
            this.DestinationPath = item.DestinationPath;
            this.TvEpisode = item.TvEpisode;
            this.TvEpisode2 = item.TvEpisode2;
            this.Category = item.Category;
            this.Enable = item.Enable;
            this.Movie = item.Movie;
            this.ScanDirectory = item.ScanDirectory;
            this.NewShow = item.NewShow;
            this.Show = item.Show;
            this.Replace = item.Replace;
            this.Number = item.Number;
            this.QueueStatus = item.QueueStatus;
            this.ActionComplete = item.ActionComplete;
            this.Notes = item.Notes;
            this.ActionTime = item.ActionTime;
        }

        #endregion

        #region Comparisons

        /// <summary>
        /// Static variable that determine whether CompareBy method will sort in ascending order
        /// </summary>
        public static bool AscendingSort = false;

        /// <summary>
        /// Sets compare results to acending/descending based on AscendingSort property.
        /// </summary>
        /// <param name="ascendingResult">Compare result for ascending order</param>
        /// <returns>Compare results to give order determined by AscendingSort</returns>
        private static int SetSort(int ascendingResult)
        {
            if (AscendingSort || ascendingResult == 0)
                return ascendingResult;
            else if (ascendingResult == -1)
                return 1;
            else
                return -1;
        }

        /// <summary>
        /// Compares two OrgItem instances by the status
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item</param>
        /// <returns>Compare results of two items</returns>
        public static int CompareByStatus(OrgItem x, OrgItem y)
        {
            int sortResult;
            if (x == null)
            {
                if (y == null)
                    sortResult = 0;
                else
                    sortResult = -1;
            }
            else
            {
                if (y == null)
                    sortResult = 1;
                else
                    sortResult = x.Status.ToString().CompareTo(y.Status.ToString());
            }

            if (sortResult == 0)
                return CompareByShowName(x, y);

            return SetSort(sortResult);
        }

        /// <summary>
        /// Compares two OrgItem instances by the action
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item</param>
        /// <returns>Compare results of two items</returns>
        public static int CompareByNumber(OrgItem x, OrgItem y)
        {
            int sortResult;
            if (x == null)
            {
                if (y == null)
                    sortResult = 0;
                else
                    sortResult = -1;
            }
            else
            {
                if (y == null)
                    sortResult = 1;
                else
                    sortResult = x.Number.CompareTo(y.Number);
            }

            return SetSort(sortResult);
        }

        /// <summary>
        /// Compares two OrgItem instances by the action
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item</param>
        /// <returns>Compare results of two items</returns>
        public static int CompareByAction(OrgItem x, OrgItem y)
        {
            int sortResult;
            if (x == null)
            {
                if (y == null)
                    sortResult = 0;
                else
                    sortResult = -1;
            }
            else
            {
                if (y == null)
                    sortResult = 1;
                else
                    sortResult = x.Action.ToString().CompareTo(y.Action.ToString());
            }

            if (sortResult == 0)
                return CompareBySourceFile(x, y);

            return SetSort(sortResult);
        }

        /// <summary>
        /// Compares two OrgItem instances by the category 
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item</param>
        /// <returns>Compare results of two items</returns>
        public static int CompareByCategory(OrgItem x, OrgItem y)
        {
            int sortResult;
            if (x == null)
            {
                if (y == null)
                    sortResult = 0;
                else
                    sortResult = -1;
            }
            else
            {
                if (y == null)
                    sortResult = 1;
                else
                    sortResult = x.Category.ToString().CompareTo(y.Category.ToString());
            }

            if (sortResult == 0)
                return CompareBySourceFile(x, y);

            return SetSort(sortResult);
        }

        /// <summary>
        /// Compares two OrgItem instances by the source path
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item</param>
        /// <returns>Compare results of two items</returns>
        public static int CompareBySourceFile(OrgItem x, OrgItem y)
        {
            int sortResult;
            if (x == null)
            {
                if (y == null)
                    sortResult = 0;
                else
                    sortResult = -1;
            }
            else
            {
                if (y == null)
                    sortResult = 1;
                else
                    sortResult = Path.GetFileName(x.SourcePath).CompareTo(Path.GetFileName(y.SourcePath));
            }

            return SetSort(sortResult);
        }

        /// <summary>
        /// Compares two OrgItem instances by the folder of source
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item</param>
        /// <returns>Compare results of two items</returns>
        public static int CompareBySourceFolder(OrgItem x, OrgItem y)
        {
            int sortResult;
            if (x == null || string.IsNullOrEmpty(x.SourcePath))
            {
                if (y == null || string.IsNullOrEmpty(y.SourcePath))
                    sortResult = 0;
                else
                    sortResult = -1;
            }
            else
            {
                if (y == null || string.IsNullOrEmpty(y.SourcePath))
                    sortResult = 1;
                else
                    sortResult = Path.GetDirectoryName(x.SourcePath).CompareTo(Path.GetDirectoryName(y.SourcePath));
            }

            if (sortResult == 0)
                return CompareBySourceFile(x, y);

            return SetSort(sortResult);
        }

        /// <summary>
        /// Compares two OrgItem instances by the destination path
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item</param>
        /// <returns>Compare results of two items</returns>
        public static int CompareByDestinationFile(OrgItem x, OrgItem y)
        {
            int sortResult;
            if (x == null)
            {
                if (y == null)
                    sortResult = 0;
                else
                    sortResult = -1;
            }
            else
            {
                if (y == null)
                    sortResult = 1;
                else
                {
                    string xStr = "", yStr = "";

                    if (x.DestinationPath != FileHelper.DELETE_DIRECTORY)
                        xStr = Path.GetFileName(x.DestinationPath);
                    if (y.DestinationPath != FileHelper.DELETE_DIRECTORY)
                        yStr = Path.GetFileName(y.DestinationPath);

                    sortResult = xStr.CompareTo(yStr);
                }
            }

            return SetSort(sortResult);
        }

        /// <summary>
        /// Compares two OrgItem instances by the folder of destination path
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item</param>
        /// <returns>Compare results of two items</returns>
        public static int CompareByDestinationFolder(OrgItem x, OrgItem y)
        {
            int sortResult;
            if (x == null)
            {
                if (y == null)
                    sortResult = 0;
                else
                    sortResult = -1;
            }
            else
            {
                if (y == null)
                    sortResult = 1;
                else
                {
                    string xStr = "", yStr = "";

                    if (x.DestinationPath != FileHelper.DELETE_DIRECTORY)
                        xStr = Path.GetFileName(x.DestinationPath);
                    else
                        xStr = x.DestinationPath;
                    if (y.DestinationPath != FileHelper.DELETE_DIRECTORY)
                        yStr = Path.GetFileName(y.DestinationPath);
                    else
                        yStr = y.DestinationPath;

                    sortResult = xStr.CompareTo(yStr);
                }
            }

            if (sortResult == 0)
                return CompareByDestinationFile(x, y);

            return SetSort(sortResult);
        }

        /// <summary>
        /// Compares two OrgItem instances by the movie
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item</param>
        /// <returns>Compare results of two items</returns>
        public static int CompareByMovie(OrgItem x, OrgItem y)
        {
            int sortResult;
            if (x == null)
            {
                if (y == null)
                    sortResult = 0;
                else
                    sortResult = -1;
            }
            else
            {
                if (y == null)
                    sortResult = 1;
                else
                    sortResult = Path.GetDirectoryName(x.Movie.DatabaseName).CompareTo(Path.GetDirectoryName(y.Movie.DatabaseName));
            }

            if (sortResult == 0)
                return CompareBySourceFile(x, y);

            return SetSort(sortResult);
        }

        /// <summary>
        /// Compares two OrgItem instances by the tv episode's number
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item</param>
        /// <returns>Compare results of two items</returns>
        public static int CompareByEpisodeNumber(OrgItem x, OrgItem y)
        {
            int sortResult;
            if (x == null)
            {
                if (y == null)
                    sortResult = 0;
                else
                    sortResult = -1;
            }
            else
            {
                if (y == null)
                    sortResult = 1;
                else
                    sortResult = x.TvEpisode.Number.CompareTo(y.TvEpisode.Number);
            }

            if (sortResult == 0)
                return CompareBySourceFile(x, y);

            return SetSort(sortResult);
        }

        /// <summary>
        /// Compares two OrgItem instances by the tv episode's season number
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item</param>
        /// <returns>Compare results of two items</returns>
        public static int CompareBySeasonNumber(OrgItem x, OrgItem y)
        {
            int sortResult;
            if (x == null)
            {
                if (y == null)
                    sortResult = 0;
                else
                    sortResult = -1;
            }
            else
            {
                if (y == null)
                    sortResult = 1;
                else
                    sortResult = x.TvEpisode.Season.CompareTo(y.TvEpisode.Season);
            }

            if (sortResult == 0)
                return CompareByEpisodeNumber(x, y);

            return SetSort(sortResult);
        }

        /// <summary>
        /// Compares two OrgItem instances by the tv episode's show name
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item</param>
        /// <returns>Compare results of two items</returns>
        public static int CompareByShowName(OrgItem x, OrgItem y)
        {
            int sortResult;
            if (x == null)
            {
                if (y == null)
                    sortResult = 0;
                else
                    sortResult = -1;
            }
            else
            {
                if (y == null)
                    sortResult = 1;
                else
                    sortResult = x.TvEpisode.Show.CompareTo(y.TvEpisode.Show);
            }

            if (sortResult == 0)
                return CompareBySeasonNumber(x, y);

            return SetSort(sortResult);
        }

        /// <summary>
        /// Compares two OrgItem instances by the  date time of action
        /// </summary>
        /// <param name="x">The first item</param>
        /// <param name="y">The second item</param>
        /// <returns>Compare results of two items</returns>
        public static int CompareByDateTime(OrgItem x, OrgItem y)
        {
            int sortResult;
            if (x == null)
            {
                if (y == null)
                    sortResult = 0;
                else
                    sortResult = -1;
            }
            else
            {
                if (y == null)
                    sortResult = 1;
                else
                    sortResult = x.ActionTime.CompareTo(y.ActionTime);
            }
            
            return SetSort(sortResult);
        }

        #endregion

        #region Sort

        /// <summary>
        /// Sorts a list of OrgItems based on a specific data (column) type.
        /// </summary>
        /// <param name="orgItems">List of items to be sorted</param>
        /// <param name="sortType">Column type to sort by</param>
        public static void Sort(List<OrgItem> orgItems, OrgColumnType sortType)
        {
            // Sort the Scan results (so that indices from that will still match listview after sort)
            switch (sortType)
            {
                case OrgColumnType.DateTime:
                    orgItems.Sort(OrgItem.CompareByDateTime);
                    break;
                case OrgColumnType.Show:
                    orgItems.Sort(OrgItem.CompareByShowName);
                    break;
                case OrgColumnType.Season:
                    orgItems.Sort(OrgItem.CompareBySeasonNumber);
                    break;
                case OrgColumnType.Episode:
                    orgItems.Sort(OrgItem.CompareByEpisodeNumber);
                    break;
                case OrgColumnType.Movie:
                    orgItems.Sort(OrgItem.CompareByMovie);
                    break;
                case OrgColumnType.Source_Folder:
                    orgItems.Sort(OrgItem.CompareBySourceFolder);
                    break;
                case OrgColumnType.Source_File:
                    orgItems.Sort(OrgItem.CompareBySourceFile);
                    break;
                case OrgColumnType.Category:
                    orgItems.Sort(OrgItem.CompareByCategory);
                    break;
                case OrgColumnType.Status:
                    orgItems.Sort(OrgItem.CompareByStatus);
                    break;
                case OrgColumnType.Action:
                    orgItems.Sort(OrgItem.CompareByAction);
                    break;
                case OrgColumnType.Destination_Folder:
                    orgItems.Sort(OrgItem.CompareByDestinationFolder);
                    break;
                case OrgColumnType.Destination_File:
                    orgItems.Sort(OrgItem.CompareByDestinationFile);
                    break;
                case OrgColumnType.Number:
                    orgItems.Sort(OrgItem.CompareByNumber);
                    break;
                default:
                    throw new Exception("Unknown column type");
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event for monitoring progress of item action.
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> ActionProgressChanged;

        /// <summary>
        /// Fires ActionProgressChanged event to the inputted percentage.
        /// </summary>
        /// <param name="progress">Percentage of progress for the action</param>
        protected void OnProgressChange(int progress)
        {
            this.Progress = progress;
            if (ActionProgressChanged != null)
                ActionProgressChanged(this, new ProgressChangedEventArgs(progress, null));
        }

        #endregion

        #region Actions

        /// <summary>
        /// Performs the action for the item.
        /// </summary>
        public void PerformAction()
        {
            // Initialize itme progress to 0 if it hasn't been started.
            if (!actionStarted)
                OnProgressChange(0);

            // Set running property
            this.actionRunning = true;
            this.ActionSucess = true;

            DateTime startTime = DateTime.Now;

            // Perform the appropriate action
            try
            {
                switch (this.Action)
                {
                    case OrgAction.Copy:
                    case OrgAction.Move:
                    case OrgAction.Rename:
                        if (this.Category == FileCategory.Folder)
                        {
                            this.ActionComplete = CopyMoveFolder();

                            if (this.ActionComplete)
                            {
                                if (this.Movie != null)
                                    this.Movie.Path = this.DestinationPath;
                                else if (this.Show != null)
                                    this.Show.Path = this.DestinationPath;
                            }
                        }
                        else
                            this.ActionComplete = CopyMoveFile(this.SourcePath, this.DestinationPath, 0, 100);
                        break;
                    case OrgAction.Delete:
                        if (this.Category == FileCategory.Folder)
                            DeleteDirectory(this.SourcePath);
                        else
                            File.Delete(this.SourcePath);
                        this.ActionComplete = true;
                        break;
                    default:
                        throw new Exception("Unknown queue action");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught on OrgItem action: " + e.ToString());
                this.ActionComplete = true;
                this.ActionSucess = false;
                this.QueueStatus = OrgQueueStatus.Failed;
            }

            // Check if action is complete
            if (this.ActionComplete)
            {
                OnProgressChange(100);
                actionStarted = false;
                this.ActionTime = DateTime.Now;

                // Pause here if needed to make queue not go crazy with refreshes
                int time = (int)(DateTime.Now - startTime).TotalMilliseconds;
                if (time < 25)
                    Thread.Sleep(25 - time);

                // Check if sucessful
                if (this.ActionSucess)
                {
                    // Cleanup folder (delete empty sub-folders)
                    if (this.Category != FileCategory.Folder)
                        CleanupFolder();

                    // Add current file to ignore if copy action
                    if (this.Action == OrgAction.Copy)
                        foreach (OrgFolder sd in Settings.ScanDirectories)
                            if (this.ScanDirectory != null && sd.FolderPath == this.ScanDirectory.FolderPath)
                                sd.AddIgnoreFile(this.SourcePath);

                    // Set Completed status
                    this.QueueStatus = OrgQueueStatus.Completed;
                }
            }

            // Clear action running
            actionRunning = false;
        }

        /// <summary>
        /// Delete all empty directories from scan folder item belongs to if it is enabled
        /// </summary>
        private void CleanupFolder()
        {
            // Get scan folder that file belongs to
            OrgFolder itemFolder = null;
            foreach (OrgFolder folder in Settings.ScanDirectories)
                if (this.SourcePath.StartsWith(folder.FolderPath))
                {
                    itemFolder = folder;
                    break;
                }

            // Check that folder was found and that cleanup is allowed
            if (itemFolder == null || !itemFolder.AutomaticallyDeleteEmptyFolders || !itemFolder.Recursive)
                return;

            DeleteEmptyDirectories(itemFolder.FolderPath, true);
        }

        /// <summary>
        /// Recursively delete empty directories
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="root"></param>
        private void DeleteEmptyDirectories(string directory, bool root)
        {
            string[] subDirs = Directory.GetDirectories(directory);
            foreach (string subDir in subDirs)
                DeleteEmptyDirectories(subDir, false);

           if (Directory.GetFiles(directory).Length > 0)
                return;
           if (Directory.GetDirectories(directory).Length > 0)
               return;

           if (!root)
               Directory.Delete(directory);
            return;
        }

        /// <summary>
        /// Performs copy/move action for a file. Reports progress during transfer.
        /// </summary>
        /// <returns>Whether or not the copy/move was completed</returns>
        private bool CopyMoveFile(string sourcePath, string destinationPath, double baseProgress, double progressAmount)
        {
            // Check that the destination folder exists, if not create it
            string destDir = Path.GetDirectoryName(destinationPath);
            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            // Check if destination already exists
            if (!this.actionStarted && File.Exists(destinationPath))
            {
                if (sourcePath.ToLower() == destinationPath.ToLower())
                {
                    File.Move(sourcePath, sourcePath + "-temp");
                    File.Move(sourcePath + "-temp", destinationPath);
                    return true;
                }
                else
                {
                    // Check if user wants to override it
                    string ask = "Destination file '" + destinationPath + "' already existst. Queue action will overwrite it, would you like to continue?";
                    TaskDialog diag = new TaskDialog(); // TODO!
                    diag.VerificationText = ask;
                    if (!this.Replace && diag.Show().ButtonType == ButtonType.Yes)
                    {
                        this.ActionSucess = false;
                        this.QueueStatus = OrgQueueStatus.Cancelled;
                        return true;
                    }
                }
            }

            // Check drive space
            if (!CheckDriveSpace((new FileInfo(sourcePath)).Length))
                return false;

            // Rename or move on same drive
            if (this.Action == OrgAction.Rename || (this.Action == OrgAction.Move && Path.GetPathRoot(sourcePath) == Path.GetPathRoot(destinationPath)))
            {
                if(File.Exists(destinationPath))
                    File.Delete(destinationPath);
                File.Move(sourcePath, destinationPath);
                return true;
            }

            // Create streams for reading source and writing destination
            FileStream sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
            FileStream destinationStream = new FileStream(destinationPath, FileMode.OpenOrCreate, FileAccess.Write);

            // Initialize transfer variables
            int bytesRead = -1;
            long totalSize = sourceStream.Length;
            byte[] bytes = new byte[BUFFER_SIZE];
            long totalFileRead = 0;

            // If resuming the operation then move streams positions
            if (actionStarted)
            {
                sourceStream.Position = destinationStream.Length;
                destinationStream.Position = destinationStream.Length;
                totalFileRead = destinationStream.Length;
            }

            // Set started
            actionStarted = true;

            // Transfer the file
            while (true)
            {
                // Check for pauses in the operation
                if (this.QueueStatus == OrgQueueStatus.Paused || OrgItem.QueuePaused)
                    break;

                // Move data from source file to buffer, if no more data we're done!
                bytesRead = sourceStream.Read(bytes, 0, BUFFER_SIZE);
                if (bytesRead == 0)
                    break;

                // Copy buffered data to destination
                destinationStream.Write(bytes, 0, bytesRead);

                // Update progress
                totalFileRead += bytesRead;
                OnProgressChange((int)(baseProgress + (double)totalFileRead / totalSize * progressAmount));
            }

            // Close streams
            sourceStream.Close();
            sourceStream.Dispose();
            destinationStream.Flush();
            destinationStream.Close();
            destinationStream.Dispose();

            // If operation was paused return not completed
            if (this.QueueStatus == OrgQueueStatus.Paused || QueuePaused)
                return false;

            // For move operation delete the source file now that it's been copied to destination
            if (this.Action == OrgAction.Move)
                File.Delete(sourcePath);

            // Return completed
            return true;
        }

        /// <summary>
        /// Performs copy/move action for a folder. Reports progress during transfer.
        /// </summary>
        /// <returns>Whether or not the copy/move was completed</returns>
        private bool CopyMoveFolder()
        {
            // Check for case change only condition
            if (this.SourcePath.ToLower() == this.DestinationPath.ToLower())
            {
                Directory.Move(this.SourcePath, this.SourcePath + "-temp");
                Directory.Move(this.SourcePath + "-temp", this.DestinationPath);
                return true;
            }

            // Check for directory already exists condition
            if (!actionStarted && Directory.Exists(this.DestinationPath))
            {
                throw new NotImplementedException();
                // TODO: message box for this
                //DialogResult results = MessageBox.Show("Destination folder '" + this.DestinationPath + "' already existst. Folder rename action will merge the two folder (source files will replace destination files), would you like to continue?", "Merge destination folder?", MessageBoxButtons.YesNo);
                //if (results == DialogResult.No)
                //{
                //    this.ActionSucess = false;
                //    this.QueueStatus = OrgQueueStatus.Cancelled;
                //    return true;
                //}
            }
            
            // Rename or move on same drive
            else if (this.Action == OrgAction.Rename || Path.GetPathRoot(this.SourcePath) == Path.GetPathRoot(this.DestinationPath))
            {
                Directory.Move(this.SourcePath, this.DestinationPath);
                return true;
            }

            // Get list of file in directory (recursive)
            List<string> sourceFiles = new List<string>();
            long totalSize = 0;
            BuildFileList(this.SourcePath, sourceFiles, ref totalSize);

            // Check drive space
            CheckDriveSpace(totalSize);

            // Move each file one at a time
            for (int i = 0; i < sourceFiles.Count; i++)
            {
                // Set progress
                double progress = (double)i / (sourceFiles.Count) * 100;
                OnProgressChange((int)progress);

                // Check for pauses in the operation
                if (this.QueueStatus == OrgQueueStatus.Paused || OrgItem.QueuePaused)
                    break;

                // Create destination path
                string destinationFile = sourceFiles[i].Replace(this.SourcePath, this.DestinationPath);

                // Delete destination if needed (source will replace it)
                if (File.Exists(destinationFile))
                    File.Delete(destinationFile);

                // Check that the destination folder exists, if not create it
                string destDir = Path.GetDirectoryName(destinationFile);
                if (!Directory.Exists(destDir))
                    Directory.CreateDirectory(destDir);

                // Move the file
                if (!CopyMoveFile(sourceFiles[i], destinationFile, progress, 100D / sourceFiles.Count))
                    break;
            }

            // If operation was paused return not completed
            if (this.QueueStatus == OrgQueueStatus.Paused || QueuePaused)
                return false;

            // Set final progress
            OnProgressChange(100);

            // For move operation delete the source now that it's been copied to destination
            if (this.Action == OrgAction.Move)
                DeleteDirectory(this.SourcePath);

            // Return completed
            return true;
        }

        /// <summary>
        /// Check that there is space on destination drive if necessary
        /// </summary>
        /// <param name="spaceRequired">Space required to do move to destination</param>
        /// <returns>Boolean indicating if there is sufficient spcae on destination drive</returns>
        private bool CheckDriveSpace(long spaceRequired)
        {
            // Check if moving to new drive
            if (Path.GetPathRoot(this.SourcePath) != Path.GetPathRoot(this.DestinationPath))
            {
                // Check that there is sufficient room on destination
                if (spaceRequired > (new DriveInfo(Path.GetPathRoot(this.DestinationPath))).TotalFreeSpace)
                {
                    OrgItem.QueuePaused = true;
                    MessageBox.Show("Queue has been paused due to insufficient space on destination drive of current queue item");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Recursively build list of file from directory and its sub-directories
        /// </summary>
        /// <param name="directory">Path to directory</param>
        /// <param name="files">File path list being built</param>
        private void BuildFileList(string directory, List<string> files, ref long totalSize)
        {
           
            // Add all files in currently directory to list
            string[] currentFiles = Directory.GetFiles(directory);
            foreach (string file in currentFiles)
            {
                files.Add(file);
                totalSize += (new FileInfo(file)).Length;
            }

            // Recursively add sub-directories
            string[] subDirs = Directory.GetDirectories(directory);
            foreach (string subDir in subDirs)
                BuildFileList(subDir, files, ref totalSize);
            
        }

        /// <summary>
        /// Delete a directory by recursively deleting its sub-directories first.
        /// </summary>
        /// <param name="directory">Path to directory to be deleted</param>
        private void DeleteDirectory(string directory)
        {
            string[] subDirs = Directory.GetDirectories(directory);
            foreach (string subDir in subDirs)
                DeleteDirectory(subDir);
            
            string[] currentFiles = Directory.GetFiles(directory);
            foreach (string file in currentFiles)
                File.Delete(file);

            Directory.Delete(directory);
        }

        /// <summary>
        /// Cancels action for item. 
        /// </summary>
        public void CancelAction()
        {
            // Check if action was started
            if (actionStarted && File.Exists(this.DestinationPath))
            {
                // If running action, pause it and wait for it to stop
                while (actionRunning)
                {
                    this.QueueStatus = OrgQueueStatus.Paused;
                    Thread.Sleep(100);
                }

                // Delete incomplete destination file
                File.Delete(this.DestinationPath);
            }
        }

        #endregion

        #region Colors

        private static Color GetActionColor(OrgAction action)
        {            
            switch (action)
            {
                case OrgAction.Empty:
                    return Colors.LightGray;
                case OrgAction.None:
                    return Colors.Gray;
                case OrgAction.AlreadyExists:
                    return Colors.Gray;
                case OrgAction.Move:
                case OrgAction.Copy:
                    return Colors.DarkGreen;
                case OrgAction.Rename:
                    return Colors.Green;
                case OrgAction.Delete:
                    return Colors.Red;
                case OrgAction.Queued:
                    return Colors.Goldenrod;
                case OrgAction.NoRootFolder:
                    return Colors.Black;
                case OrgAction.TBD:
                    return Colors.LightGray;
                case OrgAction.Processing:
                    return Colors.DarkOrange;
                default:
                    return Colors.Black;
            }
        }

        #endregion

        #region XML

        /// <summary>
        /// Root XML element for saving instance of this class.
        /// </summary>
        private static readonly string ROOT_XML = "Action";

        /// <summary>
        /// Element names for properties that need to be saved to XML.
        /// </summary>
        private enum XmlElements { Action, SourcePath, DestinationPath, Category, ActionTime };

        /// <summary>
        /// Adds OrgItem properties to XML file.
        /// </summary>
        /// <param name="xw">XML writer to add to</param>
        public void Save(XmlWriter xw)
        {
            // Start item
            xw.WriteStartElement(ROOT_XML);

            // Write properties (note: this is overkill for single type, but setup make it easy to add more properites if necessary)
            foreach (XmlElements element in Enum.GetValues(typeof(XmlElements)))
            {
                string value = null;
                switch (element)
                {
                    case XmlElements.Action:
                        value = this.Action.ToString();
                        break;
                    case XmlElements.SourcePath:
                        value = this.SourcePath;
                        break;
                    case XmlElements.DestinationPath:
                        value = this.DestinationPath;
                        break;
                    case XmlElements.Category:
                        value = this.Category.ToString();
                        break;
                    case XmlElements.ActionTime:
                        value = this.ActionTime.ToString();
                        break;
                    default:
                        throw new Exception("Unkonw element!");
                }

                // If value is valid save it
                if (value != null)
                    xw.WriteElementString(element.ToString(), value);
            }

            // End item
            xw.WriteEndElement();
        }

        /// <summary>
        /// Loads instance from XML.
        /// </summary>
        /// <param name="itemNode">Node to load XML from</param>
        public bool Load(XmlNode itemNode)
        {
            // Check that root element matches expected
            if (itemNode.Name != ROOT_XML)
                return false;

            // Loop through child elements and load each property
            foreach (XmlNode propNode in itemNode.ChildNodes)
            {
                // Get the current element/property type
                XmlElements element;
                if (!Enum.TryParse<XmlElements>(propNode.Name, out element))
                    continue;

                // Get value
                string value = propNode.InnerText;

                // Load value into appropriate property
                switch (element)
                {
                    case XmlElements.Action:
                        OrgAction action;
                        if(Enum.TryParse<OrgAction>(value, out action))
                            this.Action = action;
                        break;
                    case XmlElements.DestinationPath:
                        this.DestinationPath = value;
                        break;
                    case XmlElements.SourcePath:
                        this.SourcePath = value;
                        break;
                    case XmlElements.Category:
                        FileCategory category;
                        if (Enum.TryParse<FileCategory>(value, out category))
                            this.Category = category;
                        break;
                    case XmlElements.ActionTime:
                        DateTime actionTime;
                        DateTime.TryParse(value, out actionTime);
                        this.ActionTime = actionTime;
                        break;
                }
            }

            // Successful load!
            return true;
        }

        #endregion
    }
}
