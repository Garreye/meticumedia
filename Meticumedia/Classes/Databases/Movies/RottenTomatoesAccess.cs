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

namespace Meticumedia.Classes
{
    /// <summary>
    /// Helper class for accessing TheMovieDb online movie database.
    /// </summary>
    public class RottenTomatoesAccess : DatabaseAccess
    {
        #region Constants
        
        /// <summary>
        /// API Key for accessing database
        /// </summary>
        protected override string API_KEY { get { return "44tyg8qh8uh5ghabuwbdwzdm"; } }

        protected override string API_KEY_PARAM_NAME { get { return "apikey"; } }

        /// <summary>
        /// URL for accessing database
        /// </summary>
        private string BASE_API_URL = "http://api.rottentomatoes.com/api/public/v1.0/";

        protected override DatabaseAccess.MirrorType SearchMirrorType { get { return MirrorType.Json; } }

        protected override DatabaseAccess.MirrorType UpdateMirrorType { get { return MirrorType.Json; } }

        /// <summary>
        /// JSON rate limiter override
        /// </summary>
        protected override RateLimiter JSON_RATE_LIMITER { get { return rateLimit; } }

        /// <summary>
        /// JSON rate limiter instance
        /// </summary>
        private RateLimiter rateLimit = new RateLimiter(true, 10, 900);

        #endregion

        #region Mirrors

        /// <summary>
        /// Updates local list of database mirrors.
        /// </summary>
        public override void UpdatesMirrors()
        {
            jsonMirrors = new List<string>();
            jsonMirrors.Add(BASE_API_URL);
            mirrorsValid = true;
        }

        #endregion

        #region Database Access

        /// <summary>
        /// Searches for movie from database.
        /// </summary>
        /// <param name="search">Search string for movie</param>
        /// <returns>Search results as list of movies</returns>
        protected override List<Content> DoSearch(string mirror, string searchString, bool includeSummaries)
        {
            // Perform request from database
            List<HttpGetParameter> parameters = new List<HttpGetParameter>();
            parameters.Add(new HttpGetParameter("q", searchString));
            JsonNode searchNode = GetJsonRequest(mirror, parameters, "movies.json");

            // Go through result nodes, convert them to movie objects, and add to list
            List<Content> searchResults = new List<Content>();
            foreach (JsonNode pageNode in searchNode.ChildNodes[0].ChildNodes)
                if (pageNode.Name == "movies")
                {
                    foreach (JsonNode movieNode in pageNode.ChildNodes)
                    {
                        Movie movieResult = new Movie();
                        ParseMovieResult(movieResult, movieNode);
                        if (movieResult.Id > 0)
                            searchResults.Add(movieResult);
                    }
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
        private void ParseMovieResult(Movie baseMovie, JsonNode resultNode)
        {
            baseMovie.DatabaseSelection = (int)MovieDatabaseSelection.RottenTomotoes;
            
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
                        baseMovie.DatabaseName = resultPropNode.Value;
                        break;
                    case "original_title":
                        if (string.IsNullOrEmpty(baseMovie.DatabaseName))
                            baseMovie.DatabaseName = resultPropNode.Value;
                        break;
                    case "year":
                        if (baseMovie.DatabaseYear < 1900)
                        {
                            int year;
                            if (int.TryParse(resultPropNode.Value, out year))
                                baseMovie.DatabaseYear = year;
                        }
                        break;
                    //case "release_dates":
                    //    foreach(JsonNode node in resultPropNode.ChildNodes)
                    //        if (node.Name == "theater")
                    //        {
                    //            DateTime date;
                    //            DateTime.TryParse(node.Value, out date);
                    //            baseMovie.Date = date;
                    //        }
                        
                    //    break;
                    case "genres":
                        baseMovie.DatabaseGenres = new GenreCollection(GenreCollection.CollectionType.Movie);
                        string[] genres = resultPropNode.Value.Split(',');
                        foreach (string genre in genres)
                        {
                            string noQuotesGenre = genre.Replace("\"", "");
                            string[] multiGenre = noQuotesGenre.Split('&');
                            foreach (string g in multiGenre)
                                baseMovie.DatabaseGenres.Add(g.Trim());
                        }
                        break;
                    case "synopsis":
                        baseMovie.Overview = resultPropNode.Value;
                        break;
                }
        }

        /// <summary>
        /// Updates movie instance properties from database
        /// </summary>
        /// <param name="baseMovie">Movie to update</param>
        /// <returns></returns>
        protected override bool DoUpdate(string mirror, Content content)
        {
            try
            {
                // Get movie info from database
                JsonNode searchNode = GetJsonRequest(mirror, null, "movies/" + content.Id.ToString() + ".json");

                // Parse info into movie instance
                ParseMovieResult((Movie)content, searchNode.ChildNodes[0]);
                content.LastUpdated = DateTime.Now;
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
