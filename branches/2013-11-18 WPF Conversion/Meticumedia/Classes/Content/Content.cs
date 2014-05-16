﻿// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Represents a single content item (i.e. a movie or TV show) that is associated with a single directory on user's computer
    /// </summary>
    public class Content : IComparable, ISerializable, INotifyPropertyChanged
    {
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

        #region Constants

        public static readonly int UNKNOWN_ID = -1;

        public static readonly int USER_DEFINED_ID = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Database selection for content information
        /// </summary>
        public int DatabaseSelection
        {
            get 
            {
                return databaseSelection;
            }
            set
            {
                databaseSelection = value;
                OnPropertyChanged("DatabaseSelection");
            }
        }

        private int databaseSelection = (int)Settings.DefaultTvDatabase;

        /// <summary>
        /// Display name of the content
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    return "Unknown";
                else
                    return name;
            }
        }

        /// <summary>
        /// Name of the content
        /// </summary>
        public string Name
        {
            get 
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
                OnPropertyChanged("DisplayName");
                OnPropertyChanged("StatusColor");
            }
        }

        private string name = string.Empty;

        /// <summary>
        /// Name of the content from online database (stored so user can change name without losing name in database)
        /// </summary>
        public string DatabaseName
        {
            get 
            {
                return databaseName;
            }
            set
            {
                databaseName = value;
                OnPropertyChanged("DatabaseName");
            }
        }

        private string databaseName = string.Empty;

        /// <summary>
        /// Date content was first released
        /// </summary>
        public DateTime Date
        {
            get 
            {
                return date;
            }
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }

        private DateTime date = new DateTime();

        /// <summary>
        /// Overview/description of content
        /// </summary>
        public string Overview
        {
            get 
            {
                return overview;
            }
            set
            {
                overview = value;
                OnPropertyChanged("Overview");
            }
        }

        public string OverviewConcise
        {
            get
            {
                if (overview.Length < 110)
                    return overview;
                else
                    return overview.Substring(0, 107) + "...";
            }
        }

        private string overview = string.Empty;
        
        /// <summary>
        /// Genres of content
        /// </summary>
        public GenreCollection Genres
        {
            get 
            {
                return genres;
            }
            set
            {
                genres = value;
                OnPropertyChanged("Genres");
            }
        }

        private GenreCollection genres;

        /// <summary>
        /// Path to directory of content. Empty string if none.
        /// </summary>
        public string Path
        {
            get 
            {
                return path;
            }
            set
            {
                path = value;
                OnPropertyChanged("Path");
            }
        }

        private string path = string.Empty;

        /// <summary>
        /// Flag indicating content directory was found in root directory
        /// </summary>
        public bool Found
        {
            get 
            {
                return found;
            }
            set
            {
                found = value;
                OnPropertyChanged("Found");
            }
        }

        private bool found = true;

        /// <summary>
        /// Path to root content folder this belongs to
        /// </summary>
        public string RootFolder
        {
            get 
            {
                return rootFolder;
            }
            set
            {
                rootFolder = value;
                OnPropertyChanged("RootFolder");
            }
        }

        private string rootFolder = string.Empty;

        /// <summary>
        /// ID for accessing content on the online database
        /// </summary>
        public int Id
        {
            get 
            {
                return id;
            }
            set
            {
                id = value;
                OnPropertyChanged("Id");
                OnPropertyChanged("StatusColor");
            }
        }

        private int id = UNKNOWN_ID;

        /// <summary>
        /// Indication of whether content has been watched by user
        /// </summary>
        public bool Watched
        {
            get 
            {
                return watched;
            }
            set
            {
                watched = value;
                OnPropertyChanged("Watched");
            }
        }

        private bool watched = false;

        /// <summary>
        /// Determines whether or not the content's directory (and files within it) is to be included
        /// in scanning of root directory for organization
        /// </summary>
        public virtual bool IncludeInScan { get { return this.doRenaming; } }

        /// <summary>
        /// Indicates whether files in content's directory are allowed to be renamed by application
        /// </summary>
        public bool DoRenaming
        {
            get 
            {
                return doRenaming;
            }
            set
            {
                doRenaming = value;
                OnPropertyChanged("DoRenaming");
            }
        }

        private bool doRenaming = true;

        /// <summary>
        /// Date/time when content was last updated from database
        /// </summary>
        public DateTime LastUpdated
        {
            get 
            {
                return lastUpdate;
            }
            set
            {
                lastUpdate = value;
                OnPropertyChanged("LastUpdated");
            }
        }

        private DateTime lastUpdate = new DateTime();

        /// <summary>
        /// Gets background color to use for episode based on whether
        /// it's ignored, aired or missing.
        /// </summary>
        /// <returns>Color to use for background</returns>
        public string StatusColor
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    return "LightCoral";
                else if (this.id == UNKNOWN_ID)
                    return "LightCoral";
                else if (this.id == USER_DEFINED_ID)
                    return "MediumSeaGreen";
                else
                    return "Black";
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public Content()
        {
            this.RootFolder = string.Empty;
            this.DatabaseName = string.Empty;
            this.Date = new DateTime();
            this.Genres = new GenreCollection(this is Movie ? GenreCollection.CollectionType.Movie : GenreCollection.CollectionType.Tv);
            this.Genres.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Genres_CollectionChanged);
        }

        /// <summary>
        /// Constructor for cloning instance
        /// </summary>
        /// <param name="content">Instance to clone</param>
        public Content(Content content)
        {
            Clone(content);
        }

        #endregion

        #region Methods

        private void Genres_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Genres");
        }

        /// <summary>
        /// Copies properties from another instance into this instance.
        /// </summary>
        /// <param name="content">Instance to copy properties from</param>
        protected void Clone(Content content)
        {
            this.Name = content.Name;
            this.DatabaseSelection = content.DatabaseSelection;
            this.DatabaseName = content.DatabaseName;
            this.Date = content.Date;
            this.Overview = content.Overview;
            this.Genres = content.Genres;
            this.Found = content.Found;

            if (!string.IsNullOrEmpty(content.RootFolder))
                this.RootFolder = content.RootFolder;

            if (!string.IsNullOrEmpty(content.Path) && content.RootFolder != content.Path)
                this.Path = content.Path;
            else
                this.Path = this.BuildFolderPath();

            this.Id = content.Id;
            this.Watched = content.Watched;
            this.DoRenaming = content.DoRenaming;
            this.LastUpdated = content.LastUpdated;
        }

        /// <summary>
        /// Builds properly formatted path for content's directory. Assumes we want directory name to match content name.
        /// </summary>
        /// <returns>Resulting built path</returns>
        public virtual string BuildFolderPath()
        {
            return System.IO.Path.Combine(this.RootFolder, this.Name);
        }

        /// <summary>
        /// Builds list of genres into a single concatonated string.
        /// </summary>
        /// <returns>All genres in single string</returns>
        public string GetGenresString()
        {
            // Add each genre name, followed by semicolon and space
            string genreStr = string.Empty;
            foreach (string genre in this.Genres)
                genreStr += genre + "; ";

            // Return string with last semicolon and space removed
            return genreStr.TrimEnd(';', ' ');
        }

        /// <summary>
        /// Attempts to find matches between a file name and name of this instance
        /// </summary>
        /// <param name="fileName">File name to match to</param>
        /// <returns>Collection of matches for file name and content<</returns>
        public MatchCollection MatchFileToContent(string fileName)
        {
            List<string> names = new List<string>();
            names.Add(this.Name);

            // Load in alternate name for TV shows
            if (this is TvShow)
            {
                TvShow show = (TvShow)this;
                foreach (string altName in show.AlternativeNameMatches)
                    names.Add(altName);
            }

            // Try to match to each name
            foreach (string name in names)
            {
                // Build regular expression to match content to - try first without removing whitespace
                string re = BuildNameRegularExpresionString(true, name);
                MatchCollection matches = null;
                if (!string.IsNullOrEmpty(re))
                    matches = Regex.Matches(FileHelper.SimplifyFileName(System.IO.Path.GetFileNameWithoutExtension(fileName)), re, RegexOptions.IgnoreCase);

                // Return matches if found
                if (matches != null && matches.Count > 0)
                    return matches;

                // Build regular expression to match content to - this time with removed whitespace
                re = BuildNameRegularExpresionString(false, name);
                if (!string.IsNullOrEmpty(re))
                    matches = Regex.Matches(FileHelper.SimplifyFileName(System.IO.Path.GetFileNameWithoutExtension(fileName)), re, RegexOptions.IgnoreCase);

                // Return matches if found
                if (matches != null && matches.Count > 0)
                    return matches;
            }

            return null;
        }

        /// <summary>
        /// Builds a regular expression string for matching against
        /// file names. String built such that it will cause matches for:
        ///     -The full show name with spaces (e.g. "How I Met Your Mother)
        ///     -The full show name without spaces (e.g. "HowIMetYourMother)
        ///     -The acronym for the show name (e.g. "himym") - if name has 3 or more words
        ///     -The name without consonents (e.g. "BttlstrGlctc")
        /// "and"/"&" are set to optional for expression
        /// </summary>
        /// <returns></returns>
        private string BuildNameRegularExpresionString(bool removeWhitespace, string showname)
        {
            // Initialize string
            string showReStr = string.Empty;

            // Get simplified name
            showname = FileHelper.SimplifyFileName(showname, true, removeWhitespace, true);

            // Split name words
            string[] showWords = showname.Split(' ');

            // Create cosonants RE
            Regex cosonantRe = new Regex("[aeiouy]");

            // Go through each word of the name
            for (int i = 0; i < showWords.Length; i++)
            {
                // Add optional 'a' for accronym
                if (showWords[i] == "and" || showWords[i] == "&")
                {
                    showReStr += @"((a|&)(nd\W*)?)?";
                    continue;
                }

                // Go through each letter of the word
                for (int j = 0; j < showWords[i].Length; j++)
                {
                    // Add letter
                    if (showWords[i][j] == '-')
                        showReStr += ".";
                    else
                        showReStr += showWords[i][j];

                    // Start optional first letter only
                    if (j == 0 && showWords.Length > 2)
                        showReStr += "(";

                    // Consonants are optional
                    else if (cosonantRe.IsMatch(showWords[i][j].ToString()) && (showWords[i].Length > 5 || showWords.Length > 1))
                        showReStr += "?";

                    // End of word can contain whitespace/seperators
                    if (j == showWords[i].Length - 1)
                    {
                        // End optional first letter only
                        if (showWords.Length > 2)
                            showReStr += @")?";

                        showReStr += @"\W*";
                    }

                }
            }

            return showReStr;
        }

        /// <summary>
        /// Virtual update missing, to be overriden by inherited classes
        /// </summary>
        public virtual void UpdateMissing()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Virtual update of content from database, to be overriden by inherited classes
        /// </summary>
        public virtual void UpdateInfoFromDatabase()
        {
            throw new NotImplementedException();
        }        

        #endregion

        #region XML

        /// <summary>
        /// Element names for properties that need to be saved to XML
        /// </summary>
        private enum XmlElements { DatabaseSelection, Name, DataBaseName, Date, Overview, Genres, Folder, RootFolder, Id, Watched, DoRenaming, LastUpdated };

        /// <summary>
        /// XML element name for a single genre
        /// </summary>
        private static readonly string GENRE_XML = "Genre";

        /// <summary>
        /// Writes properties into XML elements
        /// </summary>
        /// <param name="xw">Writer for accessing XML file</param>
        public void WriteContentElements(XmlWriter xw)
        {
            // Loop through elements/properties
            foreach (XmlElements element in Enum.GetValues(typeof(XmlElements)))
            {
                // Set value from appropriate property
                string value = null;
                switch (element)
                {
                    case XmlElements.DatabaseSelection:
                        value = this.DatabaseSelection.ToString();
                        break;
                    case XmlElements.Name:
                        value = this.Name;
                        break;
                    case XmlElements.DataBaseName:
                        value = this.DatabaseName;
                        break;
                    case XmlElements.Date:
                        value = this.Date.ToString();
                        break;
                    case XmlElements.Overview:
                        value = this.Overview;
                        break;
                    case XmlElements.Genres:
                        xw.WriteStartElement(element.ToString());
                        foreach (string genre in this.Genres)
                            xw.WriteElementString(GENRE_XML, genre);
                        xw.WriteEndElement();
                        break;
                    case XmlElements.Folder:
                        value = this.Path;
                        break;
                    case XmlElements.RootFolder:
                        value = this.RootFolder;
                        break;
                    case XmlElements.Id:
                        value = this.Id.ToString();
                        break;
                    case XmlElements.Watched:
                        value = this.Watched.ToString();
                        break;
                    case XmlElements.DoRenaming:
                        value = this.DoRenaming.ToString();
                        break;
                    case XmlElements.LastUpdated:
                        value = this.LastUpdated.ToString();
                        break;
                    default:
                        throw new Exception("Unkonw element!");
                }
                // If value is valid then write it
                if (value != null)
                    xw.WriteElementString(element.ToString(), value);
            }
        }

        /// <summary>
        /// Loads properties from XML elements
        /// </summary>
        /// <param name="showNode">The XML node containt property elements</param>
        public void ReadContentElements(XmlNode showNode)
        {
            // Go through elements of node
            foreach (XmlNode propNode in showNode.ChildNodes)
            {
                // Match the current node to a known element name
                XmlElements element;
                if (!Enum.TryParse<XmlElements>(propNode.Name, out element))
                    continue;

                // Set appropiate property from value in element
                string value = propNode.InnerText;
                switch (element)
                {
                    case XmlElements.DatabaseSelection:
                        int dbSel;
                        int.TryParse(value, out dbSel);
                        this.DatabaseSelection = dbSel;
                        break;
                    case XmlElements.Name:
                        this.Name = value;
                        break;
                    case XmlElements.DataBaseName:
                        this.DatabaseName = value;
                        break;
                    case XmlElements.Date:
                        DateTime date;
                        DateTime.TryParse(value, out date);
                        this.Date = date;
                        break;
                    case XmlElements.Overview:
                        this.Overview = value.Trim();
                        break;
                    case XmlElements.Genres:
                        this.Genres = new GenreCollection(this is Movie ? GenreCollection.CollectionType.Movie : GenreCollection.CollectionType.Tv);
                        foreach (XmlNode genreNode in propNode.ChildNodes)
                            this.Genres.Add(genreNode.InnerText);
                        break;
                    case XmlElements.Folder:
                        this.Path = value;
                        break;
                    case XmlElements.RootFolder:
                        this.RootFolder = value;
                        break;
                    case XmlElements.Id:
                        int id;
                        int.TryParse(value, out id);
                        this.Id = id;
                        break;
                    case XmlElements.Watched:
                        bool watched;
                        bool.TryParse(value, out watched);
                        this.Watched = watched;
                        break;
                    case XmlElements.DoRenaming:
                        bool doRenaming;
                        bool.TryParse(value, out doRenaming);
                        this.DoRenaming = doRenaming;
                        break;
                    case XmlElements.LastUpdated:
                        DateTime lastUpdated;
                        DateTime.TryParse(value, out lastUpdated);
                        this.LastUpdated = lastUpdated;
                        break;
                }
            }
        }

        /// <summary>
        /// Virtual XML save, to be overriden by inherited class
        /// </summary>
        public virtual void Save(XmlWriter xw)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Virtual XML load, to be overriden by inherited class
        /// </summary>
        /// <param name="showNode"></param>
        /// <returns></returns>
        public virtual bool Load(XmlNode showNode)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Serialized object - TODO: currently incomplete, only implemented to eliminate error in forms that
        /// have instances of this class as objects
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Name", this.Name);
            info.AddValue("DatabaseName", this.DatabaseName);
            info.AddValue("Date", this.Date.ToString());
            info.AddValue("Overview", this.Overview);
            //info.AddValue("Genres", this.Genres);
            info.AddValue("Folder", this.Path);
            //info.AddValue("Files", this.Files);
            info.AddValue("Found", this.Found);
            info.AddValue("ContentFolder", this.RootFolder);
            info.AddValue("Id", this.Id);
            info.AddValue("Watched", this.Watched);
            info.AddValue("IncludeInScan", this.IncludeInScan);
            info.AddValue("DoRenaming", this.DoRenaming);
            info.AddValue("LastUpdated", this.LastUpdated.ToString());

        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compare this content instance to another instance. Compares by name.
        /// </summary>
        /// <param name="obj">Object to compare this instance to</param>
        /// <returns>Comparison results</returns>
        public int CompareTo(object obj)
        {
            if (obj is Content)
            {
                Content t2 = (Content)obj;

                if (string.IsNullOrEmpty(t2.Name) && !string.IsNullOrEmpty(this.Name))
                    return -1;
                else if (!string.IsNullOrEmpty(t2.Name) && string.IsNullOrEmpty(this.Name))
                    return 1;

                return this.Name.CompareTo(t2.Name);
            }
            else
                throw new ArgumentException("Object is not a content type.");
        }

        #endregion

        #region Comparisons

        /// <summary>
        /// Set whether to return acending or descending ordering
        /// results from CombareBy methods.
        /// </summary>
        public static bool AscendingSort { get; set; }

        /// <summary>
        /// Converts results for comparison to the order
        /// set by the AcsendingSort property. 
        /// </summary>
        /// <param name="ascendingResult">The comparison results for acending order</param>
        /// <returns>Comparison results for desired sort order</returns>
        private static int SetSort(int ascendingResult)
        {
            if (AscendingSort || ascendingResult == 0)
                return ascendingResult;
            else if (ascendingResult == -1)
                return 1;
            else
                return -1;
        }

        /// <summary>
        /// Compares 2 content instances based on their Name property.
        /// </summary>
        /// <param name="x">The first movie.</param>
        /// <param name="y">The second movie.</param>
        /// <returns>The comparison results </returns>
        public static int CompareByName(Content x, Content y)
        {
            int sortResult;
            if (x == null)
            {
                if (y == null)
                    sortResult = 0;
                else
                    sortResult = -1;
            }
            else
            {
                if (y == null)
                    sortResult = 1;
                else
                    sortResult = x.CompareTo(y);
            }

            return SetSort(sortResult);
        }

        /// <summary>
        /// Compares 2 movie instances based on their Path property.
        /// </summary>
        /// <param name="x">The first movie.</param>
        /// <param name="y">The second movie.</param>
        /// <returns>The comparison results </returns>
        public static int CompareByPath(Content x, Content y)
        {
            int sortResult;
            if (x == null)
            {
                if (y == null)
                    sortResult = 0;
                else
                    sortResult = -1;
            }
            else
            {
                if (y == null)
                    sortResult = 1;
                else
                    sortResult = x.Path.CompareTo(y.Path);
            }

            return SetSort(sortResult);
        }

        /// <summary>
        /// Compares 2 movie instances based on their Year property.
        /// </summary>
        /// <param name="x">The first movie.</param>
        /// <param name="y">The second movie.</param>
        /// <returns>The comparison results </returns>
        public static int CompareByYear(Content x, Content y)
        {
            int sortResult;
            if (x == null)
            {
                if (y == null)
                    sortResult = 0;
                else
                    sortResult = 1;
            }
            else
            {
                if (y == null)
                    sortResult = -1;
                else
                    sortResult = y.Date.CompareTo(x.Date);
            }

            return SetSort(sortResult);
        }

        /// <summary>
        /// Compares 2 movie instances based on their Genre property.
        /// </summary>
        /// <param name="x">The first movie.</param>
        /// <param name="y">The second movie.</param>
        /// <returns>The comparison results </returns>
        public static int CompareByGenre(Content x, Content y)
        {
            int sortResult;
            if (x == null)
            {
                if (y == null)
                    sortResult = 0;
                else
                    sortResult = -1;
            }
            else
            {
                if (y == null)
                    sortResult = 1;
                else
                {
                    string genreX = string.Empty, genreY = string.Empty;
                    if (x.Genres != null && x.Genres.Count > 0)
                        genreX = x.Genres[0].ToString();
                    if (y.Genres != null && y.Genres.Count > 0)
                        genreY = y.Genres[0].ToString();

                    sortResult = genreX.CompareTo(genreY);
                }
            }

            return SetSort(sortResult);
        }

        #endregion
    }
}
