﻿using System;
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
using System.Collections.ObjectModel;
using Meticumedia.Classes;
using System.ComponentModel;

namespace Meticumedia.Controls
{
    /// <summary>
    /// Interaction logic for ContentControl.xaml
    /// </summary>
    public partial class TvShowsControl : UserControl
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TvShowsControl()
        {
            InitializeComponent();
            gbSelShow.Visibility = System.Windows.Visibility.Hidden;
        }

        #endregion

        #region Variables

        /// <summary>
        /// Item source for episode listbox
        /// </summary>
        private ObservableCollection<TvEpisode> episodes = new ObservableCollection<TvEpisode>();

        #endregion

        #region Event Handlers

        private void cntrlShows_SelectionChanged(object sender, ContentControl.SelectionChangedArgs e)
        {
            if (e.Selections.Count > 0)
            {
                TvShow show = (TvShow)e.Selections[0];
                
                episodes.Clear();
                foreach (TvEpisode ep in show.Episodes)
                    episodes.Add(ep);
                lbEpisodes.ItemsSource = episodes;

                ICollectionView view = CollectionViewSource.GetDefaultView(episodes);
                view.GroupDescriptions.Clear();
                view.GroupDescriptions.Add(new PropertyGroupDescription("SeasonName"));

                ctntSelShow.DataContext = show;

                gbSelShow.Visibility = System.Windows.Visibility.Visible;

                // Buil episode filters
                cmbEpFilter.Items.Clear();
                List<TvEpisodeFilter> epFilters = TvEpisodeFilter.BuildFilters(show, chkDisplayIgnoredEps.IsChecked.Value, true);
                foreach (TvEpisodeFilter filter in epFilters)
                    cmbEpFilter.Items.Add(filter);
                cmbEpFilter.SelectedIndex = 0;
            }
            else
                gbSelShow.Visibility = System.Windows.Visibility.Hidden;
        }


        /// <summary>
        /// Overview width is limited by control it is in
        /// </summary>
        private void spOverview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            tbOverview.MaxWidth = ctntSelShow.ActualWidth - 20;
        }

        #endregion
    }
}
