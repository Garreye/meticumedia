using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meticumedia.Classes;
using Meticumedia.WPF;

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

        #region Constructor

        public GeneralSettingsControlViewModel(GeneralSettings genSettings)
        {
            this.GeneralSettings = new GeneralSettings(genSettings);
        }

        #endregion
    }
}
