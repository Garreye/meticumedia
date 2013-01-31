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
        /// TV Shows contained within TV folders. 
        /// </summary>
        public static TvShowCollection Shows = new TvShowCollection();

        /// <summary>
        /// Lock for accessing Movies
        /// </summary>
        public static object ShowsLock = new object();        

        /// <summary>
        /// List of movies contained within movie folders.
        /// </summary>
        public static List<Movie> Movies = new List<Movie>();

        /// <summary>
        /// Lock for accessing Movies
        /// </summary>
        public static object MoviesLock = new object();

        /// <summary>
        /// Contains log data for organization actions.
        /// </summary>
        public static List<OrgItem> ActionLog = new List<OrgItem>();

        /// <summary>
        /// Lock for accessing Action Log
        /// </summary>
        public static object ActionLogLock = new object();

        #endregion

        #region Load/Save

        #region Variables

        /// <summary>
        /// Lock for accessing show save file
        /// </summary>
        public static object ShowsFileLock = new object();

        /// <summary>
        /// Lock for accessing movie save file
        /// </summary>
        public static object MoviesFileLock = new object();

        /// <summary>
        /// Lock for accessing log save file
        /// </summary>
        public static object ActionLogFileLock = new object();

        /// <summary>
        /// TV shows XML root element string
        /// </summary>
        private static readonly string SHOWS_XML = "TvShows";

        /// <summary>
        /// Movies XML root element string
        /// </summary>
        private static readonly string MOVIES_XML = "Movies";

        /// <summary>
        /// Action Log XML root element string
        /// </summary>
        private static readonly string ACTION_LOG_XML = "ActionLog";

        #endregion

        #region Events

        /// <summary>
        /// Static event that fires when show loading progress changes
        /// </summary>
        public static event EventHandler<ProgressChangedEventArgs> TvShowLoadProgressChange;

        /// <summary>
        /// Triggers TvShowLoadProgressChange event
        /// </summary>
        public static void OnTvShowLoadProgressChange(int percent)
        {
            if (TvShowLoadProgressChange != null)
                TvShowLoadProgressChange(null, new ProgressChangedEventArgs(percent, null));
        }

        /// <summary>
        /// Static event that fires when show loading completes
        /// </summary>
        public static event EventHandler TvShowLoadComplete;

        /// <summary>
        /// Triggers TvShowLoadComplete event
        /// </summary>
        public static void OnTvShowLoadComplete()
        {
            if (TvShowLoadComplete != null)
                TvShowLoadComplete(null, new EventArgs());
        }

        /// <summary>
        /// Static event that fires when movie loading progress changes
        /// </summary>
        public static event EventHandler<ProgressChangedEventArgs> MovieLoadProgressChange;

        /// <summary>
        /// Triggers MovieLoadProgressChange event
        /// </summary>
        public static void OnMovieLoadProgressChange(int percent)
        {
            if (MovieLoadProgressChange != null)
                MovieLoadProgressChange(null, new ProgressChangedEventArgs(percent, null));
        }

        /// <summary>
        /// Static event that fires when show loading completes
        /// </summary>
        public static event EventHandler MovieLoadComplete;

        /// <summary>
        /// Triggers MovieLoadComplete event
        /// </summary>
        public static void OnMovieLoadComplete()
        {
            if (MovieLoadComplete != null)
                MovieLoadComplete(null, new EventArgs());
        }

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
        /// Static event that fires when show loading completes
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
        /// <param name="createIfNeeded"></param>
        /// <returns></returns>
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
            SaveShows();
            SaveMovies();
            SaveLog();
        }

        /// <summary>
        /// Saves shows to XML
        /// </summary>
        public static void SaveShows()
        {
            // TV Shows
            string path = Path.Combine(GetBasePath(true), SHOWS_XML + ".xml");
            lock (ShowsLock)
            {
                lock (ShowsFileLock)
                    using (XmlWriter xw = XmlWriter.Create(path))
                    {
                        xw.WriteStartElement(SHOWS_XML);

                        xw.WriteElementString("LastUpdate", Shows.LastUpdate);

                        foreach (TvShow show in Shows)
                            show.Save(xw);
                        xw.WriteEndElement();
                    }
            }
        }

        /// <summary>
        /// Saves movies to XML
        /// </summary>
        public static void SaveMovies()
        {
            // Movies
            string path = Path.Combine(GetBasePath(true), MOVIES_XML + ".xml");
            lock (MoviesLock)
            {
                lock (MoviesFileLock)
                    using (XmlWriter xw = XmlWriter.Create(path))
                    {
                        xw.WriteStartElement(MOVIES_XML);
                        foreach (Movie movie in Movies)
                            movie.Save(xw);
                        xw.WriteEndElement();
                    }
            }
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
            try
            {
                string path = Path.Combine(GetBasePath(false), SHOWS_XML + ".xml");
                if (File.Exists(path))
                    lock (ShowsFileLock)
                    {

                        // Load XML
                        XmlTextReader reader = new XmlTextReader(path);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(reader);

                        // Load show data
                        TvShowCollection loadShows = new TvShowCollection();
                        XmlNodeList showNodes = xmlDoc.DocumentElement.ChildNodes;
                        for (int i = 0; i < showNodes.Count; i++)
                        {
                            OnTvShowLoadProgressChange((int)(((double)i / showNodes.Count) * 100));


                            if (showNodes[i].Name == "LastUpdate")
                                loadShows.LastUpdate = showNodes[i].InnerText;
                            else
                            {
                                TvShow show = new TvShow();
                                if (show.Load(showNodes[i]))
                                    loadShows.Add(show);
                            }
                        }

                        reader.Close();
                        OnTvShowLoadProgressChange(100);
                        lock (ShowsLock)
                            Shows = loadShows;
                    }
            }
            catch { }

            ScanHelper.UpdateScanDirTvItems();
            OnTvShowLoadComplete();
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
            try
            {
                string path = Path.Combine(GetBasePath(false), MOVIES_XML + ".xml");
                if (File.Exists(path))
                    lock (MoviesFileLock)
                    {

                        // Load XML
                        XmlTextReader reader = new XmlTextReader(path);
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(reader);

                        // Load movies data
                        List<Movie> loadMovies = new List<Movie>();
                        XmlNodeList movieNodes = xmlDoc.DocumentElement.ChildNodes;
                        for (int i = 0; i < movieNodes.Count; i++)
                        {
                            OnMovieLoadProgressChange((int)(((double)i / movieNodes.Count) * 100));

                            Movie movie = new Movie();
                            if (movie.Load(movieNodes[i]))
                                loadMovies.Add(movie);
                        }

                        reader.Close();
                        OnMovieLoadProgressChange(100);

                        lock (MoviesLock)
                            Movies = loadMovies;
                    }
            }
            catch { }

            OnMovieLoadComplete();
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

        #region TV Methods

        #region Add, Remove, Sort

        /// <summary>
        /// Add show to show list
        /// </summary>
        /// <param name="newShow">Show instance to add to list</param>
        public static void AddShow(TvShow newShow)
        {
            // Add show to list and sort it
            lock (ShowsLock)
            {
                Shows.Add(newShow);
                SaveShows();
            }
        }

        /// <summary>
        /// Removes a show from list of shows.
        /// </summary>
        /// <param name="show">Show instance to remove</param>
        public static void RemoveShow(TvShow show)
        {
            lock (ShowsLock)
                Shows.Remove(show);
        }

        /// <summary>
        /// Remove show from lis of shows.
        /// </summary>
        /// <param name="index">Index of show in list to remove</param>
        public static void RemoveShowAt(int index)
        {
            lock (ShowsLock)
                Shows.RemoveAt(index);
        }

        /// <summary>
        /// Sort shows list.
        /// </summary>
        public static void SortShows()
        {
            lock (ShowsLock)
                Shows.Sort();
        }

        #endregion

        #region Folder Methods

        /// <summary>
        /// Get TV shows that are contained in root folders matching a filter string.
        /// </summary>
        /// <param name="rootFolderFilter">The content folder filter string</param>
        /// <returns>List of shows containted in folder</returns>
        public static List<Content> GetShowsFromRootFolders(string rootFolderFilter, string nameFilter)
        {
            // Initialize shows
            List<Content> folderShows = new List<Content>();

            // Add shows from all folders matching folder filter
            foreach (ContentRootFolder folder in Settings.TvFolders)
                if (rootFolderFilter.StartsWith("All") || folder.FullPath == rootFolderFilter)
                    GetShowsFromRootFolder(folder, folderShows, nameFilter);

            // Return list
            return folderShows;
        }

        /// <summary>
        /// Gets TV shows that are contained within a content folder
        /// </summary>
        /// <param name="folder">The content folder to get shows from</param>
        /// <param name="folderShows">The list to add shows to</param>
        private static void GetShowsFromRootFolder(ContentRootFolder folder, List<Content> folderShows, string nameFilter)
        {
            // Go through shows and add any shows that within the content folder and scan directory
            for (int i = 0; i < Shows.Count; i++)
            {
                // Apply name filter
                bool nameMatch = string.IsNullOrEmpty(nameFilter) || Shows[i].Name.ToLower().Contains(nameFilter.ToLower());

                if (RootFolderContainsShow(Shows[i], folder) && nameMatch)
                {
                    //Shows[i].UpdateMissing();
                    folderShows.Add(Shows[i]);
                }
            }
        }

        /// <summary>
        /// Check if a root folder contains a specific TV show. Called
        /// recursively on sub-content folders. 
        /// </summary>
        /// <param name="show">Name of TV show</param>
        /// <param name="folder">Root folder to search in</param>
        /// <returns>Whether folder contains show</returns>
        private static bool RootFolderContainsShow(TvShow show, ContentRootFolder folder)
        {
            // Check if show content folder matches
            if (show.RootFolder == folder.FullPath)
                return true;
            else
                // Recursion of sub-folders
                foreach (ContentRootFolder subFolder in folder.ChildFolders)
                    if (RootFolderContainsShow(show, subFolder))
                        return true;

            // No match
            return false;
        }

        /// <summary>
        /// Get a lists of shows that have the include in scan property enabled.
        /// </summary>
        /// <returns>List of show that are included in scanning</returns>
        public static List<TvShow> GetScannableShows(bool updateMissing)
        {
            List<TvShow> includeShow = new List<TvShow>();
            for (int i = 0; i < Shows.Count; i++)
                if (Shows[i].IncludeInScan && !string.IsNullOrEmpty(Shows[i].Name))
                {
                    if (updateMissing)
                        Shows[i].UpdateMissing();
                    includeShow.Add(Shows[i]);
                }

            return includeShow;
        }

        #endregion

        #region Folder Updating

        /// <summary>
        /// Static event that fires when movie folder updating progress changes
        /// </summary>
        public static event EventHandler<OrgProgressChangesEventArgs> TvFolderUpdateProgressChange;

        /// <summary>
        /// Triggers TvFolderUpdateProgressChange event
        /// </summary>
        public static void OnTvFolderUpdateProgressChange(bool newShow, int percent, string msg)
        {
            if (TvFolderUpdateProgressChange != null)
                TvFolderUpdateProgressChange(null, new OrgProgressChangesEventArgs(newShow, percent, msg));
        }

        /// <summary>
        /// Flag indicating TV folder updating has been cancelled.
        /// </summary>
        private static bool tvUpdateCancelled = false;

        /// <summary>
        /// Updates TV Root folders.
        /// </summary>
        /// <param name="folder">Name of root folder to update - set to "All" to update all TV root folders</param>
        /// <param name="fastUpdate">Whether to do fast update (skips updating episode info for existing shows)</param> 
        public static void UpdateTvRootFolders(string folder, bool fastUpdate)
        {
            // Get episodes that need updating
            List<int> seriesIds; string time = string.Empty;
            TvDatabaseHelper.GetDataToBeUpdated(out seriesIds, out time);

            // Update folders that match folder string
            foreach (ContentRootFolder tvFolder in Settings.TvFolders)
                if (folder.StartsWith("All") || tvFolder.FullPath == folder)
                {
                    OnTvFolderUpdateProgressChange(false, 0, "Updating '" + tvFolder.FullPath + "' - Gathering Files");
                    if (!Organization.UpdateTvRootFolder(tvFolder, fastUpdate, seriesIds))
                        break;
                }

            // Clear cancel flag
            tvUpdateCancelled = false;

            // Save show changes
            if (!string.IsNullOrEmpty(time))
                Shows.LastUpdate = time;
            SortShows();
            SaveShows();
        }

        /// <summary>
        /// List of TV Show database IDs that require updating.
        /// </summary>
        private static List<int> seriesIdsToUpdate;

        /// <summary>
        /// Current TV update number identifier.
        /// </summary>
        private static int tvUpdateNumber = 0;

        /// <summary>
        /// Search through all sub-folders of a root folder and attempts
        /// to assign each one to a TV show.
        /// </summary>
        /// <param name="tvFolder">Root folder to update</param>
        /// <param name="fastUpdate">Whether update is fast (skips updating episode info for existing shows)</param>
        /// <param name="idsToUpdate">List of show IDs that need updating</param>
        /// <returns>Whether update was completed without cancelation</returns>
        private static bool UpdateTvRootFolder(ContentRootFolder tvFolder, bool fastUpdate, List<int> idsToUpdate)
        {
            // Set show IDs that need updating - TODO: deprecated, this was from TheTvDb, TvRage doesn't provide this information
            seriesIdsToUpdate = idsToUpdate;

            // Update progress
            string progressMsg = "Updating '" + tvFolder.FullPath + "'";
            OnTvFolderUpdateProgressChange(false, 0, progressMsg);

            // Initialize processing
            OrgProcessing processing = new OrgProcessing(TvUpdateProcess);
            tvUpdateNumber = processing.ProcessNumber;

            // Run processing - Build of sub-dirs is recursive, so all child root folder sub-dirs will be included
            processing.Run(tvFolder.BuildSubDirectories(false, true), ref tvUpdateCancelled, false, false);

            // Remove shows that no longer exists
            lock (ShowsLock)
                for (int i = Shows.Count - 1; i >= 0; i--)
                    if (!Shows[i].Found && Shows[i].RootFolder.StartsWith(tvFolder.FullPath))
                        RemoveShowAt(i);

            // Save movie changes
            Organization.SaveShows();

            // Set progress to completed
            progressMsg = "Updating '" + tvFolder.FullPath + "' complete!";
            OnTvFolderUpdateProgressChange(false, 0, progressMsg);

            // Return whether update was completed without cancelation
            return !tvUpdateCancelled;
        }

        /// <summary>
        /// TV update processing method for single movie path
        /// </summary>
        /// <param name="orgPath">Organization path instance to be processed</param>
        /// <param name="pathNum">The path's number out of total being processed</param>
        /// <param name="totalPaths">Total number of path being processed</param>
        /// <param name="updateNumber">The identifier for the OrgProcessing instance</param>
        /// <param name="background">Whether processing is running as a background operation</param>
        /// <param name="subSearch">Whether processing is sub-search - specific to instance</param>
        /// <param name="processComplete">Delegate to be called by processing when completed</param>
        /// <param name="numItemsProcessed">Number of paths that have been processed - used for progress updates</param>
        private static void TvUpdateProcess(OrgPath orgPath, int pathNum, int totalPaths, int updateNumber, bool background, bool subSearch, OrgProcessing.ProcessComplete complete, ref int numItemsProcessed)
        {
            // Check for cancellation - this method is called from thread pool, so cancellation could have occured by the time this is run
            if (tvUpdateCancelled)
                return;

            // Check if folder already has a match to existing show
            bool showExists = false;
            bool showComplete = false;
            TvShow newShow = null;
            for (int j = 0; j < Shows.Count; j++)
                if (orgPath.Path == Shows[j].Path)
                {
                    showExists = true;
                    Shows[j].Found = true;
                    if (!string.IsNullOrEmpty(Shows[j].Name))
                        showComplete = true;
                    newShow = Shows[j];
                    break;
                }

            // Build progess message
            string progressMsg = "Updating '" + orgPath.RootFolder.FullPath + "' - Processed '" + Path.GetFileName(orgPath.Path) + "'";

            // Check if show found
            if (showExists && showComplete)
            {
                if ((DateTime.Now - newShow.LastUpdated).TotalDays > 7 || seriesIdsToUpdate.Contains(newShow.Id))
                {
                    TvDatabaseHelper.FullShowSeasonsUpdate(newShow);
                    newShow.LastUpdated = DateTime.Now;
                }

                // Update progress
                if (updateNumber == tvUpdateNumber)
                    OnTvFolderUpdateProgressChange(false, (int)Math.Round((double)numItemsProcessed / totalPaths * 100D), progressMsg);

                // Call completed delegate
                complete();
                return;
            }

            // Get match
            TvShow match = SearchHelper.TvShowSearch.PathMatch(orgPath.RootFolder.FullPath, orgPath.Path);

            // Check that current process hasn't been replaced - search can be slow, so update may have been cancelled by the time it gets here
            if (updateNumber == tvUpdateNumber)
            {
                // Update show info
                if (showExists && match != null)
                {
                    newShow.Clone(match);
                    newShow.LastUpdated = DateTime.Now;
                }
                else if (match != null)
                    newShow = match;
                else
                    newShow = new TvShow(string.Empty, 0, 0, orgPath.Path, orgPath.RootFolder.FullPath);
                newShow.Found = true;

                // Add show to list if new
                if (!showExists)
                    AddShow(newShow);

                // Update progress
                OnTvFolderUpdateProgressChange(true, (int)Math.Round((double)numItemsProcessed / totalPaths * 100D), progressMsg);

                // Call completed delegate
                complete();
            }
        }

        /// <summary>
        /// Cancels TV folder updating thread.
        /// </summary>
        public static void CancelTvUpdating()
        {
            tvUpdateCancelled = true;
        }

        #endregion

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

        #region Movie Methods

        #region Add, Remove, Sort

        /// <summary>
        /// Add movie to movie list
        /// </summary>
        /// <param name="newMovie">Show instance to add to list</param>
        public static void AddMovie(Movie newMovie)
        {
            // Add movie to list and save
            lock (MoviesLock)
                Movies.Add(newMovie);
            SaveMovies();
        }


        /// <summary>
        /// Removes a movie from list of movies.
        /// </summary>
        /// <param name="movie">Show instance to remove</param>
        public static void RemoveMovie(Movie movie)
        {
            lock (MoviesLock)
                Movies.Remove(movie);
        }

        /// <summary>
        /// Remove movie from lis of movies.
        /// </summary>
        /// <param name="index">Index of show in list to remove</param>
        public static void RemoveMovieAt(int index)
        {
            lock (MoviesLock)
                Movies.RemoveAt(index);
        }

        /// <summary>
        /// Sort movies list.
        /// </summary>
        public static void SortMovies()
        {
            lock (MoviesLock)
                Movies.Sort();
        }

        /// <summary>
        /// Remove all movies that no longer exist
        /// </summary>
        /// <param name="movieFolder"></param>
        public static void RemoveMissingMovies(ContentRootFolder movieFolder)
        {
            lock (MoviesLock)
                for (int i = Movies.Count - 1; i >= 0; i--)
                    if (!Movies[i].Found && Movies[i].RootFolder.StartsWith(movieFolder.FullPath))
                        Movies.RemoveAt(i);
        }

        #endregion

        #region Folder Methods

        /// <summary>
        /// Gets the list of movies matching a set of filters that are contained within movie root folder(s)
        /// </summary>
        /// <param name="contentFolderName">Name of root folder to get movies from - use "All" for all movie root folders</param>
        /// <param name="genreFilter">Filter for genre type of movie - use "All" to disable filter</param>
        /// <param name="yearFilter">Enables year filtering</param>
        /// <param name="minYear">Minimum for year filter</param>
        /// <param name="maxYear">Maximum for year filter</param>
        /// <param name="nameFilter">String that must be contained in movie name - empty string disables filter</param>
        /// <returns>List of movies from root folder that match filters</returns>
        public static List<Content> GetMoviesFromRootFolders(string contentFolderName, string genreFilter, bool yearFilter, int minYear, int maxYear, string nameFilter)
        {
            // Initialize movies list
            List<Content> folderMovies = new List<Content>();

            // Go through each content folder and get movie from folders that match name
            foreach (ContentRootFolder folder in Settings.MovieFolders)
                if (contentFolderName.StartsWith("All") || folder.FullPath == contentFolderName)
                    GetMoviesFromRootFolders(folder, genreFilter, folderMovies, yearFilter, minYear, maxYear, nameFilter);

            // Returns list of movies
            return folderMovies;
        }

        /// <summary>
        /// Get the list of movies that are contained within a set of root folders
        /// </summary>
        /// <param name="folders">List of content folders to get movies from</param>
        /// <returns>The list of movies found in root folders</returns>
        public static List<Content> GetMoviesFromRootFolders(List<ContentRootFolder> folders)
        {
            List<Content> folderMovies = new List<Content>();
            foreach (ContentRootFolder folder in folders)
                GetMoviesFromRootFolders(folder, "All", folderMovies, false, 0, 0, string.Empty);
            return folderMovies;
        }
        
        /// <summary>
        /// Builds a list of movies matching filter that are contained within a root folder
        /// </summary>
        /// <param name="folder">Root folder to get movies from</param>
        /// <param name="genre">Filter for genre type of movie - use "All" to disable filter</param>
        /// <param name="folderMovies">List to add movies to</param>
        /// <param name="yearFilter">Enables year filtering</param>
        /// <param name="minYear">Minimum for year filter</param>
        /// <param name="maxYear">Maximum for year filter</param>
        /// <param name="nameFilter">String that must be contained in movie name - empty string disables filter</param>
        private static void GetMoviesFromRootFolders(ContentRootFolder folder, string genre, List<Content> folderMovies, bool yearFilter, int minYear, int maxYear, string nameFilter)
        {
            // Go through all movies
            lock (MoviesLock)
                foreach (Movie movie in Movies)
                {
                    // Apply genre filter
                    bool genreMatch = genre.StartsWith("All");
                    if (movie.Genres != null && !genreMatch)
                        foreach (string movieGenre in movie.Genres)
                            if (movieGenre == genre)
                            {
                                genreMatch = true;
                                break;
                            }

                    // Apply year filter
                    bool yearMatch = !yearFilter || (movie.Date.Year >= minYear && movie.Date.Year <= maxYear);

                    // Apply text filter
                    bool nameMatch = string.IsNullOrEmpty(nameFilter) || movie.Name.ToLower().Contains(nameFilter.ToLower());

                    // Check if movie is in the folder
                    if (RootFolderContainsMovie(movie, folder) && genreMatch && yearMatch && nameMatch)
                        folderMovies.Add(movie);
                }
        }

        /// <summary>
        /// Check if a root folder contains a specific movie. Called
        /// recursively on child root folders.
        /// </summary>
        /// <param name="movie">The movie to check for</param>
        /// <param name="folder">The folder to check if the movie is in</param>
        /// <returns>Whether the movie is contained in the folder</returns>
        private static bool RootFolderContainsMovie(Movie movie, ContentRootFolder folder)
        {            
            // Check if movie content folder matches
            if (movie.RootFolder == folder.FullPath)
                return true;
            else
                // Recursion on sub-folders
                foreach (ContentRootFolder subFolder in folder.ChildFolders)
                    if (RootFolderContainsMovie(movie, subFolder))
                        return true;

            // No match
            return false;
        }

        #endregion

        #region Folders Updating

        /// <summary>
        /// Static event that fires when movie folder updating progress changes
        /// </summary>
        public static event EventHandler<OrgProgressChangesEventArgs> MovieFolderUpdateProgressChange;

        /// <summary>
        /// Orginzation updating progress changed event arguments
        /// </summary>
        public class OrgProgressChangesEventArgs : ProgressChangedEventArgs
        {
            /// <summary>
            /// Whether last updated item was new
            /// </summary>
            public bool NewItem { get; set; }

            /// <summary>
            /// Constructor with know properties
            /// </summary>
            /// <param name="newItem">Whether new item was found</param>
            /// <param name="percent">Progress percent</param>
            /// <param name="msg">Progress message</param>
            public OrgProgressChangesEventArgs(bool newItem, int percent, string msg) : base(percent, msg)
            {
                this.NewItem = newItem;
            }
        }

        /// <summary>
        /// Triggers MovieFolderUpdateProgressChange event
        /// </summary>
        public static void OnMovieFolderUpdateProgressChange(bool newMovie, int percent, string msg)
        {
            if (MovieFolderUpdateProgressChange != null)
                MovieFolderUpdateProgressChange(null, new OrgProgressChangesEventArgs(newMovie, percent, msg));
        }

        /// <summary>
        /// Flag indicating movie folder updating should be cancelled.
        /// </summary>
        private static bool movieUpdateCancelled = false;

        /// <summary>
        /// Current Movie update number identifier
        /// </summary>
        private static int movieUpdateNumber = 0;

        /// <summary>
        /// Cancels movie folder updating (if any).
        /// </summary>
        public static void CancelMovieUpdating()
        {
            movieUpdateCancelled = true;
        }

        /// <summary>
        /// Searches through a movie folder and attempts to match sub-folders names to 
        /// a movie in online database. Recursively runs this method on sub-movie folders
        /// of movie folder.
        /// </summary>
        /// <param name="movieFolder">Folder where movie is located</param>
        public static bool UpdateMovieFolder(ContentRootFolder movieFolder)
        {
            string progressMsg = "Updating '" + movieFolder.FullPath + "' - Gathering Files";
            OnMovieFolderUpdateProgressChange(false, 0, progressMsg);
            
            OrgProcessing processing = new OrgProcessing(MovieUpdateProcess);
            movieUpdateNumber = processing.ProcessNumber;
            
            processing.Run(movieFolder.BuildSubDirectories(true, false), ref movieUpdateCancelled, false, false);

            // Remove movies that are missing (Found property is false)
            Organization.RemoveMissingMovies(movieFolder);

            // Save movie changes
            Organization.SaveMovies();

            progressMsg = "Updating '" + movieFolder.FullPath + "' complete!";
            OnMovieFolderUpdateProgressChange(false, 0, progressMsg);

            return !movieUpdateCancelled;
        }

        /// <summary>
        /// Movie update processing method for single movie path
        /// </summary>
        /// <param name="orgPath">Organization path instance to be processed</param>
        /// <param name="pathNum">The path's number out of total being processed</param>
        /// <param name="totalPaths">Total number of path being processed</param>
        /// <param name="updateNumber">The identifier for the OrgProcessing instance</param>
        /// <param name="background">Whether processing is running as a background operation</param>
        /// <param name="subSearch">Whether processing is sub-search - specific to instance</param>
        /// <param name="processComplete">Delegate to be called by processing when completed</param>
        /// <param name="numItemsProcessed">Number of paths that have been processed - used for progress updates</param>
        private static void MovieUpdateProcess(OrgPath orgPath, int pathNum, int totalPaths, int updateNumber, bool background, bool subSearch, OrgProcessing.ProcessComplete processComplete, ref int numItemsProcessed)
        {
            // Check for cancellation - this method is called from thread pool, so cancellation could have occured by the time this is run
            if (movieUpdateCancelled)
               return;

            // Check if movie already has a match to existing movie
            bool movieExists = false;
            bool movieComplete = false;
            Movie newMovie = null;
            for (int j = 0; j < Organization.Movies.Count; j++)
                if (orgPath.Path == Organization.Movies[j].Path)
                {
                    movieExists = true;
                    Organization.Movies[j].Found = true;
                    if (!string.IsNullOrEmpty(Organization.Movies[j].Name) && Organization.Movies[j].Id != 0)
                        movieComplete = true;
                    else
                        newMovie = Organization.Movies[j];
                    break;
                }

            // Build progress message
            string progressMsg = "Updating '" + orgPath.RootFolder.FullPath + "' - Processed '" + Path.GetFileName(orgPath.Path) + "'";

            // Movie found, next!
            if (movieExists && movieComplete)
            {
                // Update progress
                if (updateNumber == movieUpdateNumber)
                    OnMovieFolderUpdateProgressChange(false, (int)Math.Round((double)numItemsProcessed / totalPaths * 100D), progressMsg);

                // Call complete delegate
                processComplete();
                return;
            }

            // Get movie info
            Movie match = SearchHelper.MovieSearch.PathMatch(orgPath.RootFolder.FullPath, orgPath.Path);

            // Check that current process hasn't been replaced - search can be slow, so update may have been cancelled by the time it gets here
            if (updateNumber == movieUpdateNumber)
            {
                // Update movie info if needed
                if (movieExists)
                    newMovie.UpdateInfo(match);
                else
                    newMovie = match;
                newMovie.Found = true;

                // Add movie to list if new
                if (!movieExists)
                    Organization.AddMovie(newMovie);

                // Update progress
                OnMovieFolderUpdateProgressChange(true, (int)Math.Round((double)numItemsProcessed / totalPaths * 100D), progressMsg);

                // Call complete delegate
                processComplete();
            }
        }

        #endregion

        #endregion
    }
}