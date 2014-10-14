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
            ContentSearch.DebugNotification += ContentSearch_DebugNotification;
        }

        

        #endregion

        #region Methods

        void ContentSearch_DebugNotification(object sender, DebugNotificationArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                this.MatchProcessing.Add(e.Notification);
            });
        }

        private void RunMatch()
        {
            string matchTo = this.MatchString;
            if (!this.MatchString.Contains('.'))
                matchTo += ".avi";

            this.MatchProcessing.Clear();
            
            DirectoryScan scan = new DirectoryScan(false);
            scan.DebugNotification += ContentSearch_DebugNotification;
            OrgPath path = new OrgPath(matchTo, false, true, new OrgFolder());
            OrgItem item = scan.ProcessPath(path, false, false, false, false, 0);

            this.MatchProcessing.Add("Final result: " + item.ToString());
        }

        #endregion
    }
}
