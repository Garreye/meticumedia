// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml;
using System.Threading;
using System.Net;

namespace Meticumedia
{
    /// <summary>
    /// Helper class for accessing TheMovieDb online movie database.
    /// </summary>
    public class MovieDatabaseAccess : DatabaseAccess
    {

        #region Database Access


        /// <summary>
        /// Searches for movie from database.
        /// </summary>
        /// <param name="search">Search string for movie</param>
        /// <returns>Search results as list of movies</returns>
        public static List<Content> PerformMovieSearch(string search)
        {
            // Perform request from database
            List<GetParam> parameters = new List<GetParam>();
            parameters.Add(new GetParam("query", search));
            JsonNode searchNode = GetRequest(searchGet, parameters);

            // Go through result nodes, convert them to movie objects, and add to list
            List<Content> searchResults = new List<Content>();
            foreach (JsonNode pageNode in searchNode.ChildNodes)
                foreach (JsonNode node in pageNode.ChildNodes)
                    if (node.Name == "results")
                        foreach (JsonNode resultNode in node.ChildNodes)
                        {
                            Movie movieResult = new Movie();
                            ParseMovieResult(movieResult, resultNode);
                            if (movieResult.Id > 0)
                                searchResults.Add(movieResult);
                        }

            // Return results list
            return searchResults;
        }

        /// <summary>
        /// Converts HTTP get result node into movie.
        /// </summary>
        /// <param name="baseMovie">Movie instance to start with (to keep path/org info from)</param>
        /// <param name="resultNode">Result node from get request containing movie info</param>
        /// <returns>Movie with properties from results node</returns>
        private static void ParseMovieResult(Movie baseMovie, JsonNode resultNode)
        {
            // Go through result child nodes and get properties for movie
            foreach (JsonNode resultPropNode in resultNode.ChildNodes)
                switch (resultPropNode.Name)
                {
                    case "id":
                        int id2;
                        int.TryParse(resultPropNode.Value, out id2);
                        baseMovie.Id = id2;
                        break;
                    case "title":
                        baseMovie.Name = resultPropNode.Value;
                        break;
                    case "original_title":
                        if (string.IsNullOrEmpty(baseMovie.Name))
                            baseMovie.Name = resultPropNode.Value;
                        break;
                    case "release_date":
                        DateTime date;
                        DateTime.TryParse(resultPropNode.Value, out date);
                        baseMovie.Date = date;
                        break;
                    case "genres":
                        baseMovie.Genres = new GenreCollection(GenreCollection.CollectionType.Movie);
                        foreach (JsonNode genreNode in resultPropNode.ChildNodes)
                        {
                            foreach (JsonNode genrePropNode in genreNode.ChildNodes)
                                if (genrePropNode.Name == "name")
                                    baseMovie.Genres.Add(genrePropNode.Value);
                        }
                        break;
                    case "overview":
                        baseMovie.Overview = resultPropNode.Value;
                        break;
                }
        }

        /// <summary>
        /// Updates movie instance properties from database
        /// </summary>
        /// <param name="baseMovie">Movie to update</param>
        /// <returns></returns>
        private static void GetMovieInfo(Movie baseMovie)
        {
            // Get movie info from database
            JsonNode searchNode = GetRequest(movieGet + baseMovie.Id.ToString(), null);

            // Parse info into movie instance
            ParseMovieResult(baseMovie, searchNode.ChildNodes[0]);
            baseMovie.LastUpdated = DateTime.Now;
        }

        /// <summary>
        /// Update info for a movie from online database.
        /// </summary>
        /// <param name="movie">The movie to update</param>
        /// <returns>The updated movie</returns>
        public static void UpdateMovieInfo(Movie movie)
        {
            // Tries up to 5 times - database can fail randomly
            for (int j = 0; j < 5; j++)
            {
                try
                {
                    // Get movie info from TheMovieDb
                    // TmdbMovie fullMovie = tmdbApi.GetMovieInfo(movie.Id);

                    // Return new instance of movie with updated information
                    GetMovieInfo(movie);
                    return;

                }
                catch { }
            }
        }

        #endregion

    }
}
