// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Arguments to pass for during progress change of orginzation process thread
    /// </summary>
    class OrgProcessingArgs : ProgressChangedEventArgs
    {
        /// <summary>
        /// Whether a new item has been found during processing.
        /// </summary>
        public bool NewItem { get; set; }

        /// <summary>
        /// Constructor with known properties
        /// </summary>
        /// <param name="newItem">Whether a new item has been found</param>
        /// <param name="percent">Progress percentage</param>
        /// <param name="msg">Message to display with progress</param>
        public OrgProcessingArgs(bool newItem, int percent, string msg)
            : base(percent, msg)
        {
            this.NewItem = newItem;
        }
    }
}
