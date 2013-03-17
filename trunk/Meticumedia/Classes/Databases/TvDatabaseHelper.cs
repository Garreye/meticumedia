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

        #region Database Selection

        /// <summary>
        /// Database access instance
        /// </summary>
        private static TvDatabaseAccess databaseAccess = new TvRageAccess();

        /// <summary>
        /// Database selection types
        /// </summary>
        public enum TvDataBaseSelection { TvRage, TheTvDb }

        /// <summary>
        /// Current database selection - TODO: allow user to control in settings!
        /// </summary>
        private static TvDataBaseSelection dataBaseSelection = TvDataBaseSelection.TvRage;

        /// <summary>
        /// Sets database type
        /// </summary>
        /// <param name="selection">Database type to set</param>
        public static void SetDatabase(TvDataBaseSelection selection)
        {
            // No changes if already set
            if (selection == dataBaseSelection)
                return;
            
            // Set database access based on selection type
            switch (selection)
            {
                case TvDataBaseSelection.TvRage:
                    databaseAccess = new TvRageAccess();
                    break;
                case TvDataBaseSelection.TheTvDb:
                    // Removed to avoid using DotNetZip library, until settings setup to allow switching..
                    //databaseAccess = new TheTvDbAccess();
                    break;
                default:
                    throw new Exception("Unknown TV database selected");
            }
        }

        #endregion

        #region Searching/Updating

        /// <summary>
        /// Gets database server's time
        /// </summary>
        /// <param name="time">retrieved server time</param>
        /// <returns>whether time was successfully retrieved</returns>
        public static bool GetServerTime(out string time)
        {
            return databaseAccess.GetServerTime(out time);
        }

        /// <summary>
        /// Return lists of series Ids that need updating. 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool GetDataToBeUpdated(out List<int> info, out string time)
        {
            return databaseAccess.GetDataToBeUpdated(out info, out time);
        }

        /// <summary>
        /// Performs search for a show in database.
        /// </summary>
        /// <param name="searchString">The string to search for</param>
        /// <returns>Array of results from the search</returns>
        public static List<Content> PerformTvShowSearch(string searchString, bool includeSummaries)
        {
            return databaseAccess.PerformTvShowSearch(searchString, includeSummaries);
        }

        /// <summary>
        /// Gets season/episode information from database.
        /// </summary>
        /// <param name="show">Show to load episode information into</param>
        public static void FullShowSeasonsUpdate(TvShow show)
        {
            databaseAccess.FullShowSeasonsUpdate(show);
            show.LastUpdated = DateTime.Now;
        }

        #endregion
    }
}
