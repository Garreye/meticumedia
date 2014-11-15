// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ComponentModel;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Class representing portion of a file name - multiple instances used to specify how a file name show be built
    /// </summary>
    public class FileNamePortion : INotifyPropertyChanged
    {

        #region Properties

        /// <summary>
        /// The word type contained in the portion
        /// </summary>
        public FileWordType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }

        private FileWordType type = FileWordType.None;

        /// <summary>
        /// String to put in front of portion
        /// </summary>
        public string Header
        {
            get
            {
                return header;
            }
            set
            {
                header = value;
                OnPropertyChanged("Header");
            }
        }

        private string header = string.Empty;

        /// <summary>
        /// String to put at end of portion
        /// </summary>
        public string Footer
        {
            get
            {
                return footer;
            }
            set
            {
                footer = value;
                OnPropertyChanged("Footer");
            }
        }

        private string footer = string.Empty;

        /// <summary>
        /// Option for portion.
        /// </summary>
        public CaseOptionType CaseOption
        {
            get
            {
                return caseOption;
            }
            set
            {
                caseOption = value;
                OnPropertyChanged("CaseOption");
            }
        }

        private CaseOptionType caseOption = CaseOptionType.None;

        /// <summary>
        /// Replacement for whitespace in portion.
        /// </summary>
        public string Whitespace
        {
            get
            {
                return whitespace;
            }
            set
            {
                whitespace = value;
                OnPropertyChanged("Whitespace");
            }
        }

        private string whitespace = " ";

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public FileNamePortion()
        {
            this.Type = FileWordType.None;
            this.Header = string.Empty;
            this.Footer = " ";
            this.CaseOption = CaseOptionType.None;
            this.Whitespace = " ";
        }

        /// <summary>
        /// Constructor for cloning instance.
        /// </summary>
        /// <param name="portion">Instance to clone properties from</param>
        public FileNamePortion(FileNamePortion portion)
        {
            this.Type = portion.Type;
            this.Header = portion.Header;
            this.Footer = portion.Footer;
            this.CaseOption = portion.CaseOption;
            this.Whitespace = portion.Whitespace;
        }

        /// <summary>
        /// Constructor specifying all properties
        /// </summary>
        /// <param name="type">The word type contained in the portion</param>
        /// <param name="value">The value for the custom string</param>
        /// <param name="header">string to be placed before portion</header>
        /// <param name="footer">string to be placed after portion</header>
        /// <param name="option">Option to be applied to case of portion</param>
        public FileNamePortion(FileWordType type, string header, string footer, CaseOptionType caseOption, string whitespace)
        {
            this.Type = type;
            this.Header = header;
            this.Footer = footer;
            this.CaseOption = caseOption;
            this.Whitespace = whitespace;
        }

        /// <summary>
        /// Constructor specifying type and containers (header/footer) - no value assigned (can be used for all types except UserString)
        /// </summary>
        /// <param name="type">The word type contained in the portion</param>
        /// <param name="header">string to be placed before portion</param>
        /// <param name="footer">string to be placed after portion</param>
        /// <param name="caseOption">Option to be applied to case of portion</param>
        public FileNamePortion(FileWordType type, string header, string footer, CaseOptionType caseOption)
        {
            this.Type = type;
            this.Header = header;
            this.Footer = footer;
            this.CaseOption = CaseOptionType.None;
            this.Whitespace = " ";
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
        private enum XmlElements { Type, Container, Header, Footer, CaseOption, Whitespace };

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
                    case XmlElements.Container: // deprecated, left in for loading existing formats
                        break;
                    case XmlElements.Header:
                        value = this.Header.Replace(" ", "%space%");
                        break;
                    case XmlElements.Footer:
                        value = this.Footer.Replace(" ", "%space%");
                        break;
                    case XmlElements.CaseOption:
                        value = this.CaseOption.ToString();
                        break;
                    case XmlElements.Whitespace:
                        value = this.Whitespace.Replace(" ", "%space%");
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
                    case XmlElements.Container: // deprecated, converts from container type to header/footer values
                        //None, Whitespace, Underscores, Dashes, Brackets, SquareBrackets, SquigglyBrackets, Period, Custom
                        this.Header = string.Empty;
                        this.Footer = string.Empty;
                        switch (value)
                        {
                            case "None":
                                break;
                            case "Whitespace":
                                this.Footer = " ";
                                break;
                            case "Underscores":
                                this.Header = "_";
                                break;
                            case "Dashes":
                                this.Header = " - ";
                                break;
                            case "Brackets":
                                this.Header = "(";
                                this.Footer = ")";
                                break;
                            case "SquareBrackets":
                                this.Header = "[";
                                this.Footer = "]";
                                break;
                            case "SquigglyBrackets":
                                this.Header = "{";
                                this.Footer = "}";
                                break;
                            case "Period":
                                this.Header = ".";
                                break;
                        }
                        break;
                    case XmlElements.Header:
                        this.Header = value.Replace("%space%", " ");
                        break;
                    case XmlElements.Footer:
                        this.Footer = value.Replace("%space%", " ");
                        break;
                    case XmlElements.CaseOption:
                        CaseOptionType caseOption;
                        if (Enum.TryParse<CaseOptionType>(value, out caseOption))
                            this.CaseOption = caseOption;
                        break;
                    case XmlElements.Whitespace:
                        this.Whitespace = value.Replace("%space%", " ");
                        break;
                }
            }

            // Success
            return true;
        }

        #endregion
    }
}
