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
    /// Represents a root directory that contains directories that are each linked to content (TV show or movies)
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
        /// content to during a scan
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// List of child root folders.
        /// </summary>
        public List<ContentRootFolder> ChildFolders { get; set; }

        /// <summary>
        /// Type of content contained in folder
        /// </summary>
        public ContentType ContentType { get; set; }

        #endregion

        #region Events

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

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ContentRootFolder(ContentType type)
        {
            this.ContentType = type;
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
        public ContentRootFolder(ContentType type, string path, string fullPath)
        {
            this.ContentType = type;
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
            this.ContentType = folder.ContentType;
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
            bool clearMoviesFound = contentType == Meticumedia.ContentType.Movie;
            bool clearShowsFound = contentType == Meticumedia.ContentType.TvShow;
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
                case Meticumedia.ContentType.Movie:
                    foreach (Content content in Organization.Movies)
                        foreach (string genre in content.Genres)
                            if (!genres.Contains(genre))
                                genres.Add(genre);
                    break;
                case Meticumedia.ContentType.TvShow:
                    foreach (Content content in Organization.Shows)
                        foreach (string genre in content.Genres)
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
                    if (content.Genres != null && !genreMatch)
                        foreach (string contentGenre in content.Genres)
                            if (genre.Contains(contentGenre))
                            {
                                genreMatch = true;
                                break;
                            }

                    // Apply year filter
                    bool yearMatch = !yearFilter || (content.Date.Year >= minYear && content.Date.Year <= maxYear);

                    // Apply text filter
                    bool nameMatch = string.IsNullOrEmpty(nameFilter) || content.Name.ToLower().Contains(nameFilter.ToLower());

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
            if (content.Genres != null && !genreMatch)
                foreach (string contentGenre in content.Genres)
                    if (genre.Contains(contentGenre))
                    {
                        genreMatch = true;
                        break;
                    }

            // Apply year filter
            bool yearMatch = !yearFilter || (content.Date.Year >= minYear && content.Date.Year <= maxYear);

            // Apply text filter
            bool nameMatch = string.IsNullOrEmpty(nameFilter) || content.Name.ToLower().Contains(nameFilter.ToLower());

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
            firstPass.Run(BuildSubDirectories(this.ContentType), ref cancel, true);
            updateCancelled = cancel;

            // Initialize processing - Second pass
            OrgProcessing secondPass = new OrgProcessing(UpdateProcess);
            updateNumber = secondPass.ProcessNumber;

            // Run 2nd pass (build of sub-dirs is recursive, so all child root folder sub-dirs will be included)
            secondPass.Run(BuildSubDirectories(this.ContentType), ref cancel, false); 
            updateCancelled = cancel;

            // Get content collection to add content to
            ContentCollection content = GetContentCollection();

            // Remove shows that no longer exists
            if (!cancel)
                content.RemoveMissing(this);

            // Save changes
            //content.Sort();
            content.Save();

            // Set progress to completed
            progressMsg = "Update of '" + this.FullPath + "' complete!";
            OnUpdateProgressChange(this, false, 0, progressMsg);

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
                if (orgPath.Path == content[j].Path)
                {
                    contentExists = true;
                    content[j].Found = true;
                    if (!string.IsNullOrEmpty(content[j].Name))
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
                // Check if show needs updating
                if ((DateTime.Now - newContent.LastUpdated).TotalDays > 7)
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
                    matchSucess = SearchHelper.TvShowSearch.PathMatch(orgPath.RootFolder.FullPath, orgPath.Path, firstPass, out showMatch);
                    match = showMatch;
                    break;
                case Meticumedia.ContentType.Movie:
                    Movie movieMatch;
                    matchSucess = SearchHelper.MovieSearch.PathMatch(orgPath.RootFolder.FullPath, orgPath.Path, firstPass, out movieMatch);
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
                        ((TvShow)newContent).Clone((TvShow)match);
                        ((TvShow)newContent).UpdateMissing();
                        break;
                    case Meticumedia.ContentType.Movie:
                        ((Movie)newContent).Clone((Movie)match);
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
                    case Meticumedia.ContentType.Movie:
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
                case Meticumedia.ContentType.Movie:
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
                            ContentRootFolder subFolder = new ContentRootFolder(this.ContentType);
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
