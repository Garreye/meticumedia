using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    public class TvEpisodeTorrent
    {
        #region Properties

        public string Url { get; set; }
        
        public string Title { get; set; }

        public int Season { get; set; }

        public int Episode { get; set; }

        public int Episode2 { get; set; }

        public TorrentQuality Quality { get; set; }

        public TorrentFlag Flag { get; set; }

        public string PageUrl { get; set; }

        public string File { get; set; }

        public string Magnet { get; set; }

        #endregion

        public TvEpisodeTorrent()
        {
            this.Quality = TorrentQuality.Sd480p;
            this.Flag = TorrentFlag.None;
        }

        public TvEpisodeTorrent(TvEpisodeTorrent clone)
        {
            this.Url = clone.Url;
            this.Season = clone.Season;
            this.Episode = clone.Episode;
            this.Episode2 = clone.Episode2;
            this.Quality = clone.Quality;
            this.Flag = clone.Flag;
            this.File = clone.File;
            this.Magnet = clone.Magnet;
        }
    }
}
