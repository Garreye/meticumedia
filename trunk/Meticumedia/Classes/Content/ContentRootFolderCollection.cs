using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meticumedia.Classes
{
    public class ContentRootFolderCollection : ObservableCollection<ContentRootFolder>, INotifyPropertyChanged
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
        /// Selection for how root folder to move content to is selected
        /// </summary>
        public ContentRootFolderSelectionType Selection
        {
            get
            {
                return selection;
            }
            set
            {
                selection = value;
                OnPropertyChanged("Selection");
            }
        }
        private ContentRootFolderSelectionType selection = ContentRootFolderSelectionType.Default;

        
        /// <summary>
        /// Controls what to do when genre match is not found. Only applicable if Selection set to GenreChild
        /// </summary>
        public ContentRootFolderGenreMatchMissType GenreMatchMiss
        {
            get
            {
                return genreMatchMiss;
            }
            set
            {
                genreMatchMiss = value;
                OnPropertyChanged("GenreMatchMiss");
            }
        }
        private ContentRootFolderGenreMatchMissType genreMatchMiss = ContentRootFolderGenreMatchMissType.AutoCreate;

        /// <summary>
        /// Rules for matching content to root folder. Only applicable if Selection set to Rules
        /// </summary>
        public ObservableCollection<ContentRootFolderMatchRule> MatchRules
        {
            get
            {
                return matchRules;
            }
            set
            {
                matchRules = value;
                OnPropertyChanged("MatchRules");
            }
        }
        private ObservableCollection<ContentRootFolderMatchRule> matchRules = new ObservableCollection<ContentRootFolderMatchRule>();

        #endregion

        #region Methods



        #endregion
    }
}
