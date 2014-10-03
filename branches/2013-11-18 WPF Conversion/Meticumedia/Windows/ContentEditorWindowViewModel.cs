using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Meticumedia.Classes;
using Meticumedia.Controls;
using Meticumedia.WPF;

namespace Meticumedia.Windows
{
    public class ContentEditorWindowViewModel : ViewModel
    {
        #region Events

        public event EventHandler ResultsSet;

        private void OnResultsSet()
        {
            if (ResultsSet != null)
                ResultsSet(this, new EventArgs());
        }

        #endregion


        #region Properties

        public ContentEditorControlViewModel ControlViewModel
        {
            get
            {
                return controlViewModel;
            }
            set
            {
                controlViewModel = value;
                OnPropertyChanged(this, "ControlViewModel");
            }
        }
        private ContentEditorControlViewModel controlViewModel;

        private Content content;
        public Content Content
        {
            get
            {
                return controlViewModel.Content;
            }
            set
            {
                controlViewModel.Content = value;
                OnPropertyChanged(this, "Content");
            }
        }

        public bool ResultsOk { get; set; }

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

        private bool CanDoOkCommand()
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
                        param => this.CanCancelCommand()
                    );
                }
                return cancelCommand;
            }
        }

        private bool CanCancelCommand()
        {
            return true;
        }

        #endregion

        #region Constructor

        public ContentEditorWindowViewModel(Content content)
        {
            controlViewModel = new ContentEditorControlViewModel(content);
            
            this.ResultsOk = false;
        }

        #endregion

        #region Methods

        public void OkResults()
        {
            this.ResultsOk = true;
            OnResultsSet();
        }

        public void CancelResults()
        {
            OnResultsSet();   
        }
        
        #endregion
    }
}
