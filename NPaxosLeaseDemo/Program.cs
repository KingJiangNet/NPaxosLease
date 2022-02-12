using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using NPaxosLease;
using NPaxosLeaseWorker;

namespace NPaxosLeaseDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1 || args[0] == "/?" || string.IsNullOrEmpty(args[0]))
            {
                ShowHelp();
                return;
            }

            // Use worker
            if (string.Compare(args[0], "-worker", StringComparison.OrdinalIgnoreCase) == 0)
            {
                if (args.Length < 3)
                {
                    ShowHelp();
                    return;
                }

                string workerConsoleOutput;
                Console.WriteLine("Election result: {0}", PaxosWorkerUtility.ElectMaster(args[1], Convert.ToInt32(args[2], CultureInfo.InvariantCulture), out workerConsoleOutput));
                Console.WriteLine(workerConsoleOutput);
                return;
            }

            string configFileName = args[0];

            // Start paxos lease
            PaxosLease.Start(configFileName, () =>
            {
                Console.WriteLine("Please press [Enter] to exit");
                Console.ReadLine();
                PaxosLease.Stop();
            });
        }

        private static void ShowHelp()
        {
            // NPaxosLeaseDemo 0.conf|worker
            Console.WriteLine("Usage: NPaxosLeaseDemo configFileName");
            Console.WriteLine("Usage: NPaxosLeaseDemo -worker configFileName maxElectionSeconds");
        }
    }
}
