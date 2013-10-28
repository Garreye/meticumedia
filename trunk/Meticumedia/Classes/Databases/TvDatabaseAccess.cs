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

namespace Meticumedia
{
    /// <summary>
    /// Base class for TV database accessors (e.g. TheTvDb or TVRage)
    /// </summary>
    public class TvDatabaseAccess
    {
        #region Constants/Enums

        /// <summary>
        /// API Key for accessing database
        /// </summary>
        protected virtual string API_KEY { get { return string.Empty; } }

        /// <summary>
        /// Database mirror types
        /// </summary>
        public enum MirrorType { Xml = 1, Banner = 2, Zip = 4 }

        #endregion

        #region Variables

        /// <summary>
        /// Indicates whether mirors are valid.
        /// </summary>
        protected bool mirrorsValid = false;

        /// <summary>
        /// XML mirrors
        /// </summary>
        protected List<string> xmlMirrors;

        /// <summary>
        /// Zip mirrors
        /// </summary>
        protected List<string> zipMirrors;

        #endregion

        #region Mirrors

        /// <summary>
        /// Updates local list of database mirrors.
        /// </summary>
        public virtual void UpdatesMirrors()
        {
            mirrorsValid = true;
        }

        /// <summary>
        /// Gets a database mirror.
        /// </summary>
        /// <returns>Whether mirror was found</returns>
        public bool GetMirror(MirrorType type, out string mirror)
        {
            if (!mirrorsValid)
                UpdatesMirrors();

            if (mirrorsValid)
            {
                // Randomly select mirror
                switch (type)
                {
                    case MirrorType.Xml:
                        mirror = xmlMirrors[(new Random()).Next(xmlMirrors.Count)];
                        return true;
                    case MirrorType.Zip:
                        mirror = zipMirrors[(new Random()).Next(zipMirrors.Count)];
                        return true;
                }
            }

            mirror = string.Empty;
            return false;
        }

        #endregion

        #region Searching/Updating

        /// <summary>
        /// Gets database server's time
        /// </summary>
        /// <param name="time">retrieved server time</param>
        /// <returns>whether time was successfully retrieved</returns>
        public virtual bool GetServerTime(out string time)
        {
            time = string.Empty;
            return true;
        }

        /// <summary>
        /// Return lists of series Ids that need updating. 
        /// </summary>
        /// <param name="ids">List of series id that need to be updated locally</param>
        /// <returns></returns>
        public virtual bool GetDataToBeUpdated(out List<int> ids, out string time)
        {
            ids = new List<int>();
            time = string.Empty;
            return true;
        }

        /// <summary>
        /// Performs search for a show in database - with retrying
        /// </summary>
        /// <param name="searchString">The string to search for</param>
        /// <returns>Array of results from the search</returns>
        public List<Content> PerformTvShowSearch(string searchString, bool includeSummaries)
        {
            string mirror;
            if (!GetMirror(MirrorType.Xml, out mirror))
                return null;

            // Try multiple times - databases requests tend to fail randomly
            for (int i = 0; i < 5; i++)
                try
                {
                    return DoSearch(mirror, searchString, includeSummaries);
                }
                catch { }
            return new List<Content>();
        }

        /// <summary>
        /// Performs search for show in database. Should be overriden
        /// </summary>
        /// <param name="mirror">Mirror to use</param>
        /// <param name="searchString">Search string for show</param>
        /// <param name="includeSummaries">Whether to include summaries in search results (takes longer - set to false unless user is seeing them)</param>
        /// <returns>Results as list of shows</returns>
        protected virtual List<Content> DoSearch(string mirror, string searchString, bool includeSummaries)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets season/episode information from database.
        /// </summary>
        /// <param name="show">Show to load episode information into</param>
        public void FullShowSeasonsUpdate(TvShow show)
        {
            // Check for invalid ID
            if (show.Id == 0)
                return;

            // Try multiple times - databases requests tend to fail randomly
            for (int  i = 0; i < 5; i++)
                if(DoUpdate(show))
                {

                    // Remove episodes that are no longer in the TvDb
                    for (int j = show.Seasons.HighestSeason; j >= show.Seasons.LowestSeason; j--)
                        if(j >= 0)
                            for (int k = show.Seasons[j].Episodes.Count - 1; k >= 0; k--)
                            {
                                TvEpisode ep = show.Seasons[j].Episodes[k];
                                if (!ep.InDatabase && !ep.UserDefined && !ep.PreventDatabaseUpdates)
                                    show.Seasons[j].Episodes.Remove(ep);
                            }

                    // Update missing episodes for show
                    show.UpdateMissing();
                    show.LastUpdated = DateTime.Now;
                    break;
                }
        }

        /// <summary>
        /// Performs update of TV show information from database. Should be overriden!
        /// </summary>
        /// <param name="show">Show instance to update</param>
        /// <returns>Updated show instance</returns>
        public virtual bool DoUpdate(TvShow show)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
