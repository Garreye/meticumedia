using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Meticumedia.Classes;

namespace Meticumedia.Controls
{
    /// <summary>
    /// Interaction logic for ScheduleControl.xaml
    /// </summary>
    public partial class ScheduleControl : UserControl
    {
        #region Constructor

        public ScheduleControl()
        {
            InitializeComponent();

            numDays.ValueChanged += numDays_ValueChanged;

            // Build episode filters
            cmbFilter.Items.Clear();
            List<TvEpisodeFilter> epFilters = TvEpisodeFilter.BuildFilters(null, false, false);
            foreach (TvEpisodeFilter filter in epFilters)
                cmbFilter.Items.Add(filter);
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectionChanged += cmbFilter_SelectionChanged;
            
            Organization.Shows.LoadComplete += Shows_LoadComplete;
        }

        #endregion

        #region Event Handling

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

        private void cmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateResultsSafe();
        }

        private void cmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateResultsSafe();
        }

        private void numDays_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateResultsSafe();
        }

        #endregion

        #region Updating

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
            string currSelection = string.Empty;
            if(cmbShows.SelectedItem != null)
                currSelection = cmbShows.SelectedItem.ToString();

            // Rebuilt combobox items
            cmbShows.Items.Clear();
            cmbShows.Items.Add("All Shows");
            bool selected = false;
                for(int i=0;i<Organization.Shows.Count;i++)
                {
                    // Add item
                    if (!string.IsNullOrEmpty(Organization.Shows[i].Name) && Organization.Shows[i].Id != 0)
                        cmbShows.Items.Add(Organization.Shows[i].Name);

                    // Check if item should be selected
                    if (!string.IsNullOrWhiteSpace(currSelection) && Organization.Shows[i].Name == currSelection)
                    {
                        cmbShows.SelectedIndex = cmbShows.Items.Count - 1;
                        selected = true;
                    }
                }

            // Set selection to first item if previous was not reselected
            if (!selected)
                cmbShows.SelectedIndex = 0;
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
            if (cmbType.SelectedItem == null || numDays == null || numDays.Value == null)
                return;
            
            // Get shows to check from combo box
            List<Content> shows;
            if (cmbShows.SelectedIndex < 1)
                shows = Organization.Shows;
            else
            {
                shows = new List<Content>();
                Content show = Organization.Shows[cmbShows.SelectedIndex - 1];
                //show.UpdateMissing();
                shows.Add(show);
            }

            // Get filter
            TvEpisodeFilter epFilter = (TvEpisodeFilter)cmbFilter.SelectedItem;
            if (epFilter == null)
                epFilter = new TvEpisodeFilter(TvEpisodeFilter.FilterType.All, 0);
            
            // Get results
            results = BuildEpisodeList(shows, (int)numDays.Value, ((TvScheduleType)cmbType.SelectedItem) == TvScheduleType.Upcoming, epFilter);

            // Put results in list
            lbEpisodes.ItemsSource = results;
        }

        /// <summary>
        /// Result currently displayed in listview
        /// </summary>
        List<TvEpisode> results;

        /// <summary>
        /// Gets list of episodes matching user input for scheduling.
        /// </summary>
        /// <param name="shows">List of shows to look for episodes from</param>
        /// <param name="days">Number of days to look for episodes in</param>
        /// <param name="upcoming">Whether to check for upcoming or recent episodes</param>
        /// <returns>List of episodes</returns>
        private List<TvEpisode> BuildEpisodeList(List<Content> shows, int days, bool upcoming, TvEpisodeFilter epFilter)
        {
            // Initialize episode list
            List<TvEpisode> epList = new List<TvEpisode>();

            // Check every shows for episode that match schedule criteria
            for(int i=0;i<shows.Count;i++)
                if (((TvShow)shows[i]).IncludeInSchedule)
                {
                    foreach (TvEpisode episode in ((TvShow)shows[i]).Episodes)
                    {
                        TimeSpan timeDiff = episode.AirDate.Subtract(DateTime.Now);
                        if (!episode.Ignored && epFilter.FilterEpisode(episode) && Math.Abs(timeDiff.Days) < days)
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
