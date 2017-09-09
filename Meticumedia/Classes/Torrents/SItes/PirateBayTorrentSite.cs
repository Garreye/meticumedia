using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    public class PirateBayTorrentSite : TorrentAbstractSite
    {
        private static readonly string BASE_URL = "https://thepiratebay.org";

        private static RateLimiter Limiter = new RateLimiter(true, 1, 1000);

        public override List<TvEpisodeTorrent> GetAvailableTvTorrents(TvEpisode episode)
        {
            // Download show episodes page
            string showPageUrl = BASE_URL + "/search/" + episode.BuildEpString().Replace(" ", "%20");
            Limiter.DoWait();

            WebClient client = new WebClient();
            client.Headers.Add("User-Agent: Other");
            string data = client.DownloadString(showPageUrl);

            Regex baseRegex = new Regex("<div\\s+class=\\\"detName\\\">\\s+<a\\s+href=\\\"[^\\\"]+\\\"[^>]+>([^>]+)</a>\\s+</div>\\s+<a href=\\\"(magnet:[^\\\"]+)\\\"");
            MatchCollection matches = baseRegex.Matches(data);

            List<TvEpisodeTorrent> torrents = new List<TvEpisodeTorrent>();
            foreach (Match match in matches)
            {
                string name = match.Groups[1].Value;
                string magnet = match.Groups[2].Value;

                MatchCollection showMatches = episode.Show.MatchFileToContent(name);
                bool matchShow = showMatches != null && showMatches.Count > 0;

                int season, ep1, ep2;
                if (matchShow && FileHelper.GetEpisodeInfo(name, episode.Show.DisplayName, out season, out ep1, out ep2, true) && episode.Season == season && episode.DisplayNumber == ep1)
                {
                    // Don't want to get torrent with a bunch of episodes (double is okay)
                    if (ep2 > 0 && ep2 - ep1 > 1)
                        continue;

                    TorrentQuality quality = TorrentQuality.Sd480p;
                    if (name.ToLower().Contains("720p"))
                        quality = TorrentQuality.Hd720p;
                    else if (name.ToLower().Contains("1080p"))
                        quality = TorrentQuality.Hd1080p;

                    TvEpisodeTorrent torrentEp = new TvEpisodeTorrent();
                    torrentEp.Url = showPageUrl;
                    torrentEp.Season = season;
                    torrentEp.Episode = ep1;
                    torrentEp.Episode2 = ep2;
                    torrentEp.Quality = quality;
                    torrentEp.Title = name;
                    torrentEp.Magnet = magnet;

                    torrents.Add(torrentEp);
                }
            }

            return torrents;
        }

        protected override void UpdateTorrentLinks(TvEpisodeTorrent torrent)
        {
           
        }
    }
}
