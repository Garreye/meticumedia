using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Meticumedia.Classes
{

    public class ContentRootFolderCollection : ObservableCollection<ContentRootFolder>
    {
        #region Events

        /// <summary>
        /// Triggers PropertyChanged event.
        /// </summary>
        /// <param name="name">Name of the property that has changed value</param>
        protected void OnPropertyChanged(string name)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(name));
        }

        #endregion
        
        #region Properties

        public ContentType ContentType
        {
            get
            {
                return contentType;
            }
            set
            {
                contentType = value;
                OnPropertyChanged("ContentType");
            }
        }
        private ContentType contentType;

        /// <summary>
        /// Selection for how root folder to move content to is selected
        /// </summary>
        public ContentRootFolderSelectionCriteria Criteria
        {
            get
            {
                return criteria;
            }
            set
            {
                criteria = value;
                OnPropertyChanged("Criteria");
            }
        }
        private ContentRootFolderSelectionCriteria criteria = ContentRootFolderSelectionCriteria.Default;

        
        /// <summary>
        /// Controls what to do when genre match is not found. Only applicable if Selection set to GenreChild
        /// </summary>
        public ContentRootFolderGenreMatchMissType GenreMatchMiss
        {
            get
            {
                return genreMatchMiss;
            }
            set
            {
                genreMatchMiss = value;
                OnPropertyChanged("GenreMatchMiss");
            }
        }
        private ContentRootFolderGenreMatchMissType genreMatchMiss = ContentRootFolderGenreMatchMissType.AutoCreate;

        /// <summary>
        /// Rules for matching content to root folder. Only applicable if Selection set to Rules
        /// </summary>
        public ObservableCollection<ContentRootFolderMatchRule> MatchRules
        {
            get
            {
                return matchRules;
            }
            set
            {
                matchRules = value;
                OnPropertyChanged("MatchRules");
            }
        }
        private ObservableCollection<ContentRootFolderMatchRule> matchRules = new ObservableCollection<ContentRootFolderMatchRule>();

        #endregion

        private object folderFindLock = new object();

        public ContentRootFolderCollection(ContentType contentType)
            : base()
        {
            this.ContentType = contentType;
        }

        public ContentRootFolderCollection(ContentRootFolderCollection other)
            : base()
        {
            this.Clone(other);
        }

        #region Methods

        public void Clone(ContentRootFolderCollection other)
        {
            this.ContentType = other.ContentType;
            this.Clear();
            foreach (ContentRootFolder folder in other)
                this.Add(new ContentRootFolder(folder));
            this.Criteria = other.criteria;
            this.GenreMatchMiss = other.GenreMatchMiss;
            this.MatchRules.Clear();
            foreach (ContentRootFolderMatchRule rule in other.MatchRules)
                this.MatchRules.Add(new ContentRootFolderMatchRule(rule));
        }

        public List<ContentRootFolder> GetFolders(bool recursive)
        {
            List<ContentRootFolder> folderList = new List<ContentRootFolder>();
            foreach (ContentRootFolder folder in this)
                BuildFolderList(folderList, folder, recursive);
            return folderList;
        }

        private void BuildFolderList(List<ContentRootFolder> folderList, ContentRootFolder folder, bool recursive)
        {
            folderList.Add(folder);
            if (recursive)
                foreach (ContentRootFolder subFolder in folder.ChildFolders)
                    BuildFolderList(folderList, subFolder, recursive);
        }

        public ContentRootFolder GetFolderForContent(Content content)
        {
            lock (folderFindLock)
            {
                ContentRootFolder defaultFolder;
                GetDefaultFolder(this, out defaultFolder);
                if (defaultFolder == null)
                    return defaultFolder;

                if (content == null)
                    return defaultFolder;

                switch (this.Criteria)
                {
                    case ContentRootFolderSelectionCriteria.Default:
                        return defaultFolder;
                    case ContentRootFolderSelectionCriteria.GenreChild:
                        if (content.DisplayGenres.Count == 0)
                            return defaultFolder;

                        string genre = content.DisplayGenres[0];
                        ContentRootFolder genreFolder;
                        if (GetGenreFolder(genre, this, out genreFolder))
                            return genreFolder;

                        switch (this.GenreMatchMiss)
                        {
                            case ContentRootFolderGenreMatchMissType.Default:
                                return defaultFolder;
                            case ContentRootFolderGenreMatchMissType.AutoCreate:
                                ContentRootFolder newChild = new ContentRootFolder(this.ContentType, genre, System.IO.Path.Combine(defaultFolder.FullPath, genre));
                                newChild.Temporary = true;
                                defaultFolder.ChildFolders.Add(newChild);
                                Settings.Save();
                                return newChild;
                            default:
                                throw new Exception("Unkown genre missed condition");
                        }
                    case ContentRootFolderSelectionCriteria.Rules:
                        foreach (ContentRootFolderMatchRule rule in this.MatchRules)
                        {
                            if (rule.Match(content))
                            {
                                ContentRootFolder folderFromPath;
                                if (GetFolderFromPath(rule.Folder, this, out folderFromPath))
                                    return folderFromPath;
                            }
                        }
                        return defaultFolder;
                    default:
                        throw new Exception("Unknown content folder match criteria");
                }
            }
        }

        /// <summary>
        /// Gets movie folder that is set as default
        /// </summary>
        /// <param name="folders">List of movie folder to look through for default</param>
        /// <param name="defaultFolder">The resulting default movie folder</param>
        /// <returns>whether default was found</returns>
        private bool GetDefaultFolder(ObservableCollection<ContentRootFolder> folders, out ContentRootFolder defaultFolder)
        {
            foreach (ContentRootFolder folder in folders)
            {
                if (folder.Default)
                {
                    defaultFolder = folder;
                    return true;
                }

                if (GetDefaultFolder(folder.ChildFolders, out defaultFolder))
                    return true;
            }

            defaultFolder = null;
            return false;
        }

        /// <summary>
        /// Find  root folder who's name match specified genre
        /// </summary>
        /// <param name="genre"></param>
        /// <param name="folders"></param>
        /// <param name="genreFolder"></param>
        /// <returns></returns>
        private bool GetGenreFolder(string genre, ObservableCollection<ContentRootFolder> folders, out ContentRootFolder genreFolder)
        {
            foreach (ContentRootFolder folder in folders)
            {
                if (Path.GetFileName(folder.FullPath).ToLower() == genre.ToLower())
                {
                    genreFolder = folder;
                    return true;
                }

                if (GetGenreFolder(genre, folder.ChildFolders, out genreFolder))
                    return true;
            }

            genreFolder = null;
            return false;
        }

        /// <summary>
        /// Get root folder that match path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="folder"></param>
        /// <returns></returns>
        public bool GetFolderFromPath(string path, out ContentRootFolder folder)
        {
            return GetFolderFromPath(path, this, out folder);
        }


        /// <summary>
        /// Get content folder object from path string.
        /// </summary>
        /// <param name="folderPath">Path of content folder</param>
        /// <param name="folders">Content folders to look through</param>
        /// <param name="folder">Resulting content folder matching path</param>
        /// <returns>True if folder was found</returns>
        private bool GetFolderFromPath(string folderPath, ObservableCollection<ContentRootFolder> folders, out ContentRootFolder folder)
        {
            foreach (ContentRootFolder contentFolder in folders)
            {
                if (contentFolder.FullPath == folderPath)
                {
                    folder = contentFolder;
                    return true;
                }

                if (GetFolderFromPath(folderPath, contentFolder.ChildFolders, out folder))
                    return true;
            }

            folder = null;
            return false;
        }

        /// <summary>
        /// Remove children marked temporary (created from content matching) that were never created.
        /// </summary>
        /// <param name="folder"></param>
        public void RemoveTemporaryChildren(ContentRootFolder folder)
        {
            for (int i = folder.ChildFolders.Count - 1; i >= 0; i--)
                if (folder.ChildFolders[i].Temporary && !Directory.Exists(folder.ChildFolders[i].FullPath))
                    folder.ChildFolders.RemoveAt(i);
                else
                {
                    folder.ChildFolders[i].Temporary = false;
                    RemoveTemporaryChildren(folder.ChildFolders[i]);
                }
        }

        #endregion

        #region XML

        /// <summary>
        /// Element names for properties that need to be saved to XML
        /// </summary>
        private enum XmlElements
        {
            Folders, Criteria, GenreMatchMiss, MatchRules
        };

        /// <summary>
        /// Writes properties into XML elements
        /// </summary>
        /// <param name="xw">Writer for accessing XML file</param>
        public void Save(XmlWriter xw)
        {
            // Loop through elements/properties
            foreach (XmlElements element in Enum.GetValues(typeof(XmlElements)))
            {
                // Set value from appropriate property
                string value = null;
                switch (element)
                {
                    case XmlElements.Folders:
                        xw.WriteStartElement(element.ToString());
                        foreach (ContentRootFolder folder in this)
                            folder.Save(xw);
                        xw.WriteEndElement();
                        break;
                    case XmlElements.GenreMatchMiss:
                        value = this.GenreMatchMiss.ToString();
                        break;
                    case XmlElements.MatchRules:
                        xw.WriteStartElement(element.ToString());
                        foreach (ContentRootFolderMatchRule rule in this.MatchRules)
                            rule.Save(xw);
                        xw.WriteEndElement();
                        break;
                    case XmlElements.Criteria:
                        value = this.Criteria.ToString();
                        break;
                    default:
#if DEBUG
                        throw new Exception("Unknown XML element");
#endif
                        break;
                }
                // If value is valid then write it
                if (value != null)
                    xw.WriteElementString(element.ToString(), value);
            }
        }

        /// <summary>
        /// Loads properties from XML elements
        /// </summary>
        /// <param name="collectionNode">The XML node containt property elements</param>
        public void Load(XmlNode collectionNode)
        {
            // Go through elements of node
            foreach (XmlNode propNode in collectionNode.ChildNodes)
            {
                // Match the current node to a known element name
                XmlElements element;
                if (!Enum.TryParse<XmlElements>(propNode.Name, out element))
                    continue;

                // Set appropiate property from value in element
                string value = propNode.InnerText;
                switch (element)
                {
                    case XmlElements.Folders:
                        this.Clear();
                        foreach(XmlNode folderNode in propNode.ChildNodes)
                        {
                            ContentRootFolder folder = new ContentRootFolder(this.ContentType);
                            folder.Load(folderNode);
                            this.Add(folder);
                        }
                        break;
                    case XmlElements.GenreMatchMiss:
                        ContentRootFolderGenreMatchMissType matchMiss;
                        if (Enum.TryParse<ContentRootFolderGenreMatchMissType>(value, out matchMiss))
                            this.GenreMatchMiss = matchMiss;
                        break;
                    case XmlElements.MatchRules:
                        this.MatchRules.Clear();
                        foreach (XmlNode ruleNode in propNode.ChildNodes)
                        {
                            ContentRootFolderMatchRule rule = new ContentRootFolderMatchRule();
                            rule.Load(ruleNode);
                            this.MatchRules.Add(rule);
                        }
                        break;
                    case XmlElements.Criteria:
                        ContentRootFolderSelectionCriteria sel;
                        if (Enum.TryParse<ContentRootFolderSelectionCriteria>(value, out sel))
                            this.Criteria = sel;
                        break;
                    default:
#if DEBUG
                        throw new Exception("Unknown XML element");
#endif
                        break;
                }
            }

            // Remove temporary children
            foreach (ContentRootFolder folder in this)
                RemoveTemporaryChildren(folder);
        }

        #endregion
    }
}
