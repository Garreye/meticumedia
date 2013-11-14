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

        public List<OrgListItem> DisplayedListItems
        {
            get
            {
                List<OrgListItem> items = new List<OrgListItem>();
                foreach (OrgListItem item in orgListItems)
                    if (item.Displayed)
                        items.Add(item);
                return items;
            }
        }

        public List<OrgListItem> SelectedListItems
        {
            get
            {
                List<OrgListItem> items = new List<OrgListItem>();
                foreach (OrgListItem item in orgListItems)
                    if (item.ListItem.Selected)
                        items.Add(item);
                return items;
            }
        }

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

        public List<OrgItem> OrgItems
        {
            get
            {
                List<OrgItem> items = new List<OrgItem>();
                foreach (OrgListItem oli in this.Items)
                    items.Add(oli.OrgItem);
                return items;
            }
            set
            {
                ClearItems();
                foreach (OrgItem item in value)
                    AddItem(item);
            }
        }

        #endregion

        #region Constructor

        public OrgItemListView()
        {
            this.ColumnClick += OrgItemListView_ColumnClick;
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
            OrgListItem.Sort(orgListItems, lastClickedColumn, sortAcending);
            DisplayResults();
        }

        #endregion

        #region Display

        private void DisplayResults()
        {
            int addedCount = 0;
            for (int i = 0; i < orgListItems.Count; i++)
            {
                // Apply filter
                if (!Filter(orgListItems[i]))
                {
                    // TODO: item in list currently - remove it
                    
                    continue;
                }

                

                this.Items.Add(orgListItems[i].ListItem);
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
            this.orgListItems.Add(oli);
        }

        public void InsertItem(int index, OrgItem item)
        {
            OrgListItem oli = new OrgListItem(item, this.orgColumns);
            this.orgListItems.Insert(index, oli);
        }

        public void RemoveItem(OrgItem item)
        {
            for (int i = 0; i < this.orgListItems.Count; i++)
                if (this.orgListItems[i].OrgItem.SourcePath == item.SourcePath)
                {
                    RemoveItemAt(i);
                    break;
                }
        }

        public void RemoveItemAt(int index)
        {
            this.orgListItems.RemoveAt(index);
            this.Items.RemoveAt(index);
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
