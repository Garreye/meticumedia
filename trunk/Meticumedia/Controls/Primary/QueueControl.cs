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
            lvQueue.SetColumns(OrgItemColumnSetup.Queue);

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
            ContentControl.ItemsToQueue += new EventHandler<ItemsToQueueArgs>(HandleItemsToQueue);
            LogControl.ItemsToQueue += new EventHandler<ItemsToQueueArgs>(HandleItemsToQueue);

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
                this.QueueItems = new List<OrgItem>();
                foreach (OrgItem item in items)
                    if (item.QueueStatus != OrgQueueStatus.Completed && item.QueueStatus != OrgQueueStatus.Failed && item.QueueStatus != OrgQueueStatus.Cancelled)
                        this.QueueItems.Add(item);
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

        /// <summary>+.
        /// 
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
            OrgItem newItem = new OrgItem(item);
            newItem.QueueStatus = OrgQueueStatus.Enabled;
            if (QueueItemsComplete != null)
                QueueItemsComplete(this, new QueueItemsCompleteArgs(newItem));
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


        /// <summary>
        /// Clear button removes finished items from queue
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearQueue();
        }

        private void chkAutoClear_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoClear.Checked)
                StartQueue();
            Settings.GuiControl.AutoClearCompleted = chkAutoClear.Checked;
            Settings.Save();

        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates saved interface properties from settings
        /// </summary>
        public void UpdateFromSettings()
        {
            chkAutoClear.Checked = Settings.GuiControl.AutoClearCompleted;
            chkAutoClear.CheckedChanged += chkAutoClear_CheckedChanged;
        }

        /// <summary>
        /// Add items to queue.
        /// </summary>
        /// <param name="items">Items to add to queue</param>
        public void QueueItems(List<OrgItem> items)
        {
            // Get sender
            if (this.Parent is TabPage && this.Parent.Parent is TabControl)
                ((TabControl)this.Parent.Parent).SelectedTab = ((TabPage)this.Parent);

            // Add each item to end of queue
            //lock (queueLock)
            {
                foreach (OrgItem item in items)
                    lvQueue.AddItem(item);

                // Refresh display
                DisplayQueue();
            }

            // Trigger queue items changed event
            OnQueueItemsChanged(lvQueue.GetOrgItems());

            // Run queue
            StartQueue();
        }

        /// <summary>
        /// Diplays queue in queue listview
        /// </summary>
        private void DisplayQueue()
        {
            lvQueue.UpdateDisplay();
        }


        /// <summary>
        /// Updates queue items in listview (doesn't clear and re-add, simply updates fields)
        /// </summary>
        public void UpdateQueue()
        {
            lvQueue.UpdateDisplay();
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
            List<OrgItem> selItems = lvQueue.GetSelectedOrgItems();
            for (int i = 0; i < selItems.Count; i++)
            {
                if (selItems[i].QueueStatus == OrgQueueStatus.Enabled)
                    selItems[i].QueueStatus = OrgQueueStatus.Paused;
            }

            if (selItems.Count > 0)
                lvQueue.UpdateDisplay();
            lvQueue.Focus();
        }

        /// <summary>
        /// Unpauses all items selected in queue listview
        /// </summary>
        private void ResumeSelected()
        {
            // Deassert paused property on all selected items and save selection
            List<OrgItem> selItems = lvQueue.GetSelectedOrgItems();
            for (int i = 0; i < selItems.Count; i++)
            {
                if (selItems[i].QueueStatus == OrgQueueStatus.Paused)
                    selItems[i].QueueStatus = OrgQueueStatus.Enabled;
            }

            // Refresh queue listview
            if (lvQueue.SelectedIndices.Count > 0)
                    DisplayQueue();
            lvQueue.Focus();
            
            // Run queue
            StartQueue();
        }

        /// <summary>
        /// Cancels all items selected in queue listview
        /// </summary>
        private void CancelSelected()
        {
            // Confirm cancel with user
            if (MessageBox.Show("Are you sure you want to remove the selected items from the queue?", "Sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {                
                // Remove all selected items from queue
                lvQueue.RemoveSelectedItems();

                // Disable buttons
                SetItemButtonEnables(false);

                // Trigger queue items changed event
                OnQueueItemsChanged(lvQueue.GetOrgItems());
            }
        }
        
        /// <summary>
        /// Up button moves selected items in queue up by one.
        /// </summary>
        private void MoveUpSelected()
        {
            // Move each selected item up by one
            lvQueue.MoveUpSelectedItems();
            lvQueue.Focus();

        }

        /// <summary>
        /// Moves selected items in queue down by one.
        /// </summary>
        private void MoveDownSelected()
        {
            // Move each selected item down by one
            lvQueue.MoveDownSelectedItems();
            lvQueue.Focus();
        }

        /// <summary>
        /// Move all selected items to top of queue.
        /// </summary>
        private void MoveToTopSelected()
        {
            // Move selected item to top
            lvQueue.MoveSelectedItemToTop();
            lvQueue.Focus();
        }

        /// <summary>
        /// Move all selected item to bottom of queueu
        /// </summary>
        private void MoveToBottomSelected()
        {
            // Move selected items to bottom
            lvQueue.MoveSelectedItemToBottom();
            lvQueue.Focus();
        }

        /// <summary>
        /// Clear finished items from queue
        /// </summary>
        private void ClearQueue()
        {
            List<OrgItem> items = lvQueue.GetOrgItems();
            for (int i = items.Count - 1; i >= 0; i--)
            {
                // Remove completed/failed items
                if (items[i].QueueStatus == OrgQueueStatus.Failed || items[i].QueueStatus == OrgQueueStatus.Completed || items[i].QueueStatus == OrgQueueStatus.Cancelled)
                    RemoveQueueItem(items[i]);
            }
            OnQueueItemsChanged(lvQueue.GetOrgItems());
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
            List<OrgItem> queueItems = lvQueue.GetOrgItems();
            if (queueItems.Count == 0)
                return;

            // Give some time for display to update
            Thread.Sleep(1000);

            // Get next queue item
            while (true)
            {
                // Refresh items
                queueItems = lvQueue.GetOrgItems();
                if (queueItems.Count == 0)
                    break;

                // Check if paused
                if (!OrgItem.QueuePaused)
                {
                    // Get next active item
                    OrgItem item = queueItems[0];
                    int index = -1;

                    for (int i = 0; i < queueItems.Count; i++)
                        if (queueItems[i].QueueStatus == OrgQueueStatus.Enabled)
                        {
                            item = queueItems[i];
                            index = i;
                            break;
                        }
                        else if (RemoveQueueItemIfNeeded(queueItems[i]))
                            i--;

                    // Refresh queue
                    this.Invoke((MethodInvoker)delegate
                    {
                        UpdateQueue();
                    });

                    // If found item is paused, sleep then retry
                    if (item.QueueStatus != OrgQueueStatus.Enabled || index == -1)
                        break;

                    // Perform action
                    item.ActionProgressChanged += new EventHandler<ProgressChangedEventArgs>(item_ActionProgressChanged);
                    item.PerformAction();
                    item.ActionProgressChanged -= new EventHandler<ProgressChangedEventArgs>(item_ActionProgressChanged);

                    if (item.QueueStatus == OrgQueueStatus.Completed)
                        OnQueueItemsComplete(item);

                    // Remove item
                    this.Invoke((MethodInvoker)delegate
                    {
                        RemoveQueueItemIfNeeded(item);
                    });
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


        private bool RemoveQueueItemIfNeeded(OrgItem item)
        {
            bool doRemove = item.QueueStatus == OrgQueueStatus.Completed && chkAutoClear.Checked;
            if (doRemove)
            {
                RemoveQueueItem(item);

                // Refresh queue items
                this.Invoke((MethodInvoker)delegate
                {
                    DisplayQueue();
                });
            }

            OnQueueItemsChanged(lvQueue.GetOrgItems());
            return doRemove;
        }


        private void RemoveQueueItem(OrgItem item)
        {
            this.Invoke((MethodInvoker)delegate
            {
                lvQueue.RemoveItem(item);
            });
        }

        /// <summary>
        /// Update progress bar when queue item action progress changes
        /// </summary>
        /// <param name="sender">OrgItem</param>
        /// <param name="e">Progress args</param>
        private void item_ActionProgressChanged(object sender, ProgressChangedEventArgs e)
        {
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
            string msg = actionStr + " file '" + Path.GetFileName(item.SourcePath) + "'";
            
            
            if (e.ProgressPercentage == pbTotal.Value && msg == pbTotal.Message)
                return;
            
            this.Invoke((MethodInvoker)delegate
            {
                // Limit progress value to 100
                int progress = Math.Min(e.ProgressPercentage, 100);

                // Set progress bar value
                this.pbTotal.Value = progress;

                this.pbTotal.Message = msg;

                // Update percent in listview
                UpdateQueue();

            });
        }

        #endregion
    }
}
