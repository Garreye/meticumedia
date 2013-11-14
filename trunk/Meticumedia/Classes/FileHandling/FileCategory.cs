using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meticumedia
{
    /// <summary>
    /// Categories of useful file types
    /// </summary>
    public enum FileCategory { Unknown = 1, Ignored = 2, TvVideo = 4, NonTvVideo = 8, Trash = 16, Custom = 32, Folder = 64, All = 127 };
}
