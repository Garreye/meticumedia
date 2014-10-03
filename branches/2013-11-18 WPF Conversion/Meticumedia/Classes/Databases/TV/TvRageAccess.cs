// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Class for accesing TVRage online TV database and related functions.
    /// </summary>
    public class TvRageAccess : TvDatabaseAccess
    {
        #region Constants

        protected override string API_KEY { get { return "zqWjteEikX3q0IJLx0fc"; } }

        protected override DatabaseAccess.MirrorType SearchMirrorType { get { return MirrorType.Xml; } }

        protected override DatabaseAccess.MirrorType UpdateMirrorType { get { return MirrorType.Xml; } }

        #endregion

        #region Constuctor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TvRageAccess()
        {
        }

        #endregion

        #region Mirrors

        /// <summary>
        /// Updates mirrors.
        /// </summary>
        public override void UpdatesMirrors()
        {
            xmlMirrors = new List<string>();
            xmlMirrors.Add(@"http://services.tvrage.com/myfeeds");
            zipMirrors = new List<string>();
            mirrorsValid = true;
        }
        
        #endregion

        #region Searching/Updating

        /// <summary>
        /// Performs search for show in database. Should be overriden
        /// </summary>
        /// <param name="mirror">Mirror to use</param>
        /// <param name="searchString">Search string for show</param>
        /// <param name="includeSummaries">Whether to include summaries in search results (takes longer - set to false unless user is seeing them)</param>
        /// <returns>Results as list of shows</returns>
        protected override List<Content> DoSearch(string mirror, string searchString, bool includeSummaries)
        {
            // Init results
            List<Content> searchResults = new List<Content>();
            
            // Get search results from databasse
            string seriesLookupUrl = mirror + "/search.php?key=" + this.API_KEY + "&show=" + searchString;
            WebClient webClient = new WebClient();
            string seriesList = webClient.DownloadString(seriesLookupUrl);
            
            // Create xml object with text from mirrors url
            XmlDocument seriesDoc = new XmlDocument();
            seriesDoc.InnerXml = seriesList;

            // Get root element and children
            XmlElement root = seriesDoc.DocumentElement;
            XmlNodeList nodes = root.ChildNodes;

            // Go through each node and get parse into shows
            for (int i = 0; i < nodes.Count; i++)
            {
                // Create show
                TvShow searchResult = ParseShowInfo(nodes[i]);

                // Get summaries for each resulting show - this may be slow!
                if (includeSummaries)
                {
                    // Get show info from database
                    string showInfoUrl = mirror + "/showinfo.php?key=" + this.API_KEY + "&sid=" + searchResult.Id;
                    string showInfo = webClient.DownloadString(showInfoUrl);

                    // Create XML object
                    XmlDocument showInfoDoc = new XmlDocument();
                    showInfoDoc.InnerXml = showInfo;

                    // Parse show info from XML
                    searchResult = ParseShowInfo(showInfoDoc.DocumentElement);
                }

                // Add parsed show to results
                searchResult.DatabaseSelection = (int)TvDataBaseSelection.TvRage;
                searchResults.Add(searchResult);
            }

            // Return results
            return searchResults;
        }

        /// <summary>
        /// Parse show properties from show XML node from database
        /// </summary>
        /// <param name="node">XML node to parse info from</param>
        /// <returns>Parse TvShow object</returns>
        private static TvShow ParseShowInfo(XmlNode node)
        {
            // Init show
            TvShow show = new TvShow();

            // Parse properties out from child nodes
            XmlNodeList seriesNodes = node.ChildNodes;
            foreach (XmlNode subNode in seriesNodes)
                switch (subNode.Name.ToLower())
                {
                    case "showname":
                    case "name":
                        show.DatabaseName = subNode.InnerText;
                        break;
                    case "showid":
                        int id;
                        int.TryParse(subNode.InnerText, out id);
                        show.Id = id;
                        break;
                    case "started":
                        int year;
                        int.TryParse(subNode.InnerText, out year);
                        if (year == 0) year = 1;
                        show.DatabaseYear = year;
                        break;
                    case "genres":
                        show.DatabaseGenres = new GenreCollection(GenreCollection.CollectionType.Tv);
                        foreach (XmlNode genreNode in subNode.ChildNodes)
                            show.DatabaseGenres.Add(genreNode.InnerText);
                        break;
                    case "summary":
                        show.Overview = subNode.InnerText.Replace('\n', ' ');
                        break;
                }
            return show;
        }

        /// <summary>
        /// Gets season/episode information from database
        /// </summary>
        /// <param name="show">Show to load episode information into</param>
        protected override bool DoUpdate(string mirror, Content content)
        {
            TvShow show = (TvShow)content;
            try
            {
                // Get show info from database
                string showInfoUrl = mirror + "/showinfo.php?key=" + this.API_KEY + "&sid=" + show.Id;
                WebClient webClient = new WebClient();
                string showInfo = webClient.DownloadString(showInfoUrl);

                // Create XML object from results
                XmlDocument seriesDoc = new XmlDocument();
                seriesDoc.InnerXml = showInfo;

                // Parse show info
                show.Overview = ParseShowInfo(seriesDoc.DocumentElement).Overview;

                // Get episode info from database
                string episodeListUrl = mirror + "/episode_list.php?key=" + this.API_KEY + "&sid=" + show.Id;
                webClient = new WebClient();
                string seriesList = webClient.DownloadString(episodeListUrl);

                // Create xml object with text from mirrors url
                seriesDoc = new XmlDocument();
                seriesDoc.InnerXml = seriesList;

                // Get root element and children
                XmlElement root = seriesDoc.DocumentElement;
                XmlNodeList nodes = root.ChildNodes;

                // Go through each node and parse out episodes
                foreach (XmlNode subNode in nodes)
                {
                    switch (subNode.Name.ToLower())
                    {
                        case "episodelist":
                            foreach (XmlNode seasonNode in subNode.ChildNodes)
                            {
                                if (seasonNode.Name.ToLower() == "season")
                                {
                                    string seasonNoStr = seasonNode.Attributes["no"].Value;
                                    int seasonNo;
                                    int.TryParse(seasonNoStr, out seasonNo);

                                    foreach (XmlNode epNode in seasonNode.ChildNodes)
                                    {
                                        TvEpisode ep = new TvEpisode(show);
                                        ep.Season = seasonNo;
                                        foreach (XmlNode epPropNode in epNode.ChildNodes)
                                            switch (epPropNode.Name.ToLower())
                                            {
                                                case "seasonnum": // episode number within season
                                                    int epNum;
                                                    int.TryParse(epPropNode.InnerText, out epNum);
                                                    ep.DatabaseNumber = epNum;
                                                    break;
                                                case "airdate":
                                                    DateTime airDate;
                                                    DateTime.TryParse(epPropNode.InnerText, out airDate);
                                                    ep.AirDate = airDate;
                                                    break;
                                                case "title":
                                                    ep.DatabaseName = epPropNode.InnerText;
                                                    break;
                                                case "summary":
                                                    ep.Overview = epPropNode.InnerText.Replace('\n', ' ');
                                                    break;
                                            }
                                        ep.InDatabase = true;

                                        // If episode already exists just update it, else add it
                                        TvEpisode existingMatch;
                                        if (show.FindEpisode(ep.Season, ep.DatabaseNumber, true, out existingMatch))
                                        {
                                            if (!existingMatch.PreventDatabaseUpdates)
                                            {
                                                existingMatch.DatabaseName = ep.DatabaseName;
                                                existingMatch.AirDate = ep.AirDate;
                                                existingMatch.Overview = ep.Overview;
                                                existingMatch.InDatabase = true;
                                            }
                                        }
                                        else
                                            show.Episodes.Add(ep);
                                    }
                                }


                            }
                            break;
                    }
                }
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
