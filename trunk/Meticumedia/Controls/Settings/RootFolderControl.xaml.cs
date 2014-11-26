using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Collections.ObjectModel;
using Meticumedia.Classes;
using System.ComponentModel;
using Meticumedia.Windows;

namespace Meticumedia.Controls
{
    /// <summary>
    /// Interaction logic for RootFolderControl.xaml
    /// </summary>
    public partial class RootFolderControl : UserControl
    {
        public RootFolderControlViewModel ViewModel
        {
            get
            {
                return this.DataContext as RootFolderControlViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }
        
        public RootFolderControl()
        {
            InitializeComponent();
        }

        #region Variables

        private ContentRootFolderCollection folders;

        private ContentType contentType = ContentType.Movie;

        #endregion

        #region Event Handlers
        
        

        #endregion

        #region Save/Load

        public void LoadSettings(ContentType type)
        {
            this.folders = new ContentRootFolderCollection(type);
            this.contentType = type;
            foreach (ContentRootFolder flder in contentType == ContentType.Movie ? Settings.MovieFolders : Settings.TvFolders)
                folders.Add(new ContentRootFolder(flder));
        }

        public void SaveSettings()
        {
            switch(contentType)
            {
                case ContentType.Movie:
                    Settings.MovieFolders = folders;
                    break;
                case ContentType.TvShow:
                    Settings.TvFolders = folders;
                    break;
            }
            
        }

        #endregion
    }
}
