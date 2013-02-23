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
    /// Class defining a movie.
    /// </summary>
    public class Movie : Content
    {
        #region Constants

        /// <summary>
        /// String used for display of movie with unknown (empty) name.
        /// </summary>
        public static readonly string Unknown = "UNKNOWN";

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Movie() : base()
        {
        }

        /// <summary>
        /// Constructor with known name.
        /// </summary>
        /// <param name="name"></param>
        public Movie(string name) : this()
        {
            this.Name = name;
        }

        public Movie(string name, int id, int year, string directory, string contentFolder) : this(name)
        {
            this.Id = id;
            this.Date = new DateTime(year, 1, 1);
            this.Path = directory;
            this.RootFolder = contentFolder;
        }

        /// <summary>
        /// Constructor for cloning a Movie.
        /// </summary>
        /// <param name="movie"></param>
        public Movie(Movie movie) : this()
        {
            Clone(movie);
            this.RootFolder = movie.RootFolder;
            this.Path = movie.Path;
        }

        /// <summary>
        /// Constructor for creating instance from base class
        /// </summary>
        /// <param name="content"></param>
        public Movie(Content content) : this()
        {
            base.Clone(content);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates this movie with properties from another instance.
        /// </summary>
        /// <param name="movie"></param>
        public void Clone(Movie movie)
        {
            this.Name = movie.Name;
            this.DatabaseName = movie.DatabaseName;
            this.Overview = movie.Overview;
            this.Date = movie.Date;
            this.Found = movie.Found;
            this.Id = movie.Id;
            this.Genres = new GenreCollection(GenreCollection.CollectionType.Movie);
            if (movie.Genres != null)
                foreach (string genre in movie.Genres)
                    this.Genres.Add(genre);
        }

        /// <summary>
        /// Get string for movie.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name == string.Empty ? Unknown : this.Name;
        }

        /// <summary>
        /// Build nice file path for movie path folder
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public string BuildFilePath(string fullPath)
        {
            return System.IO.Path.Combine(BuildFolderPath(), Settings.MovieFileFormat.BuildMovieFileName(this, fullPath));
        }

        /// <summary>
        /// Build file path with no folder changes (only filename changes)
        /// </summary>
        /// <param name="fullPath">Path to start with</param>
        /// <returns></returns>
        public string BuildFilePathNoFolderChanges(string fullPath)
        {
            return System.IO.Path.Combine(this.Path, Settings.MovieFileFormat.BuildMovieFileName(this, fullPath));
        }

        /// <summary>
        /// Build path for movie folder
        /// </summary>
        /// <returns>Built path</returns>
        public override string BuildFolderPath()
        {
            if (string.IsNullOrEmpty(this.RootFolder))
            {
                ContentRootFolder defaultContent;
                if (Settings.GetDefaultMovieFolder(out defaultContent))
                    this.RootFolder = defaultContent.FullPath;
            }

            return System.IO.Path.Combine(this.RootFolder, FileHelper.GetSafeFileName(this.Name + " (" + this.Date.Year.ToString() + ")"));
        }

        #endregion

        #region XML

        /// <summary>
        /// Root XML element for saving instance to file.
        /// </summary>
        private static readonly string ROOT_XML = "Movie";

        /// <summary>
        /// Saves instance to XML file.
        /// </summary>
        /// <param name="xw">Writer for accessing XML file</param>
        public override void Save(XmlWriter xw)
        {
            // Start movie
            xw.WriteStartElement(ROOT_XML);

            // Write element from base
            this.WriteContentElements(xw);

            // End movie
            xw.WriteEndElement();
        }

        /// <summary>
        /// Loads instance from XML.
        /// </summary>
        /// <param name="itemNode">Node to load XML from</param>
        /// <returns>true if sucessfully loaded from XML</returns>
        public override bool Load(XmlNode movieNode)
        {
            // Check that node is current type
            if (movieNode.Name != ROOT_XML)
                return false;

            // Read base properties out
            base.ReadContentElements(movieNode);

            return true;
        }

        #endregion
    }
}
