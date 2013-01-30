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

namespace Meticumedia
{
    public class TvRageAccess : TvDatabaseAccess
    {
        protected override string API_KEY { get { return "zqWjteEikX3q0IJLx0fc"; } }

        #region Constuctor

        public TvRageAccess()
        {
            
        }

        #endregion

        #region Mirrors

        /// <summary>
        /// Updates local list of TheTvDb mirrors.
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

        protected override List<Content> DoSearch(string mirror, string searchString, bool includeSummaries)
        {
            List<Content> searchResults = new List<Content>();
            
            string seriesLookupUrl = mirror + "/search.php?key=" + this.API_KEY + "&show=" + searchString;
            WebClient webClient = new WebClient();
            string seriesList = webClient.DownloadString(seriesLookupUrl);
            
            // Create xml object with text from mirrors url
            XmlDocument seriesDoc = new XmlDocument();
            seriesDoc.InnerXml = seriesList;

            // Get root element and children
            XmlElement root = seriesDoc.DocumentElement;
            XmlNodeList nodes = root.ChildNodes;

            // Go through each node and get the url for all mirrors
            for (int i = 0; i < nodes.Count; i++)
            {
                // Create show
                TvShow searchResult = ParseShowInfo(nodes[i]);

                if (includeSummaries)
                {
                    string showInfoUrl = mirror + "/showinfo.php?key=" + this.API_KEY + "&sid=" + searchResult.Id;
                    string showInfo = webClient.DownloadString(showInfoUrl);

                    XmlDocument showInfoDoc = new XmlDocument();
                    showInfoDoc.InnerXml = showInfo;

                    // Get show overview from infor
                    searchResult = ParseShowInfo(showInfoDoc.DocumentElement);
                }

                searchResults.Add(searchResult);
            }


            return searchResults;
        }

        private static TvShow ParseShowInfo(XmlNode node)
        {
            TvShow show = new TvShow();

            XmlNodeList seriesNodes = node.ChildNodes;
            foreach (XmlNode subNode in seriesNodes)
                switch (subNode.Name.ToLower())
                {
                    case "showname":
                    case "name":
                        show.Name = subNode.InnerText;
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
                        DateTime airDate = new DateTime(year, 1, 1);
                        show.Date = airDate;
                        break;
                    case "genres":
                        show.Genres = new List<string>();
                        foreach (XmlNode genreNode in subNode.ChildNodes)
                            show.Genres.Add(genreNode.InnerText);
                        break;
                    case "summary":
                        show.Overview = subNode.InnerText.Replace('\n', ' ');
                        break;
                }
            return show;
        }

        /// <summary>
        /// Gets season/episode information from TheTvDb
        /// </summary>
        /// <param name="show">Show to load episode information into</param>
        public override TvShow DoUpdate(TvShow show)
        {
            // Get mirror
            string mirror;
            if (!GetMirror(MirrorType.Xml, out mirror))
                return show;


            string showInfoUrl = mirror + "/showinfo.php?key=" + this.API_KEY + "&sid=" + show.Id;
            WebClient webClient = new WebClient();
            string showInfo = webClient.DownloadString(showInfoUrl);

            XmlDocument seriesDoc = new XmlDocument();
            seriesDoc.InnerXml = showInfo;

            // Get show overview from infor
            show.Overview = ParseShowInfo(seriesDoc.DocumentElement).Overview;

            // Init episode list
            string episodeListUrl = mirror + "/episode_list.php?key=" + this.API_KEY + "&sid=" + show.Id;
            webClient = new WebClient();
            string seriesList = webClient.DownloadString(episodeListUrl);

            // Create xml object with text from mirrors url
            seriesDoc = new XmlDocument();
            seriesDoc.InnerXml = seriesList;

            // Get root element and children
            XmlElement root = seriesDoc.DocumentElement;
            XmlNodeList nodes = root.ChildNodes;

            // Go through each node and get the url for all mirrors

            foreach (XmlNode subNode in nodes)
            {
                switch (subNode.Name.ToLower())
                {
                    case "episodelist":
                        //show.Seasons = new TvSeasonCollection();
                        foreach (XmlNode seasonNode in subNode.ChildNodes)
                        {
                            if (seasonNode.Name.ToLower() == "season")
                            {
                                string seasonNoStr = seasonNode.Attributes["no"].Value;
                                int seasonNo;
                                int.TryParse(seasonNoStr, out seasonNo);
                                if (!show.Seasons.Contains(seasonNo))
                                    show.Seasons.Add(new TvSeason(seasonNo));
                                TvSeason season = show.Seasons[seasonNo];

                                foreach (XmlNode epNode in seasonNode.ChildNodes)
                                {
                                    TvEpisode ep = new TvEpisode(show.Name);
                                    ep.Season = seasonNo;
                                    foreach (XmlNode epPropNode in epNode.ChildNodes)
                                        switch (epPropNode.Name.ToLower())
                                        {
                                            case "seasonnum": // episode number within season
                                                int epNum;
                                                int.TryParse(epPropNode.InnerText, out epNum);
                                                ep.Number = epNum;
                                                break;
                                            case "airdate":
                                                DateTime airDate;
                                                DateTime.TryParse(epPropNode.InnerText, out airDate);
                                                ep.AirDate = airDate;
                                                break;
                                            case "title":
                                                ep.DataBaseName = epPropNode.InnerText;
                                                break;
                                            case "summary":
                                                ep.Overview = epPropNode.InnerText.Replace('\n', ' ');
                                                break;
                                        }
                                    ep.InDatabase = true;

                                    // If episode already exists just update it, else add it
                                    TvEpisode existingMatch;
                                    if (show.FindEpisode(ep.Season, ep.Number, out existingMatch))
                                    {
                                        existingMatch.DataBaseName = ep.DataBaseName;
                                        existingMatch.AirDate = ep.AirDate;
                                        existingMatch.Overview = ep.Overview;
                                        existingMatch.InDatabase = true;
                                    }
                                    else
                                        season.Episodes.Add(ep);
                                }
                            }


                        }
                        break;
                }
            }
            return show;
        }


        #endregion
    }
}
