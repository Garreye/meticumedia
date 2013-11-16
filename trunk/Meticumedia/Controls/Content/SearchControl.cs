// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Meticumedia
{
    /// <summary>
    /// Control for performing online database search for either TV shows or movies
    /// </summary>
    public partial class SearchControl : UserControl
    {
        #region Properties

        /// <summary>
        /// Resulting content from search from online database.
        /// </summary>
        public Content Results
        {
            get;
            private set;
        }

        /// <summary>
        /// Search string to be place in search text box
        /// </summary>
        public string SearchString
        {
            set
            {
                txtSearchEntry.Text = value;
            }
        }

        /// <summary>
        /// Indicates whether search is for show (or movie when false)
        /// </summary>
        public ContentType ContentType
        {
            get { return type; }
            set 
            { 
                type = value;
                cmbDatabase.Items.Clear();
                switch (type)
                {
                    case ContentType.Movie:
                        foreach (MovieDatabaseSelection selection in Enum.GetValues(typeof(MovieDatabaseSelection)))
                            cmbDatabase.Items.Add(selection);
                        cmbDatabase.SelectedIndex = (int)Settings.DefaultMovieDatabase;
                        break;
                    case ContentType.TvShow:
                        foreach(TvDataBaseSelection selection in Enum.GetValues(typeof(TvDataBaseSelection)))
                            cmbDatabase.Items.Add(selection);
                        cmbDatabase.SelectedIndex = (int)Settings.DefaultTvDatabase;
                        break;
                }
            }
        }

        public int DatabaseSelection
        {
            get
            {
                return cmbDatabase.SelectedIndex;
            }
            set
            {
                cmbDatabase.SelectedIndex = value;
            }
        }


        public bool MatchVisible
        {
            set
            {
                btnMatch.Visible = value;
            }
        }

        private ContentType type = ContentType.Movie;

        #endregion

        #region Events

        /// <summary>
        /// Arguments class for search result selected event
        /// </summary>
        public class SearchResultsSelectedArgs : EventArgs
        {
            /// <summary>
            /// Indicates if selected result is final selection (user double-clicked it)
            /// </summary>
            public bool CustomSelected { get; set; }

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="final"></param>
            public SearchResultsSelectedArgs(bool custom)
            {
                this.CustomSelected = custom;
            }
        }

        /// <summary>
        /// Indicates that a search result has been selected from list
        /// </summary>
        public event EventHandler<SearchResultsSelectedArgs> SearchResultsSelected;

        /// <summary>
        /// Triggers SearchResultsSelected event
        /// </summary>
        protected void OnSearchResultsSelected(bool custom)
        {
            // Set result from list if valid and trigger event
            if (lvSearchResults.SelectedItems.Count == 0 || searchResults == null)
                this.Results = null;
            else
                this.Results = searchResults[lvSearchResults.SelectedIndices[0]];

            if (SearchResultsSelected != null)
                SearchResultsSelected(this, new SearchResultsSelectedArgs(custom));

        }

        #endregion

        #region Variables

        /// <summary>
        /// Results from search
        /// </summary>
        private List<Content> searchResults;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with know search string
        /// </summary>
        /// <param name="searchString">Search string to put in search text box</param>
        /// <param name="show">Whether search is for show</param>
        public SearchControl()
        {
            InitializeComponent();

            // Defaults
            this.SearchString = string.Empty;

            // Initialize results to null
            this.Results = null;
        }

        /// <summary>
        /// Constructor for debuggin matching.
        /// </summary>
        /// <param name="hideCustom"></param>
        public SearchControl(bool hideCustom) : this()
        {
            btnSetSelected.Visible = hideCustom;
        }

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// Search button causes search for content to be performed.
        /// </summary>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        /// <summary>
        /// Performs search for content.
        /// </summary>
        private void PerformSearch()
        {
            // Do search
            switch (this.ContentType)
            {
                case ContentType.Movie:
                    MovieDatabaseSelection movieDbSel = (MovieDatabaseSelection)cmbDatabase.SelectedItem;
                    searchResults = MovieDatabaseHelper.PerformMovieSearch(movieDbSel, txtSearchEntry.Text, true);    
                    break;
                case ContentType.TvShow:
                    TvDataBaseSelection tvDbSel = (TvDataBaseSelection)cmbDatabase.SelectedItem;
                    searchResults = TvDatabaseHelper.PerformTvShowSearch(tvDbSel, txtSearchEntry.Text, true);
                    break;
                default:
                    throw new Exception("Unknown content type");
            }               

            // Display results
            DisplayResults();
        }

        private void DisplayResults()
        {
            lvSearchResults.Items.Clear();
            foreach (Content result in searchResults)
            {
                ListViewItem item = lvSearchResults.Items.Add(result.Name);
                item.SubItems.Add(result.Date.Year.ToString());
                item.SubItems.Add(result.Id.ToString());
                item.SubItems.Add(result.Overview);
            }
        }

        /// <summary>
        /// Simplifies search string
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSimplify_Click(object sender, EventArgs e)
        {
            txtSearchEntry.Text = FileHelper.SimplifyFileName(txtSearchEntry.Text, FileHelper.OptionalSimplifyRemoves.YearAndFollowing);
        }

        /// <summary>
        /// Listview selection causes set show button to be enabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSearchResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSearchResults.SelectedItems.Count > 0)
                btnSetSelected.Text = "Set Selected";
            else
                btnSetSelected.Text = "Custom";
        }

        /// <summary>
        /// Enter key on search entry performs search.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSearchEntry_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                PerformSearch();
        }

        /// <summary>
        /// Double click selects item from list and closes window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvSearchResults_DoubleClick(object sender, EventArgs e)
        {
            if (lvSearchResults.SelectedItems.Count > 0)
                OnSearchResultsSelected(false);
        }

        /// <summary>
        /// Select show button triggers show selected event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectShow_Click(object sender, EventArgs e)
        {
            OnSearchResultsSelected(btnSetSelected.Text == "Custom");
        }

        /// <summary>
        /// Match button attempts to match search entry to a single item in database - used only for debugging matching.
        /// </summary
        private void bntMatch_Click(object sender, EventArgs e)
        {
            Content match;
            switch (this.ContentType)
            {
                case ContentType.Movie:
                    Movie movieMatch;
                    SearchHelper.MovieSearch.ContentMatch(txtSearchEntry.Text, string.Empty, string.Empty, false, out movieMatch);
                    MovieDatabaseHelper.UpdateMovieInfo(movieMatch);
                    match = movieMatch;
                    break;
                case ContentType.TvShow:
                    TvShow showMatch;
                    SearchHelper.TvShowSearch.ContentMatch(txtSearchEntry.Text, string.Empty, string.Empty, false, out showMatch);
                    TvDatabaseHelper.FullShowSeasonsUpdate(showMatch);
                    match = showMatch;
                    break;
                default:
                    throw new Exception("Unknown content type");
            }                
            searchResults = new List<Content>();
            searchResults.Add(match);
            DisplayResults();
        }

        #endregion
    }
}
