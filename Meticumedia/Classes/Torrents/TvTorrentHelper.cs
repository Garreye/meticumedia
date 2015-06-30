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
        public static readonly string BASE_URL = "http://kat.cr";

        public static TvEpisodeTorrent GetEpisodeTorrent(TvEpisode episode)
        {            
            // Download show episodes page
            string showPageUrl = BASE_URL + "/usearch/" + episode.BuildEpString().Replace(" ", "%20") + "/";
            string showPage;
            HttpWebRequest wr = WebRequest.CreateHttp(showPageUrl);
            try
            {
                var response = wr.GetResponse();
                using (var z = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                {
                    using (var reader = new StreamReader(z, Encoding.UTF8))
                    {
                        showPage = reader.ReadToEnd();
                    }
                }
            }
            catch
            {
                return null;
            }

            // Get regex for finding torrent download/magnet link
            Regex torrentLinkRegex;
            switch (Settings.General.TorrentDownload)
            {
                case TorrentDownload.Magnet:
                    torrentLinkRegex = new Regex("<a\\W*data-nop\\W*title=\"Torrent magnet link\"\\W*href=\"([^\"]+)\"");
                    break;
                case TorrentDownload.DownloadTorrent:
                case TorrentDownload.DownloadAndOpenTorrent:
                    torrentLinkRegex = new Regex("<a\\W*data-download\\W*title=\"Download torrent file\"\\W*href=\"([^\"]+)\"");
                    break;
                default:
                    throw new Exception("Unkown torrent download type");
            }
            
            MatchCollection matches = torrentLinkRegex.Matches(showPage);

            Dictionary<TorrentQuality, TvEpisodeTorrent> matchingEpisodes = new Dictionary<TorrentQuality, TvEpisodeTorrent>();
            foreach (Match match in matches)
            {
                string torrentLink = match.Groups[1].Value;

                string name; Regex nameRegex;
                switch (Settings.General.TorrentDownload)
                {
                    case TorrentDownload.Magnet:
                        nameRegex = new Regex("dn=(?<name>[^&]+)");
                        break;
                    case TorrentDownload.DownloadTorrent:
                    case TorrentDownload.DownloadAndOpenTorrent:
                        nameRegex = new Regex("^(?<link>[^?]+)\\?title=(?<name>.*)");
                        break;
                    default:
                        throw new Exception("Unkown torrent download type");
                }

                Match nameMatch = nameRegex.Match(torrentLink);
                if (!nameMatch.Success)
                    continue;

                name = nameMatch.Groups["name"].Value.Replace("+", " ");
                if (!string.IsNullOrEmpty(nameMatch.Groups["link"].Value))
                    torrentLink = nameMatch.Groups["link"].Value;

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

                    switch (Settings.General.TorrentDownload)
                    {
                        case TorrentDownload.Magnet:
                            torrentEp.Magnet = torrentLink;
                            break;
                        case TorrentDownload.DownloadTorrent:
                        case TorrentDownload.DownloadAndOpenTorrent:
                            torrentEp.File = torrentLink;
                            break;
                        default:
                            throw new Exception("Unkown torrent download type");
                    }

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

            return matchingEpisodes[closestQuality];
        }

    }
}
