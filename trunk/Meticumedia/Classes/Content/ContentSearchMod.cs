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
    /// Types of modification that can be made to search strings
    /// </summary>
    public enum ContentSearchMod { None = 0, YearRemoved = 1, SpaceRemoved = 2, WordsRemoved = 4, WordSlit = 8, TheAdded = 16, BrackRemoval = 32, All = 63 }
}
