using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Meticumedia.Classes;
using Meticumedia.Windows;
using Meticumedia.WPF;

namespace Meticumedia.Controls
{
    public class ContentControlViewModel : ViewModel
    {
        #region Events

        /// <summary>
        /// Event indicating there are items to be sent to the queue
        /// </summary>
        public static event EventHandler<ItemsToQueueArgs> ItemsToQueue;

        /// <summary>
        /// Triggers ItemsToQueue event
        /// </summary>
        /// <param name="items"></param>
        protected static void OnItemsToQueue(List<OrgItem> items)
        {
            if (ItemsToQueue != null)
                ItemsToQueue(null, new ItemsToQueueArgs(items));
        }

        #endregion
        
        #region Properties

        public Content Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
                OnPropertyChanged(this, "Content");
            }
        }
        private Content content;

        public EpisodeCollectionControlViewModel EpisodesModel
        {
            get
            {
                return episodesModel;
            }
            set
            {
                episodesModel = value;
                OnPropertyChanged(this, "EpisodesModel");
            }
        }
        private EpisodeCollectionControlViewModel episodesModel;

        #endregion

        #region Commands

        private ICommand editCommand;
        public ICommand EditCommand
        {
            get
            {
                if (editCommand == null)
                {
                    editCommand = new RelayCommand(
                        param => this.EditContent(),
                        param => this.CanDoEditCommand()
                    );
                }
                return editCommand;
            }
        }

        private bool CanDoEditCommand()
        {
            return true;
        }

        #endregion

        #region Constructor

        public ContentControlViewModel(Content content)
        {
            this.Content = content;

            if (Content is TvShow)
            {
                TvShow show = this.Content as TvShow;
                this.EpisodesModel = new EpisodeCollectionControlViewModel(show.Episodes, show);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Open content editor window
        /// </summary>
        private void EditContent()
        {
            ContentEditorWindow cew = new ContentEditorWindow(this.Content);
            cew.ShowDialog();

            if (cew.Results != null)
            {
                if (this.Content is Movie)
                {
                    (this.Content as Movie).Clone(cew.Results as Movie, false);
                    Organization.Movies.Save();
                }
                else
                {
                    (this.Content as TvShow).Clone(cew.Results as TvShow, false);
                    Organization.Shows.Save();
                }
            }

        }

        #endregion

        #region Episode Filtering

        #endregion
    }
}
