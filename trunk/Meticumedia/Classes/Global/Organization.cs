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

namespace Meticumedia
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
        public static ContentCollection Shows = new ContentCollection(ContentType.TvShow);     

        /// <summary>
        /// All movies contained within all movie folders.
        /// </summary>
        public static ContentCollection Movies = new ContentCollection(ContentType.Movie);

        /// <summary>
        /// Contains log of organization actions.
        /// </summary>
        public static List<OrgItem> ActionLog = new List<OrgItem>();

        /// <summary>
        /// Lock for accessing ActionLog
        /// </summary>
        public static object ActionLogLock = new object();

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

            if (createIfNeeded && !Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);

            return basePath;
        }        

        /// <summary>
        /// Saves all organization data into XML.
        /// </summary>
        public static void Save()
        {
            Shows.Save();
            Movies.Save();
            SaveLog();
        }

        /// <summary>
        /// Saves action log to XML
        /// </summary>
        public static void SaveLog()
        {
            // Action Logs
            string path = Path.Combine(GetBasePath(true), ACTION_LOG_XML + ".xml");
            lock (ActionLogLock)
            {
                lock (ActionLogFileLock)
                    using (XmlWriter xw = XmlWriter.Create(path))
                    {
                        xw.WriteStartElement(ACTION_LOG_XML);

                        foreach (OrgItem action in ActionLog)
                            action.Save(xw);
                        xw.WriteEndElement();
                    }
            }
        }

        /// <summary>
        /// Loads organization data from XML files.
        /// </summary>
        public static void Load()
        {            
            string basePath = GetBasePath(false);
            if (!Directory.Exists(basePath))
                return;

            // Load TV shows
            LoadShowsAsync();

            // Load Movies
            LoadMoviesAsync();

            // Action Log
            LoadActionLogAsync();
        }

        /// <summary>
        /// Asynchronously load TV shows from XML
        /// </summary>
        private static void LoadShowsAsync()
        {
            BackgroundWorker tvLoadWorker = new BackgroundWorker();
            tvLoadWorker.DoWork += new DoWorkEventHandler(LoadShows);
            tvLoadWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Load shows from XML work.
        /// </summary>
        private static void LoadShows(object sender, DoWorkEventArgs e)
        {
            Shows.Load();
        }

        /// <summary>
        /// Asynchronously load movies from XML
        /// </summary>
        private static void LoadMoviesAsync()
        {
            BackgroundWorker movieLoadWorker = new BackgroundWorker();
            movieLoadWorker.DoWork += new DoWorkEventHandler(LoadMovies);
            movieLoadWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Movie loading work
        /// </summary>
        private static void LoadMovies(object sender, DoWorkEventArgs e)
        {
            Movies.Load();
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
                            ActionLog = loadActionLog;
                    }
            }
            catch { }

            OnActionLogLoadComplete();
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
        public static List<Content> GetContentFromRootFolders(List<ContentRootFolder> folders, bool recursive, GenreCollection genreFilter, bool yearFilter, int minYear, int maxYear, string nameFilter)
        {
            // Initialize movies list
            List<Content> folderMovies = new List<Content>();

            // Go through each content folder and get movie from folders that match name
            foreach (ContentRootFolder folder in folders)
                folder.GetContent(recursive, genreFilter, folderMovies, yearFilter, minYear, maxYear, nameFilter);

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
                folder.GetContent(true, GetAllGenres(folder.ContentType), folderContent, false, 0, 0, string.Empty);
            return folderContent;
        }

        /// <summary>
        /// Updates content found in root folders.
        /// </summary>
        /// <param name="folders">Root folders to update from</param>
        /// <param name="fastUpdate">Whether to do fast update (skips episodes updating for TV shows)</param> 
        public static void UpdateContentsFromRootFolders(List<ContentRootFolder> folders, bool fastUpdate)
        {
            // Check that there's root folders to update
            if (folders.Count == 0)
                return;

            // Get content type
            ContentType contentType = folders[0].ContentType;
            
            // Get IDs for content that need updating from database (TV only)
            List<int> seriesIds = new List<int>();
            string time = string.Empty;
            if (contentType == ContentType.TvShow)
                TvDatabaseHelper.GetDataToBeUpdated(out seriesIds, out time);

            // Update each folder in list
            foreach (ContentRootFolder folder in folders)
            {
                bool results;
                switch (contentType)
                {
                    case ContentType.TvShow:
                        results = folder.UpdateContent(fastUpdate, seriesIds, ref tvUpdateCancelled);
                        break;
                    case ContentType.Movie:
                        results = folder.UpdateContent(fastUpdate, seriesIds, ref movieUpdateCancelled);
                        break;
                    default:
                        throw new Exception("Unknown content type");
                }

                if (!results)
                    break;
            }

            // Clear cancel flag
            SetUpdateCancel(contentType, false);

            // Save changes
            ContentCollection collection = GetContentCollection(contentType);
            collection.Sort();
            collection.Save();
            if (!string.IsNullOrEmpty(time))
                collection.LastUpdate = time;
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
        public static void AddLogItem(OrgItem item)
        {
            lock (ActionLogLock)
                ActionLog.Insert(0, item);
            SaveLog();
        }

        /// <summary>
        /// Removes item from log.
        /// </summary>
        /// <param name="index"></param>
        public static void RemoveLogItem(int index)
        {
            lock (ActionLogLock)
                ActionLog.RemoveAt(index);
            SaveLog();
        }

        #endregion
    }
}