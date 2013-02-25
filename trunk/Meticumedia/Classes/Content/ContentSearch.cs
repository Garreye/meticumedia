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
    /// Abstract class that defines searching for and matching content from online database.
    /// Creates multiple search queries and runs them in separate threads to minimize time
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
            public List<SearchResult>[] Matches { get; set; }

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
                this.Matches = new List<SearchResult>[numSearches];
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
        /// <param name="rootFolder">Root folder content will belong to</param>
        /// <param name="path">Current path of content</param>
        /// <returns>Content from database that was matched, null if no match</returns>
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
        public Content ContentMatch(string search, string rootFolder, string folderPath)
        {
            return ContentMatch(search, rootFolder, folderPath, true);
        }

        /// <summary>
        /// Attempts to match string to content from the online database.
        /// </summary>
        /// <param name="search">Search string to match against</param>
        /// <param name="rootFolder">The root folder the content will belong to</param>
        /// <param name="folderPath">Folder path where the content should be moved to</param>
        /// <param name="threaded">Whether search is threaded, setting to false can help with debugging</param>
        /// <returns>Match content item, null if no match</returns>
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
                object[] args = { currSeachCnt, i, searches[i].SimplifiedString, folderPath, rootFolder, dirYear, searches[i].Modifications };
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

            // Use match with lowest amount of modification made to search string and longest length (I think this s the most likely to the content we actually want to match to)
            int lowestModsMatchStrLen = 0;
            ContentSearchMod lowMods = ContentSearchMod.All;
            Content lowestModsMatch = emptyContent;
            for (int i = 0; i < status.Matches.Length; i++)
                for (int j = 0; j < status.Matches[i].Count; j++)
                {
                    if (status.Matches[i][j].Mods < lowMods || (status.Matches[i][j].Mods == lowMods && status.Matches[i][j].MatchedString.Length > lowestModsMatchStrLen))
                    {
                        lowestModsMatch = status.Matches[i][j].Content;
                        lowMods = status.Matches[i][j].Mods;
                        lowestModsMatchStrLen = status.Matches[i][j].MatchedString.Length;
                    }

                }

            // Return best match
            return lowestModsMatch;
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
                {
                    searchStatus[statusIndex].Matches[searchIndex] = matches;
                    searchStatus[statusIndex].Completes[searchIndex] = true;
                }
        }

        public class SearchResult
        {
            public Content Content { get; set; }
            public ContentSearchMod Mods { get; set; }
            public string MatchedString { get; set; }

            public SearchResult()
            {
                this.Content = null;
                this.Mods = ContentSearchMod.None;
            }
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
        private bool DoMatch(string search, string folderPath, string rootFolder, int year,  ContentSearchMod baseMods, out List<SearchResult> matches)
        {
            // Search for content
            List<Content> searchResults = PerformSearch(search, false);

            // Initialize resutls
            matches = new List<SearchResult>();

            // Go through results
            foreach (Content searchResult in searchResults)
            {
                SearchResult result = new SearchResult();
                result.Mods = baseMods;

                // Verify year in result matches year from folder (if any)
                if (year != -1 && Math.Abs(year - searchResult.Date.Year) > 3)
                    continue;

                // Check if search string match results string
                string simplifiedSearch = FileHelper.SimplifyFileName(search);
                string dbContentName = FileHelper.SimplifyFileName(searchResult.Name);

                bool theAddedToMatch;

                // Try basic match
                bool match = FileHelper.CompareStrings(simplifiedSearch, dbContentName, out theAddedToMatch);
                result.MatchedString = simplifiedSearch;

                // Try match with year removed
                if (!match)
                {
                    simplifiedSearch = FileHelper.SimplifyFileName(search, true, true, false);
                    dbContentName = FileHelper.SimplifyFileName(searchResult.Name, true, true, false);
                    match = FileHelper.CompareStrings(simplifiedSearch, dbContentName, out theAddedToMatch);
                    result.MatchedString = simplifiedSearch;
                }

                // Try match with country removed
                if (!match)
                {
                    simplifiedSearch = FileHelper.SimplifyFileName(search, true, true, true);
                    dbContentName = FileHelper.SimplifyFileName(searchResult.Name, true, true, true);
                    match = FileHelper.CompareStrings(simplifiedSearch, dbContentName, out theAddedToMatch);
                    if (match)
                        result.Mods |= ContentSearchMod.WordsRemoved;
                    result.MatchedString = simplifiedSearch;
                }

                // Try match with spaces removed
                if (!match)
                {
                    string dirNoSpc = simplifiedSearch.Replace(" ", "");
                    string nameNoSpc = dbContentName.Replace(" ", "");
                    match = FileHelper.CompareStrings(dirNoSpc, nameNoSpc, out theAddedToMatch);
                    result.MatchedString = simplifiedSearch;
                    if(match)
                        result.Mods |= ContentSearchMod.SpaceRemoved;
                }

                // Try match with year added to content name
                if(!match)
                {
                    simplifiedSearch = FileHelper.SimplifyFileName(search);
                    dbContentName = FileHelper.SimplifyFileName(searchResult.Name + " " + searchResult.Date.Year.ToString());
                    match = FileHelper.CompareStrings(simplifiedSearch, dbContentName, out theAddedToMatch);
                    result.MatchedString = simplifiedSearch;
                }

                // No match, next result!
                if(!match)
                    continue;

                if (theAddedToMatch)
                    result.Mods |= ContentSearchMod.TheAdded;

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
