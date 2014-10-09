﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Meticumedia.Classes;
using Meticumedia.Controls;
using Meticumedia.WPF;

namespace Meticumedia.Windows
{
    public class OrgItemEditorWindowViewModel : ViewModel
    {
        #region Events

        public event EventHandler ResultsSet;

        private void OnResultsSet()
        {
            if (ResultsSet != null)
                ResultsSet(this, new EventArgs());
        }

        #endregion
        
        #region Properties

        /// <summary>
        /// Orignal item before editing started
        /// </summary>
        public OrgItem OriginalItem
        {
            get
            {
                return originalItem;
            }
            set
            {
                originalItem = value;
                OnPropertyChanged(this, "OriginalItem");
            }
        }
        private OrgItem originalItem;

        /// <summary>
        /// Item being edited
        /// </summary>
        public OrgItem Item
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
                OnPropertyChanged(this, "Item");
            }
        }
        private OrgItem item;

        /// <summary>
        /// Modification made set to OK by user
        /// </summary>
        public bool ResultsOk { get; private set; }

        /// <summary>
        /// View model for movie editor
        /// </summary>
        public ContentEditorControlViewModel MovieViewModel
        {
            get;
            set;

        }
        private ContentEditorControlViewModel movieViewModel;

        /// <summary>
        /// List of TV shows available for selecting
        /// </summary>
        public ObservableCollection<TvShow> Shows { get; set; }

        public int SeasonNumber
        {
            get
            {
                return this.Item.TvEpisode.Season;
            }
            set
            {
                UpdateEpisodes(value, this.EpisodeNumber);
                OnPropertyChanged(this, "SeasonNumber");
            }
        }

        public int EpisodeNumber
        {
            get
            {
                return this.Item.TvEpisode.DatabaseNumber;
            }
            set
            {
                UpdateEpisodes(this.SeasonNumber, value);
                OnPropertyChanged(this, "EpisodeNumber");
            }
        }

        private void UpdateEpisodes(int season, int episode)
        {
            // First episode
            TvEpisode firstEp;
            if (this.Item.TvEpisode.Show.FindEpisode(season, episode, true, out firstEp))
                this.Item.TvEpisode.Clone(firstEp);
            else
            {
                this.Item.TvEpisode = new TvEpisode(this.Item.TvEpisode.Show);
                this.Item.TvEpisode.Season = season;
                this.Item.TvEpisode.DatabaseNumber = episode;
            }

            // Second episode
            if (this.Item.MultiEpisode)
            {
                if (this.Item.TvEpisode.Season != season || this.Item.TvEpisode2.DatabaseNumber != episode + 1)
                {
                    //this.Item.TvEpisode2.DatabaseNumber = episode + 1;
                    TvEpisode secondEp;
                    if (this.Item.TvEpisode.Show.FindEpisode(season, episode + 1, true, out secondEp))
                        this.Item.TvEpisode2.Clone(secondEp);
                    else
                    {
                        this.Item.TvEpisode2 = new TvEpisode(this.Item.TvEpisode2.Show);
                        this.Item.TvEpisode2.Season = season;
                        this.Item.TvEpisode2.DatabaseNumber = episode + 1;
                    }
                }
            }
            else if (this.Item.TvEpisode2.DatabaseNumber > 0)
                this.Item.TvEpisode2.DatabaseNumber = -1;


            // Update destination
            this.Item.DestinationPath = this.Item.BuildDestination();
        }

        #endregion

        #region Commands

        private ICommand okCommand;
        public ICommand OkCommand
        {
            get
            {
                if (okCommand == null)
                {
                    okCommand = new RelayCommand(
                        param => this.OkResults(),
                        param => this.CanDoOkCommand()
                    );
                }
                return okCommand;
            }
        }

        private bool CanDoOkCommand()
        {
            return true;
        }


        private ICommand cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                {
                    cancelCommand = new RelayCommand(
                        param => this.CancelResults()
                    );
                }
                return cancelCommand;
            }
        }

        #endregion

        #region Constructor

        public OrgItemEditorWindowViewModel(OrgItem item)
        {
            this.OriginalItem = item;
            this.Item = new OrgItem(item);
            this.Item.PropertyChanged += Item_PropertyChanged;

            // Set path for movie to item source path (for filling in search box)
            if (string.IsNullOrEmpty(this.Item.Movie.Path))
                this.Item.Movie.Path = System.IO.Path.GetFileNameWithoutExtension(item.SourcePath);

            // Create view model for movie content editor
            this.MovieViewModel = new ContentEditorControlViewModel(this.Item.Movie, true);
            this.MovieViewModel.Content.PropertyChanged += ItemSubProperty_PropertyChanged;

            // Setup avilable shows
            this.Shows = new ObservableCollection<TvShow>();
            foreach (TvShow show in Organization.Shows)
                this.Shows.Add(show);
            if (!string.IsNullOrEmpty(this.Item.TvEpisode.Show.DatabaseName) && this.Shows.Count > 0)
                this.Item.TvEpisode.Show = this.Shows[0];
            if (!this.Shows.Contains(this.Item.TvEpisode.Show))
                this.Shows.Add(this.Item.TvEpisode.Show);
        }

        void ItemSubProperty_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Item.DestinationPath = this.Item.BuildDestination();
        }

        void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Action" && this.Item.Action == OrgAction.Delete)
                this.Item.Category = FileCategory.Trash;

            if (e.PropertyName == "MultiEpisode")
                UpdateEpisodes(this.SeasonNumber, this.EpisodeNumber);

            if (!e.PropertyName.Contains("DestinationPath"))
                this.Item.DestinationPath = this.Item.BuildDestination();
        }

        #endregion

        #region Methods

        public void OkResults()
        {
            this.ResultsOk = true;
            OnResultsSet();
        }

        public void CancelResults()
        {
            OnResultsSet();
        }

        #endregion
    }
}