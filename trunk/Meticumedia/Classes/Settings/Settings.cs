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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Static class for storing global settings
    /// </summary>
    public static class Settings
    {
        #region Events

        /// <summary>
        /// Setting has been modified.
        /// </summary>
        public static event EventHandler SettingsModified;

        /// <summary>
        /// Triggers SettingsModified event.
        /// </summary>
        private static void OnSettingsModified(bool initial)
        {
            if (SettingsModified != null)
            {
                SettingsModified(null, new EventArgs());
            }

            if (!initial)
            {
                Organization.UpdateRootFolders(ContentType.Movie);
                Organization.UpdateRootFolders(ContentType.TvShow);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// List of user specified folders to scan for new files.
        /// </summary>
        public static ObservableCollection<OrgFolder> ScanDirectories = new ObservableCollection<OrgFolder>();

        /// <summary>
        /// Format for renaming TV files
        /// </summary>
        public static FileNameFormat TvFileFormat = new FileNameFormat(ContentType.TvShow);

        /// <summary>
        /// Format for renaming movie files
        /// </summary>
        public static FileNameFormat MovieFileFormat = new FileNameFormat(ContentType.Movie);

        /// <summary>
        /// List of root folders containing movies
        /// </summary>
        public static ContentRootFolderCollection MovieFolders = new ContentRootFolderCollection(ContentType.Movie);

        /// <summary>
        /// List of root folders containing TV shows
        /// </summary>
        public static ContentRootFolderCollection TvFolders = new ContentRootFolderCollection(ContentType.TvShow);

        /// <summary>
        /// File types to match to video files
        /// </summary>
        public static ObservableCollection<string> VideoFileTypes = new ObservableCollection<string>();

        /// <summary>
        /// File types to delete during scan
        /// </summary>
        public static ObservableCollection<string> DeleteFileTypes = new ObservableCollection<string>();

        // <summary>
        /// File types to ignored during scan
        /// </summary>
        public static ObservableCollection<string> IgnoreFileTypes = new ObservableCollection<string>();

        /// <summary>
        /// Automatic file type move setups
        /// </summary>
        public static ObservableCollection<AutoMoveFileSetup> AutoMoveSetups = new ObservableCollection<AutoMoveFileSetup>();

        /// <summary>
        /// Persistent setting for options in UI.
        /// </summary>
        public static GuiSettings GuiControl = new GuiSettings();

        /// <summary>
        /// General settings for application
        /// </summary>
        public static GeneralSettings General = new GeneralSettings();

        #endregion

        #region Constants

        /// <summary>
        /// Default set of file extension for video files
        /// </summary>
        private static string[] DefaultVideoFileTypes = new string[]
        {
            "avi",
            "wmv",
            "mkv",
            "mpg",
            "mpeg",
            "mp4",
            "m4v",
            "rmvb",
            "divx",
            "iso",
            "dts",
            "ts"
        };

        /// <summary>
        /// Default set of file to be deleted from scan directory
        /// </summary>
        private static string[] DefaultDeleteFileTypes = new string[]
        {
            "log",
            "nzb",
            "db",
            "ini",
            "nfo",
            "sfv",
            "srr",
            "jpg",
            "bmp",
            "gif",
            "torrent",
            "rar",
            "zip",
            "url",
            "htm",
            "htm1",
            "srt",
            "png",
            "txt",
            "srs",
            "par2",
            @"r\d+"
        };


        /// <summary>
        /// Default set of file to be ignored from scan directory
        /// </summary>
        private static string[] DefaultIgnoreFileTypes = new string[]
        {
            "mp3",
            "flac",
            "m3u"
        };

        #endregion

        #region Methods

        /// <summary>
        /// Gets movie folder that is set as default
        /// </summary>
        /// <param name="rootFolder">The resulting default movie folder</param>
        /// <returns>whether default was found</returns>
        public static bool GetMovieFolderForContent(Content content, out ContentRootFolder rootFolder)
        {
            rootFolder = MovieFolders.GetFolderForContent(content);
            return rootFolder != null;
        }

        /// <summary>
        /// Gets TV folder that is set as default
        /// </summary>
        /// <returns></returns>
        public static bool GetTvFolderForContent(Content content, out ContentRootFolder rootFolder)
        {
            rootFolder = TvFolders.GetFolderForContent(content);
            return rootFolder != null;
        }

        /// <summary>
        /// Gets Tv folder that matches path
        /// </summary>
        /// <param name="path">The path of folder to match to</param>
        /// <returns>whether folder was found</returns>
        public static bool GetTvFolderFromPath(string path, out ContentRootFolder folder)
        {
            return TvFolders.GetFolderFromPath(path, out folder);
        }

        /// <summary>
        /// Gets movie folder that matches path
        /// </summary>
        /// <param name="path">The path of folder to match to</param>
        /// <returns>whether folder was found</returns>
        public static bool GetMovieFolderFromPath(string path, out ContentRootFolder folder)
        {
            return MovieFolders.GetFolderFromPath(path, out folder);
        }

        /// <summary>
        /// Get content folder object from path string.
        /// </summary>
        /// <param name="folderPath">Path of content folder</param>
        /// <param name="folder">Resulting content folder matching path</param>
        /// <returns>True if folder was found</returns>
        public static bool GetContentFolderFromPath(string folderPath, out ContentRootFolder folder)
        {
            if (GetMovieFolderFromPath(folderPath, out folder))
                return true;

            if (GetTvFolderFromPath(folderPath, out folder))
                return true;

            folder = new ContentRootFolder(ContentType.TvShow);
            return false;
        }

        /// <summary>
        /// Get all root folder for given content type
        /// </summary>
        /// <param name="type">Content type of root folders to get</param>
        /// <returns></returns>
        public static List<ContentRootFolder> GetAllRootFolders(ContentType type, bool recursive)
        {
            switch (type)
            {
                case ContentType.Movie:
                    return Settings.MovieFolders.GetFolders(recursive);
                case ContentType.TvShow:
                    return Settings.TvFolders.GetFolders(recursive);
                default:
                    throw new Exception("Unknown content type");
            }
        }

        #endregion

        #region XML

        /// <summary>
        /// Properties that are elements.
        /// </summary>
        private enum XmlElements 
        { 
            ScanDirectories, TvFileFormat, MovieFileFormat,  VideoFileTypes, DeleteFileTypes, IgnoreFileTypes, AutoMoveSetups, Gui, General, MovieFolderColletion, TvFolderCollection,
        
            // Deprecated
            MovieFolders, TvFolders,
        };

        /// <summary>
        /// Root element for setting XML.
        /// </summary>
        private static readonly string ROOT_XML = "Settings";

        /// <summary>
        /// Name for file type XML element.
        /// </summary>
        private static readonly string FILE_TYPE_XML = "FileType";

        /// <summary>
        /// Save settings to XML.
        /// </summary>
        public static void Save(bool triggerModifiedEvent = true)
        {
            string path = Path.Combine(Organization.GetBasePath(true), ROOT_XML + ".xml");

            // Save data into temporary file, so that if application crashes in middle of saving XML is not corrupted!
            string tempPath = Path.Combine(Organization.GetBasePath(true), ROOT_XML + "_TEMP.xml");

            using (XmlTextWriter xw = new XmlTextWriter(tempPath, Encoding.ASCII))
            {
                xw.Formatting = Formatting.Indented;

                // Start season element
                xw.WriteStartElement(ROOT_XML);

                foreach (XmlElements element in Enum.GetValues(typeof(XmlElements)))
                {
                    // Deprecated   
                    switch (element)
                    {
                        case XmlElements.MovieFolders:
                        case XmlElements.TvFolders:
                            continue;
                    }
                    
                    xw.WriteStartElement(element.ToString());
                    switch (element)
                    {
                        case XmlElements.ScanDirectories:
                            foreach (OrgFolder scanDir in ScanDirectories)
                                scanDir.Save(xw);
                            break;
                        case XmlElements.TvFileFormat:
                            TvFileFormat.Save(xw);
                            break;
                        case XmlElements.MovieFileFormat:
                            MovieFileFormat.Save(xw);
                            break;
                        case XmlElements.MovieFolderColletion:
                            MovieFolders.Save(xw);
                            break;
                        case XmlElements.TvFolderCollection:
                            TvFolders.Save(xw);
                            break;
                        case XmlElements.VideoFileTypes:
                            foreach (string fileType in VideoFileTypes)
                                xw.WriteElementString(FILE_TYPE_XML, fileType);
                            break;
                        case XmlElements.DeleteFileTypes:
                            foreach (string fileType in DeleteFileTypes)
                                xw.WriteElementString(FILE_TYPE_XML, fileType);
                            break;
                        case XmlElements.IgnoreFileTypes:
                            foreach (string fileType in IgnoreFileTypes)
                                xw.WriteElementString(FILE_TYPE_XML, fileType);
                            break;
                        case XmlElements.AutoMoveSetups:
                            foreach (AutoMoveFileSetup setup in AutoMoveSetups)
                                setup.Save(xw);
                            break;
                        case XmlElements.Gui:
                            GuiControl.Save(xw);
                            break;
                        case XmlElements.General:
                            General.Save(xw);
                            break;
                        default:
                            throw new Exception("Unkonw element!");
                    }
                    xw.WriteEndElement();
                }

                xw.WriteEndElement();
            }

            // Delete previous save data
            if (File.Exists(path))
                File.Delete(path);

            // Move tempoarary save file to default
            File.Move(tempPath, path);

            if (triggerModifiedEvent)
                OnSettingsModified(false);
        }

        /// <summary>
        /// Loads settings from XML.
        /// </summary>
        public static void Load()
        {
            string basePath = Organization.GetBasePath(false);
            if (!Directory.Exists(basePath))
                return;

            // Initialize file types to defautls
            VideoFileTypes = new ObservableCollection<string>();
            foreach (string type in DefaultVideoFileTypes)
                VideoFileTypes.Add(type);
            DeleteFileTypes = new ObservableCollection<string>();
            foreach (string type in DefaultDeleteFileTypes)
                DeleteFileTypes.Add(type);
            IgnoreFileTypes = new ObservableCollection<string>();
            foreach (string type in DefaultIgnoreFileTypes)
                IgnoreFileTypes.Add(type);

            // Load settings XML
            string path = Path.Combine(basePath, ROOT_XML + ".xml");
            if (File.Exists(path))
            {
                // Load XML
                XmlTextReader reader = new XmlTextReader(path);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(reader);

                foreach (XmlNode propNode in xmlDoc.DocumentElement.ChildNodes)
                {
                    XmlElements element;
                    if (!Enum.TryParse<XmlElements>(propNode.Name, out element))
                        continue;

                    string value = propNode.InnerText;
                    switch (element)
                    {
                        case XmlElements.ScanDirectories:
                            ScanDirectories = new ObservableCollection<OrgFolder>();
                            foreach (XmlNode scanDirNode in propNode.ChildNodes)
                            {
                                OrgFolder folder = new OrgFolder();
                                folder.Load(scanDirNode);
                                ScanDirectories.Add(folder);
                            }
                            break;
                        case XmlElements.TvFileFormat:
                            TvFileFormat.Load(propNode);
                            break;
                        case XmlElements.MovieFileFormat:
                            MovieFileFormat.Load(propNode);
                            break;
                        case XmlElements.MovieFolderColletion:
                            MovieFolders.Load(propNode);
                            break;
                        case XmlElements.TvFolderCollection:
                            TvFolders.Load(propNode);
                            break;
                        case XmlElements.VideoFileTypes:
                            VideoFileTypes = new ObservableCollection<string>();
                            foreach (XmlNode fileTypeNode in propNode.ChildNodes)
                            {
                                string videoType = fileTypeNode.InnerText;
                                if (videoType.StartsWith("."))
                                    videoType = videoType.Substring(1, videoType.Length - 1);
                                VideoFileTypes.Add(videoType);
                            }
                            break;
                        case XmlElements.DeleteFileTypes:
                            DeleteFileTypes = new ObservableCollection<string>();
                            foreach (XmlNode fileTypeNode in propNode.ChildNodes)
                            {
                                string delType = fileTypeNode.InnerText;
                                if (delType.StartsWith("."))
                                    delType = delType.Substring(1, delType.Length - 1);
                                DeleteFileTypes.Add(delType);
                            }
                            break;
                        case XmlElements.IgnoreFileTypes:
                            IgnoreFileTypes = new ObservableCollection<string>();
                            foreach (XmlNode fileTypeNode in propNode.ChildNodes)
                            {
                                string ignoreType = fileTypeNode.InnerText;
                                if (ignoreType.StartsWith("."))
                                    ignoreType = ignoreType.Substring(1, ignoreType.Length - 1);
                                IgnoreFileTypes.Add(ignoreType);
                            }
                            break;
                        case XmlElements.AutoMoveSetups:
                            AutoMoveSetups = new ObservableCollection<AutoMoveFileSetup>();
                            foreach (XmlNode setupNode in propNode.ChildNodes)
                            {
                                AutoMoveFileSetup setup = new AutoMoveFileSetup();
                                setup.Load(setupNode);
                                AutoMoveSetups.Add(setup);
                            }
                            break;
                        case XmlElements.Gui:
                            GuiControl.Load(propNode);
                            break;
                        case XmlElements.General:
                            General.Load(propNode);
                            break;
                        case XmlElements.MovieFolders:
                            MovieFolders.Clear();
                            foreach (XmlNode movieFolderNode in propNode.ChildNodes)
                            {
                                ContentRootFolder folder = new ContentRootFolder(ContentType.Movie);
                                folder.Load(movieFolderNode);
                                MovieFolders.Add(folder);
                            }
                            break;
                        case XmlElements.TvFolders:
                            TvFolders.Clear();
                            foreach (XmlNode tvFolderNode in propNode.ChildNodes)
                            {
                                ContentRootFolder folder = new ContentRootFolder(ContentType.TvShow);
                                folder.Load(tvFolderNode);
                                TvFolders.Add(folder);
                            }
                            break;
                    }
                }

            }
            OnSettingsModified(true);
        }

        #endregion
    }
}
