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
    /// Class representing portion of a file name - multiple instances used to specify how a file name show be built
    /// </summary>
    public class FileNamePortion
    {
        #region Constants/Enums

        /// <summary>
        /// Container for the portion of the file name - how it is separated from the rest of the name
        /// </summary>
        public enum ContainerTypes { Whitespace, Underscores, Dashes, Brackets, SquareBrackets, SquigglyBrackets }

        #endregion

        #region Properties

        /// <summary>
        /// The word type contained in the portion
        /// </summary>
        public FileWordType Type { get; set; }

        /// <summary>
        /// The string value of the portion - for user strings
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// How portion is to be contained/separated from rest of file name
        /// </summary>
        public ContainerTypes Container { get; set; }

        #endregion

        #region Constructors

        public FileNamePortion()
        {
            this.Type = FileWordType.None;
            this.Value = string.Empty;
            this.Container = ContainerTypes.Whitespace;
        }

        public FileNamePortion(FileNamePortion portion)
        {
            this.Type = portion.Type;
            this.Value = portion.Value;
            this.Container = portion.Container;
        }

        /// <summary>
        /// Constructor specifying all properties
        /// </summary>
        /// <param name="type">The word type contained in the portion</param>
        /// <param name="value">The string value of the portion</param>
        /// <param name="container">How portion is to be contained/separated from rest of file name</param>
        public FileNamePortion(FileWordType type, string value, ContainerTypes container)
        {
            this.Type = type;
            this.Value = value;
            this.Container = container;
        }

        /// <summary>
        /// Constructor specifying type and value - default container used (whitespace)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public FileNamePortion(FileWordType type, string value)
        {
            this.Type = type;
            this.Value = value;
            this.Container = ContainerTypes.Whitespace;
        }

        /// <summary>
        /// Constructor specifying type and contained - no value assigned (can be used for all types except UserString)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="container"></param>
        public FileNamePortion(FileWordType type, ContainerTypes container)
        {
            this.Type = type;
            this.Value = string.Empty;
            this.Container = container;
        }

        #endregion

        #region XML

        /// <summary>
        /// Root XML element for saving instance to file.
        /// </summary>
        private static readonly string ROOT_XML = "FileNamePortion";

        /// <summary>
        /// Element names for properties that need to be saved to XML.
        /// </summary>
        private enum XmlElements { Type, Value, Container };

        /// <summary>
        /// Saves instance properties to XML file.
        /// </summary>
        /// <param name="xw">Writer for accessing XML file</param>
        public void Save(XmlWriter xw)
        {
            // Start portion element
            xw.WriteStartElement(ROOT_XML);

            // Write properties as sub-elements
            foreach (XmlElements element in Enum.GetValues(typeof(XmlElements)))
            {
                string value = null;
                switch (element)
                {
                    case XmlElements.Type:
                        value = this.Type.ToString();
                        break;
                    case XmlElements.Value:
                        value = this.Value;
                        break;
                    case XmlElements.Container:
                        value = this.Container.ToString();
                        break;
                    default:
                        throw new Exception("Unkonw element!");
                }

                if (value != null)
                    xw.WriteElementString(element.ToString(), value);
            }

            // End element
            xw.WriteEndElement();
        }

        /// <summary>
        /// Loads instance properties from XML.
        /// </summary>
        /// <param name="itemNode">Node to load XML from</param>
        /// <returns>true if sucessfully loaded from XML</returns>
        public bool Load(XmlNode fileNameNode)
        {
            // Checks that node is valid type
            if (fileNameNode.Name != ROOT_XML)
                return false;
            
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
                    case XmlElements.Type:
                        FileWordType type;
                        if (Enum.TryParse<FileWordType>(value, out type))
                            this.Type = type;
                        break;
                    case XmlElements.Value:
                        if (!string.IsNullOrEmpty(value))
                            this.Value = value;
                        break;
                    case XmlElements.Container:
                        ContainerTypes types;
                        if (Enum.TryParse<ContainerTypes>(value, out types))
                            this.Container = types;
                        break;
                }
            }

            // Success
            return true;
        }

        #endregion
    }
}
