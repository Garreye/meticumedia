// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.ComponentModel;
using System.Threading;

namespace Meticumedia
{
    /// <summary>
    /// Helper for performing all organizational scans.
    /// </summary>
    public static class ScanHelper
    {
        #region Global Variables/Enums

        /// <summary>
        /// Indicates whether cancel was requested on scan work.
        /// </summary>
        private static bool cancelRequested = false;

        /// <summary>
        /// Indicates whether a scan is currently running.
        /// </summary>
        private static bool scanRunning = false;

        /// <summary>
        /// Portions of scan processing that can be running
        /// </summary>
        public enum ScanProcess { FileCollect, Directory, Tv, Movie };

        #endregion

        #region Events

        /// <summary>
        /// Static event for indicatong scan progress has changed
        /// </summary>
        public static event EventHandler<ProgressChangedEventArgs> ScanProgressChange;

        /// <summary>
        /// Triggers ScanProgressChange event
        /// </summary>
        public static void OnScanProgressChange(ScanProcess process, string info, int percent)
        {
            if (ScanProgressChange != null)
                ScanProgressChange(process, new ProgressChangedEventArgs(percent, info));
        }

        /// <summary>
        /// Static event for indicating list of TV items in scan folders has been updated
        /// </summary>
        public static event EventHandler<EventArgs> TvScanItemsUpdate;

        /// <summary>
        /// Triggers TvScanItemsUpdate event
        /// </summary>
        public static void OnTvScanItemsUpdate()
        {
            if (TvScanItemsUpdate != null)
                TvScanItemsUpdate(null, new EventArgs());
        }

        #endregion

        #region General

        /// <summary>
        /// Cancels the currently running scan (if any).
        /// </summary>
        public static void CancelScan()
        {
            if (scanRunning)
                cancelRequested = true;
        }

        #endregion

        #region Directory

        // TV episode from scan directory
        public static List<OrgItem> ScanDirTvItems = new List<OrgItem>();

        /// <summary>
        /// TV item in scan directories is updated every 2 minutes
        /// </summary>
        private static System.Timers.Timer scanDirUpdateTimer = new System.Timers.Timer(120000);

        /// <summary>
        /// Update list of tv episodes currently in scan directories
        /// </summary>
        public static void UpdateScanDirTvItems()
        {            
            // Start timer to do update periodically
            scanDirUpdateTimer.Elapsed += new System.Timers.ElapsedEventHandler(scanDirUpdateTimer_Elapsed);
            scanDirUpdateTimer.Enabled = true;
        }

        /// <summary>
        /// Updates list of TV episodes found in scan directories. Performs a directory scan in background that
        /// only matches to files that are categorized as TV video.
        /// </summary>
        private static void UpdateTvItems()
        {
            // Run scan to look for TV files
            ScanDirTvItems = ScanHelper.DirectoryScan(Settings.ScanDirectories, new List<OrgItem>(), true, true);

            // Update missing
            lock (Organization.Shows.ContentLock)
                foreach (TvShow show in Organization.Shows)
                    show.UpdateMissing();

            // Trigger event indicating update has occured
            OnTvScanItemsUpdate();
        }

        /// <summary>
        /// Episodes in scan directory are updated periodically.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void scanDirUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateTvItems();
        }

        /// <summary>
        /// Builds list of file contained in a set of organization folders.
        /// File search is done recursively on sub-folders if folder recursive
        /// property is checked.
        /// </summary>
        /// <param name="folders">Set of organization folders to get files from</param>
        /// <returns>List of files contained in folders</returns>
        private static List<OrgPath> GetFolderFiles(List<OrgFolder> folders)
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
        private static void GetFolderFiles(OrgFolder baseFolder, string folderPath, List<OrgPath> files)
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
        /// <param name="tvCheck">Whether ignore all files that aren't TV video files</param>
        /// <param name="background">Whether scan is running in background (ignores user cancellations)</param>
        /// <returns>List of actions</returns>
        public static List<OrgItem> DirectoryScan(List<OrgFolder> folders, List<OrgItem> queuedItems, bool tvCheck, bool background)
        {
            // Set scan to running
            if (!background)
                scanRunning = true;

            // Go thorough each folder and create actions for all files
            List<OrgItem> results = DirectoryScan(folders, queuedItems, 100, tvCheck, background);

            if (!background)
            {
                scanRunning = false;
                cancelRequested = false;
            }

            return results;
        }

