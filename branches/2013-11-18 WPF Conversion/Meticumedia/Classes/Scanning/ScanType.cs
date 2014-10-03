// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Meticumedia.Classes
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
        [Description("TV Folder Organization")]
        TvFolder,
        [Description("Movie Folder Organization")]
        MovieFolder 
    };
}
