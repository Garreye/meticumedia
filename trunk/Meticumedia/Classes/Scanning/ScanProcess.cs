using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Meticumedia
{    
    /// <summary>
    /// Portions of scan processing that can be running
    /// </summary>
    public enum ScanProcess 
    {
        [Description("Collecting Files to be Analyzed")]
        FileCollect,
        [Description("Scanning Folder")]
        Directory,
        [Description("Scanning Show for Missing Episodes")]
        TvMissing,
        [Description("Scanning Show for Episode that need to be Renamed")]
        TvRename,
        [Description("Scanning TV Show Root Folder")]
        TvFolder,
        [Description("Scanning Movie Root Folder")]
        Movie 
    };
}
