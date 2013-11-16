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

            // Register to action log loading complete
            Organization.ActionLogLoadComplete += new EventHandler(Organization_ActionLogLoadComplete);

            // Register loading on control
            this.Load += new EventHandler(LogControl_Load);

            lvLog.ContextMenu = contextMenu;
            contextMenu.Popup += new EventHandler(contextMenu_Popup);

            lvLog.SetColumns(OrgItemColumnSetup.Log);
            lvLog.ColumnSort = OrgColumnType.DateTime;

            // TODO: progress bar for loading
        } 

        #endregion

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
            contextMenu.MenuItems.Add("Undo Action(s)", new EventHandler(HandleUndo));
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

        private void HandleUndo(object sender, EventArgs e)
        {
            UndoSelected();
        }

        #endregion

        #region Variables

        /// <summary>
        /// Context menu for listview
        /// </summary>
        private ContextMenu contextMenu = new ContextMenu();

        /// <summary>
        /// Flag indicating that display of log item is required on load event
        /// Set if display was attempted befor handle for control was created
        /// </summary>
        private bool displayOnLoadRequired = false;

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// Displays items in log when control is loaded if needed
        /// </summary>
        private void LogControl_Load(object sender, EventArgs e)
        {
            if(displayOnLoadRequired)
                this.Invoke((MethodInvoker)delegate
                {
                    lvLog.SetOrgItems(Organization.ActionLog);
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
                lvLog.SetOrgItems(Organization.ActionLog);
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
        /// Create actions for undoing the selected items in list and add them to queue
        /// </summary>
        private void UndoSelected()
        {
            string message = string.Empty;
            List<OrgItem> undoActions = new List<OrgItem>();
            List<OrgItem> selItems = lvLog.GetSelectedOrgItems();
            for (int i = 0; i < selItems.Count; i++)
            {
                // Get item
                OrgItem logItem = selItems[i];

                // Create action with reversed source and destination
                OrgItem undoAction = new OrgItem(logItem);
                undoAction.SourcePath = logItem.DestinationPath;
                undoAction.DestinationPath = logItem.SourcePath;

                switch (logItem.Action)
                {
                    //If original file still exists, just delete the copy, otherwise move back to original
                    case OrgAction.Copy:
                        if (File.Exists(logItem.SourcePath))
                        {
                            undoAction.Action = OrgAction.Delete;
                            undoAction.DestinationPath = string.Empty;
                        }
                        else
                            undoAction.Action = OrgAction.Move;
                        break;

                    // Move and rename need no changes
                    case OrgAction.Move:
                    case OrgAction.Rename:
                        break;

                    // All other actions are not revesible
                    default:
                        undoAction = null;
                        message += "Action for file '" + logItem.DestinationPath + "' cannot be undone -" + logItem.Action.ToString() + " actions are not reversible" + Environment.NewLine;
                        break;
                }

                // Check that undo item is valid
                if (undoAction != null)
                {
                    // Verify that file still exists
                    if (File.Exists(undoAction.SourcePath))
                    {
                        // Check that file is already added to undo list
                        bool alreadyAdded = false;
                        foreach(OrgItem item in undoActions)
                            if (item.SourcePath == undoAction.SourcePath)
                            {
                                alreadyAdded = true;
                                break;
                            }
                        if (!alreadyAdded)
                            undoActions.Add(undoAction);
                    }
                    else
                        message += "Action for file '" + logItem.DestinationPath + "' cannot be undone - file no longer exists!" + Environment.NewLine;
                }
            }

            if (!string.IsNullOrWhiteSpace(message))
                MessageBox.Show(message.TrimEnd());

            if (undoActions.Count > 0)
                OnItemsToQueue(undoActions);
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
        /// Filter check box sets filter and updates
        /// </summary>
        private void chkFilter_CheckedChanged(object sender, EventArgs e)
        {
            // Action Filter
            lvLog.ActionFilter = OrgAction.Empty;
            if (chkMoveCopyFilter.Checked)
                lvLog.ActionFilter |= OrgAction.Move | OrgAction.Copy;
            if (chkRenameFilter.Checked)
                lvLog.ActionFilter |= OrgAction.Rename;
            if (chkDelFilter.Checked)
                lvLog.ActionFilter |= OrgAction.Delete;
            
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
            lvLog.InsertItem(0, item);
            //lvLog.SortItems();
            //lvLog.UpdateDisplay();
        }

        /// <summary>
        /// Displays log of actons in listview 
        /// </summary>
        public void DisplayLog()
        {
            //filteredLog = ApplyFilter(Organization.ActionLog);
            //OrgItem.AscendingSort = sortAscending;
            //OrgItem.Sort(filteredLog, sortType);

            lvLog.UpdateDisplay();
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
