// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Meticumedia
{
    public partial class FileNameFormatControl : UserControl
    {
        public FileNameFormat FileNameFormat
        {
            get;
            private set;
        }
                
        public FileNameFormatControl()
        {
            InitializeComponent();
        }

        private bool isMovieFormat = false;

        public void LoadFormat(bool movie)
        {

            // Load datagridview combo box options
            foreach (FileWordType word in Enum.GetValues(typeof(FileWordType)))
            {
                // Only add word type specific to movie/tv
                if (word == FileWordType.None || word == FileWordType.Tv || word == FileWordType.Movie)
                    continue;
                if ((movie &&  (word & FileWordType.Movie) > 0) || (!movie && (word & FileWordType.Tv) > 0))
                    colType.Items.Add(word.ToString());
            }

            foreach (FileNamePortion.ContainerTypes type in Enum.GetValues(typeof(FileNamePortion.ContainerTypes)))
                colContainer.Items.Add(type.ToString());
            
            
            isMovieFormat = movie;
            
            // Get format from settings
            if (isMovieFormat)
                this.FileNameFormat = new FileNameFormat(Settings.MovieFileFormat);
            else
                this.FileNameFormat = new FileNameFormat(Settings.TvFileFormat);

            // Load format dgv
            dgvNameFormat.Rows.Clear();
            foreach (FileNamePortion portion in this.FileNameFormat.Format)
            {
                int row = dgvNameFormat.Rows.Add();
                dgvNameFormat.Rows[row].Cells[0].Value = portion.Type.ToString();
                dgvNameFormat.Rows[row].Cells[1].Value = portion.Container.ToString();
            }

            // No episode format editor for movies
            if (isMovieFormat)
            {
                gbEpFormat.Visible = false;
                gbFormat.Height += gbEpFormat.Height;
            }

            // Load episode format
            txtSeasonHeader.Text = this.FileNameFormat.EpisodeFormat.SeasonHeader;
            txtEpisodeHeader.Text = this.FileNameFormat.EpisodeFormat.EpisodeHeader;
            chkSeasonFirst.Checked = this.FileNameFormat.EpisodeFormat.SeasonFirst;
            chkHeaderPerEpisode.Checked = this.FileNameFormat.EpisodeFormat.HeaderPerEpisode;
            chkEpisodeDoubleDigits.Checked = this.FileNameFormat.EpisodeFormat.ForceEpisodeDoubleDigits;
            chkSeasonDoubleDigits.Checked = this.FileNameFormat.EpisodeFormat.ForceSeasonDoubleDigits;

            if (isMovieFormat)
            {
                lblExample1.Text = "Example 1 - 'Donnie Darko': 720p, bluray rip - x264, 5.1 audio - english, extended cut, file part 1";
                lblExample2.Text = "Example 2 - 'The Matrix': video/audio information unknown, no parts";
            }
            else
            {
                lblExample1.Text = "Example 1 - Single Episode: Episode 5 of season 1 of the show 'Arrested Development'";
                lblExample2.Text = "Example 2 - Double Episode: Episode 23 and 24 of season 9 of the show 'Seinfeld'";
            }

            disablePreviewBuild = false;
            UpdatePreview();
        }

        public void SetFormat()
        {
            BuildFormat();
            if (isMovieFormat)
                Settings.MovieFileFormat = this.FileNameFormat;
            else
                Settings.TvFileFormat = this.FileNameFormat;
        }

        private void UpdatePreview()
        {
            if (disablePreviewBuild)
                return;
            
            BuildFormat();
            
            if (isMovieFormat)
            {
                Movie movie = new Movie("Donnie Darko");
                movie.Date = new DateTime(2001, 1, 1);
                txtFilePreview1.Text = Path.GetFileNameWithoutExtension(this.FileNameFormat.BuildMovieFileName(movie, "Donnie Darko 720p blurayrip x264 5.1 en extended cut cd1.avi"));

                movie = new Movie("The Matrix");
                movie.Date = new DateTime(1999, 1, 1);
                txtFilePreview2.Text = Path.GetFileNameWithoutExtension(this.FileNameFormat.BuildMovieFileName(movie, "The Matrix.avi"));
            }
            else
            {
                txtFilePreview1.Text = this.FileNameFormat.BuildTvFileName(new TvEpisode("Charity Drive", "Arrested Development", 1, 5, "", ""), null, string.Empty);
                txtFilePreview2.Text = this.FileNameFormat.BuildTvFileName(new TvEpisode("The Finale (Part 1)", "Seinfeld", 9, 23, "", ""), new TvEpisode("The Finale (Part 2)", "Seinfeld", 9, 24, "", ""), string.Empty);
            }
        }

        private void BuildFormat()
        {
            this.FileNameFormat = new FileNameFormat(isMovieFormat);

            // Get file name format
            this.FileNameFormat.Format = new List<FileNamePortion>();
            for (int i = 0; i < dgvNameFormat.Rows.Count; i++)
            {
                if (dgvNameFormat.Rows[i].Cells[0].Value != null && dgvNameFormat.Rows[i].Cells[1].Value != null)
                {
                    FileWordType type;
                    Enum.TryParse<FileWordType>(dgvNameFormat.Rows[i].Cells[0].Value.ToString(), out type);

                    FileNamePortion.ContainerTypes container;
                    Enum.TryParse<FileNamePortion.ContainerTypes>(dgvNameFormat.Rows[i].Cells[1].Value.ToString(), out container);

                    string value = string.Empty;
                    if (dgvNameFormat.Rows[i].Cells[2].Value != null)
                        value = dgvNameFormat.Rows[i].Cells[2].Value.ToString();

                    this.FileNameFormat.Format.Add(new FileNamePortion(type, value, container));
                }
            }

            // Get episode format
            this.FileNameFormat.EpisodeFormat.SeasonHeader = txtSeasonHeader.Text;
            this.FileNameFormat.EpisodeFormat.EpisodeHeader = txtEpisodeHeader.Text;
            this.FileNameFormat.EpisodeFormat.SeasonFirst = chkSeasonFirst.Checked;
            this.FileNameFormat.EpisodeFormat.HeaderPerEpisode = chkHeaderPerEpisode.Checked;
            this.FileNameFormat.EpisodeFormat.ForceEpisodeDoubleDigits = chkEpisodeDoubleDigits.Checked;
            this.FileNameFormat.EpisodeFormat.ForceSeasonDoubleDigits = chkSeasonDoubleDigits.Checked;
        }

        private bool disablePreviewBuild = true;

        private void episodeFormatChanges(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        private void dgvNameFormat_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            UpdatePreview();
        }

    }
}
