using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Meticumedia.Classes;
using Meticumedia.WPF;

namespace Meticumedia.Controls
{
    public class ScheduleControlViewModel : ViewModel
    {
        #region Properties

        public int Days
        {
            get
            {
                return days;
            }
            set
            {
                days = value;
                UpdateResults();
                OnPropertyChanged(this, "Days");
            }
        }
        private int days = 14;

        public TvScheduleType ScheduleType
        {
            get
            {
                return scheduleType;
            }
            set
            {
                scheduleType = value;
                UpdateResults();
                OnPropertyChanged(this, "ScheduleType");
            }
        }
        private TvScheduleType scheduleType = TvScheduleType.Recent;

        public ObservableCollection<TvShow> Shows { get; set; }

        public TvShow SelectedShow
        {
            get
            {
                return selectedShow;
            }
            set
            {
                selectedShow = value;
                UpdateResults();
                OnPropertyChanged(this, "SelectedShow");
            }
        }
        private TvShow selectedShow;

        public ObservableCollection<TvEpisode> Results { get; set; }

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

        public ScheduleControlViewModel()
        {
            this.Shows = new ObservableCollection<TvShow>();
            this.Results = new ObservableCollection<TvEpisode>();
            this.EpisodesModel = new EpisodeCollectionControlViewModel(this.Results);
            Organization.Shows.LoadComplete += Shows_LoadComplete;
        }

        #endregion

        #region Methods

        private void Shows_LoadComplete(object sender, EventArgs e)
        {
            UpdateShowsSafe();
            UpdateResultsSafe();
            Organization.Shows.CollectionChanged += Shows_CollectionChanged;
        }

        private void Shows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateShowsSafe();
            UpdateResultsSafe();
        }

        /// <summary>
        /// Thread safe results update.
        /// </summary>
        private void UpdateResultsSafe()
        {
            if (App.Current.Dispatcher.CheckAccess())
                UpdateResults();
            else
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    UpdateResults();
                });
        }

        /// <summary>
        /// Updates and display resulting TV episodes
        /// </summary>
        private void UpdateResults()
        {
            // Get shows to check from combo box
            List<TvShow> shows;
            if (this.SelectedShow == null || this.SelectedShow == TvShow.AllShows)
                shows = this.Shows.ToList();
            else
                shows = new List<TvShow>() { this.SelectedShow };

            // Get results
            List<TvEpisode> eps = BuildEpisodeList(shows, this.Days, this.ScheduleType == TvScheduleType.Upcoming);
            this.Results.Clear();
            foreach (TvEpisode ep in eps)
                this.Results.Add(ep);
        }

        /// <summary>
        /// Thread safe shows combobox updata
        /// </summary>
        private void UpdateShowsSafe()
        {
            if (App.Current.Dispatcher.CheckAccess())
                UpdateShows();
            else
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    UpdateShows();
                });
        }

        /// <summary>
        /// Updates shows contained in show selection combobox
        /// </summary>
        public void UpdateShows()
        {
            // Save current selection
            TvShow currSelection = this.SelectedShow;

            // Rebuilt combobox items
            this.Shows.Clear();
            this.Shows.Add(TvShow.AllShows);
            for (int i = 0; i < Organization.Shows.Count; i++)
            {
                // Add valid items
                if (!string.IsNullOrEmpty(Organization.Shows[i].DatabaseName) && ((TvShow)Organization.Shows[i]).IncludeInSchedule && Organization.Shows[i].Id != 0)
                    this.Shows.Add((TvShow)Organization.Shows[i]);
            }

            // Reset selection
            if (currSelection != null && this.Shows.Contains(currSelection))
                this.SelectedShow = currSelection;
            else
                this.SelectedShow = this.Shows[0];
        }


        /// <summary>
        /// Gets list of episodes matching user input for scheduling.
        /// </summary>
        /// <param name="shows">List of shows to look for episodes from</param>
        /// <param name="days">Number of days to look for episodes in</param>
        /// <param name="upcoming">Whether to check for upcoming or recent episodes</param>
        /// <returns>List of episodes</returns>
        private List<TvEpisode> BuildEpisodeList(List<TvShow> shows, int days, bool upcoming)
        {
            // Initialize episode list
            List<TvEpisode> epList = new List<TvEpisode>();

            // Check every shows for episode that match schedule criteria
            for (int i = 0; i < shows.Count; i++)
                if (shows[i].IncludeInSchedule)
                {
                    foreach (TvEpisode episode in shows[i].Episodes)
                    {
                        TimeSpan timeDiff = episode.DisplayAirDate.Subtract(DateTime.Now);
                        if (!episode.Ignored && Math.Abs(timeDiff.TotalDays) < days)
                        {
                            if (upcoming && timeDiff.Days >= 0)
                                epList.Add(episode);
                            else if (!upcoming && timeDiff.Days < 0)
                                epList.Add(episode);
                        }
                    }
                }

            // Sort (reverse to get latest at top) and return episodes  
            epList.Sort(TvEpisode.CompareByAirDate);
            if (!upcoming)
                epList.Reverse();
            return epList;
        }

        #endregion
    }
}
