using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Meticumedia.Classes;

namespace Meticumedia.Controls
{
    public class ContentCollectionControlViewModel : ProgressViewModel
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

        #region Variables

        /// <summary>
        /// Type of content being displayed
        /// </summary>
        private ContentType contentType;

        private DateTime lastRefresh = DateTime.Now;

        #endregion

        #region Properties

        /// <summary>
        /// Item source for root folder combo box
        /// </summary>
        public ObservableCollection<ContentRootFolder> Folders { get; set; }

        /// <summary>
        /// Select folder item
        /// </summary>
        public ContentRootFolder SelectedFolder
        {
            get
            {
                return selectedFolder;
            }
            set
            {
                selectedFolder = value;
                UpdateRootFilters();
                OnPropertyChanged(this, "SelectedFolder");
            }
        }
        private ContentRootFolder selectedFolder = null;

        #region Filters

        /// <summary>
        /// Item source for root folder filter combo box
        /// </summary>
        public ObservableCollection<string> FolderFilters { get; set; }

        /// <summary>
        /// Select folder filter item
        /// </summary>
        public string SelectedFolderFilter
        {
            get
            {
                return selectedFolderFilter;
            }
            set
            {
                selectedFolderFilter = value;
                UpdateGenresComboBox();
                OnPropertyChanged(this, "SelectedFolderFilter");
            }
        }
        private string selectedFolderFilter = null;

        /// <summary>
        /// Item source for genre filter combo box
        /// </summary>
        public ObservableCollection<string> GenreFilters { get; set; }

        /// <summary>
        /// Select folder filter item
        /// </summary>
        public string SelectedGenreFilter
        {
            get
            {
                return selectedGenreFilter;
            }
            set
            {
                selectedGenreFilter = value;
                this.ContentsCollectionView.Refresh();
                OnPropertyChanged(this, "SelectedGenreFilter");
            }
        }
        private string selectedGenreFilter = string.Empty;

        public bool YearFilterEnable
        {
            get
            {
                return yearFilterEnable;
            }
            set
            {
                yearFilterEnable = value;
                OnPropertyChanged(this, "YearFilterEnable");
            }
        }
        private bool yearFilterEnable = false;

        public int YearFilterStart
        {
            get
            {
                return yearFilterStart;
            }
            set
            {
                yearFilterStart = value;
                OnPropertyChanged(this, "YearFilterStart");
            }
        }
        private int yearFilterStart = 1990;

        public int YearFilterStop
        {
            get
            {
                return yearFilterStop;
            }
            set
            {
                yearFilterStop = value;
                OnPropertyChanged(this, "YearFilterStop");
            }
        }
        private int yearFilterStop = 2015;

        public string NameFilter
        {
            get
            {
                return nameFilter;
            }
            set
            {
                nameFilter = value;
                OnPropertyChanged(this, "NameFilter");
            }
        }
        private string nameFilter = string.Empty;

        #endregion

        /// <summary>
        /// Item source for content listbox
        /// </summary>
        public ObservableCollection<Content> Contents { get; set; }

        /// <summary>
        /// View collection for contents
        /// </summary>
        public ICollectionView ContentsCollectionView { get; set; }

        /// <summary>
        /// Select content item
        /// </summary>
        public Content SelectedContent
        {
            get
            {
                return selectedContent;
            }
            set
            {
                selectedContent = value;
                OnPropertyChanged(this, "SelectedContent");
                OnPropertyChanged(this, "SelectedContentViewModel");
            }
        }
        private Content selectedContent = null;

        public ContentControlViewModel SelectedContentViewModel
        {
            get
            {
                return new ContentControlViewModel(this.SelectedContent);
            }
        }

        #endregion

        #region Constructor

