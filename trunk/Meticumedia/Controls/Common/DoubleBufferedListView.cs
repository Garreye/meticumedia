// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Meticumedia
{
    /// <summary>
    /// Listview with double buffering enabled in display - to minimize jittering when items are cleared and re-added.
    /// </summary>
    public class DoubleBufferedListView : ListView
    {
        public DoubleBufferedListView()
            : base()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
