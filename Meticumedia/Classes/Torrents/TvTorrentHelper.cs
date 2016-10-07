using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    public static class TvTorrentHelper
    {        
        public static TvEpisodeTorrent GetEpisodeTorrent(TvEpisode episode)
        {            
            // Download show episodes page
            string showPageUrl = "http://rarbg.to/torrents.php?search=" + episode.BuildEpString().Replace(" ", "+");
            WebClient client = new WebClient();
            client.Headers.Add("User-Agent: Other");
            string data = client.DownloadString(showPageUrl);

            Regex baseRegex = new Regex(@"<tr\s+class=.lista2.>\s*(<td.*?/td>)\s*(<td.*?/td>)\s*(<td.*?/td>)\s*(<td.*?/td>)\s*(<td.*?/td>)\s*(<td.*?/td>)\s*(<td.*?/td>)\s*</tr>");

            Regex titleRegex = new Regex(@"title=""([^""]+)""");
            Regex urlRegex = new Regex("href=\"(/torrent/[^\"]+)\"");

            Regex sizeRegex = new Regex(@">(\S+)\s+MB<");

            MatchCollection matches = baseRegex.Matches(data);
            Dictionary<TorrentQuality, TvEpisodeTorrent> matchingEpisodes = new Dictionary<TorrentQuality, TvEpisodeTorrent>();

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
                    pageUrl = "http://rarbg.to" + urlMatch.Groups[1].Value;
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

                    if (!matchingEpisodes.ContainsKey(quality))
                        matchingEpisodes.Add(quality, null);

                    if (matchingEpisodes[quality] == null)
                        matchingEpisodes[quality] = torrentEp;

                }
            }

            if (matchingEpisodes.Count == 0)
                return null;

            // Go through matches to find closest quality to preferred
            TorrentQuality closestQuality = TorrentQuality.Sd480p;
            if (matchingEpisodes.ContainsKey(Settings.General.PreferredTorrentQuality))
                closestQuality = Settings.General.PreferredTorrentQuality;
            else
            {
                switch (Settings.General.PreferredTorrentQuality)
                {
                    case TorrentQuality.Sd480p:
                        if (matchingEpisodes.ContainsKey(TorrentQuality.Hd720p))
                            closestQuality = TorrentQuality.Hd720p;
                        else
                            closestQuality = TorrentQuality.Hd1080p;
                        break;
                    case TorrentQuality.Hd720p:
                        if (matchingEpisodes.ContainsKey(TorrentQuality.Sd480p))
                            closestQuality = TorrentQuality.Sd480p;
                        else
                            closestQuality = TorrentQuality.Hd1080p;
                        break;
                    case TorrentQuality.Hd1080p:
                        if (matchingEpisodes.ContainsKey(TorrentQuality.Hd720p))
                            closestQuality = TorrentQuality.Hd720p;
                        else
                            closestQuality = TorrentQuality.Sd480p;
                        break;
                    default:
                        throw new Exception("Unknown torrent quality");
                }
            }

            // Link is for page, need to get actual torrent
            WebClient pageClient = new WebClient();
            pageClient.Headers.Add("User-Agent: Other");
            string page = pageClient.DownloadString(matchingEpisodes[closestQuality].File);

            Regex torrentRegex = new Regex(@">\s*Torrent:\s*</td>.*?href=""(/download.php?[^\""]+)""");
            Regex magnetRegex = new Regex(@">\s*Torrent:\s*</td>.*?href=""(magnet:[^""]+)""");

            Match torrentMatch = torrentRegex.Match(page);
            if (torrentMatch.Success)
                matchingEpisodes[closestQuality].File = "http://rarbg.to" + torrentMatch.Groups[1].Value;

            Match magnetMatch = magnetRegex.Match(page);
            if (magnetMatch.Success)
                matchingEpisodes[closestQuality].Magnet = magnetMatch.Groups[1].Value;

            return matchingEpisodes[closestQuality];
        }

    }
}
