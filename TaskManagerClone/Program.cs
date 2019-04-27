using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;

namespace TaskManagerClone
{
    class Program
    {
        static void Main(string[] args)
        {
            var WorkingSetPrivateValues = LoadWorkingSetPrivateValues();

            //using (var items = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", Process.GetCurrentProcess().Id)).Get())
            using (var items = new ManagementObjectSearcher(String.Format("Select * From Win32_Process")).Get())
            {
                foreach (var item in items)
                {
                    var Name = (String)item["Name"];
                    var ProcessId = (UInt32)item["ProcessId"];

                    if (ProcessId == 0)
                    {
                        // Skip System Idle
                        continue;
                    }

                    var WorkingSetSize = (UInt64)item["WorkingSetSize"]; // bytes
                    var PageFileUsage = (UInt32)item["PageFileUsage"]; // kb (Commit Size in Task Manager)
                    var PrivatePageCount = (UInt64)item["PrivatePageCount"]; // same as PageFileUsage?

                    Debug.Assert(PageFileUsage == (PrivatePageCount / 1024));

                    var WorkingSetPrivate = WorkingSetPrivateValues.ContainsKey(ProcessId) ? (UInt32?)WorkingSetPrivateValues[ProcessId] : null;
                    Debug.Assert(WorkingSetPrivate == null || WorkingSetSize >= WorkingSetPrivate);

                    Console.WriteLine(
                        "{0,-40} {1,11:N0} {2,11:N0} {3,11:N0} {4,11:N0}",
                        String.Format("{0} ({1}):", Name, ProcessId),
                        WorkingSetSize / 1024,
                        PageFileUsage,
                        PrivatePageCount / 1024,
                        WorkingSetPrivate / 1024);
                }
            }

            Console.WriteLine("SystemPageSize: {0}", Environment.SystemPageSize);
        }

        static Dictionary<UInt32, UInt64> LoadWorkingSetPrivateValues()
        {
            var WorkingSetPrivateValues = new Dictionary<UInt32, UInt64>();
            using (var items = new ManagementObjectSearcher(String.Format("Select WorkingSetPrivate, IDProcess From Win32_PerfRawData_PerfProc_Process")).Get())
            {
                foreach (var item in items)
                {
                    var IDProcess = (UInt32)item["IDProcess"];
                    var WorkingSetPrivate = (UInt64)item["WorkingSetPrivate"];
                    WorkingSetPrivateValues[IDProcess] = WorkingSetPrivate;
                }
            }
            return WorkingSetPrivateValues;
        }
    }
}
