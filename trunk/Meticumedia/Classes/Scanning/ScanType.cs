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
    public enum ScanType 
    { 
        [Description("Scan Folder Check")]
        Directory,
        [Description("TV Episode Missing Check")]
        TvMissing,
        [Description("TV Episode Rename Check")]
        TvRename,
        [Description("TV Root Folder Check")]
        TvFolder,
        [Description("Movie Root Check")]
        MovieFolder 
    };
}
