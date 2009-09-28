using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonteCarloFlow;

namespace MonteCarloFlowTest
{
    [TestClass]
    public class FlowMetricsTest
    {
        [TestMethod]
        public void Test()
        {
            WorkItem j1 = new WorkItem(1, 4);
            WorkItem j2 = new WorkItem(3, 6);
            WorkItem j3 = new WorkItem(6, 9);

            FlowMetrics metrics = FlowMetrics.CalculateMetrics(j1, j2, j3);

            Assert.AreEqual(3,metrics.AverageCycleTime);
            Assert.AreEqual(0.375,metrics.AverageThroughput,0.01);
            Assert.AreEqual(1.125, metrics.AverageWip);
        }
    }
}
