using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Meticumedia.Classes;
using Meticumedia.WPF;

namespace Meticumedia.Controls
{
    public class FileNameControlViewModel : ViewModel
    {
        #region Properties

        public FileNameFormat FileNameFormat
        {
            get
            {
                return fileNameFormat;
            }
            set
            {
                fileNameFormat = value;
                OnPropertyChanged(this, "FileNameFormat");
            }
        }
        private FileNameFormat fileNameFormat;

        public FileNamePortion SelectedFileNamePortion
        {
            get
            {
                return selectedFileNamePortion;
            }
            set
            {
                selectedFileNamePortion = value;
                OnPropertyChanged(this, "SelectedFileNamePortion");
            }
        }
        private FileNamePortion selectedFileNamePortion;

        public ContentType ContentType
        {
            get
            {
                return contentType;
            }
            set
            {
                contentType = value;
                OnPropertyChanged(this, "ContentType");
            }
        }
        private ContentType contentType;

        public string FileNamePreview1Title
        {
            get
            {
                return fileNamePreview1Title;
            }
            set
            {
                fileNamePreview1Title = value;
                OnPropertyChanged(this, "FileNamePreview1Title");
            }
        }
        private string fileNamePreview1Title;

        public string FileNamePreview1
        {
            get
            {
                return fileNamePreview1;
            }
            set
            {
                fileNamePreview1 = value;
                OnPropertyChanged(this, "FileNamePreview1");
            }
        }
        private string fileNamePreview1;

        public string FileNamePreview2Title
        {
            get
            {
                return fileNamePreview2Title;
            }
            set
            {
                fileNamePreview2Title = value;
                OnPropertyChanged(this, "FileNamePreview2Title");
            }
        }
        private string fileNamePreview2Title;

        public string FileNamePreview2
        {
            get
            {
                return fileNamePreview2;
            }
            set
            {
                fileNamePreview2 = value;
                OnPropertyChanged(this, "FileNamePreview2");
            }
        }
        private string fileNamePreview2;

        #endregion

        #region Commands

        private ICommand addSectionCommand;
        public ICommand AddSectionCommand
        {
            get
            {
                if (addSectionCommand == null)
                {
                    addSectionCommand = new RelayCommand(
                        param => this.AddSection()
                    );
                }
                return addSectionCommand;
            }
        }

        private ICommand removeSectionCommand;
        public ICommand RemoveSectionCommand
        {
            get
            {
                if (removeSectionCommand == null)
                {
                    removeSectionCommand = new RelayCommand(
                        param => this.RemoveSection()
                    );
                }
                return removeSectionCommand;
            }
        }

        private ICommand clearSectionsCommand;
        public ICommand ClearSectionsCommand
        {
            get
            {
                if (clearSectionsCommand == null)
                {
                    clearSectionsCommand = new RelayCommand(
                        param => this.ClearSections()
                    );
                }
                return clearSectionsCommand;
            }
        }

        private ICommand moveUpSectionCommand;
        public ICommand MoveUpSectionCommand
        {
            get
            {
                if (moveUpSectionCommand == null)
                {
                    moveUpSectionCommand = new RelayCommand(
                        param => this.MoveSection(true)
                    );
                }
                return moveUpSectionCommand;
            }
        }

        private ICommand moveDownSectionCommand;
        public ICommand MoveDownSectionCommand
        {
            get
            {
                if (moveDownSectionCommand == null)
                {
                    moveDownSectionCommand = new RelayCommand(
                        param => this.MoveSection(false)
                    );
                }
                return moveDownSectionCommand;
            }
        }        

        #endregion

        #region Constructor

        public FileNameControlViewModel(FileNameFormat format, ContentType type)
        {
            this.ContentType = type;
            this.FileNameFormat = new FileNameFormat(format);
            this.FileNameFormat.PropertyChanged += Format_PropertyChanged;
            this.FileNameFormat.EpisodeFormat.PropertyChanged += Format_PropertyChanged;
            this.FileNameFormat.Format.CollectionChanged += Format_CollectionChanged;
            foreach (FileNamePortion portion in this.FileNameFormat.Format)
                portion.PropertyChanged += Format_PropertyChanged;
            UpdatePreview();
        }

        #endregion

        #region Methods

        private void UpdatePreview()
        {
            // Build preview
            switch (this.ContentType)
            {
                case ContentType.Movie:
                    Movie movie = new Movie("Donnie Darko");
                    movie.DatabaseYear = 2001;
                    this.FileNamePreview1Title = "Example 1: 'Donnie Darko', 720p, bluray rip - x264, 5.1 audio - english, extended cut, file part 1";
                    this.FileNamePreview1 = System.IO.Path.GetFileNameWithoutExtension(this.FileNameFormat.BuildMovieFileName(movie, "Donnie Darko 720p blurayrip x264 5.1 en extended cut cd1.avi"));

                    movie = new Movie("The Matrix");
                    movie.DatabaseYear = 1999;
                    this.FileNamePreview2Title = "Example 2: 'The Matrix', video/audio information unknown, no parts";
                    this.FileNamePreview2 = System.IO.Path.GetFileNameWithoutExtension(this.FileNameFormat.BuildMovieFileName(movie, "The Matrix.avi"));
                    break;
                case ContentType.TvShow:
                    this.FileNamePreview1Title = "Example 1: Single Episode: Episode 5 of season 1 of the show 'Arrested Development'"; ;
                    this.FileNamePreview1 = this.FileNameFormat.BuildTvFileName(new TvEpisode("Charity Drive", new TvShow("Arrested Development"), 1, 5, "", ""), null, string.Empty);
                    this.FileNamePreview2Title = "Example 2: Double Episode: Episode 23 and 24 of season 9 of the show 'Seinfeld'";
                    this.FileNamePreview2 = this.FileNameFormat.BuildTvFileName(new TvEpisode("The Finale (Part 1)", new TvShow("Seinfeld"), 9, 23, "", ""), new TvEpisode("The Finale (Part 2)", new TvShow("Seinfeld"), 9, 24, "", ""), string.Empty);
                    break;
            }
        }

        private void Format_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdatePreview();
        }

        private void Format_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdatePreview();

            if (App.Current.Dispatcher.CheckAccess())
            {
                if (e.NewItems != null)
                    foreach (FileNamePortion addItem in e.NewItems)
                        addItem.PropertyChanged += Format_PropertyChanged;
            }
            else
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    if (e.NewItems != null)
                        foreach (FileNamePortion addItem in e.NewItems)
                            addItem.PropertyChanged += Format_PropertyChanged;
                });
        }

        private void AddSection()
        {
            this.FileNameFormat.Format.Add(new FileNamePortion());
        }

        private void RemoveSection()
        {
            if (this.SelectedFileNamePortion == null)
                return;

            this.FileNameFormat.Format.Remove(this.SelectedFileNamePortion);
        }

        private void ClearSections()
        {
            this.FileNameFormat.Format.Clear();
        }

        private void MoveSection(bool up)
        {
            if (this.SelectedFileNamePortion == null)
                return;

            int i;
            for (i = 0; i < this.FileNameFormat.Format.Count; i++)
                if (this.SelectedFileNamePortion == this.FileNameFormat.Format[i])
                    break;

            if (up && i > 0)
                this.FileNameFormat.Format.Move(i, i - 1);
            else if (!up && i < this.FileNameFormat.Format.Count - 1)
                this.FileNameFormat.Format.Move(i, i + 1);
        }

        #endregion
    }
}
