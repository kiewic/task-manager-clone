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
            var tracker = new MemoryUsageTracker(10);
            tracker.AddCheckpoint("foo");
            tracker.AddCheckpoint("bar");
            tracker.Stringify();
        }
    }
}
