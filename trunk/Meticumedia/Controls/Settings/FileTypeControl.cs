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
    /// Control for user entry of set of file types.
    /// </summary>
    public partial class FileTypeControl : UserControl
    {
        #region Properties

        /// <summary>
        /// Title of groupbox
        /// </summary>
        public string Title
        {
            get
            {
                return gbTypes.Text;
            }
            set
            {
                gbTypes.Text = value;
            }
        }

        /// <summary>
        /// FileTypes that are in listview
        /// </summary>
        public List<string> FileTypes 
        {
            get
            {
                List<string> types = new List<string>();
                foreach (ListViewItem item in lvTypes.Items)
                    types.Add(item.Text);
                return types;
            }
            set
            {
                lvTypes.Items.Clear();
                foreach (string type in value)
                    lvTypes.Items.Add(type);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public FileTypeControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Add new type from text box when '+' button is clicked
        /// </summary>
        private void btnAddType_Click(object sender, EventArgs e)
        {
            string newType = txtNewType.Text;

            // First char must be dot
            if (newType[0] != '.')
            {
                MessageBox.Show("Extension must start with '.'");
                return;
            }

            lvTypes.Items.Add(newType);
        }

        /// <summary>
        /// Selected types in listview are removed when '-' is clicked
        /// </summary>
        private void btnRemType_Click(object sender, EventArgs e)
        {
            for (int i = lvTypes.SelectedIndices.Count - 1; i >= 0; i--)
                lvTypes.Items.RemoveAt(lvTypes.SelectedIndices[i]);
        }

        #endregion
    }
}
