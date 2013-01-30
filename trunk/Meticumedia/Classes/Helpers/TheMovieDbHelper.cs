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
        
        private static readonly string baseApiUrl = "http://private-b13e-themoviedb.apiary.io/3/";

        private static readonly string searchGet = "search/movie";
        private static readonly string movieGet = "/movie/";
        public static readonly string genresGet = "/genre/list";

        private static readonly string THE_MOVIE_DB_API_KEY = "3416706d4f30b12e50de88bbe646630f";

        #endregion

        #region Classes

        private class GetParam
        {
            public string Name { get; set; }
            public string Value { get; set; }

            public GetParam(string name, string value)
            {
                this.Name = name;
                this.Value = value;
            }
        }

        private class ResultNode
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public bool Set { get; set; }
            public List<ResultNode> ChildNodes { get; private set; }

            public ResultNode(string name, string value)
            {
                this.Name = name;
                this.Value = value;
                BuildChildren();
            }

            private enum BracketType { None, Squiggly, Square }

            private void BuildChildren()
            {
                ChildNodes = new List<ResultNode>();

                string name = string.Empty;
                string value = string.Empty;
                int bracketCnt = 0;
                BracketType bracketType = BracketType.None;
                bool insideQuotations = false;
                bool valueSet = false;
                for (int i = 0; i < this.Value.Length; i++)
                {
                    char c = this.Value[i];

                    if (!insideQuotations && (bracketType == BracketType.None || bracketType == BracketType.Square))
                    {
                        if (c == '[')
                        {
                            bracketCnt++;
                            bracketType = BracketType.Square;
                            if (bracketCnt == 1)
                            {
                                valueSet = true;
                                continue;
                            }
                        }
                        else if (c == ']')
                        {
                            bracketCnt--;
                            if (bracketCnt == 0)
                            {
                                bracketType = BracketType.None;
                                CreateChild(ref name, ref value, ref valueSet);
                                continue;
                            }
                        }
                    }
                    if (!insideQuotations && (bracketType == BracketType.None || bracketType == BracketType.Squiggly))
                    {
                        if (c == '{')
                        {
                            bracketCnt++;
                            bracketType = BracketType.Squiggly;
                            if (bracketCnt == 1)
                            {
                                valueSet = true;
                                continue;
                            }
                        }
                        else if (c == '}')
                        {
                            bracketCnt--;
                            if (bracketCnt == 0)
                            {
                                bracketType = BracketType.None;
                                CreateChild(ref name, ref value, ref valueSet);
                                continue;
                            }
                        }
                    }
                    if (bracketType == BracketType.None)
                    {
                        if (c == '\"')
                        {
                            insideQuotations = !insideQuotations;
                            continue;
                        }
                    }

                    if (!insideQuotations && c == ',' && bracketCnt == 0)
                        CreateChild(ref name, ref value, ref valueSet);
                    else if (!insideQuotations && c == ':' && !valueSet)
                        valueSet = true;
                    else if (!valueSet)
                        name += c;
                    else
                        value += c;
                }

                CreateChild(ref name, ref value, ref valueSet);
            }

            private void CreateChild(ref string name, ref string value, ref bool valueSet)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    ChildNodes.Add(new ResultNode(name, value));
                    name = string.Empty;
                    value = string.Empty;
                    valueSet = false;
                }
            }
        }

        #endregion

        #region Database Access

        private static ResultNode GetRequest(string get, List<GetParam> headers)
        {
            string url = baseApiUrl + get + "?api_key=" + THE_MOVIE_DB_API_KEY;
            if (headers != null)
                foreach (GetParam param in headers)
                    url += "&" + param.Name + "=" + param.Value;

            // prepare the web page we will be asking for
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Accept = "application/json";
            HttpWebResponse response = null;

            // execute the request
            bool requestSucess = false;
            for(int i=0;i<5;i++)
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    requestSucess = true;
                    break;
                }
                catch { }

            if (!requestSucess)
                return new ResultNode(string.Empty, string.Empty);

            StreamReader stream = new StreamReader(response.GetResponseStream());

            string sLine = "";
            string contents = string.Empty;
            while (sLine != null)
            {
                sLine = stream.ReadLine();
                if (sLine != null)
                    contents += sLine;
            }

            return new ResultNode(string.Empty, contents); ;
        }

        public static List<Content> PerformMovieSearch(string search)
        {
            List<GetParam> parameters = new List<GetParam>();
            parameters.Add(new GetParam("query", search));
            ResultNode searchNode = GetRequest(searchGet, parameters);

            List<Content> searchResults = new List<Content>();
            foreach (ResultNode pageNode in searchNode.ChildNodes)
            {
                foreach (ResultNode node in pageNode.ChildNodes)
                {
                    if (node.Name == "results")
                    {
                        foreach (ResultNode resultNode in node.ChildNodes)
                        {
                            Movie movieResult = ParseMovieResult(new Movie(), resultNode);
                            if (movieResult.Id > 0)
                                searchResults.Add(movieResult);
                        }
                    }
                }
            }

            return searchResults;
        }

        private static Movie ParseMovieResult(Movie baseMovie, ResultNode resultNode)
        {
            Movie movieResult = baseMovie;
            foreach (ResultNode resultPropNode in resultNode.ChildNodes)
            {
                switch (resultPropNode.Name)
                {
                    case "id":
                        int id2;
                        int.TryParse(resultPropNode.Value, out id2);
                        movieResult.Id = id2;
                        break;
                    case "title":
                        movieResult.Name = resultPropNode.Value;
                        break;
                    case "original_title":
                        if (string.IsNullOrEmpty(movieResult.Name))
                            movieResult.Name = resultPropNode.Value;
                        break;
                    case "release_date":
                        DateTime date;
                        DateTime.TryParse(resultPropNode.Value, out date);
                        movieResult.Date = date;
                        break;
                    case "genres":
                        movieResult.Genres = new List<string>();
                        foreach (ResultNode genreNode in resultPropNode.ChildNodes)
                        {
                            foreach (ResultNode genrePropNode in genreNode.ChildNodes)
                                if (genrePropNode.Name == "name")
                                    movieResult.Genres.Add(genrePropNode.Value);
                        }
                        break;
                    case "overview":
                        movieResult.Overview = resultPropNode.Value;
                        break;
                }
            }
            return movieResult;
        }

        private static Movie GetMovieInfo(Movie baseMovie)
        {
            ResultNode searchNode = GetRequest(movieGet + baseMovie.Id.ToString(), null);
            return ParseMovieResult(baseMovie, searchNode.ChildNodes[0]);
        }

        #endregion

                /// <summary>
        /// Update info for a movie from online database.
        /// </summary>
        /// <param name="movie">The movie to update</param>
        /// <returns>The updated movie</returns>
        public static Movie UpdateMovieInfo(Movie movie)
        {
            for (int j = 0; j < 5; j++)
            {
                try
                {
                    // Get movie info from TheMovieDb
                   // TmdbMovie fullMovie = tmdbApi.GetMovieInfo(movie.Id);

                    // Return new instance of movie with updated information
                    return GetMovieInfo(movie);

                }
                catch { }
            }

            // Update uncessfull, return new instance of movie with no changes
            return new Movie(movie);
        }

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

        #region Genres

        /// <summary>
        /// List of possible genres a movie can have.
        /// </summary>
        public static List<string> AllGenres { get; private set; }

        /// <summary>
        /// Lock for accesing genres
        /// </summary>
        private static object genreLock = new object();

        /// <summary>
        /// Static event that fires when static background color properties are changed.
        /// </summary>
        public static event EventHandler GenresChanged;

        /// <summary>
        /// Triggers BackColourChanged event
        /// </summary>
        public static void OnGenreChange()
        {
            if (GenresChanged != null)
                GenresChanged(null, new EventArgs());
        }

        /// <summary>
        /// Builds genre list from TheMovieDb list of genres
        /// </summary>
        public static void BuildGenres()
        {
            // Try to get from database, if fails load from saved
            try
            {
                UpdateAllGenres();
                SaveGenres();
            }
            catch
            {
                LoadGenres();
            } 

            OnGenreChange();
        }

        /// <summary>
        /// Update all genres from database
        /// </summary>
        public static void UpdateAllGenres()
        {
            if (AllGenres == null)
            {
                ResultNode genresNodes = GetRequest(genresGet, null);

                AllGenres = new List<string>();

                foreach (ResultNode rootNode in genresNodes.ChildNodes)
                {
                    foreach (ResultNode node in rootNode.ChildNodes)
                    {
                        foreach (ResultNode genreNode in node.ChildNodes)
                            foreach (ResultNode genrePropNode in genreNode.ChildNodes)
                                if (genrePropNode.Name == "name")
                                    AllGenres.Add(genrePropNode.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets genres from the movies in specific movie folder(s).
        /// </summary>
        /// <param name="folder">The movie folder to look in (use a string with "All" movie folders) </param>
        /// <returns>List of genres available from movie in folder(s)</returns>
        public static List<string> GetAvailableGenres(List<Content> movies)
        {
            List<string> availableGenres = new List<string>();
            foreach (string genre in AllGenres)
                foreach (Movie movie in movies)
                {
                    bool added = false;
                    foreach (string movieGenre in movie.Genres)
                        if (movieGenre == genre)
                        {
                            added = true;
                            availableGenres.Add(genre);
                            break;
                        }
                    if (added)
                        break;
                }
            return availableGenres;
        }

        /// <summary>
        /// The genres root XML string.
        /// </summary>
        private static readonly string GENRES_ROOT_XML = "Genres";

        /// <summary>
        /// Element name string for a single genre.
        /// </summary>
        private static readonly string GENRE_XML = "Genre";

        /// <summary>
        /// Loads genres from saved XML.
        /// </summary>
        public static void LoadGenres()
        {
            lock (genreLock)
            {

                // Get path for genres
                string path = Path.Combine(Organization.GetBasePath(true), GENRES_ROOT_XML + ".xml");
                if (!File.Exists(path))
                {
                    AllGenres = new List<string>();
                    return;
                }

                // Load XML
                XmlTextReader reader = new XmlTextReader(path);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);

                // Load genres
                AllGenres = new List<string>();
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    string genre = node.InnerText;
                    AllGenres.Add(genre);
                }

                reader.Close();
            }
        }

        /// <summary>
        /// Saves genres to XML, saved genres list used in case where there is problems retrieving
        /// them from TheMovieDb.
        /// </summary>
        public static void SaveGenres()
        {
            lock (genreLock)
            {

                string path = Path.Combine(Organization.GetBasePath(true), GENRES_ROOT_XML + ".xml");

                using (XmlWriter xw = XmlWriter.Create(path))
                {
                    // Start genres element
                    xw.WriteStartElement(GENRES_ROOT_XML);

                    // Save all genres
                    if (AllGenres != null)
                        foreach (string genre in AllGenres)
                            xw.WriteElementString(GENRE_XML, genre);

                    // End genres
                    xw.WriteEndElement();

                }
            }
        }

        #endregion
    }
}
