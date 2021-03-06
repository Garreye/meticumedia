﻿// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Represents a root directory that contains directories that are each linked to content (TV show or movies)
    /// </summary>
    public class ContentRootFolder : INotifyPropertyChanged
    {
        public static readonly ContentRootFolder AllTvFolders = new ContentRootFolder(ContentType.TvShow, "All TV Folders", "All TV Folders");
        public static readonly ContentRootFolder AllMoviesFolders = new ContentRootFolder(ContentType.Movie, "All Movie Folders", "All Movie Folders");
        
        #region Properties

        /// <summary>
        /// Path to folder relative to parent content root folder
        /// </summary>
        public string SubPath
        {
            get
            {
                return subPath;
            }
            set
            {
                subPath = value;
                OnPropertyChanged("SubPath");
            }
        }

        private string subPath = string.Empty;

        /// <summary>
        /// File directory path to folder
        /// </summary>
        public string FullPath
        {
            get
            {
                return fullPath;
            }
            set
            {
                fullPath = value;
                OnPropertyChanged("FullPath");
            }
        }

        private string fullPath = string.Empty;

        /// <summary>
        /// Specifies whether the software is allowed to make changes to existing
        /// files/subfolders contained within this folder
        /// </summary>
        public bool AllowOrganizing
        {
            get
            {
                return allowOrganizing;
            }
            set
            {
                allowOrganizing = value;
                OnPropertyChanged("AllowOrganizing");
            }
        }

        private bool allowOrganizing = true;

        /// <summary>
        /// Specifies whether this is the default folder to move/copy 
        /// content to during a scan
        /// </summary>
        public bool Default
        {
            get
            {
                return isDefault;
            }
            set
            {
                isDefault = value;
                OnPropertyChanged("Default");
            }
        }

        private bool isDefault = false;

        /// <summary>
        /// Specified whether all sub directories should automatically be set as child root folder
        /// </summary>
        public bool AllSubFoldersChildRootFolder
        {
            get
            {
                return allSubFoldersChildRootFolder;
            }
            set
            {
                allSubFoldersChildRootFolder = value;
                OnPropertyChanged("AllSubFoldersChildRootFolder");

                if (value)
                    SetAllSubDirsAsChildren();
            }
        }

        private bool allSubFoldersChildRootFolder = false;


        /// <summary>
        /// List of child root folders.
        /// </summary>
        public ObservableCollection<ContentRootFolder> ChildFolders
        {
            get
            {
                return childFolders;
            }
            set
            {
                childFolders = value;
                OnPropertyChanged("ChildFolders");
            }
        }

        private ObservableCollection<ContentRootFolder> childFolders = new ObservableCollection<ContentRootFolder>();

        /// <summary>
        /// Type of content contained in folder
        /// </summary>
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

        public int Id { get; private set; }

        private static int idCnt = 0;

        public bool Temporary { get; set; }

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

        /// <summary>
        /// Static event that fires when folder updating progress changes
        /// </summary>
        public static event EventHandler<OrgProgressChangesEventArgs> UpdateProgressChange;

        /// <summary>
        /// Triggers TvFolderUpdateProgressChange event
        /// </summary>
        public static void OnUpdateProgressChange(ContentRootFolder sender, bool newContent, int percent, string msg)
        {
            if (UpdateProgressChange != null)
                UpdateProgressChange(sender, new OrgProgressChangesEventArgs(newContent, percent, msg));
        }

        /// <summary>
        /// Static event that fires when folder updating complete
        /// </summary>
        public static event EventHandler UpdateProgressComplete;

        /// <summary>
        /// Triggers TvFolderUpdateProgressChange event
        /// </summary>
        public static void OnUpdateProgressComplete(ContentRootFolder sender)
        {
            UpdateProgressComplete?.Invoke(sender, new EventArgs());
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ContentRootFolder(ContentType type)
        {
            this.ContentType = type;
            childFolders.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(childFolders_CollectionChanged);
            this.Id = idCnt++;
            this.Temporary = false;
        }

        /// <summary>
        /// Constructor with known path.
        /// </summary>
        /// <param name="path"></param>
        public ContentRootFolder(ContentType type, string path, string fullPath)
            : this(type)
        {
            this.SubPath = path;
            this.FullPath = fullPath;
        }

        /// <summary>
        /// Constructor for cloning instance
        /// </summary>
        /// <param name="folder">instance to clone</param>
        public ContentRootFolder(ContentRootFolder folder)
            : this(folder.ContentType)
        {
            this.SubPath = folder.SubPath;
            this.FullPath = folder.FullPath;
            this.AllowOrganizing = folder.AllowOrganizing;
            this.Default = folder.Default;
            this.ChildFolders.Clear();
            foreach (ContentRootFolder subFolder in folder.ChildFolders)
                this.ChildFolders.Add(new ContentRootFolder(subFolder));
            this.Id = folder.Id;
            this.Temporary = folder.Temporary;
            this.AllSubFoldersChildRootFolder = folder.AllSubFoldersChildRootFolder;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Child folders collection change chained up to property changed of this.
        /// </summary>
        private void childFolders_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("ChildFolders");
        }

        /// <summary>
        /// Return path as string.
        /// </summary>
        /// <returns>Path to folder</returns>
        public override string ToString()
        {
            return this.FullPath;
        }

        /// <summary>
        /// Build list of sub-directories, with options to clear found flags for content contained in folder if desired.
        /// </summary>
        /// <param name="clearMoviesFound">Enables clearing of found flag on movies in this folder</param>
        /// <param name="clearShowsFound">Enables clearing of found flag on shows in this folder</param>
        /// <returns>List of sub-directories in this content folder</returns>
        public List<OrgPath> BuildSubDirectories(ContentType contentType)
        {
            bool clearMoviesFound = contentType == ContentType.Movie;
            bool clearShowsFound = contentType == ContentType.TvShow;
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

        /// <summary>
        /// Get available genres from content 
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public GenreCollection GetGenres()
        {
            GenreCollection genres = new GenreCollection(this.ContentType);

            switch (this.ContentType)
            {
                case ContentType.Movie:
                    for(int i=0;i<Organization.Movies.Count;i++)
                        foreach (string genre in Organization.Movies[i].DatabaseGenres)
                            if (!genres.Contains(genre))
                                genres.Add(genre);
                    break;
                case ContentType.TvShow:
                    for (int i = 0; i < Organization.Shows.Count; i++)
                        foreach (string genre in Organization.Shows[i].DatabaseGenres)
                            if (!genres.Contains(genre))
                                genres.Add(genre);
                    break;
            }

            
            return genres;
        }

        /// <summary>
        /// Builds a list of content matching filter that is contained within a root folder
        /// </summary>
        /// <param name="genre">Filter for genre type - use "All" to disable filter</param>
        /// <param name="contents">List to add content to</param>
        /// <param name="yearFilter">Enables year filtering</param>
        /// <param name="minYear">Minimum for year filter</param>
        /// <param name="maxYear">Maximum for year filter</param>
        /// <param name="nameFilter">String that must be contained in content name - empty string disables filter</param>
        public void GetContent(bool recursive, bool genreEnable, GenreCollection genre, List<Content> contents, bool yearFilter, int minYear, int maxYear, string nameFilter)
        {
            // Go through all movies
            ContentCollection contentCollection = GetContentCollection();
            lock (contentCollection.ContentLock)
            {
                //Console.WriteLine(contentCollection.ToString() + " lock get");
                foreach (Content content in contentCollection)
                {
                    // Apply genre filter
                    bool genreMatch = false;
                    if (content.DatabaseGenres != null && !genreMatch)
                        foreach (string contentGenre in content.DatabaseGenres)
                            if (genre.Contains(contentGenre))
                            {
                                genreMatch = true;
                                break;
                            }

                    // Apply year filter
                    bool yearMatch = !yearFilter || (content.DatabaseYear >= minYear && content.DatabaseYear <= maxYear);

                    // Apply text filter
                    bool nameMatch = string.IsNullOrEmpty(nameFilter) || content.DatabaseName.ToLower().Contains(nameFilter.ToLower());

                    // Check if movie is in the folder
                    if (ContainsContent(content, recursive) && ApplyContentFilter(content, genreEnable, genre, yearFilter, minYear, maxYear, nameFilter))
                        contents.Add(content);
                }
            }
            //Console.WriteLine(contentCollection.ToString() + " release get");
        }

        /// <summary>
        /// Puts a single content item through filtering
        /// </summary>
        /// <param name="content">Content to filter</param>
        /// <param name="genre">Genre content must belong to</param>
        /// <param name="yearFilter">Enable for year filter</param>
        /// <param name="minYear">Minimum for year filter</param>
        /// <param name="maxYear">Maximum for year filter</param>
        /// <param name="nameFilter">String that must be contained in content name - empty string disables filter</param>
        /// <returns></returns>
        public bool ApplyContentFilter(Content content, bool genreEnable, GenreCollection genre, bool yearFilter, int minYear, int maxYear, string nameFilter)
        {
            // Apply genre filter
            bool genreMatch = !genreEnable;
            if (content.DatabaseGenres != null && !genreMatch)
                foreach (string contentGenre in content.DatabaseGenres)
                    if (genre.Contains(contentGenre))
                    {
                        genreMatch = true;
                        break;
                    }

            // Apply year filter
            bool yearMatch = !yearFilter || (content.DatabaseYear >= minYear && content.DatabaseYear <= maxYear);

            // Apply text filter
            bool nameMatch = string.IsNullOrEmpty(nameFilter) || content.DatabaseName.ToLower().Contains(nameFilter.ToLower());

            bool test = genreMatch && yearMatch && nameMatch;
            if (!test)
                test = true;

            // Check if movie is in the folder
            return genreMatch && yearMatch && nameMatch;
        }

        /// <summary>
        /// Check if a root folder contains a specific content instance. Called
        /// recursively on child root folders.
        /// </summary>
        /// <param name="content">The movie to check for</param>
        /// <returns>Whether the movie is contained in the folder</returns>
        public bool ContainsContent(Content content, bool recursive)
        {
            // Check if movie content folder matches
            if (content.RootFolder == this.FullPath)
                return true;
            else if (recursive)
                // Recursion on sub-folders
                foreach (ContentRootFolder subFolder in this.ChildFolders)
                    if (subFolder.ContainsContent(content, recursive))
                        return true;

            // No match
            return false;
        }

        /// <summary>
        /// Get root folder that matches path string, recursive search
        /// </summary>
        /// <param name="path">Path to match to</param>
        /// <param name="baseFolder">Current root folder being searches</param>
        /// <param name="matchedFolder">Resulting matched root folder</param>
        /// <returns>Whether match was found</returns>
        public static bool GetMatchingRootFolder(string path, ContentRootFolder baseFolder, out ContentRootFolder matchedFolder)
        {
            if (path == baseFolder.FullPath)
            {
                matchedFolder = baseFolder;
                return true;
            }
            else
                foreach (ContentRootFolder child in baseFolder.ChildFolders)
                {
                    if (GetMatchingRootFolder(path, child, out matchedFolder))
                        return true;
                }

            matchedFolder = null;
            return false;
        }

        /// <summary>
        /// Set all sub directories to child root folders
        /// </summary>
        public void SetAllSubDirsAsChildren()
        {
            // Get sub-dirs
            string[] subDirs = System.IO.Directory.GetDirectories(this.FullPath);
            for (int i = 0; i < subDirs.Length; i++)
                subDirs[i] = Path.GetFileName(subDirs[i]);

            // Remove child folders that no longer exists
            for (int i = this.ChildFolders.Count - 1; i >= 0; i--)
            {
                bool exists = false;
                foreach (string subDir in subDirs)
                    if (System.IO.Path.Combine(this.FullPath, subDir) == this.ChildFolders[i].fullPath)
                    {
                        exists = true;
                        break;
                    }

                if (!exists)
                    this.ChildFolders.RemoveAt(i);
            }

            // Add all sub-dirs as children
            foreach (string subDir in subDirs)
            {
                string newPath =  System.IO.Path.Combine(this.FullPath, subDir);
                
                bool exists = false;
                foreach(ContentRootFolder child in this.ChildFolders)
                   if(child.FullPath == newPath)
                   {
                       exists = true;
                       break;
                   }

                if(!exists)
                {
                    ContentRootFolder newChild = new ContentRootFolder(this.ContentType, subDir, newPath);
                    this.ChildFolders.Add(newChild);
                }
            }
            
        }

        /// <summary>
        /// Build sub-directories of content folder as string array of the paths.
        /// </summary>
        /// <returns></returns>
        public List<string> GetFolderSubDirectoryNamesThatArentChildren()
        {
            if (!System.IO.Directory.Exists(this.FullPath))
                return new List<string>();


            List<string> nonChildSubDirs = new List<string>();
            string[] subDirs = System.IO.Directory.GetDirectories(this.FullPath);
            for (int i = 0; i < subDirs.Length; i++)
            {                
                bool exists = false;
                foreach(ContentRootFolder child in this.ChildFolders)
                    if (child.FullPath == subDirs[i])
                   {
                       exists = true;
                       break;
                   }

                if (!exists)
                {
                    string[] dirs = subDirs[i].Split('\\');
                    nonChildSubDirs.Add(dirs[dirs.Length - 1]);
                }
            }

            return nonChildSubDirs;
        }

        #endregion

        #region Updating

        /// <summary>
        /// Current update number identifier. Incremented each time update is performed, used to identify each update thread.
        /// </summary>
        private int updateNumber = 0;

        /// <summary>
        /// Flag for update cancellation
        /// </summary>
        private bool updateCancelled = false;

        /// <summary>
        /// Search through all sub-folders and attempt to assign each one to content.
        /// </summary>
        /// <param name="fastUpdate">Whether update is fast, skips detailed updating(e.g. episodes for shows)</param>
        /// <param name="idsToUpdate">List of dtabase IDs that need updating</param>
        /// <param name="cancel">Cancelation flag</param>
        /// <param name="serverTime">Time on online database server</param>
        /// <returns>Whether update was completed without cancelation</returns>
        public bool UpdateContent(bool fastUpdate, ref bool cancel)
        {
            updateCancelled = false;
            
            // Update progress
            string progressMsg = "Update of '" + this.FullPath + "' started - Building threads";
            OnUpdateProgressChange(this, false, 0, progressMsg);

            // Initialize processing - First pass
            OrgProcessing firstPass = new OrgProcessing(UpdateProcess);
            updateNumber = firstPass.ProcessNumber;

            // Run 1st pass (build of sub-dirs is recursive, so all child root folder sub-dirs will be included)
            firstPass.Run(BuildSubDirectories(this.ContentType), ref cancel, true, Settings.General.NumProcessingThreads);
            updateCancelled = cancel;

            // Initialize processing - Second pass
            OrgProcessing secondPass = new OrgProcessing(UpdateProcess);
            updateNumber = secondPass.ProcessNumber;

            // Run 2nd pass (build of sub-dirs is recursive, so all child root folder sub-dirs will be included)
            secondPass.Run(BuildSubDirectories(this.ContentType), ref cancel, false, Settings.General.NumProcessingThreads); 
            updateCancelled = cancel;

            // Get content collection to add content to
            ContentCollection content = GetContentCollection();

            // Remove shows that no longer exists
            if (!cancel)
                content.RemoveMissing();

            // Save changes
            //content.Sort();
            content.Save();

            // Set progress to completed
            progressMsg = "Update of '" + this.FullPath + "' complete!";
            OnUpdateProgressChange(this, false, 100, progressMsg);
            OnUpdateProgressComplete(this);

            // Return whether update was completed without cancelation
            return !cancel;
        }

        /// <summary>
        /// Update processing method (thread) for single content folder in root.
        /// </summary>
        /// <param name="orgPath">Organization path instance to be processed</param>
        /// <param name="pathNum">The path's number out of total being processed</param>
        /// <param name="totalPaths">Total number of paths being processed</param>
        /// <param name="processNumber">The identifier for the OrgProcessing instance</param>
        /// <param name="numItemsProcessed">Number of paths that have been processed - used for progress updates</param>
        /// <param name="numItemsStarted">Number of paths that have had been added to thread pool for processing</param>
        /// <param name="processSpecificArgs">Arguments specific to this process</param>
        private void UpdateProcess(OrgPath orgPath, int pathNum, int totalPaths, int processNumber, ref int numItemsProcessed, ref int numItemsStarted, object processSpecificArgs)
        {
            // Check for cancellation - this method is called from thread pool, so cancellation could have occured by the time this is run
            if (updateCancelled || this.updateNumber != processNumber)
                return;

            // First pass run does quick folder update by skipping online database searching
            bool firstPass = (bool)processSpecificArgs;
            string passString = firstPass ? " (First Pass)" : " (Second Pass)";

            // Set processing messge
            string progressMsg = "Updating of '" + orgPath.RootFolder.FullPath + "'" + passString + " - '" + Path.GetFileName(orgPath.Path) + "' started";
            OnUpdateProgressChange(this, false, CalcProgress(numItemsProcessed, numItemsStarted, totalPaths), progressMsg);

            // Get content collection to add content to
            ContentCollection content = GetContentCollection();

            // Check if folder already has a match to existing content
            bool contentExists = false;
            bool contentComplete = false;
            Content newContent = null;
            int index = 0;
            for (int j = 0; j < content.Count; j++)
                if (Path.Equals(orgPath.Path, content[j].Path))
                {
                    contentExists = true;
                    content[j].Found = true;
                    if (!string.IsNullOrEmpty(content[j].DatabaseName))
                        contentComplete = true;
                    newContent = content[j];
                    index = j;
                    break;
                }

            // Set completed progess message
            progressMsg = "Updating of '" + orgPath.RootFolder.FullPath + "'" + passString + " - '" + Path.GetFileName(orgPath.Path) + "' complete";

            // Check if content found
            if (contentExists && contentComplete)
            {
                // Check if content needs updating
                if ((DateTime.Now - newContent.LastUpdated).TotalDays > 7 && this.ContentType == ContentType.TvShow)
                    newContent.UpdateInfoFromDatabase();

                // Update progress
                if (this.updateNumber == processNumber)
                    OnUpdateProgressChange(this, false, CalcProgress(numItemsProcessed, numItemsStarted, totalPaths), progressMsg);
                
                return;
            }

            // Folder wasn't matched to an existing content instance, try tmatch folder to content from online database
            Content match;
            bool matchSucess;
            switch (this.ContentType)
            {
                case ContentType.TvShow:
                    TvShow showMatch;
                    matchSucess = SearchHelper.TvShowSearch.PathMatch(orgPath.RootFolder.FullPath, orgPath.Path, firstPass, true, out showMatch);
                    match = showMatch;
                    break;
                case ContentType.Movie:
                    Movie movieMatch;
                    matchSucess = SearchHelper.MovieSearch.PathMatch(orgPath.RootFolder.FullPath, orgPath.Path, firstPass, true, out movieMatch);
                    match = movieMatch;
                    break;
                default:
                    throw new Exception("unknown content type");
            }


            // Check that current process hasn't been replaced - search can be slow, so update may have been cancelled by the time it gets here
            if (updateCancelled || this.updateNumber != processNumber)
                return;

            // Folder already existed, but wasn't previously match to valid content
            if (contentExists && matchSucess)
            {
                switch (this.ContentType)
                {
                    case ContentType.TvShow:
                        ((TvShow)newContent).CloneAndHandlePath((TvShow)match, true);
                        ((TvShow)newContent).UpdateMissing();
                        break;
                    case ContentType.Movie:
                        ((Movie)newContent).CloneAndHandlePath((Movie)match, true);
                        break;
                    default:
                        throw new Exception("unknown content type");
                }
                newContent.LastUpdated = DateTime.Now;
            }
            else if (matchSucess)
                newContent = match;
            else
                switch (this.ContentType)
                {
                    case ContentType.TvShow:
                        newContent = new TvShow(string.Empty, 0, 0, orgPath.Path, orgPath.RootFolder.FullPath);
                        break;
                    case ContentType.Movie:
                        newContent = new Movie(string.Empty, 0, 0, orgPath.Path, orgPath.RootFolder.FullPath);
                        break;
                    default:
                        throw new Exception("unknown content type");
                }

            // Set found flag
            newContent.Found = true;

            // Add content to list if new
            if (!contentExists)
                content.Add(newContent);

            // Update progress
            OnUpdateProgressChange(this, true, CalcProgress(numItemsProcessed, numItemsStarted, totalPaths), progressMsg);
        }

        /// <summary>
        /// Calculates update progress percentage
        /// </summary>
        /// <param name="numItemsProcessed">Number of items processed</param>
        /// <param name="numItemsStarted">Number of items started</param>
        /// <param name="totalItems">Total number of items to be processed</param>
        /// <returns>Progress percentage</returns>
        private int CalcProgress(int numItemsProcessed, int numItemsStarted, int totalItems)
        {
            return (int)Math.Round((double)(numItemsProcessed + numItemsStarted) / (totalItems * 2) * 100D);
        }

        /// <summary>
        /// Get content collection related to root folder content type.
        /// </summary>
        /// <returns>Content collection</returns>
        private ContentCollection GetContentCollection()
        {
            ContentCollection content;
            switch (this.ContentType)
            {
                case ContentType.TvShow:
                    content = Organization.Shows;
                    break;
                case ContentType.Movie:
                    content = Organization.Movies;
                    break;
                default:
                    throw new Exception("unknown content type");
            }
            return content;
        }

        #endregion

        #region XML

        /// <summary>
        /// Properties of this class that are saved/loaded in XML elements
        /// </summary>
        private enum XmlElements { SubPath, FullPath, AllowOrganizing, Default, ChildFolders, Temporary, AllSubFoldersChildRootFolder };

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
            // Start instace element
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
                    case XmlElements.Temporary:
                        value = this.Temporary.ToString();
                        break;
                    case XmlElements.AllSubFoldersChildRootFolder:
                        value = this.AllSubFoldersChildRootFolder.ToString();
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
        public bool Load(XmlNode foldersNode)
        {
            // Check that we're on the right node
            if (foldersNode.Name != ROOT_XML)
                return false;

            // Go through elements of node
            foreach (XmlNode propNode in foldersNode.ChildNodes)
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
                        this.ChildFolders = new ObservableCollection<ContentRootFolder>();
                        foreach (XmlNode subContentNode in propNode.ChildNodes)
                        {
                            ContentRootFolder subFolder = new ContentRootFolder(this.ContentType);
                            subFolder.Load(subContentNode);
                            this.ChildFolders.Add(subFolder);
                        }
                        break;
                    case XmlElements.Temporary:
                        bool temp;
                        if (bool.TryParse(value, out temp))
                            this.Temporary = temp;
                        break;
                    case XmlElements.AllSubFoldersChildRootFolder:
                        bool allSubsChild;
                        if (bool.TryParse(value, out allSubsChild))
                            this.AllSubFoldersChildRootFolder = allSubsChild;
                        break;
                }
            }

            return true;
        }

        #endregion
    }
}
