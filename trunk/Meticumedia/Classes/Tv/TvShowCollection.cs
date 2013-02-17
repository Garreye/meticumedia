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
    /// <summary>
    /// List of TvShow with added properties for collection.
    /// </summary>
    public class TvShowCollection : List<TvShow>
    {
        #region Properties

        /// <summary>
        /// Database time when collection was updated.
        /// </summary>
        public string LastUpdate { get; set; }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public TvShowCollection()
            : base()
        {

        }
    }
}
