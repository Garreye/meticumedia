using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Meticumedia.Classes;
using Meticumedia.Controls;
using Meticumedia.WPF;
using Ookii.Dialogs.Wpf;

namespace Meticumedia.Windows
{
    public class OrgItemEditorWindowViewModel : OkCancelWindowViewModel
    {    
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

        public bool SourceEditingAllowed
        {
            get
            {
                return sourceEditingAllowed;
            }
            set
            {
                sourceEditingAllowed = value;
                OnPropertyChanged(this, "SourceEditingAllowed");
            }
        }
        private bool sourceEditingAllowed;

        /// <summary>
        /// View model for movie editor
        /// </summary>
        public ContentEditorControlViewModel MovieViewModel
        {
            get
            {
                return movieViewModel;
            }
            set
            {
                movieViewModel = value;
                OnPropertyChanged(this, "MovieViewModel");
            }
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
            this.Item.BuildDestination();
        }

        #endregion

        #region Commands

        private ICommand setSourcePathCommand;
        public ICommand SetSourcePathCommand
        {
            get
            {
                if (setSourcePathCommand == null)
                {
                    setSourcePathCommand = new RelayCommand(
                        param => this.SetSourcePath()
                    );
                }
                return setSourcePathCommand;
            }
        }

        private ICommand setDestinationPathCommand;
        public ICommand SetDestinationPathCommand
        {
            get
            {
                if (setDestinationPathCommand == null)
                {
                    setDestinationPathCommand = new RelayCommand(
                        param => this.SetDestinationPath()
                    );
                }
                return setDestinationPathCommand;
            }
        }

        private ICommand newTvShowCommand;
        public ICommand NewTvShowCommand
        {
            get
            {
                if (newTvShowCommand == null)
                {
                    newTvShowCommand = new RelayCommand(
                        param => this.NewTvShow()
                    );
                }
                return newTvShowCommand;
            }
        }

        #endregion

        #region Constructor

        public OrgItemEditorWindowViewModel(OrgItem item)
        {
            this.OriginalItem = item;
            this.Item = new OrgItem(item);
            this.Item.PropertyChanged += Item_PropertyChanged;
            this.SourceEditingAllowed = string.IsNullOrEmpty(this.Item.SourcePath);

            // Set path for movie to item source path (for filling in search box)
            if (string.IsNullOrEmpty(this.Item.Movie.Path))
                this.Item.Movie.Path = System.IO.Path.GetFileNameWithoutExtension(item.SourcePath);

            // Create view model for movie content editor
            this.MovieViewModel = new ContentEditorControlViewModel(this.Item.Movie, true);
            this.MovieViewModel.Content.PropertyChanged += ItemSubProperty_PropertyChanged;
            ContentRootFolder movieFolder;
            if (string.IsNullOrEmpty(this.Item.Movie.RootFolder) && Settings.GetDefaultMovieFolder(out movieFolder))
                this.Item.Movie.RootFolder = movieFolder.FullPath;

            // tv number update
            int season, episode1, episode2;
            if (this.Item.TvEpisode.DatabaseNumber == -1 && FileHelper.GetEpisodeInfo(item.SourcePath, string.Empty, out season, out episode1, out episode2))
            {
                this.Item.TvEpisode.Season = season;
                this.Item.TvEpisode.DatabaseNumber = episode1;
                this.Item.TvEpisode2.Season = season;
                this.Item.TvEpisode2.DatabaseNumber = episode2;
            }

            // Setup avilable shows
            this.Shows = new ObservableCollection<TvShow>();
            foreach (TvShow show in Organization.Shows)
                this.Shows.Add(show);
            if (string.IsNullOrEmpty(this.Item.TvEpisode.Show.DatabaseName) && this.Shows.Count > 0)
                this.Item.TvEpisode.Show = this.Shows[0];
            if (!string.IsNullOrEmpty(this.Item.TvEpisode.Show.DatabaseName) && !this.Shows.Contains(this.Item.TvEpisode.Show))
                this.Shows.Add(this.Item.TvEpisode.Show);
            this.Item.TvEpisode.PropertyChanged += ItemSubProperty_PropertyChanged;
        }

        private string currentShow = string.Empty;

        void ItemSubProperty_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.Item.BuildDestination();

            if (e.PropertyName == "Show" && (sender as TvEpisode).Show.DatabaseName != currentShow)
            {
                currentShow = (sender as TvEpisode).Show.DatabaseName;
                UpdateEpisodes(this.SeasonNumber, this.EpisodeNumber);
            }
        }

        void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Action" && this.Item.Action == OrgAction.Delete)
                this.Item.Category = FileCategory.Trash;

            if (e.PropertyName == "MultiEpisode")
                UpdateEpisodes(this.SeasonNumber, this.EpisodeNumber);

            if (!e.PropertyName.Contains("DestinationPath"))
                this.Item.BuildDestination();
        }

        #endregion

        #region Methods

        private void SetSourcePath()
        {
            VistaSaveFileDialog ofd = new VistaSaveFileDialog();
            ofd.Filter = "All Files|*.*";
            ofd.FileName = this.Item.SourcePath;
            if (ofd.ShowDialog() == false)
                return;

            this.Item.SourcePath = ofd.FileName;
        }

        private void SetDestinationPath()
        {
            VistaSaveFileDialog ofd = new VistaSaveFileDialog();
            ofd.Filter = "All Files|*.*";
            ofd.FileName = this.Item.DestinationPath;
            if (ofd.ShowDialog() == false)
                return;

            this.Item.DestinationPath = ofd.FileName;
        }

        private void NewTvShow()
        {
            ContentEditorWindow cew = new ContentEditorWindow(new TvShow());
            cew.ShowDialog();

            if (cew.Results != null)
            {
                TvShow show = cew.Results as TvShow;
                show.UpdateInfoFromDatabase();
                ContentRootFolder folder;
                Settings.GetDefaultTvFolder(out folder);
                show.RootFolder = folder.FullPath;
                show.Path = show.BuildFolderPath();
                this.Shows.Add(show);
                this.Item.TvEpisode.Show = show; 
            }
            
        }

        #endregion
    }
}
