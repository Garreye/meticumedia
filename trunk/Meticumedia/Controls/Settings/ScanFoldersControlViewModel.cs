using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Meticumedia.Classes;
using Meticumedia.WPF;
using Ookii.Dialogs.Wpf;

namespace Meticumedia.Controls
{
    public class ScanFoldersControlViewModel : ViewModel
    {
        #region Properties

        public ObservableCollection<OrgFolder> Folders { get; set; }

        public OrgFolder SelectedFolder
        {
            get
            {
                return selectedFolder;
            }
            set
            {
                selectedFolder = value;
                OnPropertyChanged(this, "SelectedFolder");
            }
        }
        private OrgFolder selectedFolder;

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

        private ICommand modifyFolderPathCommand;
        public ICommand ModifyFolderPathCommand
        {
            get
            {
                if (modifyFolderPathCommand == null)
                {
                    modifyFolderPathCommand = new RelayCommand(
                        param => this.ModifyFolderPath(),
                        param => this.CanDoModifyFolderPathCommand()
                    );
                }
                return modifyFolderPathCommand;
            }
        }

        private bool CanDoModifyFolderPathCommand()
        {
            return this.SelectedFolder != null;
        }

        #endregion

        #region Constructor

        public ScanFoldersControlViewModel(ObservableCollection<OrgFolder> folders)
        {
            this.Folders = new ObservableCollection<OrgFolder>();
            foreach (OrgFolder folder in folders)
                this.Folders.Add(new OrgFolder(folder));
        }

        #endregion

        #region Methods

        private void AddFolder()
        {
            // Open folder browser
            VistaFolderBrowserDialog folderSel = new VistaFolderBrowserDialog();

            // Add folder if valid folder selected
            if (folderSel.ShowDialog() == true && System.IO.Directory.Exists(folderSel.SelectedPath))
                this.Folders.Add(new OrgFolder(folderSel.SelectedPath));
        }

        private void RemoveFolder()
        {
            if (this.SelectedFolder != null)
                this.Folders.Remove(this.SelectedFolder);
        }

        private void ClearFolders()
        {
            this.Folders.Clear();
        }

        private void ModifyFolderPath()
        {
            VistaFolderBrowserDialog folderSel = new VistaFolderBrowserDialog();
            folderSel.SelectedPath = this.SelectedFolder.FolderPath;

            if (folderSel.ShowDialog() == true && System.IO.Directory.Exists(folderSel.SelectedPath))
                this.SelectedFolder.FolderPath = folderSel.SelectedPath;

        }

        #endregion
    }
}
