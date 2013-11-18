// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Meticumedia
{
    /// <summary>
    /// Formatting class for TV file names.
    /// </summary>
    public class TvEpisodeFormat
    {
        #region Properties

        /// <summary>
        /// Header string for season
        /// </summary>
        public string SeasonHeader { get; set; }

        /// <summary>
        /// Header string for episode
        /// </summary>
        public string EpisodeHeader { get; set; }

        /// <summary>
        /// Indicates whether to use header for each episode in string
        /// (e.g.  s01e21e22 (when true) vs. s01e2122 (when false))
        /// </summary>
        public bool HeaderPerEpisode { get; set; }

        /// <summary>
        /// Whether to put season number before episode number in string
        /// </summary>
        public bool SeasonFirst { get; set; }

        /// <summary>
        /// Whether to force season number to be 2 digits in string
        /// </summary>
        public bool ForceSeasonDoubleDigits { get; set; }

        /// <summary>
        /// Whether to force episode number to be 2 digits in string
        /// </summary>
        public bool ForceEpisodeDoubleDigits { get; set; }

        #endregion

        #region Constants/Enums

        /// <summary>
        /// Keywords used in formatting string.
        /// </summary>
        private enum EpisodeKeywords { season, episode }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TvEpisodeFormat()
        {
            this.SeasonHeader = "s";
            this.EpisodeHeader = "e";
            this.HeaderPerEpisode = true;
            this.SeasonFirst = true;
            this.ForceEpisodeDoubleDigits = true;
            this.ForceSeasonDoubleDigits = true;
        }

        /// <summary>
        /// Constuctor for copying instance
        /// </summary>
        /// <param name="format"></param>
        public TvEpisodeFormat(TvEpisodeFormat format)
        {
            this.SeasonHeader = format.SeasonHeader;
            this.EpisodeHeader = format.EpisodeHeader;
            this.HeaderPerEpisode = format.HeaderPerEpisode;
            this.SeasonFirst = format.SeasonFirst;
            this.ForceEpisodeDoubleDigits = format.ForceEpisodeDoubleDigits;
            this.ForceSeasonDoubleDigits = format.ForceSeasonDoubleDigits;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds episode string for TV episode for both single or multi-episode files.
        /// </summary>
        /// <param name="ep1">First episode</param>
        /// <param name="ep2">Second episode (if double episode file)</param>
        /// <returns>Resulting episode string</returns>
        public string BuildEpisodeString(TvEpisode ep1, TvEpisode ep2)
        {
            // Start out with formatting string
            string epStr = string.Empty;

            // Build episode string
            for (int i = 0; i < 2; i++)
            {
                bool season = (i == 0 && this.SeasonFirst) || (i == 1 && !this.SeasonFirst);

                if (season)
                {
                    if (this.ForceSeasonDoubleDigits)
                        epStr += this.SeasonHeader + ep1.Season.ToString("00");
                    else
                        epStr += this.SeasonHeader + ep1.Season.ToString();
                }
                else
                {
                    if (this.ForceEpisodeDoubleDigits)
                        epStr += this.EpisodeHeader + ep1.Number.ToString("00");
                    else
                        epStr += this.EpisodeHeader + ep1.Number.ToString();

                    if (ep2 != null)
                    {
                        if (this.HeaderPerEpisode)
                            epStr += this.EpisodeHeader;

                        if (this.ForceEpisodeDoubleDigits)
                            epStr += ep2.Number.ToString("00");
                        else
                            epStr += ep2.Number.ToString(); 
                    }
                }
            }
            
            // Return file name
            return epStr;
        }

        #endregion

        #region XML
        
        /// <summary>
        /// Element names for properties that need to be saved to XML.
        /// </summary>
        private enum XmlElements { SeasonHeader, EpisodeHeader, HeaderPerEpisode, SeasonFirst, ForceEpisodeDoubleDigits, ForceSeasonDoubleDigits };

        /// <summary>
        /// Saves instance properties to XML file.
        /// </summary>
        /// <param name="xw">Writer for accessing XML file</param>
        public void Save(XmlWriter xw)
        {
            // Write properties as sub-elements
            foreach (XmlElements element in Enum.GetValues(typeof(XmlElements)))
            {
                string value = null;
                switch (element)
                {
                    case XmlElements.SeasonHeader:
                        value = this.SeasonHeader;
                        break;
                    case XmlElements.EpisodeHeader:
                        value = this.EpisodeHeader;
                        break;
                    case XmlElements.HeaderPerEpisode:
                        value = this.HeaderPerEpisode.ToString();
                        break;
                    case XmlElements.SeasonFirst:
                        value = this.SeasonFirst.ToString();
                        break;
                    case XmlElements.ForceEpisodeDoubleDigits:
                        value = this.ForceEpisodeDoubleDigits.ToString();
                        break;
                    case XmlElements.ForceSeasonDoubleDigits:
                        value = this.ForceSeasonDoubleDigits.ToString();
                        break;
                    default:
                        throw new Exception("Unkonw element!");
                }

                if (value != null)
                    xw.WriteElementString(element.ToString(), value);
            }
        }

        /// <summary>
        /// Loads instance properties from XML.
        /// </summary>
        /// <param name="itemNode">Node to load XML from</param>
        /// <returns>true if sucessfully loaded from XML</returns>
        public bool Load(XmlNode fileNameNode)
        {
            // Loop through sub-nodes
            foreach (XmlNode propNode in fileNameNode.ChildNodes)
            {
                // Get element/property type
                XmlElements element;
                if (!Enum.TryParse<XmlElements>(propNode.Name, out element))
                    continue;

                // Get value string
                string value = propNode.InnerText;

                // Load value into appropriate property
                switch (element)
                {
                    case XmlElements.SeasonHeader:
                        if (!string.IsNullOrEmpty(value))
                            this.SeasonHeader = value;
                        break;
                    case XmlElements.EpisodeHeader:
                        if (!string.IsNullOrEmpty(value))
                            this.EpisodeHeader = value;
                        break;
                    case XmlElements.HeaderPerEpisode:
                        bool headerPerEpisode;
                        if (bool.TryParse(value, out headerPerEpisode))
                            this.HeaderPerEpisode = headerPerEpisode;
                        break;
                    case XmlElements.SeasonFirst:
                        bool seasonFirst;
                        if (bool.TryParse(value, out seasonFirst))
                            this.SeasonFirst = seasonFirst;
                        break;
                    case XmlElements.ForceEpisodeDoubleDigits:
                        bool forceEpDouble;
                        if (bool.TryParse(value, out forceEpDouble))
                            this.ForceEpisodeDoubleDigits = forceEpDouble;
                        break;
                    case XmlElements.ForceSeasonDoubleDigits:
                        bool forceSeasonDouble;
                        if (bool.TryParse(value, out forceSeasonDouble))
                            this.ForceSeasonDoubleDigits = forceSeasonDouble;
                        break;
                }
            }

            // Success
            return true;
        }

        #endregion
    }
}
