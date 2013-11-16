// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Meticumedia
{
    public class OrgListItem
    {
        public bool Displayed { get; set; }

        public ListViewItem ListItem { get; set; }

        public OrgItem OrgItem { get; set; }

        public OrgListItem(OrgItem orgItem, Dictionary<OrgColumnType, OrgItemColumn> lvColumns)
        {
            this.OrgItem = orgItem;
            this.ListItem = new ListViewItem();
            UpdateListViewItem(lvColumns);
        }

        public void UpdateListViewItem(Dictionary<OrgColumnType, OrgItemColumn> lvColumns)
        {
            // Initialize listview item
            int subItemCount = 0;
            bool firstItem = true;

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
                            itemText = OrgItem.ActionTime.ToString();
                            break;
                        case OrgColumnType.Show:
                            itemText = OrgItem.TvEpisode.Show;
                            break;
                        case OrgColumnType.Season:
                            if (OrgItem.TvEpisode.Season >= 0)
                                itemText = OrgItem.TvEpisode.Season.ToString();
                            break;
                        case OrgColumnType.Episode:
                            if (OrgItem.TvEpisode.Number > 0)
                            {
                                itemText = OrgItem.TvEpisode.Number.ToString();
                                if (OrgItem.TvEpisode2 != null)
                                    itemText += " & " + OrgItem.TvEpisode2.Number.ToString();
                            }
                            break;
                        case OrgColumnType.Movie:
                            if (OrgItem.Movie != null)
                                itemText = OrgItem.Movie.Name;
                            break;
                        case OrgColumnType.Source_Folder:
                            if (!string.IsNullOrEmpty(OrgItem.SourcePath))
                            {
                                if (OrgItem.Category == FileCategory.Folder)
                                    itemText = OrgItem.SourcePath;
                                else
                                    itemText = Path.GetDirectoryName(OrgItem.SourcePath);
                            }
                            break;
                        case OrgColumnType.Source_File:
                            if (!string.IsNullOrEmpty(OrgItem.SourcePath) && OrgItem.Category != FileCategory.Folder)
                                itemText = Path.GetFileName(OrgItem.SourcePath);
                            break;
                        case OrgColumnType.Category:
                            itemText = OrgItem.Category.ToString();
                            break;
                        case OrgColumnType.Status:
                            itemText = OrgItem.Status.ToString();
                            break;
                        case OrgColumnType.Action:
                            switch (OrgItem.Action)
                            {
                                case OrgAction.AlreadyExists:
                                    itemText = "None (Destination Already Exists)";
                                    break;
                                default:
                                    itemText = OrgItem.Action.ToString();
                                    break;
                            }


                            break;
                        case OrgColumnType.Destination_Folder:
                            if (OrgItem.DestinationPath == FileHelper.DELETE_DIRECTORY)
                                itemText = OrgItem.DestinationPath;
                            else if (!string.IsNullOrEmpty(OrgItem.DestinationPath))
                            {
                                if (OrgItem.Category == FileCategory.Folder)
                                    itemText = OrgItem.DestinationPath;
                                else
                                    itemText = Path.GetDirectoryName(OrgItem.DestinationPath);
                            }
                            break;
                        case OrgColumnType.Destination_File:
                            if (!string.IsNullOrEmpty(OrgItem.DestinationPath) && OrgItem.DestinationPath != FileHelper.DELETE_DIRECTORY && OrgItem.Category != FileCategory.Folder)
                                itemText = Path.GetFileName(OrgItem.DestinationPath);
                            break;
                        case OrgColumnType.Progress:
                            switch (OrgItem.QueueStatus)
                            {
                                case OrgQueueStatus.Enabled:

                                    if (OrgItem.Progress > 0)
                                        itemText = OrgItem.Progress.ToString() + "%";
                                    else
                                        itemText = "Queued";
                                    break;
                                case OrgQueueStatus.Paused:
                                    itemText = "Paused";
                                    if (OrgItem.Progress > 0)
                                        itemText += " - " + OrgItem.Progress.ToString() + "%";
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
                            itemText = OrgItem.Number.ToString();
                            break;
                        default:
                            throw new Exception("Unknown column type");
                    }

                    // Add text to listview
                    if (firstItem)
                    {
                        firstItem = false;

                        // If check state is set for the OrgItem than apply that to the listview item
                        bool resultsCheckSet = OrgItem.Check != CheckState.Indeterminate;
                        bool check;
                        Color backColor = Color.Transparent;
                        Color foreColor = Color.Black;
                        if (resultsCheckSet)
                            check = OrgItem.Check == CheckState.Checked;
                        else
                            check = true;

                        // Set listview item background color based on action
                        switch (OrgItem.Action)
                        {
                            case OrgAction.Copy:
                            case OrgAction.Move:
                                if (OrgItem.QueueStatus != OrgQueueStatus.Paused && OrgItem.Progress > 0 && !OrgItem.ActionComplete)
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
                            case OrgAction.Processing:
                                check = false;
                                foreColor = Color.Gray;
                                break;
                            case OrgAction.TBD:
                                check = false;
                                foreColor = Color.LightGray;
                                break;
                            default:
                                // Check only allowed on actions above this
                                check = false;
                                break;
                        }

                        if (OrgItem.QueueStatus == OrgQueueStatus.Failed)
                            backColor = Color.Red;
                        else if (OrgItem.QueueStatus == OrgQueueStatus.Cancelled)
                            backColor = Color.Yellow;
                        else if (OrgItem.QueueStatus == OrgQueueStatus.Completed)
                            backColor = Color.White;

                        // Set text
                        if (ListItem.Text != itemText)
                            ListItem.Text = itemText;

                        // Apply check to item
                        if (ListItem.Checked != check)
                            ListItem.Checked = check;

                        // Apply background color to item
                        if (backColor != Color.Transparent && ListItem.BackColor != backColor)
                            ListItem.BackColor = backColor;
                        if (ListItem.ForeColor != foreColor)
                            ListItem.ForeColor = foreColor;
                    }
                    else if (ListItem.SubItems.Count > subItemCount)
                    {
                        if (ListItem.SubItems[subItemCount].Text != itemText)
                            ListItem.SubItems[subItemCount].Text = itemText;
                    }
                    else
                        ListItem.SubItems.Add(itemText);

                    // Increment subitem count
                    ++subItemCount;
                }
            }
        }

        #region Sort

        /// <summary>
        /// Sorts a list of items based on a specific data (column) type.
        /// </summary>
        /// <param name="orgItems">List of items to be sorted</param>
        /// <param name="sortType">Column type to sort by</param>
        public static void Sort(List<OrgListItem> orgItems, OrgColumnType sortType, bool ascending)
        {
            OrgItem.AscendingSort = ascending;

            // Sort the Scan results (so that indices from that will still match listview after sort)
            switch (sortType)
            {
                case OrgColumnType.DateTime:
                    orgItems.Sort(
                        delegate(OrgListItem p1, OrgListItem p2)
                        {
                            return OrgItem.CompareByDateTime(p1.OrgItem, p2.OrgItem);
                        }
                    );
                    break;
                case OrgColumnType.Show:
                    orgItems.Sort(
                        delegate(OrgListItem p1, OrgListItem p2)
                        {
                            return OrgItem.CompareByShowName(p1.OrgItem, p2.OrgItem);
                        }
                    );
                    break;
                case OrgColumnType.Season:
                    orgItems.Sort(
                        delegate(OrgListItem p1, OrgListItem p2)
                        {
                            return OrgItem.CompareBySeasonNumber(p1.OrgItem, p2.OrgItem);
                        }
                    );
                    break;
                case OrgColumnType.Episode:
                    orgItems.Sort(
                        delegate(OrgListItem p1, OrgListItem p2)
                        {
                            return OrgItem.CompareByEpisodeNumber(p1.OrgItem, p2.OrgItem);
                        }
                    );
                    break;
                case OrgColumnType.Movie:
                    orgItems.Sort(
                        delegate(OrgListItem p1, OrgListItem p2)
                        {
                            return OrgItem.CompareByMovie(p1.OrgItem, p2.OrgItem);
                        }
                    );
                    break;
                case OrgColumnType.Source_Folder:
                    orgItems.Sort(
                        delegate(OrgListItem p1, OrgListItem p2)
                        {
                            return OrgItem.CompareBySourceFolder(p1.OrgItem, p2.OrgItem);
                        }
                    );
                    break;
                case OrgColumnType.Source_File:
                    orgItems.Sort(
                        delegate(OrgListItem p1, OrgListItem p2)
                        {
                            return OrgItem.CompareBySourceFile(p1.OrgItem, p2.OrgItem);
                        }
                    );
                    break;
                case OrgColumnType.Category:
                    orgItems.Sort(
                        delegate(OrgListItem p1, OrgListItem p2)
                        {
                            return OrgItem.CompareByCategory(p1.OrgItem, p2.OrgItem);
                        }
                    );
                    break;
                case OrgColumnType.Status:
                    orgItems.Sort(
                        delegate(OrgListItem p1, OrgListItem p2)
                        {
                            return OrgItem.CompareByStatus(p1.OrgItem, p2.OrgItem);
                        }
                    );
                    break;
                case OrgColumnType.Action:
                    orgItems.Sort(
                        delegate(OrgListItem p1, OrgListItem p2)
                        {
                            return OrgItem.CompareByAction(p1.OrgItem, p2.OrgItem);
                        }
                    );
                    break;
                case OrgColumnType.Destination_Folder:
                    orgItems.Sort(
                        delegate(OrgListItem p1, OrgListItem p2)
                        {
                            return OrgItem.CompareByDestinationFolder(p1.OrgItem, p2.OrgItem);
                        }
                    );
                    break;
                case OrgColumnType.Destination_File:
                    orgItems.Sort(
                        delegate(OrgListItem p1, OrgListItem p2)
                        {
                            return OrgItem.CompareByDestinationFile(p1.OrgItem, p2.OrgItem);
                        }
                    );
                    break;
                case OrgColumnType.Number:
                    orgItems.Sort(
                        delegate(OrgListItem p1, OrgListItem p2)
                        {
                            return OrgItem.CompareByNumber(p1.OrgItem, p2.OrgItem);
                        }
                    );
                    break;
                default:
                    throw new Exception("Unknown column type");
            }
        }

        #endregion
    }
}
