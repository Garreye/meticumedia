using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Collections.ObjectModel;
using Meticumedia.Classes;
using System.ComponentModel;
using Meticumedia.Windows;

namespace Meticumedia.Controls
{
    /// <summary>
    /// Interaction logic for RootFolderControl.xaml
    /// </summary>
    public partial class RootFolderControl : UserControl
    {
        public RootFolderControl()
        {
            InitializeComponent();

            TextBlock bloc = new TextBlock();
        }

        #region Variables

        private ObservableCollection<ContentRootFolder> folders = new ObservableCollection<ContentRootFolder>();

        private ContentType contentType = ContentType.Movie;

        #endregion

        #region Event Handlers

        private void tvFolders_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (tvFolders.SelectedItem != null)
            {
                ctntSelectedItem.IsEnabled = true;
                ctntSelectedItem.Content = tvFolders.SelectedItem;
            }
            else
                ctntSelectedItem.IsEnabled = false;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Open folder browser
            VistaFolderBrowserDialog folderSel = new VistaFolderBrowserDialog();

            // Add folder if valid folder selected
            if ((bool)folderSel.ShowDialog() && Directory.Exists(folderSel.SelectedPath))
            {
                ContentRootFolder folder = new ContentRootFolder(contentType, folderSel.SelectedPath, folderSel.SelectedPath);
                if (folders.Count == 0)
                    folder.Default = true;
                folders.Add(folder);
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (tvFolders.SelectedItem != null)
            {                
                ContentRootFolder folder = (ContentRootFolder)tvFolders.SelectedItem;
                RemoveFolder(folder, folders);

                if(folder.Default && folders.Count > 0)
                    folders[0].Default = true;
            }
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

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            folders.Clear();
        }

        /// <summary>
        /// On efault check must only allow one folder to be the default
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DefaultCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ClearDefault(tvFolders.SelectedItem as ContentRootFolder, folders);
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

        private void btnAddChild_Click(object sender, RoutedEventArgs e)
        {
            if (tvFolders.SelectedItem != null)
            {
                ContentRootFolder folder = (ContentRootFolder)tvFolders.SelectedItem;

                // Get list of sub-directories in content folder
                string[] subDirs = GetFolderSubDirectories(folder);

                // open selection form to allow user to chose a sub-folder
                SelectionWindow selForm = new SelectionWindow("Select Folder", subDirs);
                selForm.ShowDialog();

                // If selection is valid set sub-folder as sub-content folder
                if (!string.IsNullOrEmpty(selForm.Results))
                    folder.ChildFolders.Add(new ContentRootFolder(contentType, selForm.Results, System.IO.Path.Combine(folder.FullPath, selForm.Results)));
            }
        }

        private void btnSetAllChilds_Click(object sender, RoutedEventArgs e)
        {
            if (tvFolders.SelectedItem != null)
            {
                ContentRootFolder folder = (ContentRootFolder)tvFolders.SelectedItem;
                string[] subDirs = GetFolderSubDirectories(folder);
                foreach (string subDir in subDirs)
                    folder.ChildFolders.Add(new ContentRootFolder(contentType, subDir, System.IO.Path.Combine(folder.FullPath, subDir)));
            }
        }

        /// <summary>
        /// Build sub-directories of content folder as string array of the paths.
        /// </summary>
        /// <returns></returns>
        private string[] GetFolderSubDirectories(ContentRootFolder folder)
        {
            string[] subDirs = Directory.GetDirectories(folder.FullPath);
            for (int i = 0; i < subDirs.Length; i++)
            {
                string[] dirs = subDirs[i].Split('\\');
                subDirs[i] = dirs[dirs.Length - 1];
            }
            return subDirs;
        }

        #endregion

        #region Save/Load

        public void LoadSettings(ContentType type)
        {
            this.contentType = type;
            foreach (ContentRootFolder flder in contentType == ContentType.Movie ? Settings.MovieFolders : Settings.TvFolders)
                folders.Add(new ContentRootFolder(flder));
            tvFolders.ItemsSource = folders;
        }

        public void SaveSettings()
        {
            switch(contentType)
            {
                case ContentType.Movie:
                    Settings.MovieFolders = folders;
                    break;
                case ContentType.TvShow:
                    Settings.TvFolders = folders;
                    break;
            }
            
        }

        #endregion
    }
}
