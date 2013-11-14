// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Xml;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Ionic.Zip;

namespace Meticumedia
{
    /// <summary>
    /// Class for accesing TheTvDb online TV database and related functions.
    /// TODO: currently not in use, TVRage always used...
    /// </summary>
    public class TheTvDbAccess : TvDatabaseAccess
    {
        #region Constants/Enums

        /// <summary>
        /// API Key for accessing TheTvDb
        /// </summary>
        protected override string API_KEY { get { return "F29971EFAF45B40A"; } }

        protected override DatabaseAccess.MirrorType SearchMirrorType { get { return MirrorType.Xml; } }

        protected override DatabaseAccess.MirrorType UpdateMirrorType { get { return MirrorType.Zip; } }

        #endregion

        #region Mirrors

        /// <summary>
        /// Updates local list of TheTvDb mirrors.
        /// </summary>
        public override void UpdatesMirrors()
        {
            // Set mirrors to invalid
            mirrorsValid = false;

            try
            {
                // Get string for mirrors xml
                WebClient webClient = new WebClient();
                string mirrorsXmlStr = webClient.DownloadString("http://www.thetvdb.com/api/" + this.API_KEY + "/mirrors.xml");

                // Create xml object with text from mirrors url
                XmlDocument mirrosDoc = new XmlDocument();
                mirrosDoc.InnerXml = mirrorsXmlStr;

                // Get root element and children
                XmlElement root = mirrosDoc.DocumentElement;
                XmlNodeList rootNodes = root.ChildNodes;

                // Go through each node and get the url for all mirrors
                xmlMirrors = new List<string>();
                zipMirrors = new List<string>();
                foreach (XmlNode node in rootNodes)
                {
                    MirrorType type = MirrorType.Xml;
                    string value = string.Empty;
                    XmlNodeList subNodes = node.ChildNodes;
                    foreach (XmlNode subNode in subNodes)
                        if (subNode.Name.Equals("mirrorpath"))
                            value = subNode.InnerText;
                        else if (subNode.Name.Equals("typemask"))
                        {
                            int typeNumber;
                            int.TryParse(subNode.InnerText, out typeNumber);
                            type = (MirrorType)typeNumber;
                        }

                    if((type & MirrorType.Xml) > 0)
                        xmlMirrors.Add(value);
                    if ((type & MirrorType.Zip) > 0)
                        zipMirrors.Add(value);
                }

                // No exceptions caught so mirrors must be good
                mirrorsValid = true;
            }
            catch
            {
            }
            
        }

        #endregion

        #region Searching/Updating

        /// <summary>
        /// Gets database server's time
        /// </summary>
        /// <param name="time">retrieved server time</param>
        /// <returns>whether time was successfully retrieved</returns>
        public override bool GetServerTime(out string time)
        {
            // Init time
            time = string.Empty;

            // Get the time
            string timeUrl = "http://www.thetvdb.com/api/Updates.php?type=none";
            try
            {
                WebClient webClient = new WebClient();
                string timeXml = webClient.DownloadString(timeUrl);

                XmlDocument timeDoc = new XmlDocument();
                timeDoc.LoadXml(timeXml);

                // Get root element and children
                XmlNodeList rootNodes = timeDoc.DocumentElement.ChildNodes;

                foreach (XmlNode node in rootNodes)
                {
                    if (node.Name.ToLower() == "time")
                        time = node.InnerText;
                }
            }
            catch { }

            return !string.IsNullOrEmpty(time);
        }

