// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Meticumedia
{
    /// <summary>
    /// Main window for Meticumedia application.
    /// </summary>
    public partial class MeticumediaForm : Form
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MeticumediaForm()
        {
            InitializeComponent();

            // Load shows and settings from XML
            Settings.Load();
            Organization.Load();

            // Init word helper
            WordHelper.Initialize();
            
            // Setup shows control
            Organization.Shows.ContentSaved += new EventHandler(showsSaved);

            // Setup content controls
            cntrlMovies.UpdateFolders();
            cntrlShows.UpdateFolders();
            cntrlShows.SelectionChanged += cntrlShows_SelectionChanged;

            // Setup scan contorl
            cntrlScan.UpdateScanSelection();

            // Setup queue control
            cntrlQueue.QueueItemsChanged += new EventHandler<QueueControl.QueueItemsChangedArgs>(cntrlQueue_QueueItemsChanged);
            cntrlQueue.QueueItemsComplete += new EventHandler<QueueControl.QueueItemsCompleteArgs>(cntrlQueue_QueueItemsComplete);
            cntrlQueue.UpdateFromSettings();
  
            // Setup Schedule
            //cntrlSched.UpdateShows();

            // Settings
            SettingsForm.SettingsUpdated += new EventHandler(SettingsForm_SettingsUpdated);

            // Setup database links
            linkTvRage.Links.Add(0, linkTvRage.Text.Length, "www.tvrage.com");
            linkTvRage2.Links.Add(0, linkTvRage.Text.Length, "www.tvrage.com");
            linkMovieDb.Links.Add(0, linkMovieDb.Text.Length, "www.themoviedb.org");
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Selection change on shows display forwarded to episode control for updating
        /// </summary>
        void cntrlShows_SelectionChanged(object sender, ContentControl.SelectionChangedArgs e)
        {
            cntrlEpisodes.TvShow = (TvShow)e.Selections[0];
        }

        /// <summary>
        /// Handler for update of global settings. Triggers controls to update
        /// display of data from settings.
        /// </summary>
        private void SettingsForm_SettingsUpdated(object sender, EventArgs e)
        {
            cntrlScan.UpdateScanSelection();
            cntrlMovies.UpdateFolders();
            cntrlMovies.UpdateContentInFolders(false);
            cntrlShows.UpdateFolders();
            cntrlShows.UpdateContentInFolders(false);
        }

        /// <summary>
        /// Add completed queue items to log and update TV show missing status if needed.
        /// </summary>
        private void cntrlQueue_QueueItemsComplete(object sender, QueueControl.QueueItemsCompleteArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                // Add completed item to log
                cntrlLog.AddLogItems(e.CompleteItem);

                // Check if item was a TV episode
                if (e.CompleteItem.TvEpisode != null)
                {
                    if (e.CompleteItem.NewShow != null || e.CompleteItem.Category == FileHelper.FileCategory.Folder)
                    {
                        cntrlShows.UpdateContentInFolders(false);
                        cntrlSched.UpdateShows();
                        return;
                    }
                    else
                    {
                        // Update missing status for show episode belongs to
                        TvShow itemShow = e.CompleteItem.TvEpisode.GetShow();
                        itemShow.UpdateMissing();

                        // Update folder path for show if it was renamed!
                        if (e.CompleteItem.Action == OrgAction.Rename && e.CompleteItem.Category == FileHelper.FileCategory.Folder)
                            foreach (TvShow show in Organization.Shows)
                                if (show.Path == e.CompleteItem.SourcePath)
                                {
                                    show.Path = e.CompleteItem.DestinationPath;
                                    cntrlShows.UpdateContentInFolders(false);
                                    cntrlSched.UpdateShows();
                                    return;
                                }

                        // Update controls with shows
                        cntrlEpisodes.UpdateDisplayIfNecessary(e.CompleteItem.TvEpisode);

                    }
                }
                // Check if item was a movie
                else if (e.CompleteItem.Movie != null)
                {
                    // If new movie as it to movie list
                    if (!Organization.Movies.Contains(e.CompleteItem.Movie))
                        Organization.Movies.Add(e.CompleteItem.Movie);

                    // Trigger movies update
                    if (e.CompleteItem.Category == FileHelper.FileCategory.Folder)
                        cntrlMovies.UpdateContentInFolders(false);
                }
            });
        }

        /// <summary>
        /// Update scan control with list of items in queue (so scan knows what files are in the queue).
        /// </summary>
        private void cntrlQueue_QueueItemsChanged(object sender, QueueControl.QueueItemsChangedArgs e)
        {
            cntrlScan.UpdateQueueItems(e.QueueItems);
        }

        /// <summary>
        /// Updates controls with show information when a change to a show 
        /// occurs from the shows control.
        /// </summary>
        private void showsSaved(object sender, EventArgs e)
        {
            cntrlScan.Invoke((MethodInvoker)delegate
            {
                cntrlScan.UpdateScanSelection();
            });
        }
        
        /// <summary>
        /// Opens settings window when user clicks settings from file menu
        /// </summary>
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settingForm = new SettingsForm(0);
            settingForm.ShowDialog();
        }

        /// <summary>
        /// Closes the form when user click exit from file menu
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Open the about window when user clicks about from file menu
        /// </summary>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("meticumedia v0.8.3 (alpha)\nCopyright © 2013", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Go to website when database link clicked
        /// </summary>
        private void dbLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData as string);
        }

        /// <summary>
        /// Go to donate page when donate menu item clicked
        /// </summary>
        private void donateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=NE42NQGGL8Q9C&lc=CA&item_name=meticumedia&currency_code=CAD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted");
        }


        private void MeticumediaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Scan.CancelAllScans();
            Organization.CancelFolderUpdating(ContentType.Movie);
            Organization.CancelFolderUpdating(ContentType.TvShow);
            Organization.Save();
        }

        #endregion

    }
}
