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
    public static class TheMovieDbHelper
    {
        #region Constants
        
        /// <summary>
        /// Base API URL for accessing database.
        /// </summary>
        private static readonly string baseApiUrl = "http://private-b13e-themoviedb.apiary.io/3/";

        /// <summary>
        /// HTTP get for searching for a movie.
        /// </summary>
        private static readonly string searchGet = "search/movie";

        /// <summary>
        /// HTTP get for retrieving movie info.
        /// </summary>
        private static readonly string movieGet = "/movie/";

        /// <summary>
        /// HTTP get for retrieving list of movie genres.
        /// </summary>
        public static readonly string genresGet = "/genre/list";

        /// <summary>
        /// Key for accessing API.
        /// </summary>
        private static readonly string THE_MOVIE_DB_API_KEY = "3416706d4f30b12e50de88bbe646630f";

        #endregion

        #region Classes

        /// <summary>
        /// Paramater for HTTP get.
        /// </summary>
        private class GetParam
        {
            /// <summary>
            /// Parameter name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Parameter value
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// Constructor with known properties
            /// </summary>
            /// <param name="name">Parameter name</param>
            /// <param name="value">Parameter value</param>
            public GetParam(string name, string value)
            {
                this.Name = name;
                this.Value = value;
            }
        }

        /// <summary>
        /// Node of results from database response.
        /// </summary>
        private class ResultNode
        {
            #region Properties

            /// <summary>
            /// Name of node
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Value of node (all text after name)
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// Child nodes contained in value.
            /// </summary>
            public List<ResultNode> ChildNodes { get; private set; }

            #endregion

            #region Constructor

            /// <summary>
            /// Constructor with known name and value.
            /// </summary>
            /// <param name="name">Node's name</param>
            /// <param name="value">Node's value</param>
            public ResultNode(string name, string value)
            {
                // Store properties
                this.Name = name;
                this.Value = value;

                // Parse value for child nodes
                BuildChildren();
            }

            #endregion

            #region Child Parsing

            /// <summary>
            /// Types of brackets that seperate out child nodes
            /// </summary>
            private enum BracketType { None, Sqiggly, Square }

            /// <summary>
            /// Parse value of node to build child nodes. This method will be called recursively as each
            /// child is created.
            /// HTTP gets from the database generally have multiple nodes of the format name : value
            /// that are seperated by commas. In some cases (typically root nodes) there is no name, there
            /// is just a list of values separated by commas.
            /// The value portion can be made up of child nodes, which are parsed in this method.
            /// The name or value can be inside quotations, and the value can be inside square or curly brackets.
            /// </summary>
            private void BuildChildren()
            {
                // Init child nodes
                ChildNodes = new List<ResultNode>();
                
                // Init name/value for child
                string name = string.Empty;
                string value = string.Empty;

                // Init bracket/quotation tracking
                int bracketCnt = 0;
                BracketType bracketType = BracketType.None;
                bool insideQuotations = false;
                bool onValue = false;

                // Go through each character in value
                for (int i = 0; i < this.Value.Length; i++)
                {
                    // Grab current character
                    char c = this.Value[i];

                    // Square bracket checking - perform if not inside any brackets/quotations or if already inside square bracket and looking for end
                    if (!insideQuotations && (bracketType == BracketType.None || bracketType == BracketType.Square))
                    {
                        // Opening bracket
                        if (c == '[')
                        {
                            // Set bracket type and start counting
                            bracketType = BracketType.Square;
                            bracketCnt++;

                            // If first bracket then it's the start of child node's value
                            if (bracketCnt == 1)
                            {
                                onValue = true;
                                continue;
                            }
                        }
                        // Closing bracket
                        else if (c == ']')
                        {
                            // Decrement bracket count
                            bracketCnt--;

                            // If final bracket it's the end of child value. 
                            if (bracketCnt == 0)
                            {
                                // Create child (will clear name, value, onValue variables) and clear bracket type.
                                CreateChild(ref name, ref value, ref onValue);
                                bracketType = BracketType.None;
                                continue;
                            }
                        }
                    }
                    // Squiggly bracket checking - perform if not inside any brackets/quotations or already inside sqiggly bracket and looking for end
                    if (!insideQuotations && (bracketType == BracketType.None || bracketType == BracketType.Sqiggly))
                    {
                        // Opening bracket
                        if (c == '{')
                        {
                            // Set bracket type and start counting
                            bracketType = BracketType.Sqiggly;
                            bracketCnt++;

                            // If first bracket then it's the start of child node's value
                            if (bracketCnt == 1)
                            {
                                onValue = true;
                                continue;
                            }
                        }
                        // Closing bracket
                        else if (c == '}')
                        {
                            bracketCnt--;
                            if (bracketCnt == 0)
                            {
                                // Create child (will clear name, value, valuesSet variables) and clear bracket type.
                                CreateChild(ref name, ref value, ref onValue);
                                bracketType = BracketType.None;
                                continue;
                            }
                        }
                    }
                    // If not inside brackets, check for quotations
                    if (bracketType == BracketType.None)
                    {
                        if (c == '\"')
                        {
                            insideQuotations = !insideQuotations;
                            continue;
                        }
                    }

                    // Comma indicates end of child (if outside of quotations/brackets)
                    if (c == ',' && !insideQuotations && bracketCnt == 0)
                        CreateChild(ref name, ref value, ref onValue);
                    // Colong s separation between name and value
                    else if (c == ':' && !insideQuotations && !onValue)
                        onValue = true;
                    // Add character to name, if not on value yet
                    else if (!onValue)
                        name += c;
                    // Add character to value
                    else
                        value += c;
                }

                CreateChild(ref name, ref value, ref onValue);
            }

            /// <summary>
            /// Creates child node from name and value string and clears build variables.
            /// </summary>
            /// <param name="name">Name for child node</param>
            /// <param name="value">Value for child node</param>
            /// <param name="onValue">Whether child build is on value portion</param>
            private void CreateChild(ref string name, ref string value, ref bool onValue)
            {
                // Check that value is valid
                if (!string.IsNullOrEmpty(value))
                {
                    // Add child
                    ChildNodes.Add(new ResultNode(name, value));

                    // Clear child build variables
                    name = string.Empty;
                    value = string.Empty;
                    onValue = false;
                }
            }

            #endregion
        }

        #endregion

        #region Database Access

        private static Queue<DateTime> requestTimes = new Queue<DateTime>();

        /// <summary>
        /// Performs HTTP get from database.
        /// </summary>
        /// <param name="get">Get type string</param>
        /// <param name="parameters">Parameter for get</param>
        /// <returns>Get results as value of ResultNode with no name</returns>
        private static ResultNode GetRequest(string get, List<GetParam> parameters)
        {
            // Check rate of requests - maximum is 30 requests every 10 second
            DateTime oldest;
            lock (requestTimes)
            {
                while (requestTimes.Count >= 20)
                {
                    oldest = requestTimes.Peek();
                    double requestAge = (DateTime.Now - oldest).TotalSeconds;
                    if (requestAge > 10)
                        requestTimes.Dequeue();
                    else
                        Thread.Sleep((11 - (int)requestAge) * 1000);

                }
                requestTimes.Enqueue(DateTime.Now);
            }

            // Build URL
            string url = baseApiUrl + get + "?api_key=" + THE_MOVIE_DB_API_KEY;
            if (parameters != null)
                foreach (GetParam param in parameters)
                    url += "&" + param.Name + "=" + param.Value;

            // Setup request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "application/json";
            HttpWebResponse response = null;

            // Execute the request - perform up to 5 times, has a tendency to fail randomly
            bool requestSucess = false;
            for (int i = 0; i < 5; i++)
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    requestSucess = true;
                    break;
                }
                catch 
                {
                    Thread.Sleep(3000);
                }

            // If no response return empty results
            if (!requestSucess)
                return new ResultNode(string.Empty, string.Empty);

            // Convert response into a string
            StreamReader stream = new StreamReader(response.GetResponseStream());
            string sLine = "";
            string contents = string.Empty;
            while (sLine != null)
            {
                sLine = stream.ReadLine();
                if (sLine != null)
                    contents += sLine;
            }

            // Return response string as value of ResultNode with no name - will be parsed for child when created
            return new ResultNode(string.Empty, contents);
        }

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
            ResultNode searchNode = GetRequest(searchGet, parameters);

            // Go through result nodes, convert them to movie objects, and add to list
            List<Content> searchResults = new List<Content>();
            foreach (ResultNode pageNode in searchNode.ChildNodes)
                foreach (ResultNode node in pageNode.ChildNodes)
                    if (node.Name == "results")
                        foreach (ResultNode resultNode in node.ChildNodes)
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
        private static void ParseMovieResult(Movie baseMovie, ResultNode resultNode)
        {
            // Go through result child nodes and get properties for movie
            foreach (ResultNode resultPropNode in resultNode.ChildNodes)
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
                        foreach (ResultNode genreNode in resultPropNode.ChildNodes)
                        {
                            foreach (ResultNode genrePropNode in genreNode.ChildNodes)
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
            ResultNode searchNode = GetRequest(movieGet + baseMovie.Id.ToString(), null);

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

        #region Searching



        /*

        /// <summary>
        /// Attempt to match a folder name to a movie from the online database.
        /// </summary>
        /// <param name="search">Search string to match againg</param>
        /// <param name="movieFolder">The movie folder where the movie is in</param>
        /// <returns>Match movie item.</returns>
        public static Movie PathMovieMatch(string movieFolder, string folderPath)
        {
            // Get folder name from full path
            string[] dirs = folderPath.Split('\\');
            string endDir = dirs[dirs.Length - 1];

            return MovieMatch(endDir, movieFolder, folderPath);
        }

        /// <summary>
        /// Count of search threads created
        /// </summary>
        private static int searchCount = 0;

        private class MovieMatchStatus
        {
            public int Id { get; set; }

            public Movie[] Matches { get; set; }

            public bool[] Completes { get; set; }

            public MovieMatchStatus(int id, int numSearches)
            {
                this.Id = id;
                this.Matches = new Movie[numSearches];
                this.Completes = new bool[numSearches];
            }
        }

        private static Dictionary<int, MovieMatchStatus> searchStatus = new Dictionary<int, MovieMatchStatus>();

        /// <summary>
        /// Attempt to match a folder name to a movie from the online database.
        /// </summary>
        /// <param name="search">Search string to match againg</param>
        /// <param name="movieFolder">The movie folder where the movie is in</param>
        /// <param name="folderPath">Folder path where the movie should be moved to</param>
        /// <returns>Match movie item.</returns>
        public static Movie MovieMatch(string search, string movieFolder, string folderPath)
        {            
            // Create empty movie
            Movie emptyMovie = new Movie();
            emptyMovie.Path = folderPath;
            emptyMovie.RootFolder = movieFolder;
            emptyMovie.Found = true;

            // Check for empty search condition
            if (string.IsNullOrEmpty(search))
                return emptyMovie;

            // Get yeat and simplify folder name for easier matching
            int dirYear = FileHelper.GetYear(search);

            // Get list of simplified strings
            List<FileHelper.SimplifyStringResults> searches = FileHelper.SimplifyString(search);

            // Create new status
            int currSeachCnt = ++searchCount;
            MovieMatchStatus status = new MovieMatchStatus(currSeachCnt, searches.Count);
            searchStatus.Add(currSeachCnt, status);

            // Add thread to pool for each search that need to be performed
            for (int i = 0; i < searches.Count; i++)
            {
                object[] args = { currSeachCnt, i, searches[i].SimplifiedString, folderPath, movieFolder, dirYear };
                ThreadPool.QueueUserWorkItem(new WaitCallback(SearchThread), args);
            }

            // Wait for all search to complete
            bool allComplete = false;
            while (!allComplete)
            {
                Thread.Sleep(500);

                // Check if all searches are done
                allComplete = true;
                foreach (bool complete in status.Completes)
                    if (!complete)
                        allComplete = false;

                // A valid (non-null) result from any search thread is all we need to wait for
                for (int i = 0; i < status.Matches.Length; i++)
                    if (status.Matches[i] != null)
                        break;
            }
            
            // Return valid (non-null) result if any
            for (int i = 0; i < status.Matches.Length; i++)
                if (status.Matches[i] != null)
                    return status.Matches[i];

            // Clear status
            searchStatus.Remove(currSeachCnt);

            // Return empty movie
            return emptyMovie;
        }

        /// <summary>
        /// Search thread operation. Performs a single search for a movie from string passed in through arguments.
        /// </summary>
        /// <param name="stateInfo"></param>
        static void SearchThread(object stateInfo)
        {
            // Get arguments
            object[] args = (object[])stateInfo;
            int statusIndex = (int)args[0];
            int searchIndex = (int)args[1];
            string search = (string)args[2];
            string folderPath = (string)args[3];
            string movieFolder = (string)args[4];
            int year = (int)args[5];

            // If search number greater than completed threshold than perform search
            Movie match = null;
            if (searchStatus.ContainsKey(statusIndex))
                DoMatch(search, folderPath, movieFolder, year, out match);

            // If search number greater than completed threshold than set results variables
            // (if it is not it is because a different search thread returned a valid result and
            // the completed count was incremented to beyond the count this thread was created on)
            if (searchStatus.ContainsKey(statusIndex))
            {
                searchStatus[statusIndex].Matches[searchIndex] = match;
                searchStatus[statusIndex].Completes[searchIndex] = true;
            }       
        }

        /// <summary>
        /// Search database for movie and find a match from results.
        /// </summary>
        /// <param name="search">Search string to match movie to</param>
        /// <param name="folderPath">Path of folder containing movie</param>
        /// <param name="movieFolder">Content folder containing movie's folder</param>
        /// <param name="year">Year to match to movie</param>
        /// <param name="movie">resulting match found from search</param>
        /// <returns>whether match was successful</returns>
        private static bool DoMatch(string search, string folderPath, string movieFolder, int year, out Movie movie)
        {
            movie = null;
            
            // Search for movie
            List<Content> searchResults = PerformMovieSearch(search, folderPath, movieFolder);

            // Go through results
            foreach (Content searchResult in searchResults)
            {
                // Verify year in result matches year from folder (if any)
                if (year != -1 && Math.Abs(year - searchResult.Date.Year) > 3)
                    continue;

                // Verify name of movie on foler matches results name
                string simplifiedSearch = FileHelper.SimplifyFileName(search);
                string dbMovieName = FileHelper.SimplifyFileName(searchResult.Name);
                if (!FileHelper.CompareStrings(simplifiedSearch, dbMovieName))
                {
                    string dirNoSpc = simplifiedSearch.Replace(" ", "");
                    string nameNoSpc = dbMovieName.Replace(" ", "");
                    if (!FileHelper.CompareStrings(dirNoSpc, nameNoSpc))
                        continue;
                }

                // Get full information for matching result
                movie = UpdateMovieInfo(searchResult as Movie);
                movie.RootFolder = movieFolder;
                if (string.IsNullOrEmpty(folderPath))
                    movie.Path = movie.BuildFolderPath();
                else
                    movie.Path = folderPath;

                // Movie found, return it
                return true;
            }

            return false;
        }

        */

        #endregion
    }
}
