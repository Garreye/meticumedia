// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Meticumedia
{
    /// <summary>
    /// Form for editing an organization action
    /// </summary>
    public partial class OrgItemEditor : Form
    {
        #region Properties

        /// <summary>
        /// Resulting modified item.
        /// </summary>
        public OrgItem Result { get { return result; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with OrgItem to be edited passed in.
        /// </summary>
        /// <param name="result"></param>
        public OrgItemEditor(OrgItem result)
        {
            InitializeComponent();

            //if(result.ScanDirectory != null)
                //this.rootFolder = result.ScanDirectory.FolderPath;
            if (result.Movie != null)
                this.rootFolder = result.Movie.RootFolder;

            // Set source
            txtSourceFile.Text = result.SourcePath;

            ContentRootFolder root;
            rbMovie.Enabled = Settings.GetDefaultMovieFolder(out root);
            rbTv.Enabled = Settings.GetDefaultTvFolder(out root);

            // Select action
            switch(result.Action)
            {
                case OrgAction.AlreadyExists:
                    rbNone.Checked = true;
                    break;
                case OrgAction.None:
                    rbNone.Checked = true;
                    break;
                case OrgAction.Move:
                    rbMove.Checked = true;
                    break;
                case OrgAction.Copy:
                    rbCopy.Checked = true;
                    break;
                case OrgAction.Rename:
                    rbRename.Checked = true;
                    break;
                case OrgAction.Delete:
                    rbDelete.Checked = true;
                    break;
                case OrgAction.Queued:
                    rbNone.Checked = true;
                    break;
                default:
                    throw new Exception("unknown action");
            }

            // Select category
            switch (result.Category)
            {
                case FileHelper.FileCategory.NonTvVideo:
                    rbMovie.Checked = true;
                    break;
                case FileHelper.FileCategory.Custom:
                    rbCustom.Checked = true;
                    break;
                default:
                    rbTv.Checked = true;
                    break;
            }

            // Setup TV
            cmbShows.Items.Clear();
            ContentCollection shows = Organization.Shows;
            if (result.NewShow != null)
            {
                cmbShows.Items.Add(result.NewShow);
                selectedShow = result.NewShow;
                cmbShows.SelectedIndex = cmbShows.Items.Count - 1;
            }

            foreach (TvShow show in shows)
            {
                cmbShows.Items.Add(show);
                if (result.TvEpisode != null && show.Name == result.TvEpisode.Show || cmbShows.Items.Count == 1)
                {
                    selectedShow = show;
                    cmbShows.SelectedIndex = cmbShows.Items.Count - 1;
                }
            }
            if (result.TvEpisode != null)
            {
                numSeason.Value = result.TvEpisode.Season;
                numEpisode1.Value = result.TvEpisode.Number;
                chkEpisode2.Checked = result.TvEpisode2 != null;
                numEpisode2.Value = result.TvEpisode2 != null ? result.TvEpisode2.Number : 0;
            }

            SetEpisodeName();

            if (result.Movie != null)
                cntrlMovieEdit.Content = new Movie(result.Movie);
            else
            {
                cntrlMovieEdit.Content = new Movie();
                ContentRootFolder defaultMovieFolder;
                if(Settings.GetDefaultMovieFolder(out defaultMovieFolder))
                    cntrlMovieEdit.Content.RootFolder = string.IsNullOrEmpty(rootFolder) ? defaultMovieFolder.FullPath : rootFolder;
                cntrlMovieEdit.Content.Path = ((Movie)cntrlMovieEdit.Content).BuildFilePath(txtSourceFile.Text);
                cntrlMovieEdit.SearchEntry = FileHelper.BasicSimplify(Path.GetFileNameWithoutExtension(result.SourcePath), false);
            }
            cntrlMovieEdit.ContentChanged += new EventHandler<EventArgs>(cntrlMovieEdit_MovieChanged);

            txtDestination.Text = BuildDestination();
        }

        #endregion

        #region Variables

        /// <summary>
        /// Currently selected show
        /// </summary>
        private TvShow selectedShow = null;

        /// <summary>
        /// Root folder of item being edited belongs to.
        /// </summary>
        private string rootFolder = string.Empty;

        /// <summary>
        /// Private variable for modify Result property.
        /// </summary>
        private OrgItem result = null;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Action radio button change causes control enables and destination to be re-evaluated
        /// </summary>
        private void actionCheckedChanged(object sender, EventArgs e)
        {
            SetGroupEnables();
            txtDestination.Text = BuildDestination();
        }

        /// <summary>
        /// Category selection change causes control enables and destination to be re-evaluated
        /// </summary>
        private void categoryCheckedChanged(object sender, EventArgs e)
        {
            SetGroupEnables();
            txtDestination.Text = BuildDestination();
        }

        /// <summary>
        /// Change to field in TV group box causes control enables and destination to be re-evaluated
        /// </summary>
        private void tvFieldChanged(object sender, EventArgs e)
        {
            if (cmbShows.SelectedIndex != -1)
                selectedShow = (TvShow)cmbShows.SelectedItem;
            else
                selectedShow = null;

            SetEpisodeName();

            numEpisode2.Enabled = chkEpisode2.Checked;
            txtDestination.Text = BuildDestination();
        }

        /// <summary>
        /// Set episode name of selected episodes
        /// </summary>
        private void SetEpisodeName()
        {
            if (selectedShow != null)
            {
                TvEpisode ep1, ep2;
                GetTvEpsiodes(out ep1, out ep2);
                txtEpisodeName.Text = Settings.TvFileFormat.BuildEpisodeName(ep1, ep2);
            }
        }

        /// <summary>
        /// The destination button opens a file selection dialog
        /// </summary>
        private void btnDestination_Click(object sender, EventArgs e)
        {
            // Setup file dialog
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.InitialDirectory = Path.GetDirectoryName(txtSourceFile.Text);
            string extension = Path.GetExtension(txtSourceFile.Text).Remove(0, 1);
            sfd.Filter = extension.ToUpper() + " File|*" + Path.GetExtension(txtSourceFile.Text);

            // If file selection made set destination textbox to it
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                txtDestination.Text = sfd.FileName;
        }

        /// <summary>
        /// Movie editor control changes cause destination path to be evaluated
        /// </summary>
        private void cntrlMovieEdit_MovieChanged(object sender, EventArgs e)
        {
            txtDestination.Text = BuildDestination();
        }

        /// <summary>
        /// OK button sets results and closes form
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            // Get results from GUI
            BuildResult();

            // Confirm replace if file already exists
            if (File.Exists(result.DestinationPath))
            {
                if (MessageBox.Show("Destination file already exists. Are you sure you want to replace it?", "Sure?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    result.Replace = true;
                    this.Close();
                }
                else
                    return;
            }
            
            // Close window
            this.Close();
        }

        /// <summary>
        /// Cancel closes form (without saving results)
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// New show opens search form and adds selected show to list of shows
        /// </summary>
        private void btnNewShow_Click(object sender, EventArgs e)
        {
            SearchForm form = new SearchForm(FileHelper.RemoveEpisodeInfo(Path.GetFileNameWithoutExtension(txtSourceFile.Text)), ContentType.TvShow);
            form.ShowDialog();

            if (form.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                TvShow newShow = new TvShow(form.Results);
                newShow = TvDatabaseHelper.FullShowSeasonsUpdate(newShow);
                cmbShows.Items.Add(newShow);
                cmbShows.SelectedItem = newShow;
                selectedShow = newShow;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets action type from action radio buttons
        /// </summary>
        /// <returns>Action type set in form</returns>
        private OrgAction GetAction()
        {
            if (rbNone.Checked)
                return OrgAction.None;
            else if (rbMove.Checked)
                return OrgAction.Move;
            else if (rbCopy.Checked)
                return OrgAction.Copy;
            else if (rbRename.Checked)
                return OrgAction.Rename;
            else if (rbDelete.Checked)
                return OrgAction.Delete;
            else
                throw new Exception("unknown action");
        }

        /// <summary>
        /// Gets category from category radio buttons
        /// </summary>
        /// <returns>Item category set in form</returns>
        private FileHelper.FileCategory GetCategory()
        {
            if (rbDelete.Checked)
                return FileHelper.FileCategory.Trash;
            else if (rbCustom.Checked)
                return FileHelper.FileCategory.Custom;
            else if (rbMovie.Checked)
                return FileHelper.FileCategory.NonTvVideo;
            else
                return FileHelper.FileCategory.TvVideo;
        }

        /// <summary>
        /// Set group box enables based on action and category selections
        /// </summary>
        private void SetGroupEnables()
        {
            gbCategory.Enabled = rbMove.Checked || rbCopy.Checked || rbRename.Checked;
            gbTv.Enabled = rbTv.Checked && gbCategory.Enabled;
            gbMovie.Enabled = rbMovie.Checked && gbCategory.Enabled;
            txtDestination.ReadOnly = !(rbCustom.Checked && gbCategory.Enabled);
            btnDestination.Enabled = rbCustom.Checked && gbCategory.Enabled;
        }

        /// <summary>
        /// Build destination path based on form
        /// </summary>
        /// <returns></returns>
        private string BuildDestination()
        {
            // Get file category
            FileHelper.FileCategory category = GetCategory();

            // Build destination file based on category
            switch (category)
            {
                case FileHelper.FileCategory.Unknown:
                case FileHelper.FileCategory.Ignored:
                    return string.Empty;
                case FileHelper.FileCategory.Custom:
                    return txtDestination.Text;
                case FileHelper.FileCategory.Trash:
                    return FileHelper.DELETE_DIRECTORY;
                case FileHelper.FileCategory.TvVideo:
                    if (selectedShow == null)
                        return string.Empty;

                    TvEpisode ep1, ep2;
                    GetTvEpsiodes(out ep1, out ep2);
                    string fileName = selectedShow.BuildFilePath(ep1, ep2, string.Empty);

                    return fileName + Path.GetExtension(txtSourceFile.Text);
                case FileHelper.FileCategory.NonTvVideo:
                    if (cntrlMovieEdit.Content == null)
                        return string.Empty;
                    // TODO: don't use default path if alread in movies folder!
                    return ((Movie)cntrlMovieEdit.Content).BuildFilePath(txtSourceFile.Text);
                default:
                    throw new Exception("Unknown file category!");
            }
        }

        /// <summary>
        /// Builds TV episodes from entry in TV group box.
        /// </summary>
        /// <param name="ep1">Resulting first episode</param>
        /// <param name="ep2">Resulting second episode</param>
        private void GetTvEpsiodes(out TvEpisode ep1, out TvEpisode ep2)
        {
            ep1 = null;
            ep2 = null;
            
            if (selectedShow != null)
            {
                selectedShow.FindEpisode((int)numSeason.Value, (int)numEpisode1.Value, out ep1);
                if (chkEpisode2.Checked)
                    selectedShow.FindEpisode((int)numSeason.Value, (int)numEpisode2.Value, out ep2);
            }

            if (ep1 == null)
                ep1 = new TvEpisode(string.Empty, selectedShow.Name, (int)numSeason.Value, (int)numEpisode1.Value, "", "");
            if (chkEpisode2.Checked && ep2 == null)
                ep2 = new TvEpisode(string.Empty, selectedShow.Name, (int)numSeason.Value, (int)numEpisode2.Value, "", "");
        }

        /// <summary>
        /// Build resulting OrgItem from form entry
        /// </summary>
        private void BuildResult()
        {
            // Get TV episodes
            TvEpisode ep1, ep2;
            GetTvEpsiodes(out ep1, out ep2);

            // Build results
            if (rbMovie.Checked)
            {
                cntrlMovieEdit.Content.Found = true;
                cntrlMovieEdit.Content.Path = Path.GetDirectoryName(txtDestination.Text);
                this.result = new OrgItem(GetAction(), txtSourceFile.Text, GetCategory(), (Movie)cntrlMovieEdit.Content, txtDestination.Text, null);
            }
            else
            {
                this.result = new OrgItem(OrgStatus.Found, GetAction(), txtSourceFile.Text, txtDestination.Text, ep1, ep2, GetCategory(), null);
                if (rbTv.Checked && selectedShow != null && !Organization.Shows.Contains(selectedShow))
                    this.result.NewShow = selectedShow;                
            }

            if (this.result.Action == OrgAction.Move || this.result.Action == OrgAction.Copy || this.result.Action == OrgAction.Rename)
                this.result.Check = CheckState.Checked;
        }

        #endregion
    }
}
