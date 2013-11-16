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
    public class OrgItemListView : DoubleBufferedListView
    {
        #region Variables

        private object displayLock = new object();

        private List<OrgListItem> orgListItems = new List<OrgListItem>();

        /// <summary>
        /// Set of column currently in the listview
        /// </summary>
        private Dictionary<OrgColumnType, OrgItemColumn> orgColumns = new Dictionary<OrgColumnType, OrgItemColumn>();

        /// <summary>
        /// Column type listview is currently sorted on
        /// </summary>
        private OrgColumnType sortType = OrgColumnType.Source_Folder;

        /// <summary>
        /// Whether to sort item in list in ascending order (descending if false)
        /// </summary>
        private bool sortAcending = true;

        /// <summary>
        /// Last column clicked
        /// </summary>
        private OrgColumnType lastClickedColumn = 0;

        #endregion

        #region Properties

        public OrgColumnType ColumnSort
        {
            get { return sortType; }
            set
            {
                sortType = value;
                sortAcending = true;
            }
        }

        #endregion

        #region Constructor

        public OrgItemListView()
        {
            this.ColumnClick += OrgItemListView_ColumnClick;
            this.ActionFilter = OrgAction.All;
            this.StatusFilter = OrgStatus.All;
            this.CategoryFilter = FileCategory.All;
        }

        #endregion

        #region Filtering

        public OrgAction ActionFilter = OrgAction.All;

        public OrgStatus StatusFilter = OrgStatus.All;

        public FileCategory CategoryFilter = FileCategory.All;

        #endregion

        #region Event Handlers

        private void OrgItemListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Find the column type to sort by
            sortType = 0;
            foreach (OrgColumnType colType in Enum.GetValues(typeof(OrgColumnType)))
                if (orgColumns.ContainsKey(colType) && orgColumns[colType].Header.Index == e.Column)
                {
                    sortType = colType;
                    break;
                }

            // Set order
            if (lastClickedColumn == 0 || lastClickedColumn != sortType)
                sortAcending = true;
            else
                sortAcending = !sortAcending;
            lastClickedColumn = sortType;

            // Sort items and re-display
            SortItems();
        }

        #endregion

        #region Display

        public void UpdateDisplay()
        {
            int addCount = 0;
            for (int i = 0; i < orgListItems.Count; i++)
            {
                bool sel = orgListItems[i].Displayed && orgListItems[i].ListItem.Selected;
                orgListItems[i].UpdateListViewItem(orgColumns);
                
                // Apply filter
                if (!Filter(orgListItems[i]))
                {
                    orgListItems[i].Displayed = false;
                    if (this.Items.Contains(orgListItems[i].ListItem))
                        this.Items.Remove(orgListItems[i].ListItem);

                    continue;
                }

                orgListItems[i].Displayed = true;

                if (!this.Items.Contains(orgListItems[i].ListItem))
                {
                    this.Items.Insert(addCount, orgListItems[i].ListItem);
                }
                else if (orgListItems[i].ListItem.Index != addCount)
                {
                    this.Items.Remove(orgListItems[i].ListItem);
                    this.Items.Insert(addCount, orgListItems[i].ListItem);

                }
                if (sel)
                    this.Items[addCount].Selected = true;
                addCount++;
            }
        }

        private bool Filter(OrgListItem item)
        {
            // Action
            if ((item.OrgItem.Action & ActionFilter) == 0)
                return false;

            // Status
            if ((item.OrgItem.Status & StatusFilter) == 0)
                return false;

            // Category
            if ((item.OrgItem.Category & CategoryFilter) == 0)
                return false;

            return true;
        }

        #endregion

        #region Get/Set Items

        public List<OrgItem> GetOrgItems()
        {
            List<OrgItem> items = new List<OrgItem>();
            foreach (OrgListItem oli in orgListItems)
                items.Add(oli.OrgItem);
            return items;
        }

        public void SetOrgItems(List<OrgItem> items)
        {
            ClearItems();
            foreach (OrgItem item in items)
                AddItem(item);
        }

        public List<OrgItem> GetDisplayedOrgItems()
        {
            List<OrgItem> items = new List<OrgItem>();
            foreach (OrgListItem item in orgListItems)
                if (item.Displayed)
                    items.Add(item.OrgItem);
            return items;
        }

        public List<OrgItem> GetSelectedOrgItems()
        {
            List<OrgItem> items = new List<OrgItem>();
            List<OrgListItem> selListItems = GetSelectedListItems();
            foreach (OrgListItem item in selListItems)
                items.Add(item.OrgItem);
            return items;
        }

        public List<OrgListItem> GetDisplayedListItems()
        {
            List<OrgListItem> items = new List<OrgListItem>();
            foreach (OrgListItem item in orgListItems)
                if (item.Displayed)
                    items.Add(item);
            return items;
        }

        public List<OrgListItem> GetSelectedListItems()
        {
            List<OrgListItem> items = new List<OrgListItem>();

            List<OrgListItem> displayedItems = GetDisplayedListItems();
            for (int i = 0; i < this.SelectedIndices.Count; i++)
                items.Add(displayedItems[this.SelectedIndices[i]]);

            return items;
        }

        #endregion

        #region Item Checks

        /// <summary>
        /// Set scan item check to for all items with a specific action
        /// </summary>
        /// <param name="action">Action type for item to set check</param>
        /// <param name="check">Value to set checked to</param>
        public void SetCheckItems(OrgAction action, bool check)
        {
            SetCheckItems(new OrgAction[] { action }, check);
        }

        /// <summary>
        /// Set item check to for all items with a specific actions
        /// </summary>
        /// <param name="action">Action types for item to set check</param>
        /// <param name="check">Value to set checked to</param>
        public void SetCheckItems(OrgAction[] actions, bool check)
        {
            // Get column index for action
            int actionColIndex = orgColumns[OrgColumnType.Action].Header.Index;
            if (actionColIndex == -1)
                return;

            // Set check state for all items that match any of the action types
            lock (displayLock)
            {
                for (int i = 0; i < orgListItems.Count; i++)
                    foreach (OrgAction action in actions)
                        if (orgListItems[i].OrgItem.Action == action)
                        {
                            orgListItems[i].ListItem.Checked = check;
                            orgListItems[i].OrgItem.Check = check ? CheckState.Checked : CheckState.Unchecked;
                        }
            }
        }

        /// <summary>
        /// Get combined check state of all item of set of action types
        /// </summary>
        /// <param name="actions">Set of actions types to get check state from</param>
        /// <returns>Combined check state of all item match any of the specified actions</returns>
        public CheckState GetCheckState(OrgAction[] actions)
        {
            if (orgColumns == null)
                return CheckState.Unchecked;

            // Get column with action type in it
            int actionColIndex = orgColumns[OrgColumnType.Action].Header.Index;
            if (actionColIndex == -1)
                return CheckState.Unchecked;

            // Set booleans indicating if there are any checked/unchecked item it listview
            bool check = false;
            bool notCheck = false;
            foreach (ListViewItem item in this.Items)
                foreach (OrgAction action in actions)
                    if (item.SubItems.Count > 1 && item.SubItems[actionColIndex].Text == action.ToString())
                    {
                        if (item.Checked)
                            check = true;
                        else
                            notCheck = true;
                    };

            // Return combined check state
            if (check && !notCheck)
                return CheckState.Checked;
            else if (check && notCheck)
                return CheckState.Indeterminate;
            else
                return CheckState.Unchecked;
        }

        /// <summary>
        /// Get combined check state of all item of specific action types
        /// </summary>
        /// <param name="action">Actions type to get check state from</param>
        /// <returns>Combined check state of all item match the specified action</returns>
        public CheckState GetCheckState(OrgAction action)
        {
            return GetCheckState(new OrgAction[] { action });
        }

        #endregion

        #region Add/Remove Items Methods

        public void AddItem(OrgItem item)
        {
            OrgListItem oli = new OrgListItem(item, this.orgColumns);
            AddItem(oli);
        }

        public void AddItem(OrgListItem item)
        {
            this.orgListItems.Add(item);
            if (Filter(item))
            {
                item.Displayed = true;
                this.Items.Add(item.ListItem);
            }
        }

        public void InsertItem(int index, OrgItem item)
        {
            OrgListItem oli = new OrgListItem(item, this.orgColumns);
            InsertItem(index, oli);
        }

        public void InsertItem(int index, OrgListItem item)
        {
            this.orgListItems.Insert(index, item);
            if (Filter(item))
            {
                item.Displayed = true;
                this.Items.Insert(index, item.ListItem);
            }
        }

        public void RemoveItem(OrgItem item)
        {
            for (int i = 0; i < this.orgListItems.Count; i++)
                if (this.orgListItems[i].OrgItem == item)
                {
                    RemoveItemAt(i);
                    break;
                }
        }

        public void RemoveItem(OrgListItem item)
        {
            for (int i = 0; i < this.orgListItems.Count; i++)
                if (this.orgListItems[i] == item)
                {
                    RemoveItemAt(i);
                    break;
                }
        }

        public void RemoveItemAt(int index)
        {
            if (this.Items.Contains(orgListItems[index].ListItem))
                this.Items.Remove(orgListItems[index].ListItem);
            this.orgListItems[index].OrgItem.CancelAction();
            this.orgListItems.RemoveAt(index); 
        }

        public void SortItems()
        {
            SortItems(sortType, sortAcending);
        }

        public void SortItems(OrgColumnType type, bool ascending)
        {
            OrgListItem.Sort(orgListItems, type, ascending);
            UpdateDisplay();
        }

        public void MoveUpSelectedItems()
        {
            bool roomToMove = false;
            for (int i = 0; i < orgListItems.Count; i++)
            {
                if (orgListItems[i].ListItem.Selected)
                {
                    if (!roomToMove)
                        continue;

                    OrgListItem item = orgListItems[i];
                    RemoveItemAt(i);
                    InsertItem(i - 1, item);
                }
                else
                    roomToMove = true;
            }

        }

        public void MoveSelectedItemToTop()
        {
            int topPos = 0;
            for (int i = 0; i < orgListItems.Count; i++)
            {
                if (orgListItems[i].ListItem.Selected)
                {
                    if (i != topPos)
                    {
                        OrgListItem item = orgListItems[i];
                        RemoveItemAt(i);
                        InsertItem(topPos++, item);
                    }
                }
            }
        }

        public void MoveDownSelectedItems()
        {
            bool roomToMove = false;
            for (int i = orgListItems.Count-1; i >=0; i--)
            {
                if (orgListItems[i].ListItem.Selected)
                {
                    if (!roomToMove)
                        continue;

                    OrgListItem item = orgListItems[i];
                    RemoveItemAt(i);
                    InsertItem(i + 1, item);
                }
                else
                    roomToMove = true;
            }
        }

        public void MoveSelectedItemToBottom()
        {
            int botPos = orgListItems.Count - 1;
            for (int i = orgListItems.Count - 1; i >= 0; i--)
            {
                if (orgListItems[i].ListItem.Selected)
                {
                    if (i != botPos)
                    {
                        OrgListItem item = orgListItems[i];
                        RemoveItemAt(i);
                        InsertItem(botPos--, item);
                    }
                }
            }
        }

        public void RemoveSelectedItems()
        {
            List<OrgListItem> selItems = GetSelectedListItems();
            for (int i = selItems.Count - 1; i >= 0; i--)
                RemoveItem(selItems[i]);
        }

        public void ClearItems()
        {
            this.Items.Clear();
            orgListItems.Clear();
        }

        #endregion

        #region Columns

        /// <summary>
        /// Sets up the columns of a listview for displaying specific types of OrgItems
        /// </summary>
        /// <param name="setup">Setup type for selecting which column will be enabled</param>
        /// <param name="lv">The listview to setup columns for</param>
        /// <returns>Dictionary of column types and the column for that type that was set in the listview</returns>
        public void SetColumns(OrgItemColumnSetup setup)
        {
            // Initialize columns dictionary
            orgColumns.Clear();
            int columnOrder = 0;

            // Select which columns will be enabled based on setup
            OrgColumnType columnsToAdd = OrgColumnType.Source_Folder | OrgColumnType.Source_File | OrgColumnType.Action | OrgColumnType.Destination_Folder | OrgColumnType.Destination_File;
            if (setup == OrgItemColumnSetup.DirectoryScan || setup == OrgItemColumnSetup.MissingCheck)
                columnsToAdd |= OrgColumnType.Category;
            if (setup == OrgItemColumnSetup.MissingCheck)
                columnsToAdd |= OrgColumnType.Status | OrgColumnType.Show | OrgColumnType.Season | OrgColumnType.Episode;
            if (setup == OrgItemColumnSetup.Queue)
                columnsToAdd |= OrgColumnType.Progress;
            if (setup == OrgItemColumnSetup.Log)
                columnsToAdd |= OrgColumnType.DateTime;
            if (setup == OrgItemColumnSetup.RootFolderCheck)
                columnsToAdd |= OrgColumnType.Movie;

            // Add the enabled columns to the listview
            this.Columns.Clear();
            foreach (OrgColumnType type in Enum.GetValues(typeof(OrgColumnType)))
                if ((type & columnsToAdd) > 0)
                {
                    orgColumns.Add(type, new OrgItemColumn(type, columnOrder++));
                    this.Columns.Add(orgColumns[type].Header);
                }
        }

        #endregion
    }
}
