// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Class used for formatting file names
    /// </summary>
    public class FileNameFormat : INotifyPropertyChanged
    {
        #region Properties

        /// <summary>
        /// Definition of how file name should be formatted - specified as set of file name portions
        /// </summary>
        public ObservableCollection<FileNamePortion> Format
        {
            get
            {
                return format;
            }
            set
            {
                format = value;
                OnPropertyChanged("Format");
            }
        }

        private ObservableCollection<FileNamePortion> format = new ObservableCollection<FileNamePortion>();

        /// <summary>
        /// Format for episode string portion of file name (if any)
        /// </summary>
        public TvEpisodeFormat EpisodeFormat
        {
            get
            {
                return episodeFormat;
            }
            set
            {
                episodeFormat = value;
                OnPropertyChanged("EpisodeFormat");
            }
        }

        private TvEpisodeFormat episodeFormat = new TvEpisodeFormat();

        public ContentType ContentType {get;set;}

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

        #region Constructor

        /// <summary>
        /// Constructor that specifies whether it's for a movie or not
        /// </summary>
        /// <param name="movie">Whether file name formating will be for movie - for default format setup</param>
        public FileNameFormat(ContentType type)
        {
            // Default formats
            if (type == Classes.ContentType.Movie)
            {
                format.Add(new FileNamePortion(FileWordType.MovieName, string.Empty, " ", FileNamePortion.CaseOptionType.None));
                format.Add(new FileNamePortion(FileWordType.Year, "[", "]", FileNamePortion.CaseOptionType.None));
                format.Add(new FileNamePortion(FileWordType.VideoResolution, "[", "]", FileNamePortion.CaseOptionType.None));
                format.Add(new FileNamePortion(FileWordType.FilePart, "[", "]", FileNamePortion.CaseOptionType.None));
            }
            else
            {
                format.Add(new FileNamePortion(FileWordType.ShowName, string.Empty, " - ", FileNamePortion.CaseOptionType.None));
                format.Add(new FileNamePortion(FileWordType.EpisodeNumber, string.Empty, " - ", FileNamePortion.CaseOptionType.None));
                format.Add(new FileNamePortion(FileWordType.EpisodeName, string.Empty, string.Empty, FileNamePortion.CaseOptionType.None));
            }

            foreach (FileNamePortion fnp in format)
                fnp.PropertyChanged += new PropertyChangedEventHandler(fnp_PropertyChanged);
            this.format.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(format_CollectionChanged);
            this.episodeFormat.PropertyChanged += new PropertyChangedEventHandler(episodeFormat_PropertyChanged);
        }

        void episodeFormat_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("EpisodeFormat");
        }

        void format_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Format");

            if(e.NewItems != null)
                foreach (FileNamePortion fnp in e.NewItems)
                {
                    fnp.PropertyChanged += new PropertyChangedEventHandler(fnp_PropertyChanged);
                }
        }

        void fnp_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Format");
        }

        /// <summary>
        /// Constructor for copying instance
        /// </summary>
        /// <param name="format"></param>
        public FileNameFormat(FileNameFormat format) : this(format.ContentType)
        {
            Clone(format);
        }

        #endregion

        #region Methods

        public void Clone(FileNameFormat format)
        {
            this.Format.Clear();
            foreach (FileNamePortion portion in format.Format)
                this.Format.Add(new FileNamePortion(portion));
            this.EpisodeFormat = new TvEpisodeFormat(format.EpisodeFormat);
        }

        /// <summary>
        /// Builds formatted file name for movie from an existing file path.
        /// </summary>
        /// <param name="movie">The movie associated with file</param>
        /// <param name="fullPath">The path of current file to be renamed</param>
        /// <returns>Resulting formatted file name string</returns>
        public string BuildMovieFileName(Movie movie, string fullPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(fullPath);
            string folderPath = Path.GetDirectoryName(fullPath);
            string fileExt = Path.GetExtension(fullPath).ToLower();
            
            // Get info from file using simplify function (pulls out file words and categorizes them)
            FileHelper.OptionalSimplifyRemoves simplifyOptions = FileHelper.OptionalSimplifyRemoves.Year;
            FileHelper.SimplifyStringResults simpleResult = FileHelper.BuildSimplifyResults(fileName, false, false, simplifyOptions, true, false, true, false);

            // Get all video files from folder that don't match current path
            string differentiator = string.Empty;
            if (Directory.Exists(folderPath)) // Check in case path was empty
            {
                string[] dirFiles = Directory.GetFiles(folderPath);
                List<string> diffFiles = dirFiles.ToList().FindAll(f => Path.GetFileName(f) != Path.GetFileName(fileName) && Settings.VideoFileTypes.Contains(Path.GetExtension(f)));

                // Get differentiating part of string for other files with similar name
                foreach (string diffFile in diffFiles)
                {
                    // Get file name
                    string diffFileName = Path.GetFileNameWithoutExtension(diffFile);
                    
                    // Init differentiating string
                    string diff = string.Empty;

                    // Count number of characters in that match 2 file names and build string of different characters
                    int matchCnt = 0;
                    for (int i = 0; i < fileName.Length; i++)
                    {
                        if (i < diffFileName.Length && diffFileName[i].Equals(fileName[i]))
                            matchCnt++;
                        else
                            diff += fileName[i];
                    }

                    // If enough characters are common between the two strings it likely means there the same movie
                    if (matchCnt > 10 || matchCnt > (fileName.Length * 3) / 4)
                        // Save longest file name difference
                        if (diff.Length > differentiator.Length)
                            differentiator = diff;
                }

                // Simplify differentiator
                differentiator = Regex.Replace(differentiator, @"']+", " ");
                differentiator = Regex.Replace(differentiator, @"[!\?\u0028\u0029\:\]\[]+", " ");
                differentiator = Regex.Replace(differentiator, @"\W+|_", " ");
                differentiator = differentiator.Trim();
            }
            
            // Build file name
            string buildName = BuildFileName(movie.DisplayName, movie.DisplayYear, simpleResult, differentiator);
            
            // Remove unsafe file characters and add extension
            return FileHelper.GetSafeFileName(buildName) + fileExt;
        }

        /// <summary>
        /// Builds formatted file name for TV episode file from an existing file path.
        /// </summary>
        /// <param name="episode1">First episode in file</param>
        /// <param name="episode2">Second episode in file</param>
        /// <param name="fullPath">Path of file to be formatted</param>
        /// <returns>Path with file name formatted</returns>
        public string BuildTvFileName(TvEpisode episode1, TvEpisode episode2, string fullPath)
        {
            // Get file name
            string fileName = Path.GetFileNameWithoutExtension(fullPath);
            
            // Get info from file using simplify function (pulls out file words and categorizes them)
            FileHelper.OptionalSimplifyRemoves simplifyOptions = FileHelper.OptionalSimplifyRemoves.Year;
            FileHelper.SimplifyStringResults simplifyResults = FileHelper.BuildSimplifyResults(fileName, false, false, simplifyOptions, true, false, false, false);

            // Build file name
            string buildName = BuildFileName(episode1, episode2, simplifyResults);

            // Remove unsafe file characters and add extension
            return FileHelper.GetSafeFileName(buildName);
        }

        /// <summary>
        /// Build file name for movie.
        /// </summary>
        /// <param name="movieName">Name of movie</param>
        /// <param name="date">Date movie was released</param>
        /// <param name="simplifyResults">File name simplifying results</param>
        /// <param name="differentiator">String that differentiates file from other similar files in same directory</param>
        /// <returns>Resulting formatted file name string</returns>
        private string BuildFileName(string movieName, int year, FileHelper.SimplifyStringResults simplifyResults, string differentiator)
        {
            return BuildFileName(movieName, null, null, year, simplifyResults, differentiator);
        }

        /// <summary>
        /// Build file name for TV epiosode file.
        /// </summary>
        /// <param name="episode1">First episode in file</param>
        /// <param name="episode2">Second episode in file (if any)</param>
        /// <param name="simplifyResults">File name simplifying results</param>
        /// <returns>Resulting formatted file name string</returns>
        private string BuildFileName(TvEpisode episode1, TvEpisode episode2, FileHelper.SimplifyStringResults simplifyResults)
        {
            return BuildFileName(string.Empty, episode1, episode2, 1, simplifyResults, string.Empty);
        }

        /// <summary>
        /// Build file name for TV or movie file.
        /// </summary>
        /// <param name="movieName">Name of movie</param>
        /// <param name="episode1">First episode in file</param>
        /// <param name="episode2">Second episode in file</param>
        /// <param name="date">Date item was released</param>
        /// <param name="simplifyResults">File name simplifying results</param>
        /// <param name="differentiator"></param>
        /// <returns></returns>
        private string BuildFileName(string movieName, TvEpisode episode1, TvEpisode episode2, int year, FileHelper.SimplifyStringResults simplifyResults, string differentiator)
        {
            // Init file name
            string fileName = string.Empty;

            // Loop through each portion of format and add it to the file names
            foreach (FileNamePortion portion in this.Format)
            {
                // Get string value for current portion
                string value = portion.Header;
                switch (portion.Type)
                {
                    case FileWordType.MovieName:
                        value = movieName;
                        break;
                    case FileWordType.ShowName:
                        value = episode1.Show.DatabaseName;
                        break;
                    case FileWordType.EpisodeName:
                        value = BuildEpisodeName(episode1, episode2);
                        break;
                    case FileWordType.EpisodeNumber:
                        value = EpisodeFormat.BuildEpisodeString(episode1, episode2);
                        break;
                    case FileWordType.Year:
                        value = year.ToString();
                        break;
                    case FileWordType.FilePart:
                        if (!GetFileWord(simplifyResults, portion.Type, out value))
                            value = differentiator;
                        break;
                    default:
                        GetFileWord(simplifyResults, portion.Type, out value);
                        break;
                }

                // Add portion with header/footer and apply option
                if (!string.IsNullOrWhiteSpace(value))
                {
                    // Add header
                    fileName += portion.Header;

                    // Apply case options
                    switch (portion.CaseOption)
                    {
                        case FileNamePortion.CaseOptionType.None:
                            break;
                        case FileNamePortion.CaseOptionType.LowerCase:
                            value = value.ToLower();
                            break;
                        case FileNamePortion.CaseOptionType.UpperCase:
                            value = value.ToUpper();
                            break;
                    }

                    // Apply whitespace options
                    fileName += Regex.Replace(value, @"\s+", portion.Whitespace);

                    // Add footer
                    fileName += portion.Footer;
                }
            }

            // Return file name that was built
            return fileName;
        }

        /// <summary>
        /// Combines string value for strings for a file word type from simplify results
        /// </summary>
        /// <param name="simplifyResult">File name simplifying results</param>
        /// <param name="type">Type of file word we want string of</param>
        /// <param name="combinedWord">Resulting combination of all strings of specified type</param>
        /// <returns>whether any words of the specified type were found in results</returns>
        private static bool GetFileWord(FileHelper.SimplifyStringResults simplifyResult, FileWordType type, out string combinedWord)
        {
            // Initialize combined string
            combinedWord = string.Empty;

            // Check if any word of the desired type were remove during simplify
            if (simplifyResult.RemovedWords.ContainsKey(type))
            {
                // Get words of desired type
                List<string> words = simplifyResult.RemovedWords[type];

                // Go through each removed word and build single string with all of them together
                for (int i = 0; i < words.Count; i++)
                {
                    if (i != 0)
                        combinedWord += "_";
                    combinedWord += words[i];
                }

                // Words were found
                return true;
            }

            // No words of type found
            return false;
        }

        /// <summary>
        /// Build a combined episode name for 2 episodes. Find commanality between both names
        /// and produces a joined name. e.g. From "The Final (part 1)" & "The Fina1 (part 2) the
        /// method will return "The Final (Part 1 & 2)"
        /// </summary>
        /// <param name="episode1">First episode</param>
        /// <param name="episode2">Second epsiode</param>
        /// <returns>Joined episod names from both episodes</returns>
        public string BuildEpisodeName(TvEpisode episode1, TvEpisode episode2)
        {
            string epName;
            if (episode2 != null && !string.IsNullOrEmpty(episode2.Name) && episode2.DatabaseNumber > 0)
            {
                char[] ep1Chars = episode1.Name.ToCharArray();
                char[] ep2Chars = episode2.Name.ToCharArray();

                // Check if name have any similarities at start
                int matchLen = 0;
                for (int i = 0; i < Math.Min(ep1Chars.Length, ep2Chars.Length); i++)
                    if (ep1Chars[i] == ep2Chars[i])
                        matchLen++;
                    else
                        break;

                if (matchLen > 3)
                {
                    List<char> name1Build = ep1Chars.ToList();
                    List<char> name2Build = ep2Chars.ToList();

                    // Go through characters of episode names and remove identicals
                    bool closingBracketFound = false;
                    for (int i = Math.Min(ep1Chars.Length, ep2Chars.Length) - 1; i >= 0; i--)
                        if (ep1Chars[i] == ep2Chars[i])
                        {
                            if (ep1Chars[i] == ')' && !closingBracketFound)
                            {
                                closingBracketFound = true;
                                name1Build.RemoveAt(i);
                            }

                            name2Build.RemoveAt(i);
                        }

                    epName = new String(name1Build.ToArray()) + " & " + new String(name2Build.ToArray());

                    if (closingBracketFound)
                        epName += ")";
                }
                else
                    epName = episode1.Name + " & " + episode2.Name;
            }
            else
                epName = episode1.Name;

            return epName.Trim();
        }

        #endregion

        #region XML

        /// <summary>
        /// Element names for properties that need to be saved to XML.
        /// </summary>
        private enum XmlElements { Format, EpisodeFormat };

        /// <summary>
        /// Saves instance properties to XML file.
        /// </summary>
        /// <param name="xw">Writer for accessing XML file</param>
        public void Save(XmlWriter xw)
        {
            // Write properties as sub-elements
            foreach (XmlElements element in Enum.GetValues(typeof(XmlElements)))
            {
                string value = null;
                switch (element)
                {
                    case XmlElements.Format:
                        xw.WriteStartElement(element.ToString());
                        foreach (FileNamePortion portion in this.Format)
                            portion.Save(xw);
                        xw.WriteEndElement();
                        break;
                    case XmlElements.EpisodeFormat:
                        if (this.EpisodeFormat != null)
                        {
                            xw.WriteStartElement(element.ToString());
                            this.EpisodeFormat.Save(xw);
                            xw.WriteEndElement();
                        }
                        break;
                    default:
                        throw new Exception("Unkonw element!");
                }

                if (value != null)
                    xw.WriteElementString(element.ToString(), value);
            }
        }

        /// <summary>
        /// Loads instance properties from XML.
        /// </summary>
        /// <param name="itemNode">Node to load XML from</param>
        /// <returns>true if sucessfully loaded from XML</returns>
        public bool Load(XmlNode fileNameNode)
        {
            // Loop through sub-nodes
            foreach (XmlNode propNode in fileNameNode.ChildNodes)
            {
                // Get element/property type
                XmlElements element;;
                if(!Enum.TryParse<XmlElements>(propNode.Name, out element))
                    continue;

                // Get value string
                string value = propNode.InnerText;

                // Load value into appropriate property
                switch (element)
                {
                    case XmlElements.Format:
                        this.Format = new ObservableCollection<FileNamePortion>();
                        foreach(XmlNode formatNode in propNode.ChildNodes)
                        {
                            FileNamePortion portion = new FileNamePortion();
                            portion.Load(formatNode);
                            this.Format.Add(portion);
                        }
                        break;
                    case XmlElements.EpisodeFormat:
                        this.EpisodeFormat = new TvEpisodeFormat();
                        this.EpisodeFormat.Load(propNode);
                        break;
                }
            }

            // Success
            return true;
        }

        #endregion
    }
}
