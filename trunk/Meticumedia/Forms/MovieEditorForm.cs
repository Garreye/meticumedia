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

namespace Meticumedia
{
    /// <summary>
    /// Form for editing properties of a Movie object.
    /// </summary>
    public partial class MovieEditorForm : Form
    {
        #region Properties

        /// <summary>
        /// Resulting movie from editor form. Null when form cancelled.
        /// </summary>
        public Movie Results { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with movie to be edited passed in.
        /// </summary>
        /// <param name="movie"></param>
        public MovieEditorForm(Movie movie)
        {
            InitializeComponent();

            // Clone movie (so that changes here won't affect original unintentionally)
            cntrlMovie.Movie = new Movie(movie);
            SetUpdateButtonEnable();
            cntrlMovie.MovieChanged += new EventHandler<EventArgs>(cntrlMovie_MovieChanged);

            // Clear results
            this.Results = null;
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Enable update button if movie ID is valid.
        /// </summary>
        private void SetUpdateButtonEnable()
        {
            // Enable update button for valid IDs only
            btnUpdateInfo.Enabled = cntrlMovie.Movie.Id != 0;
        }

        /// <summary>
        /// Movie changed on movie editor control cause update button enable to be updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cntrlMovie_MovieChanged(object sender, EventArgs e)
        {
            SetUpdateButtonEnable();
        }

        /// <summary>
        /// Update movie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateInfo_Click(object sender, EventArgs e)
        {
            cntrlMovie.UpdateMovieInfo();
        }

        /// <summary>
        /// When OK button click the results are set and form is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Results = cntrlMovie.Movie;
            this.Close();
        }

        /// <summary>
        /// Cancel button simply closes the form. (Results will be null)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        #endregion
    }
}
