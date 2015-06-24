// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;

namespace Meticumedia.Classes
{
    public class GuiSettings : INotifyPropertyChanged
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
        /// Whether auto clear completed check box is checked
        /// </summary>
        public bool AutoClearCompleted
        {
            get
            {
                return autoClearCompleted;
            }
            set
            {
                autoClearCompleted = value;
                OnPropertyChanged("AutoClearCompleted");
            }
        }
        private bool autoClearCompleted;

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
