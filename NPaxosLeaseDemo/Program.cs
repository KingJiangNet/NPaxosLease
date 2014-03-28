using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NPaxosLease;

namespace NPaxosLeaseDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string configFileName;

            if (args.Length < 1 || string.IsNullOrEmpty(configFileName = args[0]))
            {
                Console.WriteLine("Usage: NPaxosLeaseDemo configFileName");
                return;
            }

            // Start paxos lease
            PaxosLease.Start(configFileName, () =>
            {
                Console.WriteLine("Please press [Enter] to exit");
                Console.ReadLine();
                PaxosLease.Stop();
            });
        }
    }
}
