using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    public enum TorrentDownload
    {
        [Description("Launch Magnet Link")]
        Magnet,

        [Description("Download Torrent")]
        DownloadTorrent,

        [Description("Download Torrent and Open")]
        DownloadAndOpenTorrent
    }
}