        /// <summary>
        /// String to use as dir. if TV folders not setup yet
        /// </summary>
        private static readonly string NO_TV_FOLDER = "NO TV FOLDERS SPECIFIED IN SETTINGS";

        /// <summary>
        /// String to use as dir. if movie folders not setup yet
        /// </summary>
        private static readonly string NO_MOVIE_FOLDER = "NO MOVIE FOLDERS SPECIFIED IN SETTINGS";

        /// <summary>
        /// Whether background directory scan is running
        /// </summary>
        private static bool backgroundDirectoryScanRunning = false;

        /// <summary>
        /// Dictionary of temporary shows that were matched to TV episode that couldn't be matched to existing shows.
        /// Indexed by scan number (multiple scan threads could be running so need index to separate where items belong)
        /// </summary>
        private static Dictionary<int, List<TvShow>> temporaryShows = new Dictionary<int, List<TvShow>>();

        /// <summary>
        /// Dictionary of scan results indexed by scan number (multiple scan threads could be running so need index to separate where items belong).
        /// </summary>
        private static Dictionary<int, List<OrgItem>> scanResults = new Dictionary<int, List<OrgItem>>();

        /// <summary>
        /// Local list of items are currently queued to be processed. (Could perform scan on directory that has items in queue, so we don't want to
        /// proposed actions for those.)
        /// </summary>
        private static List<OrgItem> itemsInQueue;

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

        /// <summary>
        /// Searchs through a single folder and builds a list of files and actions to perform
        /// for those files. Performs recursively on sub-directories if folder has recursive
        /// property set.
        /// </summary>
        /// <param name="folder">Folder to search through</param>
        /// <param name="queuedItems">Items currenlty in the queue (to be skipped)</param>
        /// <param name="progressAmount">Percentage that directory scan represents of total scan</param>
        /// <param name="tvCheck">Whether ignore all files that aren't TV video files</param>
        /// <param name="background">Whether scan is running in background (ignores user cancellations)</param>
        private static List<OrgItem> DirectoryScan(List<OrgFolder> folders, List<OrgItem> queuedItems, double progressAmount, bool tvCheck, bool background)
        {
            // Only allow 1 background directory scan at a time
            if (background)
            {
                if (backgroundDirectoryScanRunning)
                    return new List<OrgItem>();
                backgroundDirectoryScanRunning = true;
            }

            // Update progress
            if (!background)
            {
                OnScanProgressChange(ScanProcess.FileCollect, string.Empty, 0);
                dirScanProgressAmount = progressAmount;
            }

            // Create processing object
            OrgProcessing processing = new OrgProcessing(DirectoryScanProcess);
            int directoryScanNumber = processing.ProcessNumber;

            // Initialize variables used in scan
            lock (directoryScanLock)
            {
                itemsInQueue = queuedItems;
                temporaryShows.Add(directoryScanNumber, new List<TvShow>());
                scanResults.Add(directoryScanNumber, new List<OrgItem>());
            }

            // Run processing
            processing.Run(GetFolderFiles(folders), ref cancelRequested, background, tvCheck);

            // Get results and clear variables used in scan
            List<OrgItem> results;
            lock (directoryScanLock)
            {
                results = scanResults[directoryScanNumber];
                temporaryShows.Remove(directoryScanNumber);
                scanResults.Remove(directoryScanNumber);
            }

            // Update progress
            if (!background)
                OnScanProgressChange(ScanProcess.Directory, string.Empty, (int)progressAmount);
            else
                backgroundDirectoryScanRunning = false;

            // Return results
            return results;
        }
        
