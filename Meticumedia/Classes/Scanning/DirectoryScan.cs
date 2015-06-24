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
using System.Threading;

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
        public List<OrgPath> GetFolderFiles(List<OrgFolder> folders, bool fast, bool skipDatabaseMatching, out List<OrgItem> autoMoves)
        {
            autoMoves = new List<OrgItem>();

            List<OrgPath> files = new List<OrgPath>();
            foreach (OrgFolder folder in folders)
                GetFolderFiles(folder, folder.FolderPath, files, autoMoves);

            


            // Similarity checks
            if (!skipDatabaseMatching)
            {
                FileCategory[] fileCats = new FileCategory[files.Count];
                for (int i = 1; i < files.Count; i++)
                {
                    fileCats[i] = FileHelper.CategorizeFile(files[i], files[i].Path);

                    if (fileCats[i] != FileCategory.MovieVideo && fileCats[i] != FileCategory.TvVideo)
                        continue;

                    for (int j = i - 1; j >= Math.Max(0, j - 50); j--)
                    {
                        if (scanCanceled)
                            return files;

                        if (fileCats[j] != FileCategory.MovieVideo && fileCats[j] != FileCategory.TvVideo)
                            continue;

                        if (FileHelper.PathsVerySimilar(files[i].Path, files[j].Path) || (fileCats[i] == FileCategory.TvVideo && FileHelper.TrimFromEpisodeInfo(files[i].Path) == FileHelper.TrimFromEpisodeInfo(files[j].Path)))
                        {
                            if (files[j].SimilarTo > 0)
                                files[i].SimilarTo = files[j].SimilarTo;
                            else
                                files[i].SimilarTo = j;
                            break;
                        }
                    }
                }
            }

            return files;
        }

        /// <summary>
        /// Builds list of file contained in a set of organization folders.
        /// File search is done recursively on sub-folders if folder recursive
        /// property is checked.
        /// </summary>
        /// <param name="folder">Organization folder to get files from</param>
        /// <param name="files">List of file being built</param>
        private void GetFolderFiles(OrgFolder baseFolder, string folderPath, List<OrgPath> files, List<OrgItem> autoMoves)
        {
            // Get each file from the folder (try/catch here so that if user specifies as directory that needs special permission it doesn't crash)
            try
            {
                // Get all files from current path, convert to OrgPath and add to list
                string[] fileList = Directory.GetFiles(folderPath);
                foreach (string file in fileList)
                {
                    bool autoMoving = false;
                    foreach (AutoMoveFileSetup autoSetup in Settings.AutoMoveSetups)
                    {
                        OrgItem item;
                        if(autoSetup.BuildFileMoveItem(file, baseFolder, out item))
                        {
                            autoMoves.Add(item);
                            autoMoving = true;
                            break;
                        }
                    }
                    if (!autoMoving)
                        files.Add(new OrgPath(file, baseFolder.CopyFrom, baseFolder.AllowDeleting, baseFolder));
                }

                // Recursively seach sub-folders if recursive folder
                if (baseFolder.Recursive)
                {
                    string[] subDirs = Directory.GetDirectories(folderPath);
                    foreach (string subDir in subDirs)
                    {
                        bool autoMoving = false;
                        foreach (AutoMoveFileSetup autoSetup in Settings.AutoMoveSetups)
                        {
                            OrgItem item;
                            if (autoSetup.BuildFolderMoveItem(subDir, baseFolder, out item))
                            {
                                autoMoves.Add(item);
                                autoMoving = true;
                                break;
                            }
                        }
                        if (!autoMoving)
                            GetFolderFiles(baseFolder, subDir, files, autoMoves);
                    }
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
        public void RunScan(List<OrgFolder> folders, List<OrgItem> queuedItems, bool tvOnly, bool skipDatabaseMatching, bool fast, bool reuseResults)
        {
            // Go thorough each folder and create actions for all files
            RunScan(folders, queuedItems, 100, tvOnly, skipDatabaseMatching, fast, reuseResults);           
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
        public void RunScan(List<OrgFolder> folders, List<OrgItem> queuedItems, double progressAmount, bool tvOnly, bool skipDatabaseMatching, bool fast, bool reuseResults)
        {
            // Cleanup director scan log
            for (int i = Organization.DirScanLog.Count - 1; i >= 0; i--)
                if (!File.Exists(Organization.DirScanLog[i].SourcePath) && !Directory.Exists(Organization.DirScanLog[i].SourcePath))
                    Organization.DirScanLog.RemoveAt(i);
            Organization.SaveDirScanLog();
            
            // Set scan to running
            scanRunning = true;
            cancelRequested = false;
            IncrementScanNumber();

            // Update progress
            OnProgressChange(ScanProcess.FileCollect, string.Empty, 0);
            dirScanProgressAmount = progressAmount;

            // Clear update variables
            processStarted = 0;
            processCompletes = 0;

            // Initialize variables used in scan
            List<OrgPath> paths;
            lock (directoryScanLock)
            {
                itemsInQueue = queuedItems;
                temporaryShows = new List<TvShow>();

                // Create items
                List<OrgItem> autoMoves;
                paths = GetFolderFiles(folders, fast, skipDatabaseMatching, out autoMoves);
                this.Items.Clear();
                foreach(OrgPath path in paths)
                    this.Items.Add(new OrgItem(OrgAction.TBD, path.Path, FileCategory.Unknown, path.OrgFolder));
                foreach (OrgItem autoMove in autoMoves)
                    this.Items.Add(autoMove);
            }
            if (!cancelRequested)
                OnItemsInitialized(ScanProcess.Directory, this.Items);


            // Create processing object
            OrgProcessing processing = new OrgProcessing(ThreadProcess);
            int directoryScanNumber = processing.ProcessNumber;

            // Build arguments for 1st pass
            object[] args = new object[] { tvOnly, skipDatabaseMatching, fast, scanNumber, 0, reuseResults };

            // Run 1st pass
            if (this.background)
            {
                bool cancel = false;
                processing.Run(paths, ref cancel, args);
            }
            else
                processing.Run(paths, ref cancelRequested, args);

            // Second pass
            if(!fast && !skipDatabaseMatching && !scanCanceled)
            {
                // Build arguments for 2nd pass
                args = new object[] { tvOnly, skipDatabaseMatching, fast, scanNumber, 1, reuseResults };

                // Run 2nd pass
                if (this.background)
                {
                    bool cancel = false;
                    processing.Run(paths, ref cancel, args);
                }
                else
                    processing.Run(paths, ref cancelRequested, args);

            }

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
            int procNumber = (int)args[3];
            int pass = (int)args[4];
            bool reuseResults = (bool)args[5];

            // Check if item is already processed
            if (this.Items[pathNum].Action != OrgAction.TBD)
                return;

            // Check if file is in the queue
            bool alreadyQueued = false;
            if (itemsInQueue != null)
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
                ProcessUpdate(orgPath.Path, totalPaths, pass);
            else
            {                
                // Update progress
                lock (directoryScanLock)
                    this.Items[pathNum].Action = OrgAction.Processing;

                // Process path
                bool fromLog;
                OrgItem results = ProcessPath(orgPath, tvOnlyCheck, skipMatching || (!fast && pass == 0), fast || pass == 0, true, procNumber, reuseResults, out fromLog);

                // Update results and progress
                if (results != null && (results.Action != OrgAction.None || results.Category == FileCategory.Ignored || fast || pass == 1))
                {
                    UpdateResult(results, pathNum, procNumber);
                    ProcessUpdate(orgPath.Path, totalPaths, pass);

                    if (!fromLog && (results.Action != OrgAction.None && (results.Category == FileCategory.MovieVideo || results.Category == FileCategory.TvVideo)))
                    {
                        Organization.AddDirScanLogItem(new OrgItem(results));
                    }
                }
                else
                    lock (directoryScanLock)
                        this.Items[pathNum].Action = OrgAction.TBD;

            }
        }

        public OrgItem ProcessPath(OrgPath orgPath, bool tvOnlyCheck, bool skipMatching, bool fast, bool threaded, int procNumber, bool allowFromLog, out bool fromLog)
        {
            fromLog = false;
            FileCategory fileCat = FileHelper.CategorizeFile(orgPath, orgPath.Path);
            
            // Search through dir scan log for matching source
            if(allowFromLog)
                for (int i = 0; i < Organization.DirScanLog.Count; i++)
                {
                    if (Organization.DirScanLog[i].SourcePath == orgPath.Path)
                    {
                        fromLog = true;

                        if (fileCat != Organization.DirScanLog[i].Category)
                            break;

                        OrgItem newItem = UpdateItemFromPrevious(orgPath, Organization.DirScanLog[i], threaded, fast, skipMatching);
                        if (newItem != null)
                        {
                            if (newItem.CanEnable)
                                newItem.Enable = true;
                            return newItem;
                        }
                    }
                }
            
            // If similar to earlier item, wait for it to complete before continuing
            while (!skipMatching && orgPath.SimilarTo >= 0 && orgPath.SimilarTo < this.Items.Count && (this.Items[orgPath.SimilarTo].Action == OrgAction.TBD || this.Items[orgPath.SimilarTo].Action == OrgAction.Processing))
            {
                // Check for cancellation
                if (scanCanceled || procNumber < scanNumber)
                    return null;

                Thread.Sleep(50);
            }

            // If similar to earlier item, check if we can use it's info
            if (orgPath.SimilarTo > 0 && orgPath.SimilarTo < this.Items.Count)
            {
                OrgItem newItem = UpdateItemFromPrevious(orgPath, this.Items[orgPath.SimilarTo], threaded, fast, skipMatching);
                if (newItem != null)
                    return newItem;
            }

            // Create default none item
            OrgItem noneItem = new OrgItem(OrgAction.None, orgPath.Path, fileCat, orgPath.OrgFolder);
            
            // Setup match to filename and folder name (if it's in a folder inside of downloads)
            string pathBase;
            if (!string.IsNullOrEmpty(orgPath.OrgFolder.FolderPath))
                pathBase = orgPath.Path.Replace(orgPath.OrgFolder.FolderPath, "");
            else
                pathBase = orgPath.Path;
            string[] pathSplit;
            if (!string.IsNullOrEmpty(orgPath.OrgFolder.FolderPath))
                pathSplit = pathBase.Split('\\');
            else
                pathSplit = orgPath.Path.Split('\\');

            List<string> possibleMatchPaths = new List<string>();
            possibleMatchPaths.Add(pathSplit.Last());
            for (int i = pathSplit.Length - 2; i > 0; i--)
            {
                string build = string.Empty;
                for (int j = i; j < pathSplit.Length; j++)
                    build += pathSplit[j] + " ";

                possibleMatchPaths.Add(build);
            }
            for (int i = pathSplit.Length - 2; i > 0; i--)
                possibleMatchPaths.Add(pathSplit[i] + Path.GetExtension(orgPath.Path));

            // Try to match to each string
            foreach (string matchString in possibleMatchPaths)
            {
                OnDebugNotificationd("Attempting to match to path: \" " + matchString + "\"");
                
                // Get simple file name
                string simpleFile = FileHelper.BasicSimplify(Path.GetFileNameWithoutExtension(matchString), false);
                OnDebugNotificationd("Simplifies to:  \" " + simpleFile + "\"");

                // Categorize current match string
                FileCategory matchFileCat = FileHelper.CategorizeFile(orgPath, matchString);
                OnDebugNotificationd("Classified as: " + matchFileCat);

                // Check tv
                if (tvOnlyCheck && matchFileCat != FileCategory.TvVideo)
                    continue;

                // Check for cancellation
                if (scanCanceled || procNumber < scanNumber)
                    return null;

                // Set whether item is for new show
                bool newShow = false;

                // Check for cancellation
                if (scanCanceled || procNumber < scanNumber)
                    return null;

                // Add appropriate action based on file category
                switch (matchFileCat)
                {
                    // TV item
                    case FileCategory.TvVideo:

                        // Try to match file to existing show
                        Dictionary<TvShow, MatchCollection> matches = new Dictionary<TvShow, MatchCollection>();
                        for (int j = 0; j < Organization.Shows.Count; j++)
                        {
                            MatchCollection match = Organization.Shows[j].MatchFileToContent(matchString);
                            if (match != null && match.Count > 0)
                                matches.Add((TvShow)Organization.Shows[j], match);
                        }

                        // Try to match to temporary show
                        lock (directoryScanLock)
                            foreach (TvShow show in temporaryShows)
                            {
                                MatchCollection match = show.MatchFileToContent(matchString);
                                if (match != null && match.Count > 0)
                                    matches.Add(show, match);
                            }

                        // Debug notification for show matches
                        string matchNotification = "Show name matches found: ";
                        if (matches.Count == 0)
                            matchNotification += "NONE";
                        else
                        {
                            foreach (TvShow match in matches.Keys)
                                matchNotification += match.DisplayName + ", ";
                            matchNotification = matchNotification.Substring(0, matchNotification.Length - 2);
                        }
                        OnDebugNotificationd(matchNotification);

                        // Check for matches to existing show
                        TvShow bestMatch = null;
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
                            if (bestMatch != null)
                                OnDebugNotificationd("Matched to show " + bestMatch.DisplayName);
                        }

                        // Episode not matched to a TV show, search database!
                        if (bestMatch == null && !skipMatching)
                        {
                            OnDebugNotificationd("Not matched to existing show; searching database.");
                            
                            // Setup search string
                            string showFile = Path.GetFileNameWithoutExtension(matchString);

                            // Perform search for matching TV show
                            if (SearchHelper.TvShowSearch.ContentMatch(showFile, string.Empty, string.Empty, fast, threaded, out bestMatch))
                            {
                                ContentRootFolder defaultTvFolder;
                                string path = NO_TV_FOLDER;
                                if (Settings.GetTvFolderForContent(bestMatch, out defaultTvFolder))
                                    path = defaultTvFolder.FullPath;

                                bestMatch.RootFolder = path;
                                bestMatch.Path = bestMatch.BuildFolderPath();

                                // Save show in temporary shows list (in case there are more files that may match to it during scan)
                                lock (directoryScanLock)
                                    if (!temporaryShows.Contains(bestMatch))
                                        temporaryShows.Add(bestMatch);
                                newShow = true;
                            }
                            else
                                bestMatch = null;  
                        }
                        else if (temporaryShows.Contains(bestMatch))
                            newShow = true;

                        // Episode has been matched to a TV show
                        if (bestMatch != null)
                        {
                            // Try to get episode information from file
                            int seasonNum, episodeNum1, episodeNum2;
                            if (FileHelper.GetEpisodeInfo(matchString, bestMatch.DatabaseName, out seasonNum, out episodeNum1, out episodeNum2))
                            {
                                // No season match means file only had episode number, allow this for single season shows
                                if (seasonNum == -1)
                                {
                                    int maxSeason = 0;
                                    foreach (int season in bestMatch.Seasons)
                                        if (season > maxSeason)
                                            maxSeason = season;

                                    if (maxSeason == 1)
                                        seasonNum = 1;
                                }
                                
                                // Try to get the episode from the show
                                TvEpisode episode1, episode2 = null;
                                if (bestMatch.FindEpisode(seasonNum, episodeNum1, false, out episode1))
                                {
                                    if (episodeNum2 != -1)
                                        bestMatch.FindEpisode(seasonNum, episodeNum2, false, out episode2);

                                    OrgAction action = orgPath.Copy ? OrgAction.Copy : OrgAction.Move;

                                    // If item episode already exists set action to duplicate
                                    if (episode1.Missing == MissingStatus.Located)
                                        action = OrgAction.AlreadyExists;

                                    // Build action and add it to results
                                    string destination = bestMatch.BuildFilePath(episode1, episode2, Path.GetExtension(orgPath.Path));
                                    OrgItem newItem = new OrgItem(action, orgPath.Path, destination, episode1, episode2, fileCat, orgPath.OrgFolder);
                                    if (destination.StartsWith(NO_TV_FOLDER))
                                        newItem.Action = OrgAction.NoRootFolder;
                                    if (newItem.Action == OrgAction.AlreadyExists || newItem.Action == OrgAction.NoRootFolder)
                                        newItem.Enable = false;
                                    else
                                        newItem.Enable = true;
                                    newItem.Category = matchFileCat;
                                    newItem.IsNewShow = newShow; 
                                    return newItem;
                                }
                                else
                                    OnDebugNotificationd("Couldn't find episode for season " + seasonNum + " episods " + episodeNum1);
                            }
                        }

                        // No match to TV show
                        if (!tvOnlyCheck && !fast)
                        {
                            // Try to match to a movie
                            OrgItem movieItem;
                            if (CreateMovieAction(orgPath, matchString, out movieItem, threaded, fast, skipMatching, null))
                                noneItem.Clone(movieItem);
                        }

                        break;

                    // Movie item
                    case FileCategory.MovieVideo:
                        // Create action
                        OrgItem item;
                        CreateMovieAction(orgPath, matchString, out item, threaded, fast, skipMatching, null);

                        // If delete action created (for sample file)
                        if (item.Action == OrgAction.Delete)
                            return BuildDeleteAction(orgPath, fileCat);
                        else if (item.Action != OrgAction.None)
                            return item;
                        break;

                    // Trash
                    case FileCategory.Trash:
                        return BuildDeleteAction(orgPath, matchFileCat);
                    // Ignore
                    case FileCategory.Ignored:
                        return new OrgItem(OrgAction.None, orgPath.Path, matchFileCat, orgPath.OrgFolder);
                    // Unknown
                    default:
                        return new OrgItem(OrgAction.None, orgPath.Path, matchFileCat, orgPath.OrgFolder);
                }

                // Check for cancellation
                if (scanCanceled || procNumber < scanNumber)
                    return noneItem;
            }

            // If no match on anything set none action
            return noneItem;
        }

        private OrgItem UpdateItemFromPrevious(OrgPath orgPath, OrgItem reuseItem, bool threaded, bool fast, bool skipMatching)
        {
            switch (reuseItem.Action)
            {
                case OrgAction.None:
                case OrgAction.Delete:
                    OrgItem copyItem = new OrgItem(reuseItem);
                    copyItem.SourcePath = orgPath.Path;
                    copyItem.BuildDestination();
                    return copyItem;
                case OrgAction.Move:
                case OrgAction.Copy:
                    if (reuseItem.Category == FileCategory.MovieVideo)
                    {
                        OrgItem item;
                        CreateMovieAction(orgPath, orgPath.Path, out item, threaded, fast, skipMatching, reuseItem.Movie);

                        return item;
                    }
                    else if (reuseItem.Category == FileCategory.TvVideo)
                    {
                        if (!Organization.Shows.Contains(reuseItem.TvEpisode.Show) && !temporaryShows.Contains(reuseItem.TvEpisode.Show))
                            lock (directoryScanLock)
                                temporaryShows.Add(reuseItem.TvEpisode.Show);
                    }
                    break;
                default:
                    break;
            }

            return null;
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
        int processCompletes = 0;

        /// <summary>
        /// Update progress for scan processing
        /// </summary>
        /// <param name="file">Current file being processes</param>
        /// <param name="pathum">The file's number in queue of all files</param>
        /// <param name="totalPaths">Total number of files queued for processing</param>
        private void ProcessUpdate(string file, int totalPaths, int pass)
        {
            // Add new files to list of files being processed, remove completed files
            lock (processLock)
            {
                processCompletes++;
            }

            // Update progress
            int perc = (int)Math.Round((double)processCompletes / totalPaths * dirScanProgressAmount);

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
                this.Items[pathNum].Clone(item);
        }

        /// <summary>
        /// Tries to create a movie action item for a scan from a file.
        /// </summary>
        /// <param name="file">The file to create movie action from</param>
        /// <param name="item">The resulting movie action</param>
        /// <returns>Whether the file was matched to a movie</returns>
        private bool CreateMovieAction(OrgPath file, string matchString, out OrgItem item, bool threaded, bool fast, bool skipMatching, Movie knownMovie)
        {
            // Initialize item
            item = new OrgItem(OrgAction.None, file.Path, FileCategory.MovieVideo, file.OrgFolder);

            // Check if sample!
            if (matchString.ToLower().Contains("sample"))
            {
                item.Action = OrgAction.Delete;
                return false;
            }

            // Try to match file to movie
            string search = Path.GetFileNameWithoutExtension(matchString);

            // Search for match to movie
            Movie searchResult = null;
            bool searchSucess = false;
            if (!skipMatching)
                searchSucess = SearchHelper.MovieSearch.ContentMatch(search, string.Empty, string.Empty, fast, threaded, out searchResult, knownMovie);


            // Add closest match item
            if (searchSucess)
            {
                // Get root folder
                ContentRootFolder rootFolder;
                string path;
                if (Settings.GetMovieFolderForContent(searchResult, out rootFolder))
                    searchResult.RootFolder = rootFolder.FullPath;
                else
                {
                    searchResult.RootFolder = NO_MOVIE_FOLDER;
                    item.Action = OrgAction.NoRootFolder;
                }
                
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
        private OrgItem BuildDeleteAction(OrgPath file, FileCategory fileCat)
        {
            OrgItem newItem;
            if (file.AllowDelete)
            {
                newItem = new OrgItem(OrgAction.Delete, file.Path, fileCat, file.OrgFolder);
                newItem.Enable = true;
            }
            else
            {
                newItem = new OrgItem(OrgAction.None, file.Path, fileCat, file.OrgFolder);
            }
            return newItem;
        }

        #endregion
    }
}
