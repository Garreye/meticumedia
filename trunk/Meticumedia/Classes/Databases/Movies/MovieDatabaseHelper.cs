using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meticumedia
{
    class MovieDatabaseHelper
    {
        #region Databases

        /// <summary>
        /// TheMovieDb database
        /// </summary>
        private static TheMovieDbAccess TheMovieDbAccess = new TheMovieDbAccess();

        /// <summary>
        /// Rotten Tomatoes database
        /// </summary>
        private static RottenTomatoesAccess RottenTomatoesAccess = new RottenTomatoesAccess();

        /// <summary>
        /// Get private database to use from selection
        /// </summary>
        /// <param name="selection">Database selection</param>
        /// <returns></returns>
        private static DatabaseAccess GetDataBaseAccess(MovieDatabaseSelection selection)
        {
            switch (selection)
            {
                case MovieDatabaseSelection.TheMovieDb:
                    return TheMovieDbAccess;
                case MovieDatabaseSelection.RottenTomotoes:
                    return RottenTomatoesAccess;
                default:
                    throw new Exception("Unknown database selection");
            }
        }

        #endregion

        #region Searching/Updating

        /// <summary>
        /// Gets database server's time
        /// </summary>
        /// <param name="time">retrieved server time</param>
        /// <returns>whether time was successfully retrieved</returns>
        public static bool GetServerTime(MovieDatabaseSelection selection, out string time)
        {
            return GetDataBaseAccess(selection).GetServerTime(out time);
        }

        /// <summary>
        /// Return lists of movie Ids that need updating. 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool GetDataToBeUpdated(MovieDatabaseSelection selection, out List<int> info, out string time)
        {
            return GetDataBaseAccess(selection).GetDataToBeUpdated(out info, out time);
        }

        /// <summary>
        /// Performs search for a movie in database.
        /// </summary>
        /// <param name="searchString">The string to search for</param>
        /// <returns>Array of results from the search</returns>
        public static List<Content> PerformMovieSearch(MovieDatabaseSelection selection, string searchString, bool includeSummaries)
        {
            return GetDataBaseAccess(selection).PerformSearch(searchString, includeSummaries);
        }

        /// <summary>
        /// Updates movie infor from database
        /// </summary>
        /// <param name="movie"></param>
        public static void UpdateMovieInfo(Movie movie)
        {
            GetDataBaseAccess(movie.DataBase).Update(movie);
        }

        #endregion
    }
}
