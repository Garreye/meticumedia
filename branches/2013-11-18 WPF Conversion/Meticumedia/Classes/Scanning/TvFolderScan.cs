// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Meticumedia.Classes
{
    public class TvFolderScan : Scan
    {
        public TvFolderScan(bool background)
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
        public List<OrgItem> RunScan(List<ContentRootFolder> folders, List<OrgItem> queuedItems, bool fast)
        {
            // Set running flag
            scanRunning = true;
            cancelRequested = false;

            // Convert all TV folder to scan folders so we can do a scan of them
            List<OrgFolder> tvFoldersAsOrgFolders = new List<OrgFolder>();
            foreach (ContentRootFolder tvFolder in folders)
            {
                OrgFolder orgFolder = new OrgFolder(tvFolder.FullPath, false, false, false, false);
                tvFoldersAsOrgFolders.Add(orgFolder);
            }

            // Do directory scan on all TV folders
            DirectoryScan dirScan = new DirectoryScan(false);
            dirScan.ProgressChange += dirScan_ProgressChange;
            dirScan.RunScan(tvFoldersAsOrgFolders, queuedItems, 90, true, false, fast);
            List<OrgItem> results = dirScan.Items;

            // Check if show folder needs to be renamed!
            int number = results.Count;
            foreach (ContentRootFolder tvFolder in folders)
                for (int i = 0; i < Organization.Shows.Count; i++)
                {
                    TvShow show = (TvShow)Organization.Shows[i];

                    if (show.RootFolder != tvFolder.FullPath)
                        continue;

                    string builtFolder = Path.Combine(show.RootFolder, FileHelper.GetSafeFileName(show.Name));
                    if (show.Path != builtFolder)
                    {
                        OrgItem newItem = new OrgItem(OrgStatus.Organization, OrgAction.Rename, show.Path, builtFolder, new TvEpisode("", show, -1, -1, "", ""), null, FileCategory.Folder, null);
                        newItem.Enable = true;
                        newItem.Number = number++;
                        newItem.Show = show;
                        results.Add(newItem);
                    }
                }

            // Update progress
            OnProgressChange(ScanProcess.TvFolder, string.Empty, 100);

            // Clear flags
            scanRunning = false;

            // Return results
            return results;
        }

        void dirScan_ProgressChange(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            OnProgressChange(ScanProcess.TvFolder, (string)e.UserState, e.ProgressPercentage);
        }
    }
}
