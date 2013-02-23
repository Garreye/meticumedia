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

namespace Meticumedia
{
    /// <summary>
    /// Control for editing list of content folders. Folder can either be TvShow or Movie folders.
    /// </summary>
    public partial class ContentFoldersControl : UserControl
    {
        public ContentType ContentType { get; set; }
        
        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ContentFoldersControl(ContentType type)
        {
            InitializeComponent();
            this.ContentType = type;
        }

        #endregion

        #region Variables

        /// <summary>
        /// Type of content in folders list being edited.
        /// </summary>
        private ContentType content { get; set; }

        /// <summary>
        /// List of content folders
        /// </summary>
        private List<ContentRootFolder> contentFolders = new List<ContentRootFolder>();

        /// <summary>
        /// Selected content folder from listview.
        /// </summary>
        private ContentRootFolder selectedFolder = null;

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// Handle adding of new folders when user click add button.
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Open folder browser
            FolderBrowserDialog folderSel = new FolderBrowserDialog();
            folderSel.ShowDialog();

            // Check if results are valid
            if (Directory.Exists(folderSel.SelectedPath))
            {
                // Create new content folder, set it to default if no others exists
                ContentRootFolder newFolder = new ContentRootFolder(this.ContentType, folderSel.SelectedPath, folderSel.SelectedPath);
                if (contentFolders.Count == 0)
                    newFolder.Default = true;
                contentFolders.Add(newFolder);
                
                // Update display
                DisplayFoldersTree(newFolder);
            }
        }

        /// <summary>
        /// Handles removing of folder when user clicks remove buttons
        /// </summary>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            // Get selected node
            TreeNode node = tvFolders.SelectedNode;
            if (node == null)
                return;

            // Remove folder
            RemoveFolder(node.Text, contentFolders);

