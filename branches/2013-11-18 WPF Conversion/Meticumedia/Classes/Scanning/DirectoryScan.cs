// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Meticumedia.Classes
{
    public class DirectoryScan : Scan
    {
        #region Properties

        public bool Running { get { return scanRunning; } }

        #endregion

        public DirectoryScan(bool background)
            : base(background)
        {
        }

        #region Variables

        /// <summary>
        /// Dictionary of temporary shows that were matched to TV episode that couldn't be matched to existing shows.
        /// Indexed by scan number (multiple scan threads could be running so need index to separate where items belong)
        /// </summary>
        private List<TvShow> temporaryShows = new List<TvShow>();

        /// <summary>
        /// Local list of items are currently queued to be processed. (Could perform scan on directory that has items in queue, so we don't want to
        /// proposed actions for those.)
        /// </summary>
        private List<OrgItem> itemsInQueue;

        /// <summary>
        /// Lock for accessing scan variables that are used by multiple threads.
        /// </summary>
        private static object directoryScanLock = new object();

        /// <summary>
        /// Percentage that directory scan represents of total scan. TV scan runs a directory scan as 
        /// part of it to check for missing episode in scan directories, so directory is only a portion
        /// of the scan progress.
        /// </summary>
        private static double dirScanProgressAmount = 100;

        #endregion

        #region Methods

        /// <summary>
        /// Builds list of file contained in a set of organization folders.
        /// File search is done recursively on sub-folders if folder recursive
        /// property is checked.
        /// </summary>
        /// <param name="folders">Set of organization folders to get files from</param>
        /// <returns>List of files contained in folders</returns>
        private List<OrgPath> GetFolderFiles(List<OrgFolder> folders)
        {
            List<OrgPath> files = new List<OrgPath>();
            foreach (OrgFolder folder in folders)
                GetFolderFiles(folder, folder.FolderPath, files);
            return files;
        }

        /// <summary>
        /// Builds list of file contained in a set of organization folders.
        /// File search is done recursively on sub-folders if folder recursive
        /// property is checked.
        /// </summary>
        /// <param name="folder">Organization folder to get files from</param>
        /// <param name="files">List of file being built</param>
        private void GetFolderFiles(OrgFolder baseFolder, string folderPath, List<OrgPath> files)
        {
            // Get each file from the folder (try/catch here so that if user specifies as directory that needs special permission it doesn't crash)
            try
            {
                // Get all files from current path, convert to OrgPath and add to list
                string[] fileList = Directory.GetFiles(folderPath);
                foreach (string file in fileList)
                    files.Add(new OrgPath(file, baseFolder.CopyFrom, baseFolder.AllowDeleting, baseFolder));

                // Recursively seach sub-folders if recursive folder
                if (baseFolder.Recursive)
                {
                    string[] subDirs = Directory.GetDirectories(folderPath);
                    foreach (string subDir in subDirs)
                        GetFolderFiles(baseFolder, subDir, files);
                }
            }
            catch { }
        }

        /// <summary>
        /// Searches through a list of folders and compiles a builds of files and actions to perform.
        /// </summary>
        /// <param name="folders">List of folders to scan</param>
        /// <param name="queuedItems">Items that are currently in the queue (user may run a scan on dir. that has files being processed, so those are skipped)</param>
        /// <param name="tvOnly">Whether ignore all files that aren't TV video files</param>
        /// <param name="background">Whether scan is running in background (ignores user cancellations)</param>
        /// <returns>List of actions</returns>
        public void RunScan(List<OrgFolder> folders, List<OrgItem> queuedItems, bool tvOnly, bool skipDatabaseMatching, bool fast)
        {
            // Go thorough each folder and create actions for all files
            RunScan(folders, queuedItems, 100, tvOnly, skipDatabaseMatching, fast);           
        }

        /// <summary>
        /// Searchs through a single folder and builds a list of files and actions to perform
        /// for those files. Performs recursively on sub-directories if folder has recursive
        /// property set.
        /// </summary>
        /// <param name="folder">Folder to search through</param>
        /// <param name="queuedItems">Items currenlty in the queue (to be skipped)</param>
        /// <param name="progressAmount">Percentage that directory scan represents of total scan</param>
        /// <param name="tvOnly">Whether ignore all files that aren't TV video files</param>
        /// <param name="background">Whether scan is running in background (ignores user cancellations)</param>
        public void RunScan(List<OrgFolder> folders, List<OrgItem> queuedItems, double progressAmount, bool tvOnly, bool skipDatabaseMatching, bool fast)
        {
            // Set scan to running
            scanRunning = true;
            cancelRequested = false;
            IncrementScanNumber();

            // Update progress
            OnProgressChange(ScanProcess.FileCollect, string.Empty, 0);
            dirScanProgressAmount = progressAmount;

            // Clear update variables
            processStarted = 0;
            processEnded = 0;

            // Initialize variables used in scan
            List<OrgPath> paths;
            lock (directoryScanLock)
            {
                itemsInQueue = queuedItems;
                temporaryShows = new List<TvShow>();

                // Create items
                paths = GetFolderFiles(folders);
                this.Items.Clear();
                foreach(OrgPath path in paths)
                    this.Items.Add(new OrgItem(OrgAction.TBD, path.Path, FileCategory.Unknown, path.OrgFolder));
            }
            OnItemsInitialized(ScanProcess.Directory, this.Items);

            // Create processing object
            OrgProcessing processing = new OrgProcessing(ThreadProcess);
            int directoryScanNumber = processing.ProcessNumber;

            // Build arguments
            object[] args = new object[] { tvOnly, skipDatabaseMatching, fast, scanNumber };

            // Run processing
            if (this.background)
            {
                bool cancel = false;
                processing.Run(paths, ref cancel, args);
            }
            else
                processing.Run(paths, ref cancelRequested, args);

            // Set items to new object
            if (scanCanceled)
            {
                lock (directoryScanLock)
                {
                    int count = this.Items.Count;
                    this.Items = new List<OrgItem>();
                    for (int i = 0; i < count; i++)
                        this.Items.Add(new OrgItem());
                }
            }
            else
                OnProgressChange(ScanProcess.Directory, string.Empty, (int)progressAmount);
            
            scanRunning = false;
        }

        /// <summary>
        /// Directory scan processing method (thread) for a single file path.
        /// </summary>
        /// <param name="orgPath">Organization path instance to be processed</param>
        /// <param name="pathNum">The path's number out of total being processed</param>
        /// <param name="totalPaths">Total number of paths being processed</param>
        /// <param name="updateNumber">The identifier for the OrgProcessing instance</param>
        /// <param name="background">Whether processing is running as a background operation</param>
        /// <param name="subSearch">Whether processing is sub-search(TV only)</param>
        /// <param name="processComplete">Delegate to be called by processing when completed</param>
        /// <param name="numItemsProcessed">Number of paths that have been processed - used for progress updates</param>
        private void ThreadProcess(OrgPath orgPath, int pathNum, int totalPaths, int updateNumber, ref int numItemsProcessed, ref int numItemsStarted, object processSpecificArgs)
        {
            // Process specific arguments
            object[] args = (object[])processSpecificArgs;
            bool tvOnlyCheck = (bool)args[0];
            bool skipMatching = (bool)args[1];
            bool fast = (bool)args[2];
            int procNumber = (int)scanNumber;

            // Get simple file name
            string simpleFile = FileHelper.BasicSimplify(Path.GetFileNameWithoutExtension(orgPath.Path), false);

            // Categorize the file
            FileCategory fileCat = FileHelper.CategorizeFile(orgPath);

            // Check tv
            if (tvOnlyCheck && fileCat != FileCategory.TvVideo)
                return;

            // Check for cancellation
            if (scanCanceled || procNumber < scanNumber)
                return;

            // Update progress
            lock (directoryScanLock)
            {
                this.Items[pathNum].Action = OrgAction.Processing;
                this.Items[pathNum].Category = fileCat;
            }
            ProcessUpdate(orgPath.Path, true, pathNum, totalPaths);

            // Check if file is in the queue
            bool alreadyQueued = false;
            if(itemsInQueue != null)
            for (int i = 0; i < itemsInQueue.Count; i++)
                if (itemsInQueue[i].SourcePath == orgPath.Path)
                {
                    OrgItem newItem = new OrgItem(itemsInQueue[i]);
                    newItem.Action = OrgAction.Queued;
                    newItem.Enable = true;
                    UpdateResult(newItem, pathNum, procNumber);
                    alreadyQueued = true;
                    break;
                }

            // If item is already in the queue, skip it
            if (alreadyQueued)
            {
                ProcessUpdate(orgPath.Path, false, pathNum, totalPaths);
                return;
            }

            // Set whether item is for new show
            TvShow newShow = null;

            // Try to match file to existing show
            Dictionary<TvShow, MatchCollection> matches = new Dictionary<TvShow, MatchCollection>();
            for (int j = 0; j < Organization.Shows.Count; j++)
            {
                MatchCollection match = Organization.Shows[j].MatchFileToContent(orgPath.Path);
                if (match != null && match.Count > 0)
                    matches.Add((TvShow)Organization.Shows[j], match);
            }

            // Try to match to temporary show
            lock (directoryScanLock)
                foreach (TvShow show in temporaryShows)
                {
                    MatchCollection match = show.MatchFileToContent(orgPath.Path);
                    newShow = show;
                    if (match != null && match.Count > 0)
                        matches.Add(show, match);
                }

            // Check for cancellation
            if (scanCanceled || procNumber < scanNumber)
                return;

            // Add appropriate action based on file category
            switch (fileCat)
            {
                // TV item
                case FileCategory.TvVideo:
                    bool matched = false;

                    TvShow bestMatch = null;
                    bool matchSucess = false;
                    if (matches.Count > 0)
                    {
                        // Find best match, based on length
                        int longestMatch = 2; // minimum of 3 letters must match (acronym)
                        foreach (KeyValuePair<TvShow, MatchCollection> match in matches)
                            foreach (Match m in match.Value)
                            {
                                // Check that match is not part of a word
                                int matchWordCnt = 0;
                                for (int l = m.Index; l < simpleFile.Length; l++)
                                {
                                    if (simpleFile[l] == ' ')
                                        break;
                                    else
                                        ++matchWordCnt;
                                }


                                if (m.Value.Trim().Length > longestMatch && m.Length >= matchWordCnt)
                                {
                                    longestMatch = m.Length;
                                    bestMatch = match.Key;
                                }
                            }
                    }

                    // Episode not matched to a TV show, search database!
                    if (bestMatch == null && !skipMatching)
                    {
                        // Setup search string
                        string showFile = Path.GetFileNameWithoutExtension(orgPath.Path);

                        // Setup path for resulting content
                        ContentRootFolder defaultTvFolder;
                        string path = NO_TV_FOLDER;
                        if (Settings.GetDefaultTvFolder(out defaultTvFolder))
                            path = defaultTvFolder.FullPath;

                        // Perform search for matching TV show
                        matchSucess = SearchHelper.TvShowSearch.ContentMatch(showFile, path, string.Empty, fast, out bestMatch);
                        bestMatch.RootFolder = Path.Combine(path, bestMatch.DatabaseName);
                        TvDatabaseHelper.FullShowSeasonsUpdate(bestMatch);

                        // Save show in temporary shows list (in case there are more files that may match to it during scan)
                        lock (directoryScanLock)
                            if (!temporaryShows.Contains(bestMatch))
                                temporaryShows.Add(bestMatch);
                        newShow = bestMatch;
                    }

                    // Episode has been matched to a TV show
                    if (bestMatch != null)
                    {
                        // Try to get episode information from file
                        int seasonNum, episodeNum1, episodeNum2;
                        if (FileHelper.GetEpisodeInfo(orgPath.Path, bestMatch.DatabaseName, out seasonNum, out episodeNum1, out episodeNum2))
                        {
                            // Try to get the episode from the show
                            TvEpisode episode1, episode2 = null;
                            if (bestMatch.FindEpisode(seasonNum, episodeNum1, false, out episode1))
                            {
                                if (episodeNum2 != -1)
                                    bestMatch.FindEpisode(seasonNum, episodeNum2, false, out episode2);

                                OrgAction action = orgPath.Copy ? OrgAction.Copy : OrgAction.Move;

                                // If item episode already exists set action to duplicate
                                if (episode1.Missing == MissingStatus.Located)
                                {
                                    if (episode1.Ignored)
                                    {
                                        ProcessUpdate(orgPath.Path, false, pathNum, totalPaths);
                                        return;
                                    }

                                    action = OrgAction.AlreadyExists;
                                }

                                // Build action and add it to results
                                string destination = bestMatch.BuildFilePath(episode1, episode2, Path.GetExtension(orgPath.Path));
                                OrgItem newItem = new OrgItem(action, orgPath.Path, destination, episode1, episode2, fileCat, orgPath.OrgFolder, newShow);
                                if (destination.StartsWith(NO_TV_FOLDER))
                                    newItem.Action = OrgAction.NoRootFolder;
                                if (newItem.Action == OrgAction.AlreadyExists || newItem.Action == OrgAction.NoRootFolder)
                                    newItem.Enable = false;
                                else
                                    newItem.Enable = true;
                                UpdateResult(newItem, pathNum, procNumber);
                                matched = true;
                            }
                        }
                    }

                    // No match to TV show
                    if (!matched && !tvOnlyCheck && !fast)
                    {
                        // Try to match to a movie
                        OrgItem movieItem;
                        if (CreateMovieAction(orgPath, out movieItem, fast))
                            UpdateResult(movieItem, pathNum, procNumber);
                        else
                        {
                            OrgItem newItem = new OrgItem(OrgAction.None, orgPath.Path, fileCat, orgPath.OrgFolder);
                            UpdateResult(newItem, pathNum, procNumber);
                        }
                    }

                    break;

                // Movie item
                case FileCategory.MovieVideo:    
                    // Create action
                    OrgItem item;
                    CreateMovieAction(orgPath, out item, fast);

                    // If delete action created (for sample file)
                    if (item.Action == OrgAction.Delete)
                        SetDeleteAction(orgPath, fileCat, pathNum, procNumber);
                    else
                        UpdateResult(item, pathNum, procNumber);
                    break;

                // Trash
                case FileCategory.Trash:
                    SetDeleteAction(orgPath, fileCat, pathNum, procNumber);
                    break;
                // Ignore
                case FileCategory.Ignored:
                    UpdateResult(new OrgItem(OrgAction.None, orgPath.Path, fileCat, orgPath.OrgFolder), pathNum, procNumber);
                    break;
                // Unknown
                default:
                    UpdateResult(new OrgItem(OrgAction.None, orgPath.Path, fileCat, orgPath.OrgFolder), pathNum, procNumber);
                    break;
            }

            // Check for cancellation
            if (scanCanceled || procNumber < scanNumber)
                return;

            // Update progress
            ProcessUpdate(orgPath.Path, false, pathNum, totalPaths);
        }

        /// <summary>
        /// Lock for processing update variables
        /// </summary>
        private object processLock = new object();

        /// <summary>
        /// Number of processes that have been at least started
        /// </summary>
        int processStarted = 0;

        /// <summary>
        /// Number of processes that have completed
        /// </summary>
        int processEnded = 0;

        /// <summary>
        /// Update progress for scan processing
        /// </summary>
        /// <param name="file">Current file being processes</param>
        /// <param name="starting">Whether file processing is just started</param>
        /// <param name="pathum">The file's number in queue of all files</param>
        /// <param name="totalPaths">Total number of files queued for processing</param>
        private void ProcessUpdate(string file, bool starting, int pathum, int totalPaths)
        {
            // Add new files to list of files being processed, remove completed files
            lock (processLock)
            {
                if (starting)
                    processStarted++;
                else
                    processEnded++;
            }

            // Update progress
            int perc = (int)Math.Round((double)(processStarted + processEnded) / (totalPaths * 2) * dirScanProgressAmount);

            if (!scanCanceled)
                OnProgressChange(ScanProcess.Directory, string.Empty, perc);
        }

        /// <summary>
        /// Add item to directory scan results list in dictionary. Variables accessed by multiple threads, so controlled.
        /// </summary>
        /// <param name="scanNum">The scan number to add item to</param>
        /// <param name="item">Item to add to results list</param>
        private void UpdateResult(OrgItem item, int pathNum, int procNum)
        {
            if (scanCanceled || procNum < scanNumber)
                return;

            lock (directoryScanLock)
                this.Items[pathNum].UpdateInfo(item);
        }

        /// <summary>
        /// Tries to create a movie action item for a scan from a file.
        /// </summary>
        /// <param name="file">The file to create movie action from</param>
        /// <param name="item">The resulting movie action</param>
        /// <returns>Whether the file was matched to a movie</returns>
        private bool CreateMovieAction(OrgPath file, out OrgItem item, bool fast)
        {
            // Initialize item
            item = new OrgItem(OrgAction.None, file.Path, FileCategory.MovieVideo, file.OrgFolder);

            // Check if sample!
            if (file.Path.ToLower().Contains("sample"))
            {
                item.Action = OrgAction.Delete;
                return false;
            }

            // Try to match file to movie
            string search = Path.GetFileNameWithoutExtension(file.Path);

            // Get root folder
            ContentRootFolder defaultMovieFolder;
            string path;
            if (Settings.GetDefaultMovieFolder(out defaultMovieFolder))
                path = defaultMovieFolder.FullPath;
            else
            {
                path = NO_MOVIE_FOLDER;
                item.Action = OrgAction.NoRootFolder;
            }

            // Search for match to movie
            Movie searchResult;
            bool searchSucess = SearchHelper.MovieSearch.ContentMatch(search, path, string.Empty, fast, out searchResult);

            // Add closest match item
            if (searchSucess)
            {
                if (item.Action != OrgAction.NoRootFolder)
                    item.Action = file.Copy ? OrgAction.Copy : OrgAction.Move;
                item.DestinationPath = searchResult.BuildFilePath(file.Path);
                if (File.Exists(item.DestinationPath))
                    item.Action = OrgAction.AlreadyExists;
                searchResult.Path = searchResult.BuildFolderPath();
                item.Movie = searchResult;
                if (item.Action == OrgAction.AlreadyExists || item.Action == OrgAction.NoRootFolder)
                    item.Enable = false;
                else
                    item.Enable = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a delete action for a file.
        /// </summary>
        /// <param name="scanResults">The current scan action list to add action to.</param>
        /// <param name="file">The file to be deleted</param>
        /// <param name="fileCat">The category of the file</param>
        private void SetDeleteAction(OrgPath file, FileCategory fileCat, int pathNum, int procNum)
        {
            OrgItem newItem;
            if (file.AllowDelete)
            {
                newItem = new OrgItem(OrgAction.Delete, file.Path, fileCat, file.OrgFolder);
                newItem.Enable = true;
                UpdateResult(newItem, pathNum, procNum);
            }
            else
            {
                newItem = new OrgItem(OrgAction.None, file.Path, fileCat, file.OrgFolder);
                UpdateResult(newItem, pathNum, procNum);
            }
        }

        #endregion
    }
}
