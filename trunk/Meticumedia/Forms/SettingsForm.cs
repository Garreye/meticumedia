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
    /// Form for modifying global meticumedia settings.
    /// </summary>
    public partial class SettingsForm : Form
    {
        #region Constructor

        /// <summary>
        /// Constructor with tab to go to.
        /// </summary>
        /// <param name="tabIndex">Tab number to go to</param>
        public SettingsForm(int tabIndex)
        {
            InitializeComponent();
            LoadAll();
            tcSetting.SelectedIndex = tabIndex;
        }

        #endregion

        #region Events

        /// <summary>
        /// Static event that fires when static background color properties are changed.
        /// </summary>
        public static event EventHandler SettingsUpdated;

        /// <summary>
        /// Triggers BackColourChanged event
        /// </summary>
        public static void OnSettingsUpdated()
        {
            if (SettingsUpdated != null)
                SettingsUpdated(null, new EventArgs());
        }

        #endregion

        #region Form Event Handlers

        /// <summary>
        /// OK button saves settings and exits.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            SaveAll();
            OnSettingsUpdated();
            this.Close();
        }

        /// <summary>
        /// Cancel exits without saving settings.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Save/Load

        /// <summary>
        /// Load settings into each tab.
        /// </summary>
        private void LoadAll()
        {
            cntrlScanFolders.LoadScanFolders();
            cntrlTvFileNameFormat.LoadFormat(false);
            cntrlMovieFileNameFormat.LoadFormat(true);
            cntrlMovieFolders.LoadFolders(ContentType.Movie);
            cntrlTvFolders.LoadFolders(ContentType.TvShow);
            cntrlFileTypes.LoadFileTypes();
        }

        /// <summary>
        /// Set setting from each tabs and save them.
        /// </summary>
        private void SaveAll()
        {
            cntrlScanFolders.SetScanFolders();
            cntrlTvFileNameFormat.SetFormat();
            cntrlMovieFileNameFormat.SetFormat();
            cntrlMovieFolders.SetFolders();
            cntrlTvFolders.SetFolders();
            cntrlFileTypes.SaveFileTypes();
            Settings.Save();
        }

        #endregion
    }
}
