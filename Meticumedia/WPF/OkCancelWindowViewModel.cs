using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Meticumedia.WPF
{
    public class OkCancelWindowViewModel : ViewModel
    {
        #region Events

        public event EventHandler ResultsSet;

        protected void OnResultsSet()
        {
            if (ResultsSet != null)
                ResultsSet(this, new EventArgs());
        }

        #endregion

        #region Properties

        /// <summary>
        /// Window set to OK by user
        /// </summary>
        public bool ResultsOk { get; private set; }

        #endregion

        #region Commands

        private ICommand okCommand;
        public ICommand OkCommand
        {
            get
            {
                if (okCommand == null)
                {
                    okCommand = new RelayCommand(
                        param => this.OkResults(),
                        param => this.CanDoOkCommand()
                    );
                }
                return okCommand;
            }
        }

        protected virtual bool CanDoOkCommand()
        {
            return true;
        }


        private ICommand cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                {
                    cancelCommand = new RelayCommand(
                        param => this.CancelResults(),
                        param => this.CanDoCancelCommand()
                    );
                }
                return cancelCommand;
            }
        }

        protected virtual bool CanDoCancelCommand()
        {
            return true;
        }

        #endregion

        #region Methods

        protected virtual void OkResults()
        {
            this.ResultsOk = true;
            OnResultsSet();
        }

        protected virtual void CancelResults()
        {
            OnResultsSet();
        }

        #endregion
    }
}
