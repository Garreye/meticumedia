using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using Meticumedia.Classes;

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

        #region Episode Filtering

        #endregion
    }
}
