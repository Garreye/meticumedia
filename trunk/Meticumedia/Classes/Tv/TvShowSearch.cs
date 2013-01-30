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
    public class TvShowSearch : ContentSearch
    {
        protected override List<Content> PerformSearch(string search, bool includeSummaries)
        {
            return TvDatabaseHelper.PerformTvShowSearch(search, includeSummaries);
        }

        protected override List<string> GetModifiedSearches(string baseSearch)
        {
            List<string> baseSearches = new List<string>();
            baseSearches.Add(baseSearch);
            baseSearches.Add(FileHelper.TrimFromEpisodeInfo(baseSearch));
            baseSearches.Add(FileHelper.RemoveEpisodeInfo(baseSearch));
            return baseSearches;
        }

        public new TvShow ContentMatch(string search, string rootFolder, string folderPath)
        {
            return this.ContentMatch(search, rootFolder, folderPath, true);
        }

        public new TvShow ContentMatch(string search, string rootFolder, string folderPath, bool threaded)
        {
            Content match = base.ContentMatch(search, rootFolder, folderPath, threaded);
            TvShow show = new TvShow(match);
            return TvDatabaseHelper.FullShowSeasonsUpdate(show);
        }

        public new TvShow PathMatch(string rootFolder, string path)
        {
            Content match = base.PathMatch(rootFolder, path);
            TvShow show = new TvShow(match);
            return TvDatabaseHelper.FullShowSeasonsUpdate(show);
        }
    }
}
