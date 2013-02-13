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

namespace Meticumedia
{
    /// <summary>
    /// Class used for formatting file names
    /// </summary>
    public class FileNameFormat
    {
        #region Properties

        /// <summary>
        /// Definition of how file name should be formatted - specified as set of file name portions
        /// </summary>
        public List<FileNamePortion> Format { get; set; }

        /// <summary>
        /// Format for episode string portion of file name
        /// </summary>
        public TvEpisodeFormat EpisodeFormat { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that specifies whether it's for a movie or not
        /// </summary>
        /// <param name="movie">Whether file name formating will be for movie - for default format setup</param>
        public FileNameFormat(bool movie)
        {
            // Init format
            this.Format = new List<FileNamePortion>();

            // Default format
            if (movie)
            {
                Format.Add(new FileNamePortion(FileWordType.MovieName, FileNamePortion.ContainerTypes.None));
                Format.Add(new FileNamePortion(FileWordType.Year, FileNamePortion.ContainerTypes.SquareBrackets));
                Format.Add(new FileNamePortion(FileWordType.VideoResolution, FileNamePortion.ContainerTypes.SquareBrackets));
                Format.Add(new FileNamePortion(FileWordType.FilePart, FileNamePortion.ContainerTypes.SquareBrackets));
            }
            else
            {
                Format.Add(new FileNamePortion(FileWordType.ShowName, FileNamePortion.ContainerTypes.None));
                Format.Add(new FileNamePortion(FileWordType.EpisodeNumber, FileNamePortion.ContainerTypes.Dashes));
                Format.Add(new FileNamePortion(FileWordType.EpisodeName, FileNamePortion.ContainerTypes.Dashes));
                
            }
            this.EpisodeFormat = new TvEpisodeFormat();
        }

        /// <summary>
        /// Constructor for copying instance
        /// </summary>
        /// <param name="format"></param>
        public FileNameFormat(FileNameFormat format)
        {
            this.Format = new List<FileNamePortion>();
            foreach (FileNamePortion portion in format.Format)
                this.Format.Add(new FileNamePortion(portion));
            this.EpisodeFormat = new TvEpisodeFormat(format.EpisodeFormat);
        }

        #endregion

        #region Methods

        /// <summary>
        ///  Build formatted file name for movie from an existing file path.
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
            FileHelper.SimplifyStringResults simpleResult = FileHelper.BuildSimplifyResults(fileName, false, false, simplifyOptions, true, false, true);

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
            string buildName = BuildFileName(movie.Name, movie.Date, simpleResult, differentiator);
            
            // Remove unsafe file characters and add extension
            return FileHelper.GetSafeFileName(buildName) + fileExt;
        }

        public string BuildTvFileName(TvEpisode episode1, TvEpisode episode2, string fullPath)
        {
            // Get file name
            string fileName = Path.GetFileNameWithoutExtension(fullPath);
            
            // Get info from file using simplify function (pulls out file words and categorizes them)
            FileHelper.OptionalSimplifyRemoves simplifyOptions = FileHelper.OptionalSimplifyRemoves.Year;
            FileHelper.SimplifyStringResults simplifyResults = FileHelper.BuildSimplifyResults(fileName, false, false, simplifyOptions, true, false, false);

            // Build file name
            string buildName = BuildFileName(episode1, episode2, simplifyResults);

            // Remove unsafe file characters and add extension
            return FileHelper.GetSafeFileName(buildName);
        }

        /// <summary>
        /// Build file name for movie
        /// </summary>
        /// <param name="movieName">Name of movie</param>
        /// <param name="date">Date movie was released</param>
        /// <param name="simpleResultify">File name simplifying results</param>
        /// <param name="differentiator">String that differentiates file from other similar files in same directory</param>
        /// <returns>Resulting formatted file name string</returns>
        private string BuildFileName(string movieName, DateTime date, FileHelper.SimplifyStringResults simpleResultify, string differentiator)
        {
            return BuildFileName(movieName, null, null, date, simpleResultify, differentiator);
        }

