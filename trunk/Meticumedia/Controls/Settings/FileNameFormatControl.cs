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
    /// <summary>
    /// Control for editing file name format (for both TV and movies)
    /// </summary>
    public partial class FileNameFormatControl : UserControl
    {
        #region Properties

        /// <summary>
        /// File name format instance being edited.
        /// </summary>
        public FileNameFormat FileNameFormat
        {
            get;
            private set;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public FileNameFormatControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Variables

        /// <summary>
        /// Whether file name format is for movie (vs. tv episode)
        /// </summary>
        private bool isMovieFormat = false;

        /// <summary>
        /// Disables preview building - to use during loading
        /// </summary>
        private bool disablePreviewBuild = true;

        #endregion

        #region Methods

        /// <summary>
        /// Loads file name format from settings into control.
        /// </summary>
        /// <param name="movie">Whether to load movie format (or tv format if false)</param>
        public void LoadFormat(bool movie)
        {
            // Save movie flag
            isMovieFormat = movie;
            
            // Disable building
            disablePreviewBuild = true;
            
            // Load datagridview combo box options
            foreach (FileWordType word in Enum.GetValues(typeof(FileWordType)))
            {
                // Only add word type specific to movie/tv
                if (word == FileWordType.None || word == FileWordType.Tv || word == FileWordType.Movie)
                    continue;
                if ((movie &&  (word & FileWordType.Movie) > 0) || (!movie && (word & FileWordType.Tv) > 0))
                    colType.Items.Add(word.ToString());
            }
            foreach (FileNamePortion.CaseOptionType type in Enum.GetValues(typeof(FileNamePortion.CaseOptionType)))
                colCase.Items.Add(type.ToString());            
            
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
                dgvNameFormat.Rows[row].Cells[0].Value = portion.Header;
                dgvNameFormat.Rows[row].Cells[1].Value = portion.Type.ToString();
                dgvNameFormat.Rows[row].Cells[2].Value = portion.Footer;
                dgvNameFormat.Rows[row].Cells[3].Value = portion.Whitespace;
                dgvNameFormat.Rows[row].Cells[4].Value = portion.CaseOption.ToString();
                dgvNameFormat.Rows[row].Cells[5].Value = portion.Value;
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

            // Set preview labels
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

            // Enable building
            disablePreviewBuild = false;

            // Update preview
            UpdatePreview();
        }

        /// <summary>
        /// Save file name format from control into settings.
        /// </summary>
        public void SetFormat()
        {
            BuildFormat();
            if (isMovieFormat)
                Settings.MovieFileFormat = this.FileNameFormat;
            else
                Settings.TvFileFormat = this.FileNameFormat;
        }

        /// <summary>
        /// Update file name format previews in control.
        /// </summary>
        private void UpdatePreview()
        {
            // Check if building disabled
            if (disablePreviewBuild)
                return;
            
            // Update format from control entrues
            BuildFormat();
            
            // Build preview
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

        /// <summary>
        /// Update localc file name format variable from control entries
        /// </summary>
        private void BuildFormat()
        {
            // Reset format
            this.FileNameFormat = new FileNameFormat(isMovieFormat);

            // Build base format from datagrid view
            this.FileNameFormat.Format = new List<FileNamePortion>();
            for (int i = 0; i < dgvNameFormat.Rows.Count; i++)
            {
                // If no value selected then row is invalid
                if (dgvNameFormat.Rows[i].Cells[1].Value != null)
                {
                    string header = string.Empty;
                    if (dgvNameFormat.Rows[i].Cells[0].Value != null)
                        header = dgvNameFormat.Rows[i].Cells[0].Value.ToString();
                    
                    FileWordType type;
                    Enum.TryParse<FileWordType>(dgvNameFormat.Rows[i].Cells[1].Value.ToString(), out type);

                    string footer = string.Empty;
                    if (dgvNameFormat.Rows[i].Cells[2].Value != null)
                        footer = dgvNameFormat.Rows[i].Cells[2].Value.ToString();

                    string whitespace = string.Empty;
                    if (dgvNameFormat.Rows[i].Cells[3].Value != null)
                        whitespace = dgvNameFormat.Rows[i].Cells[3].Value.ToString();

                    FileNamePortion.CaseOptionType caseOption;
                    Enum.TryParse<FileNamePortion.CaseOptionType>(dgvNameFormat.Rows[i].Cells[4].Value.ToString(), out caseOption);

                    string value = string.Empty;
                    if (dgvNameFormat.Rows[i].Cells[5].Value != null)
                        value = dgvNameFormat.Rows[i].Cells[5].Value.ToString();

                    this.FileNameFormat.Format.Add(new FileNamePortion(type, value, header, footer, caseOption, whitespace));
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

        #endregion

        #region Event Handlers

        /// <summary>
        /// Changes to episode format trigger preview update.
        /// </summary>
        private void episodeFormatChanges(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        /// <summary>
        /// Changes to datagridview trigger preview update.
        /// </summary>
        private void dgvNameFormat_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            UpdatePreview();
        }

        #endregion
    }
}
