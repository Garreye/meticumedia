// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Meticumedia.Classes
{
    /// <summary>
    /// All possible organization actions that can be performed.
    /// </summary>
    public enum OrgAction 
    {
        [Description("Unknown")]
        Empty = 0,

        [Description("None")]
        None = 1,

        [Description("Destination already exists")]
        AlreadyExists = 2,

        [Description("Move")]
        Move = 4,

        [Description("Copy")]
        Copy = 8,

        [Description("Rename")]
        Rename = 16,

        [Description("Delete")]
        Delete = 32,

        [Description("Queued")]
        Queued = 64,

        [Description("No root folder set in settings!")]
        NoRootFolder = 128,

        [Description("TBD")]
        TBD = 256,

        [Description("Processing")]
        Processing = 512,

        [Description("Multiple")]
        All = 1023 };
}
