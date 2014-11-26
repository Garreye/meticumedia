using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Meticumedia.Classes
{
    public class ContentRootFolderMatchRule : INotifyPropertyChanged
    {
        #region Events

        /// <summary>
        /// INotifyPropertyChanged interface event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Triggers PropertyChanged event.
        /// </summary>
        /// <param name="name">Name of the property that has changed value</param>
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Property of content rule applies to
        /// </summary>
        public ContentRootFolderMatchRuleProperty Property
        {
            get
            {
                return property;
            }
            set
            {
                property = value;
                OnPropertyChanged("Type");
            }
        }
        private ContentRootFolderMatchRuleProperty property = ContentRootFolderMatchRuleProperty.Name;

        /// <summary>
        /// Rule Type
        /// </summary>
        public ContentRootFolderMatchRuleType Type
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
        private ContentRootFolderMatchRuleType type = ContentRootFolderMatchRuleType.Equals;

        /// <summary>
        /// Value for rule to match property/rule type to.
        /// </summary>
        public string Value
        {
            get
            {
                return ruleValue;
            }
            set
            {
                ruleValue = value;
                OnPropertyChanged("Value");
            }
        }
        private string ruleValue = string.Empty;

        /// <summary>
        /// Root folder path to move content to if rule matches.
        /// </summary>
        public string Folder
        {
            get
            {
                return folder;
            }
            set
            {
                folder = value;
                OnPropertyChanged("Folder");
            }
        }
        private string folder;

        #endregion

        #region Constructor

        public ContentRootFolderMatchRule()
        {
        }

        public ContentRootFolderMatchRule(ContentRootFolderMatchRule other)
        {
            this.Property = other.Property;
            this.Type = other.Type;
            this.Value = other.Value;
            this.Folder = other.Folder;
        }

        #endregion

        #region Methods

        public bool Match(Content content)
        {
            string prop;
            switch (this.Property)
            {
                case ContentRootFolderMatchRuleProperty.Name:
                    prop = content.DisplayName;
                    break;
                case ContentRootFolderMatchRuleProperty.Genre:
                    if (content.DisplayGenres.Count == 0)
                        return false;
                    prop = content.DisplayGenres[0];
                    break;
                case ContentRootFolderMatchRuleProperty.Year:
                    prop = content.DisplayYear.ToString();
                    break;
                default:
                    throw new Exception("Unknown match rule property.");
            }

            string propLower = prop.ToLower();
            string valueLower = this.Value.ToLower();

            switch (this.Type)
            {
                case ContentRootFolderMatchRuleType.Equals:
                    return propLower == valueLower;
                case ContentRootFolderMatchRuleType.Contains:
                    return propLower.Contains(valueLower);
                case ContentRootFolderMatchRuleType.StartsWith:
                    return propLower.StartsWith(valueLower);
                case ContentRootFolderMatchRuleType.EndsWith:
                    return propLower.EndsWith(valueLower);
                case ContentRootFolderMatchRuleType.RegularExpression:
                    Regex re = new Regex(this.Value, RegexOptions.IgnoreCase);
                    return re.IsMatch(prop);
                case ContentRootFolderMatchRuleType.Between:
                    Regex yearsRegex = new Regex(@"^(\d{4}).*(\d{4})$");
                    Match yearMatch = yearsRegex.Match(this.Value);
                    if (yearMatch.Success)
                    {
                        int year1 = 0, year2 = 0;
                        int.TryParse(yearMatch.Groups[1].Value, out year1);
                        int.TryParse(yearMatch.Groups[1].Value, out year2);

                        int minYear = Math.Min(year1, year2);
                        int maxYear = Math.Min(year1, year2);

                        return content.DisplayYear >= minYear && content.DisplayYear <= maxYear;
                    }
                    return false;
                default:
                    throw new Exception("Unknown match rule criteria.");
            }
        }

        #endregion

        #region XML

        /// <summary>
        /// Root XML element string for this class.
        /// </summary>
        private static readonly string ROOT_XML = "ContentRoot";

        /// <summary>
        /// Element names for properties that need to be saved to XML
        /// </summary>
        private enum XmlElements
        {
            Property, Type, Value, Folder
        };

        /// <summary>
        /// Writes properties into XML elements
        /// </summary>
        /// <param name="xw">Writer for accessing XML file</param>
        public void Save(XmlWriter xw)
        {
            // Start instace element
            xw.WriteStartElement(ROOT_XML);
            
            // Loop through elements/properties
            foreach (XmlElements element in Enum.GetValues(typeof(XmlElements)))
            {
                // Set value from appropriate property
                string value = null;
                switch (element)
                {
                    case XmlElements.Property:
                        value = this.Property.ToString();
                        break;
                    case XmlElements.Type:
                        value = this.Type.ToString();
                        break;
                    case XmlElements.Value:
                        value = this.Value;
                        break;
                    case XmlElements.Folder:
                        value = this.Folder;
                        break;
                    default:
#if DEBUG
                        throw new Exception("Unknown XML element");
#endif
                        break;
                }
                // If value is valid then write it
                if (value != null)
                    xw.WriteElementString(element.ToString(), value);
            }

            xw.WriteEndElement();
        }

        /// <summary>
        /// Loads properties from XML elements
        /// </summary>
        /// <param name="ruleNode">The XML node containt property elements</param>
        public void Load(XmlNode ruleNode)
        {
            // Go through elements of node
            foreach (XmlNode propNode in ruleNode.ChildNodes)
            {
                // Match the current node to a known element name
                XmlElements element;
                if (!Enum.TryParse<XmlElements>(propNode.Name, out element))
                    continue;

                // Set appropiate property from value in element
                string value = propNode.InnerText;
                switch (element)
                {
                    case XmlElements.Property:
                        ContentRootFolderMatchRuleProperty prop;
                        if (Enum.TryParse<ContentRootFolderMatchRuleProperty>(value, out prop))
                            this.Property = prop;
                        break;
                    case XmlElements.Type:
                        ContentRootFolderMatchRuleType ruleType;
                        if (Enum.TryParse<ContentRootFolderMatchRuleType>(value, out ruleType))
                            this.Type = ruleType;
                        break;
                    case XmlElements.Value:
                        this.Value = value;
                        break;
                    case XmlElements.Folder:
                        this.Folder = value;
                        break;
                    default:
#if DEBUG
                        throw new Exception("Unknown XML element");
#endif
                        break;
                }
            }
        }

        #endregion
    }
}
