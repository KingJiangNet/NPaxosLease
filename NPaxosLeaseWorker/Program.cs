using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NPaxosLease;

namespace NPaxosLeaseWorker
{
    class Program
    {
        static int Main(string[] args)
        {
            if (!Environment.Is64BitProcess)
                throw new BadImageFormatException("NPaxosLeaseWorker only can be run in 64bit mode.");

            int paxosEndpointIndex = -1;
            string configFileName;

            if (args.Length < 2 || args[0] == "/?" || string.IsNullOrEmpty(configFileName = args[0]))
            {
                // NPaxosLeaseWorker 0.conf
                Console.WriteLine("A worker process that returns the id(index) of elected node(endpoint).");
                Console.WriteLine("Usage: NPaxosLeaseWorker configFileName maxElectionSeconds");
                return paxosEndpointIndex;
            }

            // Start paxos lease
            try
            {
                int maxElectionSeconds = Convert.ToInt32(args[1], CultureInfo.InvariantCulture);

                PaxosLease.Start(configFileName, () =>
                {
                    SpinWait.SpinUntil(() =>
                    {
                        paxosEndpointIndex = PaxosLease.GetMaster();

                        return paxosEndpointIndex >= 0;
                    },
                    TimeSpan.FromSeconds(maxElectionSeconds));

                    // Stop paxos lease
                    PaxosLease.Stop();
                });
            }
            catch(Exception ex)
            {
                paxosEndpointIndex = -2;
                Console.WriteLine("Failed: {0}", ex);
            }

            Console.WriteLine("Result: {0}", paxosEndpointIndex);

            return paxosEndpointIndex;
        }
    }
}
