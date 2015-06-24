// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;
using System.ComponentModel;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Organization folder, aka "scan folder". Used for scanning for new files that need to be organized (e.g. download folder)
    /// </summary>
    public class OrgFolder : INotifyPropertyChanged
    {
        public static readonly OrgFolder AllFolders = new OrgFolder("All Folders");
        
        #region Properties
        
        /// <summary>
        /// Path to the folder
        /// </summary>
        public string FolderPath
        {
            get
            {
                return folderPath;
            }
            set
            {
                folderPath = value;
                OnPropertyChanged("FolderPath");
            }
        }

        private string folderPath = string.Empty;

        /// <summary>
        /// Whether file will be copied instead of moved from the folder
        /// </summary>
        public bool CopyFrom
        {
            get
            {
                return copyFrom;
            }
            set
            {
                copyFrom = value;
                OnPropertyChanged("CopyFrom");
            }
        }

        private bool copyFrom = false;

        /// <summary>
        /// Whether the software should do recursive file searches in the folder
        /// </summary>
        public bool Recursive
        {
            get
            {
                return recursive;
            }
            set
            {
                recursive = value;
                OnPropertyChanged("Recursive");
            }
        }

        private bool recursive = true;

        /// <summary>
        /// Whether the file are allowed to be set to be deleted when organizing the folder
        /// </summary>
        public bool AllowDeleting
        {
            get
            {
                return allowDeleting;
            }
            set
            {
                allowDeleting = value;
                OnPropertyChanged("AllowDeleting");
            }
        }

        private bool allowDeleting = true;

        /// <summary>
        /// Whether to automatically delete empty sub-fodlers during processing
        /// </summary>
        public bool AutomaticallyDeleteEmptyFolders
        {
            get
            {
                return automaticallyDeleteEmptyFolders;
            }
            set
            {
                automaticallyDeleteEmptyFolders = value;
                OnPropertyChanged("AutomaticallyDeleteEmptyFolders");
            }
        }

        private bool automaticallyDeleteEmptyFolders = true;

        /// <summary>
        /// List of files to ignore during scan. Intended for use with directories that are copied from, 
        /// so that once a file has been copied to content folder it is skipped during scanning in future
        /// </summary>
        private List<string> ignoreFiles = new List<string>();

        public int Id { get; private set; }

        private static int idCnt = 0;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor with known data.
        /// </summary>
        /// <param name="path">path to folder</param>
        /// <param name="copyFrom">set whether folder should be copied rather than moved from</param>
        /// <param name="recursive">sets whether scans on folder should be recursive</param>
        /// <param name="allowDeleting">sets whether software is allowed to delete file from folder</param>
        /// <param name="checkForEmptyFolder">sets whether software checks if the folder is empty</param>
        public OrgFolder(string path, bool copyFrom, bool recursive, bool allowDeleting, bool checkForEmptyFolder)
            : this(path)
        {
            this.CopyFrom = copyFrom;
            this.Recursive = recursive;
            this.AllowDeleting = allowDeleting;
            this.AutomaticallyDeleteEmptyFolders = checkForEmptyFolder;
            this.ignoreFiles = new List<string>();
        }

        /// <summary>
        /// Constructor with just the path. Defaults set for other properties.
        /// </summary>
        /// <param name="path"></param>
        public OrgFolder(string path)
            : this()
        {
            this.FolderPath = path;
        }

        /// <summary>
        /// Default constructor. Path will be empty, other properties set to default.
        /// </summary>
        public OrgFolder()
        {
            this.Id = idCnt++;
        }

        /// <summary>
        /// Constructor for copying instance.
        /// </summary>
        public OrgFolder(OrgFolder folder)
        {
            Clone(folder);
        }

        public void Clone(OrgFolder folder)
        {
            if (this.FolderPath != folder.FolderPath)
                this.FolderPath = folder.FolderPath;
            if (this.CopyFrom != folder.CopyFrom)
                this.CopyFrom = folder.CopyFrom;
            if (this.Recursive != folder.Recursive)
                this.Recursive = folder.Recursive;
            if (this.AllowDeleting != folder.AllowDeleting)
                this.AllowDeleting = folder.AllowDeleting;
            this.AutomaticallyDeleteEmptyFolders = folder.AutomaticallyDeleteEmptyFolders;
            this.ignoreFiles.Clear();
            foreach (string ignrFile in folder.ignoreFiles)
                this.ignoreFiles.Add(ignrFile);
            if (this.Id != folder.Id)
                this.Id = folder.Id;
        }

        #endregion

        #region Ignore Files

        /// <summary>
        /// Add file to list to ignore during scans.
        /// </summary>
        /// <param name="path"></param>
        public void AddIgnoreFile(string path)
        {
            // Add relative path
            ignoreFiles.Add(path.Remove(0, this.FolderPath.Length + 1));
        }

        /// <summary>
        /// Remove file from list to ignore during scans.
        /// </summary>
        /// <param name="path"></param>
        public bool RemoveIgnoreFile(string path)
        {
            string relPath = path.Remove(0, this.FolderPath.Length + 1);
            if (ignoreFiles.Contains(relPath))
            {
                ignoreFiles.Remove(relPath);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check whether a file is set to be ignored.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsIgnored(string path)
        {
            if (path.Length > this.FolderPath.Length)
                return ignoreFiles.Contains(path.Remove(0, this.FolderPath.Length + 1));
            else
                return false;
        }

        #endregion

        #region XML

        /// <summary>
        /// Element names for properties that need to be saved to XML.
        /// </summary>
        private enum XmlElements { FolderPath, CopyFrom, Recursive, AllowDeleting, CheckForEmptyFolder, IgnoreFiles };

        /// <summary>
        /// Root XML element for saving instance of this class.
        /// </summary>
        private static readonly string ROOT_XML = "OrgFolder";

        /// <summary>
        /// XML element name for individual files path to be ignored.
        /// </summary>
        private static readonly string IGNORE_XML = "IgnoreFile";

        /// <summary>
        /// Adds OrgFolder properties to XML file.
        /// </summary>
        /// <param name="xw">XML writer to add to</param>
        public void Save(XmlWriter xw)
        {
            // Start season element
            xw.WriteStartElement(ROOT_XML);

            foreach (XmlElements element in Enum.GetValues(typeof(XmlElements)))
            {
                string value = null;
                switch (element)
                {
                    case XmlElements.FolderPath:
                        value = this.FolderPath;
                        break;
                    case XmlElements.CopyFrom:
                        value = this.CopyFrom.ToString();
                        break;
                    case XmlElements.Recursive:
                        value = this.Recursive.ToString();
                        break;
                    case XmlElements.AllowDeleting:
                        value = this.AllowDeleting.ToString();
                        break;
                    case XmlElements.CheckForEmptyFolder:
                        value = this.AutomaticallyDeleteEmptyFolders.ToString();
                        break;
                    case XmlElements.IgnoreFiles:
                        xw.WriteStartElement(element.ToString());
                        foreach (string path in this.ignoreFiles)
                            xw.WriteElementString(IGNORE_XML, path);
                        xw.WriteEndElement();
                        break;
                    default:
                        throw new Exception("Unkonw element!");
                }

                if (value != null)
                    xw.WriteElementString(element.ToString(), value);
            }

            xw.WriteEndElement();
        }

        /// <summary>
        /// Loads instance from XML.
        /// </summary>
        /// <param name="seasonNode">Node to load XML from</param>
        public bool Load(XmlNode seasonNode)
        {
            if (seasonNode.Name != ROOT_XML)
                return false;

            foreach (XmlNode propNode in seasonNode.ChildNodes)
            {
                XmlElements element;
                if (!Enum.TryParse<XmlElements>(propNode.Name, out element))
                    continue;

                string value = propNode.InnerText;
                switch (element)
                {
                    case XmlElements.FolderPath:
                        this.FolderPath = value;
                        break;
                    case XmlElements.CopyFrom:
                        bool copyFrom;
                        bool.TryParse(value, out copyFrom);
                        this.CopyFrom = copyFrom;
                        break;
                    case XmlElements.Recursive:
                        bool recursive;
                        bool.TryParse(value, out recursive);
                        this.Recursive = recursive;
                        break;
                    case XmlElements.AllowDeleting:
                        bool allowDeleting;
                        bool.TryParse(value, out allowDeleting);
                        this.AllowDeleting = allowDeleting;
                        break;
                    case XmlElements.CheckForEmptyFolder:
                        bool checkForEmptyFolder;
                        bool.TryParse(value, out checkForEmptyFolder);
                        this.AutomaticallyDeleteEmptyFolders = checkForEmptyFolder;
                        break;
                    case XmlElements.IgnoreFiles:
                        this.ignoreFiles = new List<string>();
                        foreach (XmlNode ignoreNode in propNode.ChildNodes)
                            ignoreFiles.Add(ignoreNode.InnerText);
                        break;
                }
            }

            return true;
        }

        #endregion

        public override string ToString()
        {
            return this.FolderPath;
        }
    }
}
