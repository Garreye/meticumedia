﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public AutoMoveFileSetup(string ext, string destination) : this()
        {
            this.FileTypes.Add(ext);
            this.DestinationPath = destination;
        }

        #endregion

        #region Methods

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
            item = null;

            if (!this.MoveFolder)
                return false;

            string[] fileList = Directory.GetFiles(folderPath);
            foreach (string file in fileList)
            {
                OrgItem fileItem;
                if (this.BuildFileMoveItem(file, scanDir, out fileItem))
                {
                    item = new OrgItem(folderPath, this, scanDir, true);


                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
