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
    /// Categories of useful file types
    /// </summary>
    public enum FileCategory 
    {
        [Description("Uncategorized")]
        Empty = 0,

        [Description("Unknown")]
        Unknown = 1,

        [Description("Ignored")]
        Ignored = 2,

        [Description("TV Video")]
        TvVideo = 4,

        [Description("Movie Video")]
        MovieVideo = 8,

        [Description("Trash")]
        Trash = 16,

        [Description("Custom")]
        Custom = 32,

        [Description("Folder")]
        Folder = 64,

        [Description("Mutiple categories")]
        All = 127 
    };
}
