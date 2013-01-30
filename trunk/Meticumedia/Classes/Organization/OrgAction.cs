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
    /// Enum of possible organization actions that can be performed on files.
    /// </summary>
    public enum OrgAction { None,  AlreadyExists, Move, Copy, Rename, Delete, Queued, Processing, NoRootFolder };

}
