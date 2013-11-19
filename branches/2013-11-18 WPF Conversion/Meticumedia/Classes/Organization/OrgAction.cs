// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meticumedia.Classes
{
    /// <summary>
    /// All possible organization actions that can be performed.
    /// </summary>
    public enum OrgAction { Empty = 0, None = 1,  AlreadyExists = 2, Move = 4, Copy = 8, Rename = 16, Delete = 32, Queued = 64, NoRootFolder = 128, TBD = 256, Processing = 512, All = 1023 };

}
