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
    /// Control for display an legend item
    /// </summary>
    public partial class LegendBoxControl : UserControl
    {
        #region Properties

        /// <summary>
        /// Color of legend item
        /// </summary>
        public Color Color 
        {
            get
            {
                return pnlColor.BackColor;
            }
            set
            {
                pnlColor.BackColor = value;
            }
        }

        /// <summary>
        /// Label for legend item
        /// </summary>
        public string Label
        {
            get
            {
                return lblName.Text;
            }
            set
            {
                lblName.Text = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LegendBoxControl()
        {
            InitializeComponent();
        }

        #endregion
    }
}
