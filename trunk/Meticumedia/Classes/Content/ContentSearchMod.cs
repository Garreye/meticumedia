using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meticumedia
{
    /// <summary>
    /// Types of modification that can be made to search strings
    /// </summary>
    public enum ContentSearchMod { None = 0, YearRemoved = 1, SpaceRemoved = 2, WordsRemoved = 4, WordSlit = 8, TheAdded = 16, All = 31 }
}
