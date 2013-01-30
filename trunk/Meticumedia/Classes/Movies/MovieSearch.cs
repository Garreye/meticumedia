// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meticumedia
{
    public class MovieSearch : ContentSearch
    {
        protected override List<Content> PerformSearch(string search, bool includeSummaries)
        {
            return TheMovieDbHelper.PerformMovieSearch(search);
        }

        protected override List<string> GetModifiedSearches(string baseSearch)
        {
            List<string> baseSearches = new List<string>();
            baseSearches.Add(baseSearch);
            return baseSearches;
        }

        public new Movie ContentMatch(string search, string rootFolder, string folderPath)
        {
            return this.ContentMatch(search, rootFolder, folderPath, true);
        }

        public new Movie ContentMatch(string search, string rootFolder, string folderPath, bool threaded)
        {
            Content match = base.ContentMatch(search, rootFolder, folderPath, threaded);
            Movie movie = new Movie(match);
            return TheMovieDbHelper.UpdateMovieInfo(movie); 
        }

        public new Movie PathMatch(string rootFolder, string path)
        {
            Content match = base.PathMatch(rootFolder, path);
            Movie movie = new Movie(match);
            if (movie.Id > 0)
                return TheMovieDbHelper.UpdateMovieInfo(movie);
            else
                return movie;
        }

    }
}
