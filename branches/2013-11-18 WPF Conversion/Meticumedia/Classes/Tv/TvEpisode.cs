// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Windows.Media;
using Ookii.Dialogs.Wpf;
using System.ComponentModel;
using Meticumedia.Windows;

namespace Meticumedia.Classes
{
    /// <summary>
    /// An episode of a TV show.
    /// </summary>
    public class TvEpisode : IComparable, INotifyPropertyChanged
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
        /// User-defined name for the episode
        /// </summary>
        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
                OnPropertyChanged("UserName");
                if (!this.UseDatabaseName)
                {
                    OnPropertyChanged("DisplayName");
                    OnPropertyChanged("StatusColor");
                }
            }
        }
        private string userName = string.Empty;

        /// <summary>
        /// Name of the episode from online database
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
                if (this.UseDatabaseName)
                {
                    OnPropertyChanged("DisplayName");
                    OnPropertyChanged("StatusColor");
                }
            }
        }

        private string databaseName = string.Empty;

        /// <summary>
        /// Defines if database name should be used for display
        /// </summary>
        public bool UseDatabaseName
        {
            get
            {
                return useDatabaseName;
            }
            set
            {
                useDatabaseName = value;
                if (!useDatabaseName && string.IsNullOrWhiteSpace(this.UserName))
                    this.UserName = this.DatabaseName;

                OnPropertyChanged("UseDatabaseName");
            }
        }
        private bool useDatabaseName = true;

        /// <summary>
        /// Display name of the episode
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (useDatabaseName)
                    return databaseName;
                else
                    return userName;
            }
            set
            {
                if (!useDatabaseName)
                    this.UserName = value;
            }
        }
        
        /// <summary>
        /// Name of the show the episode belongs to
        /// </summary>
        public TvShow Show
        {
            get
            {
                return show;
            }
            set
            {
                show = value;
                OnPropertyChanged("Show");
            }
        }
        private TvShow show = new TvShow();

        /// <summary>
        /// Season number the episode belongs to
        /// </summary>
        public int Season
        {
            get
            {
                return season;
            }
            set
            {
                season = value;
                OnPropertyChanged("Season");
                OnPropertyChanged("SeasonName");
            }
        }
        private int season = -1;

        /// <summary>
        /// Name of seardon the episode belongs to
        /// </summary>
        public string SeasonName
        {
            get
            {
                if (this.Season == 0)
                    return "Specials";
                else
                    return "Season " + this.Season;
            }
        }

        /// <summary>
        /// The episode's number within the season
        /// </summary>
        public int DisplayNumber 
        {
            get
            {
                if (this.UseDatabaseNumber)
                {
                    if (this.Show.DvdEpisodeOrder)
                        return this.DatabaseDvdNumber;
                    else
                        return this.DatabaseNumber;
                }
                else
                    return this.UserNumber;
            }
            set
            {
                if (!this.UseDatabaseNumber)
                    this.UserNumber = value;
            }
        }
        
        /// <summary>
        /// The database's number for the episode within the season 
        /// </summary>
        public int DatabaseNumber
        {
            get { return databaseNumber; }
            set
            {
                databaseNumber = value;
                OnPropertyChanged("DatabaseNumber");
                if (userNumber < 0)
                    OnPropertyChanged("Number");
            }
        }

        /// <summary>
        /// The database's DVD number for the episode within the season 
        /// </summary>
        public int DatabaseDvdNumber
        {
            get { return databaseDvdNumber; }
            set
            {
                databaseDvdNumber = value;
                OnPropertyChanged("DatabaseDvdNumber");
                if (this.Show.DvdEpisodeOrder && this.UseDatabaseNumber)
                    OnPropertyChanged("DisplayNumber");
            }
        }

        /// <summary>
        /// The database's user defined number for the episode within the season 
        /// </summary>
        public int UserNumber
        {
            get { return userNumber; }
            set
            {
                userNumber = value;
                OnPropertyChanged("UserNumber");
                if (!UseDatabaseNumber)
                    OnPropertyChanged("DisplayNumber");
            }
        }

        /// <summary>
        /// Defines if user-defined name should be used for display
        /// </summary>
        public bool UseDatabaseNumber
        {
            get
            {
                return useDatabaseNumber;
            }
            set
            {
                useDatabaseNumber = value;


                OnPropertyChanged("UseDatabaseNumber");
                OnPropertyChanged("DisplayNumber");
            }
        }
        private bool useDatabaseNumber = true;


        /// <summary>
        /// Date when the episode first aired from database
        /// </summary>
        public DateTime DatabaseAirDate
        {
            get
            {
                return databaseAirDate;
            }
            set
            {
                databaseAirDate = value;
                OnPropertyChanged("DatabaseAirDate");
                if (this.UseDatabaseAirDate)
                {
                    OnPropertyChanged("DisplayAirDate");
                    OnPropertyChanged("Aired");
                    OnPropertyChanged("StatusColor");
                }
            }
        }
        private DateTime databaseAirDate = new DateTime();

        /// <summary>
        /// Date when the episode first aired from database
        /// </summary>
        public DateTime UserAirDate
        {
            get
            {
                return userAirDate;
            }
            set
            {
                userAirDate = value;
                OnPropertyChanged("UserAirDate");
                if (this.UseDatabaseAirDate)
                {
                    OnPropertyChanged("DisplayAirDate");
                    OnPropertyChanged("Aired");
                    OnPropertyChanged("StatusColor");
                }
            }
        }
        private DateTime userAirDate = new DateTime();

        public DateTime DisplayAirDate
        {
            get
            {
                if (this.UseDatabaseAirDate)
                    return this.DatabaseAirDate;
                else
                    return this.UserAirDate;
            }
            set
            {

                if (!this.UseDatabaseAirDate)
                    this.UserAirDate = value;
            }
        }

        /// <summary>
        /// Defines if database defined air date should be used for display
        /// </summary>
        public bool UseDatabaseAirDate
        {
            get
            {
                return useDatabaseAirDate;
            }
            set
            {
                useDatabaseAirDate = value;

                if (!useDatabaseAirDate && this.UserAirDate.Equals(new DateTime()))
                    this.UserAirDate = this.DatabaseAirDate;

                OnPropertyChanged("UseDatabaseAirDate");
                OnPropertyChanged("DisplayAirDate");
                OnPropertyChanged("Aired");
                OnPropertyChanged("StatusColor");
            }
        }
        private bool useDatabaseAirDate = true;

        /// <summary>
        /// Gets whether or not the episode has aired yet (invalid dates will return false)
        /// </summary>
        public bool Aired
        {
            get { return this.DisplayAirDate.Subtract(DateTime.Now).Days < 0 && this.DisplayAirDate.Year > 1900; }
        }

        /// <summary>
        /// Overview/description of the episode from database
        /// </summary>
        public string DatabaseOverview
        {
            get
            {
                return databseOverview.Replace(Environment.NewLine, " ").Trim();
            }
            set
            {
                databseOverview = value;
                OnPropertyChanged("DatabaseOverview");
            }
        }
        private string databseOverview = string.Empty;

        /// <summary>
        /// Overview/description of the episode from user
        /// </summary>
        public string UserOverview
        {
            get
            {
                return userOverview.Replace(Environment.NewLine, " ").Trim();
            }
            set
            {
                userOverview = value;
                OnPropertyChanged("UserOverview");
            }
        }
        private string userOverview = string.Empty;

        /// <summary>
        /// Defines if database defined overview should be used for display
        /// </summary>
        public bool UseDatabaseOverview
        {
            get
            {
                return useDatabaseOverview;
            }
            set
            {
                useDatabaseOverview = value;
                if(!useDatabaseOverview && string.IsNullOrEmpty(this.UserOverview))
                    this.UserOverview = this.DatabaseOverview;

                OnPropertyChanged("UseDatabaseOverview");
                OnPropertyChanged("DisplayOverview");
            }
        }
        private bool useDatabaseOverview = true;

        public string DisplayOverview
        {
            get
            {
                if (this.UseDatabaseOverview)
                    return this.DatabaseOverview;
                else
                    return this.UserOverview;
            }
            set
            {

                if (!this.UseDatabaseOverview)
                    this.UserOverview = value;
            }
        }
        
        /// <summary>
        /// Sets whether the episode is ignored from display and scans
        /// </summary>
        public bool Ignored
        {
            get
            {
                return ignored;
            }
            set
            {
                ignored = value;
                OnPropertyChanged("Ignored");
                OnPropertyChanged("Status");
                OnPropertyChanged("StatusColor");
            }
        }

        private bool ignored = false;

        /// <summary>
        /// Indicates if the episode is missing from the file directory
        /// </summary>
        public MissingStatus Missing
        {
            get
            {
                return missing;
            }
            set
            {
                missing = value;
                OnPropertyChanged("Missing");
                OnPropertyChanged("Status");
                OnPropertyChanged("StatusColor");
            }
        }

        private MissingStatus missing = MissingStatus.Missing;

        /// <summary>
        /// Indicated whether the file has been watched or not (for future use)
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
        /// The file associated with the episode (if any)
        /// </summary>
        public TvFile File
        {
            get
            {
                return file;
            }
            set
            {
                file = value;
                OnPropertyChanged("File");
            }
        }

        private TvFile file = new TvFile();
      
        /// <summary>
        /// Indicates whether the episode is found in TheTvDb database.
        /// TODO: this is hacky!
        /// </summary>
        public bool InDatabase
        {
            get
            {
                return inDatabase;
            }
            set
            {
                inDatabase = value;
                OnPropertyChanged("InDatabase");
            }
        }

        private bool inDatabase = false;

        public bool UserDefined
        {
            get
            {
                return userNumber >= 0 && databaseNumber <= 0;
            }
        }


        /// <summary>
        /// Prevent database from updating properties
        /// </summary>
        public bool PreventDatabaseUpdates
        {
            get
            {
                return preventDatabaseUpdates;
            }
            set
            {
                preventDatabaseUpdates = value;
                OnPropertyChanged("PreventDatabaseUpdates");
            }
        }

        private bool preventDatabaseUpdates = false;

        /// <summary>
        /// Gets background color to use for episode based on whether
        /// it's ignored, aired or missing.
        /// </summary>
        /// <returns>Color to use for background</returns>
        public string StatusColor
        {
            get
            {
                if (this.Ignored)
                    return "LightSlateGray";
                else if (!this.Aired)
                    return "Gray";
                else if (this.Missing == MissingStatus.Missing)
                    return "LightCoral";
                else if (this.Missing == MissingStatus.InScanDirectory)
                    return "MediumSeaGreen";
                else if (this.Watched)
                    return "LightSlateGray";
                else
                    return "Black";
            }
        }

        public string AirDateString
        {
            get
            {
                if (this.DisplayAirDate.Year == 1)
                    return string.Empty;
                else
                    return " - " + this.DisplayAirDate.ToString("MMMM dd, yyyy");
            }
        }

        public string Status
        {
            get
            {                
                if (this.Ignored)
                    return "(Ignored)";
                else if (!this.Aired && this.DisplayAirDate.Year > 1)
                    return "(Upcoming)";
                else if (this.Missing == MissingStatus.Missing)
                    return "(Missing)";
                else if (this.Missing == MissingStatus.InScanDirectory)
                    return "(In Scan Directory)";
                else
                    return "";
            }
        }

        #endregion

        #region Variables

        /// <summary>
        /// Private variable for storing DatabaseNumber property
        /// </summary>
        private int databaseNumber = -1;

        /// <summary>
        /// Private variable for storing DatabaseDvdNumber property
        /// </summary>
        private int databaseDvdNumber = -1;

        /// <summary>
        /// Private variable for storing UserDefinedNumber property
        /// </summary>
        private int userNumber = -1;

        /// <summary>
        /// Private variable for storing MissingBackColor property
        /// </summary>
        private static Color missingBackColor = Colors.LightSkyBlue;

        /// <summary>
        /// Private variable for storing InScanDirectoryColor property
        /// </summary>
        private static Color inScanDirectoryColor = Colors.LightGray;

        /// <summary>
        /// Variable for storing IgnoredBackColor property
        /// </summary>
        private static Color ignoredBackColor = Colors.LightCoral;

        /// <summary>
        /// Variable for storing UnairedBackColor property
        /// </summary>
        private static Color unairedBackColor = Colors.LightGray;
        
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
        public TvEpisode(String name, TvShow show, int season, int number, string air, string overview) : this(show)
        {
            this.DatabaseName = name;
            this.Season = season;
            this.DatabaseNumber = number;
            DateTime airDate;
            DateTime.TryParse(air, out airDate);
            this.DatabaseAirDate = airDate;
            this.DatabaseOverview = overview;
        }

        /// <summary>
        /// Constructor for episode with know show name only.
        /// </summary>
        /// <param name="show">Name of show the episode belongs to</param>
        public TvEpisode(TvShow show)
        {
            this.Show = show;
        }

        /// <summary>
        /// Constructor for cloning instance.
        /// </summary>
        /// <param name="episode">Instance to clone</param>
        public TvEpisode(TvEpisode episode)
        {
            this.Clone(episode);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates this instance with information from another instance
        /// </summary>
        /// <param name="episode"></param>
        public void Clone(TvEpisode episode)
        {
            this.UseDatabaseName = episode.UseDatabaseName;
            this.UserName = episode.UserName;
            this.DatabaseName = episode.DatabaseName;
            this.Show = episode.Show;
            this.Season = episode.Season;
            this.UseDatabaseNumber = episode.UseDatabaseNumber;
            this.UserNumber = episode.UserNumber;
            this.DatabaseNumber = episode.DatabaseNumber;
            this.DatabaseDvdNumber = episode.DatabaseDvdNumber;
            this.UseDatabaseAirDate = episode.UseDatabaseAirDate;
            this.DatabaseAirDate = episode.DatabaseAirDate;
            this.UserAirDate = episode.UserAirDate;
            this.File = new TvFile(episode.File);
            this.Ignored = episode.Ignored;
            this.InDatabase = episode.InDatabase;
            this.Missing = episode.Missing;
            this.UseDatabaseOverview = episode.UseDatabaseOverview;
            this.DatabaseOverview = episode.DatabaseOverview;
            this.UserOverview = episode.UserOverview;
            this.Watched = episode.Watched;
            this.PreventDatabaseUpdates = episode.PreventDatabaseUpdates;
        }

        /// <summary>
        /// Build basic string for searching for this episode
        /// </summary>
        /// <returns></returns>
        public string BuildEpString()
        {
            string epInfo = this.Show + " s" + this.Season.ToString("00") + "e" + this.DisplayNumber.ToString("00");
            return FileHelper.SimplifyFileName(epInfo);
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
            VistaOpenFileDialog ofd = new VistaOpenFileDialog();
            ofd.Filter = "All Files|*.*";
            if (!(bool)ofd.ShowDialog())
                return false;
            
            // Try to get episode information from file
            int fileEpSeason, fileEpNumber1, fileEpNumber2;
            bool fileInfoFound = FileHelper.GetEpisodeInfo(ofd.FileName, this.Show.DatabaseName, out fileEpSeason, out fileEpNumber1, out fileEpNumber2);

            // Assign episodes
            TvEpisode ep1 = this;
            TvEpisode ep2 = null;

            // Locate could be for double episode file, only taken if one of the episode from file matches selected
            if (fileInfoFound && fileEpSeason == this.Season)
            {
                if (fileEpNumber1 == this.DisplayNumber)
                {
                    if (fileEpNumber2 > 0)
                        show.FindEpisode(ep1.Season, fileEpNumber2, false, out ep2);
                    else
                        ep2 = null;
                }
                else if (fileEpNumber2 == this.DisplayNumber)
                {
                    if (fileEpNumber1 > 0 && show.FindEpisode(this.Season, fileEpNumber1, false, out ep1))
                        ep2 = this;
                    else
                        ep1 = this;
                }
            }

            // Build org item
            OrgAction action = copyAction ? OrgAction.Copy : OrgAction.Move;
            string destination = show.BuildFilePath(ep1, ep2, Path.GetExtension(ofd.FileName));
            OrgItem item = new OrgItem(OrgStatus.Found, action, ofd.FileName, destination, ep1, ep2, FileCategory.TvVideo, null);

            // Display modifier
            if (showActionModifer)
            {
                OrgItemEditorWindow editor = new OrgItemEditorWindow(item);
                editor.ShowDialog();

                // Add results if valid
                if (editor.Results == null)
                    return false;

                items.Add(editor.Results);
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
            if (this.Missing == MissingStatus.Located && !string.IsNullOrEmpty(this.File.FilePath) && System.IO.File.Exists(this.File.FilePath))
                Process.Start(this.File.FilePath);
            
            // File is in scan directory
            else if (this.Missing == MissingStatus.InScanDirectory)
            {
                // Search through scan directory items
                foreach (OrgItem item in TvItemInScanDirHelper.Items)
                {
                    List<TvEpisode> eps = new List<TvEpisode>();
                    if (item.TvEpisode != null) eps.Add(item.TvEpisode);
                    if (item.TvEpisode2 != null) eps.Add(item.TvEpisode2);

                    foreach (TvEpisode ep in eps)
                    {
                        if (ep.Show == this.Show && ep.Season == this.Season && ep.DisplayNumber == this.DisplayNumber)
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
            return ep.Season == this.Season && ep.DisplayNumber == this.DisplayNumber && ep.DisplayName == this.DisplayName;
        }

        /// <summary>
        /// Use base hash code. (Implements to prevent warning that Equals was overridden but not GetHasCode)
        /// </summary>
        /// <returns>base hash code</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.Show.DisplayName + " season " + this.Season + " episode " + this.DisplayNumber;
        }
        
        #endregion

        #region Comparsions

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

                if (this.Season == t2.Season && this.DisplayNumber == t2.DisplayNumber)
                    return this.DisplayName.CompareTo(t2.DisplayName);
                else
                    return (this.Season * 1000 + this.DisplayNumber).CompareTo(t2.Season * 1000 + t2.DisplayNumber);
            }
            else
                throw new ArgumentException("Object is not a TvShow.");
        }

        /// <summary>
        /// Compares 2 content instances based on their Name property.
        /// </summary>
        /// <param name="x">The first movie.</param>
        /// <param name="y">The second movie.</param>
        /// <returns>The comparison results </returns>
        public static int CompareByAirDate(TvEpisode x, TvEpisode y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0;
                else
                    return -1;
            }
            else
            {
                if (y == null)
                    return 1;
                else
                    return x.DisplayAirDate.CompareTo(y.DisplayAirDate);
            }
        }

        #endregion

        #region XML

        /// <summary>
        /// Element names for properties that need to be saved to XML.
        /// </summary>
        private enum XmlElements
        {
            DatabaseName, UserDefinedName, UseDatabaseName, Season, DatabaseNumber, UserDefinedNumber, UseDatabaseNumber, DatabaseDvdNumber, Ignored, Missing, Watched, File, PreventDatabaseUpdates,
            DatabaseOverview, UserOverview, UseDatabaseOverview, DatabaseAirDate, UserAirDate, UseDatabaseAirDate,

            // Deprecated
             Name, NameIsUderDefined, AirDate, Overview
        }

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
                    case XmlElements.DatabaseName:
                        value = this.DatabaseName;
                        break;
                    case XmlElements.UserDefinedName:
                        value = this.UserName;
                        break;
                    case XmlElements.UseDatabaseName:
                        value = this.UseDatabaseName.ToString();
                        break;
                    case XmlElements.Season:
                        value = this.Season.ToString();
                        break;
                    case XmlElements.DatabaseNumber:
                        value = this.DatabaseNumber.ToString();
                        break;
                    case XmlElements.DatabaseDvdNumber:
                        value = this.DatabaseDvdNumber.ToString();
                        break;
                    case XmlElements.UserDefinedNumber:
                        value = this.UserNumber.ToString();
                        break;
                    case XmlElements.UseDatabaseNumber:
                        value = this.UseDatabaseNumber.ToString();
                        break;
                    case XmlElements.DatabaseOverview:
                        value = this.DatabaseOverview;
                        break;
                    case XmlElements.UserOverview:
                        value = this.UserOverview;
                        break;
                    case XmlElements.UseDatabaseOverview:
                        value = this.UseDatabaseOverview.ToString();
                        break;
                    case XmlElements.DatabaseAirDate:
                        value = this.DatabaseAirDate.ToString();
                        break;
                    case XmlElements.UserAirDate:
                        value = this.UserAirDate.ToString();
                        break;
                    case XmlElements.UseDatabaseAirDate:
                        value = this.UseDatabaseAirDate.ToString();
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
                    case XmlElements.PreventDatabaseUpdates:
                        value = this.PreventDatabaseUpdates.ToString();
                        break;

                    // Deprecated
                    case XmlElements.Name:
                    case XmlElements.NameIsUderDefined:
                    case XmlElements.AirDate:
                    case XmlElements.Overview:
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

            // Previous version support
            string name = string.Empty;
            bool nameIsUserDefined = false;

            // Loop through sub-nodes
            foreach (XmlNode propNode in epNode.ChildNodes)
            {
                // Get element/property type
                XmlElements element;
                if (!Enum.TryParse<XmlElements>(propNode.Name, true, out element))
                    continue;

                // Get value string
                string value = propNode.InnerText;

                // Load value into appropriate property
                switch (element)
                {
                    
                    case XmlElements.DatabaseName:
                        this.DatabaseName = value;
                        break;
                    case XmlElements.UserDefinedName:
                        this.UserName = value;
                        break;
                    case XmlElements.UseDatabaseName:
                        bool useDbName;
                        if (bool.TryParse(value, out useDbName))
                            this.UseDatabaseName = useDbName;
                        break;
                    case XmlElements.Season:
                        int season;
                        int.TryParse(value, out season);
                        this.Season = season;
                        break;
                    case XmlElements.DatabaseNumber:
                        int dbNumber;
                        int.TryParse(value, out dbNumber);
                        if (dbNumber < 0)
                            dbNumber = 0;
                        this.DatabaseNumber = dbNumber;
                        break;
                    case XmlElements.DatabaseDvdNumber:
                        int dbDvdNumber;
                        int.TryParse(value, out dbDvdNumber);
                        this.DatabaseDvdNumber = dbDvdNumber;
                        break;
                    case XmlElements.UserDefinedNumber:
                        int udNumber;
                        int.TryParse(value, out udNumber);
                        this.UserNumber = udNumber;
                        break;
                    case XmlElements.UseDatabaseNumber:
                        bool useDbNum;
                        if (bool.TryParse(value, out useDbNum))
                            this.UseDatabaseNumber = useDbNum;
                        break;
                    case XmlElements.DatabaseOverview:
                        this.DatabaseOverview = value;
                        break;
                    case XmlElements.UserOverview:
                        this.UserOverview = value;
                        break;
                    case XmlElements.UseDatabaseOverview:
                        bool useDbOverview;
                        if (bool.TryParse(value, out useDbOverview))
                            this.UseDatabaseOverview = useDbOverview;
                        break;
                    
                    case XmlElements.DatabaseAirDate:
                        DateTime dbAirDate;
                        DateTime.TryParse(value, out dbAirDate);
                        this.DatabaseAirDate = dbAirDate;
                        break;
                    case XmlElements.UserAirDate:
                        DateTime userAirDate;
                        DateTime.TryParse(value, out userAirDate);
                        this.UserAirDate = userAirDate;
                        break;
                    case XmlElements.UseDatabaseAirDate:
                        bool useDbAirDate;
                        bool.TryParse(value, out useDbAirDate);
                        this.UseDatabaseAirDate = useDbAirDate;
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
                    case XmlElements.PreventDatabaseUpdates:
                        bool prevent;
                        bool.TryParse(value, out prevent);
                        this.PreventDatabaseUpdates = prevent;
                        break;

                    // Deprecated
                    case XmlElements.Name:
                        name = value;
                        break;
                    case XmlElements.NameIsUderDefined:
                        bool.TryParse(value, out nameIsUserDefined);
                        break;
                    case XmlElements.AirDate:
                        DateTime airDate;
                        DateTime.TryParse(value, out airDate);
                        this.DatabaseAirDate = airDate;
                        break;
                    case XmlElements.Overview:
                        this.DatabaseOverview = value;
                        break;
                }
            }

            // Previous version support
            if (nameIsUserDefined && !string.IsNullOrEmpty(name))
                this.userName = name;

            // Sucess
            return true;
        }

        #endregion
    }
}
