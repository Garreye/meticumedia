using System;
using System.Collections;
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
    public class FileTypesControlViewModel : ViewModel
    {
        #region Properties

        public string FileTypeEntry
        {
            get
            {
                return fileTypeEntry;
            }
            set
            {
                fileTypeEntry = value;
                OnPropertyChanged(this, "FileTypeEntry");
            }
        }
        private string fileTypeEntry;

        public ObservableCollection<string> FileTypes
        {
            get
            {
                return fileTypes;
            }
            set
            {
                fileTypes = value;
                OnPropertyChanged(this, "FileTypes");
            }
        }
        private ObservableCollection<string> fileTypes;

        public IList SelectedTypes { get; set; }

        #endregion

        #region Commands

        private ICommand addTypeCommand;
        public ICommand AddTypeCommand
        {
            get
            {
                if (addTypeCommand == null)
                {
                    addTypeCommand = new RelayCommand(
                        param => this.AddType(),
                        param => this.CanDoAddTypeCommand()
                    );
                }
                return addTypeCommand;
            }
        }

        private bool CanDoAddTypeCommand()
        {
            return !string.IsNullOrEmpty(this.FileTypeEntry);
        }

        private ICommand removeTypesCommand;
        public ICommand RemoveTypesCommand
        {
            get
            {
                if (removeTypesCommand == null)
                {
                    removeTypesCommand = new RelayCommand(
                        param => this.RemoveTypes(),
                        param => this.CanDoRemoveTypesCommand()
                    );
                }
                return removeTypesCommand;
            }
        }

        private bool CanDoRemoveTypesCommand()
        {
            return this.SelectedTypes != null && this.SelectedTypes.Count > 0;
        }

        #endregion

        #region Constructor

        public FileTypesControlViewModel(ObservableCollection<string> fileTypes)
        {
            this.FileTypes = new ObservableCollection<string>();
            foreach (string type in fileTypes)
                this.FileTypes.Add(type);
        }

        #endregion

        #region Methods

        private void AddType()
        {
            this.FileTypes.Add(this.FileTypeEntry);
        }

        private void RemoveTypes()
        {
            for (int i = this.SelectedTypes.Count - 1; i >= 0; i--)
                this.FileTypes.Remove(this.SelectedTypes[i] as string);
        }

        #endregion
    }
}
