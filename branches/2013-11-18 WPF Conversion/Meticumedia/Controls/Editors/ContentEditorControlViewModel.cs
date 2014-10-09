using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Meticumedia.Classes;
using Meticumedia.Controls;
using Meticumedia.Windows;
using Meticumedia.WPF;

namespace Meticumedia.Controls
{
    public class ContentEditorControlViewModel : ViewModel
    {        
        #region Properties

        /// <summary>
        /// Content being editied
        /// </summary>
        public Content Content
        {
            get
            {
                return content;
            }
            set
            {
                content = value;
                OnPropertyChanged(this, "Content");
                OnPropertyChanged(this, "TvShow");
            }
        }
        private Content content;

        public TvShow TvShow
        {
            get
            {
                if (this.Content is TvShow)
                    return this.Content as TvShow;
                else
                    return null;
            }
        }

        /// <summary>
        /// Genre currenlty selected in Genres listbox
        /// </summary>
        public string SelectedGenre
        {
            get
            {
                return selectedGenre;
            }
            set
            {
                selectedGenre = value;
                OnPropertyChanged(this, "SelectedGenre");
            }
        }
        private string selectedGenre = string.Empty;

        /// <summary>
        /// Possible databases for content
        /// </summary>
        public ObservableCollection<string> Databases { get; set; }

        /// <summary>
        /// Alternate name text box entry
        /// </summary>
        public string AlternateNameEntry
        {
            get
            {
                return alternateNameEntry;
            }
            set
            {
                alternateNameEntry = value;
                OnPropertyChanged(this, "AlternateNameEntry");
            }
        }
        private string alternateNameEntry;

        /// <summary>
        /// Genre currenlty selected in Genres listbox
        /// </summary>
        public string SelectedAlternateName
        {
            get
            {
                return selectedAlternateName;
            }
            set
            {
                selectedAlternateName = value;
                OnPropertyChanged(this, "SelectedAlternateName");
            }
        }
        private string selectedAlternateName = string.Empty;

        public Visibility DatabaseSearchVisibility
        {
            get
            {
                return databaseSearchVisibility;
            }
            set
            {
                databaseSearchVisibility = value;
                OnPropertyChanged(this, "DatabaseSearchVisibility");
            }
        }
        private Visibility databaseSearchVisibility = Visibility.Collapsed;

        public Visibility DatabaseStatusVisibility
        {
            get
            {
                return databaseStatusVisibility;
            }
            set
            {
                databaseStatusVisibility = value;
                OnPropertyChanged(this, "DatabaseStatusVisibility");
            }
        }
        private Visibility databaseStatusVisibility = Visibility.Visible;

        public ObservableCollection<object> DatabaseOptions { get; set; }

        public int SelectedSearchDatabaseIndex
        {
            get
            {
                return selectedSearchDatabaseIndex;
            }
            set
            {
                selectedSearchDatabaseIndex = value;
                OnPropertyChanged(this, "SelectedSearchDatabaseIndex");
            }
        }
        private int selectedSearchDatabaseIndex;

        public string SearchStatus
        {
            get
            {
                return searchStatus;
            }
            set
            {
                searchStatus = value;
                OnPropertyChanged(this, "SearchStatus");
            }
        }
        private string searchStatus = string.Empty;

        public Visibility SearchResultsVisibility
        {
            get
            {
                return searchResultsVisibility;
            }
            set
            {
                searchResultsVisibility = value;
                OnPropertyChanged(this, "SearchResultsVisibility");
            }
        }
        private Visibility searchResultsVisibility = Visibility.Collapsed;

        public Visibility SearchStatusVisibility
        {
            get
            {
                return searchStatusVisibility;
            }
            set
            {
                searchStatusVisibility = value;
                OnPropertyChanged(this, "SearchStatusVisibility");
            }
        }
        private Visibility searchStatusVisibility = Visibility.Visible;

        /// <summary>
        /// 
        /// </summary>
        public string SearchString
        {
            get
            {
                return searchString;
            }
            set
            {
                searchString = value;
                OnPropertyChanged(this, "SearchString");
            }
        }
        private string searchString;

        

