using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Meticumedia.Classes;
using Meticumedia.WPF;

namespace Meticumedia.Controls
{
    public class QueueControlViewModel: ProgressViewModel
    {
        #region Events

        /// <summary>
        /// Indicates that queue items have been added/removed/modified
        /// </summary>
        public static event EventHandler<QueueItemsChangedArgs> QueueItemsChanged;

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
        protected static void OnQueueItemsChanged(object sender, ObservableCollection<OrgItem> queueItems)
        {
            if (QueueItemsChanged != null)
                QueueItemsChanged(sender, new QueueItemsChangedArgs(queueItems.ToList()));
        }

        /// <summary>
        /// Indicates that an item in the complete has been completed.
        /// </summary>
        public static event EventHandler<QueueItemsCompleteArgs> QueueItemsComplete;

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

        #region Commands

        private ICommand pauseQueueCommand;
        public ICommand PauseQueueCommand
        {
            get
            {
                if (pauseQueueCommand == null)
                {
                    pauseQueueCommand = new RelayCommand(
                        param => this.PauseQueue(),
                        param => this.CanDoPauseQueueCommand()
                    );
                }
                return pauseQueueCommand;
            }
        }

        private bool CanDoPauseQueueCommand()
        {
            return true;
        }

        private ICommand pauseSelectedCommand;
        public ICommand PauseSelectedCommand
        {
            get
            {
                if (pauseSelectedCommand == null)
                {
                    pauseSelectedCommand = new RelayCommand(
                        param => this.PauseSelected(),
                        param => this.CanDoPauseSelectedCommand()
                    );
                }
                return pauseSelectedCommand;
            }
        }

        private bool CanDoPauseSelectedCommand()
        {
            return true;
        }

        private ICommand resumeSelectedCommand;
        public ICommand ResumeSelectedCommand
        {
            get
            {
                if (resumeSelectedCommand == null)
                {
                    resumeSelectedCommand = new RelayCommand(
                        param => this.ResumeSelected(),
                        param => this.CanDoResumeSelectedCommand()
                    );
                }
                return resumeSelectedCommand;
            }
        }

        private bool CanDoResumeSelectedCommand()
        {
            return true;
        }

        private ICommand cancelSelectedCommand;
        public ICommand CancelSelectedCommand
        {
            get
            {
                if (cancelSelectedCommand == null)
                {
                    cancelSelectedCommand = new RelayCommand(
                        param => this.CancelSelected(),
                        param => this.CanDoCancelSelectedCommand()
                    );
                }
                return cancelSelectedCommand;
            }
        }

        private bool CanDoCancelSelectedCommand()
        {
            return true;
        }

        private ICommand moveUpSelectedCommand;
        public ICommand MoveUpSelectedCommand
        {
            get
            {
                if (moveUpSelectedCommand == null)
                {
                    moveUpSelectedCommand = new RelayCommand(
                        param => this.MoveUpSelectedItems(),
                        param => this.CanDoMoveUpSelectedCommand()
                    );
                }
                return moveUpSelectedCommand;
            }
        }

        private bool CanDoMoveUpSelectedCommand()
        {
            return true;
        }

        private ICommand moveDownSelectedCommand;
        public ICommand MoveDownSelectedCommand
        {
            get
            {
                if (moveDownSelectedCommand == null)
                {
                    moveDownSelectedCommand = new RelayCommand(
                        param => this.MoveDownSelectedItems(),
                        param => this.CanDoMoveDownSelectedCommand()
                    );
                }
                return moveDownSelectedCommand;
            }
        }

        private bool CanDoMoveDownSelectedCommand()
        {
            return true;
        }

        private ICommand moveToTopSelectedCommand;
        public ICommand MoveToTopSelectedCommand
        {
            get
            {
                if (moveToTopSelectedCommand == null)
                {
                    moveToTopSelectedCommand = new RelayCommand(
                        param => this.MoveSelectedItemToTop(),
                        param => this.CanDoMoveToTopSelectedCommand()
                    );
                }
                return moveToTopSelectedCommand;
            }
        }

