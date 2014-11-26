using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Meticumedia.Classes;
using Meticumedia.Windows;
using Meticumedia.WPF;
using Ookii.Dialogs.Wpf;

namespace Meticumedia.Controls
{
    public class RootFolderControlViewModel : ViewModel
    {
        #region Properties

        public ContentRootFolderCollection RootFolders { get; set; }

        public ContentRootFolder SelectedRootFolder
        {
            get
            {
                return selectedRootFolder;
            }
            set
            {
                selectedRootFolder = value;
                OnPropertyChanged(this, "SelectedRootFolder");
            }
        }
        private ContentRootFolder selectedRootFolder;

        public ObservableCollection<string> AvailableFolders { get; set; }

        public ContentRootFolderMatchRule SelectedRule
        {
            get
            {
                return selectedRule;
            }
            set
            {
                selectedRule = value;
                OnPropertyChanged(this, "SelectedRule");
            }
        }
        private ContentRootFolderMatchRule selectedRule;

        public ContentType ContentType { get; set; }

        #endregion

        #region Commands

        private ICommand addFolderCommand;
        public ICommand AddFolderCommand
        {
            get
            {
                if (addFolderCommand == null)
                {
                    addFolderCommand = new RelayCommand(
                        param => this.AddFolder()
                    );
                }
                return addFolderCommand;
            }
        }

        private ICommand removeFolderCommand;
        public ICommand RemoveFolderCommand
        {
            get
            {
                if (removeFolderCommand == null)
                {
                    removeFolderCommand = new RelayCommand(
                        param => this.RemoveFolder()
                    );
                }
                return removeFolderCommand;
            }
        }

        private ICommand clearFoldersCommand;
        public ICommand ClearFoldersCommand
        {
            get
            {
                if (clearFoldersCommand == null)
                {
                    clearFoldersCommand = new RelayCommand(
                        param => this.ClearFolders()
                    );
                }
                return clearFoldersCommand;
            }
        }

        private ICommand addChildCommand;
        public ICommand AddChildCommand
        {
            get
            {
                if (addChildCommand == null)
                {
                    addChildCommand = new RelayCommand(
                        param => this.AddChild()
                    );
                }
                return addChildCommand;
            }
        }

        private ICommand setAllSubFoldersAsChildrenCommand;
        public ICommand SetAllSubFoldersAsChildrenCommand
        {
            get
            {
                if (setAllSubFoldersAsChildrenCommand == null)
                {
                    setAllSubFoldersAsChildrenCommand = new RelayCommand(
                        param => this.SetAllChilds()
                    );
                }
                return setAllSubFoldersAsChildrenCommand;
            }
        }

        private ICommand addRuleCommand;
        public ICommand AddRuleCommand
        {
            get
            {
                if (addRuleCommand == null)
                {
                    addRuleCommand = new RelayCommand(
                        param => this.AddRule()
                    );
                }
                return addRuleCommand;
            }
        }

        private ICommand removeRuleCommand;
        public ICommand RemoveRuleCommand
        {
            get
            {
                if (removeRuleCommand == null)
                {
                    removeRuleCommand = new RelayCommand(
                        param => this.RemoveRule()
                    );
                }
                return removeRuleCommand;
            }
        }

        private ICommand clearRulesCommand;
        public ICommand ClearRulesCommand
        {
            get
            {
                if (clearRulesCommand == null)
                {
                    clearRulesCommand = new RelayCommand(
                        param => this.ClearRules()
                    );
                }
                return clearRulesCommand;
            }
        }

        #endregion

        #region Constructor

        public RootFolderControlViewModel(ContentRootFolderCollection folders, ContentType type)
        {
            this.ContentType = type;
            this.RootFolders = new ContentRootFolderCollection(folders);
            AttachPropChangedEvent(this.RootFolders);

            this.AvailableFolders = new ObservableCollection<string>();
            UpdateAvailableFolders();
        }

        private void UpdateAvailableFolders()
        {
            // Get all folders
            List<ContentRootFolder> recursiveFolders = this.RootFolders.GetFolders(true);

            // Remove extras in available
            for (int i = this.AvailableFolders.Count - 1; i >= 0; i--)
            {
                bool found = false;
                foreach (ContentRootFolder folder in recursiveFolders)
                    if (!this.AvailableFolders.Contains(folder.FullPath))
                    {
                        found = true;
                        break;
                    }

                if (!found)
                    this.AvailableFolders.RemoveAt(i);

            }

            // Add any missing in available
            foreach (ContentRootFolder folder in recursiveFolders)
                if (!this.AvailableFolders.Contains(folder.FullPath))
                    this.AvailableFolders.Add(folder.FullPath);
        }


        #endregion

        #region Methods

