using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    public class RarbgTorrentSite : TorrentAbstractSite
    {
        private static readonly string BASE_URL = "http://rarbg.to";

        public override List<TvEpisodeTorrent> GetAvailableTvTorrents(TvEpisode episode)
        {
            // Download show episodes page
            string showPageUrl = BASE_URL + "/torrents.php?search=" + episode.BuildEpString().Replace(" ", "+");
            WebClient client = new WebClient();
            client.Headers.Add("User-Agent: Other");
            string data = client.DownloadString(showPageUrl);

            Regex baseRegex = new Regex(@"<tr\s+class=.lista2.>\s*(<td.*?/td>)\s*(<td.*?/td>)\s*(<td.*?/td>)\s*(<td.*?/td>)\s*(<td.*?/td>)\s*(<td.*?/td>)\s*(<td.*?/td>)\s*</tr>");

            Regex titleRegex = new Regex(@"title=""([^""]+)""");
            Regex urlRegex = new Regex("href=\"(/torrent/[^\"]+)\"");

            Regex sizeRegex = new Regex(@">(\S+)\s+MB<");

            MatchCollection matches = baseRegex.Matches(data);

            List<TvEpisodeTorrent> torrents = new List<TvEpisodeTorrent>();
            foreach (Match match in matches)
            {
                string name;
                Match titleMatch = titleRegex.Match(match.Groups[2].Value);
                if (titleMatch.Success)
                    name = titleMatch.Groups[1].Value;
                else
                    continue;

                string pageUrl;
                Match urlMatch = urlRegex.Match(match.Groups[2].Value);
                if (urlMatch.Success)
                    pageUrl = BASE_URL + urlMatch.Groups[1].Value;
                else
                    continue;

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
                    torrentEp.PageUrl = pageUrl;

                    torrents.Add(torrentEp);
                }
            }

            return torrents;
        }

        protected override void UpdateTorrentLinks(TvEpisodeTorrent torrent)
        {
            // Link is for page, need to get actual torrent
            WebClient pageClient = new WebClient();
            pageClient.Headers.Add("User-Agent: Other");
            string page = pageClient.DownloadString(torrent.PageUrl);

            Regex torrentRegex = new Regex(@">\s*Torrent:\s*</td>.*?href=""(/download.php?[^\""]+)""");
            Regex magnetRegex = new Regex(@">\s*Torrent:\s*</td>.*?href=""(magnet:[^""]+)""");

            Match torrentMatch = torrentRegex.Match(page);
            if (torrentMatch.Success)
                torrent.File = BASE_URL + torrentMatch.Groups[1].Value;

            Match magnetMatch = magnetRegex.Match(page);
            if (magnetMatch.Success)
                torrent.Magnet = magnetMatch.Groups[1].Value;
        }
    }
}