        /// <summary>
        /// Results from database search
        /// </summary>
        public ObservableCollection<Content> SearchResults { get; set; }

        /// <summary>
        /// Content selected in list of search results
        /// </summary>
        public Content SelectedSearchContent
        {
            get
            {
                return selectedSearchContent;
            }
            set
            {
                selectedSearchContent = value;
                OnPropertyChanged(this, "SelectedSearchContent");
            }
        }
        private Content selectedSearchContent;

        /// <summary>
        /// Changes made set to OK by user
        /// </summary>
        public bool ResultsOk { get; set; }

        /// <summary>
        /// Whether content being edited is TV show
        /// </summary>
        public bool ContentIsTvShow
        {
            get
            {
                return this.Content is TvShow;
            }
        }

        #endregion

        #region Variables

        private BackgroundWorker searchWorker;

        /// <summary>
        /// Timer for updating search results status.
        /// </summary>
        private static System.Timers.Timer searchUpdateTimer;

        #endregion

        #region Commands

        private ICommand addGenreCommand;
        public ICommand AddGenreCommand
        {
            get
            {
                if (addGenreCommand == null)
                {
                    addGenreCommand = new RelayCommand(
                        param => this.AddGenre()
                    );
                }
                return addGenreCommand;
            }
        }
        private ICommand removeGenreCommand;
        public ICommand RemoveGenreCommand
        {
            get
            {
                if (removeGenreCommand == null)
                {
                    removeGenreCommand = new RelayCommand(
                        param => this.RemoveGenre()
                    );
                }
                return removeGenreCommand;
            }
        }

        private ICommand clearGenresCommand;
        public ICommand ClearGenresCommand
        {
            get
            {
                if (clearGenresCommand == null)
                {
                    clearGenresCommand = new RelayCommand(
                        param => this.ClearGenres()
                    );
                }
                return clearGenresCommand;
            }
        }


        private ICommand addAltNameCommand;
        public ICommand AddAltNameCommand
        {
            get
            {
                if (addAltNameCommand == null)
                {
                    addAltNameCommand = new RelayCommand(
                        param => this.AddAltName()
                    );
                }
                return addAltNameCommand;
            }
        }

        private ICommand removeAltNameCommand;
        public ICommand RemoveAltNameCommand
        {
            get
            {
                if (removeAltNameCommand == null)
                {
                    removeAltNameCommand = new RelayCommand(
                        param => this.RemoveAltName()
                    );
                }
                return removeAltNameCommand;
            }
        }

        private ICommand clearAltNamesCommand;
        public ICommand ClearAltNamesCommand
        {
            get
            {
                if (clearAltNamesCommand == null)
                {
                    clearAltNamesCommand = new RelayCommand(
                        param => this.ClearAltNames()
                    );
                }
                return clearAltNamesCommand;
            }
        }

        private ICommand modifyDatabaseIdCommand;
        public ICommand ModifyDatabaseIdCommand
        {
            get
            {
                if (modifyDatabaseIdCommand == null)
                {
                    modifyDatabaseIdCommand = new RelayCommand(
                        param => this.ModifyDatabaseId()
                    );
                }
                return modifyDatabaseIdCommand;
            }
        }

        private ICommand cancelModifyDatabaseIdCommand;
        public ICommand CancelModifyDatabaseIdCommand
        {
            get
            {
                if (cancelModifyDatabaseIdCommand == null)
                {
                    cancelModifyDatabaseIdCommand = new RelayCommand(
                        param => this.CancelModifyDatabaseId()
                    );
                }
                return cancelModifyDatabaseIdCommand;
            }
        }

        private ICommand applyDatabaseIdCommand;
        public ICommand ApplyDatabaseIdCommand
        {
            get
            {
                if (applyDatabaseIdCommand == null)
                {
                    applyDatabaseIdCommand = new RelayCommand(
                        param => this.ApplyDatabaseId(),
                        param => this.CanDoApplyDatabaseIdCommand()
                    );
                }
                return applyDatabaseIdCommand;
            }
        }

