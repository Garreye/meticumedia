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
    /// Types of content that can be found in content folders
    /// </summary>
    public enum ContentType 
    {
        [Description("Movie")]
        Movie,

        [Description("TV Show")]
        TvShow,

        [Description("Undefined")]
        Undefined
    }
}
