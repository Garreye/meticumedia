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

        public static object ShowsFileLock = new object();

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
        /// Lock for accessing Movies
        /// </summary>
        public static object ActionLogLock = new object();

        #endregion

        #region Load/Save

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
        /// TV shows XML root string
        /// </summary>
        private static readonly string SHOWS_XML = "TvShows";

        /// <summary>
        /// Movies XML root string
        /// </summary>
        private static readonly string MOVIES_XML = "Movies";

        /// <summary>
        /// Action Log XML root string
        /// </summary>
        private static readonly string ACTION_LOG_XML = "ActionLog";

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
        /// Saves shows to XML.
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
        /// Saves movies to XML.
        /// </summary>
        public static void SaveMovies()
        {
            // Movies
            string path = Path.Combine(GetBasePath(true), MOVIES_XML + ".xml");
            lock (MoviesLock)
            {
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
            lock (ActionLog)
            {
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

                    lock (ActionLog)
                        ActionLog = loadActionLog;
                }
            }
            catch { }

            OnActionLogLoadComplete();
        }

        #endregion

        #region TV

        #region Add, Remove, Sort

        /// <summary>
        /// Add show to show list
        /// </summary>
        /// <param name="newShow">Show instance to add to list</param>
        public static void AddShow(TvShow newShow)
        {
            // Update season/episode info for show
            //TvDatabaseHelper.FullShowSeasonsUpdate(newShow);

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
            {
                Shows.Remove(show);
            }
        }

        /// <summary>
        /// Remove show from lis of shows.
        /// </summary>
        /// <param name="index">Index of show in list to remove</param>
        public static void RemoveShowAt(int index)
        {
            lock (ShowsLock)
            {
                Shows.RemoveAt(index);
            }
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
        /// Get TV shows that are contained in any content folders matching a filter string.
        /// </summary>
        /// <param name="contentFolderFilter">The content folder filter string</param>
        /// <returns>List of shows containted in folder</returns>
        public static List<Content> GetShowsFromFolders(string contentFolderFilter, string nameFilter)
        {
            // Initialize shows
            List<Content> folderShows = new List<Content>();

            // Add shows from all folders matching folder filter
            foreach (ContentRootFolder folder in Settings.TvFolders)
                if (contentFolderFilter.StartsWith("All") || folder.FullPath == contentFolderFilter)
                    GetShowsFromFolders(folder, folderShows, nameFilter);

            // Return list
            return folderShows;
        }

        /// <summary>
        /// Gets TV shows that are contained within a content folder
        /// </summary>
        /// <param name="folder">The content folder to get shows from</param>
        /// <param name="folderShows">The list to add shows to</param>
        private static void GetShowsFromFolders(ContentRootFolder folder, List<Content> folderShows, string nameFilter)
        {
            // Go through shows and add any shows that within the content folder and scan directory
            for (int i = 0; i < Shows.Count; i++)
            {
                // Apply name filter
                bool nameMatch = string.IsNullOrEmpty(nameFilter) || Shows[i].Name.ToLower().Contains(nameFilter.ToLower());

                if (FolderContainsShow(Shows[i], folder) && nameMatch)
                {
                    //Shows[i].UpdateMissing();
                    folderShows.Add(Shows[i]);
                }
            }
        }

        /// <summary>
        /// Check if a content folder contains a specific TV show. Called
        /// recursively on sub-content folders. 
        /// </summary>
        /// <param name="show"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        private static bool FolderContainsShow(TvShow show, ContentRootFolder folder)
        {
            // Check if show content folder matches
            if (show.RootFolder == folder.FullPath)
                return true;
            else
                // Recursion of sub-folders
                foreach (ContentRootFolder subFolder in folder.ChildFolders)
                    if (FolderContainsShow(show, subFolder))
                        return true;

            // No match
            return false;
        }

        /// <summary>
        /// Get a lists of shows that have the include in scan property enabled.
        /// </summary>
        /// <returns></returns>
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
        /// Triggers MovieFolderUpdateProgressChange event
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
        /// Update tv folders.
        /// </summary>
        /// <param name="folder"></param>
        public static void UpdateTvFolders(string folder, bool fastUpdate)
        {
            // Get episodes that need updating
            List<int> seriesIds; string time = string.Empty;
            TvDatabaseHelper.GetDataToBeUpdated(out seriesIds, out time);

            // Update folders that match folder string
            foreach (ContentRootFolder tvFolder in Settings.TvFolders)
                if (folder.StartsWith("All") || tvFolder.FullPath == folder)
                {
                    OnTvFolderUpdateProgressChange(false, 0, "Updating '" + tvFolder.FullPath + "' - Gathering Files");
                    if (!Organization.UpdateTvFolder(string.Empty, tvFolder, fastUpdate, seriesIds))
                        break;
                }

            tvUpdateCancelled = false;

            // Save show changes
            if (!string.IsNullOrEmpty(time))
                Shows.LastUpdate = time;
            SortShows();
            SaveShows();
        }

        /// <summary>
        /// Search through all sub-folders of a content folder and attempts
        /// to assign each one to a TV show. Recursively performed on sub-content
        /// folders.
        /// </summary>
        /// <param name="tvFolder"></param>
        private static bool UpdateTvFolder(string basePath, ContentRootFolder tvFolder, bool fastUpdate, List<int> idsToUpdate)
        {
            seriesIdsToUpdate = idsToUpdate;

            string progressMsg = "Updating '" + tvFolder.FullPath + "'";
            OnTvFolderUpdateProgressChange(false, 0, progressMsg);

            OrgProcessing processing = new OrgProcessing(TvUpdateProcess);
            tvUpdateNumber = processing.ProcessNumber;

            processing.Run(tvFolder.BuildSubDirectories(false, true), ref tvUpdateCancelled, false, false);

            // Remove shows that no longer exists
            lock (ShowsLock)
                for (int i = Shows.Count - 1; i >= 0; i--)
                    if (!Shows[i].Found && Shows[i].RootFolder.StartsWith(tvFolder.FullPath))
                        RemoveShowAt(i);

            // Save movie changes
            Organization.SaveShows();

            progressMsg = "Updating '" + tvFolder.FullPath + "' complete!";
            OnTvFolderUpdateProgressChange(false, 0, progressMsg);

            return !tvUpdateCancelled;
        }

        private static List<int> seriesIdsToUpdate;

        private static void TvUpdateProcess(OrgPath orgPath, int pathNum, int totalPaths, int updateNumber, bool background, bool subSearch, OrgProcessing.ProcessComplete complete, ref int numItemsProcessed)
        {
            //Console.WriteLine("TV Updater #" + pathNum + ": " + orgPath.Path);

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

            string progressMsg = "Updating '" + orgPath.RootFolder.FullPath + "' - Processed '" + Path.GetFileName(orgPath.Path) + "'";

            // Show found, next!
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

                complete();

                return;
            }

            // Get match
            TvShow match = SearchHelper.TvShowSearch.PathMatch(orgPath.RootFolder.FullPath, orgPath.Path);

            if (updateNumber == tvUpdateNumber)
            {
                if (showExists && match != null)
                {
                    newShow.UpdateInfo(match);
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

                complete();
            }

            
        }

        private static int tvUpdateNumber = 0;

        /// <summary>
        /// Cancels TV folder updating thread.
        /// </summary>
        public static void CancelTvUpdating()
        {
            tvUpdateCancelled = true;
        }

        #endregion

        #endregion

        #region Log

        /// <summary>
        /// Adds item to log.
        /// </summary>
        /// <param name="item"></param>
        public static void AddLogItem(OrgItem item)
        {
            ActionLog.Insert(0, item);
            SaveLog();
        }

        /// <summary>
        /// Removes item from log.
        /// </summary>
        /// <param name="index"></param>
        public static void RemoveLogItem(int index)
        {
            ActionLog.RemoveAt(index);
            SaveLog();
        }

        #endregion

        #region Movies

        #region Add, Remove, Sort

        /// <summary>
        /// Add movie to movie list
        /// </summary>
        /// <param name="newMovie">Show instance to add to list</param>
        public static void AddMovie(Movie newMovie)
        {
            // Add movie to list and sort
            lock (MoviesLock)
            {
                Movies.Add(newMovie);
                //Movies.Sort();
            }
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
        /// Gets the list of movies that are contained within a content folder that
        /// match a genre filter.
        /// </summary>
        /// <param name="contentFolderName">The content folder to look through</param>
        /// <param name="genreFilter">The genre filter that movie must match to be added to the returned list</param>
        /// <returns>List of movie found in content folder</returns>
        public static List<Content> GetMoviesFromFolders(string contentFolderName, string genreFilter, bool yearFilter, int minYear, int maxYear, string nameFilter)
        {
            // Initialize movies list
            List<Content> folderMovies = new List<Content>();

            // Go through each content folder and get movie from folders that match name
            foreach (ContentRootFolder folder in Settings.MovieFolders)
                if (contentFolderName.StartsWith("All") || folder.FullPath == contentFolderName)
                    GetMoviesFromFolders(folder, genreFilter, folderMovies, yearFilter, minYear, maxYear, nameFilter);

            // Returns list of movies
            return folderMovies;
        }

        /// <summary>
        /// Get the list of movies that are contained within a set of content folders.
        /// </summary>
        /// <param name="folders">List of content folders to get movies from.</param>
        /// <returns>The list of movies</returns>
        public static List<Content> GetMoviesFromFolders(List<ContentRootFolder> folders)
        {
            List<Content> folderMovies = new List<Content>();
            foreach (ContentRootFolder folder in folders)
                GetMoviesFromFolders(folder, "All", folderMovies, false, 0, 0, string.Empty);
            return folderMovies;
        }

        /// <summary>
        /// Build a list of movies that are contained within a content folder that
        /// match a genre filter.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="genre"></param>
        /// <param name="folderMovies"></param>
        private static void GetMoviesFromFolders(ContentRootFolder folder, string genre, List<Content> folderMovies, bool yearFilter, int minYear, int maxYear, string nameFilter)
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
                    if (FolderContainsMovie(movie, folder) && genreMatch && yearMatch && nameMatch)
                        folderMovies.Add(movie);
                }
        }

        /// <summary>
        /// Check if a content folder contains a specific movie. Called
        /// recursively on sub-content folders.
        /// </summary>
        /// <param name="movie">The movie to check for</param>
        /// <param name="folder">The folder to check if the movie is in</param>
        /// <returns>Whether the movie is contained in the folder</returns>
        private static bool FolderContainsMovie(Movie movie, ContentRootFolder folder)
        {            
            // Check if movie content folder matches
            if (movie.RootFolder == folder.FullPath)
                return true;
            else
                // Recursion on sub-folders
                foreach (ContentRootFolder subFolder in folder.ChildFolders)
                    if (FolderContainsMovie(movie, subFolder))
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

        public class OrgProgressChangesEventArgs : ProgressChangedEventArgs
        {
            public bool NewItem { get; set; }

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

        private static int movieUpdateNumber = 0;

        private static void MovieUpdateProcess(OrgPath orgPath, int pathNum, int totalPaths, int updateNumber, bool background, bool subSearch, OrgProcessing.ProcessComplete processComplete, ref int numItemsProcessed)
        {
            //Console.WriteLine("Movie Updater #" + pathNum + ": " + orgPath.Path);

            if (movieUpdateCancelled)
            {
               return;
            }

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

            string progressMsg = "Updating '" + orgPath.RootFolder.FullPath + "' - Processed '" + Path.GetFileName(orgPath.Path) + "'";

            // Movie found, next!
            if (movieExists && movieComplete)
            {
                // Update progress
                if (updateNumber == movieUpdateNumber)
                    OnMovieFolderUpdateProgressChange(false, (int)Math.Round((double)numItemsProcessed / totalPaths * 100D), progressMsg);

                processComplete();

                return;
            }

            // Get movie info
            Movie match = SearchHelper.MovieSearch.PathMatch(orgPath.RootFolder.FullPath, orgPath.Path);

            if (updateNumber == movieUpdateNumber)
            {
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

                processComplete();
            }
        }

        #endregion

        #endregion
    }
}