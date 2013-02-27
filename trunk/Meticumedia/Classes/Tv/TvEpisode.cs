// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Meticumedia
{
    /// <summary>
    /// An episode of a TV show.
    /// </summary>
    public class TvEpisode : IComparable
    {
        #region Events

        /// <summary>
        /// Static event that fires when static background color properties are changed.
        /// </summary>
        public static event EventHandler BackColourChanged;

        /// <summary>
        /// Triggers BackColourChanged event
        /// </summary>
        protected static void OnBackColourChange()
        {
            if (BackColourChanged != null)
                BackColourChanged(null, new EventArgs());
        }

        #endregion
        
        #region Static Properties

        /// <summary>
        /// Color to be used for highlighting missing episodes
        /// </summary>
        public static Color MissingBackColor
        {
            get { return missingBackColor; }
            set
            {
                missingBackColor = value;
                OnBackColourChange();
            }
        }

        /// <summary>
        /// Color to be used for highlighting missing episodes
        /// </summary>
        public static Color InScanDirectoryColor
        {
            get { return inScanDirectoryColor; }
            set
            {
                inScanDirectoryColor = value;
                OnBackColourChange();
            }
        }

        /// <summary>
        /// Color to be used for highlighting ignored episodes
        /// </summary>
        public static Color IgnoredBackColor
        {
            get { return ignoredBackColor; }
            set
            {
                ignoredBackColor = value;
                OnBackColourChange();
            }
        }

        /// <summary>
        /// Color to be used for highlighting unaired episodes
        /// </summary>
        public static Color UnairedBackColor
        {
            get { return unairedBackColor; }
            set
            {
                unairedBackColor = value;
                OnBackColourChange();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The name of the episode
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The database name of the episode
        /// </summary>
        public string DataBaseName 
        {
            get { return databaseName; }
            set
            {
                databaseName = value;
                if (!this.NameIsUserSet && !string.IsNullOrEmpty(value))
                    this.Name = value;
            }
        }

        /// <summary>
        /// Flag indicating that Name property was overidden by the user
        /// and that it shouldn't be overriden by database!
        /// </summary>
        public bool NameIsUserSet { get; set; }

        /// <summary>
        /// Name of the show the episode belongs to
        /// </summary>
        public string Show { get; set; }

        /// <summary>
        /// Season number the episode belongs to
        /// </summary>
        public int Season { get; set; }

        /// <summary>
        /// The episode's number within the season
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Date when the episode first aired
        /// </summary>
        public DateTime AirDate { get; set; }

        /// <summary>
        /// Gets whether or not the episode has aired yet (invalid dates will return false)
        /// </summary>
        public bool Aired
        {
            get { return this.AirDate.Subtract(DateTime.Now).Days < 0 && this.AirDate.Year > 1900; }
        }

        /// <summary>
        /// Overview/description of the episode
        /// </summary>
        public string Overview { get; set; }
        
        /// <summary>
        /// Sets whether the episode is ignored from display and scans
        /// </summary>
        public bool Ignored { get; set; }

        /// <summary>
        /// Indicates if the episode is missing from the file directory
        /// </summary>
        public MissingStatus Missing { get; set; }

        /// <summary>
        /// Indicated whether the file has been watched or not (for future use)
        /// </summary>
        public bool Watched { get; set; }

        /// <summary>
        /// The file associated with the episode (if any)
        /// </summary>
        public TvFile File { get; set; }
      
        /// <summary>
        /// Indicates whether the episode is found in TheTvDb database.
        /// TODO: this is hacky!
        /// </summary>
        public bool InDatabase { get; set; }

        /// <summary>
        /// Whether episode was added to show by user (vs. automatically added from database)
        /// </summary>
        public bool UserDefined { get; set; }

        /// <summary>
        /// Prevent database from updating properties
        /// </summary>
        public bool PreventDatabaseUpdates { get; set; }

        #endregion

        #region Variables

        /// <summary>
        /// Missing status of episode in show folder or scan dir.
        /// </summary>
        public enum MissingStatus { Located, Missing, InScanDirectory }

        /// <summary>
        /// Private variable for storing DatabaseName property
        /// </summary>
        private string databaseName = string.Empty;

        /// <summary>
        /// Private variable for storing MissingBackColor property
        /// </summary>
        private static Color missingBackColor = Color.LightSkyBlue;

        /// <summary>
        /// Private variable for storing InScanDirectoryColor property
        /// </summary>
        private static Color inScanDirectoryColor = Color.LightGray;

        /// <summary>
        /// Variable for storing IgnoredBackColor property
        /// </summary>
        private static Color ignoredBackColor = Color.LightCoral;

        /// <summary>
        /// Variable for storing UnairedBackColor property
        /// </summary>
        private static Color unairedBackColor = Color.LightYellow;
        
        #endregion
        
        #region Constructor

        /// <summary>
        /// Constructor for episode with known properties.
        /// </summary>
        /// <param name="name">Name of the episode</param>
        /// <param name="show">Name of show the episode belongs to</param>
        /// <param name="season">Season number the episode belongs to</param>
        /// <param name="number">Number of the episode within the season</param>
        /// <param name="air">Air data string</param>
        /// <param name="overview">Episdoe overview/description</param>
        public TvEpisode(String name, string show, int season, int number, string air, string overview) : this(show)
        {
            this.NameIsUserSet = false;
            this.DataBaseName = name;
            this.Season = season;
            this.Number = number;
            DateTime airDate;
            DateTime.TryParse(air, out airDate);
            this.AirDate = airDate;
            this.Overview = overview;
        }

        /// <summary>
        /// Constructor for episode with know show name only.
        /// </summary>
        /// <param name="show">Name of show the episode belongs to</param>
        public TvEpisode(string show) : this()
        {
            this.Show = show;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TvEpisode()
        {
            this.NameIsUserSet = false;
            this.Name = string.Empty;
            this.DataBaseName = string.Empty;
            this.Show = string.Empty;
            this.Season = -1;
            this.Number = -1;
            this.AirDate = new DateTime();
            this.File = new TvFile();
            this.Ignored = false;
            this.InDatabase = false;
            this.Missing = MissingStatus.Missing;
            this.Overview = string.Empty;
            this.Watched = false;
            this.UserDefined = false;
            this.PreventDatabaseUpdates = false;
        }

        /// <summary>
        /// Constructor for cloning instance.
        /// </summary>
        /// <param name="episode">Instance to clone</param>
        public TvEpisode(TvEpisode episode)
        {
            this.UpdateInfo(episode);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates this instance with information from another instance
        /// </summary>
        /// <param name="episode"></param>
        public void UpdateInfo(TvEpisode episode)
        {
            this.NameIsUserSet = episode.NameIsUserSet || episode.Name != episode.DataBaseName;
            this.Name = episode.Name;
            this.DataBaseName = episode.DataBaseName;
            this.Show = episode.Show;
            this.Season = episode.Season;
            this.Number = episode.Number;
            this.AirDate = episode.AirDate;
            this.File = new TvFile(episode.File);
            this.Ignored = episode.Ignored;
            this.InDatabase = episode.InDatabase;
            this.Missing = episode.Missing;
            this.Overview = episode.Overview;
            this.Watched = episode.Watched;
            this.UserDefined = episode.UserDefined;
            this.PreventDatabaseUpdates = episode.PreventDatabaseUpdates;
        }

        /// <summary>
        /// Open file dialog so user can locate episode in file directory.
        /// </summary>
        /// <param name="showActionModifer">Whether to display action modifier after path is selected</param>
        /// <param name="copyAction">Whether to copy file, move otherwise</param>
        /// <param name="items">Organization items to add move/copy action to once located</param>
        /// <returns>true if locate was sucessful</returns>
        public bool UserLocate(bool showActionModifer, bool copyAction, out List<OrgItem> items)
        {
            // Initialize org items
            items = new List<OrgItem>();
            
            // Open file dialog
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "All Files|*.*";
            if (ofd.ShowDialog() != DialogResult.OK)
                return false;
            
            // Try to get episode information from file
            int fileEpSeason, fileEpNumber1, fileEpNumber2;
            bool fileInfoFound = FileHelper.GetEpisodeInfo(ofd.FileName, this.Show, out fileEpSeason, out fileEpNumber1, out fileEpNumber2);

            // Get this episode's show object
            TvShow show = GetShow();

            // Assign episodes
            TvEpisode ep1 = this;
            TvEpisode ep2 = null;

            // Locate could be for double episode file, only taken if one of the episode from file matches selected
            if (fileInfoFound && fileEpSeason == this.Season)
            {
                if (fileEpNumber1 == this.Number)
                {
                    if (fileEpNumber2 > 0)
                        show.FindEpisode(ep1.Season, fileEpNumber2, out ep2);
                    else
                        ep2 = null;
                }
                else if (fileEpNumber2 == this.Number)
                {
                    if (fileEpNumber1 > 0 && show.FindEpisode(this.Season, fileEpNumber1, out ep1))
                        ep2 = this;
                    else
                        ep1 = this;
                }
            }

            // Build org item
            OrgAction action = copyAction ? OrgAction.Move : OrgAction.Copy;
            string destination = show.BuildFilePath(ep1, ep2, Path.GetExtension(ofd.FileName));
            OrgItem item = new OrgItem(OrgStatus.Found, action, ofd.FileName, destination, ep1, ep2, FileHelper.FileCategory.TvVideo, null);

            // Display modifier
            if (showActionModifer)
            {
                OrgItemEditor oie = new OrgItemEditor(item);
                oie.ShowDialog();

                // Add results if valid
                if (oie.Result == null)
                    return false;

                items.Add(oie.Result);
                return true;
            }

            items.Add(item);
            return true;
        }

        /// <summary>
        /// Opens episode file for playback.
        /// </summary>
        public void PlayEpisodeFile()
        {
            // File is in TV show folder
            if (this.Missing == TvEpisode.MissingStatus.Located && !string.IsNullOrEmpty(this.File.FilePath) && System.IO.File.Exists(this.File.FilePath))
                Process.Start(this.File.FilePath); // TODO: if 2nd part of multi-part then notify!
            
            // File is in scan directory
            else if (this.Missing == MissingStatus.InScanDirectory)
            {
                // Search through scan directory items
                foreach (OrgItem item in ScanHelper.ScanDirTvItems)
                {
                    List<TvEpisode> eps = new List<TvEpisode>();
                    if (item.TvEpisode != null) eps.Add(item.TvEpisode);
                    if (item.TvEpisode2 != null) eps.Add(item.TvEpisode2);

                    foreach (TvEpisode ep in eps)
                    {
                        if (ep.Show == this.Show && ep.Season == this.Season && ep.Number == this.Number)
                            Process.Start(item.SourcePath);
                    }
                }
            }
        }

        /// <summary>
        /// Equals check for this episode and another episode
        /// </summary>
        /// <param name="obj">Episode to compare to</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            // False for null/invalid objects
            if (obj == null || !(obj is TvEpisode))
                return false;

            // Case object to episode
            TvEpisode ep = (TvEpisode)obj;

            // Compare is on season and episode number only (show name may not be set yet)
            return ep.Season == this.Season && ep.Number == this.Number;
        }

        /// <summary>
        /// Use base hash code. (Implements to prevent warning that Equals was overridden but not GetHasCode)
        /// </summary>
        /// <returns>base hash code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        
        /// <summary>
        /// Gets background color to use for episode based on whether
        /// it's ignored, aired or missing.
        /// </summary>
        /// <returns>Color to use for background</returns>
        public Color GetBackColor()
        {
            if (this.Ignored)
                return TvEpisode.IgnoredBackColor;
            else if (!this.Aired)
                return TvEpisode.UnairedBackColor;
            else if (this.Missing == MissingStatus.Missing)
                return TvEpisode.MissingBackColor;
            else if (this.Missing == MissingStatus.InScanDirectory)
                return TvEpisode.InScanDirectoryColor;
            else
                return Color.Transparent;
        }

        /// <summary>
        /// Get show episode belongs to
        /// </summary>
        /// <returns></returns>
        public TvShow GetShow()
        {
            for (int i = 0; i < Organization.Shows.Count; i++)
                if (Organization.Shows[i].Name == this.Show)
                    return (TvShow)Organization.Shows[i];

            return null;
        }

        #endregion

        #region IComparable Members

        /// <summary>
        /// Compare method for sorting epsiodes, uses airdate.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj is TvEpisode)
            {
                TvEpisode t2 = (TvEpisode)obj;
                return this.AirDate.CompareTo(t2.AirDate);
            }
            else
                throw new ArgumentException("Object is not a TvShow.");
        }

        #endregion

        #region XML

        /// <summary>
        /// Element names for properties that need to be saved to XML.
        /// </summary>
        private enum XmlElements { NameIsUserSet, Name, DataBaseName, Show, Season, Number, AirDate, Overview, Ignored, Missing, Watched, File, UserDefined, PreventDatabaseUpdates };

        /// <summary>
        /// Root XML element for saving instance to file.
        /// </summary>
        private static readonly string ROOT_XML = "TvEpisode";

        /// <summary>
        /// Saves instance to XML file.
        /// </summary>
        /// <param name="xw">Writer for accessing XML file</param>
        public void Save(XmlWriter xw)
        {
            // Start episode element
            xw.WriteStartElement(ROOT_XML);

            // Write properties as sub-elements
            foreach (XmlElements element in Enum.GetValues(typeof(XmlElements)))
            {
                string value = null;
                switch (element)
                {
                    case XmlElements.Name:
                        value = this.Name;
                        break;
                    case XmlElements.DataBaseName:
                        value = this.DataBaseName;
                        break;
                    case XmlElements.NameIsUserSet:
                        value = this.NameIsUserSet.ToString();
                        break;
                    case XmlElements.Show:
                        value = this.Show;
                        break;
                    case XmlElements.Season:
                        value = this.Season.ToString();
                        break;
                    case XmlElements.Number:
                        value = this.Number.ToString();
                        break;
                    case XmlElements.AirDate:
                        value = this.AirDate.ToString();
                        break;
                    case XmlElements.Overview:
                        value = this.Overview;
                        break;
                    case XmlElements.Ignored:
                        value = this.Ignored.ToString();
                        break;
                    case  XmlElements.Missing:
                        value = this.Missing.ToString();
                        break;
                    case XmlElements.Watched:
                        value = this.Watched.ToString();
                        break;
                    case XmlElements.File:
                        xw.WriteStartElement(element.ToString());
                        this.File.Save(xw);
                        xw.WriteEndElement();
                        break;
                    case XmlElements.UserDefined:
                        value = this.UserDefined.ToString();
                        break;
                    case XmlElements.PreventDatabaseUpdates:
                        value = this.PreventDatabaseUpdates.ToString();
                        break;
                    default:
                        throw new Exception("Unkonw element!");
                }

                if (value != null)
                    xw.WriteElementString(element.ToString(), value);
            }

            // End episode
            xw.WriteEndElement();
        }

        /// <summary>
        /// Loads instance from XML.
        /// </summary>
        /// <param name="itemNode">Node to load XML from</param>
        /// <returns>true if sucessfully loaded from XML</returns>
        public bool Load(XmlNode epNode)
        {
            // Checks that node is valid type
            if (epNode.Name != ROOT_XML)
                return false;

            // Loop through sub-nodes
            foreach (XmlNode propNode in epNode.ChildNodes)
            {
                // Get element/property type
                XmlElements element;
                if (!Enum.TryParse<XmlElements>(propNode.Name, out element))
                    continue;

                // Get value string
                string value = propNode.InnerText;

                // Load value into appropriate property
                switch (element)
                {
                    case XmlElements.Name:
                        this.Name = value;
                        break;
                    case XmlElements.DataBaseName:
                        this.DataBaseName = value;
                        break;
                    case XmlElements.NameIsUserSet:
                        bool nameUserSet;
                        bool.TryParse(value, out nameUserSet);
                        this.NameIsUserSet = nameUserSet;
                        break;
                    case XmlElements.Show:
                        this.Show = value;
                        break;
                    case XmlElements.Season:
                        int season;
                        int.TryParse(value, out season);
                        this.Season = season;
                        break;
                    case XmlElements.Number:
                        int number;
                        int.TryParse(value, out number);
                        this.Number = number;
                        break;
                    case XmlElements.AirDate:
                        DateTime airDate;
                        DateTime.TryParse(value, out airDate);
                        this.AirDate = airDate;
                        break;
                    case XmlElements.Overview:
                        this.Overview = value;
                        break;
                    case XmlElements.Ignored:
                        bool ignored;
                        bool.TryParse(value, out ignored);
                        this.Ignored = ignored;
                        break;
                    case XmlElements.Missing:
                        MissingStatus missing;
                        if (Enum.TryParse<MissingStatus>(value, out missing))
                            this.Missing = missing;
                        break;
                    case XmlElements.Watched:
                        bool watched;
                        bool.TryParse(value, out watched);
                        this.Watched = watched;
                        break;
                    case XmlElements.File:
                        this.File.Load(propNode);
                        break;
                    case XmlElements.UserDefined:
                        bool userDef;
                        bool.TryParse(value, out userDef);
                        this.UserDefined = userDef;
                        break;
                    case XmlElements.PreventDatabaseUpdates:
                        bool prevent;
                        bool.TryParse(value, out prevent);
                        this.PreventDatabaseUpdates = prevent;
                        break;
                }
            }

            // Sucess
            return true;
        }

        #endregion
    }
}
