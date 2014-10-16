// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Organization path item. Contains file info and property related to organization (pushed from parent content folder).
    /// </summary>
    public class OrgPath
    {
        /// <summary>
        /// Path to file
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Indicates if file should be copied (or moved if false) during organization action.
        /// (Pushed from parent folder)
        /// </summary>
        public bool Copy { get; set; }

        /// <summary>
        /// Indicates if file is allowed to be deleted (pushed from parent folder)
        /// </summary>
        public bool AllowDelete { get; set; }

        /// <summary>
        /// Root folder file belongs to (if any)
        /// </summary>
        public ContentRootFolder RootFolder { get; set; }

        /// <summary>
        /// Scan folder file source belongs to (if any)
        /// </summary>
        public OrgFolder OrgFolder { get; set; }

        /// <summary>
        /// Content associated with file (if any)
        /// </summary>
        public Content Content { get; set; }

        /// <summary>
        /// Constructor for file from scan folder
        /// </summary>
        /// <param name="path">file's path</param>
        /// <param name="copy">whether file is allowed to be copied</param>
        /// <param name="allowDelete">whether file is allowed to be deleted</param>
        /// <param name="folder">scan folder file belongs to</param>
        public OrgPath(string path, bool copy, bool allowDelete, OrgFolder folder)
        {
            this.Path = path;
            this.Copy = copy;
            this.AllowDelete = allowDelete;
            this.OrgFolder = folder;
        }

        /// <summary>
        /// Constructor for file from content folder (movie or tv)
        /// </summary>
        /// <param name="path">file's path</param>
        /// <param name="copy">whether file is allowed to be copied</param>
        /// <param name="allowDelete">whether file is allowed to be deleted</param>
        /// <param name="folder">content folder file belongs to</param>
        public OrgPath(string path, bool copy, bool allowDelete, ContentRootFolder folder, Content content)
        {
            this.Path = path;
            this.Copy = copy;
            this.AllowDelete = allowDelete;
            this.RootFolder = folder;
            this.Content = content;
        }

        public override string ToString()
        {
            return this.Path;
        }
    }
}
