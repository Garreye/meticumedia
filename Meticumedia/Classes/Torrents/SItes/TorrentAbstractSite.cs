using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    public class TorrentAbstractSite
    {
        public TvEpisodeTorrent GetBestTvTorrent(TvEpisode episode)
        {
            List<TvEpisodeTorrent> availableTorrents = GetAvailableTvTorrents(episode);
            if (availableTorrents.Count == 0)
                return null;

            Dictionary<TorrentQuality, TvEpisodeTorrent> qualityTorrents = new Dictionary<TorrentQuality, TvEpisodeTorrent>();
            foreach (TvEpisodeTorrent torrent in availableTorrents)
            {
                TorrentQuality quality = torrent.Quality;

                if (!qualityTorrents.ContainsKey(quality))
                    qualityTorrents.Add(quality, null);

                if (qualityTorrents[quality] == null)
                    qualityTorrents[quality] = torrent;
            }

            // Go through matches to find closest quality to preferred
            TorrentQuality closestQuality = TorrentQuality.Sd480p;
            if (qualityTorrents.ContainsKey(Settings.General.PreferredTorrentQuality))
                closestQuality = Settings.General.PreferredTorrentQuality;
            else
            {
                switch (Settings.General.PreferredTorrentQuality)
                {
                    case TorrentQuality.Sd480p:
                        if (qualityTorrents.ContainsKey(TorrentQuality.Hd720p))
                            closestQuality = TorrentQuality.Hd720p;
                        else
                            closestQuality = TorrentQuality.Hd1080p;
                        break;
                    case TorrentQuality.Hd720p:
                        if (qualityTorrents.ContainsKey(TorrentQuality.Sd480p))
                            closestQuality = TorrentQuality.Sd480p;
                        else
                            closestQuality = TorrentQuality.Hd1080p;
                        break;
                    case TorrentQuality.Hd1080p:
                        if (qualityTorrents.ContainsKey(TorrentQuality.Hd720p))
                            closestQuality = TorrentQuality.Hd720p;
                        else
                            closestQuality = TorrentQuality.Sd480p;
                        break;
                    default:
                        throw new Exception("Unknown torrent quality");
                }
            }

            TvEpisodeTorrent closestTorrent = qualityTorrents[closestQuality];
            UpdateTorrentLinks(closestTorrent);

           return closestTorrent;
        }

        /// <summary>
        /// Get available torrents for episode
        /// </summary>
        /// <returns></returns>
        public virtual List<TvEpisodeTorrent> GetAvailableTvTorrents(TvEpisode episode)
        {
            throw new NotImplementedException();
        }

        protected virtual void UpdateTorrentLinks(TvEpisodeTorrent torrent)
        {
            throw new NotImplementedException();
        }
    }
}
