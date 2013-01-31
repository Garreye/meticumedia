﻿// --------------------------------------------------------------------------------
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
using System.Timers;

namespace Meticumedia
{
    /// <summary>
    /// Control for displaying and modifyin queue of organization actions
    /// </summary>
    public partial class QueueControl : UserControl
    {
        #region Variables

        /// <summary>
        /// Organization actions currently in the queue
        /// </summary>
        private List<OrgItem> queueItems = new List<OrgItem>();

        /// <summary>
        /// Lock for access queueItems list
        /// </summary>
        private object queueLock = new object();

        /// <summary>
        /// Set of columns in the queue listview
        /// </summary>
        private Dictionary<OrgColumnType, OrgItemColumn> queueColumns;

        /// <summary>
        /// Worker for running queue actions
        /// </summary>
        private BackgroundWorker queueWorker;

        /// <summary>
        /// Context menu for queue list
        /// </summary>
        private ContextMenu contextMenu = new ContextMenu();

        /// <summary>
        /// Timer for toggling color of resume/pause button to highlight when queue is paused
        /// </summary>
        private System.Timers.Timer buttonTimer = new System.Timers.Timer(700);

        /// <summary>
        /// Default color of resume/pause button
        /// </summary>
        private Color basePauseButtonColor;
      
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public QueueControl()
        {
            InitializeComponent();

            // Build queue listview columns
            queueColumns = OrgItemListHelper.SetOrgItemListColumns(OrgItemListHelper.OrgColumnSetup.Queue, lvQueue);

            // Initialize queue worker
            queueWorker = new BackgroundWorker();
            queueWorker.WorkerReportsProgress = true;
            queueWorker.WorkerSupportsCancellation = true;
            queueWorker.DoWork += new DoWorkEventHandler(queueWorker_DoWork);

            // Set queue to running at startup
            OrgItem.QueuePaused = false;

            // Register to queuing event
            ScanControl.ItemsToQueue += new EventHandler<ItemsToQueueArgs>(HandleItemsToQueue);
            ContentListView.ItemsToQueue += new EventHandler<ItemsToQueueArgs>(HandleItemsToQueue);
            ShowsControl.ItemsToQueue += new EventHandler<ItemsToQueueArgs>(HandleItemsToQueue);

            // Register context menu
            lvQueue.ContextMenu = contextMenu;
            contextMenu.Popup += new EventHandler(contextMenu_Popup);

            // Setup pause button flash timer
            buttonTimer.Elapsed += new ElapsedEventHandler(buttonTimer_Elapsed);
            buttonTimer.Enabled = false;
        }

        #endregion

        #region Context Menu

        /// <summary>
        /// Build context menu for queue listview when it pops up
        /// </summary>
        private void contextMenu_Popup(object sender, EventArgs e)
        {
            BuildContextMenu();
        }

        /// <summary>
        /// Build context menu for queue listview based on what is selected
        /// </summary>
        private void BuildContextMenu()
        {
            // Clear item
            contextMenu.MenuItems.Clear();

            // Check for selection
            if (lvQueue.SelectedItems.Count == 0)
                return;

            // Get selected
            List<OrgItem> items = new List<OrgItem>();
            foreach (int index in lvQueue.SelectedIndices)
                items.Add(queueItems[index]);

            // TODO: check which of these need to be shown!
            contextMenu.MenuItems.Add("Pause Selected", new EventHandler(HandlePause));
            contextMenu.MenuItems.Add("Resume Selected", new EventHandler(HandleResume));
            contextMenu.MenuItems.Add("Cancel Selected", new EventHandler(HandleCancel));
            contextMenu.MenuItems.Add("Move Up", new EventHandler(HandleMoveUp));
            contextMenu.MenuItems.Add("Move Down", new EventHandler(HandleMoveDown));
            contextMenu.MenuItems.Add("Move to Top", new EventHandler(HandleMoveTop));
            contextMenu.MenuItems.Add("Move to Bottom", new EventHandler(HandleMoveBottom));
        }

        /// <summary>
        /// Handle pause selection form context menut
        /// </summary>
        private void HandlePause(object sender, EventArgs e)
        {
            PauseSelected();
        }

