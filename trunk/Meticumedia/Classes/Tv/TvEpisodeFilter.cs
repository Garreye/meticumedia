using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meticumedia
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
        public enum FilterType { All, Missing, InScanDir, Unaired, Season };

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
            switch (this.Type)
            {
                case FilterType.All:
                    return true;
                case FilterType.Missing:
                    if (ep.Missing == TvEpisode.MissingStatus.Missing && ep.Aired)
                        return true;
                    break;
                case FilterType.InScanDir:
                     if (ep.Missing == TvEpisode.MissingStatus.InScanDirectory)
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
            switch (this.Type)
            {
                case FilterType.All:
                    return "All Episodes";
                case FilterType.Missing:
                    return "Missing Episodes";
                case FilterType.InScanDir:
                    return "Episodes in Scan Directory";
                    break;
                case FilterType.Season:
                    return "Season " + this.Season;
                case FilterType.Unaired:
                    return "Unaired";
                default:
                    throw new Exception("Unknown type");
            }
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
        public static List<TvEpisodeFilter> BuildFilters(TvShow show, bool displayIgnored, bool seasons)
        {
            List<TvEpisodeFilter> filters = new List<TvEpisodeFilter>();

            filters.Add(new TvEpisodeFilter(FilterType.All, 0));
            filters.Add(new TvEpisodeFilter(FilterType.Missing, 0));
            filters.Add(new TvEpisodeFilter(FilterType.InScanDir, 0));
            filters.Add(new TvEpisodeFilter(FilterType.Unaired, 0));

            if (seasons)
                foreach (TvSeason season in show.Seasons)
                    if (!season.Ignored || displayIgnored)
                        filters.Add(new TvEpisodeFilter(FilterType.Season, season.Number));

            return filters;
        }

        #endregion
    }
}
