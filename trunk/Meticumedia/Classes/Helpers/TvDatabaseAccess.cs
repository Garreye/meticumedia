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
    public class TvDatabaseAccess
    {
        #region Constants/Enums

        /// <summary>
        /// API Key for accessing TheTvDb
        /// </summary>
        protected virtual string API_KEY { get { return string.Empty; } }

        /// <summary>
        /// Database mirror types
        /// </summary>
        public enum MirrorType { Xml = 1, Banner = 2, Zip = 4 }

                /// <summary>
        /// TheTvDb XML mirror
        /// </summary>
        protected List<string> xmlMirrors;

        /// <summary>
        /// TheTvDb zip mirror
        /// </summary>
        protected List<string> zipMirrors;

        #endregion

        #region Variables

        /// <summary>
        /// Indicates wheter mirors are valid.
        /// </summary>
        protected static bool mirrorsValid = false;

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
        /// <returns></returns>
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
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual bool GetDataToBeUpdated(out List<int> info, out string time)
        {
            info = new List<int>();
            time = string.Empty;
            return true;
        }

        /// <summary>
        /// Performs search for a show in TheTvDb.
        /// </summary>
        /// <param name="searchString">The string to search for</param>
        /// <returns>Array of results from the search</returns>
        public List<Content> PerformTvShowSearch(string searchString, bool includeSummaries)
        {
            string mirror;
            if (!GetMirror(MirrorType.Xml, out mirror))
                return null;

            for (int i = 0; i < 5; i++)
                try
                {
                    return DoSearch(mirror, searchString, includeSummaries);
                }
                catch { }
            return new List<Content>();
        }

        protected virtual List<Content> DoSearch(string mirror, string searchString, bool includeSummaries)
        {
            return new List<Content>();
        }

        /// <summary>
        /// Gets season/episode information from TheTvDb. Use for newly added shows only,
        /// will replace all episode information in show.
        /// </summary>
        /// <param name="show">Show to load episode information into</param>
        public TvShow FullShowSeasonsUpdate(TvShow show)
        {
            // Check for invalid ID
            if (show.Id == 0)
                return show;

            for (int  i = 0; i < 5; i++)
                try
                {
                    show =  DoUpdate(show);

                    // Remove episodes that are no longer in the TvDb
                    for (int j = show.Seasons.HighestSeason; j >= show.Seasons.LowestSeason; j--)
                        for (int k = show.Seasons[j].Episodes.Count - 1; k >= 0; k--)
                            if (!show.Seasons[j].Episodes[k].InDatabase)
                                show.Seasons[j].Episodes.Remove(show.Seasons[j].Episodes[k]);

                    // Update missing episodes for show
                    show.UpdateMissing();
                    show.LastUpdated = DateTime.Now;
                    break;
                }
                catch { }

            return show;
        }

        public virtual TvShow DoUpdate(TvShow show)
        {
            return show;
        }

        #endregion
    }
}
