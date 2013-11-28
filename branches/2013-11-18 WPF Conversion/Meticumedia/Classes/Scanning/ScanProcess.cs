﻿// --------------------------------------------------------------------------------
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
    public enum ScanProcess 
    {
        [Description("Collecting Files to be Analyzed")]
        FileCollect,
        [Description("Checking Scan Folders")]
        Directory,
        [Description("Checking Shows for Missing Episodes")]
        TvMissing,
        [Description("Checking Shows for Episode That Need to be Renamed")]
        TvRename,
        [Description("Scanning TV Show Root Folders")]
        TvFolder,
        [Description("Scanning Movie Root Folders")]
        Movie 
    };
}