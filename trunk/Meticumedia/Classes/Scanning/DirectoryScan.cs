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

namespace Meticumedia
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
        /// Dictionary of scan results indexed by scan number (multiple scan threads could be running so need index to separate where items belong).
        /// </summary>
        private List<OrgItem> scanResults = new List<OrgItem>();

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
        public List<OrgItem> RunScan(List<OrgFolder> folders, List<OrgItem> queuedItems, bool tvOnly, bool skipDatabaseMatching, bool fast)
        {
            // Set scan to running
            scanRunning = true;

            // Go thorough each folder and create actions for all files
            List<OrgItem> results = RunScan(folders, queuedItems, 100, tvOnly, skipDatabaseMatching, fast);

            scanRunning = false;
            cancelRequested = false;

            return results;
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
        public List<OrgItem> RunScan(List<OrgFolder> folders, List<OrgItem> queuedItems, double progressAmount, bool tvOnly, bool skipDatabaseMatching, bool fast)
        {
            // Update progress
            OnProgressChange(ScanProcess.FileCollect, string.Empty, 0);
            dirScanProgressAmount = progressAmount;

            // Clear update variables
            processingFiles = new List<string>();
            processStarted = 0;
            processEnded = 0;

            // Create processing object
            OrgProcessing processing = new OrgProcessing(ThreadProcess);
            int directoryScanNumber = processing.ProcessNumber;

            // Initialize variables used in scan
            lock (directoryScanLock)
            {
                itemsInQueue = queuedItems;
                temporaryShows = new List<TvShow>();
                scanResults = new List<OrgItem>();
            }

            // Build arguments
            object[] args = new object[] { tvOnly, skipDatabaseMatching, fast };

            // Run processing
            if (this.background)
            {
                bool cancel = false;
                processing.Run(GetFolderFiles(folders), ref cancel, args);
            }
            else
                processing.Run(GetFolderFiles(folders), ref cancelRequested, args);

            // Update progress
            OnProgressChange(ScanProcess.Directory, string.Empty, (int)progressAmount);

            // Return results
            return scanResults;
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

            // Check for cancellation
            if ((cancelRequested && !background) || cancelAllRequested)
                return;

            // Get simple file name
            string simpleFile = FileHelper.BasicSimplify(Path.GetFileNameWithoutExtension(orgPath.Path), false);

            // Categorize the file
            FileHelper.FileCategory fileCat = FileHelper.CategorizeFile(orgPath);

            // Check tv
            if (tvOnlyCheck && fileCat != FileHelper.FileCategory.TvVideo)
                return;

            // Update progress
            ProcessUpdate(orgPath.Path, true, pathNum, totalPaths);

            // Check if file is in the queue
            bool alreadyQueued = false;
            for (int i = 0; i < itemsInQueue.Count; i++)
                if (itemsInQueue[i].SourcePath == orgPath.Path)
                {
                    OrgItem newItem = new OrgItem(itemsInQueue[i]);
                    newItem.Action = OrgAction.Queued;
                    newItem.Check = System.Windows.Forms.CheckState.Unchecked;
                    AddResult(newItem);
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


            // Add appropriate action based on file category
            switch (fileCat)
            {
                // TV item
                case FileHelper.FileCategory.TvVideo:
                    // Check if sample!
                    if (orgPath.Path.ToLower().Contains("sample"))
                    {
                        AddDeleteAction(orgPath, fileCat);
                        break;
                    }

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


                                if (m.Length > longestMatch && m.Length >= matchWordCnt)
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
                        bestMatch.RootFolder = Path.Combine(path, bestMatch.Name);
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
                        if (FileHelper.GetEpisodeInfo(orgPath.Path, bestMatch.Name, out seasonNum, out episodeNum1, out episodeNum2))
                        {
                            // Try to get the episode from the show
                            TvEpisode episode1, episode2 = null;
                            if (bestMatch.FindEpisode(seasonNum, episodeNum1, false, out episode1))
                            {
                                if (episodeNum2 != -1)
                                    bestMatch.FindEpisode(seasonNum, episodeNum2, false, out episode2);

                                OrgAction action = orgPath.Copy ? OrgAction.Copy : OrgAction.Move;

                                // If item episode already exists set action to duplicate
                                if (episode1.Missing == TvEpisode.MissingStatus.Located)
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
                                    newItem.Check = System.Windows.Forms.CheckState.Unchecked;
                                else
                                    newItem.Check = System.Windows.Forms.CheckState.Checked;
                                AddResult(newItem);
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
                            AddResult(movieItem);
                        else
                        {
                            OrgItem newItem = new OrgItem(OrgAction.None, orgPath.Path, fileCat, orgPath.OrgFolder);
                            AddResult(newItem);
                        }
                    }

                    break;

                // Movie item
                case FileHelper.FileCategory.NonTvVideo:
                    // Create action
                    OrgItem item;
                    CreateMovieAction(orgPath, out item, fast);

                    // If delete action created (for sample file)
                    if (item.Action == OrgAction.Delete)
                        AddDeleteAction(orgPath, fileCat);
                    else
                        AddResult(item);
                    break;

                // Trash
                case FileHelper.FileCategory.Trash:
                    AddDeleteAction(orgPath, fileCat);
                    break;
                // Ignore
                case FileHelper.FileCategory.Ignored:
                    AddResult(new OrgItem(OrgAction.None, orgPath.Path, fileCat, orgPath.OrgFolder));
                    break;
                // Unknown
                default:
                    AddResult(new OrgItem(OrgAction.None, orgPath.Path, fileCat, orgPath.OrgFolder));
                    break;
            }

            // Update progress
            ProcessUpdate(orgPath.Path, false, pathNum, totalPaths);
        }

        /// <summary>
        /// Lock for processing update variables
        /// </summary>
        private object processLock = new object();

        /// <summary>
        /// List of files currently being processed
        /// </summary>
        private List<string> processingFiles = new List<string>();

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
                {
                    processingFiles.Add(file);
                    processStarted++;
                }
                else
                {
                    processingFiles.Remove(file);
                    processEnded++;
                }
            }

            // Update progress
            int perc = (int)Math.Round((double)(processStarted + processEnded) / (totalPaths * 2) * dirScanProgressAmount);
            string path;
            if (processingFiles.Count > 1) 
                path = processingFiles.Count + " others";
            else
                path = processingFiles.Count > 0 ? processingFiles[0] : file;
            OnProgressChange(ScanProcess.Directory, path, perc);
        }

        /// <summary>
        /// Add item to directory scan results list in dictionary. Variables accessed by multiple threads, so controlled.
        /// </summary>
        /// <param name="scanNum">The scan number to add item to</param>
        /// <param name="item">Item to add to results list</param>
        private void AddResult(OrgItem item)
        {
            lock (directoryScanLock)
                scanResults.Add(item);
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
            item = new OrgItem(OrgAction.None, file.Path, FileHelper.FileCategory.NonTvVideo, file.OrgFolder);

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
                    item.Check = System.Windows.Forms.CheckState.Unchecked;
                else
                    item.Check = System.Windows.Forms.CheckState.Checked;
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
        private void AddDeleteAction(OrgPath file, FileHelper.FileCategory fileCat)
        {
            OrgItem newItem;
            if (file.AllowDelete)
            {
                newItem = new OrgItem(OrgAction.Delete, file.Path, fileCat, file.OrgFolder);
                newItem.Check = System.Windows.Forms.CheckState.Checked;
                AddResult(newItem);
            }
            else
            {
                newItem = new OrgItem(OrgAction.None, file.Path, fileCat, file.OrgFolder);
                AddResult(newItem);
            }
        }

        #endregion
    }
}
