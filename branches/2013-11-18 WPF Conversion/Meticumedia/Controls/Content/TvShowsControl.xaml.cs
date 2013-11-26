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
using System.Collections.ObjectModel;
using Meticumedia.Classes;
using System.ComponentModel;

namespace Meticumedia.Controls
{
    /// <summary>
    /// Interaction logic for ContentControl.xaml
    /// </summary>
    public partial class ContentControl : UserControl
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ContentControl()
        {
            InitializeComponent();
            
            Organization.Shows.LoadComplete += new EventHandler(Shows_LoadComplete);

        }

        #endregion

        #region Variables

        /// <summary>
        /// Item source for shows listbox
        /// </summary>
        private ObservableCollection<TvShow> shows;


        /// <summary>
        /// Item source for episode listbox
        /// </summary>
        private ObservableCollection<TvEpisode> episodes = new ObservableCollection<TvEpisode>();

        #endregion

        #region Event Handlers

        /// <summary>
        /// Loading complete of shows load them into shows listbox
        /// </summary>
        private void Shows_LoadComplete(object sender, EventArgs e)
        {
            shows = new ObservableCollection<TvShow>();
            lock (Organization.Shows.ContentLock)
            {
                foreach (Content show in Organization.Shows)
                    shows.Add((TvShow)show);
            }
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                lbShows.ItemsSource = shows;
            });
            Organization.Shows.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Shows_CollectionChanged);
        }

        /// <summary>
        /// Changes to shows collection are reflected in item source for shows listbox
        /// </summary>
        private void Shows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                if (e.OldItems != null)
                    foreach (Content remItem in e.OldItems)
                        shows.Remove((TvShow)remItem);
                if (e.NewItems != null)
                    foreach (Content addItem in e.NewItems)
                        shows.Add((TvShow)addItem);
            });
        }

        /// <summary>
        /// Selected show has it's episodes loaded
        /// </summary>
        private void lbShows_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbShows.SelectedItem != null)
            {
                episodes.Clear();
                foreach (TvEpisode ep in ((TvShow)lbShows.SelectedItem).Episodes)
                    episodes.Add(ep);
                lbEpisodes.ItemsSource = episodes;

                ICollectionView view = CollectionViewSource.GetDefaultView(episodes);
                view.GroupDescriptions.Clear();
                view.GroupDescriptions.Add(new PropertyGroupDescription("SeasonName"));
            }

        }

        #endregion
    }
}
