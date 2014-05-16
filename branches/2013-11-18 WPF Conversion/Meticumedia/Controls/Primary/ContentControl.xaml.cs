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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Meticumedia.Controls
{
    /// <summary>
    /// Interaction logic for ContentControl.xaml
    /// </summary>
    public partial class ContentControl : UserControl
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

        public ContentType ContentType { get; set; }

        #endregion

        #region Variables

        /// <summary>
        /// Item source for root folde combo box
        /// </summary>
        private ObservableCollection<ContentRootFolder> folders = new ObservableCollection<ContentRootFolder>();


        /// <summary>
        /// Item source for shows listbox
        /// 
        /// </summary>
        private ObservableCollection<Content> contents;

        /// <summary>
        /// Item source for episode listbox
        /// </summary>
        private ObservableCollection<TvEpisode> episodes = new ObservableCollection<TvEpisode>();

        #endregion

        #region Constructor

        public ContentControl()
        {
            this.Loaded += new RoutedEventHandler(ContentControl_Loaded);
            Settings.SettingsModified += new EventHandler(Settings_SettingsModified);
            InitializeComponent();
            lbContent.Items.Filter = ContentFilter;
            gbSelItem.Visibility = System.Windows.Visibility.Hidden;
        }

        #endregion

        #region Event Handlers

        private void ContentControl_Loaded(object sender, RoutedEventArgs e)
        {
            switch (this.ContentType)
            {
                case ContentType.Movie:
                    Organization.Movies.LoadProgressChange += new EventHandler<System.ComponentModel.ProgressChangedEventArgs>(Content_LoadProgressChange);
                    Organization.Movies.LoadComplete += new EventHandler(Content_LoadComplete);
                    gbEpisodes.Visibility = System.Windows.Visibility.Collapsed;
                    break;
                case ContentType.TvShow:
                    Organization.Shows.LoadProgressChange += new EventHandler<System.ComponentModel.ProgressChangedEventArgs>(Content_LoadProgressChange);
                    Organization.Shows.LoadComplete += new EventHandler(Content_LoadComplete);
                    break;
            }
            ContentRootFolder.UpdateProgressChange += new EventHandler<OrgProgressChangesEventArgs>(ContentRootFolder_UpdateProgressChange);
        }

        /// <summary>
        /// When settings are modified the folders are updated
        /// </summary>
        private void Settings_SettingsModified(object sender, EventArgs e)
        {
            int selId = -1;
            if (cmbRootFolder.SelectedItem != null)
                selId = ((ContentRootFolder)cmbRootFolder.SelectedItem).Id;

            folders.Clear();
            folders.Add(new ContentRootFolder(this.ContentType, "All Folders", "All Folders"));
            switch (this.ContentType)
            {
                case ContentType.TvShow:
                    foreach (ContentRootFolder folder in Settings.TvFolders)
                        folders.Add(new ContentRootFolder(folder));
                    break;
                case ContentType.Movie:
                    foreach (ContentRootFolder folder in Settings.MovieFolders)
                        folders.Add(new ContentRootFolder(folder));
                    break;
            }

            // Set combo source
            if (cmbRootFolder.ItemsSource == null)
                cmbRootFolder.ItemsSource = folders;

            // Set selection
            if (selId >= 0)
                foreach (ContentRootFolder folder in folders)
                    if (folder.Id == selId)
                    {
                        cmbRootFolder.SelectedItem = folder;
                        break;
                    }
            if (cmbRootFolder.SelectedIndex == -1)
                cmbRootFolder.SelectedIndex = 0;
        }

        /// <summary>
        /// Loading complete of shows load them into shows listbox
        /// </summary>
        private void Content_LoadComplete(object sender, EventArgs e)
        {
            ContentCollection contentCollection = (ContentCollection)sender;
            if (contentCollection.ContentType != this.ContentType)
                return;
            
            contents = new ObservableCollection<Content>();
            lock (contentCollection.ContentLock)
            {
                foreach (Content cntnt in contentCollection)
                    contents.Add(cntnt);
            }
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                lbContent.ItemsSource = contents;
                UpdateGenresComboBox();
            });
            contentCollection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Contents_CollectionChanged);

            UpdateProgress(100, "Loading complete!", true);
        }

        /// <summary>
        /// Changes to shows collection are reflected in item source for shows listbox
        /// </summary>
        private void Contents_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                if (e.OldItems != null)
                    foreach (Content remItem in e.OldItems)
                        contents.Remove(remItem);
                if (e.NewItems != null)
                    foreach (Content addItem in e.NewItems)
                        contents.Add(addItem);
            });
        }

        /// <summary>
        /// Year numericUpDowns enabled when year filter checked
        /// </summary>
        private void chkYear_Click(object sender, RoutedEventArgs e)
        {
            numYearStart.IsEnabled = chkYear.IsChecked.Value;
            numYearStop.IsEnabled = chkYear.IsChecked.Value;
        }

        /// <summary>
        /// Folder view filter needs updating when root changed
        /// </summary>
        private void cmbRootFolder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateRootFilters();
            UpdateGenresComboBox();
        }

        /// <summary>
        /// 
        /// </summary>
        private void cmbRootFolderView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateGenresComboBox();
        }

        private void cmbGenre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lbContent.Items.Filter = ContentFilter;
        }

        private void lbShows_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Content> selections = new List<Content>();
            foreach (object obj in lbContent.SelectedItems)
                selections.Add((Content)obj);

            if (selections.Count > 0)
            {
                switch (this.ContentType)
                {
                    case Classes.ContentType.TvShow:
                        TvShow show = (TvShow)selections[0];

                        episodes.Clear();
                        foreach (TvEpisode ep in show.Episodes)
                            episodes.Add(ep);
                        lbEpisodes.ItemsSource = episodes;

                        ICollectionView view = CollectionViewSource.GetDefaultView(episodes);
                        view.GroupDescriptions.Clear();
                        view.GroupDescriptions.Add(new PropertyGroupDescription("SeasonName"));
                        ctntSelItem.DataContext = show;                        

                        // Buil episode filters
                        cmbEpFilter.Items.Clear();
                        List<TvEpisodeFilter> epFilters = TvEpisodeFilter.BuildFilters(show, chkDisplayIgnoredEps.IsChecked.Value, true);
                        foreach (TvEpisodeFilter filter in epFilters)
                            cmbEpFilter.Items.Add(filter);
                        cmbEpFilter.SelectedIndex = 0;
                        break;
                    case Classes.ContentType.Movie:
                        Movie movie = (Movie)selections[0];
                        ctntSelItem.DataContext = movie;
                        break;
                }
                gbSelItem.Visibility = System.Windows.Visibility.Visible;
            }
            else
                gbSelItem.Visibility = System.Windows.Visibility.Hidden;


        }
        
        private void txtNameFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            lbContent.Items.Filter = ContentFilter;
        }

        /// <summary>
        /// Overview width is limited by control it is in
        /// </summary>
        private void spOverview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            tbOverview.MaxWidth = ctntSelItem.ActualWidth - 20;
        }

        /// <summary>
        /// Load progress shown in bar
        /// </summary>
        private void Content_LoadProgressChange(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            UpdateProgress(e.ProgressPercentage, "Loading " + this.ContentType.Description() + "s " + (string)e.UserState, e.ProgressPercentage >= 100);
        }

        private void ContentRootFolder_UpdateProgressChange(object sender, OrgProgressChangesEventArgs e)
        {
            ContentRootFolder folder = sender as ContentRootFolder;
            if (folder.ContentType == this.ContentType)
            {
                UpdateProgress(e.ProgressPercentage, (string)e.UserState, e.ProgressPercentage >= 100);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates progress bar percent and message
        /// </summary>
        /// <param name="percent"></param>
        /// <param name="msg"></param>
        private void UpdateProgress(int percent, string msg, bool hide)
        {
            this.Dispatcher.Invoke((Action)delegate
            {
                tbPbText.Text = msg;
                pbUpdating.Value = percent;

                if (hide)
                    grdProgress.Visibility = System.Windows.Visibility.Collapsed;
                else
                    grdProgress.Visibility = System.Windows.Visibility.Visible;

            });
        }

        /// <summary>
        /// Update Genres in combo box based on what genres are actually in the content displayed
        /// </summary>
        private void UpdateGenresComboBox()
        {
            // Save current selection
            string selectedGenre = string.Empty;
            if (cmbGenre.SelectedItem != null)
                selectedGenre = cmbGenre.SelectedItem.ToString();

            // Get selected folder
            ContentRootFolder selFolder = null;
            if (cmbRootFolder.SelectedItem != null)
                selFolder = (ContentRootFolder)cmbRootFolder.SelectedItem;
            else
                return;

            // Add all available genres
            GenreCollection genres = null;
            switch (this.ContentType)
            {
                case ContentType.Movie:
                    if (selFolder.FullPath == "All")
                        genres = Organization.AllMovieGenres;
                    else
                        genres = selFolder.GetGenres();
                    break;
                case ContentType.TvShow:
                    if (selFolder.FullPath == "All")
                        genres = Organization.AllTvGenres;
                    else
                        genres = selFolder.GetGenres();
                    break;
                default:
                    throw new Exception("Unknown content type");
            }

            // Add genres to combo box
            cmbGenre.Items.Clear();
            cmbGenre.Items.Add("All Genres");
            lock (genres.AccessLock)
                foreach (string genre in genres)
                {
                    int item = cmbGenre.Items.Add(genre);

                    // Reselect genre
                    if (selectedGenre == genre)
                        cmbGenre.SelectedIndex = item;
                }

            // Set default selection if needed
            if (cmbGenre.SelectedIndex < 0)
                cmbGenre.SelectedIndex = 0;
        }

        /// <summary>
        /// Update root folder filters list
        /// </summary>
        private void UpdateRootFilters()
        {
            cmbRootFilter.Items.Clear();
            cmbRootFilter.Items.Add("Recursive");
            if (cmbRootFolder.SelectedIndex > 0 && cmbRootFolder.SelectedItem != null)
                AddRootFolderFilterItems((ContentRootFolder)cmbRootFolder.SelectedItem);
            cmbRootFilter.SelectedIndex = 0;
        }

        /// <summary>
        /// Add item to root folder combo box
        /// </summary>
        /// <param name="rootFolder"></param>
        private void AddRootFolderFilterItems(ContentRootFolder rootFolder)
        {
            cmbRootFilter.Items.Add("Non-recursive: " + rootFolder.FullPath);
            foreach (ContentRootFolder child in rootFolder.ChildFolders)
                AddRootFolderFilterItems(child);
        }

        /// <summary>
        /// Filter for content listbox
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool ContentFilter(object obj)
        {
            Content content = obj as Content;
 
            // Apply genre filter
            string genre = (string)cmbGenre.SelectedItem;
            bool genreMatch = string.IsNullOrEmpty(genre) || genre.StartsWith("All");
            if (content.Genres != null && !genreMatch)
                foreach (string contentGenre in content.Genres)
                    if (genre.Contains(contentGenre))
                    {
                        genreMatch = true;
                        break;
                    }

            // Apply year filter
            bool yearFilter = (bool)chkYear.IsChecked;
            int minYear = (int)numYearStart.Value;
            int maxYear = (int)numYearStop.Value;
            bool yearMatch = !yearFilter || (content.Date.Year >= minYear && content.Date.Year <= maxYear);

            // Apply text filter
            string nameFilter = txtNameFilter.Text;
            bool nameMatch = string.IsNullOrEmpty(nameFilter) || content.Name.ToLower().Contains(nameFilter.ToLower());

            // Get root folders
            bool recursive;
            List<ContentRootFolder> selRootFolders = GetFilteredRootFolders(out recursive);

            // Check if movie is in the folder
            bool contentInSelFolder = false;
            foreach (ContentRootFolder folder in selRootFolders)
                if (folder.ContainsContent(content, recursive))
                {
                    contentInSelFolder = true;
                    break;
                }

            return genreMatch && yearMatch && nameMatch && contentInSelFolder;
        }

        /// <summary>
        /// Get root folder instances from combo box
        /// </summary>
        /// <returns>List of root folders that are selected</returns>
        public List<ContentRootFolder> GetSelectedRootFolders()
        {
            // Get all root folder
            List<ContentRootFolder> allRootFolders = Settings.GetAllRootFolders(this.ContentType);

            // Get selection string
            if (cmbRootFolder.SelectedItem == null)
                return allRootFolders;
            string selFolderStr = cmbRootFolder.SelectedItem.ToString();

            // Return all if selected
            if (selFolderStr.StartsWith("All"))
                return allRootFolders;

            // Return single select root folder as list
            List<ContentRootFolder> selFolders = new List<ContentRootFolder>();
            foreach (ContentRootFolder folder in allRootFolders)
                if (folder.FullPath == selFolderStr)
                {
                    selFolders.Add(folder);
                    break;
                }
            return selFolders;
        }

        /// <summary>
        /// Gets root folders that fall into filter
        /// </summary>
        /// <param name="recursive">Whether to search recursively through child root folders</param>
        /// <returns></returns>
        public List<ContentRootFolder> GetFilteredRootFolders(out bool recursive)
        {           
            List<ContentRootFolder> baseRootFolders = GetSelectedRootFolders();
            recursive = false;

            if (cmbRootFilter.SelectedItem == null)
                return new List<ContentRootFolder>();

            if (((string)cmbRootFilter.SelectedItem) == "Recursive")
            {
                recursive = true;
                return baseRootFolders;
            }

            string rootFolderPath = ((string)cmbRootFilter.SelectedItem).Replace("Non-recursive: ", "");
            ContentRootFolder filteredFolder;
            if (baseRootFolders.Count > 0 && ContentRootFolder.GetMatchingRootFolder(rootFolderPath, baseRootFolders[0], out filteredFolder))
            {
                List<ContentRootFolder> filteredList = new List<ContentRootFolder>();
                filteredList.Add(filteredFolder);
                return filteredList;
            }

            return baseRootFolders;
        }

        #endregion

    }
}
