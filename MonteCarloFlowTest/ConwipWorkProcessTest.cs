using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonteCarloFlow;

namespace MonteCarloFlowTest
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ConwipWorkProcessTest
    {
        [TestMethod]
        public void ShouldRunSingleStationSingleDeterministicMachine()
        {
            ConwipWorkProcess process = new ConwipWorkProcess(new InfiniteBacklog(),4);
            WorkStation ws1 = new WorkStation(new WorkInProcessLimit(4));

            process.Add(ws1);

            IProbabilityDistribution distribution1 = new DeterministicDistribution(1);

            ws1.AddMachine(new Machine(distribution1));

            FlowTestHelper.TickAndAssert(process, 1, 0, 0);
            FlowTestHelper.TickAndAssert(process, 1, 0, 1);
            FlowTestHelper.TickAndAssert(process, 1, 0, 2);
            FlowTestHelper.TickAndAssert(process, 1, 0, 3);
            FlowTestHelper.TickAndAssert(process, 1, 0, 4);
        }


        [TestMethod]
        public void ShouldRunTwoStationSingleDeterministicMachineWorkProcess()
        {
            ConwipWorkProcess process = new ConwipWorkProcess(new InfiniteBacklog(),8);

            WorkInProcessLimit wipLimit = new WorkInProcessLimit(8);

            WorkStation ws1 = new WorkStation(wipLimit);
            WorkStation ws2 = new WorkStation(wipLimit);

            process.Add(ws1);
            process.Add(ws2);

            IProbabilityDistribution distribution1 = new DeterministicDistribution(1);
            IProbabilityDistribution distribution2 = new DeterministicDistribution(2);

            ws1.AddMachine(new Machine(distribution1));
            ws2.AddMachine(new Machine(distribution2));

            FlowTestHelper.TickAndAssert(process, 1, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(process, 1, 0, 1, 0, 0);
            FlowTestHelper.TickAndAssert(process, 1, 1, 1, 0, 0);
            FlowTestHelper.TickAndAssert(process, 1, 1, 1, 0, 1);
            FlowTestHelper.TickAndAssert(process, 1, 2, 1, 0, 1);
            FlowTestHelper.TickAndAssert(process, 1, 2, 1, 0, 2);

        }

        [TestMethod]
        public void ShouldRunTwoStationDeterministicMachineWorkProcessAndLastWorkStationWithTwoMachines()
        {
            ConwipWorkProcess process = new ConwipWorkProcess(new InfiniteBacklog(),8);

            WorkInProcessLimit wipLimit = new WorkInProcessLimit(8);

            WorkStation ws1 = new WorkStation(wipLimit);
            WorkStation ws2 = new WorkStation(wipLimit);

            process.Add(ws1);
            process.Add(ws2);

            IProbabilityDistribution distribution1 = new DeterministicDistribution(1);
            IProbabilityDistribution distribution2 = new DeterministicDistribution(2);

            ws1.AddMachine(new Machine(distribution1));
            ws2.AddMachine(new Machine(distribution2));
            ws2.AddMachine(new Machine(distribution2));

            FlowTestHelper.TickAndAssert(process, 1, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(process, 1, 0, 1, 0, 0);
            FlowTestHelper.TickAndAssert(process, 1, 0, 2, 0, 0);
            FlowTestHelper.TickAndAssert(process, 1, 0, 2, 0, 1);
            FlowTestHelper.TickAndAssert(process, 1, 0, 2, 0, 2);
            FlowTestHelper.TickAndAssert(process, 1, 0, 2, 0, 3);
        }


        [TestMethod]
        public void ShouldRunPooledMachines()
        {
            ConwipWorkProcess process = new ConwipWorkProcess(new InfiniteBacklog(), 8);

            WorkInProcessLimit wipLimit = new WorkInProcessLimit(8);

            WorkStation ws1 = new WorkStation(wipLimit);
            WorkStation ws2 = new WorkStation(wipLimit);

            process.Add(ws1);
            process.Add(ws2);

            IProbabilityDistribution distribution1 = new DeterministicDistribution(4);
            IProbabilityDistribution distribution2 = new DeterministicDistribution(4);

            ResourcePool pool = new ResourcePool(3);

            ws1.AddMachine(new PooledMachine(distribution1, pool));
            ws1.AddMachine(new PooledMachine(distribution1, pool));
            ws1.AddMachine(new PooledMachine(distribution1, pool));

            ws2.AddMachine(new PooledMachine(distribution2, pool));
            ws2.AddMachine(new PooledMachine(distribution2, pool));
            ws2.AddMachine(new PooledMachine(distribution2, pool));

            FlowTestHelper.TickAndAssert(process, 1, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(process, 2, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(process, 3, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(process, 3, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(process, 2, 0, 1, 0, 0);
            FlowTestHelper.TickAndAssert(process, 1, 0, 2, 0, 0);
            FlowTestHelper.TickAndAssert(process, 0, 0, 3, 0, 0);
            FlowTestHelper.TickAndAssert(process, 0, 0, 3, 0, 0);
            FlowTestHelper.TickAndAssert(process, 1, 0, 2, 0, 1);
        }

        [TestMethod]
        public void ShouldHandleProcessingTimeOfZeroForPooledMachine()
        {
            ConwipWorkProcess process = new ConwipWorkProcess(new InfiniteBacklog(), 5);

            WorkInProcessLimit wipLimit = new WorkInProcessLimit(5);

            WorkStation ws1 = new WorkStation(wipLimit);
            WorkStation ws2 = new WorkStation(wipLimit);
            WorkStation ws3 = new WorkStation(wipLimit);

            process.Add(ws1);
            process.Add(ws2);
            process.Add(ws3);

            IProbabilityDistribution distribution1 = new DeterministicDistribution(0);
            IProbabilityDistribution distribution2 = new DeterministicDistribution(1);
            IProbabilityDistribution distribution3 = new DeterministicDistribution(1);
            IProbabilityDistribution distribution4 = new DeterministicDistribution(1);

            ResourcePool pool = new ResourcePool(2);

            ws1.AddMachine(new PooledMachine(distribution1, pool));
            ws1.AddMachine(new PooledMachine(distribution1, pool));
            ws1.AddMachine(new PooledMachine(distribution1, pool));
            ws1.AddMachine(new PooledMachine(distribution1, pool));
            ws1.AddMachine(new PooledMachine(distribution1, pool));

            ws2.AddMachine(new PooledMachine(distribution2, pool));
            ws2.AddMachine(new PooledMachine(distribution2, pool));
            ws2.AddMachine(new PooledMachine(distribution2, pool));
            ws2.AddMachine(new PooledMachine(distribution3, pool));
            ws2.AddMachine(new PooledMachine(distribution3, pool));

            ws3.AddMachine(new PooledMachine(distribution4, pool));
            ws3.AddMachine(new PooledMachine(distribution4, pool));
            ws3.AddMachine(new PooledMachine(distribution4, pool));
            ws3.AddMachine(new PooledMachine(distribution4, pool));
            ws3.AddMachine(new PooledMachine(distribution4, pool));


            FlowTestHelper.TickAndAssert(process, 1, 0, 0, 0,0,0,0);
            FlowTestHelper.TickAndAssert(process, 1, 0, 1, 0,0,0,0);
            FlowTestHelper.TickAndAssert(process, 0, 0, 1, 0 ,1,0,0);
            FlowTestHelper.TickAndAssert(process, 1, 0, 0, 0, 1, 0, 1);
            FlowTestHelper.TickAndAssert(process, 1, 0, 1, 0, 0, 0, 2);
            FlowTestHelper.TickAndAssert(process, 0, 0, 1, 0, 1, 0, 2);
            FlowTestHelper.TickAndAssert(process, 1, 0, 0, 0, 1, 0, 3);
        }

        [TestMethod]
        public void ShouldPreventExcessWipWhenFirstStationIsFaster()
        {
            ConwipWorkProcess process = new ConwipWorkProcess(new InfiniteBacklog(),4);
            
            WorkInProcessLimit wipLimit = new WorkInProcessLimit(4);
            WorkStation ws1 = new WorkStation(wipLimit);
            WorkStation ws2 = new WorkStation(wipLimit);

            process.Add(ws1);
            process.Add(ws2);

            IProbabilityDistribution distribution1 = new DeterministicDistribution(1);
            IProbabilityDistribution distribution2 = new DeterministicDistribution(2);

            ws1.AddMachine(new Machine(distribution1));
            ws2.AddMachine(new Machine(distribution2));

            FlowTestHelper.TickAndAssert(process, 1, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(process, 1, 0, 1, 0, 0);
            FlowTestHelper.TickAndAssert(process, 1, 1, 1, 0, 0);
            FlowTestHelper.TickAndAssert(process, 1, 1, 1, 0, 1);
            FlowTestHelper.TickAndAssert(process, 1, 2, 1, 0, 1);
            FlowTestHelper.TickAndAssert(process, 1, 2, 1, 0, 2);
            FlowTestHelper.TickAndAssert(process, 0, 3, 1, 0, 2);
            FlowTestHelper.TickAndAssert(process, 1, 2, 1, 0, 3);
            FlowTestHelper.TickAndAssert(process, 0, 3, 1, 0, 3);

            List<WorkItem> completed = new List<WorkItem>(process.CompletedJobs);
            Assert.AreEqual(3,completed[0].CycleTime);
            Assert.AreEqual(4,completed[1].CycleTime);
            Assert.AreEqual(5,completed[2].CycleTime);

        }

        [TestMethod]
        public void ShouldStopProcessingWhenAWorkStationIsStopped()
        {
            ConwipWorkProcess process = new ConwipWorkProcess(new InfiniteBacklog(),4);

            WorkInProcessLimit wipLimit = new WorkInProcessLimit(4);

            WorkStation ws1 = new WorkStation(wipLimit);
            WorkStation ws2 = new WorkStation(wipLimit);
            WorkStation ws3 = new WorkStation(wipLimit);

            process.Add(ws1);
            process.Add(ws2);
            process.Add(ws3);

            IProbabilityDistribution distribution1 = new DeterministicDistribution(1);
            IProbabilityDistribution distribution2 = new DeterministicDistribution(2);

            ws1.AddMachine(new Machine(distribution1));
            ws2.AddMachine(new Machine(distribution2));
            ws2.AddMachine(new Machine(distribution2));
            ws3.AddMachine(new Machine(distribution1));

            process.Tick(7);
            FlowTestHelper.TickAndAssert(process,1,0,2,0,1,0,4);

            ws3.Stop();
            process.Tick(4);
            FlowTestHelper.TickAndAssert(process, 0, 0, 0, 4, 0, 0, 5);

            ws3.Start();
            process.Tick(4);

            FlowTestHelper.TickAndAssert(process, 1, 0, 2, 0, 1, 0, 9);
        }
    }
}