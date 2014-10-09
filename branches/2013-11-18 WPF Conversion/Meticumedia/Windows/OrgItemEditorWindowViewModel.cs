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
    public class OrgItemEditorWindowViewModel : ViewModel
    {
        #region Properties

        /// <summary>
        /// Orignal item before editing started
        /// </summary>
        public OrgItem OriginalItem
        {
            get
            {
                return originalItem;
            }
            set
            {
                originalItem = value;
                OnPropertyChanged(this, "OriginalItem");
            }
        }
        private OrgItem originalItem;

        /// <summary>
        /// Item being edited
        /// </summary>
        public OrgItem Item
        {
            get
            {
                return item;
            }
            set
            {
                item = value;
                OnPropertyChanged(this, "Item");
            }
        }
        private OrgItem item;

        /// <summary>
        /// Modification made set to OK by user
        /// </summary>
        public bool ResultsOk { get; private set; }

        /// <summary>
        /// View model for movie editor
        /// </summary>
        public ContentEditorControlViewModel MovieViewModel
        {
            get;
            set;

        }
        private ContentEditorControlViewModel movieViewModel;

        /// <summary>
        /// List of TV shows available for selecting
        /// </summary>
        public ObservableCollection<TvShow> Shows { get; set; }

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
                        param => this.CancelResults()
                    );
                }
                return cancelCommand;
            }
        }

        #endregion

        #region Constructor

        public OrgItemEditorWindowViewModel(OrgItem item)
        {
            this.OriginalItem = item;
            this.Item = new OrgItem(item);
            this.Item.PropertyChanged += Item_PropertyChanged;

            // Setup avilable shows
            this.Shows = new ObservableCollection<TvShow>();
            foreach (TvShow show in Organization.Shows)
                this.Shows.Add(show);
            if (!string.IsNullOrEmpty(this.Item.TvEpisode.Show.DatabaseName) && this.Shows.Count > 0)
                this.Item.TvEpisode.Show = this.Shows[0];
            if (!this.Shows.Contains(this.Item.TvEpisode.Show))
                this.Shows.Add(this.Item.TvEpisode.Show);
        }

        void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Category":
                    switch (this.Item.Category)
                    {
                        case FileCategory.Empty:
                            break;
                        case FileCategory.Unknown:
                            break;
                        case FileCategory.Ignored:
                            break;
                        case FileCategory.TvVideo:
                            break;
                        case FileCategory.MovieVideo:
                            movieViewModel.Content = this.Item.Movie;
                            break;
                        case FileCategory.Trash:
                            break;
                        case FileCategory.Custom:
                            break;
                        case FileCategory.Folder:
                            break;
                        case FileCategory.All:
                            break;
                        default:
                            break;
                    }
                    break;
                case "Action":
                    switch (this.Item.Action)
                    {
                        case OrgAction.Empty:
                            break;
                        case OrgAction.None:
                            break;
                        case OrgAction.AlreadyExists:
                            break;
                        case OrgAction.Move:
                            break;
                        case OrgAction.Copy:
                            break;
                        case OrgAction.Rename:
                            break;
                        case OrgAction.Delete:
                            break;
                        case OrgAction.Queued:
                            break;
                        case OrgAction.NoRootFolder:
                            break;
                        case OrgAction.TBD:
                            break;
                        case OrgAction.Processing:
                            break;
                        case OrgAction.All:
                            break;
                        default:
                            break;
                    }
                    break;
            }
        }

        #endregion

        #region Methods

        public void OkResults()
        {
            this.ResultsOk = true;   
        }

        public void CancelResults()
        {
            
        }

        #endregion
    }
}
