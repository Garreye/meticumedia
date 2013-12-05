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

        /// <summary>
        /// Event indicating selection of content has changed
        /// </summary>
        public event EventHandler<SelectionChangedArgs> SelectionChanged;

        /// <summary>
        /// Triggers SelectionChanged event
        /// </summary>
        /// <param name="items"></param>
        protected void OnSelectionChanged(List<Content> items)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, new SelectionChangedArgs(items));
        }

        /// <summary>
        /// Arguments for selection changes event
        /// </summary>
        public class SelectionChangedArgs : EventArgs
        {
            /// <summary>
            /// List of organization item to be queued
            /// </summary>
            public List<Content> Selections { get; private set; }

            /// <summary>
            /// Constructor with item to be queue
            /// </summary>
            /// <param name="items">Items to be queue</param>
            public SelectionChangedArgs(List<Content> items)
            {
                this.Selections = items;
            }
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
        /// </summary>
        private ObservableCollection<Content> contents;

        #endregion

        #region Constructor

        public ContentControl()
        {
            InitializeComponent();

            Organization.Shows.LoadComplete += new EventHandler(Shows_LoadComplete);
            Settings.SettingsModified += new EventHandler(Settings_SettingsModified);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// When settings are modified the folders are updated
        /// </summary>
        private void Settings_SettingsModified(object sender, EventArgs e)
        {
            int selId = -1;
            if (cmbRootFolder.SelectedItem != null)
                selId = ((ContentRootFolder)cmbRootFolder.SelectedItem).Id;

            folders.Clear();
            folders.Add(new ContentRootFolder(ContentType.TvShow, "All Folders", "All Folders"));
            foreach (ContentRootFolder folder in Settings.TvFolders)
                folders.Add(new ContentRootFolder(folder));

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
        private void Shows_LoadComplete(object sender, EventArgs e)
        {
            contents = new ObservableCollection<Content>();
            lock (Organization.Shows.ContentLock)
            {
                foreach (Content show in Organization.Shows)
                    contents.Add(show);
            }
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                lbShows.ItemsSource = contents;
                UpdateGenresComboBox();
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
                        contents.Remove((TvShow)remItem);
                if (e.NewItems != null)
                    foreach (Content addItem in e.NewItems)
                        contents.Add((TvShow)addItem);
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

        private void lbShows_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Content> selections = new List<Content>();
            foreach (object obj in lbShows.SelectedItems)
                selections.Add((Content)obj);
            OnSelectionChanged(selections);
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

        #endregion

        #region Methods

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

        #endregion




    }
}
