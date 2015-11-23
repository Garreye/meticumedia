// --------------------------------------------------------------------------------
// Source code available at http://code.google.com/p/meticumedia/
// This code is released under GPLv3 http://www.gnu.org/licenses/gpl.html
// --------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Meticumedia.Classes
{
    /// <summary>
    /// Class that handles threading methods for processing of a set of organization paths.
    /// </summary>
    public class OrgProcessing
    {
        #region Delegates

        /// <summary>
        /// Delegate to be called by process thread when it is completed - used to keep track of number of completed threads
        /// </summary>
        public delegate void ProcessComplete();

        /// <summary>
        /// Delegate to handle processing of a single OrgPath instance specific during processing run.
        /// </summary>
        /// <param name="orgPath">Organization path instance to be processed</param>
        /// <param name="pathNum">The path's number out of total being processed</param>
        /// <param name="totalPaths">Total number of path being processed</param>
        /// <param name="updateNumber">The identifier for the OrgProcessing instance</param>
        /// <param name="background">Whether processing is running as a background operation</param>
        /// <param name="subSearch">Whether processing is sub-search - specific to instance</param>
        /// <param name="processComplete">Delegate to be called by processing when completed</param>
        /// <param name="numItemsProcessed">Number of paths that have been processed - used for progress updates</param>
        public delegate void Processing(OrgPath orgPath, int pathNum, int totalPaths, int updateNumber, ref int numItemsProcessed, ref int numItemsStarted, object processSpecificArgs);

        #endregion

        #region Properties

        /// <summary>
        /// The identifier for this instance of OrgProcessing.
        /// </summary>
        public int ProcessNumber { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with processing method to apply to OrgPath items during run.
        /// </summary>
        /// <param name="process">Delegate for processing OrgPath items during run</param>
        public OrgProcessing(Processing process)
        {
            // Store process
            this.process = process;

            // Set process number
            lock (totalLock)
                this.ProcessNumber = OrgProcessing.totalProcesses++;
        }

        #endregion

        #region Variables

        /// <summary>
        /// Lock for totalProcesses variable
        /// </summary>
        private static object totalLock = new object();

        /// <summary>
        /// Total number of instances of this class created.
        /// Used to set ProcessNumber property which identifies each instance.
        /// </summary>
        private static int totalProcesses = 0;

        /// <summary>
        /// Delegate to be used to process OrgPath item during run
        /// </summary>
        private Processing process = null;

        /// <summary>
        /// Total number of items processed during run
        /// </summary>
        private int numItemProcessed = 0;

        /// <summary>
        /// Total number of items that have been started during run
        /// </summary>
        private int numItemStarted = 0;

        /// <summary>
        /// Lock for modifying numItemProcessed variable
        /// </summary>
        private object processingLock = new object();

        #endregion

        #region Methods

        /// <summary>
        /// Run processing on set of OrgPath
        /// </summary>
        /// <param name="paths">List of paths to be processed</param>
        /// <param name="cancel">Cancellation variable</param>
        /// <param name="background">Whether processing is run in background</param>
        /// <param name="subSearch">Whether processing is sub-search type</param>
        public void Run(List<OrgPath> paths, ref bool cancel, object processSpecificArgs, int numProcessingThreads)
        {
            // Loop through all paths
            lock (processingLock)
                numItemProcessed = 0;

            // Get order to process paths in
            int i = 0;
            List<int> pathOrder = new List<int>();
            for (i = 0; i < paths.Count; i++)
                if (paths[i].SimilarTo < 0)
                    pathOrder.Add(i);
            for (i = 0; i < paths.Count; i++)
                if (paths[i].SimilarTo >= 0)
                    pathOrder.Add(i);


            for (i = 0; i < pathOrder.Count; i++)
            {
                // Limit number of threads
                while (i >= numItemProcessed + numProcessingThreads && !cancel)
                    Thread.Sleep(100);

                // Check for cancellation
                if (cancel)
                    break;

                // Create new processing thread for path
                object[] args = { paths[pathOrder[i]], paths.Count, pathOrder[i], processSpecificArgs };
                ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessThread), args);
            }

            // Wait for all threads to complete
            while (numItemProcessed < i && !cancel)
                Thread.Sleep(100);
        }

        /// <summary>
        /// Thread wrapper for processing delegate.
        /// </summary>
        /// <param name="stateInfo">Arguments for processing</param>
        private void ProcessThread(object stateInfo)
        {
            object[] args = (object[])stateInfo;
            OrgPath orgPath = (OrgPath)args[0];
            int totalPaths = (int)args[1];
            int updateNumber = (int)args[2];
            object processSpecificArgs = args[3];

            lock (processingLock)
                ++numItemStarted;

            process(orgPath, updateNumber, totalPaths, this.ProcessNumber, ref numItemProcessed, ref numItemStarted, processSpecificArgs);

            lock (processingLock)
                ++numItemProcessed;
        }

        #endregion
    }
}
