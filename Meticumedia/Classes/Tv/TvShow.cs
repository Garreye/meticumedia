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
        public static readonly TvShow AllShows = new TvShow("All shows");
        
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
        public bool IncludeInSchedule
        {
            get { return this.includeInSchedule; }
            set
            {
                this.includeInSchedule = value;
                OnPropertyChanged("IncludeInSchedule");
            }
        }

        private bool includeInSchedule = true;

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

        public override List<int> Seasons
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
            this.DatabaseYear = year;
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
            this.DatabaseName = name;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TvShow()
            : base()
        {
            this.ContentType = Classes.ContentType.TvShow;
            ContentRootFolder defaultFolder;
            if (Settings.GetTvFolderForContent(null, out defaultFolder))
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
            base.CloneAndHandlePath(content, true);
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
            return this.DatabaseName == string.Empty ? "UNKNOWN" : this.DatabaseName;
        }

        public new void Clone(Content content)
        {
            CloneAndHandlePath(content, true, false);
        }

        /// <summary>
        /// Copies properties from another instance into this instance.
        /// </summary>
        /// <param name="content">Instance to copy properties from</param>
        /// <param name="replacePath">Whether path related properties should be cloned or not</param>
        /// <param name="handleEmptyPath">Whether to build path if one being cloned is empty</param>
        public override void CloneAndHandlePath(Content content, bool replacePath, bool handleEmptyPath = true)
        {
            if (!(content is TvShow))
                throw new Exception("Content must be TvShow");
            TvShow show = content as TvShow;

            base.CloneAndHandlePath(show, replacePath, handleEmptyPath);

            this.IncludeInSchedule = show.IncludeInSchedule;
            this.DoMissingCheck = show.DoMissingCheck;
            this.DvdEpisodeOrder = show.DvdEpisodeOrder;

            // TODO: this is a hack
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                this.Episodes.Clear();
                foreach (TvEpisode episode in show.Episodes)
                {
                    TvEpisode copyEp = new TvEpisode(episode);
                    this.Episodes.Add(copyEp);
                    copyEp.Show = this;
                }
            });
            this.AlternativeNameMatches = new ObservableCollection<string>();
            foreach (string altName in show.AlternativeNameMatches)
                this.AlternativeNameMatches.Add(altName);
        }

        /// <summary>
        /// Search show directory for episodes and mark the missing status for each
        /// </summary>
        public override void UpdateMissing()
        {
            // Mark all as missing
            foreach (TvEpisode ep in this.Episodes)
            {
                if (!File.Exists(ep.File.FilePath))
                    ep.Missing = MissingStatus.Missing;
            }

            // Search through TV show directory
            if (Directory.Exists(this.Path))
                UpdateMissingRecursive(this.Path);

            // Search through scan directory
            foreach (OrgItem item in TvItemInScanDirHelper.Items)
            {
                List<TvEpisode> eps = new List<TvEpisode>();
                if (item.TvEpisode != null) eps.Add(item.TvEpisode);
                if (item.TvEpisode2 != null) eps.Add(item.TvEpisode2);

                foreach (TvEpisode ep in eps)
                {
                    if (ep.Show.DatabaseName == this.DatabaseName)
                    {
                        TvEpisode matchEp;
                        if (this.FindEpisode(ep.Season, ep.DisplayNumber, false, out matchEp))
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
                if (FileHelper.GetEpisodeInfo(file, this.DatabaseName, out season, out episode1, out episode2))
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
                if (ep.Season == seasonNumber && ((ep.DisplayNumber == episodeNumber && !databaseNum) || (ep.DatabaseNumber == episodeNumber && databaseNum)))
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

        #region Equals

        public override bool Equals(object obj)
        {
            if (!(obj is TvShow))
                return false;

            if (!base.Equals(obj))
                return false;

            TvShow show = obj as TvShow;

            if (this.IncludeInSchedule != show.IncludeInSchedule ||
                this.DoMissingCheck != show.DoMissingCheck ||
                this.DvdEpisodeOrder != show.DvdEpisodeOrder||
                this.Episodes.Count != show.Episodes.Count ||
                this.AlternativeNameMatches.Count != show.AlternativeNameMatches.Count)
            {
                return false;
            }

            for(int i=0;i<this.Episodes.Count;i++)
                if(!this.Episodes[i].Equals(show.Episodes[i]))
                    return false;

            for(int i=0;i<this.AlternativeNameMatches.Count;i++)
                if(!this.AlternativeNameMatches[i].Equals(show.AlternativeNameMatches[i]))
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            //int hash = 13;
            //hash = (hash * 7) + base.GetHashCode();
            //hash = (hash * 7) + IncludeInSchedule.GetHashCode();
            //hash = (hash * 7) + DoMissingCheck.GetHashCode();
            //hash = (hash * 7) + DvdEpisodeOrder.GetHashCode();
            //hash = (hash * 7) + AlternativeNameMatches.GetHashCode();
            //return hash;
            return base.GetHashCode();
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
                        this.Episodes.Clear();
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
                        this.Episodes.Clear();
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