        /// <summary>
        /// Add item to directory scan results list in dictionary. Variables accessed by multiple threads, so controlled.
        /// </summary>
        /// <param name="scanNum">The scan number to add item to</param>
        /// <param name="item">Item to add to results list</param>
        private static void AddDirScanResult(int scanNum, OrgItem item)
        {
            lock (directoryScanLock)
                if (scanResults.ContainsKey(scanNum))
                    scanResults[scanNum].Add(item);
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
        private static void DirectoryScanProcess(OrgPath orgPath, int pathNum, int totalPaths, int updateNumber, bool background, bool subSearch, OrgProcessing.ProcessComplete complete, ref int numItemsProcessed)
        {
            // Sub-search flag is tv only check flag
            bool tvOnlyCheck = subSearch;

            // Check for cancellation
            if (cancelRequested && !background)
            {
                complete();
                return;
            }

            // Get simple file name
            string simpleFile = FileHelper.BasicSimplify(Path.GetFileNameWithoutExtension(orgPath.Path), false);

            // Categorize the file
            FileHelper.FileCategory fileCat = FileHelper.CategorizeFile(orgPath);

            // Check tv
            if (tvOnlyCheck && fileCat != FileHelper.FileCategory.TvVideo)
            {
                complete();
                return;
            }

            // Update progress
            if (!background)
                OnScanProgressChange(ScanProcess.Directory, orgPath.Path, (int)Math.Round((double)pathNum / totalPaths * dirScanProgressAmount));

            // Check if file is in the queue
            bool alreadyQueued = false;
            for(int i=0;i<itemsInQueue.Count;i++)
                if (itemsInQueue[i].SourcePath == orgPath.Path)
                {
                    OrgItem newItem = new OrgItem(itemsInQueue[i]);
                    newItem.Action = OrgAction.Queued;
                    newItem.Check = System.Windows.Forms.CheckState.Unchecked;
                    AddDirScanResult(updateNumber, newItem);
                    alreadyQueued = true;
                    break;
                }

            // If item is already in the queue, skip it
            if (alreadyQueued)
            {
                complete();
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
                {
                    matches.Add((TvShow)Organization.Shows[j], match);
                }
            }

            // Try to match to temporary show
            lock(directoryScanLock)
                foreach (TvShow show in temporaryShows[updateNumber])
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
                        AddDeleteAction(updateNumber, orgPath, fileCat);
                        break;
                    }

                    bool matched = false;

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


                                if (m.Length > longestMatch && m.Length >= matchWordCnt)
                                {
                                    longestMatch = m.Length;
                                    bestMatch = match.Key;
                                }
                            }
                    }

                    // Episode not matched to a TV show, search database!
                    if (bestMatch == null && !tvOnlyCheck)
                    {
                        // Setup search string
                        string showFile = Path.GetFileNameWithoutExtension(orgPath.Path);

                        // Setup path for resulting content
                        ContentRootFolder defaultTvFolder;
                        string path = NO_TV_FOLDER;
                        if (Settings.GetDefaultTvFolder(out defaultTvFolder))
                            path = defaultTvFolder.FullPath;

                        // Perform search for matching TV show
                        bestMatch = SearchHelper.TvShowSearch.ContentMatch(showFile, path, string.Empty);
                        bestMatch.RootFolder = Path.Combine(path, bestMatch.Name);
                        TvDatabaseHelper.FullShowSeasonsUpdate(bestMatch);

                        // Save show in temporary shows list (in case there are more files that may match to it during scan)
                        lock (directoryScanLock)
                            if (!temporaryShows[updateNumber].Contains(bestMatch))
                                temporaryShows[updateNumber].Add(bestMatch);
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
                            if (bestMatch.FindEpisode(seasonNum, episodeNum1, out episode1))
                            {
                                if (episodeNum2 != -1)
                                    bestMatch.FindEpisode(seasonNum, episodeNum2, out episode2);

                                OrgAction action = orgPath.Copy ? OrgAction.Copy : OrgAction.Move;

                                // If item episode already exists set action to duplicate
                                if (episode1.Missing == TvEpisode.MissingStatus.Located)
                                {
                                    if (background || episode1.Ignored)
                                    {
                                        complete();
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
                                AddDirScanResult(updateNumber, newItem);
                                matched = true;
                            }
                        }
                    }

                    // No match to TV show
                    if (!matched && !tvOnlyCheck)
                    {
                        // Try to match to a movie
                        OrgItem movieItem;
                        if (CreateMovieAction(orgPath, out movieItem))
                            AddDirScanResult(updateNumber, movieItem);
                        else
                        {
                            OrgItem newItem = new OrgItem(OrgAction.None, orgPath.Path, fileCat, orgPath.OrgFolder);
                            AddDirScanResult(updateNumber, newItem);
                        }
                    }

                    break;

                // Movie item
                case FileHelper.FileCategory.NonTvVideo:
                    // Create action
                    OrgItem item;
                    CreateMovieAction(orgPath, out item);

                    // If delete action created (for sample file)
                    if (item.Action == OrgAction.Delete)
                        AddDeleteAction(updateNumber, orgPath, fileCat);
                    else
                        AddDirScanResult(updateNumber, item);
                    break;

                // Trash
                case FileHelper.FileCategory.Trash:
                    AddDeleteAction(updateNumber, orgPath, fileCat);
                    break;
                // Ignore
                case FileHelper.FileCategory.Ignored:
                    AddDirScanResult(updateNumber, new OrgItem(OrgAction.None, orgPath.Path, fileCat, orgPath.OrgFolder));
                    break;
                // Unknown
                default:
                    AddDirScanResult(updateNumber, new OrgItem(OrgAction.None, orgPath.Path, fileCat, orgPath.OrgFolder));
                    break;
            }

            complete();
        }

        /// <summary>
        /// Tries to create a movie action item for a scan from a file.
        /// </summary>
        /// <param name="file">The file to create movie action from</param>
        /// <param name="item">The resulting movie action</param>
        /// <returns>Whether the file was matched to a movie</returns>
        private static bool CreateMovieAction(OrgPath file, out OrgItem item)
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
            Movie searchResult = SearchHelper.MovieSearch.ContentMatch(search, path, string.Empty);

            // Add closest match item
            if (!string.IsNullOrEmpty(searchResult.Name))
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
        private static void AddDeleteAction(int scanNum, OrgPath file, FileHelper.FileCategory fileCat)
        {
            OrgItem newItem;
            if (file.AllowDelete)
            {
                newItem = new OrgItem(OrgAction.Delete, file.Path, fileCat, file.OrgFolder);
                newItem.Check = System.Windows.Forms.CheckState.Checked;
                AddDirScanResult(scanNum, newItem);
            }
            else
            {
                newItem = new OrgItem(OrgAction.None, file.Path, fileCat, file.OrgFolder);
                AddDirScanResult(scanNum, newItem);
            }
        }

        #endregion

        #region TV Missing/Rename Check

        /// <summary>
        /// Run through all TV shows all looks for episodes that may need to be renamed and for missing episodes.
        /// For missing episodes it attempts to match them to files from the search directories.
        /// </summary>
        /// <param name="shows">Shows to scan</param>
        /// <param name="queuedItems">Items currently in queue (to be skipped)</param>
        /// <returns></returns>
        public static List<OrgItem> RunTvCheckScan(List<Content> shows, List<OrgItem> queuedItems)
        {
            // Set running flag
            scanRunning = true;
            
            // Do directory check on all directories (to look for missing episodes)
            List<OrgItem> directoryItems =  DirectoryScan(Settings.ScanDirectories, queuedItems, 70, true, false);

            // Initialiaze scan items
            List<OrgItem> missingCheckItem = new List<OrgItem>();

            // Initialize item numbers
            int number = 0;

            // Go through each show
            for (int i = 0; i < shows.Count; i++)
            {
                TvShow show = (TvShow)shows[i];
                
                if (cancelRequested)
                    break;

                OnScanProgressChange(ScanProcess.Tv, shows[i].Name, (int)Math.Round((double)i / (shows.Count) * 30) + 70);
                
                // Go through missing episodes
                //List<TvEpisode> missingEps = show.GetMissingEpisodes();
                foreach (TvSeason season in show.Seasons)
                {
                    if (cancelRequested)
                        break;
                    
                    foreach (TvEpisode ep in season.Episodes)
                    {
                        // Check for cancellation
                        if (cancelRequested)
                            break;

                        // Skipped ignored episodes
                        if (ep.Ignored)
                            continue;

                        // Init found flag
                        bool found = false;

                        // Missing check
                        if (ep.Missing == TvEpisode.MissingStatus.Missing || ep.Missing == TvEpisode.MissingStatus.InScanDirectory)
                        {
                            if (show.DoMissingCheck)
                            {
                                // Check directory item for episode
                                foreach (OrgItem item in directoryItems)
                                    if ((item.Action == OrgAction.Move || item.Action == OrgAction.Copy) && show.CheckFileToShow(Path.GetFileName(item.SourcePath)))
                                    {
                                        // Only add item for first part of multi-part file
                                        if (ep.Equals(item.TvEpisode))
                                        {
                                            OrgItem newItem = new OrgItem(OrgStatus.Found, item.Action, item.SourcePath, item.DestinationPath, ep, item.TvEpisode2, FileHelper.FileCategory.TvVideo, item.ScanDirectory);
                                            newItem.Check = System.Windows.Forms.CheckState.Checked;
                                            newItem.Number = number++;
                                            if (!shows[i].IncludeInScan)
                                                newItem.Category = FileHelper.FileCategory.Ignored;
                                            missingCheckItem.Add(newItem);
                                            found = true;
                                            break;
                                        }
                                        else if (ep.Equals(item.TvEpisode2))
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                            }
                            else
                                continue;
                        }
                        // Rename check
                        else 
                        {
                            if (shows[i].DoRenaming)
                            {
                                found = true;
                                TvEpisode ep2 = null;

                                if (ep.File.MultiPart)
                                {
                                    if (ep.File.Part == 1)
                                    {
                                        // TODO: Need episode collection for addressing episodes like seasons, so I can do the following:
                                        // TvEpisode ep2 = show.Seasons[ep.Season].Episodes[ep.Number + 1];
                                        // instead of the below:
                                        foreach (TvEpisode epEnumerated in show.Seasons[ep.Season].Episodes)
                                            if (epEnumerated.Number == ep.Number + 1)
                                            {
                                                ep2 = epEnumerated;
                                                break;
                                            }
                                    }
                                    else
                                        continue;
                                }

                                // Build desired path
                                string builtPath = show.BuildFilePath(ep, ep2, Path.GetExtension(ep.File.FilePath));

                                // Check if rename needed (or move within folder)
                                if (ep.File.FilePath != builtPath)
                                {
                                    OrgItem newItem = new OrgItem(OrgStatus.Organization, OrgAction.Rename, ep.File.FilePath, builtPath, ep, ep2, FileHelper.FileCategory.TvVideo, null);
                                    newItem.Check = System.Windows.Forms.CheckState.Checked;
                                    if (!shows[i].IncludeInScan)
                                        newItem.Category = FileHelper.FileCategory.Ignored;
                                    newItem.Number = number++;
                                    missingCheckItem.Add(newItem);
                                }
                            }
                            else
                                continue;
                        }

                        // TODO: loose file check - do directory scan on TV root folder with recursion off..

                        // Add empty item for missing
                        if (!found && ep.Aired && show.DoMissingCheck)
                        {
                            OrgItem newItem = new OrgItem(OrgStatus.Missing, OrgAction.None, ep, null, FileHelper.FileCategory.TvVideo, null);
                            if (!shows[i].IncludeInScan)
                                newItem.Category = FileHelper.FileCategory.Ignored;
                            newItem.Number = number++;
                            missingCheckItem.Add(newItem);
                        }
                    }
                }

                // Check if show folder needs to be renamed!
                string builtFolder = Path.Combine(shows[i].RootFolder, FileHelper.GetSafeFileName(shows[i].Name));
                if (shows[i].Path != builtFolder)
                {
                    OrgItem newItem = new OrgItem(OrgStatus.Organization, OrgAction.Rename, shows[i].Path, builtFolder, new TvEpisode("", shows[i].Name, -1, -1, "", ""), null, FileHelper.FileCategory.Folder, null);
                    newItem.Check = System.Windows.Forms.CheckState.Checked;
                    newItem.Number = number++;
                    missingCheckItem.Add(newItem);
                }
            }

            // Update progress
            OnScanProgressChange(ScanProcess.Tv, string.Empty, 100);

            // Clear flags
            scanRunning = false;
            cancelRequested = false;

            // Return results
            return missingCheckItem;
        }


        #endregion

        #region Movie Rename Check

        /// <summary>
        /// Get unknown files (files that are not in a content folder) from list of movie root folders .
        /// </summary>
        /// <param name="folders">Root folders to look for files in</param>
        /// <returns>List of unknown files</returns>
        private static List<OrgPath> GetUnknownFiles(List<ContentRootFolder> folders)
        {
            // Initialize file list
            List<OrgPath> files = new List<OrgPath>();

            // Get files from each folder
            foreach (ContentRootFolder folder in folders)
                GetUnknownFiles(folder, files);

            // Return files
            return files;
        }

        /// <summary>
        /// Recursively gets unknown files (files that are not in a content folder) from a movie root folder and its children.
        /// </summary>
        /// <param name="folder">Root folders to look for files in</param>
        /// <param name="files">List of files found to add to</param>
        private static void GetUnknownFiles(ContentRootFolder folder, List<OrgPath> files)
        {  
            // Only get files from folders that allow organization
            if (folder.AllowOrganizing)
            {
                string[] fileList = Directory.GetFiles(folder.FullPath);
                foreach (string file in fileList)
                    files.Add(new OrgPath(file, true, folder.AllowOrganizing, folder, null));
            }

            // Recursion on sub-content folders
            foreach (ContentRootFolder subfolder in folder.ChildFolders)
                GetUnknownFiles(subfolder, files);
        }

        /// <summary>
        /// Get files from  that are known to belong to a movie (in content folder) from list of root folders
        /// </summary>
        /// <param name="folders">Root folders to look for files in</param>
        /// <returns>List of known files</returns>
        private static List<OrgPath> GetKnownFiles(List<ContentRootFolder> folders)
        {
            // Initialize file list
            List<OrgPath> files = new List<OrgPath>();

            // Get files from each folder
            foreach (ContentRootFolder folder in folders)
                GetKnownFiles(folder, folder.FullPath, files, 0, null);

            // Return files
            return files;
        }

        /// <summary>
        /// Recursively gets known files (files that are in a content folder) from a movie root folder and its children.
        /// </summary>
        /// <param name="folder">Root folders to look for files in</param>
        /// <param name="folderPath">Current folder path</param>
        /// <param name="files">List of files to add to</param>
        /// <param name="depth">Current depth from root folder</param>
        /// <param name="movie">Movie current path belongs to</param>
        private static void GetKnownFiles(ContentRootFolder folder, string folderPath, List<OrgPath> files, int depth, Movie movie)
        {
            // Match to movie
            if (depth == 1)
            {
                foreach (Movie mov in Organization.Movies)
                    if (mov.Path == folderPath)
                    {
                        movie = mov;
                        break;
                    }
            }

            // Only get files from folders that allow organization
            if (folder.AllowOrganizing)
            {
                // Get files only from non-content sub-folders
                if (depth > 0)
                {
                    string[] fileList = Directory.GetFiles(folderPath);
                    foreach (string file in fileList)
                        files.Add(new OrgPath(file, true, folder.AllowOrganizing, folder, movie));
                }

                // Recursion on sub folders
                string[] subDirs = Directory.GetDirectories(folderPath);
                foreach (string subDir in subDirs)
                {
                    ContentRootFolder subDirContent = null;
                    foreach (ContentRootFolder subfolder in folder.ChildFolders)
                        if (subfolder.FullPath == subDir)
                        {
                            subDirContent = subfolder;
                            break;
                        }

                    if (subDirContent != null)
                        GetKnownFiles(subDirContent, subDirContent.FullPath, files, 0, movie);
                    else
                        GetKnownFiles(folder, subDir, files, depth + 1, movie);
                }
            }
   
        }

        /// <summary>
        /// Performs a scan on set of movie folders.
        /// </summary>
        /// <param name="folders">Movie folders to perform scan on</param>
        /// <param name="queuedItems">Items already in the queue</param>
        /// <returns>List of organization items resulting from scan</returns>
        public static List<OrgItem> MovieFolderScan(List<ContentRootFolder> folders, List<OrgItem> queuedItems)
        {
            // Set scanning flag
            scanRunning = true;

            // Go thorough each folder and create actions for all files
            List<OrgItem> scanResults = new List<OrgItem>();
            MovieFolderScan(folders, scanResults, queuedItems);

            // Clear scanning and cancel flag
            scanRunning = false;
            cancelRequested = false;

            // Return results
            return scanResults;
        }

        /// <summary>
        /// Recursively run a scan on a set of movie folders and their sub-folders
        /// </summary>
        /// <param name="folders">Movie folder to perform scan on</param>
        /// <param name="scanResults">Scan results to build on.</param>
        /// <param name="queuedItems">Items already in the queue</param>
        private static void MovieFolderScan(List<ContentRootFolder> folders, List<OrgItem> scanResults, List<OrgItem> queuedItems)
        {
            // Get unknown files (files not in content folder) from root folders
            OnScanProgressChange(ScanProcess.FileCollect, string.Empty, 0);
            List<OrgPath> files = GetUnknownFiles(folders);

            // Initialize item numbering
            int number = 0;

            // Go through unknwon files and try to match each file to a movie
            for (int i = 0; i < files.Count; i++)
            {
                // Check for cancellation
                if (cancelRequested)
                    break;
                
                // Update progress
                OnScanProgressChange(ScanProcess.Movie, files[i].Path, (int)Math.Round((double)i / files.Count * 70));

                // Categorize the file
                FileHelper.FileCategory fileCat = FileHelper.CategorizeFile(files[i]);

                // Check that video file (tv is okay, may match incorrectly)
                if (fileCat != FileHelper.FileCategory.NonTvVideo && fileCat != FileHelper.FileCategory.TvVideo)
                    continue;

                // Check that file is not already in the queue
                bool alreadyQueued = false;
                foreach (OrgItem queued in queuedItems)
                    if (queued.SourcePath == files[i].Path)
                    {
                        OrgItem newItem = new OrgItem(queued);
                        newItem.Action = OrgAction.Queued;
                        newItem.Number = number++;
                        scanResults.Add(newItem);
                        alreadyQueued = true;
                        break;
                    }
                if (alreadyQueued)
                    continue;

                // Try to match file to movie
                string search = Path.GetFileNameWithoutExtension(files[i].Path);
                Movie searchResult = SearchHelper.MovieSearch.ContentMatch(search, files[i].RootFolder.FullPath, string.Empty);

                // Add closest match item
                OrgItem item = new OrgItem(OrgAction.None, files[i].Path, fileCat, files[i].OrgFolder);
                item.Number = number++;
                if (!string.IsNullOrEmpty(searchResult.Name))
                {
                    item.Action = OrgAction.Move;
                    item.DestinationPath = searchResult.BuildFilePath(files[i].Path);
                    searchResult.Path = searchResult.BuildFolderPath();
                    item.Movie = searchResult;
                    item.Check = System.Windows.Forms.CheckState.Checked;
                }
                scanResults.Add(item);
            }

            // Get knwon movie files (files from within content folders)
            files = GetKnownFiles(folders);

            // Go through each known file and check if renaming is needed
            for (int i = 0; i < files.Count; i++)
            {
                // Check for cancellation
                if (cancelRequested)
                    break;

                // Update progress
                OnScanProgressChange(ScanProcess.Movie, files[i].Path, (int)Math.Round((double)i / files.Count * 20) + 70);

                // Categorize the file
                FileHelper.FileCategory fileCat = FileHelper.CategorizeFile(files[i]);

                // Check that video file (tv is okay, may match incorrectly)
                if (fileCat != FileHelper.FileCategory.NonTvVideo && fileCat != FileHelper.FileCategory.TvVideo)
                    continue;

                // Check that movie is valide
                Movie movie = (Movie)files[i].Content;
                if (movie == null || string.IsNullOrEmpty(movie.Name))
                    continue;

                // Check that file is not already in the queue
                bool alreadyQueued = false;
                foreach (OrgItem queued in queuedItems)
                    if (queued.SourcePath == files[i].Path)
                    {
                        alreadyQueued = true;
                        break;
                    }
                if (alreadyQueued)
                    continue;

                // Check if file needs to be renamed
                string newPath = movie.BuildFilePathNoFolderChanges(files[i].Path);
                if (newPath != files[i].Path)
                {
                    // Add rename to results
                    OrgItem item = new OrgItem(OrgAction.Rename, files[i].Path, FileHelper.FileCategory.NonTvVideo, files[i].OrgFolder);
                    item.Number = number++;
                    if(Path.GetDirectoryName(newPath) != Path.GetDirectoryName(files[i].Path))
                        item.Action = OrgAction.Move;

                    item.DestinationPath = newPath;
                    item.Movie = movie;
                    item.Check = System.Windows.Forms.CheckState.Checked;
                    scanResults.Add(item);
                }
            }

            // Check if any movie folders need to be renamed!
            foreach (Movie movie in Organization.GetContentFromRootFolders(folders)) 
            {
                if (!string.IsNullOrEmpty(movie.Name) && movie.Path != movie.BuildFolderPath())
                {
                    OrgItem item = new OrgItem(OrgAction.Rename, movie.Path, FileHelper.FileCategory.Folder, movie, movie.BuildFolderPath(), null);
                    item.Check = System.Windows.Forms.CheckState.Checked;
                    item.Number = number++;
                    scanResults.Add(item);
                }
            }

            // Update progress
            OnScanProgressChange(ScanProcess.Movie, string.Empty, 100);
        }

        #endregion
    }
}

