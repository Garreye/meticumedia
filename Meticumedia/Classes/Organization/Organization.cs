// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Threading;
using Meticumedia.Classes;
using System.Collections.ObjectModel;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Contains data and methods related to organization on a very high level.
    /// </summary>
    public static class Organization
    {
        #region Properties

        /// <summary>
        /// All TV Shows contained within all TV folders. 
        /// </summary>
        public static ContentCollection Shows = new ContentCollection(ContentType.TvShow, "Shows");    

        /// <summary>
        /// All movies contained within all movie folders.
        /// </summary>
        public static ContentCollection Movies = new ContentCollection(ContentType.Movie, "Movies");

        /// <summary>
        /// Contains log of organization actions.
        /// </summary>
        public static ObservableCollection<OrgItem> ActionLog = new ObservableCollection<OrgItem>();

        /// <summary>
        /// Lock for accessing ActionLog
        /// </summary>
        public static object ActionLogLock = new object();

        /// <summary>
        /// Contains previously suggested actions for directory scan.
        /// </summary>
        public static ObservableCollection<OrgItem> DirScanLog = new ObservableCollection<OrgItem>();

        /// <summary>
        /// Lock for accessing DirScanLog
        /// </summary>
        public static object DirScanLogLock = new object();

        /// <summary>
        /// All available TV genres
        /// </summary>
        public static GenreCollection AllTvGenres = new GenreCollection(GenreCollection.CollectionType.Global | GenreCollection.CollectionType.Tv);

        /// <summary>
        /// All available movie genres
        /// </summary>
        public static GenreCollection AllMovieGenres = new GenreCollection(GenreCollection.CollectionType.Global | GenreCollection.CollectionType.Movie);

        #endregion

        #region Load/Save

        #region Variables

        /// <summary>
        /// Lock for accessing log save file
        /// </summary>
        public static object ActionLogFileLock = new object();

        /// <summary>
        /// Action Log XML root element string
        /// </summary>
        private static readonly string ACTION_LOG_XML = "ActionLog";

        /// <summary>
        /// Lock for accessing scan dir log save file
        /// </summary>
        public static object DirScanLogFileLock = new object();

        /// <summary>
        /// Scan dir log XML root element string
        /// </summary>
        private static readonly string DIR_SCAN_LOG_XML = "DirScanLog";

        #endregion

        #region Events

        /// <summary>
        /// Static event that fires when action log loading progress changes
        /// </summary>
        public static event EventHandler<ProgressChangedEventArgs> ActionLogLoadProgressChange;

        /// <summary>
        /// Triggers ActionLogLoadProgressChange event
        /// </summary>
        public static void OnActionLogLoadProgressChange(int percent)
        {
            if (ActionLogLoadProgressChange != null)
                ActionLogLoadProgressChange(null, new ProgressChangedEventArgs(percent, null));
        }

        /// <summary>
        /// Static event that fires when action log loading completes
        /// </summary>
        public static event EventHandler ActionLogLoadComplete;

        /// <summary>
        /// Triggers ActionLogLoadComplete event
        /// </summary>
        public static void OnActionLogLoadComplete()
        {
            if (ActionLogLoadComplete != null)
                ActionLogLoadComplete(null, new EventArgs());
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets base path for storing data on user's PC. Uses application data path.
        /// </summary>
        /// <param name="createIfNeeded">Indicates whether path should be created if it doesn't exist (data is going to be save there)</param>
        /// <returns>Whether path exists (true even if just created)</returns>
        public static string GetBasePath(bool createIfNeeded)
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            basePath = Path.Combine(basePath, "Meticumedia");

#if DEBUG
            basePath = Path.Combine(basePath, "WPFDEBUG");
#endif

            if (createIfNeeded && !Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            return basePath;
        }        

        /// <summary>
        /// Saves all organization data into XML.
        /// </summary>
        public static void Save()
        {
            if (Shows.LoadCompleted)
                Shows.Save();
            if (Movies.LoadCompleted)
                Movies.Save();
            //SaveLog();
        }

        /// <summary>
        /// Saves action log to XML
        /// </summary>
        public static void SaveActionLog()
        {
            // Action Logs
            string path = Path.Combine(GetBasePath(true), ACTION_LOG_XML + ".xml");

            // Save data into temporary file, so that if application crashes in middle of saving XML is not corrupted!
            string tempPath = Path.Combine(Organization.GetBasePath(true), ACTION_LOG_XML + "_TEMP" + Guid.NewGuid().ToString() + ".xml");

            lock (ActionLogLock)
            {
                lock (ActionLogFileLock)
                {
                    using (XmlTextWriter xw = new XmlTextWriter(tempPath, Encoding.ASCII))
                    {
                        xw.Formatting = Formatting.Indented;

                        xw.WriteStartElement(ACTION_LOG_XML);

                        foreach (OrgItem action in ActionLog)
                            action.Save(xw, false);
                        xw.WriteEndElement();
                    }

                    // Delete previous save data
                    if (File.Exists(path))
                        File.Delete(path);

                    // Move tempoarary save file to default
                    File.Move(tempPath, path);
                }
            }
        }

         /// <summary>
        /// Saves directory scan log to XML
        /// </summary>
        public static void SaveDirScanLog()
        {
            // Directory scan Logs
            string path = Path.Combine(GetBasePath(true), DIR_SCAN_LOG_XML + ".xml");

            // Save data into temporary file, so that if application crashes in middle of saving XML is not corrupted!
            string tempPath = Path.Combine(Organization.GetBasePath(true), ACTION_LOG_XML + "_TEMP.xml");

            lock (DirScanLogFileLock)
            {
                using (XmlTextWriter xw = new XmlTextWriter(tempPath, Encoding.ASCII))
                {
                    xw.Formatting = Formatting.Indented;

                    xw.WriteStartElement(DIR_SCAN_LOG_XML);

                    for (int i = 0; i < DirScanLog.Count; i++)
                        DirScanLog[i].Save(xw, true);
                    xw.WriteEndElement();
                }

                // Delete previous save data
                if (File.Exists(path))
                    File.Delete(path);

                // Move tempoarary save file to default
                try
                {
                    File.Move(tempPath, path);
                }
                catch { }
            }
            
        }

        /// <summary>
        /// Loads organization data from XML files.
        /// </summary>
        public static void Load(bool doUpdating)
        {            
            string basePath = GetBasePath(false);
            if (!Directory.Exists(basePath))
                return;

            Organization.DoUpdating = doUpdating;

            // Load TV shows
            LoadShowsAsync();

            // Load Movies
            LoadMoviesAsync();

            // Action Log
            LoadActionLogAsync();

            // Dir scan log
            LoadScanDirLogAsync();
        }

        private static bool DoUpdating = true;

        /// <summary>
        /// Asynchronously load TV shows from XML
        /// </summary>
        private static void LoadShowsAsync()
        {
            BackgroundWorker tvLoadWorker = new BackgroundWorker();
            tvLoadWorker.DoWork += new DoWorkEventHandler(tvLoadWorker_DoWork);
            tvLoadWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(tvLoadWorker_RunWorkerCompleted);
            tvLoadWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Load shows from XML work.
        /// </summary>
        private static void tvLoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Shows.Load(Organization.DoUpdating);
        }

        /// <summary>
        /// On loading complete, update show folders
        /// </summary>
        private static void tvLoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Update TV folder
            if (Organization.DoUpdating)
                UpdateRootFolders(ContentType.TvShow);
        }

        /// <summary>
        /// Asynchronously load movies from XML
        /// </summary>
        private static void LoadMoviesAsync()
        {
            BackgroundWorker movieLoadWorker = new BackgroundWorker();
            movieLoadWorker.DoWork += new DoWorkEventHandler(movieLoadWorker_DoWork);
            movieLoadWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(movieLoadWorker_RunWorkerCompleted);
            movieLoadWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Movie loading work
        /// </summary>
        static void movieLoadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Movies.Load(Organization.DoUpdating);
        }

        /// <summary>
        /// On loading complete, update movie folders
        /// </summary>
        static void movieLoadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(Organization.DoUpdating)
                UpdateRootFolders(ContentType.Movie);
        }

        /// <summary>
        /// Asynchronously load action log from XML
        /// </summary>
        private static void LoadActionLogAsync()
        {
            BackgroundWorker actionLogLoadWorker = new BackgroundWorker();
            actionLogLoadWorker.DoWork += new DoWorkEventHandler(LoadActionLog);
            actionLogLoadWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Action log loading work
        /// </summary>
        private static void LoadActionLog(object sender, DoWorkEventArgs e)
        {
            try
            {
                string path = Path.Combine(GetBasePath(false), ACTION_LOG_XML + ".xml");
                if (File.Exists(path))
                    lock (ActionLogFileLock)
                    {
                        // Load XML
                        XmlTextReader reader = new XmlTextReader(path);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(reader);

                        // Load movies data
                        List<OrgItem> loadActionLog = new List<OrgItem>();
                        XmlNodeList logNodes = xmlDoc.DocumentElement.ChildNodes;
                        for (int i = 0; i < logNodes.Count; i++)
                        {
                            OnActionLogLoadProgressChange((int)(((double)i / logNodes.Count) * 100));

                            OrgItem item = new OrgItem();
                            if (item.Load(logNodes[i]))
                                loadActionLog.Add(item);
                        }

                        reader.Close();
                        OnActionLogLoadProgressChange(100);

                        lock (ActionLogLock)
                        {
                            ActionLog.Clear();
                            foreach (OrgItem item in loadActionLog)
                                ActionLog.Add(item);
                        }
                    }
            }
            catch { }

            OnActionLogLoadComplete();
        }

                /// <summary>
        /// Asynchronously load action log from XML
        /// </summary>
        private static void LoadScanDirLogAsync()
        {
            BackgroundWorker scanDirLogLoadWorker = new BackgroundWorker();
            scanDirLogLoadWorker.DoWork += new DoWorkEventHandler(LoadScanDirLog);
            scanDirLogLoadWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Action log loading work
        /// </summary>
        public static void LoadScanDirLog(object sender, DoWorkEventArgs e)
        {
            try
            {
                string path = Path.Combine(GetBasePath(false), DIR_SCAN_LOG_XML + ".xml");
                List<OrgItem> loadScanDirLog = new List<OrgItem>();
                if (File.Exists(path))
                {
                    lock (DirScanLogFileLock)
                    {
                        // Load XML
                        XmlTextReader reader = new XmlTextReader(path);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(reader);

                        // Load movies data

                        XmlNodeList logNodes = xmlDoc.DocumentElement.ChildNodes;
                        for (int i = 0; i < logNodes.Count; i++)
                        {
                            OrgItem item = new OrgItem();
                            if (item.Load(logNodes[i]))
                            {
                                if (File.Exists(item.SourcePath))
                                {
                                    for (int j = 0; j < Settings.ScanDirectories.Count; j++)
                                        if (item.sourcePath.ToLower().Contains(Settings.ScanDirectories[j].FolderPath.ToLower()))
                                        {
                                            loadScanDirLog.Add(item);
                                            break;
                                        }
                                }
                            }
                        }

                        reader.Close();
                    }

                    lock (DirScanLogLock)
                    {
                        DirScanLog.Clear();
                        foreach (OrgItem item in loadScanDirLog)
                            DirScanLog.Add(item);
                    }
                }
                
            }
            catch { }
        }

        #endregion

        #endregion

        #region Root Folder Updating

        #region Events

        /// <summary>
        /// Event indicating there are items to be sent to the queue
        /// </summary>
        public static event EventHandler UpdateCancelled;

        /// <summary>
        /// Triggers ItemsToQueue event
        /// </summary>
        /// <param name="items"></param>
        public static void OnUpdateCancelled(ContentType type)
        {
            if (UpdateCancelled != null)
                UpdateCancelled(type, new EventArgs());
        }

        #endregion

        /// <summary>
        /// Flag indicating TV root folder updating has been cancelled.
        /// </summary>
        private static bool tvUpdateCancelled = false;

        /// <summary>
        /// Flag indicating movie root folder updating has been cancelled.
        /// </summary>
        private static bool movieUpdateCancelled = false;

        /// <summary>
        /// Cancels root folder updating.
        /// </summary>
        public static void CancelFolderUpdating(ContentType contentType)
        {
            SetUpdateCancel(contentType, true);
        }

        /// <summary>
        /// Set update cancel flag value.
        /// </summary>
        /// <param name="contentType">Flag type to set</param>
        /// <param name="cancel">Value to set cancel flag to</param>
        private static void SetUpdateCancel(ContentType contentType, bool cancel)
        {
            switch (contentType)
            {
                case ContentType.Movie:
                    movieUpdateCancelled = cancel;
                    break;
                case ContentType.TvShow:
                    tvUpdateCancelled = cancel;
                    break;
                default:
                    throw new Exception("Unknown content type");
            }

            if (cancel)
                OnUpdateCancelled(contentType);
        }

        /// <summary>
        /// Gets the list of content matching a set of filters that are contained within root folder(s)
        /// </summary>
        /// <param name="folders">Root folders to look for content</param>
        /// <param name="genreFilter">Filter for genre type of movie - use "All" to disable filter</param>
        /// <param name="yearFilter">Enables year filtering</param>
        /// <param name="minYear">Minimum for year filter</param>
        /// <param name="maxYear">Maximum for year filter</param>
        /// <param name="nameFilter">String that must be contained in movie name - empty string disables filter</param>
        /// <returns>List of movies from root folder that match filters</returns>
        public static List<Content> GetContentFromRootFolders(List<ContentRootFolder> folders, bool recursive, bool genreEnable, GenreCollection genreFilter, bool yearFilter, int minYear, int maxYear, string nameFilter)
        {
            // Initialize movies list
            List<Content> folderMovies = new List<Content>();

            // Go through each content folder and get movie from folders that match name
            foreach (ContentRootFolder folder in folders)
                folder.GetContent(recursive, genreEnable, genreFilter, folderMovies, yearFilter, minYear, maxYear, nameFilter);

            // Returns list of movies
            return folderMovies;
        }

        /// <summary>
        /// Get the list of content that is contained within a set of root folders
        /// </summary>
        /// <param name="folders">List of content folders to get movies from</param>
        /// <returns>The list of movies found in root folders</returns>
        public static List<Content> GetContentFromRootFolders(List<ContentRootFolder> folders)
        {
            List<Content> folderContent = new List<Content>();
            foreach (ContentRootFolder folder in folders)
                folder.GetContent(true, false, GetAllGenres(folder.ContentType), folderContent, false, 0, 0, string.Empty);
            return folderContent;
        }

        /// <summary>
        /// Updates content found in root folders.
        /// </summary>
        /// <param name="folders">Root folders to update from</param>
        /// <param name="fastUpdate">Whether to do fast update (skips episodes updating for TV shows)</param> 
        private static void UpdateContentsFromRootFolders(List<ContentRootFolder> folders, bool fastUpdate, ContentType contentType)
        {
            // Update each folder in list
            foreach (ContentRootFolder folder in folders)
            {
                bool results;
                switch (contentType)
                {
                    case ContentType.TvShow:
                        results = folder.UpdateContent(fastUpdate, ref tvUpdateCancelled);
                        break;
                    case ContentType.Movie:
                        results = folder.UpdateContent(fastUpdate, ref movieUpdateCancelled);
                        break;
                    default:
                        throw new Exception("Unknown content type");
                }

                if (!results)
                    break;
            }
            

            // Clear cancel flag
            SetUpdateCancel(contentType, false);            
        }

        /// <summary>
        /// Update root folders
        /// </summary>
        /// <param name="type"></param>
        public static void UpdateRootFolders(ContentType type)
        {
            // Update each selected movie folder
            List<ContentRootFolder> rootFolders = Settings.GetAllRootFolders(type, false);
            Organization.UpdateRootFolderContents(rootFolders, false, type);
        }

        public static void UpdateRootFolderContents(List<ContentRootFolder> folders, bool fastUpdate, ContentType contentType)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            object[] args = new object[] { folders, fastUpdate, contentType };
            worker.RunWorkerAsync(args);
        }

        static void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Save();
        }

        static void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] args = (object[])e.Argument;
            List<ContentRootFolder> folders = (List<ContentRootFolder>)args[0];
            bool fastUpdate = (bool)args[1];
            ContentType contentType = (ContentType)args[2];

            UpdateContentsFromRootFolders(folders, fastUpdate, contentType);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get content collection for specified content type
        /// </summary>
        /// <param name="type">Content type to get</param>
        /// <returns>Resulting content collection</returns>
        public static ContentCollection GetContentCollection(ContentType type)
        {
            switch (type)
            {
                case ContentType.Movie:
                    return Movies;
                case ContentType.TvShow:
                    return Shows;
                default:
                    throw new Exception("Unknown content type");
            }
        }

        /// <summary>
        /// Get genres for specified content type
        /// </summary>
        /// <param name="type">Content type to get genres for</param>
        /// <returns>Resulting content collection</returns>
        public static GenreCollection GetAllGenres(ContentType type)
        {
            GenreCollection allGenres;
            switch (type)
            {
                case ContentType.Movie:
                    allGenres = AllMovieGenres;
                    break;
                case ContentType.TvShow:
                    allGenres = AllTvGenres;
                    break;
                default:
                    throw new Exception("Unknown content type");
            }
            return allGenres;
        }

        #endregion

        #region Log Methods

        /// <summary>
        /// Adds item to log.
        /// </summary>
        /// <param name="item"></param>
        public static void AddActionLogItem(OrgItem item)
        {
            lock (ActionLogLock)
                ActionLog.Insert(0, item);
            SaveActionLog();
        }

        /// <summary>
        /// Removes item from log.
        /// </summary>
        /// <param name="index"></param>
        public static void RemoveActionLogItem(int index)
        {
            lock (ActionLogLock)
                ActionLog.RemoveAt(index);
            SaveActionLog();
        }

        /// <summary>
        /// Adds item to log.
        /// </summary>
        /// <param name="item"></param>
        public static void AddDirScanLogItem(OrgItem item)
        {
            lock (DirScanLogLock)
                DirScanLog.Insert(0, item);
            SaveDirScanLog();
        }

        /// <summary>
        /// Removes item from log.
        /// </summary>
        /// <param name="index"></param>
        public static void RemoveDirScanLogItem(int index)
        {
            lock (DirScanLogLock)
                DirScanLog.RemoveAt(index);
            SaveDirScanLog();
        }

        #endregion
    }
}