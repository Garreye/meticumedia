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
    /// Control for modifying scan folders in settings.
    /// </summary>
    public partial class ScanFolderControl : UserControl
    {
        #region Constructor

        /// <summary>
        /// Default constructr
        /// </summary>
        public ScanFolderControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Variables

        private List<OrgFolder> folders;

        /// <summary>
        /// Currently selected scan folder
        /// </summary>
        private OrgFolder selectedScanFolder = null;

        /// <summary>
        /// Index of selected scan folder
        /// </summary>
        private int selectedScanFolderIndex = -1;

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// Add button add new scan folder to list.
        /// </summary>
        private void bntScanAdd_Click(object sender, EventArgs e)
        {
            // Open folder browser
            FolderBrowserDialog folderSel = new FolderBrowserDialog();

            // Add folder if valid folder selected
            if (folderSel.ShowDialog() == DialogResult.OK && Directory.Exists(folderSel.SelectedPath))
                folders.Add(new OrgFolder(folderSel.SelectedPath));
            DisplayFolders();
        }

        /// <summary>
        /// Remove button removes select folder from list
        /// </summary>
        private void btnScanRemove_Click(object sender, EventArgs e)
        {
            if (lvScanFolders.SelectedIndices.Count == 0)
                return;

            folders.RemoveAt(lvScanFolders.SelectedIndices[0]);
            DisplayFolders();
        }

        /// <summary>
        /// Clear button removes all folders from list
        /// </summary>
        private void btnScanClear_Click(object sender, EventArgs e)
        {
            folders.Clear();
            DisplayFolders();
        }

        /// <summary>
        /// Scan directory folder browser button handler
        /// </summary>
        private void btnScanDirSel_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderSel = new FolderBrowserDialog();

            if (folderSel.ShowDialog() == DialogResult.OK && Directory.Exists(folderSel.SelectedPath))
                txtScanDir.Text = folderSel.SelectedPath;
        }

        /// <summary>
        /// Selecting a folder from listview loads its property for display
        /// </summary>
        private void lvScanFolders_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplaySelectedScanFolder();
        }

        /// <summary>
        /// Check for property changes when directory textbox is changed
        /// </summary>
        private void txtScanDir_TextChanged(object sender, EventArgs e)
        {
            CheckScanForMods();
        }

        /// <summary>
        /// Check for property changes when move from checkbox changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbScanMoveFrom_CheckedChanged(object sender, EventArgs e)
        {
            CheckScanForMods();
        }

        /// <summary>
        /// Check for property changes when recursive check changes
        /// </summary>
        private void chkRecursive_CheckedChanged(object sender, EventArgs e)
        {
            chkAutoDeleteFolders.Enabled = chkAllowDeleting.Checked & chkRecursive.Checked;
            chkAutoDeleteFolders.Checked &= chkAutoDeleteFolders.Enabled;
            CheckScanForMods();
        }

        /// <summary>
        /// Check for property changes when allow deleting check changes
        /// </summary>
        private void chkAllowDeleting_CheckedChanged(object sender, EventArgs e)
        {
            chkAutoDeleteFolders.Enabled = chkAllowDeleting.Checked & chkRecursive.Checked;
            chkAutoDeleteFolders.Checked &= chkAutoDeleteFolders.Enabled;
            CheckScanForMods();
        }

        /// <summary>
        /// Check for property changes when auto delete folders check changes
        /// </summary>
        private void chkAutoDeleteFolders_CheckedChanged(object sender, EventArgs e)
        {
            CheckScanForMods();
        }

        /// <summary>
        /// Apply property changes for folder to folder instance when update button is clicked
        /// </summary>
        private void btnScanUpdate_Click(object sender, EventArgs e)
        {
            UpdateScanFolder();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads scan folders into listview
        /// </summary>
        public void LoadScanFolders()
        {
            // Copy scan directories to local folders variable
            folders = new List<OrgFolder>();
            foreach (OrgFolder flder in Settings.ScanDirectories)
                folders.Add(new OrgFolder(flder));

            // Display fodlers
            DisplayFolders();
        }

        // Display folders in listview
        private void DisplayFolders()
        {
            lvScanFolders.Items.Clear();
            foreach (OrgFolder folder in folders)
                AddScanFolderToList(folder);
        }

        /// <summary>
        /// Save scan folders to setting
        /// </summary>
        public void SetScanFolders()
        {
            Settings.ScanDirectories = folders;
        }

        /// <summary>
        /// Add a single scan folder to listview
        /// </summary>
        /// <param name="folder"></param>
        private void AddScanFolderToList(OrgFolder folder)
        {
            ListViewItem item = lvScanFolders.Items.Add(folder.FolderPath);
            item.SubItems.Add(folder.CopyFrom ? "Copy" : "Move");
            item.SubItems.Add(folder.Recursive.ToString());
            item.SubItems.Add(folder.AllowDeleting.ToString());
            item.SubItems.Add(folder.AutomaticallyDeleteEmptyFolders.ToString());
            item.Selected = true;
            lvScanFolders.Focus();
        }

        /// <summary>
        /// Displays selected scan folder properties in properties group box
        /// </summary>
        private void DisplaySelectedScanFolder()
        {
            // If no selection clear properties controls
            if (lvScanFolders.SelectedIndices.Count == 0)
            {
                selectedScanFolderIndex = -1;
                selectedScanFolder = null;
                txtScanDir.Text = string.Empty;
                gbScanProperties.Enabled = false;
                return;
            }

            // Load properties from selecting into appropriate controls
            gbScanProperties.Enabled = true;
            selectedScanFolderIndex = lvScanFolders.SelectedIndices[0];
            selectedScanFolder = folders[selectedScanFolderIndex];
            txtScanDir.Text = selectedScanFolder.FolderPath;
            rbScanCopyFrom.Checked = selectedScanFolder.CopyFrom;
            rbScanMoveFrom.Checked = !selectedScanFolder.CopyFrom;
            chkRecursive.Checked = selectedScanFolder.Recursive;
            chkAllowDeleting.Checked = selectedScanFolder.AllowDeleting;
            chkAutoDeleteFolders.Checked = selectedScanFolder.AutomaticallyDeleteEmptyFolders;
        }


        /// <summary>
        /// Compare selected scan folder properties to thoses in properties group box for changes.
        /// Enables update button if there are changes.
        /// </summary>
        private void CheckScanForMods()
        {
            // Compare each property
            bool mods = false;
            if (selectedScanFolder != null)
            {
                if (txtScanDir.Text != selectedScanFolder.FolderPath)
                    mods = true;
                else if (rbScanCopyFrom.Checked != selectedScanFolder.CopyFrom)
                    mods = true;
                else if (chkRecursive.Checked != selectedScanFolder.Recursive)
                    mods = true;
                else if (chkAllowDeleting.Checked != selectedScanFolder.AllowDeleting)
                    mods = true;
                else if (chkAutoDeleteFolders.Checked != selectedScanFolder.AutomaticallyDeleteEmptyFolders)
                    mods = true;

            }
            
            // Enable update button if there have been mods
            btnScanUpdate.Enabled = mods;
        }

        /// <summary>
        /// Update scan folders listview with changes made in properties group box.
        /// </summary>
        private void UpdateScanFolder()
        {
            // Check that entered directory is valid
            string path = txtScanDir.Text;
            if (!Directory.Exists(path))
            {
                MessageBox.Show("Director does not exist");
                return;
            }

            // Update selected folder
            selectedScanFolder.FolderPath = path;
            selectedScanFolder.CopyFrom = rbScanCopyFrom.Checked;
            selectedScanFolder.Recursive = chkRecursive.Checked;
            selectedScanFolder.AllowDeleting = chkAllowDeleting.Checked;
            selectedScanFolder.AutomaticallyDeleteEmptyFolders = chkAutoDeleteFolders.Checked;

            // Update listview
            lvScanFolders.Items[selectedScanFolderIndex].SubItems[0].Text = path;
            lvScanFolders.Items[selectedScanFolderIndex].SubItems[1].Text = selectedScanFolder.CopyFrom ? "Copy" : "Move";
            lvScanFolders.Items[selectedScanFolderIndex].SubItems[2].Text = selectedScanFolder.Recursive.ToString();
            lvScanFolders.Items[selectedScanFolderIndex].SubItems[3].Text = selectedScanFolder.AllowDeleting.ToString();
            lvScanFolders.Items[selectedScanFolderIndex].SubItems[4].Text = selectedScanFolder.AutomaticallyDeleteEmptyFolders.ToString();

            // Redo mods check
            CheckScanForMods();
        }

        #endregion
    }
}