        private void AttachPropChangedEvent(ObservableCollection<ContentRootFolder> folders)
        {
            // Go thorough all content folders in list
            foreach (ContentRootFolder folder in folders)
            {
                folder.PropertyChanged += cloneFolder_PropertyChanged;

                // Recursion of sub-content folders
                if (folder.ChildFolders.Count > 0)
                    AttachPropChangedEvent(folder.ChildFolders);
            }
        }

        private void cloneFolder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Default" && (sender as ContentRootFolder).Default)
                ClearDefault(sender as ContentRootFolder, this.RootFolders);
        }

        /// <summary>
        /// Clear default flag on all folders except for one (the new default).
        /// </summary>
        /// <param name="exception">Folder to allow default flag to be on.</param>
        /// <param name="folders">List of content folders to clear default on</param>
        private void ClearDefault(ContentRootFolder exception, ObservableCollection<ContentRootFolder> folders)
        {
            // Go thorough all content folders in list
            foreach (ContentRootFolder folder in folders)
            {
                // Clear default flag if not exception
                if (folder != exception)
                    folder.Default = false;

                // Recursion of sub-content folders
                if (folder.ChildFolders.Count > 0)
                    ClearDefault(exception, folder.ChildFolders);
            }
        }

        private void AddFolder()
        {
            // Open folder browser
            VistaFolderBrowserDialog folderSel = new VistaFolderBrowserDialog();

            // Add folder if valid folder selected
            if (folderSel.ShowDialog() == true && System.IO.Directory.Exists(folderSel.SelectedPath))
            {
                ContentRootFolder folder = new ContentRootFolder(this.ContentType, folderSel.SelectedPath, folderSel.SelectedPath);
                if (RootFolders.Count == 0)
                    folder.Default = true;
                folder.PropertyChanged += cloneFolder_PropertyChanged;
                RootFolders.Add(folder);

                UpdateAvailableFolders();
            }
        }

        private void RemoveFolder()
        {
            if (this.SelectedRootFolder != null)
                RemoveFolder(this.SelectedRootFolder, this.RootFolders);

            if (this.RootFolders.Count > 0 && !CheckForDefault(this.RootFolders))
                this.RootFolders[0].Default = true;
            UpdateAvailableFolders();
                
        }

        private bool CheckForDefault(ObservableCollection<ContentRootFolder> folders)
        {
            foreach (ContentRootFolder folder in folders)
            {
                if (folder.Default)
                    return true;

                if (CheckForDefault(folder.ChildFolders))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Recursive search to remove folder
        /// </summary>
        /// <param name="selFolder">Folder to remove</param>
        /// <param name="folders">List of folders to look for folder to remove in</param>
        /// <returns>True if folder was removed</returns>
        private bool RemoveFolder(ContentRootFolder selFolder, ObservableCollection<ContentRootFolder> folders)
        {
            // Loop through folders
            if (folders.Contains(selFolder))
            {
                folders.Remove(selFolder);
                return true;
            }

            for (int i = 0; i < folders.Count; i++)
            {
                // Check sub-folders
                if (RemoveFolder(selFolder, folders[i].ChildFolders))
                    return true;
            }

            // Folder not removed yet
            return false;
        }

        private void ClearFolders()
        {
            foreach(ContentRootFolder folder in this.RootFolders)
                folder.PropertyChanged -= cloneFolder_PropertyChanged;
            this.RootFolders.Clear();
            UpdateAvailableFolders();
        }

        private void AddChild()
        {
            if (this.SelectedRootFolder != null)
            {
                // Get list of sub-directories in content folder
                string[] subDirs = this.SelectedRootFolder.GetFolderSubDirectoryNamesThatArentChildren().ToArray();

                // open selection form to allow user to chose a sub-folder
                SelectionWindow selForm = new SelectionWindow("Select Folder", subDirs);
                selForm.ShowDialog();

                // If selection is valid set sub-folder as sub-content folder
                if (!string.IsNullOrEmpty(selForm.Results))
                {
                    ContentRootFolder newChild = new ContentRootFolder(this.ContentType, selForm.Results, System.IO.Path.Combine(this.SelectedRootFolder.FullPath, selForm.Results));
                    newChild.PropertyChanged += cloneFolder_PropertyChanged;
                    this.SelectedRootFolder.ChildFolders.Add(newChild);
                }
                UpdateAvailableFolders();
            }
        }

        private void SetAllChilds()
        {
            if (this.SelectedRootFolder != null)
            {
                this.SelectedRootFolder.SetAllSubDirsAsChildren();
                UpdateAvailableFolders();
            }
        }

        private void AddRule()
        {
            this.RootFolders.MatchRules.Add(new ContentRootFolderMatchRule());
        }

        private void RemoveRule()
        {
            this.RootFolders.MatchRules.Remove(this.SelectedRule);
        }

        private void ClearRules()
        {
            this.RootFolders.MatchRules.Clear();
        }

        #endregion

    }
}
