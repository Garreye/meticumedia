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
    /// Television show class.
    /// </summary>
    public class TvShow : Content
    {
        #region Properties

        /// <summary>
        /// Episodes of show, indexed by season
        /// </summary>
        public TvSeasonCollection Seasons { get; set; }

        /// <summary>
        /// Whether to check for missing episodes during scan
        /// </summary>
        public bool DoMissingCheck { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with known properties
        /// </summary>
        /// <param name="name">Name of the show</param>
        /// <param name="id">TheTvDb ID for the show</param>
        /// <param name="directory">Directory where episode for the show are contained</param>
        public TvShow(string name, int id, int year, string directory, string contentFolder)
            : this()
        {
            this.Name = name;
            this.Id = id;
            this.Path = directory;
            this.RootFolder = contentFolder;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TvShow()
        {
            this.Seasons = new TvSeasonCollection();
            this.Name = string.Empty;
            this.Id = 0;
            this.Date = new DateTime();
            this.Path = string.Empty;
            this.RootFolder = string.Empty;
            ContentRootFolder defaultFolder;
            if (Settings.GetDefaultTvFolder(out defaultFolder))
                this.RootFolder = defaultFolder.FullPath;
            this.Overview = string.Empty;
            this.DoRenaming = true;
            this.IncludeInScan = true;
            this.DoMissingCheck = true;
        }

        /// <summary>
        /// Constructor for cloning instance.
        /// </summary>
        /// <param name="show"></param>
        public TvShow(TvShow show)
            : this()
        {
            Clone(show);
        }

        /// <summary>
        /// Constructor for creating instance from inherited class
        /// </summary>
        /// <param name="content"></param>
        public TvShow(Content content)
            : this()
        {
            base.Clone(content);
        }

        #endregion

        #region Methods

        /// <summary>
        /// String for instance is name if valid, else "UNKNOWN"
        /// </summary>
        /// <returns>string for instance</returns>
        public override string ToString()
        {
            return this.Name == string.Empty ? "UNKNOWN" : this.Name;
        }

        /// <summary>
        /// CLones another instance of class 
        /// </summary>
        /// <param name="show">Show to clon</param>
        public void Clone(TvShow show)
        {
            this.Name = show.Name;
            this.RootFolder = show.RootFolder;
            this.Overview = show.Overview;
            this.DoRenaming = show.DoRenaming;
            this.Path = show.Path;
            this.Found = show.Found;
            this.IncludeInScan = show.IncludeInScan;
            this.Seasons = show.Seasons;
            this.Id = show.Id;
            this.Date = show.Date;
            this.DoMissingCheck = show.DoMissingCheck;
            foreach (TvSeason season in this.Seasons)
                foreach (TvEpisode episode in season.Episodes)
                    episode.Show = show.Name;
        }

        /// <summary>
        /// Search show directory for episodes and mark the missing status for each
        /// </summary>
        public void UpdateMissing()
        {
            // Mark all as missing - TODO: check file path and check that file still exists, if so don't mark as missing!
            foreach (TvSeason season in Seasons)
                foreach (TvEpisode ep in season.Episodes)
                    ep.Missing = TvEpisode.MissingStatus.Missing;

            // Search through TV show directory
            if (Directory.Exists(this.Path))
                UpdateMissingRecursive(this.Path);

            // Search through scan directoru
            foreach (OrgItem item in ScanHelper.ScanDirTvItems)
            {
                List<TvEpisode> eps = new List<TvEpisode>();
                if (item.TvEpisode != null) eps.Add(item.TvEpisode);
                if (item.TvEpisode2 != null) eps.Add(item.TvEpisode2);

                foreach (TvEpisode ep in eps)
                {
                    if (ep.Show == this.Name)
                    {
                        TvEpisode matchEp;
                        if (this.FindEpisode(ep.Season, ep.Number, out matchEp))
                            if (matchEp.Missing == TvEpisode.MissingStatus.Missing)
                                matchEp.Missing = TvEpisode.MissingStatus.InScanDirectory;
                    }
                }
            }
        }

        /// <summary>
        /// Recursive search a directory and mark missing episodes.
        /// </summary>
        /// <param name="directory">Path of directory to check for episode files</param>
        private void UpdateMissingRecursive(string directory)
        {
            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
            {
                int season, episode1, episode2;
                if (FileHelper.GetEpisodeInfo(file, this.Name, out season, out episode1, out episode2))
                {
                    // Mark episodes as not missing
                    TvEpisode ep1 = null, ep2 = null;
                    bool matched1 = FindEpisode(season, episode1, out ep1);
                    bool matched2 = episode2 != -1 && FindEpisode(season, episode2, out ep2);

                    if (matched1) ep1.Missing = TvEpisode.MissingStatus.Located;
                    if (matched2) ep2.Missing = TvEpisode.MissingStatus.Located;

                    // Update file info
                    if (matched1 && matched2)
                    {
                        ep1.File = new TvFile(file, 1);
                        ep2.File = new TvFile(file, 2);
                    }
                    else if (matched1)
                        ep1.File = new TvFile(file);
                }
            }

            // Recursively check subdirectories
            string[] subDirs = Directory.GetDirectories(directory);
            foreach (string subDir in subDirs)
                UpdateMissingRecursive(subDir);
        }

        /// <summary>
        /// Builds a regular expression string for matching against
        /// file names. String built such that it will cause matches for:
        ///     -The full show name with spaces (e.g. "How I Met Your Mother)
        ///     -The full show name without spaces (e.g. "HowIMetYourMother)
        ///     -The acronym for the show name (e.g. "himym") - if name has 3 or more words
        ///     -The name without cosonents (e.g. "BttlstrGlctc")
        /// "and"/"&" are set to optional matches
        /// </summary>
        /// <returns></returns>
        private string BuildNameRegularExpresionString(bool removeWhitespace)
        {
            // Initialize string
            string showReStr = string.Empty;

            // Get simplified name
            string showname = FileHelper.SimplifyFileName(this.Name, true, removeWhitespace);

            // Split name words
            string[] showWords = showname.Split(' ');

            // Create cosonants RE
            Regex cosonantRe = new Regex("[aeiouy]");

            // Go through each word of the name
            for (int i = 0; i < showWords.Length; i++)
            {
                // Add 'a' for accronym
                if (showWords[i] == "and" || showWords[i] == "&")
                {
                    showReStr += @"(a|&(nd\W+)?)?";
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
        /// Attempts to find matches between a file name and the show name
        /// </summary>
        /// <param name="fileName">Collection of matches for file name and show</param>
        /// <returns></returns>
        public MatchCollection MatchFileToShow(string fileName)
        {
            string re = BuildNameRegularExpresionString(true);
            MatchCollection matches = null;
            if (!string.IsNullOrEmpty(re))
                matches = Regex.Matches(FileHelper.SimplifyFileName(System.IO.Path.GetFileNameWithoutExtension(fileName)), re, RegexOptions.IgnoreCase);

            if (matches != null && matches.Count > 0)
                return matches;

            re = BuildNameRegularExpresionString(false);
            if (!string.IsNullOrEmpty(re))
                return Regex.Matches(FileHelper.SimplifyFileName(System.IO.Path.GetFileNameWithoutExtension(fileName)), re, RegexOptions.IgnoreCase);

            return null;
        }

        /// <summary>
        /// Attempts to match a file to the show name
        /// </summary>
        /// <param name="fileName">Collection of matches for file name and show</param>
        /// <returns></returns>
        public bool CheckFileToShow(string fileName)
        {
            MatchCollection matches = MatchFileToShow(fileName);
            foreach (Match m in matches)
                if (m.Length >= 3)
                    return true;
            return false;

        }

        /// <summary>
        /// Gets missing episode for the show
        /// </summary>
        /// <returns>List of missing episodes</returns>
        public List<TvEpisode> GetMissingEpisodes(bool includeIgnored)
        {
            List<TvEpisode> list = new List<TvEpisode>();
            foreach (TvSeason season in Seasons)
                foreach (TvEpisode ep in season.Episodes)
                    if (ep.Missing == TvEpisode.MissingStatus.Missing && ep.Aired && (!ep.Ignored || includeIgnored))
                        list.Add(ep);
            return list;
        }

        /// <summary>
        /// Find an episode from the show.
        /// </summary>
        /// <param name="seasonNumber">The season of the episode</param>
        /// <param name="episodeNumber">The number of the epsiode within the season</param>
        /// <param name="episode">The matched episode if one is found, else null</param>
        /// <returns>true if the episode was found, false otherwise</returns>
        public bool FindEpisode(int seasonNumber, int episodeNumber, out TvEpisode episode)
        {
            episode = null;
            foreach (TvSeason season in Seasons)
                foreach (TvEpisode ep in season.Episodes)
                    if (ep.Season == seasonNumber && ep.Number == episodeNumber)
                    {
                        episode = ep;
                        return true;
                    }
            return false;
        }

        /// <summary>
        /// Build the file name string for a given episode
        /// </summary>
        /// <param name="episode1">Episode to build file name for</param>
        /// <param name="episode2">Second episode for double-episode files</param>
        /// <returns>Episode file name string</returns>
        public string BuildFileName(TvEpisode episode1, TvEpisode episode2, string path)
        {
            return Settings.TvFileFormat.BuildTvFileName(episode1, episode2, path);
        }

        /// <summary>
        /// Build full path of file for moving/copying TV episode file.
        /// </summary>
        /// <param name="episode1">Episode to build file path for</param>
        /// <param name="episode2">Second episode for double-episode files</param>
        /// <param name="extension">File extension for path</param>
        /// <returns>Episode file path</returns>
        public string BuildFilePath(TvEpisode episode1, TvEpisode episode2, string path)
        {
            // New season created to prevent exception if episode doesn't fall into a valid season
            return this.Path + "\\" + (new TvSeason(episode1.Season)).ToString() + "\\" + BuildFileName(episode1, episode2, path) + System.IO.Path.GetExtension(path).ToLower();
        }

        #endregion

        #region XML

        /// <summary>
        /// Root XML element for saving instance to file.
        /// </summary>
        private static readonly string ROOT_XML = "TvShow";

        /// <summary>
        /// Element names for properties that need to be saved to XML.
        /// </summary>
        private enum XmlElements { Seasons, DoMissing };

        /// <summary>
        /// Saves instance to XML file.
        /// </summary>
        /// <param name="xw">Writer for accessing XML file</param>
        public void Save(XmlWriter xw)
        {
            // Start show
            xw.WriteStartElement(ROOT_XML);

            // Write element from base
            this.WriteContentElements(xw);

            foreach (XmlElements element in Enum.GetValues(typeof(XmlElements)))
            {
                string value = null;
                switch (element)
                {
                    case XmlElements.Seasons:
                        xw.WriteStartElement(element.ToString());
                        foreach (TvSeason season in this.Seasons)
                            season.Save(xw);
                        xw.WriteEndElement();
                        break;

                    case XmlElements.DoMissing:
                        value = this.DoMissingCheck.ToString();
                        break;
                    default:
                        throw new Exception("Unkonw element!");
                }

                if (value != null)
                    xw.WriteElementString(element.ToString(), value);
            }

            // End show
            xw.WriteEndElement();
        }

        /// <summary>
        /// Loads instance from XML.
        /// </summary>
        /// <param name="itemNode">Node to load XML from</param>
        /// <returns>true if sucessfully loaded from XML</returns>
        public bool Load(XmlNode showNode)
        {
            // Check that node is proper type
            if (showNode.Name != ROOT_XML)
                return false;

            // Read base properties out
            base.ReadContentElements(showNode);

            // Read other elements
            foreach (XmlNode propNode in showNode.ChildNodes)
            {
                XmlElements element;
                if (!Enum.TryParse<XmlElements>(propNode.Name, out element))
                    continue;

                string value = propNode.InnerText;
                switch (element)
                {
                    case XmlElements.Seasons:
                        this.Seasons = new TvSeasonCollection();
                        foreach (XmlNode seasNode in propNode.ChildNodes)
                        {
                            TvSeason season = new TvSeason();
                            if (season.Load(seasNode))
                                this.Seasons.Add(season);
                        }
                        break;
                    case XmlElements.DoMissing:
                        bool doMissing;
                        bool.TryParse(value, out doMissing);
                        this.DoMissingCheck = doMissing;
                        break;
                }
            }



            // Sucess
            return true;
        }

        #endregion
    }
}
