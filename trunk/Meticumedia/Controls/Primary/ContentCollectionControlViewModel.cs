using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Meticumedia.Classes;
using Meticumedia.Windows;
using Meticumedia.WPF;

namespace Meticumedia.Controls
{
    public class ContentCollectionControlViewModel : OrgItemQueueableViewModel
    {
        #region Variables

        /// <summary>
        /// Type of content being displayed
        /// </summary>
        private ContentType contentType;

        private DateTime lastRefresh = DateTime.Now;

        private ListBox contentListBox;

        #endregion

        #region Properties

        public ContentType ContentType { get; set; }

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
                OnPropertyChanged(this, "SelectedGenreFilter");
                this.ContentsCollectionView.Refresh();
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
                this.ContentsCollectionView.Refresh();
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
                this.ContentsCollectionView.Refresh();
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
                this.ContentsCollectionView.Refresh();
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
                this.ContentsCollectionView.Refresh();
            }
        }
        private string nameFilter = string.Empty;

        public bool WatchedFilter
        {
            get
            {
                return watchedFilter;
            }
            set
            {
                watchedFilter = value;
                OnPropertyChanged(this, "WatchedFilter");
                this.ContentsCollectionView.Refresh();
            }
        }
        private bool watchedFilter = true;

        public bool UnwatchedFilter
        {
            get
            {
                return unwatchedFilter;
            }
            set
            {
                unwatchedFilter = value;
                OnPropertyChanged(this, "UnwatchedFilter");
                this.ContentsCollectionView.Refresh();
            }
        }
        private bool unwatchedFilter = true;

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

        Dictionary<Content, ContentControlViewModel> contentViewModels = new Dictionary<Content, ContentControlViewModel>();

        public IList SelectedContents
        {
            get
            {
                return selectedContents;
            }
            set
            {
                selectedContents = value;
                OnPropertyChanged(this, "SelectedContentViewModel");
                OnPropertyChanged(this, "SingleItemSelectionVisibility");
            }
        }
        private IList selectedContents;

        public ContentControlViewModel SelectedContentViewModel
        {
            get
            {
                if (this.SelectedContents == null || this.SelectedContents.Count != 1 || this.SelectedContent == null)
                    selectedContentViewModel = null;
                else if (selectedContentViewModel == null || !selectedContentViewModel.Content.Equals(this.SelectedContent))
                {
                    if (!contentViewModels.ContainsKey(this.SelectedContent))
                        contentViewModels.Add(this.SelectedContent, new ContentControlViewModel(this.SelectedContent));
                    selectedContentViewModel = contentViewModels[this.SelectedContent];
                }
                return selectedContentViewModel;
            }
        }
        private ContentControlViewModel selectedContentViewModel;

        public Visibility SingleItemSelectionVisibility
        {
            get
            {
                return this.SelectedContents != null && this.SelectedContents.Count == 1 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility MoveToVisibility
        {
            get
            {
                return moveToVisibility;
            }
            set
            {
                moveToVisibility = value;
                OnPropertyChanged(this, "MoveToVisibility");
            }
        }
        private Visibility moveToVisibility = Visibility.Collapsed;

        public ObservableCollection<MenuItem> MoveRootFolderItems { get; set; }

        #endregion

        #region Commands

        private ICommand editCommand;
        public ICommand EditCommand
        {
            get
            {
                if (editCommand == null)
                {
                    editCommand = new RelayCommand(
                        param => this.EditContent()
                    );
                }
                return editCommand;
            }
        }

        private ICommand markAsWatchedCommand;
        public ICommand MarkAsWatchedCommand
        {
            get
            {
                if (markAsWatchedCommand == null)
                {
                    markAsWatchedCommand = new RelayCommand(
                        param => this.SetWatched(true)
                    );
                }
                return markAsWatchedCommand;
            }
        }

        private ICommand unmarkAsWatchedCommand;
        public ICommand UnmarkAsWatchedCommand
        {
            get
            {
                if (unmarkAsWatchedCommand == null)
                {
                    unmarkAsWatchedCommand = new RelayCommand(
                        param => this.SetWatched(false)
                    );
                }
                return unmarkAsWatchedCommand;
            }
        }

        private ICommand setFolderToRootCommand;
        public ICommand SetFolderToRootCommand
        {
            get
            {
                if (setFolderToRootCommand == null)
                {
                    setFolderToRootCommand = new RelayCommand(
                        param => this.SetAsRootFolder()
                    );
                }
                return setFolderToRootCommand;
            }
        }

        private ICommand deleteCommand;
        public ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                {
                    deleteCommand = new RelayCommand(
                        param => this.DeleteContent()
                    );
                }
                return deleteCommand;
            }
        }

        #endregion

        #region Constructor

        public ContentCollectionControlViewModel(ContentType contentType, ListBox contentListBox)
        {
            this.contentType = contentType;
            this.contentListBox = contentListBox;
            this.FolderFilters = new ObservableCollection<string>();
            this.GenreFilters = new ObservableCollection<string>();
            this.MoveRootFolderItems = new ObservableCollection<MenuItem>();

            this.contentListBox.SelectionChanged += listView_SelectionChanged;

            this.ContentType = contentType;
            ContentCollection contentCollection = contentType == ContentType.TvShow ? Organization.Shows : Organization.Movies;
            lock (contentCollection.ContentLock)
            {
                foreach (Content cntnt in contentCollection)
                    Contents.Add(cntnt);
                contentCollection.LoadProgressChange += Content_LoadProgressChange;
                contentCollection.CollectionChanged += Contents_CollectionChanged;
                contentCollection.LoadComplete += Content_LoadComplete;
            }

            Settings.SettingsModified += new EventHandler(Settings_SettingsModified);
            ContentRootFolder.UpdateProgressChange += ContentRootFolder_UpdateProgressChange;

            this.Folders = new ObservableCollection<ContentRootFolder>();

            this.Contents = new ObservableCollection<Content>();
            this.ContentsCollectionView = CollectionViewSource.GetDefaultView( this.Contents);
            this.ContentsCollectionView.Filter = new Predicate<object>(ContentFilter);
            this.ContentsCollectionView.SortDescriptions.Add(new SortDescription("DisplayName", ListSortDirection.Ascending));

            // Set properties to trigger live updating
            ICollectionViewLiveShaping liveCollection = this.ContentsCollectionView as ICollectionViewLiveShaping;
            liveCollection.LiveFilteringProperties.Add("DisplayGenres");
            liveCollection.LiveFilteringProperties.Add("DisplayYear");
            liveCollection.LiveFilteringProperties.Add("RootFolder");
            liveCollection.LiveFilteringProperties.Add("DisplayName");
            liveCollection.LiveFilteringProperties.Add("Watched");
            liveCollection.IsLiveFiltering = true;

            liveCollection.IsLiveSorting = true;
            liveCollection.LiveSortingProperties.Add("DisplayName");
        }

        

        #endregion

        #region Event Handlers

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged(this, "SingleItemSelectionVisibility");
            UpdateRootFolderItems();
        }

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
                    List<ContentRootFolder> allTvFolders = Settings.TvFolders.GetFolders(true);
                    foreach (ContentRootFolder folder in allTvFolders)
                        Folders.Add(new ContentRootFolder(folder));
                    break;
                case ContentType.Movie:
                    List<ContentRootFolder> allMovieFolders = Settings.MovieFolders.GetFolders(true);
                    foreach (ContentRootFolder folder in allMovieFolders)
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

        private bool contentLoaded = false;

        /// <summary>
        /// Loading complete of shows load them into shows listbox
        /// </summary>
        private void Content_LoadComplete(object sender, EventArgs e)
        {
            ContentCollection contentCollection = (ContentCollection)sender;
            if (contentCollection.ContentType != this.contentType)
                return;

            contentLoaded = true;
            UpdateGenresComboBoxSafe();
            UpdateProgressSafe(100, "Loading complete!", true);
        }

        /// <summary>
        /// Changes to shows collection are reflected in item source for shows listbox
        /// </summary>
        private void Contents_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (contentLoaded)
                UpdateGenresComboBoxSafe();

            // Invoke can result in dead-lock if another app thread if waiting for collection.ContentLock
            ContentCollection collection = sender as ContentCollection;
            
            if (App.Current.Dispatcher.CheckAccess())
                UpdateContents(e, collection);
            else
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    UpdateContents(e, collection);
                });
            
        }

        private void UpdateContents(System.Collections.Specialized.NotifyCollectionChangedEventArgs e, ContentCollection collection)
        {
            lock (collection.ContentLock)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    Contents.Clear();
                    contentViewModels.Clear();
                }

                if (e.OldItems != null)
                    foreach (Content remItem in e.OldItems)
                    {
                        Contents.Remove(remItem);
                        contentViewModels.Remove(remItem);
                    }
                if (e.NewItems != null)
                    foreach (Content addItem in e.NewItems)
                    {
                        Contents.Add(addItem);
                        //contentViewModels.Add(addItem, new ContentControlViewModel(addItem));
                    }
            }

        }

        /// <summary>
        /// Load progress shown in bar
        /// </summary>
        private void Content_LoadProgressChange(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            UpdateProgressSafe(e.ProgressPercentage, "Loading " + this.contentType.Description() + "s " + (string)e.UserState, e.ProgressPercentage < 100);
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

        private void UpdateRootFolderItems()
        {
            if (this.SelectedContents == null)
                return;
            
            List<string> rootFolderOfSelected = new List<string>();
            foreach (Content content in this.SelectedContents)
                if (!rootFolderOfSelected.Contains(content.RootFolder))
                    rootFolderOfSelected.Add(content.RootFolder);

            // Exclude root folder if all selections are the same
            string excludeRootFolder = string.Empty;
            if (rootFolderOfSelected.Count == 1)
                excludeRootFolder = rootFolderOfSelected[0];
            
            List<ContentRootFolder> rootFolders = Settings.GetAllRootFolders(this.ContentType, true);

            this.MoveRootFolderItems.Clear();
            foreach (ContentRootFolder folder in rootFolders)
            {
                if (folder.FullPath == excludeRootFolder)
                    continue;
                
                MenuItem item = new MenuItem();
                item.Header = folder.FullPath;
                item.Command = new RelayCommand(param => this.MoveContent(folder.FullPath));

                this.MoveRootFolderItems.Add(item);
            }

            this.MoveToVisibility = this.MoveRootFolderItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Open content editor window
        /// </summary>
        private void EditContent()
        {
            ContentEditorWindow cew = new ContentEditorWindow(this.SelectedContent);
            cew.ShowDialog();

            if (cew.Results != null)
            {
                if (this.SelectedContent is Movie)
                {
                    (this.SelectedContent as Movie).CloneAndHandlePath(cew.Results as Movie, false);
                    Organization.Movies.Save();
                }
                else
                {
                    (this.SelectedContent as TvShow).CloneAndHandlePath(cew.Results as TvShow, false);
                    Organization.Shows.Save();
                }
            }

        }

        private void SetWatched(bool watched)
        {
            this.SelectedContent.Watched = watched;
        }

        private void SetAsRootFolder()
        {
            // Mark each item
            foreach (Content cnt in this.SelectedContents)
            {
                // Format path
                string fullPath = System.IO.Path.GetFullPath(cnt.Path).TrimEnd(System.IO.Path.DirectorySeparatorChar);
                string path = System.IO.Path.GetFileName(fullPath);

                // Get content folder for content
                ContentRootFolder folder;
                if (Settings.GetContentFolderFromPath(cnt.RootFolder, out folder))
                    // Convert movie folder as sub-folder
                    folder.ChildFolders.Add(new ContentRootFolder(this.ContentType, path, fullPath));
            }
            Settings.Save();
            Organization.UpdateRootFolders(this.ContentType);
        }

        private void MoveContent(string destinationFolder)
        {
            if (!string.IsNullOrEmpty(destinationFolder))
            {
                List<OrgItem> items = new List<OrgItem>();
                foreach (Content content in this.SelectedContents)
                {
                    // Check that move is necessary
                    if (content.RootFolder == destinationFolder)
                        continue;

                    // Format path
                    string currentPath = Path.GetFullPath(content.Path).TrimEnd(Path.DirectorySeparatorChar);
                    string endFolder = Path.GetFileName(currentPath);
                    string newPath = Path.Combine(destinationFolder, endFolder);
                    items.Add(new OrgItem(OrgAction.Move, currentPath, content, newPath));
                }
                OnItemsToQueue(items);
            }
        }

        private void DeleteContent()
        {
            if (MessageBox.Show("Are you want to delete the selected items? This operation cannot be undone", "Sure?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                List<OrgItem> items = new List<OrgItem>();
                foreach (Content content in this.SelectedContents)
                {
                   OrgItem item = new OrgItem(OrgAction.Delete, content.Path, content, null);
                   items.Add(item);
                }
                OnItemsToQueue(items);
            }
        }

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
             this.FolderFilters.Add("Non-Recursive");
            //if (this.SelectedFolder != null)
            //    AddRootFolderFilterItems(this.SelectedFolder);
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

            // Check if content is in the folder
            bool contentInSelFolder = false;
            if (selRootFolders.Count == 0)
                contentInSelFolder = true;
            else
            {
                foreach (ContentRootFolder folder in selRootFolders)
                    if (folder.ContainsContent(content, recursive))
                    {
                        contentInSelFolder = true;
                        break;
                    }
            }

            bool watchedFilter = (content.Watched && this.WatchedFilter) || (!content.Watched && this.UnwatchedFilter);

            bool filterResult = genreMatch && yearMatch && nameMatch && contentInSelFolder && watchedFilter;
            return filterResult;
        }

        /// <summary>
        /// Get root folder instances from combo box
        /// </summary>
        /// <returns>List of root folders that are selected</returns>
        public List<ContentRootFolder> GetSelectedRootFolders()
        {
            // Get all root folder
            List<ContentRootFolder> allRootFolders = Settings.GetAllRootFolders(this.contentType, true);

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

            recursive = this.SelectedFolderFilter == "Recursive";
            return baseRootFolders;

            //string rootFolderPath = this.SelectedFolderFilter.Replace("Non-recursive: ", "");
            //ContentRootFolder filteredFolder;
            //if (baseRootFolders.Count > 0 && ContentRootFolder.GetMatchingRootFolder(rootFolderPath, baseRootFolders[0], out filteredFolder))
            //{
            //    List<ContentRootFolder> filteredList = new List<ContentRootFolder>();
            //    filteredList.Add(filteredFolder);
            //    return filteredList;
            //}

            //return baseRootFolders;
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
