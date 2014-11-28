using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Meticumedia.Classes;
using Meticumedia.WPF;
using Xceed.Wpf.Toolkit;

namespace Meticumedia.Controls
{
    public class LogControlViewModel : OrgItemDisplayViewModel
    {
        #region Commands

        private ICommand removeSelectedItemsCommand;
        public ICommand RemoveSelectedItemsCommand
        {
            get
            {
                if (removeSelectedItemsCommand == null)
                {
                    removeSelectedItemsCommand = new RelayCommand(
                        param => this.RemoveSelectedItems()
                    );
                }
                return removeSelectedItemsCommand;
            }
        }

        private ICommand undoSelectedItemsCommand;
        public ICommand UndoSelectedItemsCommand
        {
            get
            {
                if (undoSelectedItemsCommand == null)
                {
                    undoSelectedItemsCommand = new RelayCommand(
                        param => this.UndoSelectedItems()
                    );
                }
                return undoSelectedItemsCommand;
            }
        }

        #endregion

        #region Constructor

        public LogControlViewModel() : base()
        {
            Organization.ActionLog.CollectionChanged += ActionLog_CollectionChanged;
        }

        private void ActionLog_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                    this.OrgItems.Clear();

                if (e.OldItems != null)
                    foreach (OrgItem remItem in e.OldItems)
                        OrgItems.Remove(remItem);
                if (e.NewItems != null)
                    foreach (OrgItem addItem in e.NewItems)
                        OrgItems.Add(addItem);
                
               // OrgItemsCollection.Refresh();
            });
        }

        #endregion

        #region Methods

        private void RemoveSelectedItems()
        {
            Organization.ActionLog.CollectionChanged -= ActionLog_CollectionChanged;
            for(int i=this.SelectedOrgItems.Count-1;i>=0;i--)
            {
                OrgItem item = this.SelectedOrgItems[i] as OrgItem;
                if (Organization.ActionLog.Contains(item))
                    Organization.ActionLog.Remove(item);
                this.OrgItems.Remove(item);
            }
            Organization.SaveActionLog();
            Organization.ActionLog.CollectionChanged += ActionLog_CollectionChanged;
        }

        private void UndoSelectedItems()
        {
            string message = string.Empty;
            List<OrgItem> undoActions = new List<OrgItem>();
            for (int i = 0; i < this.SelectedOrgItems.Count; i++)
            {
                // Get item
                OrgItem logItem = this.SelectedOrgItems[i] as OrgItem;

                // Create action with reversed source and destination
                OrgItem undoAction = new OrgItem(logItem);
                undoAction.SourcePath = logItem.DestinationPath;
                undoAction.DestinationPath = logItem.SourcePath;

                switch (logItem.Action)
                {
                    //If original file still exists, just delete the copy, otherwise move back to original
                    case OrgAction.Copy:
                        if (System.IO.File.Exists(logItem.SourcePath))
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
                        string fileName = logItem.Action == OrgAction.Delete ? logItem.SourcePath : logItem.DestinationPath;
                        message += "Action for file '" + fileName + "' cannot be undone -" + logItem.Action.ToString() + " actions are not reversible" + Environment.NewLine;
                        break;
                }

                // Check that undo item is valid
                if (undoAction != null)
                {
                    // Verify that file still exists
                    if (System.IO.File.Exists(undoAction.SourcePath))
                    {
                        // Check that file is already added to undo list
                        bool alreadyAdded = false;
                        foreach (OrgItem item in undoActions)
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

        #endregion

    }
}
