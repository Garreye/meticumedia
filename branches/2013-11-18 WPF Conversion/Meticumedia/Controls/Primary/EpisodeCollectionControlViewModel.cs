using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Meticumedia.Classes;
using Meticumedia.WPF;

namespace Meticumedia.Controls
{
    public class EpisodeCollectionControlViewModel : OrgItemQueueableViewModel
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
                OnPropertyChanged(this, "DisplayOverview");
                OnPropertyChanged(this, "ShowInfoFontSize");
            }
        }
        private bool displayOverview = false;

        public int ShowInfoFontSize
        {
            get
            {
                return this.DisplayOverview ? 12 : 18;
            }
        }

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

        public IList SelectedEpisodes
        {
            get
            {
                return selectedEpisodes;
            }
            set
            {
                selectedEpisodes = value;
                OnPropertyChanged(this, "SelectedEpisodes");
                OnPropertyChanged(this, "SinglePlayableItemSelectionVisibility");
                OnPropertyChanged(this, "SingleItemSelectionVisibility");
                OnPropertyChanged(this, "IgnorableSelectionVisibility");
                OnPropertyChanged(this, "UnignorableSelectionVisibility");
                OnPropertyChanged(this, "DeletableSelectionVisibility");                
            }
        }
        private IList selectedEpisodes;

        public Visibility SingleItemSelectionVisibility
        {
            get
            {
                return this.SelectedEpisodes != null && this.SelectedEpisodes.Count == 1 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility SinglePlayableItemSelectionVisibility
        {
            get
            {
                return this.SelectedEpisodes != null && this.SelectedEpisodes.Count == 1 && ((TvEpisode)this.SelectedEpisodes[0]).Missing != MissingStatus.Missing ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility IgnorableSelectionVisibility
        {
            get
            {
                if (this.SelectedEpisodes == null)
                    return Visibility.Collapsed;
                foreach (TvEpisode ep in this.SelectedEpisodes)
                    if (!ep.Ignored)
                        return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        public Visibility UnignorableSelectionVisibility
        {
            get
            {
                if (this.SelectedEpisodes == null)
                    return Visibility.Collapsed;
                foreach (TvEpisode ep in this.SelectedEpisodes)
                    if (ep.Ignored)
                        return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        public Visibility DeletableSelectionVisibility
        {
            get
            {
                if (this.SelectedEpisodes == null)
                    return Visibility.Collapsed;
                foreach (TvEpisode ep in this.SelectedEpisodes)
                    if (ep.Missing != MissingStatus.Missing)
                        return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        #endregion

        #region Commands

        private ICommand playCommand;
        public ICommand PlayCommand
        {
            get
            {
                if (playCommand == null)
                {
                    playCommand = new RelayCommand(
                        param => this.PlayEpisode()
                    );
                }
                return playCommand;
            }
        }

        private ICommand copyEpisodeInfoToClipboardCommand;
        public ICommand CopyEpisodeInfoToClipboardCommand
        {
            get
            {
                if (copyEpisodeInfoToClipboardCommand == null)
                {
                    copyEpisodeInfoToClipboardCommand = new RelayCommand(
                        param => this.CopyEpisodeInfoToClipboard()
                    );
                }
                return copyEpisodeInfoToClipboardCommand;
            }
        }

        private ICommand editCommand;
        public ICommand EditCommand
        {
            get
            {
                if (editCommand == null)
                {
                    editCommand = new RelayCommand(
                        param => this.EditEpisode()
                    );
                }
                return editCommand;
            }
        }

        private ICommand locateAndCopyEpisodeCommand;
        public ICommand LocateAndCopyEpisodeCommand
        {
            get
            {
                if (locateAndCopyEpisodeCommand == null)
                {
                    locateAndCopyEpisodeCommand = new RelayCommand(
                        param => this.LocateEpisode(true)
                    );
                }
                return locateAndCopyEpisodeCommand;
            }
        }

        private ICommand locateAndMoveEpisodeCommand;
        public ICommand LocateAndMoveEpisodeCommand
        {
            get
            {
                if (locateAndMoveEpisodeCommand == null)
                {
                    locateAndMoveEpisodeCommand = new RelayCommand(
                        param => this.LocateEpisode(false)
                    );
                }
                return locateAndMoveEpisodeCommand;
            }
        }

        private ICommand ignoreEpisodeCommand;
        public ICommand IgnoreEpisodeCommand
        {
            get
            {
                if (ignoreEpisodeCommand == null)
                {
                    ignoreEpisodeCommand = new RelayCommand(
                        param => this.SetEpisodeIgnore(true)
                    );
                }
                return ignoreEpisodeCommand;
            }
        }

        private ICommand unignoreEpisodeCommand;
        public ICommand UnignoreEpisodeCommand
        {
            get
            {
                if (unignoreEpisodeCommand == null)
                {
                    unignoreEpisodeCommand = new RelayCommand(
                        param => this.SetEpisodeIgnore(false)
                    );
                }
                return unignoreEpisodeCommand;
            }
        }

        private ICommand deleteEpisodeFilesCommand;
        public ICommand DeleteEpisodeFilesCommand
        {
            get
            {
                if (deleteEpisodeFilesCommand == null)
                {
                    deleteEpisodeFilesCommand = new RelayCommand(
                        param => this.DeleteEpisodeFiles()
                    );
                }
                return deleteEpisodeFilesCommand;
            }
        }

        private ICommand addEpisodeCommand;
        public ICommand AddEpisodeCommand
        {
            get
            {
                if (addEpisodeCommand == null)
                {
                    addEpisodeCommand = new RelayCommand(
                        param => this.AddNewEpisode()
                    );
                }
                return addEpisodeCommand;
            }
        }

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

            this.EpisodesCollectionView = CollectionViewSource.GetDefaultView(episodes);
            this.EpisodesCollectionView.Filter = new Predicate<object>(FilterEpisode);

            // TODO: this make loading the control slow as fuck
            EnableLiveFiltering();

            this.Groupings = new ObservableCollection<TvGroupingType>();
            this.Groupings.Add(TvGroupingType.None);
            if (show == null)
            {
                this.Groupings.Add(TvGroupingType.Show);
                this.DisplayOverview = false;
            }
            else
            {
                this.Groupings.Add(TvGroupingType.Season);
                this.DisplayOverview = true;
            }
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

        private void EnableLiveFiltering()
        {
            // Set properties to trigger live updating
            ICollectionViewLiveShaping liveCollection = this.EpisodesCollectionView as ICollectionViewLiveShaping;
            liveCollection.LiveFilteringProperties.Add("Season");
            liveCollection.LiveFilteringProperties.Add("Name");
            liveCollection.LiveFilteringProperties.Add("Status");
            liveCollection.IsLiveFiltering = true;
            liveCollection.LiveGroupingProperties.Add("Missing");
            liveCollection.LiveGroupingProperties.Add("Season");
            liveCollection.LiveGroupingProperties.Add("Show");
            liveCollection.IsLiveGrouping = true;
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

        private void PlayEpisode()
        {
            foreach (TvEpisode ep in this.SelectedEpisodes)
                ep.PlayEpisodeFile();
        }

        private void CopyEpisodeInfoToClipboard()
        {
            Clipboard.SetText(((TvEpisode)this.SelectedEpisodes[0]).BuildEpString());
        }

        private void EditEpisode()
        {
            MessageBox.Show("Implement editor!");
        }

        private void LocateEpisode(bool copy)
        {
            List<OrgItem> items;
            if (((TvEpisode)this.SelectedEpisodes[0]).UserLocate(true, copy, out items))
                OnItemsToQueue(items);
        }

        private void SetEpisodeIgnore(bool ignore)
        {
            foreach (TvEpisode ep in this.SelectedEpisodes)
                ep.Ignored = ignore;
        }

        private void DeleteEpisodeFiles()
        {
            if (MessageBox.Show("Are you sure you want to delete the files for selected episode? This operation cannot be undone", "Sure?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                List<OrgItem> items = new List<OrgItem>();
                foreach (TvEpisode ep in this.SelectedEpisodes)
                    if (System.IO.File.Exists(ep.File.FilePath))
                        items.Add(new OrgItem(OrgAction.Delete, ep.File.FilePath, FileCategory.Trash, null));
                OnItemsToQueue(items);
            }
        }

        private void AddNewEpisode()
        {
            MessageBox.Show("Adding requires someone to implement editor!");
        }

        #endregion
    }
}
