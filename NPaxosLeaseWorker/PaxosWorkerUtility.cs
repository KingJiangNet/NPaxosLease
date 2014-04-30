using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace NPaxosLeaseWorker
{
    /// <summary>
    /// The paxos worker utility.
    /// </summary>
    public static class PaxosWorkerUtility
    {
        #region Public Methods
        /// <summary>
        /// Elects the master(leader).
        /// </summary>
        /// <param name="configFileName">Name of the configuration file.</param>
        /// <param name="maxElectionSeconds">The maximum election seconds.</param>
        /// <param name="workerConsoleOutput">The worker console output.</param>
        /// <returns></returns>
        /// <exception cref="System.BadImageFormatException">NPaxosLeaseWorker only can be run in 64bit mode.</exception>
        public static int ElectMaster(string configFileName, int maxElectionSeconds, out string workerConsoleOutput)
        {
            if (!Environment.Is64BitProcess)
                throw new BadImageFormatException("NPaxosLeaseWorker only can be run in 64bit mode.");

            int paxosEndpointIndex;
            string path = Assembly.GetExecutingAssembly().Location;
            ProcessStartInfo startInfo = new ProcessStartInfo(path,
                string.Format("\"{0}\" {1}", configFileName, maxElectionSeconds.ToString(CultureInfo.InvariantCulture)));
                        
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;

            var process = Process.Start(startInfo);
            var output = new StringBuilder();

            process.OutputDataReceived += ((sender, e) => { output.AppendLine(e.Data); });
            process.ErrorDataReceived += ((sender, e) => { output.AppendLine(e.Data); });

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit(maxElectionSeconds * 1000);

            if(!process.HasExited)
            {
                process.Kill();
                paxosEndpointIndex = -3;
            }
            else
            {
                paxosEndpointIndex = process.ExitCode;
            }

            workerConsoleOutput = output.ToString();

            return paxosEndpointIndex;
        }
        #endregion Public Methods
    }
}
