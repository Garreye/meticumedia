// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Meticumedia
{
    public class OrgProcessing
    {
        #region Delegates

        public delegate void ProcessComplete();

        public delegate void Processing(OrgPath orgPath, int pathNum, int totalPaths, int updateNumber, bool background, bool subSearch, ProcessComplete processComplete, ref int numItemsProcessed);

        #endregion

        public static object TotalLock = new object();

        public static int TotalProcesses = 0;

        public int ProcessNumber { get; private set; }

        #region Constructor

        public OrgProcessing(Processing process)
        {
            this.process = process;
            lock (TotalLock)
                this.ProcessNumber = OrgProcessing.TotalProcesses++;
        }

        #endregion

        #region Variables

        private Processing process = null;

        private int numItemProcessed = 0;

        private object processingLock = new object();

        #endregion

        #region Methods

        public void Run(List<OrgPath> paths, ref bool cancel, bool background, bool subSearch)
        {
            // Loop through all paths
            lock (processingLock)
                numItemProcessed = 0;
            for (int i = 0; i < paths.Count; i++)
            {
                // Limit number of threads
                while (i > numItemProcessed + 25 && (!cancel || background))
                    Thread.Sleep(100);

                // Check for cancellation
                if (cancel && !background)
                    break;

                // Create new processing thread for path
                object[] args = { paths[i], paths.Count, i, background, subSearch };
                ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessThread), args);
            }

            // Wait for all threads to complete
            while (numItemProcessed < paths.Count && (!cancel || background))
                Thread.Sleep(100);
        }

        private void ProcessThread(object stateInfo)
        {
            object[] args = (object[])stateInfo;
            OrgPath orgPath = (OrgPath)args[0];
            int totalPaths = (int)args[1];
            int updateNumer = (int)args[2];
            bool background = (bool)args[3];
            bool subSearch = (bool)args[4];

            process(orgPath, updateNumer, totalPaths, this.ProcessNumber, background, subSearch, Complete, ref numItemProcessed);
        }

        private void Complete()
        {
            lock (processingLock)
                ++numItemProcessed;
        }

        #endregion
    }
}
