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
using System.Threading;

namespace Meticumedia
{
    /// <summary>
    /// Control for display log of actions performed by meticumedia.
    /// </summary>
    public partial class LogControl : UserControl
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LogControl()
        {
            InitializeComponent();

            // Setup columns in listview
            logColumns = OrgItemListHelper.SetOrgItemListColumns(OrgItemListHelper.OrgColumnSetup.Log, lvLog);

            // Register to action log loading complete
            Organization.ActionLogLoadComplete += new EventHandler(Organization_ActionLogLoadComplete);

            // Register loading on control
            this.Load += new EventHandler(LogControl_Load);

            lvLog.ContextMenu = contextMenu;
            contextMenu.Popup += new EventHandler(contextMenu_Popup);

            // TODO: progress bar for loading
        } 

        #endregion

        #region Context Menu

        /// <summary>
        /// Context menu is built on pop-up based on what is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenu_Popup(object sender, EventArgs e)
        {
            BuildContextMenu();
        }

        /// <summary>
        /// Builds context menu for log listview based on what is selected
        /// </summary>
        private void BuildContextMenu()
        {
            // Clear item
            contextMenu.MenuItems.Clear();

            // Check for selection
            if (lvLog.SelectedItems.Count == 0)
                return;

            // Get selected
            List<OrgItem> items = new List<OrgItem>();
            foreach (int index in lvLog.SelectedIndices)
                items.Add(Organization.ActionLog[index]);

            // Remove items from log
            contextMenu.MenuItems.Add("Remove Selected", new EventHandler(HandleRemove));
        }

        /// <summary>
        /// Handle remove selection from context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleRemove(object sender, EventArgs e)
        {
            RemoveSelected();
        }

        #endregion

        #region Variables

        /// <summary>
        /// Column contained in listview
        /// </summary>
        private Dictionary<OrgColumnType, OrgItemColumn> logColumns;

        /// <summary>
        /// Context menu for listview
        /// </summary>
        private ContextMenu contextMenu = new ContextMenu();

        /// <summary>
        /// Flag indicating that display of log item is required on load event
        /// Set if display was attempted befor handle for control was created
        /// </summary>
        private bool displayOnLoadRequired = false;

        /// <summary>
        /// Colum type that listview is currently sorted on.
        /// </summary>
        private OrgColumnType sortType = OrgColumnType.DateTime;

        /// <summary>
        /// Flag indicating whether sorting based on column clicked is currently set to ascending (used for toggling between ascending/descending)
        /// </summary>
        private bool sortAscending = false;

        /// <summary>
        /// Last column clicked in listview
        /// </summary>
        private OrgColumnType lastClickedColumn = 0;

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// Clicking on listview column sorts the list by value in column
        /// </summary>
        private void lvLog_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Find the column type to sort by
            sortType = 0;
            foreach (OrgColumnType colType in Enum.GetValues(typeof(OrgColumnType)))
                if (logColumns.ContainsKey(colType) && logColumns[colType].Header.Index == e.Column)
                {
                    sortType = colType;
                    break;
                }

            // Set order
            if (lastClickedColumn == 0 || lastClickedColumn != sortType)
                sortAscending = true;
            else
                sortAscending = !sortAscending;

            lastClickedColumn = sortType;

            // Sort items and re-display
            DisplayLog();
        }

        /// <summary>
        /// Displays items in log when control is loaded if needed
        /// </summary>
        private void LogControl_Load(object sender, EventArgs e)
        {
            if(displayOnLoadRequired)
                this.Invoke((MethodInvoker)delegate
                {
                    DisplayLog();
                });
        }

        /// <summary>
        /// Display action log items once loading is complete.
        /// </summary>
        private void Organization_ActionLogLoadComplete(object sender, EventArgs e)
        {
            // If handle not created yet than set flag indicating display is required
            if (!this.IsHandleCreated)
            {
                displayOnLoadRequired = true;
                return;
            }

            // Display items in log
            this.Invoke((MethodInvoker)delegate
            {
                DisplayLog();
            });
        }

        /// <summary>
        /// Delete button removes selected items from log (and thus listview)
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            RemoveSelected();
        }

        /// <summary>
        /// Removes selected item in listview from log
        /// </summary>
        private void RemoveSelected()
        {
            // Confirm removal
            if (MessageBox.Show("Are you sure you want to remove the selected items from the log?", "Sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int i = lvLog.SelectedIndices.Count - 1; i >= 0; i--)
                {
                    int index = lvLog.SelectedIndices[i];
                    Organization.RemoveLogItem(lvLog.SelectedIndices[i]);
                }
                DisplayLog();
            }
        }

        /// <summary>
        /// Clear button removes all items from log (and thus listview)
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            // Ask the user if they are sure they want to clear log
            if (MessageBox.Show("Are you sure you want to remove all items from the log?", "Sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                for (int i = lvLog.Items.Count - 1; i >= 0; i--)
                    Organization.RemoveLogItem(i);
                DisplayLog();
            }
        }

        /// <summary>
        /// Move/copy filter check box refresh list
        /// </summary>
        private void chkMoveCopyFilter_CheckedChanged(object sender, EventArgs e)
        {
            DisplayLog();
        }

        /// <summary>
        /// Rename filter check box refresh list
        /// </summary>
        private void chkRenameFilter_CheckedChanged(object sender, EventArgs e)
        {
            DisplayLog();
        }

        /// <summary>
        /// Delete filter check box refresh list
        /// </summary>
        private void chkDelFilter_CheckedChanged(object sender, EventArgs e)
        {
            DisplayLog();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds an action to log (and thus listview)
        /// </summary>
        /// <param name="item"></param>
        public void AddLogItems(OrgItem item)
        {
            Organization.AddLogItem(item);
            DisplayLog();
        }

        /// <summary>
        /// Displays log of actons in listview 
        /// </summary>
        public void DisplayLog()
        {
            List<OrgItem> filteredLog = ApplyFilter(Organization.ActionLog);
            OrgItem.AscendingSort = sortAscending;
            OrgItem.Sort(filteredLog, sortType);
            OrgItemListHelper.DisplayOrgItemInList(filteredLog, lvLog, logColumns);
        }

        /// <summary>
        /// Applies filters to items in listview
        /// </summary>
        /// <param name="unfilteredLog">All log items to be filtered</param>
        /// <returns>List of filtered items</returns>
        private List<OrgItem> ApplyFilter(List<OrgItem> unfilteredLog)
        {
            List<OrgItem> filteredLog = new List<OrgItem>();

            foreach (OrgItem item in unfilteredLog)
            {
                switch (item.Action)
                {
                    case OrgAction.Move:
                    case OrgAction.Copy:
                        if (!chkMoveCopyFilter.Checked)
                            continue;
                        break;
                    case OrgAction.Rename:
                        if (!chkRenameFilter.Checked)
                            continue;
                        break;
                    case OrgAction.Delete:
                        if (!chkDelFilter.Checked)
                            continue;
                        break;
                    default:
                        throw new Exception("Bad filter type");

                }
                filteredLog.Add(item);
            }

            return filteredLog;
        }

        #endregion        
    }
}
