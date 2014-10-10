using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Meticumedia.Classes;
using Meticumedia.WPF;

namespace Meticumedia.Controls
{
    public class EpisodeCollectionControlViewModel : ViewModel
    {
        #region Properties

        public ObservableCollection<TvEpisode> Episodes { get; set; }

        /// <summary>
        /// Episode filters
        /// </summary>
        public ObservableCollection<TvEpisodeFilter> EpisodeFilters { get; set; }

        public TvEpisodeFilter SelectedEpisodeFilter
        {
            get
            {
                return selectedEpisodeFilter;
            }
            set
            {
                selectedEpisodeFilter = value;
                if (this.EpisodesCollectionView != null)
                    this.EpisodesCollectionView.Refresh();
                OnPropertyChanged(this, "SelectedEpisodeFilter");
            }
        }
        private TvEpisodeFilter selectedEpisodeFilter;

        /// <summary>
        /// View collection for contents
        /// </summary>
        public ICollectionView EpisodesCollectionView { get; set; }

        public Visibility DisplayShowName
        {
            get
            {
                return this.SelectedGrouping == TvGroupingType.Show ? Visibility.Collapsed : displayShowName;
            }
            set
            {
                displayShowName = value;
                OnPropertyChanged(this, "DisplayShowName");
            }
        }
        private Visibility displayShowName = Visibility.Visible;

        public Visibility DisplaySeasonNumber
        {
            get
            {
                return this.SelectedGrouping == TvGroupingType.Season ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public bool DisplayOverview
        {
            get
            {
                return displayOverview;
            }
            set
            {
                displayOverview = value;
                if (this.EpisodesCollectionView != null)
                    this.EpisodesCollectionView.Refresh();
                OnPropertyChanged(this, "DisplayOverview");
            }
        }
        private bool displayOverview = false;

        public ObservableCollection<TvGroupingType> Groupings { get; set; }

        public TvGroupingType SelectedGrouping
        {
            get
            {
                return selectedGrouping;
            }
            set
            {
                selectedGrouping = value;
                UpdateGroupingSafe();
                OnPropertyChanged(this, "SelectedGrouping");
                OnPropertyChanged(this, "DisplaySeasonNumber");
                OnPropertyChanged(this, "DisplayShowName");
            }
        }
        private TvGroupingType selectedGrouping = TvGroupingType.None;

        #endregion

        #region Constructor

        public EpisodeCollectionControlViewModel(ObservableCollection<TvEpisode> episodes, TvShow show = null )
        {
            List<TvEpisodeFilter> filters = TvEpisodeFilter.BuildFilters(show, show != null);
            this.EpisodeFilters = new ObservableCollection<TvEpisodeFilter>();
            foreach (TvEpisodeFilter filter in filters)
                this.EpisodeFilters.Add(filter);

            if (this.EpisodeFilters.Count > 0)
                this.SelectedEpisodeFilter = this.EpisodeFilters[0];

            CollectionViewSource episodesViewSource = new CollectionViewSource() { Source = episodes };
            this.EpisodesCollectionView = episodesViewSource.View;
            this.EpisodesCollectionView.Filter = new Predicate<object>(FilterEpisode);

            // TODO: this make loading the control slow as fuck
            // Set properties to trigger live updating
            //ICollectionViewLiveShaping liveCollection = this.EpisodesCollectionView as ICollectionViewLiveShaping;
            //liveCollection.LiveFilteringProperties.Add("Genres");
            //liveCollection.LiveFilteringProperties.Add("Date");
            //liveCollection.LiveFilteringProperties.Add("RootFolder");
            //liveCollection.LiveFilteringProperties.Add("Name");
            //liveCollection.IsLiveFiltering = true;
            //liveCollection.LiveGroupingProperties.Add("Missing");
            //liveCollection.LiveGroupingProperties.Add("Season");
            //liveCollection.LiveGroupingProperties.Add("Show");
            //liveCollection.IsLiveGrouping = true;

            this.Groupings = new ObservableCollection<TvGroupingType>();
            this.Groupings.Add(TvGroupingType.None);
            if (show == null)
                this.Groupings.Add(TvGroupingType.Show);
            else
                this.Groupings.Add(TvGroupingType.Season);
            this.Groupings.Add(TvGroupingType.Status);

            if (show == null)
            {
                this.SelectedGrouping = TvGroupingType.Show;
                this.DisplayShowName = Visibility.Visible;
            }
            else
            {
                this.SelectedGrouping = TvGroupingType.Season;
                this.DisplayShowName = Visibility.Collapsed;
            }
        }

        #endregion

        #region Methods

        private bool FilterEpisode(object obj)
        {
            TvEpisode ep = obj as TvEpisode;

            if (this.SelectedEpisodeFilter != null)
            {
                if (!this.SelectedEpisodeFilter.FilterEpisode(ep))
                    return false;
            }

            return true;
        }

        private void UpdateGroupingSafe()
        {
            if (App.Current.Dispatcher.CheckAccess())
                this.UpdateGrouping();
            else
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    this.UpdateGrouping();
                });
        }

        private void UpdateGrouping()
        {
            this.EpisodesCollectionView.GroupDescriptions.Clear();
            switch (this.SelectedGrouping)
            {
                case TvGroupingType.None:
                    break;
                case TvGroupingType.Show:
                    this.EpisodesCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Show"));
                    break;
                case TvGroupingType.Season:
                    this.EpisodesCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("SeasonName"));
                    break;
                case TvGroupingType.Status:
                    this.EpisodesCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Missing"));
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}