        private bool CanDoApplyDatabaseIdCommand()
        {
            return true;
            //return this.SelectedSearchContent != null;
        }

        private ICommand databaseSearchCommand;
        public ICommand DatabaseSearchCommand
        {
            get
            {
                if (databaseSearchCommand == null)
                {
                    databaseSearchCommand = new RelayCommand(
                        param => this.DatabaseSearch()
                    );
                }
                return databaseSearchCommand;
            }
        }

        private ICommand searchStringSimplifyCommand;
        public ICommand SearchStringSimplifyCommand
        {
            get
            {
                if (searchStringSimplifyCommand == null)
                {
                    searchStringSimplifyCommand = new RelayCommand(
                        param => this.SimplifySearchString()
                    );
                }
                return searchStringSimplifyCommand;
            }
        }

        #endregion

        #region Constructor

        public ContentEditorControlViewModel(Content content, bool directEditing)
        {
            this.ResultsOk = false;

            // Edit item directly or through clone
            if (directEditing)
                this.Content = content;
            else
                // Clone content to allow it to be edited, but cancelled
                switch (content.ContentType)
                {
                    case ContentType.Movie:
                        this.Content = new Movie(content as Movie);
                        break;
                    case ContentType.TvShow:
                        this.Content = new TvShow(content as TvShow);
                        break;
                    default:
                        throw new Exception("Unknown content type");
                }

            if (this.Content.Id == Content.UNKNOWN_ID && string.IsNullOrEmpty(this.Content.UserName))
            {
                this.DatabaseStatusVisibility = Visibility.Collapsed;
                this.DatabaseSearchVisibility = Visibility.Visible;
            }
            
            // Get databases for content type
            this.Databases = new ObservableCollection<string>();
            switch (this.Content.ContentType)
            {
                case ContentType.Movie:
                    foreach (MovieDatabaseSelection selection in Enum.GetValues(typeof(MovieDatabaseSelection)))
                        this.Databases.Add(selection.Description());
                    this.SelectedSearchDatabaseIndex = (int)Settings.DefaultMovieDatabase;
                    break;
                case ContentType.TvShow:
                    foreach (TvDataBaseSelection selection in Enum.GetValues(typeof(TvDataBaseSelection)))
                        this.Databases.Add(selection.Description());
                    this.SelectedSearchDatabaseIndex = (int)Settings.DefaultTvDatabase;
                    break;
                default:
                    throw new Exception("Unknown content type");
            }

            this.SearchResults = new ObservableCollection<Content>();
            this.SearchString = System.IO.Path.GetFileName(content.Path);

            searchWorker = new BackgroundWorker();
            searchWorker.WorkerSupportsCancellation = true;
            searchWorker.DoWork += searchWorker_DoWork;
            searchWorker.RunWorkerCompleted += searchWorker_RunWorkerCompleted;

            searchUpdateTimer = new System.Timers.Timer(500);
            searchUpdateTimer.Elapsed += searchUpdateTimer_Elapsed;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add genre to content
        /// </summary>
        private void AddGenre()
        {
            // Create array of genre string for selection
            GenreCollection allGenres;
            if (this.Content is Movie)
                allGenres = Organization.AllMovieGenres;
            else
                allGenres = Organization.AllTvGenres;

            List<string> selectableGenres = new List<string>();
            for (int i = 0; i < allGenres.Count; i++)
            {
                // Don't list genres already added
                if (content.DisplayGenres.Contains(allGenres[i]))
                    continue;

                // Add genre name
                selectableGenres.Add(allGenres[i]);
            }
            string[] selectableGenresArray = selectableGenres.ToArray();

            SelectionWindow selWindow = new SelectionWindow("Select Genre to Add", selectableGenresArray);
            selWindow.ShowDialog();

            // If selection is valid set sub-folder as sub-content folder
            if (!string.IsNullOrEmpty(selWindow.Results))
                foreach (string genre in allGenres)
                    if (genre == selWindow.Results)
                    {
                        this.content.DisplayGenres.Add(genre);
                        break;
                    }

        }

        /// <summary>
        /// Remove selected genre from content
        /// </summary>
        private void RemoveGenre()
        {
            if (string.IsNullOrWhiteSpace(this.SelectedGenre))
                return;

            if (this.content.DisplayGenres.Contains(this.SelectedGenre))
                this.content.DisplayGenres.Remove(this.SelectedGenre);
        }

        /// <summary>
        /// Clear all genres from content
        /// </summary>
        private void ClearGenres()
        {
            this.content.DisplayGenres.Clear();
        }

        /// <summary>
        /// Add alternate name to show
        /// </summary>
        private void AddAltName()
        {

            // If selection is valid set sub-folder as sub-content folder
            if (!string.IsNullOrEmpty(this.AlternateNameEntry) && !this.TvShow.AlternativeNameMatches.Contains(this.AlternateNameEntry))
            {
                this.TvShow.AlternativeNameMatches.Add(this.AlternateNameEntry);
                this.AlternateNameEntry = string.Empty;
            }
        }

        /// <summary>
        /// Remove selected alternate name from show
        /// </summary>
        private void RemoveAltName()
        {
            if (this.TvShow.AlternativeNameMatches.Contains(this.SelectedAlternateName))
                this.TvShow.AlternativeNameMatches.Remove(this.SelectedAlternateName);
        }

        /// <summary>
        /// Clear alternate names from show
        /// </summary>
        private void ClearAltNames()
        {
            this.TvShow.AlternativeNameMatches.Clear();
        }

        private void ModifyDatabaseId()
        {
            this.DatabaseStatusVisibility = Visibility.Collapsed;
            this.DatabaseSearchVisibility = Visibility.Visible;
        }

        private void CancelModifyDatabaseId()
        {
            this.DatabaseSearchVisibility = Visibility.Collapsed;
            this.DatabaseStatusVisibility = Visibility.Visible;
        }

        private void ApplyDatabaseId()
        {
            switch (this.Content.ContentType)
            {
                case ContentType.Movie:
                    ((Movie)this.Content).Clone((Movie)this.SelectedSearchContent, false);
                    break;
                case ContentType.TvShow:
                    ((TvShow)this.Content).Clone((TvShow)this.SelectedSearchContent, false);
                    break;
            }
            this.Content.UpdateInfoFromDatabase();

            this.DatabaseSearchVisibility = Visibility.Collapsed;
            this.DatabaseStatusVisibility = Visibility.Visible;
        }

        private void DatabaseSearch()
        {
            this.SearchStatusVisibility = Visibility.Visible;
            this.SearchResultsVisibility = Visibility.Collapsed;
            searchUpdateTimer.Enabled = true;
            searchWorker.RunWorkerAsync();
        }

        private void SimplifySearchString()
        {
        }
        
        #endregion

        #region Search Event Handlers

        void searchUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                if (this.SearchStatus.Length == 0 || this.SearchStatus.Length > 16)
                    this.SearchStatus = "Searching";
                else
                    this.SearchStatus += ".";
            });
        }

        void searchWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            searchUpdateTimer.Enabled = false;
            this.SearchStatusVisibility = Visibility.Collapsed;
            this.SearchResultsVisibility = Visibility.Visible;
        }

        void searchWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {            
                this.SearchResults.Clear();
            });
            List<Content> results;
            switch (this.Content.ContentType)
            {
                case ContentType.Movie:
                    MovieDatabaseSelection movieDbSel = (MovieDatabaseSelection)this.SelectedSearchDatabaseIndex;
                    results = MovieDatabaseHelper.PerformMovieSearch(movieDbSel, this.SearchString, true);
                    break;
                case ContentType.TvShow:
                    TvDataBaseSelection tvDbSel = (TvDataBaseSelection)this.SelectedSearchDatabaseIndex;
                    results = TvDatabaseHelper.PerformTvShowSearch(tvDbSel, this.SearchString, true);
                    break;
                default:
                    throw new Exception("Unknown content type");
            }
            if (results != null)
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    foreach (Content result in results)
                        this.SearchResults.Add(result);
                });
        }

        #endregion
    }
}
