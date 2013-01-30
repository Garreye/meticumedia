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
    public class TvDatabaseHelper
    {
        #region Database Selection

        /// <summary>
        /// Database access - change type to TheTvDbAccess to use TheTvDb online database 
        /// </summary>
        private static TvDatabaseAccess databaseAccess = new TvRageAccess();

        public enum TvDataBaseSelection { TvRage, TheTvDb }

        private static TvDataBaseSelection dataBaseSelection = TvDataBaseSelection.TvRage;

        public static void SetDataBase(TvDataBaseSelection selection)
        {
            if (selection == dataBaseSelection)
                return;
            
            switch (selection)
            {
                case TvDataBaseSelection.TvRage:
                    databaseAccess = new TvRageAccess();
                    break;
                case TvDataBaseSelection.TheTvDb:
                    // Removed to avoid using DotNetZip library..
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
        /// Performs search for a show in TheTvDb.
        /// </summary>
        /// <param name="searchString">The string to search for</param>
        /// <returns>Array of results from the search</returns>
        public static List<Content> PerformTvShowSearch(string searchString, bool includeSummaries)
        {
            for(int i=0;i<5;i++)
                try
                {
                    return databaseAccess.PerformTvShowSearch(searchString, includeSummaries);
                }
                catch { }
            return new List<Content>();
        }

        /// <summary>
        /// Gets season/episode information from TheTvDb. Use for newly added shows only,
        /// will replace all episode information in show.
        /// </summary>
        /// <param name="show">Show to load episode information into</param>
        public static TvShow FullShowSeasonsUpdate(TvShow show)
        {
            return databaseAccess.FullShowSeasonsUpdate(show);
        }
        #endregion
    }
}
