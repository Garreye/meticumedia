using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Meticumedia
{
    /// <summary>
    /// Organization updating progress changed event arguments
    /// </summary>
    public class OrgProgressChangesEventArgs : ProgressChangedEventArgs
    {
        /// <summary>
        /// Whether last updated item was new
        /// </summary>
        public bool NewItem { get; set; }

        /// <summary>
        /// Constructor with know properties
        /// </summary>
        /// <param name="newItem">Whether new item was found</param>
        /// <param name="percent">Progress percent</param>
        /// <param name="msg">Progress message</param>
        public OrgProgressChangesEventArgs(bool newItem, int percent, string msg)
            : base(percent, msg)
        {
            this.NewItem = newItem;
        }
    }
}
