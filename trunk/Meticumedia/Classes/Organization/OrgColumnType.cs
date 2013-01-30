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
    /// Enum of types of columns that can be used for diplaying information in listviews containing OrgItem objects
    /// </summary>
    public enum OrgColumnType { DateTime = 1, Status = 2, Show = 4, Season = 8, Episode = 16, Source_Folder = 32, Source_File = 64, Movie = 128, Category = 256,
        Action = 512, Progress = 1024, Destination_Folder = 2048, Destination_File = 4096, Number = 8192}
}
