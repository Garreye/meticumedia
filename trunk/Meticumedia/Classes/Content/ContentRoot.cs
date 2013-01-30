// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Meticumedia
{
    /// <summary>
    /// Represents a folder that contains sub-folders with content in them (e.g. root TV or Movie folder)
    /// </summary>
    public class ContentRootFolder
    {
        #region Properties

        /// <summary>
        /// Path to folder relative to parent content root folder
        /// </summary>
        public string SubPath { get; set; }

        /// <summary>
        /// File directory path to folder
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Specifies whether the software is allowed to make changes to existing
        /// files/subfolders contained within this folder
        /// </summary>
        public bool AllowOrganizing { get; set; }

        /// <summary>
        /// Specifies whether this is the default folder to move/copy 
        /// movies to during a scan
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// List of folders within this folder that contains movies.
        /// </summary>
        public List<ContentRootFolder> ChildFolders { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ContentRootFolder()
        {
            this.SubPath = string.Empty;
            this.FullPath = string.Empty;
            this.AllowOrganizing = true;
            this.Default = false;
            this.ChildFolders = new List<ContentRootFolder>();
        }

        /// <summary>
        /// Constructor with known path.
        /// </summary>
        /// <param name="path"></param>
        public ContentRootFolder(string path, string fullPath)
        {
            this.SubPath = path;
            this.FullPath = fullPath;
            this.AllowOrganizing = true;
            this.Default = false;
            this.ChildFolders = new List<ContentRootFolder>();
        }

        /// <summary>
        /// Constructor for cloning instance
        /// </summary>
        /// <param name="folder">instance to clone</param>
        public ContentRootFolder(ContentRootFolder folder)
        {
            this.SubPath = folder.SubPath;
            this.FullPath = folder.FullPath;
            this.AllowOrganizing = folder.AllowOrganizing;
            this.Default = folder.Default;
            this.ChildFolders = new List<ContentRootFolder>();
            foreach (ContentRootFolder subFolder in folder.ChildFolders)
                this.ChildFolders.Add(new ContentRootFolder(subFolder));
        }

        #endregion

        #region Methods

        /// <summary>
        /// Return path as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.FullPath;
        }

        /// <summary>
        /// Build list of sub-directories, with options to clear found flags for content contained in folder if desired.
        /// </summary>
        /// <param name="clearMoviesFound">Enables clearing of found flag on movie in this folder</param>
        /// <param name="clearShowsFound">Enables clearing of found flag on shows in this folder</param>
        /// <returns>List of sub-directories in this content folder</returns>
        public List<OrgPath> BuildSubDirectories(bool clearMoviesFound, bool clearShowsFound)
        {
            List<OrgPath> subFolders = new List<OrgPath>();
            BuildSubDirectories(this, subFolders, clearMoviesFound, clearShowsFound);
            return subFolders;
        }

        /// <summary>
        /// Recursively builds list of sub-directories in content folder
        /// </summary>
        /// <param name="folder">Current folder</param>
        /// <param name="subFolders">List of sub-directories</param>
        /// <param name="clearMoviesFound">Enables clearing of found flag on movie in this folder</param>
        /// <param name="clearShowsFound">Enables clearing of found flag on shows in this folder</param>
        private void BuildSubDirectories(ContentRootFolder folder, List<OrgPath> subFolders, bool clearMoviesFound, bool clearShowsFound)
        {
            // Clear found flag for all movies
            if (clearMoviesFound)
            {
                for (int i = 0; i < Organization.Movies.Count; i++)
                    if (Organization.Movies[i].RootFolder == folder.FullPath)
                        Organization.Movies[i].Found = false;
            }

            // Clear found flag on all movies
            if (clearShowsFound)
            {
                for (int i = 0; i < Organization.Shows.Count; i++)
                    if (Organization.Shows[i].RootFolder == folder.FullPath)
                        Organization.Shows[i].Found = false;
            }

            // Get each subfolder from the content folder that isn't a sub-content folder
            if (Directory.Exists(folder.FullPath))
            {
                string[] subFolderList = Directory.GetDirectories(folder.FullPath);

                foreach (string subFldr in subFolderList)
                {
                    bool isSubContentFolder = false;
                    foreach (ContentRootFolder subContent in folder.ChildFolders)
                    {
                        if (subFldr == subContent.FullPath)
                        {
                            isSubContentFolder = true;
                            break;
                        }
                    }

                    if (!isSubContentFolder)
                        subFolders.Add(new OrgPath(subFldr, false, true, folder, null));
                }
            

            // Recursively seach sub-content folders 
            foreach (ContentRootFolder subContent in folder.ChildFolders)
                subContent.BuildSubDirectories(subContent, subFolders, clearMoviesFound, clearShowsFound);
            }
        }

        #endregion

        #region XML

        /// <summary>
        /// Properties of this clasee that saved/loaded in XML elements
        /// </summary>
        private enum XmlElements { SubPath, FullPath, AllowOrganizing, Default, ChildFolders };

        /// <summary>
        /// Root XML element string for this class.
        /// </summary>
        private static readonly string ROOT_XML = "ContentRoot";

        /// <summary>
        /// Writes properties into XML elements
        /// </summary>
        /// <param name="xw">The XML writer</param>
        public void Save(XmlWriter xw)
        {
            // Start season element
            xw.WriteStartElement(ROOT_XML);

            // Save each property
            foreach (XmlElements element in Enum.GetValues(typeof(XmlElements)))
            {
                string value = null;
                switch (element)
                {
                    case XmlElements.SubPath:
                        value = this.SubPath;
                        break;
                    case XmlElements.FullPath:
                        value = this.FullPath;
                        break;
                    case XmlElements.AllowOrganizing:
                        value = this.AllowOrganizing.ToString();
                        break;
                    case XmlElements.Default:
                        value = this.Default.ToString();
                        break;
                    case XmlElements.ChildFolders:
                        xw.WriteStartElement(element.ToString());
                        foreach (ContentRootFolder subFolder in this.ChildFolders)
                            subFolder.Save(xw);
                        xw.WriteEndElement();
                        break;
                    default:
                        throw new Exception("Unkonw element!");
                }

                if (value != null)
                    xw.WriteElementString(element.ToString(), value);
            }

            // End season
            xw.WriteEndElement();
        }

        /// <summary>
        /// Loads properties from XML elements
        /// </summary>
        /// <param name="showNode">The XML node containt property elements</param>
        /// <returns>Whether XML was loaded properly</returns>
        public bool Load(XmlNode seasonNode)
        {
            // Check that we're on the right node
            if (seasonNode.Name != ROOT_XML)
                return false;

            // Go through elements of node
            foreach (XmlNode propNode in seasonNode.ChildNodes)
            {
                // Match the current node to a known element name
                XmlElements element;
                if (!Enum.TryParse<XmlElements>(propNode.Name, out element))
                    continue;

                // Set appropiate property from value in element
                string value = propNode.InnerText;
                switch (element)
                {
                    case XmlElements.SubPath:
                        this.SubPath = value;
                        break;
                    case XmlElements.FullPath:
                        this.FullPath = value;
                        break;
                    case XmlElements.AllowOrganizing:
                        bool allowOrg;
                        if (bool.TryParse(value, out allowOrg))
                            this.AllowOrganizing = allowOrg;
                        break;
                    case XmlElements.Default:
                        bool def;
                        if (bool.TryParse(value, out def))
                            this.Default = def;
                        break;
                    case XmlElements.ChildFolders:
                        this.ChildFolders = new List<ContentRootFolder>();
                        foreach(XmlNode subContentNode in propNode.ChildNodes)
                        {
                            ContentRootFolder subFolder = new ContentRootFolder();
                            subFolder.Load(subContentNode);
                            this.ChildFolders.Add(subFolder);
                        }
                        break;
                }
            }

            return true;
        }

        #endregion
    }
}
