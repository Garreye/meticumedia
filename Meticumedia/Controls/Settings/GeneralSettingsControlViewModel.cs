using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticumedia.Classes;
using Meticumedia.WPF;
using System.Windows.Input;
using Ookii.Dialogs.Wpf;

namespace Meticumedia.Controls
{
    public class GeneralSettingsControlViewModel : ViewModel
    {
        #region Properties

        public GeneralSettings GeneralSettings
        {
            get
            {
                return generalSettings;
            }
            set
            {
                generalSettings = value;
                OnPropertyChanged(this, "GeneralSettings");
            }
        }
        private GeneralSettings generalSettings;


        #endregion

        #region Commands

        private ICommand setTorrentDirectoryCommand;
        public ICommand SetTorrentDirectoryCommand
        {
            get
            {
                if (setTorrentDirectoryCommand == null)
                {
                    setTorrentDirectoryCommand = new RelayCommand(
                        param => this.SetTorrentDirectory()
                    );
                }
                return setTorrentDirectoryCommand;
            }
        }

        #endregion

        #region Constructor

        public GeneralSettingsControlViewModel(GeneralSettings genSettings)
        {
            this.GeneralSettings = new GeneralSettings(genSettings);
        }

        #endregion

        private void SetTorrentDirectory()
        {
            // Open folder browser
            VistaFolderBrowserDialog folderSel = new VistaFolderBrowserDialog();

            // Add folder if valid folder selected
            if (folderSel.ShowDialog() == true && System.IO.Directory.Exists(folderSel.SelectedPath))
            {
                this.GeneralSettings.TorrentDirectory = folderSel.SelectedPath;
            }
        }
    }
}
