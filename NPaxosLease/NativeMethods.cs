using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NPaxosLease
{
    internal static class NativeMethods
    {
        [DllImport("NPaxosLease.Interop.dll", CharSet = CharSet.Ansi)]
        public static extern void Start(string configFileName);

        [DllImport("NPaxosLease.Interop.dll")]
        public static extern void Stop();

        [DllImport("NPaxosLease.Interop.dll")]
        public static extern bool IsRunning();

        [DllImport("NPaxosLease.Interop.dll")]
        public static extern int GetMaster();

        [DllImport("NPaxosLease.Interop.dll")]
        public static extern bool IsMasterLeaseActive();

        //[DllImport(@"NPaxosLease.Interop.dll", CharSet = CharSet.Unicode)]
        //[return: MarshalAs(UnmanagedType.BStr)]
        //public static extern string Init(string configFileName);
    }
}
