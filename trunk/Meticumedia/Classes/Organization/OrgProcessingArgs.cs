// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Meticumedia.Classes.Organization
{
    class OrgProcessingArgs : ProgressChangedEventArgs
    {
        public bool NewItem { get; set; }

        public OrgProcessingArgs(bool newItem, int percent, string msg)
            : base(percent, msg)
        {
            this.NewItem = newItem;
        }
    }
}
