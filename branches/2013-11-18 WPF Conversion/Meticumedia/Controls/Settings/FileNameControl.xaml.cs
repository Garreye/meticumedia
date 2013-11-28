using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Meticumedia.Classes;

namespace Meticumedia.Controls
{
    /// <summary>
    /// Interaction logic for TvFileNameControl.xaml
    /// </summary>
    public partial class FileNameControl : UserControl
    {
        public FileNameControl()
        {
            InitializeComponent();
        }

        private FileNameFormat fileNameFormat;

        private ContentType contentType;

        #region Save/Load

        public void LoadSettings(ContentType type)
        {
            this.contentType = type;
            switch (type)
            {
                case ContentType.Movie:
                    this.fileNameFormat = new FileNameFormat(Settings.MovieFileFormat);
                    break;
                case ContentType.TvShow:
                    this.fileNameFormat = new FileNameFormat(Settings.TvFileFormat);
                    break;
            }
            this.fileNameFormat.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(fileNameFormat_PropertyChanged);
            ctntFileNameFormat.Content = this.fileNameFormat;
            UpdatePreview();
        }

        void fileNameFormat_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdatePreview();
        }

        /// <summary>
        /// Update file name format previews in control.
        /// </summary>
        private void UpdatePreview()
        {
            // Build preview
            switch (contentType)
            {
                case ContentType.Movie:
                    Movie movie = new Movie("Donnie Darko");
                    movie.Date = new DateTime(2001, 1, 1);
                    txtFilePreview1.Text = System.IO.Path.GetFileNameWithoutExtension(this.fileNameFormat.BuildMovieFileName(movie, "Donnie Darko 720p blurayrip x264 5.1 en extended cut cd1.avi"));

                    movie = new Movie("The Matrix");
                    movie.Date = new DateTime(1999, 1, 1);
                    txtFilePreview2.Text = System.IO.Path.GetFileNameWithoutExtension(this.fileNameFormat.BuildMovieFileName(movie, "The Matrix.avi"));
                    break;
                case ContentType.TvShow:
                    txtFilePreview1.Text = this.fileNameFormat.BuildTvFileName(new TvEpisode("Charity Drive", new TvShow("Arrested Development"), 1, 5, "", ""), null, string.Empty);
                    txtFilePreview2.Text = this.fileNameFormat.BuildTvFileName(new TvEpisode("The Finale (Part 1)", new TvShow("Seinfeld"), 9, 23, "", ""), new TvEpisode("The Finale (Part 2)", new TvShow("Seinfeld"), 9, 24, "", ""), string.Empty);
                    break;
            }
        }

        public void SaveSettings()
        {
            switch (contentType)
            {
                case ContentType.Movie:
                    Settings.MovieFileFormat = fileNameFormat;
                    break;
                case ContentType.TvShow:
                    Settings.TvFileFormat = fileNameFormat;
                    break;
            }

        }

        #endregion
    }
}
