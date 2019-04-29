using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TaskManagerClone;

namespace MemoryTrackerUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMaxSize()
        {
            var tracker = new MemoryUsageTracker(10);
            for (int i = 1; i <= 17; i++)
            {
                tracker.AddCheckpoint(string.Format("checkpoint-{0}", i));
            }
            tracker.Stringify();
        }
    }
}
