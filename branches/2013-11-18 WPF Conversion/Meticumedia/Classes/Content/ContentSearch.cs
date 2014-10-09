// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Abstract class that defines searching for and matching content from online database.
    /// Creates multiple search queries and runs them in separate threads to minimize time
    /// to get maximum number of results.
    /// </summary>
    public abstract class ContentSearch
    {
        /// <summary>
        /// Type of content bein searched for
        /// </summary>
        public ContentType ContentType { get; set; }
        
        #region Search Result Classe

        /// <summary>
        /// Class for single search result
        /// </summary>
        private class SearchResult
        {
            /// <summary>
            /// Content from search results
            /// </summary>
            public Content Content { get; set; }

            /// <summary>
            /// Modifications made to search string
            /// </summary>
            public ContentSearchMod Mods { get; set; }

            /// <summary>
            /// String that was matched to content
            /// </summary>
            public string MatchedString { get; set; }

            /// <summary>
            /// Default constructor
            /// </summary>
            public SearchResult()
            {
                this.Content = null;
                this.Mods = ContentSearchMod.None;
            }
        }

        #endregion

        #region Match Status Class

        /// <summary>
        /// Containt status for all queries from a single search.
        /// </summary>
        private class MatchStatus
        {
            #region Variables

            /// <summary>
            /// Best matches from each thread in search
            /// </summary>
            private List<SearchResult>[] matches;

            /// <summary>
            /// Searches that have been completed
            /// </summary>
            private bool[] completes;

            /// <summary>
            /// Searches have been started (thread added to thread pool)
            /// </summary>
            private bool[] starts;

            #endregion

            #region Properties

            public ContentType ContentType { get; set; }

            /// <summary>
            /// Get number of searches started
            /// </summary>
            /// <param name="status">Search status</param>
            /// <returns>Number of searches started</returns>
            public int NumStarted
            {
                get
                {
                    int starts = 0;
                    foreach (bool started in this.starts)
                        if (started)
                            ++starts;
                    return starts;
                }
            }

            /// <summary>
            /// Get number of searches completed
            /// </summary>
            /// <param name="status">Search status</param>
            /// <returns>Number of searches completed</returns>
            public int NumCompleted
            {
                get
                {
                    int completed = 0;
                    foreach (bool complete in this.completes)
                        if (complete)
                            ++completed;
                    return completed;
                }
            }

            #endregion

            #region Constructor

            /// <summary>
            /// Constructor with number of threads search will contain
            /// </summary>
            /// <param name="numSearches"></param>
            public MatchStatus(int numSearches, ContentType contentType)
            {
                this.matches = new List<SearchResult>[numSearches];
                this.completes = new bool[numSearches];
                this.starts = new bool[numSearches];
                this.ContentType = contentType;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Get search match with lowest modification to search string
            /// </summary>
            /// <param name="status">Search status instance</param>
            /// <param name="lowestModsMatchStrLen">Length of best result's content name</param>
            /// <param name="results">Best resulting content</param>
            /// <returns>Whether a valid content match result was found</returns>
            public bool GetSearchResultWithLowestMods(out ContentSearchMod modsOnResultsSearch, out Content results)
            {
                int lowestModsMatchStrLen = 0;
                modsOnResultsSearch = ContentSearchMod.All;

                switch (this.ContentType)
                {
                    case ContentType.Movie:
                        results = new Movie();
                        break;
                    case ContentType.TvShow:
                        results = new TvShow();
                        break;
                    default:
                        throw new Exception("Unknown content type");
                }

                // Use match with lowest amount of modification made to search string and longest length (I think this is the most likely to the content we actually want to match to)
                for (int i = 0; i < this.matches.Length; i++)
                    if (this.matches[i] != null)
                        for (int j = 0; j < this.matches[i].Count; j++)
                        {
                            if (this.matches[i][j].Mods < modsOnResultsSearch || (this.matches[i][j].Mods == modsOnResultsSearch && this.matches[i][j].MatchedString.Length > lowestModsMatchStrLen))
                            {
                                results = this.matches[i][j].Content;
                                modsOnResultsSearch = this.matches[i][j].Mods;
                                lowestModsMatchStrLen = this.matches[i][j].MatchedString.Length;
                            }

                        }
                return !string.IsNullOrWhiteSpace(results.DatabaseName);
            }

            public void SetSearchMatches(int index, List<SearchResult> matches)
            {
                this.matches[index] = matches;
                this.completes[index] = true;
            }

            public void SetSearchStarted(int index)
            {
                this.starts[index] = true;
            }

            #endregion
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ContentSearch()
        {
            this.ContentType = Classes.ContentType.Undefined;
        }

        #endregion

        #region Searching

        /// <summary>
        /// Match a folder path to content in database
        /// </summary>
        /// <param name="rootFolder">Root folder content will belong to</param>
        /// <param name="path">Current path of content</param>
        /// <returns>Content from database that was matched, null if no match</returns>
        protected bool PathMatch(string rootFolder, string path, bool fast, out Content match)
        {
            // Get folder name from full path
            string[] dirs = path.Split('\\');
            string endDir = dirs[dirs.Length - 1];

            // Do match
            return ContentMatch(endDir, rootFolder, path, fast, out match);
        }

        /// <summary>
        /// Count of search threads created
        /// </summary>
        private int searchCount = 0;

        /// <summary>
        /// Lock for accessing search variables
        /// </summary>
        private object searchLock = new object();

        /// <summary>
        /// Status of all currently running searches - indexed by search number
        /// </summary>
        private Dictionary<int, MatchStatus> searchStatus = new Dictionary<int, MatchStatus>();

        /// <summary>
        /// Attempts to match string to content from the online database.
        /// </summary>
        /// <param name="search">Search string to match against</param>
        /// <param name="rootFolder">The root folder the content will belong to</param>
        /// <param name="folderPath">Folder path where the content should be moved to</param>
        /// <returns>Match content item, null if no match</returns>
        protected bool ContentMatch(string search, string rootFolder, string folderPath, bool fast, out Content match)
        {
            // Create empty content
            Content emptyContent;
            switch (this.ContentType)
            {
                case ContentType.Movie:
                    emptyContent = new Movie();
                    break;
                case ContentType.TvShow:
                    emptyContent = new TvShow();
                    break;
                default:
                    throw new Exception("Unknown content type");
            }
            emptyContent.Path = folderPath;
            emptyContent.RootFolder = rootFolder;
            emptyContent.Found = true;

            // Check for empty search condition
            if (string.IsNullOrEmpty(search))
            {
                match = emptyContent;
                return false;
            }

            // Get year from search string
            int dirYear = FileHelper.GetYear(search);

            // Get list of simplified strings
            List<FileHelper.SimplifyStringResults> searches = new List<FileHelper.SimplifyStringResults>();
            
            // Get list of search bases
            List<string> searchBases = GetModifiedSearches(search);

            // Fast search - use bases
            if (fast)
            {
                FileHelper.SimplifyStringResults result = FileHelper.BuildSimplifyResults(searchBases[0], false, false, FileHelper.OptionalSimplifyRemoves.YearAndFollowing, true, false, true, false);
                searches.Add(result);
            }
            // Full search: Go through each search base and get simplified search options
            else
                foreach (string searchBase in searchBases)
                {
                    // Get results from current base
                    List<FileHelper.SimplifyStringResults> currSearches = FileHelper.SimplifyString(searchBase);
                    currSearches.Add(new FileHelper.SimplifyStringResults(searchBase, new Dictionary<FileWordType, List<string>>(), ContentSearchMod.None));

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
                            searches.Add(results);
                    }
                }
            searches.Sort();

            // Create new status
            int currSeachCnt;
            MatchStatus status;
            lock (searchLock)
            {
                currSeachCnt = ++searchCount;
                status = new MatchStatus(searches.Count, this.ContentType);
                searchStatus.Add(currSeachCnt, status);
            }

            ContentSearchMod lowMods;
            Content lowestModsMatch;

            // Add thread to pool for each search that need to be performed
            int searchNum = 0;
            while (searchNum < searches.Count)
            {
                // Check for any search results so far
                if (status.GetSearchResultWithLowestMods(out lowMods, out lowestModsMatch))
                {
                    // If search results have no mods or just year removed use them as final results
                    if (lowMods == ContentSearchMod.None || lowMods == ContentSearchMod.YearRemoved)
                    {
                        match = lowestModsMatch;
                        return true;
                    }
                }

                // Limit number of search threads created
                if (status.NumStarted - status.NumCompleted >= Settings.NumSimultaneousSearches)
                {
                    Thread.Sleep(100);
                    continue;
                }

                // Add next search to thread pool
                object[] args = { currSeachCnt, searchNum, searches[searchNum].SimplifiedString, folderPath, rootFolder, dirYear, searches[searchNum].Modifications };
                ThreadPool.QueueUserWorkItem(new WaitCallback(SearchThread), args);
                lock (searchLock)
                    status.SetSearchStarted(searchNum);

                searchNum++;
            }

            // Wait for all search to complete
            while (status.NumCompleted < searches.Count)
            {
                // Check for any search results so far
                if (status.GetSearchResultWithLowestMods(out lowMods, out lowestModsMatch))
                {
                    // If search results have no mods or just year removed use them as final results
                    if (lowMods == ContentSearchMod.None || lowMods == ContentSearchMod.YearRemoved)
                    {
                        match = lowestModsMatch;
                        return true;
                    }
                }

                Thread.Sleep(100);
            }

            // Clear status
            lock (searchLock)
                searchStatus.Remove(currSeachCnt);

            // Return result with lowest mods to search string
            if (status.GetSearchResultWithLowestMods(out lowMods, out lowestModsMatch))
            {
                match = lowestModsMatch;
                return true;
            }
            else
            {
                match = emptyContent;
                return false;
            }
        }

        /// <summary>
        /// Method for create multiple searches bases specific to inheriting class.
        /// If not overriden simply returns list with the input as the only item.
        /// </summary>
        /// <param name="baseSearch">Starting search string</param>
        /// <returns>List of strings to use as search starting points</returns>
        protected virtual List<string> GetModifiedSearches(string baseSearch)
        {
            List<string> baseSearches = new List<string>();
            baseSearches.Add(baseSearch);
            return baseSearches;
        }

        /// <summary>
        /// Search thread operation. Performs a single search for content from string passed in through arguments.
        /// </summary>
        /// <param name="stateInfo">Arguments for thread</param>
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
            ContentSearchMod mods = (ContentSearchMod)args[6];

            // Check search is still active
            List<SearchResult> matches = new List<SearchResult>();
            if (searchStatus.ContainsKey(statusIndex))
                DoMatch(search, folderPath, movieFolder, year, mods, out matches);

            // Check search is still active then put results into status
            lock (searchLock)
                if (searchStatus.ContainsKey(statusIndex))
                    searchStatus[statusIndex].SetSearchMatches(searchIndex, matches);
        }

        /// <summary>
        /// Search database for content and find a match from results.
        /// </summary>
        /// <param name="search">Search string to match content to</param>
        /// <param name="folderPath">Path of folder containing content</param>
        /// <param name="rootFolder">Root folder containing content folder</param>
        /// <param name="year">Year to match to content</param>
        /// <param name="result">resulting match found from search</param>
        /// <returns>whether match was successful</returns>
        private bool DoMatch(string search, string folderPath, string rootFolder, int year, ContentSearchMod baseMods, out List<SearchResult> matches)
        {
            // Search for content
            List<Content> searchResults = PerformSearch(search, false);

            // Initialize resutls
            matches = new List<SearchResult>();

            // Go through results
            if (searchResults != null)
                foreach (Content searchResult in searchResults)
                {
                    SearchResult result = new SearchResult();
                    result.Mods = baseMods;

                    // Verify year in result matches year from folder (if any)
                    if (year != -1 && Math.Abs(year - searchResult.DatabaseYear) > 3)
                        continue;

                    // Check if search string match results string
                    string simplifiedSearch = FileHelper.SimplifyFileName(search);
                    string dbContentName = FileHelper.SimplifyFileName(searchResult.DatabaseName);

                    bool theAddedToMatch;
                    bool singleLetterDiff;

                    // Try basic match
                    bool match = FileHelper.CompareStrings(simplifiedSearch, dbContentName, out theAddedToMatch, out singleLetterDiff);
                    result.MatchedString = simplifiedSearch;

                    // Try match with year removed
                    if (!match)
                    {
                        simplifiedSearch = FileHelper.SimplifyFileName(search, true, true, false);
                        dbContentName = FileHelper.SimplifyFileName(searchResult.DatabaseName, true, true, false);
                        match = FileHelper.CompareStrings(simplifiedSearch, dbContentName, out theAddedToMatch, out singleLetterDiff);
                        result.MatchedString = simplifiedSearch;
                    }

                    // Try match with country removed
                    if (!match)
                    {
                        simplifiedSearch = FileHelper.SimplifyFileName(search, true, true, true);
                        dbContentName = FileHelper.SimplifyFileName(searchResult.DatabaseName, true, true, true);
                        match = FileHelper.CompareStrings(simplifiedSearch, dbContentName, out theAddedToMatch, out singleLetterDiff);
                        if (match)
                            result.Mods |= ContentSearchMod.WordsRemoved;
                        result.MatchedString = simplifiedSearch;
                    }

                    // Try match with spaces removed
                    if (!match)
                    {
                        string dirNoSpc = simplifiedSearch.Replace(" ", "");
                        string nameNoSpc = dbContentName.Replace(" ", "");
                        match = FileHelper.CompareStrings(dirNoSpc, nameNoSpc, out theAddedToMatch, out singleLetterDiff);
                        result.MatchedString = simplifiedSearch;
                        if (match)
                            result.Mods |= ContentSearchMod.SpaceRemoved;
                    }

                    // Try match with year added to content name
                    if (!match)
                    {
                        simplifiedSearch = FileHelper.SimplifyFileName(search);
                        dbContentName = FileHelper.SimplifyFileName(searchResult.DatabaseName + " " + searchResult.DatabaseYear.ToString());
                        match = FileHelper.CompareStrings(simplifiedSearch, dbContentName, out theAddedToMatch, out singleLetterDiff);
                        result.MatchedString = simplifiedSearch;
                    }

                    // No match, next result!
                    if (!match)
                        continue;

                    if (theAddedToMatch)
                        result.Mods |= ContentSearchMod.TheAdded;
                    if (singleLetterDiff)
                        result.Mods |= ContentSearchMod.SingleLetterAdded;

                    // Set results folder/path
                    result.Content = searchResult;
                    result.Content.RootFolder = rootFolder;
                    if (string.IsNullOrEmpty(folderPath))
                        result.Content.Path = result.Content.BuildFolderPath();
                    else
                        result.Content.Path = folderPath;

                    // Save results
                    matches.Add(result);
                }

            return matches.Count > 0;
        }

        /// <summary>
        /// Performs search for content in database - specific to content type
        /// </summary>
        /// <param name="search">string to search for</param>
        /// <param name="includeSummaries">whether to get summary of each return search result</param>
        /// <returns>List of content matching search string from online database</returns>
        protected abstract List<Content> PerformSearch(string search, bool includeSummaries);

        #endregion
    }
}
