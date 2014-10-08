// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Helper class for performing search in online database for movie or tv show.
    /// </summary>
    public static class SearchHelper
    {
        /// <summary>
        /// TV show search instance
        /// </summary>
        public static TvShowSearch TvShowSearch = new TvShowSearch();

        /// <summary>
        /// Movie search instance
        /// </summary>
        public static MovieSearch MovieSearch = new MovieSearch();
    }
}