            // Update display
            DisplayFoldersTree(null);
            DisplaySelectedFolder();
        }

        /// <summary>
        /// Removes all folders when clear button is clicked
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            contentFolders = new List<ContentRootFolder>();
            DisplayFoldersTree(null);
            DisplaySelectedFolder();
        }

        /// <summary>
        /// Displays folder properties when a folder is selected in treeview
        /// </summary>
        private void tvFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            DisplaySelectedFolder();
        }

        /// <summary>
        /// Updates modifies status when allow organization check changes
        /// </summary>
        private void chkAllowOrg_CheckedChanged(object sender, EventArgs e)
        {
            CheckFolderForMods();
        }

        /// <summary>
        /// Updates modifies status when default check changes
        /// </summary>
        private void chkDefault_CheckedChanged(object sender, EventArgs e)
        {
            CheckFolderForMods();
        }

        /// <summary>
        /// Open form for setting a  sub-folder as sub-content folder
        /// </summary>
        private void btnAddSubfolder_Click(object sender, EventArgs e)
        {
            // Get list of sub-directories in content folder
            string[] subDirs = GetFolderSubDirectories();

            // open selection form to allow user to chose a sub-folder
            SelectionForm selForm = new SelectionForm("Select Folder", subDirs);
            selForm.ShowDialog();

            // If selection is valid set sub-folder as sub-content folder
            if (!string.IsNullOrEmpty(selForm.Results))
                selectedFolder.ChildFolders.Add(new ContentRootFolder(this.ContentType, selForm.Results, Path.Combine(selectedFolder.FullPath, selForm.Results)));

            // Refresh displau
            DisplayFoldersTree(selectedFolder);
        }

        /// <summary>
        /// Build sub-directories of content folder as string array of the paths.
        /// </summary>
        /// <returns></returns>
        private string[] GetFolderSubDirectories()
        {
            if (selectedFolder == null)
                return new string[0];
            
            string[] subDirs = Directory.GetDirectories(selectedFolder.FullPath);
            for (int i = 0; i < subDirs.Length; i++)
            {
                string[] dirs = subDirs[i].Split('\\');
                subDirs[i] = dirs[dirs.Length - 1];
            }
            return subDirs;
        }

        /// <summary>
        /// Set all sub-directories as sub content folders when button is clicked.
        /// </summary>
        private void btnAddAllSubs_Click(object sender, EventArgs e)
        {
            string[] subDirs = GetFolderSubDirectories();
            string message = "Are you sure you want to set the following as sub-folders of '" + selectedFolder.FullPath + "'?";
            foreach(string subDir in subDirs)
                message += Environment.NewLine + "\t" + subDir;
            
            if (MessageBox.Show(message, "Sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                foreach(string subDir in subDirs)
                    selectedFolder.ChildFolders.Add(new ContentRootFolder(this.ContentType, subDir, Path.Combine(selectedFolder.FullPath, subDir)));

            // Refresh displau
            DisplayFoldersTree(selectedFolder);
        }

        /// <summary>
        /// Updates property mods to folder instances
        /// </summary>
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateFolder();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads folders from setting for either TV or movies folders.
        /// </summary>
        /// <param name="content"></param>
        public void LoadFolders(ContentType content)
        {
            // Clone folders from settings and add each folder to tree view
            contentFolders = new List<ContentRootFolder>();

            // Set content type
            this.content = content;

            // Gets folders from settings
            List<ContentRootFolder> folders;
            switch (content)
            {
                case ContentType.Movie:
                    folders = Settings.MovieFolders;
                    break;
                case ContentType.TvShow:
                    folders = Settings.TvFolders;
                    break;
                default:
                    throw new Exception("Unknown type");
            }

            // Create local clone of folders
            foreach (ContentRootFolder folder in folders)
                contentFolders.Add(new ContentRootFolder(folder));

            // Display folders
            DisplayFoldersTree(null);
        }

        /// <summary>
        /// Displays folder hierarchy in treeview
        /// </summary>
        /// <param name="select"></param>
        private void DisplayFoldersTree(ContentRootFolder select)
        {
            // Clear tree view
            tvFolders.Nodes.Clear();

            // Add each movie to tree
            foreach (ContentRootFolder folder in contentFolders)
                AddFolderToTree(folder, tvFolders.Nodes);
        }

        /// <summary>
        /// Recusively add content folder and its sub-content folders to tree view.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="nodes"></param>
        private void AddFolderToTree(ContentRootFolder folder, TreeNodeCollection nodes)
        {
            // Add content and sub-content folders to current node
            TreeNode node = nodes.Add(folder.SubPath);
            foreach (ContentRootFolder subFolder in folder.ChildFolders)
                AddFolderToTree(subFolder, node.Nodes);

            // Highligh default folder
            if (folder.Default)
                node.ForeColor = Color.Blue;
        }

        /// <summary>
        /// Applies changes made in control to settings.
        /// </summary>
        public void SetFolders()
        {
            switch (content)
            {
                case ContentType.Movie:
                    Settings.MovieFolders = contentFolders;
                    break;
                case ContentType.TvShow:
                    Settings.TvFolders = contentFolders;
                    break;
                default:
                    throw new Exception("Unknown type");
            }
        }

        /// <summary>
        /// Display properties of selected folder.
        /// </summary>
        private void DisplaySelectedFolder()
        {
            // Get selected node
            TreeNode node = tvFolders.SelectedNode;
            if (tvFolders.SelectedNode == null)
            {
                selectedFolder = null;
                gbMfProperties.Enabled = false;
                return;
            }

            // Load properties and enable properties gorup box
            GetFolderFromList(node.Text, this.contentFolders, out selectedFolder);
            gbMfProperties.Enabled = true;
            chkMfAllowOrg.Checked = selectedFolder.AllowOrganizing;
            chkMfDefault.Checked = selectedFolder.Default || (tvFolders.Nodes.Count == 1 && tvFolders.Nodes[0].Nodes.Count == 0);
            chkMfDefault.Enabled = (tvFolders.Nodes.Count > 1 || tvFolders.Nodes[0].Nodes.Count > 0) && !selectedFolder.Default;
        }

        /// <summary>
        /// Gets content folder instance from selected node in tree view.
        /// </summary>
        /// <param name="text">Folder path to match to</param>
        /// <param name="folders">List of folder to search in</param>
        /// <param name="match">Matched folder</param>
        /// <returns>True if folder was found</returns>
        private bool GetFolderFromList(string text, List<ContentRootFolder> folders, out ContentRootFolder match)
        {
            // Loop through content folders
            foreach (ContentRootFolder folder in folders)
            {
                // Match folder to selected text
                if (folder.SubPath == text)
                {
                    match = folder;
                    return true;
                }

                // Recusion on sub-folders
                if (GetFolderFromList(text, folder.ChildFolders, out match))
                    return true;
            }

            // No match
            match = new ContentRootFolder(folders[0].ContentType);
            return false;
        }

        /// <summary>
        /// Remove folder from content folders hierarchy
        /// </summary>
        /// <param name="text">Path to folder</param>
        /// <param name="folders">List of folders to look for folder to remove in</param>
        /// <returns>True if folder was removed</returns>
        private bool RemoveFolder(string text, List<ContentRootFolder> folders)
        {
            // Loop through folders
            for (int i = 0; i < folders.Count; i++)
            {
                // Remove folder match text from list
                if (folders[i].SubPath == text)
                {
                    folders.RemoveAt(i);
                    return true;
                }

                // Check sub-folders
                if (RemoveFolder(text, folders[i].ChildFolders))
                    return true;
            }
            
            // Folder not removed yet
            return false;
        }

        /// <summary>
        /// Checks if properties of selected folder have been modified.
        /// </summary>
        private void CheckFolderForMods()
        {
            bool mods = false;
            if (selectedFolder != null)
            {
                if (chkMfAllowOrg.Checked != selectedFolder.AllowOrganizing)
                    mods = true;
                else if (chkMfDefault.Checked != selectedFolder.Default)
                    mods = true;
            }
            btnMfUpdate.Enabled = mods;
        }

        /// <summary>
        /// Applies property mods to folder instance
        /// </summary>
        private void UpdateFolder()
        {
            selectedFolder.AllowOrganizing = chkMfAllowOrg.Checked;
            selectedFolder.Default = chkMfDefault.Checked;

            // Only one folder can be the default
            if (selectedFolder.Default)
            {
                ClearDefault(selectedFolder, contentFolders);
                DisplayFoldersTree(selectedFolder);
            }
            CheckFolderForMods();

        }

        /// <summary>
        /// Clear default flag on all folders except for one (the new default).
        /// </summary>
        /// <param name="exception">Folder to allow default flag to be on.</param>
        /// <param name="folders">List of content folders to clear default on</param>
        private void ClearDefault(ContentRootFolder exception, List<ContentRootFolder> folders)
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

        #endregion
    }
}
