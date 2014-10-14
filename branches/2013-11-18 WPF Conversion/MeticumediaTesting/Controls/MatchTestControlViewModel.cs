using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Meticumedia.WPF;
using System.Windows.Input;
using Meticumedia.Classes;

namespace MeticumediaTesting
{
    public class MatchTestWindowViewModel : ViewModel
    {
        #region Properties

        public string MatchString
        {
            get
            {
                return matchString;
            }
            set
            {
                matchString = value;
                OnPropertyChanged(this, "MatchString");
            }
        }
        private string matchString = string.Empty;

        public ObservableCollection<string> MatchProcessing { get; set; }

        #endregion

        #region Commands

        private ICommand runMatchCommand;
        public ICommand RunMatchCommand
        {
            get
            {
                if (runMatchCommand == null)
                {
                    runMatchCommand = new RelayCommand(
                        param => this.RunMatch()
                    );
                }
                return runMatchCommand;
            }
        }

        #endregion

        #region Constructor

        public MatchTestWindowViewModel()
        {
            this.MatchProcessing = new ObservableCollection<string>();
        }

        #endregion

        #region Methods

        private void RunMatch()
        {
            DirectoryScan scan = new DirectoryScan(false);
            string matchTo = this.MatchString + ".avi";
            OrgPath path = new OrgPath(matchTo, false, true, new OrgFolder(matchTo));
            OrgItem item = scan.ProcessPath(path, false, false, false, 0);

            this.MatchProcessing.Add(item.ToString());
        }

        #endregion
    }
}
