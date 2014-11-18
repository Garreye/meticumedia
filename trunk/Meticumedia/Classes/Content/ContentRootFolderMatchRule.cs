using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Root folder to move content to if rule matches.
        /// </summary>
        public ContentRootFolder Folder
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
        private ContentRootFolder folder;

        #endregion

        #region Constructor

        public ContentRootFolderMatchRule()
        {
        }

        #endregion

        #region Methods

        public bool Match(Content content)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
