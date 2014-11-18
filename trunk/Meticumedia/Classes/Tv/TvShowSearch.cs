// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Class for performing database searching for a TV show.
    /// </summary>
    public class TvShowSearch : ContentSearch
    {
        public TvShowSearch()
        {
            this.ContentType = Classes.ContentType.TvShow;
        }
        
        /// <summary>
        /// Performs search for TV in database
        /// </summary>
        /// <param name="search">string to search for</param>
        /// <param name="includeSummaries">whether to get summary of each return search result</param>
        /// <returns>List of content matching search string from online database</returns>
        protected override List<Content> PerformSearch(string search, bool includeSummaries)
        {
            return TvDatabaseHelper.PerformTvShowSearch(Settings.General.DefaultTvDatabase, search, includeSummaries);
        }

        /// <summary>
        /// Create list of searches bases for matching to a TV show from base search string:
        ///   - no changes to base
        ///   - everything trimmed after episode number (e.g. "seinfeld." from "seinfeld.s01e01.pilot")
        ///   - with episode number removed
        /// </summary>
        /// <param name="baseSearch">Starting search string</param>
        /// <returns>List of strings to use as search starting points</returns>
        protected override List<string> GetModifiedSearches(string baseSearch)
        {
            List<string> baseSearches = new List<string>();

            // Add trimmed search bases first, so they have priority (typically more likely to match to database)
            string trimSearch = FileHelper.TrimFromEpisodeInfo(baseSearch);
            if (trimSearch != baseSearch)
                baseSearches.Add(trimSearch);

            string trimSearch2 = FileHelper.RemoveEpisodeInfo(baseSearch);
            if (trimSearch != baseSearch && trimSearch2 != trimSearch)
                baseSearches.Add(trimSearch);
            baseSearches.Add(baseSearch);

            return baseSearches;
        }

        /// <summary>
        /// Attempts to match string to show from the online database.
        /// </summary>
        /// <param name="search">Search string to match against</param>
        /// <param name="rootFolder">The root folder the content will belong to</param>
        /// <param name="folderPath">Folder path where the content should be moved to</param>
        /// <param name="threaded">Whether search is threaded, setting to false can help with debugging</param>
        /// <returns>Match show item, null if no match</returns>
        public bool ContentMatch(string search, string rootFolder, string folderPath, bool fast, bool threaded, out TvShow match)
        {
            Content contentMatch;
            bool results = base.ContentMatch(search, rootFolder, folderPath, fast, threaded, out contentMatch, null);
            match = new TvShow(contentMatch);
            if (results)
                TvDatabaseHelper.FullShowSeasonsUpdate(match);
            return results;
        }

        /// <summary>
        /// Match a folder path to show in database
        /// </summary>
        /// <param name="rootFolder">Root folder content will belong to</param>
        /// <param name="path">Current path of content</param>
        /// <returns>Show from database that was matched, null if no match</returns>
        public bool PathMatch(string rootFolder, string path, bool fast, bool threaded, out TvShow match)
        {
            Content contentMatch;
            bool results = base.PathMatch(rootFolder, path, fast, threaded, out contentMatch, null);
            match = new TvShow(contentMatch);
            TvDatabaseHelper.FullShowSeasonsUpdate(match);
            return results;
        }
    }
}