        public ContentCollectionControlViewModel(ContentType contentType)
        {
            this.contentType = contentType;
            this.FolderFilters = new ObservableCollection<string>();
            this.GenreFilters = new ObservableCollection<string>();

            ContentCollection contentCollection = contentType == ContentType.TvShow ? Organization.Shows : Organization.Movies;
            lock (contentCollection.ContentLock)
            {
                foreach (Content cntnt in contentCollection)
                    Contents.Add(cntnt);
                contentCollection.CollectionChanged += Contents_CollectionChanged;
                contentCollection.LoadComplete += Content_LoadComplete;
            }

            Settings.SettingsModified += new EventHandler(Settings_SettingsModified);
            ContentRootFolder.UpdateProgressChange += ContentRootFolder_UpdateProgressChange;

            this.Folders = new ObservableCollection<ContentRootFolder>();

            this.Contents = new ObservableCollection<Content>();
            CollectionViewSource contentViewSource = new CollectionViewSource() { Source = this.Contents };
            this.ContentsCollectionView = contentViewSource.View;
            this.ContentsCollectionView.Filter = new Predicate<object>(ContentFilter);

            // Set properties to trigger live updating
            ICollectionViewLiveShaping liveCollection = this.ContentsCollectionView as ICollectionViewLiveShaping;
            liveCollection.LiveFilteringProperties.Add("DisplayGenres");
            liveCollection.LiveFilteringProperties.Add("DisplayYear");
            liveCollection.LiveFilteringProperties.Add("RootFolder");
            liveCollection.LiveFilteringProperties.Add("DisplayName");
            liveCollection.IsLiveFiltering = true;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// When settings are modified the folders are updated
        /// </summary>
        private void Settings_SettingsModified(object sender, EventArgs e)
        {
            int selId = -1;
            if (this.SelectedFolder != null)
                selId = this.SelectedFolder.Id;

            Folders.Clear();
            switch (this.contentType)
            {
                case ContentType.Movie:
                    this.Folders.Add(ContentRootFolder.AllMoviesFolders);
                    break;
                case ContentType.TvShow:
                    this.Folders.Add(ContentRootFolder.AllTvFolders);
                    break;
            }
            
            switch (this.contentType)
            {
                case ContentType.TvShow:
                    foreach (ContentRootFolder folder in Settings.TvFolders)
                        Folders.Add(new ContentRootFolder(folder));
                    break;
                case ContentType.Movie:
                    foreach (ContentRootFolder folder in Settings.MovieFolders)
                        Folders.Add(new ContentRootFolder(folder));
                    break;
            }

            // Set selection
            if (selId >= 0)
            {
                foreach (ContentRootFolder folder in Folders)
                    if (folder.Id == selId)
                    {
                        this.SelectedFolder = folder;
                        break;
                    }
            }
            else
                this.SelectedFolder = this.Folders[0];
        }

        /// <summary>
        /// Loading complete of shows load them into shows listbox
        /// </summary>
        private void Content_LoadComplete(object sender, EventArgs e)
        {
            ContentCollection contentCollection = (ContentCollection)sender;
            if (contentCollection.ContentType != this.contentType)
                return;

            UpdateGenresComboBoxSafe();
            UpdateProgressSafe(100, "Loading complete!", true);
        }

        /// <summary>
        /// Changes to shows collection are reflected in item source for shows listbox
        /// </summary>
        private void Contents_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                UpdateContents(e);
            });
        }

        private void UpdateContents(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                Contents.Clear();

            if (e.OldItems != null)
                foreach (Content remItem in e.OldItems)
                    Contents.Remove(remItem);
            if (e.NewItems != null)
                foreach (Content addItem in e.NewItems)
                    Contents.Add(addItem);

        }

        /// <summary>
        /// Load progress shown in bar
        /// </summary>
        private void Content_LoadProgressChange(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            UpdateProgressSafe(e.ProgressPercentage, "Loading " + this.contentType.Description() + "s " + (string)e.UserState, e.ProgressPercentage >= 100);
        }

        private void ContentRootFolder_UpdateProgressChange(object sender, OrgProgressChangesEventArgs e)
        {
            ContentRootFolder folder = sender as ContentRootFolder;
            if (folder.ContentType == this.contentType)
            {
                UpdateProgressSafe(e.ProgressPercentage, (string)e.UserState, e.ProgressPercentage < 100);
            }
        }

        #endregion

        #region Methods

        private void RefreshContentsSafe(bool limitRate)
        {
            if (limitRate && (DateTime.Now - lastRefresh).TotalSeconds < 5)
                return;

            lastRefresh = DateTime.Now;

            try
            {
                if (App.Current.Dispatcher.CheckAccess())
                    this.ContentsCollectionView.Refresh();
                else
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        this.ContentsCollectionView.Refresh();
                    });
            }
            catch { }
        }

        private void UpdateGenresComboBoxSafe()
        {
            if (App.Current.Dispatcher.CheckAccess())
                UpdateGenresComboBox();
            else
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    UpdateGenresComboBox();
                });
        }

        /// <summary>
        /// Update Genres in combo box based on what genres are actually in the content displayed
        /// </summary>
        private void UpdateGenresComboBox()
        {
            // Save current selection
            string selGenre = this.SelectedGenreFilter;

            // Get selected folder
            ContentRootFolder selFolder = this.SelectedFolder;
            if (selFolder == null)
                return;

            // Add all available genres
            GenreCollection genres = null;
            switch (this.contentType)
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
            this.GenreFilters.Clear();
            this.GenreFilters.Add("All Genres");
            lock (genres.AccessLock)
                foreach (string genre in genres)
                    this.GenreFilters.Add(genre);

            // Set default selection if needed
            if (!string.IsNullOrEmpty(selGenre) && this.GenreFilters.Contains(selGenre))
                this.SelectedGenreFilter = selGenre;
            else
                this.SelectedGenreFilter = this.GenreFilters[0];
            
        }

        /// <summary>
        /// Update root folder filters list
        /// </summary>
        private void UpdateRootFilters()
        {
             this.FolderFilters.Clear();
             this.FolderFilters.Add("Recursive");
            if (this.SelectedFolder != null)
                AddRootFolderFilterItems(this.SelectedFolder);
            this.SelectedFolderFilter = this.FolderFilters[0];
        }

        /// <summary>
        /// Add item to root folder combo box
        /// </summary>
        /// <param name="rootFolder"></param>
        private void AddRootFolderFilterItems(ContentRootFolder rootFolder)
        {
            this.FolderFilters.Add("Non-recursive: " + rootFolder.FullPath);
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
            string genre = this.SelectedGenreFilter;
            bool genreMatch = string.IsNullOrEmpty(genre) || genre.StartsWith("All");
            if (content.DatabaseGenres != null && !genreMatch)
                foreach (string contentGenre in content.DatabaseGenres)
                    if (genre.Contains(contentGenre))
                    {
                        genreMatch = true;
                        break;
                    }

            // Apply year filter
            bool yearMatch = !this.YearFilterEnable || (content.DatabaseYear >= this.YearFilterStart && content.DatabaseYear <= this.YearFilterStop);

            // Apply text filter
            bool nameMatch = string.IsNullOrEmpty(this.NameFilter) || content.DatabaseName.ToLower().Contains(this.NameFilter.ToLower());

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
            List<ContentRootFolder> allRootFolders = Settings.GetAllRootFolders(this.contentType);

            // Get selection string
            if (this.SelectedFolder == null)
                return allRootFolders;

            // Return all if selected
            if (this.SelectedFolder.FullPath.StartsWith("All"))
                return allRootFolders;

            // Return single select root folder as list
            List<ContentRootFolder> selFolders = new List<ContentRootFolder>();
            foreach (ContentRootFolder folder in allRootFolders)
                if (folder.FullPath == this.SelectedFolder.FullPath)
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

            if (this.SelectedFolderFilter == null)
                return new List<ContentRootFolder>();

            if (this.SelectedFolderFilter == "Recursive")
            {
                recursive = true;
                return baseRootFolders;
            }

            string rootFolderPath = this.SelectedFolderFilter.Replace("Non-recursive: ", "");
            ContentRootFolder filteredFolder;
            if (baseRootFolders.Count > 0 && ContentRootFolder.GetMatchingRootFolder(rootFolderPath, baseRootFolders[0], out filteredFolder))
            {
                List<ContentRootFolder> filteredList = new List<ContentRootFolder>();
                filteredList.Add(filteredFolder);
                return filteredList;
            }

            return baseRootFolders;
        }

        /// <summary>
        /// Get genre instances from combo box
        /// </summary>
        /// <returns>List of genres that are selected</returns>
        public GenreCollection GetSelectedGenres(out bool filterEnable)
        {
            // Get all genres
            GenreCollection allGenres = Organization.GetAllGenres(this.contentType);

            // Get selection string
            if (this.SelectedGenreFilter == null)
            {
                filterEnable = false;
                return allGenres;
            }

            // Return all if selected
            if (this.SelectedGenreFilter.StartsWith("All"))
            {
                filterEnable = false;
                return allGenres;
            }

            // Return single select genre as list
            GenreCollection genres = new GenreCollection(this.contentType);
            foreach (string genre in allGenres)
                if (this.SelectedGenreFilter == genre)
                {
                    genres.Add(genre);
                    break;
                }
            filterEnable = true;
            return genres;
        }

        /// <summary>
        /// Updates contents from Organization based on selected folder.
        /// </summary>
        public void UpdateContent()
        {
            // Save current selection from listview
            Content currentSel = this.SelectedContent;

            // Get selected folder
            ContentRootFolder folderName = this.SelectedFolder;

            // Get root folders
            bool recursive;
            List<ContentRootFolder> selRootFolders = GetFilteredRootFolders(out recursive);

            bool genreFilterEnable;
            GenreCollection genreFilter = GetSelectedGenres(out genreFilterEnable);

            // Set contents for listview
            List<Content> contents = Organization.GetContentFromRootFolders(selRootFolders, recursive, genreFilterEnable, genreFilter, this.YearFilterEnable, this.YearFilterStart, this.YearFilterStop, this.NameFilter);
            Contents.Clear();
            foreach (Content content in contents)
                this.Contents.Add(content);
            
        }

        #endregion

    }
}
