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
    public partial class MovieControl : ContentControl
    {
        public MovieControl() : base(ContentType.Movie)
        {
            InitializeComponent();
        }
    }
}
