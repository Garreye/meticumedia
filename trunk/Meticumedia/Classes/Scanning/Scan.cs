using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Meticumedia
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
        /// Static event for indicatong scan progress has changed
        /// </summary>
        public event EventHandler<ProgressChangedEventArgs> ProgressChange;

        /// <summary>
        /// Triggers ScanProgressChange event
        /// </summary>
        public void OnProgressChange(ScanProcess process, string info, int percent)
        {
            if (ProgressChange != null)
                ProgressChange(process, new ProgressChangedEventArgs(percent, info));
        }

        #endregion

        #region Constructor

        public Scan(bool background)
        {
            this.background = background;
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
