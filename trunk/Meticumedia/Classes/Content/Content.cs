// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;

namespace Meticumedia
{
    /// <summary>
    /// Represents a folder that contains content for a single item (movie or TV show)
    /// </summary>
    public class Content : IComparable, ISerializable
    {        
        #region Properties

        /// <summary>
        /// Name of the content
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name of the content from online database (stored so user can change name without losing name in database)
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// Date content was first released
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Overview of content
        /// </summary>
        public string Overview { get; set; }
        
        /// <summary>
        /// Genre of content
        /// </summary>
        public List<string> Genres { get; set; }   

        /// <summary>
        /// Path to folder contain content files. Empty string if none.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Flag indicating content was found in directory
        /// </summary>
        public bool Found { get; set; }

        /// <summary>
        /// Path to root content folder this belongs to
        /// </summary>
        public string RootFolder { get; set; }

        /// <summary>
        /// ID for accessing content on the online database
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Indication of whether content has been watched
        /// </summary>
        public bool Watched { get; set; }

        /// <summary>
        /// Determines whether or not the show is to be included
        /// in scanning for episode that need to be renamed or are
        /// missing.
        /// </summary>
        public bool IncludeInScan { get; set; }

        /// <summary>
        /// Indicates whether files in folder are allowed to be renamed by application
        /// </summary>
        public bool DoRenaming { get; set; }

        /// <summary>
        /// Date/time when content was last updated from database
        /// </summary>
        public DateTime LastUpdated { get; set; }

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
            this.DoRenaming = true;
            this.Path = string.Empty;
            this.Found = true;
            this.Id = 0;
            this.IncludeInScan = true;
            this.Name = string.Empty;
            this.Overview = string.Empty;
            this.Watched = false;
            this.Genres = new List<string>();
            this.LastUpdated = new DateTime(1, 1, 1);
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

        protected void Clone(Content content)
        {
            this.Name = content.Name;
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
            this.IncludeInScan = content.IncludeInScan;
            this.DoRenaming = content.DoRenaming;
            this.LastUpdated = content.LastUpdated;
        }

        public virtual string BuildFolderPath()
        {
            return System.IO.Path.Combine(this.RootFolder, this.Name);
        }

        /// <summary>
        /// Builds list of genres into a single concatonated string.
        /// </summary>
        /// <returns></returns>
        public string GetGenresString()
        {
            // Add each genre name, followed by semicolon and space
            string genreStr = string.Empty;
            foreach (string genre in this.Genres)
                genreStr += genre + "; ";

            // Return string with last semicolon and space removed
            return genreStr.TrimEnd(';', ' ');
        }

        #endregion

        #region XML

        /// <summary>
        /// Element names for properties that need to be saved to XML
        /// </summary>
        private enum XmlElements { Name, DataBaseName, Date, Overview, Genres, Folder, RootFolder, Id, Watched, IncludeInScan, DoRenaming, LastUpdated };

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
                    case XmlElements.IncludeInScan:
                        value = this.IncludeInScan.ToString();
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
                        this.Overview = value;
                        break;
                    case XmlElements.Genres:
                        this.Genres = new List<string>();
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
                    case XmlElements.IncludeInScan:
                        bool includeInScan;
                        bool.TryParse(value, out includeInScan);
                        this.IncludeInScan = includeInScan;
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

        #endregion

        #region Serialization

        /// <summary>
        /// Serialized object - currently incomplete, only implemented to eliminate error in forms that
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
        /// <returns></returns>
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