        /// <summary>
        /// Handle resume selection form context menu
        /// </summary>
        private void HandleResume(object sender, EventArgs e)
        {
            ResumeSelected();
        }

        /// <summary>
        /// Handle cancel selection form context menu
        /// </summary>
        private void HandleCancel(object sender, EventArgs e)
        {
            CancelSelected();
        }

        /// <summary>
        /// Handle move up selection form context menu
        /// </summary>
        private void HandleMoveUp(object sender, EventArgs e)
        {
            MoveUpSelected();
        }

        /// <summary>
        /// Handle move down selection form context menu
        /// </summary>
        private void HandleMoveDown(object sender, EventArgs e)
        {
            MoveDownSelected();
        }

        /// <summary>
        /// Handle move to top selection form context menu
        /// </summary>
        private void HandleMoveTop(object sender, EventArgs e)
        {
            MoveToTopSelected();
        }

        /// <summary>
        /// Handle move to bottom selection form context menu
        /// </summary>
        private void HandleMoveBottom(object sender, EventArgs e)
        {
            MoveToBottomSelected();
        }

        #endregion

        #region Events

        /// <summary>
        /// Indicates that queue items have been added/removed/modified
        /// </summary>
        public event EventHandler<QueueItemsChangedArgs> QueueItemsChanged;

        /// <summary>
        /// Event arguments for modification of queue items.
        /// </summary>
        public class QueueItemsChangedArgs : EventArgs
        {
            /// <summary>
            /// List of items currently in the queue
            /// </summary>
            public List<OrgItem> QueueItems { get; private set; }

            /// <summary>
            /// Constructor with queue items
            /// </summary>
            /// <param name="items">Item currently in the queu</param>
            public QueueItemsChangedArgs(List<OrgItem> items)
            {
                this.QueueItems = items;
            }
        }

        /// <summary>
        /// Triggers QueueItemsChanged event.
        /// </summary>
        /// <param name="items">List of items currently in the queue</param>
        protected void OnQueueItemsChanged(List<OrgItem> items)
        {
            if (QueueItemsChanged != null)
                QueueItemsChanged(this, new QueueItemsChangedArgs(items));
        }

        /// <summary>
        /// Indicates that an item in the complete has been completed.
        /// </summary>
        public event EventHandler<QueueItemsCompleteArgs> QueueItemsComplete;

        /// <summary>
        /// Event argument for queue item completed
        /// </summary>
        public class QueueItemsCompleteArgs : EventArgs
        {
            /// <summary>
            /// The organization item that has been completed
            /// </summary>
            public OrgItem CompleteItem { get; private set; }

            /// <summary>
            /// Constructor with completed item
            /// </summary>
            /// <param name="item">The organization item that has been completed</param>
            public QueueItemsCompleteArgs(OrgItem item)
            {
                this.CompleteItem = item;
            }
        }

        /// <summary>
        /// Triggers QueueItemsComplete event.
        /// </summary>
        /// <param name="item"></param>
        protected void OnQueueItemsComplete(OrgItem item)
        {
            if (QueueItemsComplete != null)
                QueueItemsComplete(this, new QueueItemsCompleteArgs(item));
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles items to be queue events
        /// </summary>
        void HandleItemsToQueue(object sender, ItemsToQueueArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                QueueItems(e.QueueItems);
            });
        }

