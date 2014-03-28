using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NPaxosLease
{
    public static class PaxosLease
    {
        #region Internal Methods
        private static void RegisterCallback(Action callback)
        {
            if (callback != null)
            {
                // Wait to call back
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    SpinWait.SpinUntil(NativeMethods.IsRunning);
                    callback();
                });
            }
        }
        #endregion Internal Methods

        #region Public Methods
        /// <summary>
        /// Starts with the specified config file name.
        /// </summary>
        /// <param name="configFileName">Name of the config file.</param>
        /// <param name="callback">The callback.</param>
        public static void Start(string configFileName, Action callback = null)
        {
            RegisterCallback(callback);

            NativeMethods.Start(configFileName);
        }

        /// <summary>
        /// Starts with the specified config file name asynchronously.
        /// </summary>
        /// <param name="configFileName">Name of the config file.</param>
        /// <param name="callback">The callback.</param>
        public static void StartAsync(string configFileName, Action callback = null)
        {
            RegisterCallback(callback);

            // Start
            new Thread(() =>
            {
                PaxosLease.Start(configFileName);
            }).Start();
        }

        /// <summary>
        /// Stops the paxos lease.
        /// </summary>
        public static void Stop()
        {
            NativeMethods.Stop();
        }

        /// <summary>
        /// Gets the master.
        /// </summary>
        public static int GetMaster()
        {
            return NativeMethods.GetMaster();
        }
        #endregion Public Methods
    }
}
