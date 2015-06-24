using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Meticumedia.Classes;
using Meticumedia.WPF;
using Ookii.Dialogs.Wpf;

namespace Meticumedia.Controls
{
    public class AutoMoveSetupControlViewModel : ViewModel
    {
        #region Properties

        public AutoMoveFileSetup Setup
        {
            get
            {

                return setup;
            }
            set
            {
                setup = value;
                OnPropertyChanged(this, "Setup");
            }
        }
        private AutoMoveFileSetup setup;


        public FileTypesControlViewModel FileTypesViewModel
        {
            get
            {
                return fileTypesViewModel;
            }
            set
            {
                fileTypesViewModel = value;
                OnPropertyChanged(this, "FileTypesViewModel");
            }
        }
        private FileTypesControlViewModel fileTypesViewModel;

        #endregion

        #region Commands

        private ICommand modifyFolderPathCommand;
        public ICommand ModifyFolderPathCommand
        {
            get
            {
                if (modifyFolderPathCommand == null)
                {
                    modifyFolderPathCommand = new RelayCommand(
                        param => this.ModifyFolderPath()
                    );
                }
                return modifyFolderPathCommand;
            }
        }

        #endregion

        #region Constructor

        public AutoMoveSetupControlViewModel(AutoMoveFileSetup setup)
        {
            this.Setup = new AutoMoveFileSetup(setup);
            this.FileTypesViewModel = new FileTypesControlViewModel(setup.FileTypes);
            this.FileTypesViewModel.FileTypes.CollectionChanged += FileTypes_CollectionChanged;
        }


        #endregion

        #region Methods

        /// <summary>
        /// Keep file types in setup mirrored to file types in view model.
        /// </summary>
        private void FileTypes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.Setup.FileTypes.Clear();
            foreach (string fileType in this.FileTypesViewModel.FileTypes)
                this.Setup.FileTypes.Add(fileType);
        }

        private void ModifyFolderPath()
        {
            VistaFolderBrowserDialog folderSel = new VistaFolderBrowserDialog();
            folderSel.SelectedPath = this.Setup.DestinationPath;

            if (folderSel.ShowDialog() == true && System.IO.Directory.Exists(folderSel.SelectedPath))
                this.Setup.DestinationPath = folderSel.SelectedPath;
        }

        #endregion
    }
}
