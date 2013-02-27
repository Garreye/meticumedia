using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Meticumedia
{
    public class GuiSettings
    {
        #region Properties

        /// <summary>
        /// Whether auto clear completed check box is checked
        /// </summary>
        public bool AutoClearCompleted { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Defautl constructor
        /// </summary>
        public GuiSettings()
        {
            this.AutoClearCompleted = false;
        }

        #endregion

        #region XML

        /// <summary>
        /// Element names for properties that need to be saved to XML.
        /// </summary>
        private enum XmlElements { AutoClearCompleted };

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
                    case XmlElements.AutoClearCompleted:
                        value = this.AutoClearCompleted.ToString();
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
                XmlElements element; ;
                if (!Enum.TryParse<XmlElements>(propNode.Name, out element))
                    continue;

                // Get value string
                string value = propNode.InnerText;

                // Load value into appropriate property
                switch (element)
                {
                    case XmlElements.AutoClearCompleted:
                        bool autoClear;
                        bool.TryParse(value, out autoClear);
                        this.AutoClearCompleted = autoClear;
                        break;
                }
            }

            // Success
            return true;
        }

        #endregion
    }
}
