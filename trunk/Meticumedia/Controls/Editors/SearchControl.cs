﻿// --------------------------------------------------------------------------------
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
        public bool IsShow { get; set; }

        public bool MatchVisible
        {
            set
            {
                btnMatch.Visible = value;
            }
        }

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
            this.IsShow = false;
            this.SearchString = string.Empty;

            // Initialize results to null
            this.Results = null;
        }

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
            if (IsShow)
                searchResults = TvDatabaseHelper.PerformTvShowSearch(txtSearchEntry.Text, true);
            else
                searchResults = TheMovieDbHelper.PerformMovieSearch(txtSearchEntry.Text);

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
           txtSearchEntry.Text = FileHelper.SimplifyFileName(txtSearchEntry.Text);
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

        #endregion

        private void bntMatch_Click(object sender, EventArgs e)
        {
            Content match;
            if (IsShow)
                match = SearchHelper.TvShowSearch.ContentMatch(txtSearchEntry.Text, string.Empty, string.Empty, false);
            else
                match = SearchHelper.MovieSearch.ContentMatch(txtSearchEntry.Text, string.Empty, string.Empty, false);
            searchResults = new List<Content>();
            searchResults.Add(match);
            DisplayResults();
        }
    }
}