        /// <summary>
        /// Selecting a queue item sets enables for control buttons.
        /// </summary>
        private void lvQueue_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetItemButtonEnables(lvQueue.SelectedItems.Count > 0);
        }

        /// <summary>
        /// Pause button pauses all item selected in queue listview
        /// </summary>
        private void btnPause_Click(object sender, EventArgs e)
        {
            PauseSelected();
        }

        /// <summary>
        /// Play button unpauses all items selected in queue listview
        /// </summary>
        private void btnPlay_Click(object sender, EventArgs e)
        {
            ResumeSelected();
        }

        /// <summary>
        /// Cancel button cancels all items selected in queue listview
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            CancelSelected();           
        }

        /// <summary>
        /// Up button moves selected items in queue up by one.
        /// </summary>
        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            MoveUpSelected();
        }

        /// <summary>
        /// Down button moves selected items in queue down by one.
        /// </summary>
        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            MoveDownSelected();
        }

        /// <summary>
        /// Move all selected items to top of queue.
        /// </summary>
        private void btnMoveTop_Click(object sender, EventArgs e)
        {
            MoveToTopSelected();
        }

        /// <summary>
        /// Handle move to bottom button event
        /// </summary>
        private void btnMoveBottom_Click(object sender, EventArgs e)
        {
            MoveToBottomSelected();
        }

        /// <summary>
        /// Queue pause/resume button pauses/resumes entire queue.
        /// </summary>
        private void btnQueuePause_Click(object sender, EventArgs e)
        {
            // Set static pause property for organization items
            if (btnQueuePause.Text == "Pause")
            {
                btnQueuePause.Text = "Paused";
                basePauseButtonColor = btnQueuePause.BackColor;
                OrgItem.QueuePaused = true;
                buttonTimer.Enabled = true;
            }
            else
            {
                btnQueuePause.Text = "Pause";
                OrgItem.QueuePaused = false;
                btnQueuePause.BackColor = basePauseButtonColor;
                buttonTimer.Enabled = false;
            }
        }

        /// <summary>
        /// Toggle color of run/pause button
        /// </summary>
        void buttonTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (btnQueuePause.BackColor == basePauseButtonColor)
                btnQueuePause.BackColor = Color.Yellow;
            else
                btnQueuePause.BackColor = basePauseButtonColor;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add items to queue.
        /// </summary>
        /// <param name="items">Items to add to queue</param>
        public void QueueItems(List<OrgItem> items)
        {
            if (this.Parent is TabPage && this.Parent.Parent is TabControl)
                ((TabControl)this.Parent.Parent).SelectedTab = ((TabPage)this.Parent);

            // Add each item to end of queue
            lock (queueLock)
                foreach (OrgItem item in items)
                    queueItems.Add(item);

            // Refresh display
            DisplayQueue();

            // Trigger queue items changed event
            OnQueueItemsChanged(queueItems);

            // Run queue
            StartQueue();
        }

        /// <summary>
        /// Diplays queue in queue listview
        /// </summary>
        private void DisplayQueue()
        {
            DisplayQueue(new int[1] { -1 });
        }

        /// <summary>
        /// Displays queue in listview with specific items selected.
        /// </summary>
        /// <param name="select"></param>
        private void DisplayQueue(int[] select)
        {
            lock (queueLock)
                OrgItemListHelper.DisplayOrgItemInList(queueItems, lvQueue, queueColumns, select, false);
        }

        /// <summary>
        /// Updates queue items in listview (doesn't clear and re-add, simply updates fields)
        /// </summary>
        public void UpdateQueue()
        {
            lock (queueLock)
                OrgItemListHelper.DisplayOrgItemInList(queueItems, lvQueue, queueColumns, new int[1] { -1 }, true);
        }

        /// <summary>
        /// Set queue control button enables.
        /// </summary>
        /// <param name="enable">Value to set enables to</param>
        private void SetItemButtonEnables(bool enable)
        {
            btnPause.Enabled = enable;
            btnPlay.Enabled = enable;
            btnCancel.Enabled = enable;
            btnMoveUp.Enabled = enable;
            btnMoveDown.Enabled = enable;
            btnMoveTop.Enabled = enable;
            btnMoveBottom.Enabled = enable;
        }

        /// <summary>
        /// Pauses all item selected in queue listview
        /// </summary>
        private void PauseSelected()
        {
            // Assert paused property on all selected items and save selection
            int[] selects = new int[lvQueue.SelectedIndices.Count];
            lock (queueLock)
            {
                for (int i = 0; i < lvQueue.SelectedIndices.Count; i++)
                {
                    queueItems[lvQueue.SelectedIndices[i]].Paused = true;
                    selects[i] = lvQueue.SelectedIndices[i];
                }
            }

            // Refresh queue listview
            if (lvQueue.SelectedIndices.Count > 0)
                DisplayQueue(selects);
        }

        /// <summary>
        /// Unpauses all items selected in queue listview
        /// </summary>
        private void ResumeSelected()
        {
            // Deassert paused property on all selected items and save selection
            int[] selects = new int[lvQueue.SelectedIndices.Count];
            for (int i = 0; i < lvQueue.SelectedIndices.Count; i++)
            {
                queueItems[lvQueue.SelectedIndices[i]].Paused = false;
                selects[i] = lvQueue.SelectedIndices[i];
            }

            // Refresh queue listview
            if (lvQueue.SelectedIndices.Count > 0)
                DisplayQueue(selects);
        }

        /// <summary>
        /// Cancels all items selected in queue listview
        /// </summary>
        private void CancelSelected()
        {
            // Confirm cancel with user
            if (MessageBox.Show("Are you sure you want to remove the selected items from the queue?", "Sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Remove all selected items from queue (and listview)
                lock (queueLock)
                {
                    for (int i = lvQueue.SelectedIndices.Count - 1; i >= 0; i--)
                    {
                        int index = lvQueue.SelectedIndices[i];
                        queueItems[index].CancelAction();
                        queueItems.RemoveAt(lvQueue.SelectedIndices[i]);
                    }

                    // Refresh display
                    DisplayQueue();
                }
                SetItemButtonEnables(false);

                // Trigger queue items changed event
                OnQueueItemsChanged(queueItems);
            }
        }
        
        /// <summary>
        /// Up button moves selected items in queue up by one.
        /// </summary>
        private void MoveUpSelected()
        {
            // Move each selected item up by one
            int[] selects = new int[lvQueue.SelectedIndices.Count];
            lock (queueLock)
            {
                for (int i = lvQueue.SelectedIndices.Count - 1; i >= 0; i--)
                {
                    // Can't move top item up!
                    if (lvQueue.SelectedIndices[i] == 0)
                    {
                        selects[i] = 0;
                        continue;
                    }

                    // Move item up
                    int index = lvQueue.SelectedIndices[i];
                    OrgItem item = queueItems[index];

                    queueItems.Remove(item);
                    queueItems.Insert(index - 1, item);

                    selects[i] = index - 1;
                }
            }

            // Reresh display
            if (lvQueue.SelectedIndices.Count > 0)
                DisplayQueue(selects);
        }

        /// <summary>
        /// Moves selected items in queue down by one.
        /// </summary>
        private void MoveDownSelected()
        {
            // Move each selected item down by one
            int[] selects = new int[lvQueue.SelectedIndices.Count];
            lock (queueLock)
            {
                for (int i = lvQueue.SelectedIndices.Count - 1; i >= 0; i--)
                {
                    // Can't move last item down!
                    if (lvQueue.SelectedIndices[i] >= lvQueue.Items.Count - 1)
                    {
                        selects[i] = lvQueue.Items.Count - 1;
                        continue;
                    }

                    // Move item down
                    int index = lvQueue.SelectedIndices[i];
                    OrgItem item = queueItems[index];
                    queueItems.Remove(item);
                    queueItems.Insert(index + 1, item);
                    selects[i] = index + 1;
                }
            }

            // Refresh display
            if (lvQueue.SelectedIndices.Count > 0)
                DisplayQueue(selects);
        }

        /// <summary>
        /// Move all selected items to top of queue.
        /// </summary>
        private void MoveToTopSelected()
        {
            // Move selected item to top (loop in reverse order to maintain order within selections)
            int[] selects = new int[lvQueue.SelectedIndices.Count];
            lock (queueLock)
            {
                for (int i = lvQueue.SelectedIndices.Count - 1; i >= 0; i--)
                {
                    // Move item to top
                    int index = lvQueue.SelectedIndices[i] + lvQueue.SelectedIndices.Count - 1 - i;
                    OrgItem item = queueItems[index];
                    queueItems.Remove(item);
                    queueItems.Insert(0, item);
                    selects[i] = lvQueue.SelectedIndices.Count - 1 - i;
                }
            }

            // Refresh display
            if (lvQueue.SelectedIndices.Count > 0)
                DisplayQueue(selects);
        }

        /// <summary>
        /// Move all selected item to bottom of queueu
        /// </summary>
        private void MoveToBottomSelected()
        {
            // Move selected items to bottom
            int[] selects = new int[lvQueue.SelectedIndices.Count];
            lock (queueLock)
            {
                for (int i = 0; i < lvQueue.SelectedIndices.Count; i++)
                {
                    // Move item to bottom
                    int index = lvQueue.SelectedIndices[i] - i;
                    OrgItem item = queueItems[index];
                    queueItems.Remove(item);
                    queueItems.Insert(lvQueue.Items.Count - 1, item);
                    selects[i] = lvQueue.Items.Count - 1 - i;
                }
            }

            // Refresh display
            if (lvQueue.SelectedIndices.Count > 0)
                DisplayQueue(selects);
        }

        #endregion        

        #region Processing

        /// <summary>
        /// Start processing item in queue (if not already running)
        /// </summary>
        private void StartQueue()
        {
            // Check if worker is already running
            if (!queueWorker.IsBusy)
                queueWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Work event for queue processing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void queueWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            QueueRun();
        }

        /// <summary>
        /// Processes items in queue.
        /// </summary>
        private void QueueRun()
        {
            // Check if queue is empty
            if (queueItems.Count == 0)
                return;

            // Give some time for display to update
            Thread.Sleep(1000);

            // Get next queue item
            while (queueItems.Count > 0)
            {
                // Check if paused
                if (!OrgItem.QueuePaused)
                {
                    // Get next unpaused item
                    OrgItem item = queueItems[0];
                    int index = 0;
                    lock (queueLock)
                    {
                        for (int i = 0; i < queueItems.Count; i++)
                            if (!queueItems[i].Paused)
                            {
                                item = queueItems[i];
                                index = i;
                                break;
                            }
                    }

                    // Refresh queue
                    this.Invoke((MethodInvoker)delegate
                    {
                        UpdateQueue();
                    });

                    // If found item is paused, sleep then retry
                    if (item.Paused)
                    {
                        Thread.Sleep(500);
                        continue;
                    }

                    // Perform action
                    item.ActionProgressChanged += new EventHandler<ProgressChangedEventArgs>(item_ActionProgressChanged);
                    item.PerformAction();
                    item.ActionProgressChanged -= new EventHandler<ProgressChangedEventArgs>(item_ActionProgressChanged);

                    // Remove item
                    if (item.ActionComplete)
                    {
                        lock (queueLock)
                        {
                            if (item.ActionSucess)
                                OnQueueItemsComplete(item);
                            queueItems.Remove(item);
                            this.Invoke((MethodInvoker)delegate
                            {
                                //DisplayQueue();
                                lvQueue.Items.RemoveAt(index);
                            });

                            OnQueueItemsChanged(queueItems);
                        }
                    }
                }
                else
                    Thread.Sleep(500);
            }

            // Clear progress bar once all item completed
            this.Invoke((MethodInvoker)delegate
            {
                this.pbTotal.Value = 0;
                this.pbTotal.Message = string.Empty;
            });
        }

        /// <summary>
        /// Update progress bar when queue item action progress changes
        /// </summary>
        /// <param name="sender">OrgItem</param>
        /// <param name="e">Progress args</param>
        private void item_ActionProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                // Limit progress value to 100
                int progress = Math.Min(e.ProgressPercentage, 100);

                // Set progress bar value
                this.pbTotal.Value = progress;

                // Set progress bar message
                OrgItem item = (OrgItem)sender;
                string actionStr = string.Empty;
                switch (item.Action)
                {
                    case OrgAction.Copy:
                        actionStr = "Copying";
                        break;
                    case OrgAction.Delete:
                        actionStr = "Deleting";
                        break;
                    case OrgAction.Move:
                        actionStr = "Moving";
                        break;
                    case OrgAction.Rename:
                        actionStr = "Renaming";
                        break;
                    default:
                        actionStr = "Processing";
                        break;
                }
                this.pbTotal.Message = actionStr + " file '" + Path.GetFileName(item.SourcePath) + "'";

                // Update percent in listview
                UpdateQueue();

            });
        }

        #endregion
    }
}