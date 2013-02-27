// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Meticumedia
{
    /// <summary>
    /// Helper class for displaying list of OrgItem instances in a listview.
    /// </summary>
    public static class OrgItemListHelper
    {
        /// <summary>
        /// Type of setups for columns for displaying items
        /// </summary>
        public enum OrgColumnSetup { MissingCheck, DirectoryScan, MovieFolderCheck, Queue, Log }
        
        /// <summary>
        /// Sets up the columns of a listview for displaying OrgItem lists.
        /// </summary>
        /// <param name="setup">Setup type for selecting which column will be enabled</param>
        /// <param name="lv">The listview to setup columns for</param>
        /// <returns>Dictionary of column types and the column for that type that was set in the listview</returns>
        public static Dictionary<OrgColumnType, OrgItemColumn> SetOrgItemListColumns(OrgColumnSetup setup, ListView lv)
        {
            // Initialize columns dictionary
            Dictionary<OrgColumnType, OrgItemColumn> scanColumns = new Dictionary<OrgColumnType, OrgItemColumn>();
            int columnOrder = 0;

            // Select which columns will be enabled based on setup
            OrgColumnType columnsToAdd = OrgColumnType.Source_Folder | OrgColumnType.Source_File | OrgColumnType.Action | OrgColumnType.Destination_Folder | OrgColumnType.Destination_File;
            if (setup == OrgColumnSetup.DirectoryScan || setup == OrgColumnSetup.MissingCheck)
                columnsToAdd |= OrgColumnType.Category;
            if (setup == OrgColumnSetup.MissingCheck)
                columnsToAdd |= OrgColumnType.Status | OrgColumnType.Show | OrgColumnType.Season | OrgColumnType.Episode;
            if (setup == OrgColumnSetup.Queue)
                columnsToAdd |= OrgColumnType.Progress;
            if (setup == OrgColumnSetup.Log)
                columnsToAdd |= OrgColumnType.DateTime;
            if (setup == OrgColumnSetup.MovieFolderCheck)
                columnsToAdd |= OrgColumnType.Movie;

            // Add the enabled columns to the listview
            lv.Columns.Clear();
            foreach (OrgColumnType type in Enum.GetValues(typeof(OrgColumnType)))
                if ((type & columnsToAdd) > 0)
                {
                    scanColumns.Add(type, new OrgItemColumn(type, columnOrder++));
                    lv.Columns.Add(scanColumns[type].Header);
                }

            // Return columns dictionary
            return scanColumns;
        }

        /// <summary>
        /// Displays a list of OrgItems in a listview.
        /// </summary>
        /// <param name="orgItems">List of OrgItem instances</param>
        /// <param name="lv">The listview where list is to be displayed</param>
        /// <param name="lvColumns">The columns dictionary for the listview (create by using SetOrgItemListColumns method)</param>
        public static void DisplayOrgItemInList(List<OrgItem> orgItems, ListView lv, Dictionary<OrgColumnType, OrgItemColumn> lvColumns)
        {
            DisplayOrgItemInList(orgItems, lv, lvColumns, new int[1] { -1 }, false);
        }

        /// <summary>
        /// Displays or updates a list of OrgItems in a listview and sets the selection to the desired index.
        /// </summary>
        /// <param name="orgItems">List of OrgItem instances</param>
        /// <param name="lv">The listview where list is to be displayed</param>
        /// <param name="lvColumns">The columns dictionary for the listview (create by using SetOrgItemListColumns method)</param>
        /// <param name="selects">The indices to set the selection to. Use -1 for no selection.</param>
        /// <param name="update">Set whether the list is only being updated. Used when changes may have occured to the properties of the OrgItem in the list, but not to the list.</param>
        public static void DisplayOrgItemInList(List<OrgItem> orgItems, ListView lv, Dictionary<OrgColumnType, OrgItemColumn> lvColumns, int[] selects, bool update)
        {
            // Clear listview if new display
            if (!update)
                lv.Items.Clear();

            // Loop through all the items in the list
            for (int i = 0; i < orgItems.Count; i++)
            {
                if (update && i > lv.Items.Count - 1)
                    break;
                
                // Set the current item
                OrgItem orgItem = orgItems[i];

                // Initialize listview item
                ListViewItem item = null;
                int subItemCount = 0;

                // Loop through each column type
                foreach (OrgColumnType type in Enum.GetValues(typeof(OrgColumnType)))
                {
                    // Check if column is enabled in listview
                    if (lvColumns.ContainsKey(type))
                    {
                        // Set text to be displayed in cell based on current column
                        string itemText = string.Empty;
                        switch (type)
                        {
                            case OrgColumnType.DateTime:
                                itemText = orgItem.ActionTime.ToString();
                                break;
                            case OrgColumnType.Show:
                                itemText = orgItem.TvEpisode.Show;
                                break;
                            case OrgColumnType.Season:
                                if (orgItem.TvEpisode.Season >= 0)
                                    itemText = orgItem.TvEpisode.Season.ToString();
                                break;
                            case OrgColumnType.Episode:
                                if (orgItem.TvEpisode.Number > 0)
                                {
                                    itemText = orgItem.TvEpisode.Number.ToString();
                                    if (orgItem.TvEpisode2 != null)
                                        itemText += " & " + orgItem.TvEpisode2.Number.ToString();
                                }
                                break;
                            case OrgColumnType.Movie:
                                if (orgItem.Movie != null)
                                    itemText = orgItem.Movie.Name;
                                break;
                            case OrgColumnType.Source_Folder:
                                if (!string.IsNullOrEmpty(orgItem.SourcePath))
                                {
                                    if (orgItem.Category == FileHelper.FileCategory.Folder)
                                        itemText = orgItem.SourcePath;
                                    else
                                        itemText = Path.GetDirectoryName(orgItem.SourcePath);
                                }
                                break;
                            case OrgColumnType.Source_File:
                                if (!string.IsNullOrEmpty(orgItem.SourcePath) && orgItem.Category != FileHelper.FileCategory.Folder)
                                    itemText = Path.GetFileName(orgItem.SourcePath);
                                break;
                            case OrgColumnType.Category:
                                itemText = orgItem.Category.ToString();
                                break;
                            case OrgColumnType.Status:
                                itemText = orgItem.Status.ToString();
                                break;
                            case OrgColumnType.Action:
                                switch (orgItem.Action)
                                {
                                    case OrgAction.AlreadyExists:
                                        itemText = "None (Destination Already Exists)";
                                        break;
                                    default:
                                        itemText = orgItem.Action.ToString();
                                        break;
                                }

                                
                                break;
                            case OrgColumnType.Destination_Folder:
                                if (orgItem.DestinationPath == FileHelper.DELETE_DIRECTORY)
                                    itemText = orgItem.DestinationPath;
                                else if (!string.IsNullOrEmpty(orgItem.DestinationPath))
                                {
                                    if (orgItem.Category == FileHelper.FileCategory.Folder)
                                        itemText = orgItem.DestinationPath;
                                    else
                                        itemText = Path.GetDirectoryName(orgItem.DestinationPath);
                                }
                                break;
                            case OrgColumnType.Destination_File:
                                if (!string.IsNullOrEmpty(orgItem.DestinationPath) && orgItem.DestinationPath != FileHelper.DELETE_DIRECTORY && orgItem.Category != FileHelper.FileCategory.Folder)
                                    itemText = Path.GetFileName(orgItem.DestinationPath);
                                break;
                            case OrgColumnType.Progress:
                                switch (orgItem.QueueStatus)
                                {
                                    case OrgQueueStatus.Enabled:
                                        itemText = "Queued";
                                        break;
                                    case OrgQueueStatus.Paused:
                                        itemText = "Paused";
                                        if (orgItem.Progress > 0)
                                            itemText += " - " + orgItem.Progress.ToString() + "%";
                                        break;
                                    case OrgQueueStatus.Failed:
                                        itemText = "Failed";
                                        break;
                                    case OrgQueueStatus.Completed:
                                        itemText = "Completed";
                                        break;
                                    case OrgQueueStatus.Cancelled:
                                        itemText = "Cancelled";
                                        break;
                                }
                                break;
                            case OrgColumnType.Number:
                                itemText = orgItem.Number.ToString();
                                break;
                            default:
                                throw new Exception("Unknown column type");
                        }

                        // Check if listview item needs to be initialized
                        if (item == null)
                        {
                            // If check state is set for the OrgItem than apply that to the listview item
                            bool resultsCheckSet = orgItem.Check != CheckState.Indeterminate;
                            bool check;
                            Color backColor = Color.Transparent;
                            if (resultsCheckSet)
                                check = orgItem.Check == CheckState.Checked;
                            else
                                check = true;

                            // Set listview item background color based on action
                            switch (orgItem.Action)
                            {
                                case OrgAction.Copy:
                                case OrgAction.Move:
                                    if (orgItem.QueueStatus != OrgQueueStatus.Paused && orgItem.Progress > 0 && !orgItem.ActionComplete)
                                        backColor = Color.LightYellow;
                                    else
                                        backColor = Color.LightGray;
                                    break;
                                case OrgAction.Delete:
                                    backColor = Color.LightCoral;
                                    break;
                                case OrgAction.Queued:
                                    backColor = Color.LightGoldenrodYellow;
                                    break;
                                case OrgAction.Rename:
                                    backColor = Color.LightGreen;
                                    break;
                                case OrgAction.AlreadyExists:
                                    backColor = Color.LightYellow;
                                    if (!resultsCheckSet)
                                        check = false;
                                    break;
                                default:
                                    // Check only allowed on actions above this
                                    if (!resultsCheckSet)
                                        check = false;
                                    break;
                            }

                            if (orgItem.QueueStatus == OrgQueueStatus.Failed)
                                backColor = Color.Red;
                            else if (orgItem.QueueStatus == OrgQueueStatus.Cancelled)
                                backColor = Color.Yellow;
                            else if (orgItem.QueueStatus == OrgQueueStatus.Completed)
                                backColor = Color.White;

                            // If update than just grab listview item at current index
                            if (update)
                            {
                                item = lv.Items[i];
                                if (item.Text != itemText)
                                    item.Text = itemText;
                            }
                            // Create new listview item
                            else
                                item = lv.Items.Add(itemText);

                            // Set selections
                            foreach (int sel in selects)
                                if (i == sel)
                                {
                                    item.Selected = true;
                                    item.EnsureVisible();
                                }
                            
                            // Apply check to item
                            if (item.Checked != check)
                                item.Checked = check;

                            // Apply background color to item
                            if (backColor != Color.Transparent && item.BackColor != backColor)
                                item.BackColor = backColor;
                        }
                        // Apply change to text for updates
                        else if (update)
                        {
                            if (item.SubItems[subItemCount].Text != itemText)
                                item.SubItems[subItemCount].Text = itemText;
                        }
                        // Add cell text
                        else
                            item.SubItems.Add(itemText);

                        // Increment subitem count
                        ++subItemCount;
                    }
                }

                // Activate listview if new display
                if (!update)
                    lv.Select();
            }
        }
    }
}
