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
using System.Collections.ObjectModel;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Television show class.
    /// </summary>
    public class TvShow : Content
    {
        #region Properties

        /// <summary>
        /// Database to use for show episodes
        /// </summary>
        public TvDataBaseSelection Database { get { return (TvDataBaseSelection)DatabaseSelection; } }

        public ObservableCollection<TvEpisode> Episodes
            {
                get { return this.episodes; }
            set
            {
                this.episodes = value;
                OnPropertyChanged("Episodes");
            }
        }

        private ObservableCollection<TvEpisode> episodes = new ObservableCollection<TvEpisode>();

        /// <summary>
        /// Whether to check for missing episodes during scan
        /// </summary>
        public bool DoMissingCheck
        {
            get { return this.doMissingCheck; }
            set
            {
                this.doMissingCheck = value;
                OnPropertyChanged("DoMissingCheck");
            }
        }

        private bool doMissingCheck = true;

        /// <summary>
        /// Whether to include show in scehdule.
        /// </summary>
        public bool IncludeInSchedule{
            get { return this.includeInSchedule; }
            set
            {
                this.includeInSchedule = value;
                OnPropertyChanged("IncludeInSchedule");
            }
        }

        private bool includeInSchedule = true;

        /// <summary>
        /// Determines whether or not the content is to be included in scanning.
        /// </summary>
        public override bool IncludeInScan { get { return this.DoRenaming || this.DoMissingCheck; } }

        /// <summary>
        /// List of alternative names to match to
        /// </summary>
        public ObservableCollection<string> AlternativeNameMatches
        {
            get { return this.alternativeNameMatches; }
            set
            {
                this.alternativeNameMatches = value;
                OnPropertyChanged("AlternativeNameMatches");
            }
        }

        private ObservableCollection<string> alternativeNameMatches = new ObservableCollection<string>();

        public List<int> Seasons
        {
            get
            {
                List<int> seasons = new List<int>();
                foreach (TvEpisode ep in this.Episodes)
                    if (!seasons.Contains(ep.Season))
                        seasons.Add(ep.Season);
                return seasons;
            }
        }

        /// <summary>
        /// Use DVD episode ordering
        /// </summary>
        public bool DvdEpisodeOrder
        {
            get { return this.dvdEpisodeOrder; }
            set
            {
                this.dvdEpisodeOrder = value;
                OnPropertyChanged("DvdEpisodeOrder");
            }
        }

        private bool dvdEpisodeOrder = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with known properties
        /// </summary>
        /// <param name="name">Name of the show</param>
        /// <param name="id">TheTvDb ID for the show</param>
        /// <param name="directory">Directory where episode for the show are contained</param>
        public TvShow(string name, int id, int year, string directory, string contentFolder)
            : this(name)
        {
            this.Id = id;
            this.Date = new DateTime(year > 0 ? year : 1, 1, 1);
            this.Path = directory;
            this.RootFolder = contentFolder;
        }

        /// <summary>
        /// Constructor with known name
        /// </summary>
        /// <param name="name">Name of the show</param>
        /// <param name="id">TheTvDb ID for the show</param>
        /// <param name="directory">Directory where episode for the show are contained</param>
        public TvShow(string name)
            : this()
        {
            this.Name = name;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TvShow()
            : base()
        {
            ContentRootFolder defaultFolder;
            if (Settings.GetDefaultTvFolder(out defaultFolder))
                this.RootFolder = defaultFolder.FullPath;

            this.episodes.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(episodes_CollectionChanged);
            this.alternativeNameMatches.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(alternativeNameMatches_CollectionChanged);
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

        #region Event Handlers

        private void alternativeNameMatches_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("AlternativeNameMatches");
        }

        private void episodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Episodes");
            OnPropertyChanged("Seasons");
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
        /// Clones another instance of class 
        /// </summary>
        /// <param name="show">Show to clone</param>
        public void Clone(TvShow show)
        {
            this.DatabaseSelection = show.DatabaseSelection;
            this.Name = show.Name;
            this.RootFolder = show.RootFolder;
            this.Overview = show.Overview;
            this.DoRenaming = show.DoRenaming;
            this.Path = show.Path;
            this.Found = show.Found;
            this.IncludeInSchedule = show.IncludeInSchedule;
            this.Genres = show.Genres;
            this.Id = show.Id;
            this.Date = show.Date;
            this.DoMissingCheck = show.DoMissingCheck;
            this.DvdEpisodeOrder = show.DvdEpisodeOrder;
            this.Episodes.Clear();
            foreach (TvEpisode episode in show.Episodes)
            {
                TvEpisode copyEp = new TvEpisode(episode);
                this.Episodes.Add(copyEp);
                copyEp.Show = this;
            }
            this.AlternativeNameMatches = new ObservableCollection<string>();
            foreach (string altName in show.AlternativeNameMatches)
                this.AlternativeNameMatches.Add(altName);
        }

        /// <summary>
        /// Search show directory for episodes and mark the missing status for each
        /// </summary>
        public override void UpdateMissing()
        {
            // Mark all as missing - TODO: check file path and check that file still exists, if so don't mark as missing!
            foreach (TvEpisode ep in this.Episodes)
                ep.Missing = MissingStatus.Missing;

            // Search through TV show directory
            if (Directory.Exists(this.Path))
                UpdateMissingRecursive(this.Path);

            // Search through scan directoru
            foreach (OrgItem item in TvItemInScanDirHelper.Items)
            {
                List<TvEpisode> eps = new List<TvEpisode>();
                if (item.TvEpisode != null) eps.Add(item.TvEpisode);
                if (item.TvEpisode2 != null) eps.Add(item.TvEpisode2);

                foreach (TvEpisode ep in eps)
                {
                    if (ep.Show.Name == this.Name)
                    {
                        TvEpisode matchEp;
                        if (this.FindEpisode(ep.Season, ep.Number, false, out matchEp))
                            if (matchEp.Missing == MissingStatus.Missing)
                                matchEp.Missing = MissingStatus.InScanDirectory;
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
                    bool matched1 = FindEpisode(season, episode1, false, out ep1);
                    bool matched2 = episode2 != -1 && FindEpisode(season, episode2, false, out ep2);

                    if (matched1) ep1.Missing = MissingStatus.Located;
                    if (matched2) ep2.Missing = MissingStatus.Located;

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
        /// Gets missing episode for the show
        /// </summary>
        /// <returns>List of missing episodes</returns>
        public List<TvEpisode> GetMissingEpisodes(bool includeIgnored)
        {
            List<TvEpisode> list = new List<TvEpisode>();
            foreach (TvEpisode ep in this.Episodes)
                if (ep.Missing == MissingStatus.Missing && ep.Aired && (!ep.Ignored || includeIgnored))
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
        public bool FindEpisode(int seasonNumber, int episodeNumber, bool databaseNum, out TvEpisode episode)
        {
            episode = null;
            foreach (TvEpisode ep in this.Episodes)
                if (ep.Season == seasonNumber && ((ep.Number == episodeNumber && !databaseNum) || (ep.DatabaseNumber == episodeNumber && databaseNum)))
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
            return this.Path + "\\" + episode1.SeasonName + "\\" + BuildFileName(episode1, episode2, path) + System.IO.Path.GetExtension(path).ToLower();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void UpdateInfoFromDatabase()
        {
            TvDatabaseHelper.FullShowSeasonsUpdate(this);
            this.LastUpdated = DateTime.Now;
        }

        #endregion

        #region XML

        /// <summary>
        /// Root XML element for saving instance to file.
        /// </summary>
        private static readonly string ROOT_XML = "TvShow";

        /// <summary>
        /// XML element name for single match
        /// </summary>
        private static readonly string MATCH_XML = "Match";

        /// <summary>
        /// Element names for properties that need to be saved to XML.
        /// </summary>
        private enum XmlElements { Seasons, Episodes, DoMissing, IncludeInSchedule, AlternativeNameMatches, DvdEpisodeOrder };

        /// <summary>
        /// Saves instance to XML file.
        /// </summary>
        /// <param name="xw">Writer for accessing XML file</param>
        public override void Save(XmlWriter xw)
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
                        // Deprecated
                        break;
                    case XmlElements.Episodes:
                        xw.WriteStartElement(element.ToString());
                        foreach (TvEpisode episode in this.Episodes)
                            episode.Save(xw);
                        xw.WriteEndElement();
                        break;
                    case XmlElements.DoMissing:
                        value = this.DoMissingCheck.ToString();
                        break;
                    case XmlElements.IncludeInSchedule:
                        value = this.IncludeInSchedule.ToString();
                        break;
                    case XmlElements.AlternativeNameMatches:
                        xw.WriteStartElement(element.ToString());
                        foreach (string match in AlternativeNameMatches)
                            xw.WriteElementString(MATCH_XML, match);
                        xw.WriteEndElement();
                        break;
                    case XmlElements.DvdEpisodeOrder:
                        value = this.DvdEpisodeOrder.ToString();
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
        public override bool Load(XmlNode showNode)
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
                    case XmlElements.Seasons: // Support for older versions
                        this.Episodes = new ObservableCollection<TvEpisode>();
                        foreach (XmlNode seasNode in propNode.ChildNodes)
                        {
                            foreach (XmlNode seasPropNode in seasNode.ChildNodes)
                            {
                                if(seasPropNode.Name == "Episodes")
                                {
                                    
                                    foreach(XmlNode epNode in seasPropNode.ChildNodes)
                                    {
                                        TvEpisode episode = new TvEpisode(this);
                                        episode.Load(epNode);
                                        Episodes.Add(episode);
                                    }
                                }
                            }
                        }
                        break;
                    case XmlElements.Episodes:
                        this.Episodes = new ObservableCollection<TvEpisode>();
                        foreach(XmlNode epNode in propNode.ChildNodes)
                        {
                            TvEpisode episode = new TvEpisode(this);
                            episode.Load(epNode);
                            Episodes.Add(episode);
                        }
                        break;
                    case XmlElements.DoMissing:
                        bool doMissing;
                        bool.TryParse(value, out doMissing);
                        this.DoMissingCheck = doMissing;
                        break;
                    case XmlElements.IncludeInSchedule:
                        bool include;
                        bool.TryParse(value, out include);
                        this.IncludeInSchedule = include;
                        break;
                    case XmlElements.AlternativeNameMatches:
                        this.AlternativeNameMatches = new ObservableCollection<string>();
                        foreach (XmlNode matchNode in propNode.ChildNodes)
                            if (!string.IsNullOrWhiteSpace(matchNode.InnerText))
                                this.AlternativeNameMatches.Add(matchNode.InnerText);
                        break;
                    case XmlElements.DvdEpisodeOrder:
                        bool dvdOrder;
                        bool.TryParse(value, out dvdOrder);
                        this.DvdEpisodeOrder = dvdOrder;
                        break;
                }
            }



            // Sucess
            return true;
        }

        #endregion
    }
}
