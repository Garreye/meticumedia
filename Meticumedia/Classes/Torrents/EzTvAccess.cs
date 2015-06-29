using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    public static class EzTvAccess
    {        
        #region Constants

        public static readonly string BASE_URL = "https://eztv.ch/";

        #endregion

        #region Properties

        public static Dictionary<int, EzTvShow> Shows { get; set; }

        public static List<EzTvShow> ShowsList
        {
            get
            {
                return Shows.Values.ToList();
            }
        }

        #endregion

        #region Constructor

        static EzTvAccess()
        {
            Shows = new Dictionary<int, EzTvShow>();
        }

        #endregion

        /// <summary>
        /// Update list of shows on EzTv
        /// </summary>
        public static void UpdateShows()
        {
            // Check if shows need to be updated
            if (Shows.Count == 0)
            {
                // Download shows list
                string showListUrl = BASE_URL + "showlist/";
                WebClient webClient = new WebClient();
                string showList = webClient.DownloadString(showListUrl);

                // Build list of shows
                Regex showRegex = new Regex("<a\\W+href=\"/shows/(\\d+)/[^/]+/\"[^>]+>([^>]+)<");
                MatchCollection matches = showRegex.Matches(showList);
                foreach (Match match in matches)
                {
                    string name = match.Groups[2].Value;
                    if (name.ToLower().EndsWith(", the"))
                        name = "The " + name.Substring(0, name.Length - 5);

                    EzTvShow ezShow = new EzTvShow(name, int.Parse(match.Groups[1].Value));
                    if (!Shows.ContainsKey(ezShow.Id))
                        Shows.Add(ezShow.Id, ezShow);
                }
            }
        }

        public static void GetShowId(TvShow show)
        {
            if (show.Id <= 0)
                return;
            
            UpdateShows();

            // Initialize list of matches
            List<int> matchYears = new List<int>();
            List<EzTvShow> matches = new List<EzTvShow>();

            // Check all shows for name match
            foreach (EzTvShow ezShow in Shows.Values)
            {
                // Get year from show name
                int year = FileHelper.GetYear(ezShow.Name);
                
                // Simplify strings for matching
                string simplifiedShowName = FileHelper.SimplifyFileName(show.DatabaseName, true, true, true);
                string simplifiedEzName = FileHelper.SimplifyFileName(ezShow.Name, true, true, true);

                // Compare strings
                bool theAddedToMatch, singleLetterDiff;
                bool match = FileHelper.CompareStrings(simplifiedShowName, simplifiedEzName, out theAddedToMatch, out singleLetterDiff);

                if (match & !theAddedToMatch & !singleLetterDiff)
                {
                    matches.Add(ezShow);
                    matchYears.Add(year);
                }
            }

            if (matches.Count > 0)
            {
                // FInd match with closest year match
                int closestYear = int.MaxValue;
                EzTvShow closestShow = null;
                for (int i = 0; i < matches.Count; i++)
                {
                    int yearDiff = Math.Abs(show.DatabaseYear - matchYears[i]);
                    if (yearDiff < closestYear)
                    {
                        closestYear = yearDiff;
                        closestShow = matches[i];
                    }
                }

                // Set show
                show.EzTvShowId = closestShow.Id;

                // Get episodes for show
                GetEpisodes(show.EzTvShow);
            }
        }

        public static void GetEpisodes(EzTvShow show)
        {
            show.Episodes.Clear();
            
            // Download show episodes page
            string showPageUrl = BASE_URL + "shows/" + show.Id + "/";
            WebClient webClient = new WebClient();
            string showPage = webClient.DownloadString(showPageUrl);
            
            Regex episodeRegex = new Regex("<a\\W+href=\"/ep/\\d+/[^/]+/\"[^>]+>([^>]+)</a>\\W*</td>\\W*<td[^>]+>\\W*(?:<a\\W+href=\"([^\"]+)\"[^>]+>\\W*</a>\\W*)+</td>");
            MatchCollection matches = episodeRegex.Matches(showPage);

            foreach (Match match in matches)
            {
                EzTvEpisode ezEpisode = new EzTvEpisode();

                string episodeString = match.Groups[1].Value;
                int season, ep1, ep2;
                if (FileHelper.GetEpisodeInfo(episodeString, show.Name, out season, out ep1, out ep2))
                {
                    ezEpisode.Title = episodeString;
                    ezEpisode.Season = season;
                    ezEpisode.Episode = ep1;
                    ezEpisode.Episode2 = ep2;

                    // Get quality
                    if (episodeString.ToLower().Contains("1080p"))
                        ezEpisode.Quality = TorrentQuality.Hd1080p;
                    else if (episodeString.ToLower().Contains("720p"))
                        ezEpisode.Quality = TorrentQuality.Hd720p;

                    // Get flags
                    if (episodeString.ToLower().Contains("proper"))
                        ezEpisode.Flag = TorrentFlag.Proper;
                    else if (episodeString.ToLower().Contains("internal"))
                        ezEpisode.Flag = TorrentFlag.Internal;

                    // Get links
                    foreach (Capture capture in match.Groups[2].Captures)
                    {
                        if (capture.Value.StartsWith("http"))
                        {
                            string ext = Path.GetExtension(capture.Value);
                            if (ext == ".torrent")
                                ezEpisode.Mirrors.Add(capture.Value);
                        }
                        else
                            ezEpisode.Magnet = capture.Value;
                    }

                    show.Episodes.Add(ezEpisode);
                }
            }
        }

    }
}
