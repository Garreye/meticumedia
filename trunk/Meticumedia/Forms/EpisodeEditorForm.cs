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
    /// Form for editing a TV episode. Simply a shell for TV episode editor control.
    /// </summary>
    public partial class EpisodeEditorForm : Form
    {
        /// <summary>
        /// TV episode being edited.
        /// </summary>
        public TvEpisode Episode = null;

        public bool NumbersEnabled
        {
            set
            {
                cntrlEp.NumbersEnabled = value;
            }
        }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public EpisodeEditorForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor with episode to be edited passed in
        /// </summary>
        /// <param name="ep">Episode to edit</param>
        public EpisodeEditorForm(TvEpisode ep) : this()
        {
            cntrlEp.Episode = ep;
        }

        /// <summary>
        /// OK button saves changes from control episode to Episode property and closes form.
        /// </summary>
        private void btnOk_Click(object sender, EventArgs e)
        {
            Episode = cntrlEp.Episode;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Cancel closes form without saving episode.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
   