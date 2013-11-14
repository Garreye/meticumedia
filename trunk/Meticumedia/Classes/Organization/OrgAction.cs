// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meticumedia
{
    /// <summary>
    /// All possible organization actions that can be performed.
    /// </summary>
    public enum OrgAction { None = 0,  AlreadyExists = 1, Move = 2, Copy = 4, Rename = 8, Delete = 16, Queued = 32, NoRootFolder = 64, TBD = 128, Processing = 256, All = 511 };

}
