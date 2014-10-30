using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticumedia.Classes;
using Meticumedia.WPF;

namespace Meticumedia.Controls
{
    public class FileTypeSettingsControlViewModel : ViewModel
    {
        #region Properties

        public FileTypesControlViewModel VideoFileTypesViewModel
        {
            get
            {
                return videoFileTypesViewModel;
            }
            set
            {
                videoFileTypesViewModel = value;
                OnPropertyChanged(this, "VideoFileTypesViewModel");
            }
        }
        private FileTypesControlViewModel videoFileTypesViewModel;

        public FileTypesControlViewModel DeleteFileTypesViewModel
        {
            get
            {
                return seleteFileTypesViewModel;
            }
            set
            {
                seleteFileTypesViewModel = value;
                OnPropertyChanged(this, "DeleteFileTypesViewModel");
            }
        }
        private FileTypesControlViewModel seleteFileTypesViewModel;

        public FileTypesControlViewModel IgnoreFileTypesViewModel
        {
            get
            {
                return ignoreFileTypesViewModel;
            }
            set
            {
                ignoreFileTypesViewModel = value;
                OnPropertyChanged(this, "IgnoreFileTypesViewModel");
            }
        }
        private FileTypesControlViewModel ignoreFileTypesViewModel;

        public AutoMoveSetupsControlViewModel AutoMoveSetupsViewModel
        {
            get
            {
                return autoMoveSetupsViewModel;
            }
            set
            {
                autoMoveSetupsViewModel = value;
                OnPropertyChanged(this, "AutoMoveSetupsViewModel");
            }
        }
        private AutoMoveSetupsControlViewModel autoMoveSetupsViewModel;

        #endregion

        #region Constructor

        public FileTypeSettingsControlViewModel(ObservableCollection<string> videoFileTypes, ObservableCollection<string> deleteFileTypes, ObservableCollection<string> ignoreFileTypes, ObservableCollection<AutoMoveFileSetup> autoMoveSetups)
        {
            this.VideoFileTypesViewModel = new FileTypesControlViewModel(videoFileTypes);
            this.DeleteFileTypesViewModel = new FileTypesControlViewModel(deleteFileTypes);
            this.IgnoreFileTypesViewModel = new FileTypesControlViewModel(ignoreFileTypes);
            this.AutoMoveSetupsViewModel = new AutoMoveSetupsControlViewModel(autoMoveSetups);
        }

        #endregion

        #region Methods

        #endregion
    }
}
