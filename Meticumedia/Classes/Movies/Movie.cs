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
using System.Diagnostics;

namespace Meticumedia.Classes
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

        #region Properties

        /// <summary>
        /// Database to use for movie
        /// </summary>
        public MovieDatabaseSelection Database { get { return (MovieDatabaseSelection)DatabaseSelection; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Movie() : base()
        {
            this.ContentType = Classes.ContentType.Movie;
        }

        /// <summary>
        /// Constructor with known name.
        /// </summary>
        /// <param name="name"></param>
        public Movie(string name) : this()
        {
            this.DatabaseName = name;
        }

        public Movie(string name, int id, int year, string directory, string contentFolder) : this(name)
        {
            this.Id = id;
            this.DatabaseYear = year;
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
        }

        /// <summary>
        /// Constructor for creating instance from base class
        /// </summary>
        /// <param name="content"></param>
        public Movie(Content content) : this()
        {
            base.CloneAndHandlePath(content, true);
        }

        #endregion

        #region Methods

        public void PlayMovieFle()
        {
            PlayVideoFiles(this.Path);            
        }

        private void PlayVideoFiles(string path)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                if (FileHelper.CategorizeFile(new OrgPath(file, false, false, null, null), file) == FileCategory.MovieVideo)
                    Process.Start(file);
            }

            string[] subDirs = Directory.GetDirectories(path);
            foreach (string subDir in subDirs)
                PlayVideoFiles(subDir);
        }

        /// <summary>
        /// Get string for movie.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.DatabaseName == string.Empty ? Unknown : this.DatabaseName;
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
                ContentRootFolder rootFolder;
                if (Settings.GetMovieFolderForContent(this, out rootFolder))
                    this.RootFolder = rootFolder.FullPath;
            }

            return System.IO.Path.Combine(this.RootFolder, FileHelper.GetSafeFileName(this.DisplayName + " (" + this.DisplayYear.ToString() + ")"));
        }

        /// <summary>
        /// Updates properties from online database
        /// </summary>
        public override void UpdateInfoFromDatabase()
        {
            MovieDatabaseHelper.UpdateMovieInfo(this);
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
