// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Meticumedia.Classes
{
    public abstract class Scan
    {
        #region Constants/Variables

        /// <summary>
        /// Indicates whether cancel was requested on scan work.
        /// </summary>
        protected bool cancelRequested = false;

        /// <summary>
        /// Indicates whether cancel was requested on scan work.
        /// </summary>
        protected static bool cancelAllRequested = false;

        /// <summary>
        /// Indicates whether a scan is currently running.
        /// </summary>
        protected bool scanRunning = false;

        protected bool background = false;

        protected bool scanCanceled { get { return (cancelRequested && !background) || cancelAllRequested; } }

        protected int scanNumber { get; private set; }

        protected void IncrementScanNumber()
        {
            ++scanNumber;
        }

        /// <summary>
        /// String to use as dir. if TV folders not setup yet
        /// </summary>
        protected static readonly string NO_TV_FOLDER = "NO TV FOLDERS SPECIFIED IN SETTINGS";

        /// <summary>
        /// String to use as dir. if movie folders not setup yet
        /// </summary>
        protected static readonly string NO_MOVIE_FOLDER = "NO MOVIE FOLDERS SPECIFIED IN SETTINGS";

        #endregion

        #region Events

        /// <summary>
        /// Event indicatong scan progress has changed
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> ProgressChange;

        /// <summary>
        /// Triggers ProgressChange event
        /// </summary>
        public void OnProgressChange(ScanProcess process, string info, int percent)
        {
            if (ProgressChange != null)
                ProgressChange(process, new ProgressChangedEventArgs(percent, info));
        }

        /// <summary>
        /// Event indicatong scan progress has changed
        /// </summary>
        public event EventHandler<ItemsInitializedArgs> ItemsInitialized;

        /// <summary>
        /// Triggers ItemsInitialized event
        /// </summary>
        public void OnItemsInitialized(ScanProcess process, List<OrgItem> items)
        {
            if (ItemsInitialized != null)
                ItemsInitialized(process, new ItemsInitializedArgs(items));
        }

        /// <summary>
        /// Scan items initilization event arguments
        /// </summary>
        public class ItemsInitializedArgs : EventArgs
        {
            public List<OrgItem> Items { get; private set; }

            public ItemsInitializedArgs(List<OrgItem> items)
            {
                this.Items = items;
            }
        }

        /// <summary>
        /// Debug noitification message event
        /// </summary>
        public event EventHandler<DebugNotificationArgs> DebugNotification;

        /// <summary>
        /// Triggers DebugNotification event
        /// </summary>
        protected void OnDebugNotificationd(string message)
        {
            if (DebugNotification != null)
                DebugNotification(this, new DebugNotificationArgs(message) );
        }

        #endregion

        #region Properties

        public List<OrgItem> Items { get; protected set; }

        #endregion

        #region Constructor

        public Scan(bool background)
        {
            this.background = background;
            this.Items = new List<OrgItem>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cancels the currently running scan (if any).
        /// </summary>
        public void CancelScan()
        {
            if (scanRunning)
            {
                cancelRequested = true;
                do
                {
                    Thread.Sleep(100);
                } while (scanRunning);
            }
        }

        public static void CancelAllScans()
        {
            cancelAllRequested = true;
        }

        #endregion
        
    }
}
