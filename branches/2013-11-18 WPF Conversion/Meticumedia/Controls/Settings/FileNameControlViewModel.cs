using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        #region Constructor

        public FileNameControlViewModel(FileNameFormat format, ContentType type)
        {
            this.ContentType = type;
            this.FileNameFormat = new FileNameFormat(format);
            this.FileNameFormat.PropertyChanged += Format_PropertyChanged;
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

        #endregion

    }
}
