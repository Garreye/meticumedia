using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    public enum TorrentQuality
    {
        [Description("Standard (480p)")]
        Sd480p,

        [Description("HD (720p)")]
        Hd720p,

        [Description("HD (1080p)")]
        Hd1080p
    }
}
