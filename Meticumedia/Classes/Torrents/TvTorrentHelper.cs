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
        public static async Task<TvEpisodeTorrent> GetEpisodeTorrentAsync(TvEpisode episode)
        {
            TvEpisodeTorrent result = await Task.Run(() => GetEpisodeTorrent(episode));
            return result;
        }

        public static TvEpisodeTorrent GetEpisodeTorrent(TvEpisode episode)
        {
            try
            {
                PirateBayTorrentSite site = new PirateBayTorrentSite();
                TvEpisodeTorrent torrent = site.GetBestTvTorrent(episode);
                return torrent;
            }
            catch
            {
                return null;
            }
        }

    }
}
