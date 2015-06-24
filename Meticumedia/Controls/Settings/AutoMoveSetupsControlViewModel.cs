using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Meticumedia.Classes;
using Meticumedia.WPF;

namespace Meticumedia.Controls
{
    public class AutoMoveSetupsControlViewModel : ViewModel
    {
        #region Properties

        public ObservableCollection<AutoMoveSetupControlViewModel> Setups { get; set; }

        public AutoMoveSetupControlViewModel SelectedSetup
        {
            get
            {
                return selectedSetup;
            }
            set
            {
                selectedSetup = value;
                OnPropertyChanged(this, "SelectedSetup");
            }
        }
        private AutoMoveSetupControlViewModel selectedSetup;

        #endregion

        #region Commands

        private ICommand addSetupCommand;
        public ICommand AddSetupCommand
        {
            get
            {
                if (addSetupCommand == null)
                {
                    addSetupCommand = new RelayCommand(
                        param => this.AddSetup()
                    );
                }
                return addSetupCommand;
            }
        }

        private ICommand removeSetupCommand;
        public ICommand RemoveSetupCommand
        {
            get
            {
                if (removeSetupCommand == null)
                {
                    removeSetupCommand = new RelayCommand(
                        param => this.RemoveSetup(),
                        param => this.CanDoRemoveSetups()
                    );
                }
                return removeSetupCommand;
            }
        }

        private bool CanDoRemoveSetups()
        {
            return this.SelectedSetup != null;
        }

        private ICommand clearSetupsCommand;
        public ICommand ClearSetupsCommand
        {
            get
            {
                if (clearSetupsCommand == null)
                {
                    clearSetupsCommand = new RelayCommand(
                        param => this.ClearSetups()
                    );
                }
                return clearSetupsCommand;
            }
        }

        #endregion

        #region Constructor

        public AutoMoveSetupsControlViewModel(ObservableCollection<AutoMoveFileSetup> setups)
        {
            this.Setups = new ObservableCollection<AutoMoveSetupControlViewModel>();
            foreach (AutoMoveFileSetup setup in setups)
                this.Setups.Add(new AutoMoveSetupControlViewModel(setup));
        }

        #endregion

        #region Methods

        private void AddSetup()
        {
            this.Setups.Add(new AutoMoveSetupControlViewModel(new AutoMoveFileSetup()));
        }

        private void RemoveSetup()
        {
            if (this.SelectedSetup == null)
                return;

            this.Setups.Remove(this.SelectedSetup);
        }

        private void ClearSetups()
        {
            this.Setups.Clear();
        }
        #endregion

    }
}