        /// <summary>
        /// Return lists of series Ids that need updating. 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public override bool GetDataToBeUpdated(out List<int> info, out string time)
        {
            // Init info
            info = new List<int>();
            time = string.Empty;

            // Get mirror
            string mirror;
            if (!GetMirror(MirrorType.Xml, out mirror))
                return false;

            try
            {

                if (Organization.Shows.LastUpdate == null)
                {
                    GetServerTime(out time);
                    return true;
                }

                string updateUrl = "http://www.thetvdb.com/api/Updates.php?type=all&time=" + Organization.Shows.LastUpdate;

                WebClient webClient = new WebClient();
                string updateXml = webClient.DownloadString(updateUrl);

                XmlDocument updateDoc = new XmlDocument();
                updateDoc.LoadXml(updateXml);

                // Get root element and children
                XmlNodeList rootNodes = updateDoc.DocumentElement.ChildNodes;

                info = new List<int>();
                foreach (XmlNode node in rootNodes)
                {
                    int series;
                    if (node.Name.ToLower() == "time")
                    {
                        time = node.InnerText;
                    }
                    if (node.Name.ToLower() == "episode")
                    {
                        string epUrl = mirror + "/api/" + API_KEY + "/episodes/" + node.InnerText + "/en.xml";
                        string epXml = webClient.DownloadString(epUrl);

                        XmlDocument epDoc = new XmlDocument();
                        epDoc.LoadXml(epXml);

                        // Get root element and children
                        XmlNodeList epRootNodes = epDoc.DocumentElement.ChildNodes[0].ChildNodes;
                        foreach (XmlNode epNode in epRootNodes)
                        {
                            if (epNode.Name.ToLower() == "seriesid")
                            {
                                int.TryParse(epNode.InnerText, out series);
                                if (!info.Contains(series))
                                    info.Add(series);
                            }
                        }

                    }
                    else if (node.Name.ToLower() == "series")
                    {
                        int.TryParse(node.InnerText, out series);
                        if (!info.Contains(series))
                            info.Add(series);
                    }
                }

                return true;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// Performs search for a show in TheTvDb.
        /// </summary>
        /// <param name="searchString">The string to search for</param>
        /// <returns>Array of results from the search</returns>
        protected override List<Content> DoSearch(string mirror, string searchString, bool includeSummaries)
        {
            // Get list of results from search
            List<Content> searchResults = new List<Content>();
            string seriesLookupUrl = mirror + "/api/GetSeries.php?seriesname=" + searchString;
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
                TvShow searchResult = new TvShow();

                XmlNodeList seriesNodes = nodes[i].ChildNodes;
                foreach (XmlNode subNode in seriesNodes)
                    switch (subNode.Name.ToLower())
                    {
                        case "seriesname":
                            searchResult.Name = subNode.InnerText;
                            char[] invalidChars = { '\\', '/', ':', '*', '?', '<', '>', '|' };
                            foreach (char c in invalidChars)
                                if (searchResult.Name.Contains(c))
                                    searchResult.Name = searchResult.Name.Replace(c.ToString(), " ");
                            break;
                        case "seriesid":
                            int id;
                            int.TryParse(subNode.InnerText, out id);
                            searchResult.Id = id;
                            break;
                        case "overview":
                            searchResult.Overview = subNode.InnerText;
                            break;
                        case "firstaired":
                            DateTime airDate;
                            DateTime.TryParse(subNode.InnerText, out airDate);
                            searchResult.Date = airDate;
                            break;
                    }

                searchResult.DatabaseSelection = (int)TvDataBaseSelection.TheTvDb;
                searchResults.Add(searchResult);
            }

            return searchResults;
        }

        /// <summary>
        /// Updates show's season/episode information from database. Use for newly 
        /// added shows only, as it will replace all episode information in show.
        /// </summary>
        /// <param name="show">Show to load episode information into</param>
        protected override bool DoUpdate(string mirror, Content content)
        {
            TvShow show = (TvShow)content;

            bool success = true;
            string tempPath = string.Empty;
            try
            {

                // Download episodes XML string
                string showUrl = mirror + "/api/" + API_KEY + "/series/" + show.Id + "/all/en.zip";
                string basePath = Organization.GetBasePath(true);
                Guid guid = Guid.NewGuid();
                tempPath = Path.Combine(basePath, guid.ToString());
                Directory.CreateDirectory(tempPath);
                string zipPath = Path.Combine(tempPath, "data" + ".zip");
                string extractPath = Path.Combine(tempPath, "en.xml");

                WebClient webClient = new WebClient();
                webClient.DownloadFile(showUrl, zipPath);

                ZipFile zip = ZipFile.Read(zipPath);
                foreach (ZipEntry entry in zip.Entries)
                    if (entry.FileName == "en.xml")
                    {
                        entry.Extract(tempPath, ExtractExistingFileAction.OverwriteSilently);
                        break;
                    }
                zip.Dispose();

                XmlDocument showDoc = new XmlDocument();
                showDoc.Load(extractPath);

                // Get root element and children
                XmlElement root = showDoc.DocumentElement;
                XmlNodeList rootNodes = root.ChildNodes;

                // Go through each node and get info for each episode
                foreach (XmlNode node in rootNodes)
                {
                    if (node.Name == "Series")
                    {
                        XmlNodeList serNodes = node.ChildNodes;
                        foreach (XmlNode subNode in serNodes)
                        {
                            switch (subNode.Name)
                            {
                                case "Genre":
                                    string[] genres = subNode.InnerText.Split('|');
                                    foreach(string genre in genres)
                                        if(!string.IsNullOrWhiteSpace(genre) && !show.Genres.Contains(genre))
                                            show.Genres.Add(genre);
                                    break;
                            }
                        }
                    }
                    else if (node.Name == "Episode")
                    {
                        TvEpisode ep = new TvEpisode(show.Name);
                        int season = -1;

                        XmlNodeList subNodes = node.ChildNodes;
                        foreach (XmlNode subNode in subNodes)
                        {
                            switch (subNode.Name)
                            {
                                case "EpisodeNumber":
                                    int epNumber = 0;
                                    int.TryParse(subNode.InnerText, out epNumber);
                                    ep.DatabaseNumber = epNumber;
                                    break;
                                case "DVD_episodenumber":
                                    double dvdNumber = -1;
                                    if (double.TryParse(subNode.InnerText, out dvdNumber))
                                        ep.DatabaseDvdNumber = (int)dvdNumber;
                                    break;
                                case "EpisodeName":
                                    ep.DatabaseName = subNode.InnerText;
                                    break;
                                case "SeasonNumber":
                                    season = Convert.ToInt32(subNode.InnerText);
                                    ep.Season = season;
                                    break;
                                case "FirstAired":
                                    DateTime airData;
                                    DateTime.TryParse(subNode.InnerText, out airData);
                                    ep.AirDate = airData;
                                    break;
                                case "Overview":
                                    ep.Overview = subNode.InnerText;
                                    break;
                            }
                        }
                        ep.InDatabase = true;

                        if (ep.Number > -1 && season > -1)
                        {
                            if (!show.Seasons.Contains(season))
                                show.Seasons.Add(new TvSeason(season));

                            // If episode already exists just update it, else add it
                            TvEpisode existingMatch;
                            if (show.FindEpisode(ep.Season, ep.DatabaseNumber, true, out existingMatch))
                            {
                                existingMatch.DatabaseName = ep.DatabaseName;
                                existingMatch.AirDate = ep.AirDate;
                                existingMatch.Overview = ep.Overview;
                                existingMatch.InDatabase = true;
                            }
                            else
                                show.Seasons[season].Episodes.Add(ep);
                        }
                    }
                }

                showDoc = null;
            }
            catch(Exception e)
            {
                success = false;
            }
            finally
            {
                if (Directory.Exists(tempPath))
                    Directory.Delete(tempPath, true);
            }

            return success;
        }

        #endregion
    }
}
