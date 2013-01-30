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

namespace Meticumedia
{
    /// <summary>
    /// Control for editing lists of files types saved in settings.
    /// </summary>
    public partial class FileTypesControl : UserControl
    {

        #region Constructor

        /// <summary>
        /// Default Contructor
        /// </summary>
        public FileTypesControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads files types from settings into control
        /// </summary>
        public void LoadFileTypes()
        {
            ftcVideo.FileTypes = Settings.VideoFileTypes;
            ftcDelete.FileTypes = Settings.DeleteFileTypes;
        }

        /// <summary>
        /// Sets file type from control to settings
        /// </summary>
        public void SaveFileTypes()
        {
            Settings.VideoFileTypes = ftcVideo.FileTypes;
            Settings.DeleteFileTypes = ftcDelete.FileTypes;
        }

        #endregion
    }
}
