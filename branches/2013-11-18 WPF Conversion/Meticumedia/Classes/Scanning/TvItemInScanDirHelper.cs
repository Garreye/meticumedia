// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Threading;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Helper for performing all organizational scans.
    /// </summary>
    public static class TvItemInScanDirHelper
    {
        #region Events

        /// <summary>
        /// Static event for indicating list of TV items in scan folders has been updated
        /// </summary>
        public static event EventHandler<EventArgs> ItemsUpdate;

        /// <summary>
        /// Triggers TvScanItemsUpdate event
        /// </summary>
        public static void OnItemsUpdate()
        {
            if (ItemsUpdate != null)
                ItemsUpdate(null, new EventArgs());
        }

        #endregion

        #region Updating

        public static bool Initialized = false;

        // TV episode from scan directory
        public static List<OrgItem> Items = new List<OrgItem>();

        /// <summary>
        /// TV item in scan directories is updated periodically
        /// </summary>
        private static System.Timers.Timer updateTimer = new System.Timers.Timer(500);

        /// <summary>
        /// Update list of tv episodes currently in scan directories
        /// </summary>
        public static void StartUpdateTimer()
        {            
            // Start timer to do update periodically
            updateTimer.Elapsed += new System.Timers.ElapsedEventHandler(scanDirUpdateTimer_Elapsed);
            updateTimer.Enabled = true;
        }

        /// <summary>
        /// Updates list of TV episodes found in scan directories. Performs a directory scan in background that
        /// only matches to files that are categorized as TV video.
        /// </summary>
        public static void DoUpdate(bool dirScanSearch)
        {
            // Run scan to look for TV files
            if (dirScanSearch)
            {
                DirectoryScan scan = new DirectoryScan(true);
                scan.RunScan(Settings.ScanDirectories.ToList(), new List<OrgItem>(), true, true, true);
                Items = scan.Items;
            }

            // Update missing
            lock (Organization.Shows.ContentLock)
                foreach (TvShow show in Organization.Shows)
                    show.UpdateMissing();

            // Trigger event indicating update has occured
            OnItemsUpdate();
            if (dirScanSearch)
                Initialized = true;
        }

        /// <summary>
        /// Episodes in scan directory are updated periodically.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void scanDirUpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            updateTimer.Interval = 120000;
            DoUpdate(true);
        }

        #endregion
    }
}