        private bool CanDoMoveToTopSelectedCommand()
        {
            return true;
        }

        private ICommand moveToBottomSelectedCommand;
        public ICommand MoveToBottomSelectedCommand
        {
            get
            {
                if (moveToBottomSelectedCommand == null)
                {
                    moveToBottomSelectedCommand = new RelayCommand(
                        param => this.MoveSelectedItemToBottom(),
                        param => this.CanDoMoveToBottomSelectedCommand()
                    );
                }
                return moveToBottomSelectedCommand;
            }
        }

        private bool CanDoMoveToBottomSelectedCommand()
        {
            return true;
        }

        private ICommand clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (clearCommand == null)
                {
                    clearCommand = new RelayCommand(
                        param => this.ClearQueue(),
                        param => this.CanDoClearCommand()
                    );
                }
                return clearCommand;
            }
        }

        private bool CanDoClearCommand()
        {
            return true;
        }


        #endregion

        #region Properties

        public ObservableCollection<OrgItem> QueueItems { get; set; }

        public IList SelectedQueueItems { get; set; }

        public bool AutoClearCompleted
        {
            get
            {
                return autoClearComplete;
            }
            set
            {
                autoClearComplete = value;
                OnPropertyChanged(this, "AutoClearCompleted");
            }
        }
        private bool autoClearComplete = true;

        public enum PauseButtonStates { Pause, Resume };

        public PauseButtonStates PauseButtonState
        {
            get
            {
                return pauseButtonState;
            }
            set
            {
                pauseButtonState = value;
                OnPropertyChanged(this, "PauseButtonState");
            }
        }
        private PauseButtonStates pauseButtonState = PauseButtonStates.Pause;


        #endregion

        #region Variables

        private BackgroundWorker queueWorker;

        #endregion

        #region Constructor

        public QueueControlViewModel()
        {
            this.QueueItems = new ObservableCollection<OrgItem>();
            this.QueueItems.CollectionChanged += QueueItems_CollectionChanged;

            // Initialize queue worker
            queueWorker = new BackgroundWorker();
            queueWorker.WorkerReportsProgress = true;
            queueWorker.WorkerSupportsCancellation = true;
            queueWorker.DoWork += new DoWorkEventHandler(queueWorker_DoWork);

            // Set queue to running at startup
            OrgItem.QueuePaused = false;

            // Register to queuing event
            ScanControlViewModel.ItemsToQueue += HandleItemsToQueue;
            ContentCollectionControlViewModel.ItemsToQueue += HandleItemsToQueue;
            ContentCollectionControlViewModel.ItemsToQueue += HandleItemsToQueue;
            //LogControl.ItemsToQueue += new EventHandler<ItemsToQueueArgs>(HandleItemsToQueue);

        }       

        #endregion

        #region Event Handlers

        void QueueItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnQueueItemsChanged(this, this.QueueItems);
        } 

        /// <summary>
        /// Handles items to be queue events
        /// </summary>
        void HandleItemsToQueue(object sender, ItemsToQueueArgs e)
        {
            AddItemsToQueueSafe(e.QueueItems);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Toggles pause/go on entire queue
        /// </summary>
        private void PauseQueue()
        {
            if (PauseButtonState == PauseButtonStates.Pause)
            {
                OrgItem.QueuePaused = true;
                PauseButtonState = PauseButtonStates.Resume;
            }
            else
            {
                
                OrgItem.QueuePaused = false;
                PauseButtonState = PauseButtonStates.Pause;
            }
        }

        /// <summary>
        /// Updates saved interface properties from settings
        /// </summary>
        public void UpdateFromSettings(List<OrgItem> items)
        {
            this.AutoClearCompleted = Settings.GuiControl.AutoClearCompleted;
            //chkAutoClear.CheckedChanged += chkAutoClear_CheckedChanged;
        }

        private void AddItemsToQueueSafe(List<OrgItem> items)
        {
            if (App.Current.Dispatcher.CheckAccess())
                AddItemsToQueue(items);
            else
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    AddItemsToQueue(items);
                });
        }

        /// <summary>
        /// Add items to queue.
        /// </summary>
        /// <param name="items">Items to add to queue</param>
        private void AddItemsToQueue(List<OrgItem> items)
        {
            // Add each item to end of queue
            foreach (OrgItem item in items)
                this.QueueItems.Add(item);

            // Run queue
            StartQueue();
        }


        /// <summary>
        /// Set queue control button enables.
        /// </summary>
        /// <param name="enable">Value to set enables to</param>
        private void SetItemButtonEnables(bool enable)
        {
            //btnPause.Enabled = enable;
            //btnPlay.Enabled = enable;
            //btnCancel.Enabled = enable;
            //btnMoveUp.Enabled = enable;
            //btnMoveDown.Enabled = enable;
            //btnMoveTop.Enabled = enable;
            //btnMoveBottom.Enabled = enable;
        }

        /// <summary>
        /// Pauses all item selected in queue listview
        /// </summary>
        private void PauseSelected()
        {
            if (this.SelectedQueueItems == null)
                return;
            
            for (int i = 0; i < this.SelectedQueueItems.Count; i++)
            {
                if (((OrgItem)this.SelectedQueueItems[i]).QueueStatus == OrgQueueStatus.Enabled)
                    ((OrgItem)this.SelectedQueueItems[i]).QueueStatus = OrgQueueStatus.Paused;
            }
            //lvQueue.Focus();
        }

        /// <summary>
        /// Unpauses all items selected in queue listview
        /// </summary>
        private void ResumeSelected()
        {
            if (this.SelectedQueueItems == null)
                return;
            
            // Deassert paused property on all selected items and save selection
            for (int i = 0; i < this.SelectedQueueItems.Count; i++)
            {
                if (((OrgItem)this.SelectedQueueItems[i]).QueueStatus == OrgQueueStatus.Paused)
                    ((OrgItem)this.SelectedQueueItems[i]).QueueStatus = OrgQueueStatus.Enabled;
            }

            // Refresh queue listview
            //lvQueue.Focus();

            // Run queue
            StartQueue();
        }

        /// <summary>
        /// Cancels all items selected in queue listview
        /// </summary>
        private void CancelSelected()
        {
            if (this.SelectedQueueItems == null)
                return;
            
            // Confirm cancel with user
            if (MessageBox.Show("Are you sure you want to remove the selected items from the queue?", "Sure?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                // Remove all selected items from queue
                for (int i = 0; i < this.SelectedQueueItems.Count; i++)
                    this.QueueItems.Remove((OrgItem)this.SelectedQueueItems[i]);

                // Disable buttons
                //SetItemButtonEnables(false);
            }
        }

        private void MoveUpSelectedItemsSafe()
        {
            if (App.Current.Dispatcher.CheckAccess())
                MoveUpSelectedItems();
            else
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    MoveUpSelectedItems();
                });
            }
        }

        public void MoveUpSelectedItems()
        {
            if (this.SelectedQueueItems == null)
                return;
            
            bool roomToMove = false;
            for (int i = 0; i < this.QueueItems.Count; i++)
            {
                OrgItem item = this.QueueItems[i];
                if (this.SelectedQueueItems.Contains(item))
                {
                    if (!roomToMove)
                        continue;

                    this.QueueItems.Remove(item);
                    this.QueueItems.Insert(i - 1, item);
                }
                else
                    roomToMove = true;
            }
        }

        private void MoveSelectedItemToTopSafe()
        {
            if (App.Current.Dispatcher.CheckAccess())
                MoveSelectedItemToTop();
            else
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    MoveSelectedItemToTop();
                });
            }
        }

        public void MoveSelectedItemToTop()
        {
            if (this.SelectedQueueItems == null)
                return;
            
            int topPos = 0;
            for (int i = 0; i < this.QueueItems.Count; i++)
            {
                OrgItem item = this.QueueItems[i];
                if (!this.SelectedQueueItems.Contains(item))
                    continue;

                if (i != topPos)
                {
                    this.QueueItems.Remove(item);
                    this.QueueItems.Insert(topPos++, item);
                }
            }
        }

        private void MoveDownSelectedItemsSafe()
        {
            if (App.Current.Dispatcher.CheckAccess())
                MoveDownSelectedItems();
            else
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    MoveDownSelectedItems();
                });
            }
        }

        public void MoveDownSelectedItems()
        {
            if (this.SelectedQueueItems == null)
                return;
            
            bool roomToMove = false;
            for (int i = this.QueueItems.Count - 1; i >= 0; i--)
            {
                OrgItem item = this.QueueItems[i];
                if (this.SelectedQueueItems.Contains(item))
                {
                    if (!roomToMove)
                        continue;

                    this.QueueItems.Remove(item);
                    this.QueueItems.Insert(i + 1, item);
                }
                else
                    roomToMove = true;
            }
        }

        private void MoveSelectedItemToBottomSafe()
        {
            if (App.Current.Dispatcher.CheckAccess())
                MoveSelectedItemToBottom();
            else
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    MoveSelectedItemToBottom();
                });
            }
        }

        public void MoveSelectedItemToBottom()
        {
            if (this.SelectedQueueItems == null)
                return;

            int botPos = this.QueueItems.Count - 1;
            for (int i = this.QueueItems.Count - 1; i >= 0; i--)
            {
                OrgItem item = this.QueueItems[i];
                if (!this.SelectedQueueItems.Contains(item))
                    continue;

                if (i != botPos)
                {
                    this.QueueItems.Remove(item);
                    this.QueueItems.Insert(botPos--, item);
                }
            }
        }


        private void ClearQueueSafe()
        {
            if (App.Current.Dispatcher.CheckAccess())
                ClearQueue();
            else
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    ClearQueue();
                });
            }
        }

        /// <summary>
        /// Clear finished items from queue
        /// </summary>
        private void ClearQueue()
        {
            this.QueueItems.Clear();
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
            if (this.QueueItems.Count == 0)
                return;

            // Give some time for display to update
            Thread.Sleep(1000);

            // Get next queue item
            while (true)
            {
                // Refresh items
                if (this.QueueItems.Count == 0)
                    break;

                // Check if paused
                if (!OrgItem.QueuePaused)
                {
                    // Get next active item
                    OrgItem item = this.QueueItems[0];
                    int index = -1;

                    for (int i = 0; i < this.QueueItems.Count; i++)
                        if (this.QueueItems[i].QueueStatus == OrgQueueStatus.Enabled)
                        {
                            item = this.QueueItems[i];
                            index = i;
                            break;
                        }
                        else if (RemoveQueueItemIfNeededSafe(this.QueueItems[i]))
                            i--;

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
                    RemoveQueueItemIfNeededSafe(item);
                }

                else
                    Thread.Sleep(500);
            }

            // Clear progress bar once all item completed
            this.UpdateProgressSafe(0, string.Empty);
        }

        private bool RemoveQueueItemIfNeededSafe(OrgItem item)
        {
            if (App.Current.Dispatcher.CheckAccess())
                return RemoveQueueItemIfNeeded(item);
            else
            {
                bool removed = false;
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    removed = RemoveQueueItemIfNeeded(item);
                });
                return removed;
            }
        }

        private bool RemoveQueueItemIfNeeded(OrgItem item)
        {
            bool doRemove = item.QueueStatus == OrgQueueStatus.Completed && this.AutoClearCompleted;
            if (doRemove)
            {
                RemoveQueueItem(item);
            }
            
            return doRemove;
        }

        private void RemoveQueueItemSafe(OrgItem item)
        {
            if (App.Current.Dispatcher.CheckAccess())
                RemoveQueueItem(item);
            else
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                     RemoveQueueItem(item);
                });
            }
        }

        private void RemoveQueueItem(OrgItem item)
        {
            this.QueueItems.Remove(item);
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
            string msg = actionStr + " file '" + System.IO.Path.GetFileName(item.SourcePath) + "'";


            if (e.ProgressPercentage == this.Progress && msg == this.ProgressMessage)
                return;

            this.UpdateProgressSafe(e.ProgressPercentage, msg);
        }

        #endregion

    }
}
