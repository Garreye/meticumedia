// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace Meticumedia
{
    /// <summary>
    /// Listview for displaying folders contained in content folder
    /// </summary>
    public class ContentListView : DoubleBufferedListView
    {
        #region Events

        /// <summary>
        /// Event indicating there are items to be sent to the queue
        /// </summary>
        public static event EventHandler<ItemsToQueueArgs> ItemsToQueue;

        /// <summary>
        /// Triggers ItemsToQueue event
        /// </summary>
        /// <param name="items"></param>
        private static void OnItemsToQueue(List<OrgItem> items)
        {
            if (ItemsToQueue != null)
                ItemsToQueue(null, new ItemsToQueueArgs(items));
        }

        /// <summary>
        /// Event indicating there are items to be edited
        /// </summary>
        public event EventHandler ItemToEdit;

        /// <summary>
        /// Triggers ItemToEdit event
        /// </summary>
        /// <param name="items"></param>
        private void OnItemToEdit(Content item)
        {
            if (ItemToEdit != null)
                ItemToEdit(item, new EventArgs());
        }

        /// <summary>
        /// Event indicating that content in list needs to be saved
        /// </summary>
        public event EventHandler SaveContentsRequired;

        /// <summary>
        /// Triggers SaveContentsRequired event
        /// </summary>
        /// <param name="items"></param>
        private void OnSaveContentsRequired()
        {
            if (SaveContentsRequired != null)
                SaveContentsRequired(this, new EventArgs());
        }

        /// <summary>
        /// Event indicating there are items to be sent to the queue
        /// </summary>
        public event EventHandler UpdateContentsRequired;

        /// <summary>
        /// Triggers ItemsToQueue event
        /// </summary>
        /// <param name="items"></param>
        private void OnUpdateContentsRequired()
        {
            if (UpdateContentsRequired != null)
                UpdateContentsRequired(this, new EventArgs());
        }


        #endregion

        #region Properties

        /// <summary>
        /// Content Folders to be displayed
        /// </summary>
        public List<Content> Contents { get; set; }

        /// <summary>
        /// Sets whether watched contents should be hidden
        /// </summary>
        public bool HideWatched 
        {
            get { return hideWatched; }
            set 
            { 
                hideWatched = value;
                DisplayContent(false);
            }
        }

        public ContentType ContentType { get; set; }

        #endregion

        #region Variables

        /// <summary>
        /// Contents currents displayed in listview (may differ from all content due to filtering)
        /// </summary>
        private List<Content> displayedContents = new List<Content>();

        /// <summary>
        /// Local variable for HideWatched property
        /// </summary>
        private bool hideWatched = false;

        /// <summary>
        /// Column that was last sorted on
        /// </summary>
        public int lastSortColumn = 0;

        /// <summary>
        /// Flag indicating whether sorting based on column clicked is currently set to ascending (used for toggling between ascending/descending)
        /// </summary>
        private bool ascendingSort = true;

        /// <summary>
        /// Context menu for list view
        /// </summary>
        private ContextMenu contextMenu = new ContextMenu();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ContentListView() : base()
        {
            this.Contents = new List<Content>();
            this.ContentType = ContentType.Movie;

            // Setup context menu
            this.ContextMenu = contextMenu;
            contextMenu.Popup += new EventHandler(contextMenu_Popup);

            // Column sorting event
            this.ColumnClick += new ColumnClickEventHandler(ContentListView_ColumnClick);
            this.KeyUp += new KeyEventHandler(ContentListView_KeyUp);
            this.DoubleClick += new EventHandler(ContentListView_DoubleClick);
        }

        /// <summary>
        /// Double-clicking item in listview opens editor for it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ContentListView_DoubleClick(object sender, EventArgs e)
        {
            Content selContent;
            if (GetSeletectedContent(out selContent))
                OnItemToEdit(selContent);
        }

        #endregion

        #region Context Menu

        /// <summary>
        /// Context menu is built on right-click
        /// </summary>
        private void contextMenu_Popup(object sender, EventArgs e)
        {
            BuildContextMenu();
        }

        /// <summary>
        /// Builds context menu based on what is currently selected
        /// </summary>
        private void BuildContextMenu()
        {
            contextMenu.MenuItems.Clear();

            // Get selected movies
            List<Content> selContent;
            if (!GetSeletectedContents(out selContent))
                return;

            if (selContent.Count == 1)
                contextMenu.MenuItems.Add("Edit", HandleEdit);

            bool allWatched = true;
            bool allUnwatched = true;
            foreach (Content content in selContent)
                if (content.Watched)
                    allUnwatched = false;
                else
                    allWatched = false;

            if ((allUnwatched && !allWatched) || (!allWatched && !allUnwatched))
                contextMenu.MenuItems.Add("Mark as Watched", HandleMarkAsWatch);
            if ((allWatched && !allUnwatched) || (!allWatched && !allUnwatched))
                contextMenu.MenuItems.Add("Unmark as Watched", HandleUnmarkAsWatch);

            contextMenu.MenuItems.Add("Set as Child Root fplder(s)", HandleSetAsSubFolder);

            // Set folder to exclude from move list
            string exclude = selContent[0].RootFolder;
            for (int i = 1; i < selContent.Count; i++)
                if (selContent[i].RootFolder != exclude)
                {
                    exclude = string.Empty;
                    break;
                }

            List<string> foldersStrs = new List<string>();
            List<ContentRootFolder> folders = (Contents.Count > 0 && Contents[0] is Movie) ? Settings.MovieFolders : Settings.TvFolders;
            foreach (ContentRootFolder folder in folders)
                AddFolderPaths(exclude, foldersStrs, folder);
            if (foldersStrs.Count > 0)
            {
                MenuItem moveItem = contextMenu.MenuItems.Add("Move To");
                foreach (string folderPath in foldersStrs)
                    moveItem.MenuItems.Add(folderPath, HandleMove);
            }

            contextMenu.MenuItems.Add("Delete", HandleDelete);
        }

        /// <summary>
        /// Handle edit selection from context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleEdit(object sender, EventArgs e)
        {
            Content selContent;
            if (GetSeletectedContent(out selContent))
                OnItemToEdit(selContent);
        }

        /// <summary>
        /// Handle mark as watched selection from context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleMarkAsWatch(object sender, EventArgs e)
        {
            MarkSelectedWatched(true);
        }

        /// <summary>
        /// Set watched property for selected item in listview
        /// </summary>
        public void MarkSelectedWatched(bool watched)
        {
            // Get the selected contents
            List<Content> selContents;
            if (!GetSeletectedContents(out selContents))
                return;

            // Mark each item
            foreach (Content cnt in selContents)
                cnt.Watched = watched;

            // Save changes
            OnSaveContentsRequired();

            // Refresh Display
            DisplayContent(false);
        }

        /// <summary>
        /// Handle un-mark as watched selection from context menu
        /// </summary>
        private void HandleUnmarkAsWatch(object sender, EventArgs e)
        {
            MarkSelectedWatched(false);
        }

        /// <summary>
        /// Handle set as sub-fodler selection from context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleSetAsSubFolder(object sender, EventArgs e)
        {
            // Get the selected contents
            List<Content> selContents;
            if (!GetSeletectedContents(out selContents))
                return;

            // Mark each item
            foreach (Content cnt in selContents)
            {
                // Format path
                string fullPath = Path.GetFullPath(cnt.Path).TrimEnd(Path.DirectorySeparatorChar);
                string path = Path.GetFileName(fullPath);

                // Get content folder for content
                ContentRootFolder folder;
                if (Settings.GetContentFolder(cnt.RootFolder, out folder))
                    // Convert movie folder as sub-folder
                    folder.ChildFolders.Add(new ContentRootFolder(this.ContentType, path, fullPath));
            }
            Settings.Save();
            OnUpdateContentsRequired();
        }

        /// <summary>
        /// Handle move selection from context menu
        /// </summary>
        private void HandleMove(object sender, EventArgs e)
        {
            // Get selected movies
            List<Content> selContents;
            if (!GetSeletectedContents(out selContents))
                return;

            string destinationFolder = ((MenuItem)sender).Text;

            if (!string.IsNullOrEmpty(destinationFolder))
            {
                List<OrgItem> items = new List<OrgItem>();
                foreach (Content content in selContents)
                {
                    // Check that move is necessary
                    if (content.RootFolder == destinationFolder)
                        continue;

                    // Format path
                    string currentPath = Path.GetFullPath(content.Path).TrimEnd(Path.DirectorySeparatorChar);
                    string endFolder = Path.GetFileName(currentPath);
                    string newPath = Path.Combine(destinationFolder, endFolder);
                    content.RootFolder = destinationFolder;
                    content.Path = newPath;
                    if (content is Movie)
                        items.Add(new OrgItem(OrgAction.Move, currentPath, FileHelper.FileCategory.Folder, content as Movie, newPath, null));
                    else
                        items.Add(new OrgItem(OrgAction.Move, currentPath, FileHelper.FileCategory.Folder, null, newPath, null));
                }
                OnItemsToQueue(items);
            }
        }

        /// <summary>
        /// Recursively add content folder and its sub-content folders to list of folder paths
        /// </summary>
        /// <param name="currentFolder">current folder path that should not be added to list </param>
        /// <param name="folderStrings">list of folder paths being built</param>
        /// <param name="folder">current folder to be added to list</param>
        private void AddFolderPaths(string currentFolder, List<string> folderStrings, ContentRootFolder folder)
        {
            if (folder.FullPath != currentFolder)
                folderStrings.Add(folder.FullPath);

            foreach (ContentRootFolder subFolder in folder.ChildFolders)
                AddFolderPaths(currentFolder, folderStrings, subFolder);
        }

        /// <summary>
        /// Handle delete selection from context menu
        /// </summary>
        private void HandleDelete(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you want to delete the selected items? This operation cannot be undone", "Sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Get selected content
                List<Content> selContent;
                if (!GetSeletectedContents(out selContent))
                    return;

                List<OrgItem> items = new List<OrgItem>();
                foreach (Content content in selContent)
                    items.Add(new OrgItem(OrgAction.Delete, content.Path, FileHelper.FileCategory.Folder, null));
                OnItemsToQueue(items);
            }
        }

        #endregion

        #region Display

        /// <summary>
        /// Displays content in listview for selected root folder.
        /// </summary>
        public void DisplayContent(bool newOnly)
        {
            int selectedIndex = -1;
            if (this.SelectedIndices.Count > 0)
                selectedIndex = this.SelectedIndices[0];

            int top = 0;
            if (this.Items.Count > 0)
                top = this.TopItem.Index;

            // Clear list
            if (!newOnly)
            {
                this.Items.Clear();
                displayedContents = new List<Content>();
            }

            // Add each content to list
            if (Contents != null)
                for(int i=this.Items.Count;i<Contents.Count;i++)
                    AddContentToList(Contents[i]);

            if (top < this.Items.Count && !newOnly)
            {
                this.TopItem = this.Items[top];

                if (selectedIndex != -1 && selectedIndex < this.Items.Count)
                {
                    this.Items[selectedIndex].EnsureVisible();
                    this.Focus();
                    this.Items[selectedIndex].Selected = true;
                    this.Items[selectedIndex].Focused = true;

                }

            }
        }

        /// <summary>
        /// Adds a single content item to listview.
        /// </summary>
        /// <param name="content">The movie to add to listview</param>
        private void AddContentToList(Content content)
        {
            if (this.hideWatched && content.Watched)
                return;

            ListViewItem item = this.Items.Add(content.Path);
            item.SubItems.Add(content.ToString());
            item.SubItems.Add(content.Date.Year.ToString());
            item.SubItems.Add(content.GetGenresString());
            item.SubItems.Add(content.Overview.Replace(Environment.NewLine, ""));
            if (content.Name.Equals(string.Empty) || content.Id == 0)
                item.BackColor = Color.LightCoral;
            else if (content.Watched)
                item.BackColor = Color.LightGray;

            displayedContents.Add(content);
        }

        /// <summary>
        /// Sorts content based on columns index from listview.
        /// </summary>
        /// <param name="lvColumnIndex">Column index to sort on</param>
        public void SortContents(int lvColumnIndex, bool forceSameOrder)
        {
            if (Contents == null)
                return;
            
            // Set ascending/descending
            if (lastSortColumn == lvColumnIndex)
            {
                if (!forceSameOrder)
                    ascendingSort = !ascendingSort;
            }
            else
                ascendingSort = true;

            Content.AscendingSort = ascendingSort;
            
            // Sort based on column that was clicked
            bool sorted = true;
            switch (lvColumnIndex)
            {
                case 0:
                    Contents.Sort(Content.CompareByPath);
                    break;
                case 1:
                    Contents.Sort(Content.CompareByName);
                    break;
                case 2:
                    Contents.Sort(Content.CompareByYear);
                    break;
                case 3:
                    Contents.Sort(Content.CompareByGenre);
                    break;
                default:
                    sorted = false;
                    break;
            }

            // Saved sorted column
            if (sorted)
                lastSortColumn = lvColumnIndex;

            // Re-display movies
            DisplayContent(false);
        }

        #endregion

        #region Events

        /// <summary>
        /// Watched property for selected movies can be set using
        /// 'W' and 'U' keys
        /// </summary>
        void ContentListView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                SetWatchedForSelected(true);
            else if (e.KeyCode == Keys.U)
                SetWatchedForSelected(false);
        }

        /// <summary>
        /// Sorts content in listview when a column is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ContentListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SortContents(e.Column, false);
        }

        /// <summary>
        /// Sets the watched property for all selected movies in movie listview.
        /// </summary>
        /// <param name="watched">The value to set the watched property to</param>
        public void SetWatchedForSelected(bool watched)
        {
            // Get the movie
            List<Content> selFolders;
            if (!GetSeletectedContents(out selFolders))
                return;

            // Set watched property for selected movies
            foreach (Content fldr in selFolders)
                fldr.Watched = watched;

            Organization.Movies.Save();

            // Refresh display
            DisplayContent(false);
        }

        /// <summary>
        /// Gets the currently selected movies from the movie listview.
        /// </summary>
        /// <param name="selContent">Resulting selected movies</param>
        /// <returns>Whether any selected movies were found</returns>
        public bool GetSeletectedContents(out List<Content> selContent)
        {
            // Initialize selected movies
            selContent = new List<Content>();

            // Get each selected folder
            for (int i = 0; i < this.SelectedIndices.Count; i++)
                selContent.Add(displayedContents[this.SelectedIndices[i]]);

            // Return selected movies
            return selContent.Count > 0;
        }

        /// <summary>
        /// Get first selected movie in movies listview.
        /// </summary>
        /// <param name="folder">Resulting selected movie</param>
        /// <returns>Whether a selected movie was found</returns>
        public bool GetSeletectedContent(out Content folder)
        {
            // Initialize selected movie
            folder = null;

            // Get all selected movies, if none found return false
            List<Content> selFolders;
            if (!GetSeletectedContents(out selFolders))
                return false;

            // Set selected movie to first selected and return true
            folder = selFolders[0];
            return true;
        }

        #endregion
    }
}
