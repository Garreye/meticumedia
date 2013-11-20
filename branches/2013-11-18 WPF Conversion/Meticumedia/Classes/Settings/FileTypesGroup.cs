using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Meticumedia.Classes
{
    public class FileTypesGroup : INotifyPropertyChanged
    {
        #region Events

        public new event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        public ObservableCollection<string> Types
        {
            get
            {
                return types;
            }
            set
            {
                types = value;
                OnPropertyChanged("Types");
            }
        }

        private ObservableCollection<string> types = new ObservableCollection<string>();

        public string NextItem
        {
            get
            {
                return nextItem;
            }
            set
            {
                nextItem = value;
                OnPropertyChanged("NextItem");
            }
        }

        private string nextItem = string.Empty;

        public FileTypesGroup() : base()
        {
        }
    }
}
