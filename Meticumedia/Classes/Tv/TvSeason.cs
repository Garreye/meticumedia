// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Meticumedia
{
    /// <summary>
    /// A season of a TV show. Contains all the episodes for the season.
    /// </summary>
    public class TvSeason
    {
        #region Properties

        /// <summary>
        /// The season's number
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets whether the season it ignored based on whether
        /// all its episodes are ignored
        /// </summary>
        public bool Ignored { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for season with known number.
        /// </summary>
        /// <param name="number"></param>
        public TvSeason(int number) : this()
        {
            this.Number = number;
            this.Ignored = false;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TvSeason()
        {
            this.Number = -1;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Override ToString method for object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Number == 0)
                return "Specials";
            else
                return "Season " + this.Number;
        }

        #endregion

        #region XML

        /// <summary>
        /// Element names for properties that need to be saved to XML.
        /// </summary>
        private enum XmlElements { Number, Episodes };

        /// <summary>
        /// Root XML element for saving instance to file.
        /// </summary>
        private static readonly string ROOT_XML = "TvSeason";

        /// <summary>
        /// Saves instance to XML file.
        /// </summary>
        /// <param name="xw">Writer for accessing XML file</param>
        public void Save(XmlWriter xw)
        {
            // Start season element
            xw.WriteStartElement(ROOT_XML);

            foreach (XmlElements element in Enum.GetValues(typeof(XmlElements)))
            {
                string value = null;
                switch (element)
                {
                    case XmlElements.Number:
                        value = this.Number.ToString();
                        break;
                    //case XmlElements.Episodes:
                    //    xw.WriteStartElement(element.ToString());
                    //    foreach (TvEpisode episode in this.Episodes)
                    //        episode.Save(xw);
                    //    xw.WriteEndElement();
                    //    break;
                    default:
                        throw new Exception("Unkonw element!");
                }

                if (value != null)
                    xw.WriteElementString(element.ToString(), value);
            }

            xw.WriteEndElement();
        }

        /// <summary>
        /// Loads instance from XML.
        /// </summary>
        /// <param name="itemNode">Node to load XML from</param>
        /// <returns>true if sucessfully loaded from XML</returns>
        public bool Load(XmlNode seasonNode)
        {
            if (seasonNode.Name != ROOT_XML)
                return false;
            
            foreach (XmlNode propNode in seasonNode.ChildNodes)
            {
                XmlElements element = XmlElements.Number;
                if (!Enum.TryParse<XmlElements>(propNode.Name, out element))
                    continue;

                string value = propNode.InnerText;
                switch (element)
                {
                    case XmlElements.Number:
                        int number;
                        if (int.TryParse(value, out number))
                            this.Number = number;
                        break;
                    case XmlElements.Episodes:
                        this.Episodes = new List<TvEpisode>();
                        foreach(XmlNode epNode in propNode.ChildNodes)
                        {
                            TvEpisode episode = new TvEpisode();
                            episode.Load(epNode);
                            Episodes.Add(episode);
                        }
                        Episodes.Sort();
                        break;
                }
            }

            return true;
        }

        #endregion
    }
}
