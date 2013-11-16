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
using System.IO;

namespace Meticumedia
{
    /// <summary>
    /// Control that displays episodes for a single TV show
    /// </summary>
    public partial class EpisodesControl : UserControl
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

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public EpisodesControl()
        {
            InitializeComponent();

            SetLegend();
            TvEpisode.BackColourChanged +=new EventHandler(TvEpisode_BackColourChanged);

            // Setup context
            lvEpisodes.ContextMenu = episodeContextMenu;
            episodeContextMenu.Popup += new EventHandler(episodeContextMenu_Popup);
        }

        #endregion

        #region Variables

        /// <summary>
        /// List of currently displayed episodes
        /// </summary>
        private List<TvEpisode> displayedEpisodes = new List<TvEpisode>();

        /// <summary>
        /// Context menu for episodes list
        /// </summary>
        private ContextMenu episodeContextMenu = new ContextMenu();

        #endregion

        #region Context Menu

        /// <summary>
        /// Context menu for episodes listview is built on pop-up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void episodeContextMenu_Popup(object sender, EventArgs e)
        {
            BuildContextMenu();
        }

        /// <summary>
        /// Builds context menu based on what is currently selected
        /// </summary>
        private void BuildContextMenu()
        {
            episodeContextMenu.MenuItems.Clear();

            // Get selected episodes
            List<TvEpisode> selEpisodes;
            if (GetSelectedEpisodes(out selEpisodes))
            {
                bool singleMissing = selEpisodes.Count == 1 && selEpisodes[0].Missing == TvEpisode.MissingStatus.Missing;

                // Add options
                if (!singleMissing)
                    episodeContextMenu.MenuItems.Add("Play", new EventHandler(HandlePlay));
                episodeContextMenu.MenuItems.Add("Copy Episode String to Clipboard", new EventHandler(HandleCopyToClipboard));
                episodeContextMenu.MenuItems.Add("Edit", new EventHandler(HandleEdit));

                if (selEpisodes.Count == 1 && selEpisodes[0].Missing == TvEpisode.MissingStatus.Missing)
                {
                    episodeContextMenu.MenuItems.Add("Locate and Move", new EventHandler(HandleLocate));
                    episodeContextMenu.MenuItems.Add("Locate and Copy", new EventHandler(HandleLocate));
                }

                bool allIgnored = true;
                bool allUnignored = true;
                foreach (TvEpisode ep in selEpisodes)
                    if (ep.Ignored)
                        allUnignored = false;
                    else
                        allIgnored = false;

                if ((allUnignored && !allIgnored) || (!allIgnored && !allUnignored))
                    episodeContextMenu.MenuItems.Add("Ignore", new EventHandler(HandleIgnore));
                if ((allIgnored && !allUnignored) || (!allIgnored && !allUnignored))
                    episodeContextMenu.MenuItems.Add("Unignore", new EventHandler(HandleUnignore));

                episodeContextMenu.MenuItems.Add("Delete Episode File", new EventHandler(HandleDelete));
            }

            episodeContextMenu.MenuItems.Add("New Episode", new EventHandler(HandleAddEpisode));
        }

        /// <summary>
        /// Handles play selection from context menu
        /// </summary>
        private void HandlePlay(object sender, EventArgs e)
        {
            PlaySelectedEpisode();
        }

        /// <summary>
        /// Handles copy ep name to clipboard selection from context menu
        /// </summary>
        private void HandleCopyToClipboard(object sender, EventArgs e)
        {
            CopyEpInfoToClipboard();
        }

        /// <summary>
        /// Handles edit selection from context menu
        /// </summary>
        private void HandleEdit(object sender, EventArgs e)
        {
            TvEpisode selEp;
            if (GetSelectedEpisode(out selEp))
            {
                EpisodeEditorForm eef = new EpisodeEditorForm(selEp);

                if (eef.ShowDialog() == DialogResult.OK)
                {
                    selEp.UpdateInfo(eef.Episode);
                    Organization.Shows.Save();
                    DisplayEpisodesList();
                }
            }
        }

        /// <summary>
        /// Handles locate selection from context menu
        /// </summary>
        private void HandleLocate(object sender, EventArgs e)
        {
            // Get selected episode
            List<TvEpisode> selEpisodes;
            if (!GetSelectedEpisodes(out selEpisodes))
                return;

            // Locate
            List<OrgItem> items;
            if (selEpisodes[0].UserLocate(true, ((MenuItem)sender).Text.Contains("Move"), out items))
                OnItemsToQueue(items);
        }

        /// <summary>
        /// Handles ignore selection from context menu
        /// </summary>
        private void HandleIgnore(object sender, EventArgs e)
        {
            SetIgnoreOnSelected(true);
            DisplayEpisodesList();
        }

        /// <summary>
        /// Handles unignore selection from context menu
        /// </summary>
        private void HandleUnignore(object sender, EventArgs e)
        {
            SetIgnoreOnSelected(false);
            DisplayEpisodesList();
        }

