using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerClone
{
    public class MemoryUsageTracker
    {
        private readonly int maxCount;
        private readonly MemoryUsageTrackerRoot root;

        public MemoryUsageTracker(int maxSize)
        {
            this.maxCount = maxSize;
            root = new MemoryUsageTrackerRoot
            {
                Processes = CreateChildProcesses(),
                Checkpoints = new Queue<Checkpoint>(),
            };
        }

        public void AddCheckpoint(string label)
        {
            this.root.Checkpoints.Enqueue(CreateCheckpoint(label));
            if (this.root.Checkpoints.Count > this.maxCount)
            {
                this.root.Checkpoints.Dequeue();
            }
        }

        public string Stringify()
        {
            return this.ToString();
        }

        private IList<ProcessDetails> CreateChildProcesses()
        {
            var result = new List<ProcessDetails>();
            using (var items = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", Process.GetCurrentProcess().Id)).Get())
            {
                foreach (var item in items)
                {
                    var Name = (String)item["Name"];
                    var ProcessId = (UInt32)item["ProcessId"];
                    var CommandLine = (String)item["CommandLine"];
                    result.Add(new ProcessDetails()
                    {
                        Name = Name,
                        ProcessId = ProcessId,
                        CommandLine = CommandLine,
                    });
                }
            }
            return result;
        }

        private Checkpoint CreateCheckpoint(string label)
        {
            var result = new Checkpoint()
            {
                Label = label,
                Processes = new List<CheckpointItem>(this.root.Processes.Count),
            };
            using (var items = new ManagementObjectSearcher(String.Format("Select * From Win32_Process Where ParentProcessID={0}", Process.GetCurrentProcess().Id)).Get())
            {
                foreach (var item in items)
                {
                    var ProcessId = (UInt32)item["ProcessId"];
                    var WorkingSetSize = (UInt64)item["WorkingSetSize"]; // bytes (Working Set in Task Manager)
                    var PageFileUsage = (UInt32)item["PageFileUsage"]; // kb (Commit Size in Task Manager)
                    result.Processes.Add(new CheckpointItem()
                    {
                        ProcessId = ProcessId,
                        WorkingSet = WorkingSetSize / (ulong)1024,
                        CommitSize = PageFileUsage,
                    });
                }
            }
            return result;
        }

        class MemoryUsageTrackerRoot
        {
            public IList<ProcessDetails> Processes { get; set; }
            public Queue<Checkpoint> Checkpoints { get; set; }
        }

        class ProcessDetails {
            public uint ProcessId { get; set; }
            public string Name { get; set; }
            public string CommandLine { get; set; }
        }

        class Checkpoint
        {
            public string Label { get; set; }
            public IList<CheckpointItem> Processes { get; set; }
        }

        class CheckpointItem
        {
            /// <summary>
            /// Process ID
            /// </summary>
            public uint ProcessId { get; set; }

            /// <summary>
            /// Working set memory in kilobytes
            /// </summary>
            public ulong WorkingSet { get; set; }

            /// <summary>
            /// Commit size (PageFileUsage) in kylobytes
            /// </summary>
            public uint CommitSize { get; set; }
        }
    }
}
