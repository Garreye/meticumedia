using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Meticumedia.Classes
{
    public class AutoMoveFileSetup : INotifyPropertyChanged
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

        #endregion

        #region Properties

        public ObservableCollection<string> FileTypes { get; set; }

        public string DestinationPath
        {
            get
            {
                return destinationPath;
            }
            set
            {
                destinationPath = value;
                OnPropertyChanged("DestinationPath");
            }
        }
        private string destinationPath;

        public bool MoveFolder
        {
            get
            {
                return moveFolder;
            }
            set
            {
                moveFolder = value;
                OnPropertyChanged("MoveFolder");
            }
        }
        private bool moveFolder;

        #endregion

        #region Construcotr

        public AutoMoveFileSetup()
        {
            this.FileTypes = new ObservableCollection<string>();
            this.MoveFolder = true;
        }

        public AutoMoveFileSetup(string ext, string destination)
            : this()
        {
            this.FileTypes.Add(ext);
            this.DestinationPath = destination;
        }

        public AutoMoveFileSetup(AutoMoveFileSetup clone)
            : this()
        {
            Clone(clone);
        }

        #endregion

        #region Methods

        public void Clone(AutoMoveFileSetup setup)
        {
            this.FileTypes.Clear();
            foreach (string type in setup.FileTypes)
                this.FileTypes.Add(type);
            this.DestinationPath = setup.DestinationPath;
            this.MoveFolder = setup.MoveFolder;
        }

        public bool BuildFileMoveItem(string filePath, OrgFolder scanDir, out OrgItem item)
        {
            item = null;
            foreach (string fileType in this.FileTypes)
                if (FileHelper.FileTypeMatch(fileType, filePath))
                {
                    item = new OrgItem(filePath, this, scanDir, false);

                    if (File.Exists(item.DestinationPath))
                    {
                        item.Action = OrgAction.AlreadyExists;
                        item.Enable = false;
                    }

                    return true;
                }
            return false;
        }

        public bool BuildFolderMoveItem(string folderPath, OrgFolder scanDir, out OrgItem item)
        {
            item = new OrgItem(folderPath, this, scanDir, true);

            if (!this.MoveFolder)
                return false;

            string[] fileList = Directory.GetFiles(folderPath);
            foreach (string file in fileList)
            {
                OrgItem fileItem;
                if (this.BuildFileMoveItem(file, scanDir, out fileItem))
                    return true;
            }
            string[] subDirs = Directory.GetDirectories(folderPath);
            foreach (string subDir in subDirs)
            {
                OrgItem subItem;
                if (this.BuildFolderMoveItem(subDir, scanDir, out subItem))
                    return true;
            }

            return false;
        }

        #endregion

        #region XML

        /// <summary>
        /// Root XML element string for this class.
        /// </summary>
        private static readonly string ROOT_XML = "AutoMoveSetup";

        /// <summary>
        /// XML element string for single file type
        /// </summary>
        private static readonly string FILE_TYPE_XML = "FileType";

        /// <summary>
        /// Element names for properties that need to be saved to XML.
        /// </summary>
        private enum XmlElements { FileTypes, DestinationPath, MoveFolder };

        /// <summary>
        /// Saves instance properties to XML file.
        /// </summary>
        /// <param name="xw">Writer for accessing XML file</param>
        public void Save(XmlWriter xw)
        {
            xw.WriteStartElement(ROOT_XML);
            
            // Write properties as sub-elements
            foreach (XmlElements element in Enum.GetValues(typeof(XmlElements)))
            {
                string value = null;
                switch (element)
                {
                    case XmlElements.FileTypes:
                        xw.WriteStartElement(element.ToString());
                        foreach (string type in this.FileTypes)
                            xw.WriteElementString(FILE_TYPE_XML, type);
                        xw.WriteEndElement();
                        break;
                    case XmlElements.DestinationPath:
                        value = this.DestinationPath;
                        break;
                    case XmlElements.MoveFolder:
                        value = this.MoveFolder.ToString();
                        break;
                    default:
                        throw new Exception("Unkonw element!");
                }

                if (value != null)
                    xw.WriteElementString(element.ToString(), value);
            }

            xw.WriteEndElement();
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
                    case XmlElements.FileTypes:
                        this.FileTypes.Clear();
                        foreach (XmlNode typeNode in propNode.ChildNodes)
                            this.FileTypes.Add(typeNode.InnerText);
                        break;
                    case XmlElements.DestinationPath:
                        this.DestinationPath = value;
                        break;
                    case XmlElements.MoveFolder:
                        bool move;
                        if (bool.TryParse(value, out move))
                            this.MoveFolder = move;
                        break;
                }
            }

            // Success
            return true;
        }

        #endregion
    }
}
