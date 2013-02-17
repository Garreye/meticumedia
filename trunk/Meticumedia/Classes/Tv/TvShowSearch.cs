// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Meticumedia
{
    /// <summary>
    /// Class for performing database searching for a TV show.
    /// </summary>
    public class TvShowSearch : ContentSearch
    {
        /// <summary>
        /// Performs search for TV in database
        /// </summary>
        /// <param name="search">string to search for</param>
        /// <param name="includeSummaries">whether to get summary of each return search result</param>
        /// <returns>List of content matching search string from online database</returns>
        protected override List<Content> PerformSearch(string search, bool includeSummaries)
        {
            return TvDatabaseHelper.PerformTvShowSearch(search, includeSummaries);
        }

        /// <summary>
        /// Create lsit of searches bases for matching to a TV show from base search string:
        ///   - no changes to base
        ///   - everything trimmed after episode number (e.g. "seinfeld." from "seinfeld.s01e01.pilot")
        ///   - with episode number removed
        /// </summary>
        /// <param name="baseSearch">Starting search string</param>
        /// <returns>List of strings to use as search starting points</returns>
        protected override List<string> GetModifiedSearches(string baseSearch)
        {
            List<string> baseSearches = new List<string>();
            baseSearches.Add(FileHelper.TrimFromEpisodeInfo(baseSearch));
            baseSearches.Add(FileHelper.RemoveEpisodeInfo(baseSearch));
            baseSearches.Add(baseSearch);
            return baseSearches;
        }

        /// <summary>
        /// Attempts to match string to show from the online database.
        /// </summary>
        /// <param name="search">Search string to match against</param>
        /// <param name="rootFolder">The root folder the content will belong to</param>
        /// <param name="folderPath">Folder path where the content should be moved to</param>
        /// <returns>Match show item, null if no match</returns>
        public new TvShow ContentMatch(string search, string rootFolder, string folderPath)
        {
            return this.ContentMatch(search, rootFolder, folderPath, true);
        }

        /// <summary>
        /// Attempts to match string to show from the online database.
        /// </summary>
        /// <param name="search">Search string to match against</param>
        /// <param name="rootFolder">The root folder the content will belong to</param>
        /// <param name="folderPath">Folder path where the content should be moved to</param>
        /// <param name="threaded">Whether search is threaded, setting to false can help with debugging</param>
        /// <returns>Match show item, null if no match</returns>
        public new TvShow ContentMatch(string search, string rootFolder, string folderPath, bool threaded)
        {
            Content match = base.ContentMatch(search, rootFolder, folderPath, threaded);
            TvShow show = new TvShow(match);
            return TvDatabaseHelper.FullShowSeasonsUpdate(show);
        }

        /// <summary>
        /// Match a folder path to show in database
        /// </summary>
        /// <param name="rootFolder">Root folder content will belong to</param>
        /// <param name="path">Current path of content</param>
        /// <returns>Show from database that was matched, null if no match</returns>
        public new TvShow PathMatch(string rootFolder, string path)
        {
            Content match = base.PathMatch(rootFolder, path);
            TvShow show = new TvShow(match);
            return TvDatabaseHelper.FullShowSeasonsUpdate(show);
        }
    }
}
