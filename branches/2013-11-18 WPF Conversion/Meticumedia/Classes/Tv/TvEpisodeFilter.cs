// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Defines filter that can be applied to TV episodes.
    /// </summary>
    public class TvEpisodeFilter
    {
        #region Properties

        /// <summary>
        /// Type of filters that can be applies to episodes.
        /// </summary>
        public enum FilterType { All, Regular, Ignored, Missing, Located, InScanDir, Unaired, Season };

        /// <summary>
        /// The type of episode filter being used.
        /// </summary>
        public FilterType Type { get; set; }

        /// <summary>
        /// The season number used when Type is season filter.
        /// </summary>
        public int Season { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with known properties
        /// </summary>
        /// <param name="type"></param>
        /// <param name="season"></param>
        public TvEpisodeFilter(FilterType type, int season)
        {
            this.Type = type;
            this.Season = season;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Runs an episode through filter.
        /// </summary>
        /// <param name="ep">The episode to filter</param>
        /// <returns>True if the episode makes it through filter</returns>
        public bool FilterEpisode(TvEpisode ep)
        {
            if (ep.Ignored && this.Type != FilterType.Ignored)
                return false;
            
            switch (this.Type)
            {
                case FilterType.All:
                    return true;
                case FilterType.Regular:
                    if (ep.Season > 0)
                        return true;
                    break;
                case FilterType.Ignored:
                    return ep.Ignored;
                case FilterType.Located:
                    if (ep.Missing != MissingStatus.Missing)
                        return true;
                    break;
                case FilterType.Missing:
                    if (ep.Season > 0 && ep.Missing == MissingStatus.Missing && ep.Aired)
                        return true;
                    break;
                case FilterType.InScanDir:
                     if (ep.Missing == MissingStatus.InScanDirectory)
                        return true;
                    break;
                case FilterType.Season:
                    if (ep.Season == this.Season)
                        return true;
                    break;
                case FilterType.Unaired:
                    if (!ep.Aired)
                        return true;
                    break;
                default:
                    throw new Exception("Unknown filter type!");
            }

            return false;
        }

        /// <summary>
        /// Return string for filter.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string filterString;
            switch (this.Type)
            {
                case FilterType.All:
                    filterString = "All Episodes";
                    break;
                case FilterType.Regular:
                    filterString = "All Regular Season Episodes";
                    break;
                case FilterType.Ignored:
                    filterString = "Ignored Episodes";
                    break;
                case FilterType.Missing:
                    filterString = "Missing Episodes";
                    break;
                case FilterType.Located:
                    filterString = "Located Episodes";
                    break;
                case FilterType.InScanDir:
                    filterString = "Episodes in Scan Directory";
                    break;
                case FilterType.Season:
                    if (this.Season == 0)
                        filterString = "Specials";
                    else
                        filterString = "Season " + this.Season;
                    break;
                case FilterType.Unaired:
                    filterString = "Unaired";
                    break;
                default:
                    throw new Exception("Unknown type");
            }

            return filterString;
        }

        /// <summary>
        /// Equals check for this filter and another filter.
        /// Uses ToString to compare.
        /// </summary>
        /// <param name="obj">EpisodeFilter to compare to</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            // False for null/invalid objects
            if (obj == null || !(obj is TvEpisodeFilter))
                return false;

            // Case object to episode
            TvEpisodeFilter epFilter = (TvEpisodeFilter)obj;

            // Compare is on season and episode number only (show name may not be set yet)
            return epFilter.ToString() == this.ToString();
        }

        /// <summary>
        /// Overrides to prevent warning that Equals is overriden but no GetHashCode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Build array of possible episode filters that can be used for a TvShow.
        /// </summary>
        /// <param name="show">The show to build filters for</param>
        /// <param name="displayIgnored">Whether to add ignored season filters</param>
        /// <returns></returns>
        public static List<TvEpisodeFilter> BuildFilters(TvShow show, bool schedule)
        {
            List<TvEpisodeFilter> filters = new List<TvEpisodeFilter>();

            filters.Add(new TvEpisodeFilter(FilterType.Regular, 0));
            filters.Add(new TvEpisodeFilter(FilterType.All, 0));
            if (!schedule)
                filters.Add(new TvEpisodeFilter(FilterType.Ignored, 0));
            filters.Add(new TvEpisodeFilter(FilterType.Missing, 0));
            filters.Add(new TvEpisodeFilter(FilterType.Located, 0));
            filters.Add(new TvEpisodeFilter(FilterType.InScanDir, 0));
            if (!schedule)
                filters.Add(new TvEpisodeFilter(FilterType.Unaired, 0));

            if (!schedule)
                foreach (int season in show.Seasons)
                    filters.Add(new TvEpisodeFilter(FilterType.Season, season));

            return filters;
        }

        #endregion
    }
}
