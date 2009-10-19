using System;
using Flow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonteCarloFlow;

namespace MonteCarloFlowTest
{
    internal class FlowTestHelper
    {
        public static void TickAndAssert(WorkProcess process, params int[] expectedCounts)
        {
            TickAndAssert(1,process,expectedCounts);
        }

        public static void TickAndAssert(int ticks, WorkProcess process, params int[] expectedCounts)
        {
            if (expectedCounts.Length % 2 != 1)
            {
                throw new ArgumentException("Should be odd number of items", "expectedCounts");
            }

            process.Tick(ticks);

            int workStationCount = expectedCounts.Length / 2;

            for (int index = 0; index < workStationCount; index++)
            {
                Assert.AreEqual(expectedCounts[index * 2], process[index].InProcessCount);
                Assert.AreEqual(expectedCounts[index * 2 + 1], process[index].CompletionQueueCount);
            }

            Assert.AreEqual(expectedCounts[expectedCounts.Length - 1], process.CompletionQueueCount);
        }

    }
}