// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Meticumedia
{
    public class TvShowCollection : List<TvShow>
    {
        #region Properties

        public string LastUpdate { get; set; }

        #endregion

        public TvShowCollection()
            : base()
        {

        }
    }
}
