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
    public static class TorrentTvAccess
    {        
        #region Constants

        public static readonly string BASE_URL = "http://kat.cr";

        #endregion

        public static TorrentTvEpisode GetEpisode(TvEpisode episode)
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
            
            Regex magnetLinkRegex = new Regex("<a\\W*data-nop\\W*title=\"Torrent magnet link\"\\W*href=\"([^\"]+)\"");
            MatchCollection matches = magnetLinkRegex.Matches(showPage);

            Dictionary<TorrentQuality, Dictionary<TorrentFlag, TorrentTvEpisode>> matchingEpisodes = new Dictionary<TorrentQuality, Dictionary<TorrentFlag, TorrentTvEpisode>>();
            foreach (Match match in matches)
            {
                string magnet = match.Groups[1].Value;
                Regex nameRegex = new Regex("dn=([^&]+)");
                Match nameMatch = nameRegex.Match(magnet);

                if (nameMatch.Success)
                {
                    string name = nameMatch.Groups[1].Value.Replace("+", " ");

                    MatchCollection showMatches = episode.Show.MatchFileToContent(name);
                    bool matchShow = showMatches != null && showMatches.Count > 0;

                    int season, ep1, ep2;
                    if (matchShow  && FileHelper.GetEpisodeInfo(name, episode.Show.DisplayName, out season, out ep1, out ep2) && episode.Season == season && episode.DisplayNumber == ep1)
                    {
                        TorrentQuality quality = TorrentQuality.Sd480p;
                        if (name.ToLower().Contains("720p"))
                            quality = TorrentQuality.Hd720p;
                        else if (name.ToLower().Contains("1080p"))
                            quality = TorrentQuality.Hd1080p;

                        // Get flags
                        TorrentFlag flag = TorrentFlag.None;
                        if (name.ToLower().Contains("proper"))
                            flag = TorrentFlag.Proper;
                        else if (name.ToLower().Contains("internal"))
                            flag = TorrentFlag.Internal;

                        TorrentTvEpisode torrentEp = new TorrentTvEpisode();
                        torrentEp.Season = season;
                        torrentEp.Episode = ep1;
                        torrentEp.Episode2 = ep2;
                        torrentEp.Quality = quality;
                        torrentEp.Flag = flag;
                        torrentEp.Magnet = magnet;
                        torrentEp.Title = name;

                        if (!matchingEpisodes.ContainsKey(quality))
                            matchingEpisodes.Add(quality, new Dictionary<TorrentFlag, TorrentTvEpisode>());

                        if (!matchingEpisodes[quality].ContainsKey(flag))
                            matchingEpisodes[quality].Add(flag, null);

                        if (matchingEpisodes[quality][flag] == null)
                            matchingEpisodes[quality][flag] = torrentEp;
                    }                    
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

            TorrentFlag useFlag;
            if (matchingEpisodes[closestQuality].ContainsKey(TorrentFlag.Proper))
                useFlag = TorrentFlag.Proper;
            else if (matchingEpisodes[closestQuality].ContainsKey(TorrentFlag.None))
                useFlag = TorrentFlag.None;
            else
                useFlag = TorrentFlag.Internal;

            return matchingEpisodes[closestQuality][useFlag];
        }

    }
}
