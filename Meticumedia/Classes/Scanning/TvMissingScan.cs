// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Meticumedia.Classes
{
    public class TvMissingScan : Scan
    {

        public TvMissingScan(bool background)
            : base(background)
        {
        }
        
        /// <summary>
        /// Run through all TV shows all looks for episodes that may need to be renamed and for missing episodes.
        /// For missing episodes it attempts to match them to files from the search directories.
        /// </summary>
        /// <param name="shows">Shows to scan</param>
        /// <param name="queuedItems">Items currently in queue (to be skipped)</param>
        /// <returns></returns>
        public List<OrgItem> RunScan(List<Content> shows, List<OrgItem> queuedItems, bool fast)
        {
            // Set running flag
            scanRunning = true;
            cancelRequested = false;

            // Do directory check on all directories (to look for missing episodes)
            while (!TvItemInScanDirHelper.Initialized)
                Thread.Sleep(100);

            List<OrgItem> directoryItems = TvItemInScanDirHelper.Items;

            // Initialiaze scan items
            List<OrgItem> missingCheckItem = new List<OrgItem>();

            // Initialize item numbers
            int number = 0;

            double progressPerShow = 1D / shows.Count * 100D;

            // Go through each show
            for (int i = 0; i < shows.Count; i++)
            {
                TvShow show = (TvShow)shows[i];

                if (cancelRequested)
                    break;

                double showsProgress = (double)i * progressPerShow;
                OnProgressChange(ScanProcess.TvMissing, shows[i].DatabaseName, (int)Math.Round(showsProgress));

                double progressPerEp = 1D / show.Episodes.Count * progressPerShow;

                // Go through missing episodes
                for (int j = 0; j < show.Episodes.Count; j++)
                {
                    // Get episode
                    TvEpisode ep = show.Episodes[j];

                    // Update progress
                    OnProgressChange(ScanProcess.TvMissing, shows[i].DatabaseName, (int)Math.Round(showsProgress + j * progressPerEp));

                    // Check for cancellation
                    if (cancelRequested)
                        break;

                    // Skipped ignored episodes
                    if (ep.Ignored || !show.DoMissingCheck)
                        continue;

                    // Init found flag
                    bool found = false;

                    // Check if episode is missing
                    if (ep.Missing == MissingStatus.Missing || ep.Missing == MissingStatus.InScanDirectory)
                    {
                        // Check directory item for episode
                        foreach (OrgItem item in directoryItems)
                            if ((item.Action == OrgAction.Move || item.Action == OrgAction.Copy) && item.TvEpisode != null && item.TvEpisode.Show.DatabaseName == show.DatabaseName)
                            {
                                // Only add item for first part of multi-part file
                                if (ep.Equals(item.TvEpisode))
                                {
                                    OrgItem newItem = new OrgItem(OrgStatus.Found, item.Action, item.SourcePath, item.DestinationPath, ep, item.TvEpisode2, FileCategory.TvVideo, item.ScanDirectory);
                                    newItem.Enable = true;
                                    newItem.Number = number++;
                                    if (!show.DoMissingCheck)
                                        newItem.Category = FileCategory.Ignored;
                                    missingCheckItem.Add(newItem);
                                    found = true;
                                    break;
                                }
                                else if (ep.Equals(item.TvEpisode2))
                                {
                                    found = true;
                                    break;
                                }
                            }
                    }
                    else
                        continue;

                    // Add empty item for missing
                    if (!found && ep.Aired && show.DoMissingCheck)
                    {
                        OrgItem newItem;
                        TvEpisodeTorrent ezTvEpisode = TvTorrentHelper.GetEpisodeTorrent(ep);
                        if (ezTvEpisode != null)
                        {
                            newItem = new OrgItem(OrgStatus.Missing, OrgAction.Torrent, ep, null, FileCategory.TvVideo, null, ezTvEpisode);
                            newItem.BuildDestination();
                        }
                        else
                            newItem = new OrgItem(OrgStatus.Missing, OrgAction.None, ep, null, FileCategory.TvVideo, null, null);

                        newItem.Number = number++;
                        missingCheckItem.Add(newItem);
                    }
                }
            }

            // Update progress
            OnProgressChange(ScanProcess.TvMissing, string.Empty, 100);

            // Clear flags
            scanRunning = false;

            // Return results
            return missingCheckItem;
        }
    }
}
