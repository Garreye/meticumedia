// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Meticumedia
{
    /// <summary>
    /// File linked to a TV episode.
    /// </summary>
    public class TvFile
    {
        #region Properties

        /// <summary>
        /// Path of the file
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// Which part of the file the episode tied to this object belongs to
        /// </summary>
        public int Part { get; private set; }

        /// <summary>
        /// Indicates whether the file contains multiple episode
        /// </summary>
        public bool MultiPart { get; private set; }

        #endregion

        #region Contructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public TvFile()
        {
            this.FilePath = string.Empty;
            this.Part = 1;
            this.MultiPart = false;
        }


        /// <summary>
        /// Constructor for cloning instance.
        /// </summary>
        /// <param name="file">Instance to clone</param>
        public TvFile(TvFile file)
        {
            this.FilePath = file.FilePath;
            this.Part = file.Part;
            this.MultiPart = this.MultiPart;
        }

        /// <summary>
        /// Constructor for single episode file
        /// </summary>
        /// <param name="filePath">path of the file</param>
        public TvFile(string filePath)
        {
            this.FilePath = filePath;
            this.Part = 1;
            this.MultiPart = false;
        }

        /// <summary>
        /// Constructor for multi episode file
        /// </summary>
        /// <param name="filePath">path of the file</param>
        /// <param name="part">part of the file the episode tied to this object belongs to</param>
        public TvFile(string filePath, int part)
        {
            this.FilePath = filePath;
            this.Part = part;
            this.MultiPart = true;
        }

        #endregion

        #region XML

        /// <summary>
        /// Element names for properties that need to be saved to XML.
        /// </summary>
        private enum XmlElements { FilePath, Part, MultiPart };

        /// <summary>
        /// Saves instance to XML file.
        /// </summary>
        /// <param name="xw">Writer for accessing XML file</param>
        public void Save(XmlWriter xw)
        {
            foreach (XmlElements element in Enum.GetValues(typeof(XmlElements)))
            {
                string value;
                switch (element)
                {
                    case XmlElements.FilePath:
                        value = this.FilePath;
                        break;
                    case XmlElements.Part:
                        value = this.Part.ToString();
                        break;
                    case XmlElements.MultiPart:
                        value = this.MultiPart.ToString();
                        break;
                    default:
                        throw new Exception("Unkonw element!");
                }

                xw.WriteElementString(element.ToString(), value);
            }
        }

        /// <summary>
        /// Loads instance from XML.
        /// </summary>
        /// <param name="itemNode">Node to load XML from</param>
        /// <returns>true if sucessfully loaded from XML</returns>
        public void Load(XmlNode fileNode)
        {
            // Loop through sub-nodes
            foreach (XmlNode propNode in fileNode.ChildNodes)
            {
                // Get element/property type
                XmlElements element = XmlElements.FilePath;
                bool exists = false;
                foreach (XmlElements el in Enum.GetValues(typeof(XmlElements)))
                    if (el.ToString() == propNode.Name)
                    {
                        element = el;
                        exists = true;
                        break;
                    }

                // If unknown element go to next
                if (!exists)
                    continue;

                // Get value string
                string value = propNode.InnerText;

                // Load value into appropriate property
                switch (element)
                {
                    case XmlElements.FilePath:
                        this.FilePath = value;
                        break;
                    case XmlElements.Part:
                        int part;
                        int.TryParse(value, out part);
                        this.Part = part;
                        break;
                    case XmlElements.MultiPart:
                        bool multiPart;
                        bool.TryParse(value, out multiPart);
                        this.MultiPart = multiPart;
                        break;
                }
            }
        }

        #endregion
    }
}
