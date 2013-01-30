// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Meticumedia
{
    /// <summary>
    /// Abstract class that handle searching for and matching content from online database.
    /// Create multiple search queries and runs them in separate threads to minimize time
    /// to get maximum number of results.
    /// </summary>
    public abstract class ContentSearch
    {
        #region Match Status Class

        /// <summary>
        /// Containt status for all queries from a single search.
        /// </summary>
        private class MatchStatus
        {
            /// <summary>
            /// Best match from each thread in search
            /// </summary>
            public Content[] Matches { get; set; }

            /// <summary>
            /// Flag indicating whether each thread in search is completed
            /// </summary>
            public bool[] Completes { get; set; }

            /// <summary>
            /// Constructor with number of threads search will contain
            /// </summary>
            /// <param name="numSearches"></param>
            public MatchStatus(int numSearches)
            {
                this.Matches = new Content[numSearches];
                this.Completes = new bool[numSearches];
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ContentSearch()
        {
        }

        #endregion

        #region Searching

        /// <summary>
        /// Match a folder path to content in database
        /// </summary>
        /// <param name="rootFolder"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        protected Content PathMatch(string rootFolder, string path)
        {
            // Get folder name from full path
            string[] dirs = path.Split('\\');
            string endDir = dirs[dirs.Length - 1];

            // Do match
            return ContentMatch(endDir, rootFolder, path);
        }

        /// <summary>
        /// Count of search threads created
        /// </summary>
        private int searchCount = 0;

        private object searchLock = new object();

        /// <summary>
        /// Status of all currently running searches - indexed by search number
        /// </summary>
        private Dictionary<int, MatchStatus> searchStatus = new Dictionary<int, MatchStatus>();

        protected Content ContentMatch(string search, string rootFolder, string folderPath)
        {
            return ContentMatch(search, rootFolder, folderPath, true);
        }

        /// <summary>
        /// Attempt to match a folder name to a movie from the online database.
        /// </summary>
        /// <param name="search">Search string to match againg</param>
        /// <param name="rootFolder">The movie folder where the movie is in</param>
        /// <param name="folderPath">Folder path where the movie should be moved to</param>
        /// <returns>Match movie item.</returns>
        protected Content ContentMatch(string search, string rootFolder, string folderPath, bool threaded)
        {
            // Create empty content
            Content emptyContent = new Content();
            emptyContent.Path = folderPath;
            emptyContent.RootFolder = rootFolder;
            emptyContent.Found = true;

            // Check for empty search condition
            if (string.IsNullOrEmpty(search))
                return emptyContent;

            // Get year from search string
            int dirYear = FileHelper.GetYear(search);

            // Get list of simplified strings
            List<FileHelper.SimplifyStringResults> searches = new List<FileHelper.SimplifyStringResults>();
            
            List<string> searchBases = GetModifiedSearches(search);

            foreach (string searchBase in searchBases)
            {
                // Get results from current base
                List<FileHelper.SimplifyStringResults> currSearches = FileHelper.SimplifyString(searchBase);
                currSearches.Add(new FileHelper.SimplifyStringResults(searchBase, new Dictionary<FileWordType, List<string>>()));

                // Add each result to full list of searches
                foreach (FileHelper.SimplifyStringResults results in currSearches)
                {
                    // Check if search already exist
                    bool exists = false;
                    foreach (FileHelper.SimplifyStringResults s in searches)
                        if (s.SimplifiedString == results.SimplifiedString)
                        {
                            exists = true;
                            break;
                        }

                    // If doesn't exist add it to searches
                    if (!exists)
                    {
                        searches.Add(results);
                        //Console.WriteLine(results.SimplifiedString);
                    }
                }
            }

            // Create new status
            int currSeachCnt;
            MatchStatus status;
            lock (searchLock)
            {
                currSeachCnt = ++searchCount;
                status = new MatchStatus(searches.Count);
                searchStatus.Add(currSeachCnt, status);
            }

            // Add thread to pool for each search that need to be performed
            for (int i = 0; i < searches.Count; i++)
            {
                object[] args = { currSeachCnt, i, searches[i].SimplifiedString, folderPath, rootFolder, dirYear };
                if (threaded)
                    ThreadPool.QueueUserWorkItem(new WaitCallback(SearchThread), args);
                else
                    SearchThread(args);

            }

            // Wait for all search to complete
            bool allComplete = false;
            while (!allComplete)
            {
                Thread.Sleep(10);

                // Check if all searches are done
                allComplete = true;
                foreach (bool complete in status.Completes)
                    if (!complete)
                        allComplete = false;
            }

            // Clear status
            lock (searchLock)
                searchStatus.Remove(currSeachCnt);

            // Use match from longest search string (most likely to the content we actually want to match to)
            Content bestMatch = emptyContent;
            int bestMatchStrLen = 0;
            for (int i = 0; i < status.Matches.Length; i++)
                if (status.Matches[i] != null)
                {
                    if (searches[i].SimplifiedString.Length > bestMatchStrLen)
                    {
                        bestMatch = status.Matches[i];
                        bestMatchStrLen = searches[i].SimplifiedString.Length;
                    }
                }

            // Return empty movie
            return bestMatch;
        }

        /// <summary>
        /// Method for create multiple searches bases specific to inheriting class.
        /// (e.g. for TV show match we want to search with full path name and then
        /// with a substring up to episode information - "seinfeld" from "seinfeld s01e01 xvid 480p rip bullshit")
        /// </summary>
        /// <param name="baseSearch"></param>
        /// <returns></returns>
        protected abstract List<string> GetModifiedSearches(string baseSearch);

        /// <summary>
        /// Search thread operation. Performs a single search for content from string passed in through arguments.
        /// </summary>
        /// <param name="stateInfo"></param>
        private void SearchThread(object stateInfo)
        {
            // Get arguments
            object[] args = (object[])stateInfo;
            int statusIndex = (int)args[0];
            int searchIndex = (int)args[1];
            string search = (string)args[2];
            string folderPath = (string)args[3];
            string movieFolder = (string)args[4];
            int year = (int)args[5];

            // Check search is still active
            Content match = null;
            if (searchStatus.ContainsKey(statusIndex))
                DoMatch(search, folderPath, movieFolder, year, out match);

            // Check search is still active then put results into status
            lock (searchLock)
                if (searchStatus.ContainsKey(statusIndex))
                {
                    searchStatus[statusIndex].Matches[searchIndex] = match;
                    searchStatus[statusIndex].Completes[searchIndex] = true;
                }

            //Console.WriteLine("Search thread #" + statusIndex + " - " + searchIndex + ": " + search + " END - " + (match != null));
        }

        /// <summary>
        /// Search database for content and find a match from results.
        /// </summary>
        /// <param name="search">Search string to match movie to</param>
        /// <param name="folderPath">Path of folder containing movie</param>
        /// <param name="rootFolder">Content folder containing movie's folder</param>
        /// <param name="year">Year to match to movie</param>
        /// <param name="movie">resulting match found from search</param>
        /// <returns>whether match was successful</returns>
        private bool DoMatch(string search, string folderPath, string rootFolder, int year, out Content result)
        {
            result = null;

            // Search for movie
            List<Content> searchResults = PerformSearch(search, false);

            // Go through results
            foreach (Content searchResult in searchResults)
            {
                // Verify year in result matches year from folder (if any)
                if (year != -1 && Math.Abs(year - searchResult.Date.Year) > 3)
                    continue;

                // Check if search string match results string
                string simplifiedSearch = FileHelper.SimplifyFileName(search);
                string dbMovieName = FileHelper.SimplifyFileName(searchResult.Name);

                // Try basic match
                bool match = FileHelper.CompareStrings(simplifiedSearch, dbMovieName);

                // Try match with year removed
                if (!match)
                {
                    simplifiedSearch = FileHelper.SimplifyFileName(search, true, true);
                    dbMovieName = FileHelper.SimplifyFileName(searchResult.Name, true, true);
                    match = FileHelper.CompareStrings(simplifiedSearch, dbMovieName);
                }

                // Try match with spaces removed
                if (!match)
                {
                    string dirNoSpc = simplifiedSearch.Replace(" ", "");
                    string nameNoSpc = dbMovieName.Replace(" ", "");
                    match = FileHelper.CompareStrings(dirNoSpc, nameNoSpc);
                }

                // Try match with year added to movie name
                if(!match)
                {
                    dbMovieName = FileHelper.SimplifyFileName(searchResult.Name + " " + searchResult.Date.Year.ToString());
                    match = FileHelper.CompareStrings(simplifiedSearch, dbMovieName);
                }

                // No match, next result!
                if(!match)
                    continue;

                // Get full information for matching result
                result = searchResult;
                result.RootFolder = rootFolder;
                if (string.IsNullOrEmpty(folderPath))
                    result.Path = result.BuildFolderPath();
                else
                    result.Path = folderPath;                 

                // Content found
                return true;
            }

            return false;
        }

        /// <summary>
        /// Performs search for content on database - specific to content type
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        protected abstract List<Content> PerformSearch(string search, bool includeSummaries);


        #endregion

    }
}