        /// <summary>
        /// Handles delete selection from context menu
        /// </summary>
        private void HandleDelete(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you want to delete the files for selected episode? This operation cannot be undone", "Sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Get selected episode
                List<TvEpisode> selEpisodes;
                if (!GetSelectedEpisodes(out selEpisodes))
                    return;

                List<OrgItem> items = new List<OrgItem>();
                foreach (TvEpisode ep in selEpisodes)
                    if (File.Exists(ep.File.FilePath))
                        items.Add(new OrgItem(OrgAction.Delete, ep.File.FilePath, FileCategory.Trash, null));
                OnItemsToQueue(items);
            }
        }

        /// <summary>
        /// Handle adding new episode selection from context menu
        /// </summary>
        private void HandleAddEpisode(object sender, EventArgs e)
        {
            // Create episode editor
            EpisodeEditorForm eef = new EpisodeEditorForm(new TvEpisode());
            eef.NumbersEnabled = true;

            // Show editor and check for OK
            if (eef.ShowDialog() == DialogResult.OK)
            {
                // Create season if needed
                if (!this.show.Seasons.Contains(eef.Episode.Season))
                    this.show.Seasons.Add(new TvSeason(eef.Episode.Season));

                // Check if episode already exists
                TvEpisode ep;
                if (this.show.FindEpisode(eef.Episode.Season, eef.Episode.Number, false, out ep))
                    if (MessageBox.Show("Episode with this number already exists would you like to replace it?", "Replace", MessageBoxButtons.YesNo) != DialogResult.Yes)
                        return;
                    else
                        this.show.Seasons[eef.Episode.Season].Episodes.Remove(ep);

                // Add episode
                TvEpisode newEp = new TvEpisode(eef.Episode);
                this.show.Seasons[eef.Episode.Season].Episodes.Add(newEp);
                this.show.Seasons[eef.Episode.Season].Episodes.Sort();
                Organization.Shows.Save();
                DisplayEpisodesList();
            }
            
        }

        #endregion

        #region Selected Show Display

        /// <summary>
        /// Show that episodes are being displayed for
        /// </summary>
        public TvShow TvShow
        {
            get { return show; }
            set
            {
                show = value;
                DisplayShow(true);
            }
        }

        /// <summary>
        /// Local variable for TvShow property
        /// </summary>
        private TvShow show = null;

        /// <summary>
        /// Displays show that is selected in show list.
        /// </summary>
        /// <param name="maintainSelection"></param>
        private void DisplayShow(bool maintainSelection)
        {
            // Check that a show is selected
            if (show == null)
                return;

            // Set episodes text
            gbEpisodes.Text = "Episodes of '" + show.Name + "'";

            // Save current filter
            TvEpisodeFilter currentFilterSel = (TvEpisodeFilter)cmbEpFilter.SelectedItem;

            // Buil episode filters
            cmbEpFilter.Items.Clear();
            List<TvEpisodeFilter> epFilters = TvEpisodeFilter.BuildFilters(show, chkDisplayIgnored.Checked, true);
            foreach (TvEpisodeFilter filter in epFilters)
                cmbEpFilter.Items.Add(filter);

            // Set filter selection back to what it was befor building if needed
            if (maintainSelection)
            {
                bool set = false;
                for (int i = 0; i < cmbEpFilter.Items.Count; i++)
                    if (((TvEpisodeFilter)cmbEpFilter.Items[i]).Equals(currentFilterSel))
                    {
                        cmbEpFilter.SelectedIndex = i;
                        set = true;
                        break;
                    }

                if (!set)
                    cmbEpFilter.SelectedIndex = 0;
            }
            else
                cmbEpFilter.SelectedIndex = 0;

            // Display episodes for show in episodes listview
            DisplayEpisodesList();

        }

        /// <summary>
        /// Adds episodes from the selected show to the episodes list
        /// based on the episode filter that is applied.
        /// </summary>
        private void DisplayEpisodesList()
        {
            // Check that filter is valid
            if (cmbEpFilter.SelectedIndex == -1)
                return;

            // Get filter
            TvEpisodeFilter epFilter = (TvEpisodeFilter)cmbEpFilter.SelectedItem;

            // Clear list
            lvEpisodes.Items.Clear();
            displayedEpisodes.Clear();

            // Loop through each episode of each season and
            foreach (TvSeason season in show.Seasons)
                foreach (TvEpisode ep in season.Episodes)
                    // Add episodes that make it through the filter
                    if (epFilter.FilterEpisode(ep))
                        AddEpisodeToList(ep);

        }

        /// <summary>
        /// Adds a single TV episode to episodes list.
        /// </summary>
        /// <param name="ep"></param>
        private void AddEpisodeToList(TvEpisode ep)
        {
            if (!ep.Ignored || chkDisplayIgnored.Checked)
            {
                ListViewItem item = lvEpisodes.Items.Add(ep.Season.ToString());
                item.SubItems.Add(ep.Number.ToString());
                item.SubItems.Add(ep.Name);
                item.SubItems.Add(String.Format("{0:MMM dd yyyy}", ep.AirDate));
                item.SubItems.Add(ep.Overview);
                item.BackColor = ep.GetBackColor();
                displayedEpisodes.Add(ep);
            }
        }

        /// <summary>
        /// Set ignore property on episodes in episodes list 
        /// that are selected.
        /// </summary>
        /// <param name="ignore">The value to set the ignore property to</param>
        private void SetIgnoreOnSelected(bool ignore)
        {
            List<TvEpisode> selEps;
            if (!GetSelectedEpisodes(out selEps))
                return;

            foreach (TvEpisode ep in selEps)
                ep.Ignored = ignore;

            Organization.Shows.Save();
        }

        /// <summary>
        /// Get episodes objects that are selected from list view
        /// </summary>
        private bool GetSelectedEpisodes(out List<TvEpisode> selEps)
        {
            selEps = new List<TvEpisode>();
            foreach (int index in lvEpisodes.SelectedIndices)
                selEps.Add(displayedEpisodes[index]);

            return selEps.Count > 0;
        }

        /// <summary>
        /// Get episode object that is firstly selected from listview
        /// </summary>
        private bool GetSelectedEpisode(out TvEpisode selEp)
        {
            List<TvEpisode> selEps;
            if (GetSelectedEpisodes(out selEps))
            {
                selEp = selEps[0];
                return true;
            }

            selEp = null;
            return false;
        }

        /// <summary>
        /// Refresh display if currently displayed show belongs to episode
        /// </summary>
        /// <param name="episode">Episode that has been updated</param>
        public void UpdateDisplayIfNecessary(TvEpisode episode)
        {
            if (show != null && show.Name == episode.Show)
                DisplayShow(true);
        }  

        #endregion

        #region Event Handlers

        /// <summary>
        /// Updates displayed legend colours when they are changed.
        /// </summary>
        private void TvEpisode_BackColourChanged(object sender, EventArgs e)
        {
            SetLegend();
        }

        /// <summary>
        /// Sets legend colors to episode colors.
        /// </summary>
        private void SetLegend()
        {
            lbMissing.Color = TvEpisode.MissingBackColor;
            lbScanDir.Color = TvEpisode.InScanDirectoryColor;
            lbIgnored.Color = TvEpisode.IgnoredBackColor;
            lbUnaired.Color = TvEpisode.UnairedBackColor;
        }

        /// <summary>
        /// Handle change of episode filter; re-displayed episodes that match filter.
        /// </summary>
        private void cmbEpFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayEpisodesList();
        }

        /// <summary>
        /// Handle change of display ignored check box
        /// </summary>
        private void chkDisplayIgnored_CheckedChanged(object sender, EventArgs e)
        {
            lbIgnored.Visible = chkDisplayIgnored.Checked;
            btnUnignore.Visible = chkDisplayIgnored.Checked;
            DisplayShow(true);
            DisplayEpisodesList();
        }

        /// <summary>
        /// Set ignore property to true on selected episode when button is clicked.
        /// </summary>
        private void btnIgnore_Click(object sender, EventArgs e)
        {
            SetIgnoreOnSelected(true);
            DisplayEpisodesList();
        }

        /// <summary>
        /// Set ignore property to false on selected episode when button is clicked.
        /// </summary>
        private void btnUnignore_Click(object sender, EventArgs e)
        {
            SetIgnoreOnSelected(false);
            DisplayEpisodesList();
        }

        /// <summary>
        /// Double clicking on episode in list plays the episode.
        /// </summary>
        private void lvEpisodes_DoubleClick(object sender, EventArgs e)
        {
            List<TvEpisode> selEpisodes;
            if (GetSelectedEpisodes(out selEpisodes))
            {
                bool singleMissing = selEpisodes.Count == 1 && selEpisodes[0].Missing == TvEpisode.MissingStatus.Missing;

                // Add options
                if (!singleMissing)
                    PlaySelectedEpisode();
                else
                    CopyEpInfoToClipboard();
            }
        }

        /// <summary>
        /// Play episode that is selected
        /// </summary>
        private void PlaySelectedEpisode()
        {
            if (lvEpisodes.SelectedItems.Count == 0)
                return;

            ListViewItem item = lvEpisodes.SelectedItems[0];

            int seasonNumber = int.Parse(item.Text);
            int episodeNumber = int.Parse(item.SubItems[1].Text);

            TvEpisode episode;
            if (show.FindEpisode(seasonNumber, episodeNumber, false, out episode))
                episode.PlayEpisodeFile();
        }

        /// <summary>
        /// Copy selected episode show name and episode number to clipboard
        /// </summary>
        private void CopyEpInfoToClipboard()
        {
            TvEpisode ep;
            if (!GetSelectedEpisode(out ep))
                return;

            Clipboard.SetText(ep.BuildEpString());
        }


        private void btnMoveUp_Click(object sender, EventArgs e)
        {

        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {

        }

        #endregion

    }
}
