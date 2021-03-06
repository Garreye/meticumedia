﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Meticumedia.Classes
{
    public class GeneralSettings : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(object sender, string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(sender, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
        
        #region Properties

        public int NumProcessingThreads
        {
            get
            {
                return numProcessingThreads;
            }
            set
            {
                numProcessingThreads = value;
                OnPropertyChanged(this, "NumProcessingThreads");
            }
        }
        private int numProcessingThreads;

        public int NumSimultaneousSearches
        {
            get
            {
                return numSimultaneousSearches;
            }
            set
            {
                numSimultaneousSearches = value;
                OnPropertyChanged(this, "NumSimultaneousSearches");
            }
        }
        private int numSimultaneousSearches;

        public TvDataBaseSelection DefaultTvDatabase
        {
            get
            {
                return defaultTvDatabase;
            }
            set
            {
                defaultTvDatabase = value;
                OnPropertyChanged(this, "DefaultTvDatabase");
            }
        }
        private TvDataBaseSelection defaultTvDatabase;

        public MovieDatabaseSelection DefaultMovieDatabase
        {
            get
            {
                return defaultMovieDatabase;
            }
            set
            {
                defaultMovieDatabase = value;
                OnPropertyChanged(this, "DefaultMovieDatabase");
            }
        }
        private MovieDatabaseSelection defaultMovieDatabase;



        public string TorrentDirectory
        {
            get
            {
                return torrentDirectory;
            }
            set
            {
                torrentDirectory = value;
                OnPropertyChanged(this, "TorrentDirectory");
            }
        }
        private string torrentDirectory;

        public TorrentDownload TorrentDownload
        {
            get
            {
                return torrentDownload;
            }
            set
            {
                torrentDownload = value;
                OnPropertyChanged(this, "TorrentDownload");
            }
        }
        private TorrentDownload torrentDownload = TorrentDownload.Magnet;

        public TorrentQuality PreferredTorrentQuality
        {
            get
            {
                return preferredTorrentQuality;
            }
            set
            {
                preferredTorrentQuality = value;
                OnPropertyChanged(this, "PreferredTorrentQuality");
            }
        }
        private TorrentQuality preferredTorrentQuality = TorrentQuality.Sd480p;

        #endregion

        #region Constructors

        public GeneralSettings()
        {
            this.NumProcessingThreads = 10;
            this.NumSimultaneousSearches = 5;
            this.DefaultMovieDatabase = MovieDatabaseSelection.TheMovieDb;
            this.DefaultTvDatabase = TvDataBaseSelection.TheTvDb;
            this.TorrentDirectory = Path.Combine(Organization.GetBasePath(true), "Torrents");
        }

        public GeneralSettings(GeneralSettings settings)
        {
            Clone(settings);
        }

        #endregion

        #region Methods

        public void Clone(GeneralSettings settings)
        {
            this.NumProcessingThreads = settings.NumProcessingThreads;
            this.NumSimultaneousSearches = settings.NumSimultaneousSearches;
            this.DefaultMovieDatabase = settings.DefaultMovieDatabase;
            this.DefaultTvDatabase = settings.DefaultTvDatabase;
            this.TorrentDirectory = settings.TorrentDirectory;
            this.PreferredTorrentQuality = settings.PreferredTorrentQuality;
            this.TorrentDownload = settings.TorrentDownload;
        }

        #endregion

        #region XML

        /// <summary>
        /// Element names for properties that need to be saved to XML.
        /// </summary>
        private enum XmlElements { NumProcessingThreads, NumSimultaneousSearches, DefaultMovieDatabase, DefaultTvDatabase, TorrentDirectory, PreferredTorrentQuality, TorrentDownload };

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
                    case XmlElements.NumProcessingThreads:
                        value = this.NumProcessingThreads.ToString();
                        break;
                    case XmlElements.NumSimultaneousSearches:
                        value = this.NumSimultaneousSearches.ToString();
                        break;
                    case XmlElements.DefaultMovieDatabase:
                        value = this.DefaultMovieDatabase.ToString();
                        break;
                    case XmlElements.DefaultTvDatabase:
                        value = this.DefaultTvDatabase.ToString();
                        break;
                    case XmlElements.TorrentDirectory:
                        value = this.TorrentDirectory;
                        break;
                    case XmlElements.PreferredTorrentQuality:
                        value = this.PreferredTorrentQuality.ToString();
                        break;
                    case XmlElements.TorrentDownload:
                        value = this.TorrentDownload.ToString();
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
                XmlElements element; ;
                if (!Enum.TryParse<XmlElements>(propNode.Name, out element))
                    continue;

                // Get value string
                string value = propNode.InnerText;

                // Load value into appropriate property
                switch (element)
                {
                    case XmlElements.NumProcessingThreads:
                        int numProc;
                        if(int.TryParse(value, out numProc))
                            this.NumProcessingThreads = numProc;
                        break;
                    case XmlElements.NumSimultaneousSearches:
                        int numSearch;
                        if(int.TryParse(value, out numSearch))
                            this.NumSimultaneousSearches = numSearch;
                        break;
                    case XmlElements.DefaultMovieDatabase:
                        MovieDatabaseSelection movieDb;
                        if (Enum.TryParse<MovieDatabaseSelection>(value, out movieDb))
                            this.DefaultMovieDatabase = movieDb;
                        break;
                    case XmlElements.DefaultTvDatabase:
                        TvDataBaseSelection tvDb;
                        if (Enum.TryParse<TvDataBaseSelection>(value, out tvDb))
                            this.DefaultTvDatabase = tvDb;
                        break;
                    case XmlElements.TorrentDirectory:
                        this.TorrentDirectory = value;
                        break;
                    case XmlElements.PreferredTorrentQuality:
                        TorrentQuality torrentQual;
                        if (Enum.TryParse<TorrentQuality>(value, out torrentQual))
                            this.PreferredTorrentQuality = torrentQual;
                        break;
                    case XmlElements.TorrentDownload:
                        TorrentDownload torrentDwnld;
                        if (Enum.TryParse<TorrentDownload>(value, out torrentDwnld))
                            this.TorrentDownload = torrentDwnld;
                        break;
                }
            }

            // Success
            return true;
        }

        #endregion
    }
}
