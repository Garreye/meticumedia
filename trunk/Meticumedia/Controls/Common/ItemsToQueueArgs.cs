// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meticumedia
{
    /// <summary>
    /// Event argument for queueing items
    /// </summary>
    public class ItemsToQueueArgs : EventArgs
    {
        /// <summary>
        /// List of organization item to be queued
        /// </summary>
        public List<OrgItem> QueueItems { get; private set; }

        /// <summary>
        /// Constructor with item to be queue
        /// </summary>
        /// <param name="items">Items to be queue</param>
        public ItemsToQueueArgs(List<OrgItem> items)
        {
            this.QueueItems = items;
        }
    }
}
