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
    /// <summary>
    /// Helper class for accessing TV database.
    /// </summary>
    public class TvDatabaseHelper
    {
        #region Searching/Updating

        private static TvRageAccess TvRageAccess = new TvRageAccess();

        private static TheTvDbAccess TheTvDbAccess = new TheTvDbAccess();

        private static TvDatabaseAccess GetDataBaseAccess(TvDataBaseSelection selection)
        {
            switch (selection)
            {
                case TvDataBaseSelection.TheTvDb:
                    return TheTvDbAccess;
                case TvDataBaseSelection.TvRage:
                    return TvRageAccess;
                default:
                    throw new Exception("Unknown database selection");
            }
        }

        /// <summary>
        /// Gets database server's time
        /// </summary>
        /// <param name="time">retrieved server time</param>
        /// <returns>whether time was successfully retrieved</returns>
        public static bool GetServerTime(TvDataBaseSelection selection, out string time)
        {
            return GetDataBaseAccess(selection).GetServerTime(out time);
        }

        /// <summary>
        /// Return lists of series Ids that need updating. 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool GetDataToBeUpdated(TvDataBaseSelection selection, out List<int> info, out string time)
        {
            return GetDataBaseAccess(selection).GetDataToBeUpdated(out info, out time);
        }

        /// <summary>
        /// Performs search for a show in database.
        /// </summary>
        /// <param name="searchString">The string to search for</param>
        /// <returns>Array of results from the search</returns>
        public static List<Content> PerformTvShowSearch(TvDataBaseSelection selection, string searchString, bool includeSummaries)
        {
            return GetDataBaseAccess(selection).PerformTvShowSearch(searchString, includeSummaries);
        }

        /// <summary>
        /// Gets season/episode information from database.
        /// </summary>
        /// <param name="show">Show to load episode information into</param>
        public static void FullShowSeasonsUpdate(TvShow show)
        {
            GetDataBaseAccess(show.DataBase).FullShowSeasonsUpdate(show);
            show.LastUpdated = DateTime.Now;
        }

        #endregion
    }
}
