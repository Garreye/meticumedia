// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Base class for TV database accessors (e.g. TheTvDb or TVRage)
    /// </summary>
    public class TvDatabaseAccess : DatabaseAccess
    {
        /// <summary>
        /// Gets season/episode information from database.
        /// </summary>
        /// <param name="show">Show to load episode information into</param>
        public void FullUpdate(TvShow show)
        {
            // Check for invalid ID
            if (show.Id <= 0)
                return;

            // Try multiple times - databases requests tend to fail randomly
            for (int  i = 0; i < 5; i++)
                if (Update(show))
                {

                    // Remove episodes that are no longer in the TvDb
                    for (int k = show.Episodes.Count - 1; k >= 0; k--)
                    {
                        TvEpisode ep = show.Episodes[k];
                        if (!ep.InDatabase && !ep.UserDefined && !ep.PreventDatabaseUpdates)
                            show.Episodes.Remove(ep);
                    }

                    // Update missing episodes for show
                    show.UpdateMissing();
                    show.LastUpdated = DateTime.Now;
                    break;
                }
        }

    }
}