        private string BuildFileName(TvEpisode episode1, TvEpisode episode2, FileHelper.SimplifyStringResults simpleResultify)
        {
            return BuildFileName(string.Empty, episode1, episode2, new DateTime(), simpleResultify, string.Empty);
        }

        private string BuildFileName(string movieName, TvEpisode episode1, TvEpisode episode2, DateTime date, FileHelper.SimplifyStringResults simpleResultify, string differentiator)
        {
            // Init file name
            string fileName = string.Empty;

            // Loop through each portion of format and add it to the file names
            foreach (FileNamePortion portion in this.Format)
            {
                // Get string value for current portion
                string value;
                switch (portion.Type)
                {
                    case FileWordType.MovieName:
                        value = movieName;
                        break;
                    case FileWordType.ShowName:
                        value = episode1.Show;
                        break;
                    case FileWordType.EpisodeName:
                        value = BuildEpisodeName(episode1, episode2);
                        break;
                    case FileWordType.EpisodeNumber:
                        value = EpisodeFormat.BuildEpisodeString(episode1, episode2);
                        break;
                    case FileWordType.CustomString:
                        value = portion.Value;
                        break;
                    case FileWordType.Year:
                        if (!GetFileWord(simpleResultify, portion.Type, out value))
                            value = date.Year.ToString();
                        break;
                    case FileWordType.FilePart:
                        if (!GetFileWord(simpleResultify, portion.Type, out value))
                            value = differentiator;
                        break;
                    default:
                        GetFileWord(simpleResultify, portion.Type, out value);
                        break;
                }

                // Add opening containers
                if (!string.IsNullOrEmpty(value))
                {
                    switch (portion.Container)
                    {
                        case FileNamePortion.ContainerTypes.Whitespace:
                            if (!string.IsNullOrEmpty(fileName))
                                fileName += " ";
                            break;
                        case FileNamePortion.ContainerTypes.Underscores:
                            if (!string.IsNullOrEmpty(fileName))
                                fileName += "_";
                            break;
                        case FileNamePortion.ContainerTypes.Dashes:
                            if (!string.IsNullOrEmpty(fileName))
                                fileName += " - ";
                            break;
                        case FileNamePortion.ContainerTypes.Brackets:
                            fileName += " (";
                            break;
                        case FileNamePortion.ContainerTypes.SquareBrackets:
                            fileName += " [";
                            break;
                        case FileNamePortion.ContainerTypes.SquigglyBrackets:
                            fileName += "{";
                            break;
                        case FileNamePortion.ContainerTypes.Period:
                            fileName += ".";
                            break;
                        case FileNamePortion.ContainerTypes.Custom:
                            fileName += portion.Value;
                            break;
                    }

                    // Add portionvalue
                    fileName += value;

                    // Add closing container
                    switch (portion.Container)
                    {
                        case FileNamePortion.ContainerTypes.Whitespace:
                        case FileNamePortion.ContainerTypes.Underscores:
                        case FileNamePortion.ContainerTypes.Dashes:
                            break;
                        case FileNamePortion.ContainerTypes.Brackets:
                            if (!string.IsNullOrEmpty(fileName))
                                fileName += ")";
                            break;
                        case FileNamePortion.ContainerTypes.SquareBrackets:
                            if (!string.IsNullOrEmpty(fileName))
                                fileName += "]";
                            break;
                        case FileNamePortion.ContainerTypes.SquigglyBrackets:
                            if (!string.IsNullOrEmpty(fileName))
                                fileName += "}";
                            break;
                    }
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
            if (episode2 != null && !string.IsNullOrEmpty(episode2.Name))
            {
                char[] ep1Chars = episode1.Name.ToCharArray();
                char[] ep2Chars = episode2.Name.ToCharArray();

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
                        this.Format = new List<FileNamePortion>();
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
