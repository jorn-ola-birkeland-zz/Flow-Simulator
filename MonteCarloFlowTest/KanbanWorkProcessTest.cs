using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MonteCarloFlow;

namespace MonteCarloFlowTest
{
    [TestClass]
    public class KanbanWorkProcessTest
    {
        [TestMethod]
        public void ShouldRunSingleStationSingleDeterministicMachine()
        {
            KanbanWorkProcess process = new KanbanWorkProcess(new InfiniteBacklog(),4);
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
            KanbanWorkProcess process = new KanbanWorkProcess(new InfiniteBacklog(), 2);
            WorkStation ws1 = new WorkStation(new WorkInProcessLimit(4));
            WorkStation ws2 = new WorkStation(new WorkInProcessLimit(2)); // Note: Bug in SetWorkstationWipLimit below?

            process.Add(ws1);
            process.Add(ws2);

            process.SetWorkstationWipLevel(0,4);
            process.SetWorkstationWipLevel(0,4);

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
            KanbanWorkProcess process = new KanbanWorkProcess(new InfiniteBacklog(), 8);
            WorkStation ws1 = new WorkStation(new WorkInProcessLimit(8));
            WorkStation ws2 = new WorkStation(new WorkInProcessLimit(8));

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
        public void ShouldPreventExcessWipWhenFirstStationIsFaster()
        {
            KanbanWorkProcess process = new KanbanWorkProcess(new InfiniteBacklog(), 2);
            WorkStation ws1 = new WorkStation(new WorkInProcessLimit(3));
            WorkStation ws2 = new WorkStation(new WorkInProcessLimit(2));

            process.Add(ws1);
            process.Add(ws2);

            process.SetWorkstationWipLevel(0,3);
            process.SetWorkstationWipLevel(1,2);

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
            FlowTestHelper.TickAndAssert(process, 1, 2, 1, 0, 4);
            FlowTestHelper.TickAndAssert(process, 0, 3, 1, 0, 4);
            FlowTestHelper.TickAndAssert(process, 1, 2, 1, 0, 5);

            List<WorkItem> completed = new List<WorkItem>(process.CompletedJobs);
            Assert.AreEqual(3, completed[0].CycleTime);
            Assert.AreEqual(4, completed[1].CycleTime);
            Assert.AreEqual(5, completed[2].CycleTime);
            Assert.AreEqual(6, completed[3].CycleTime);
            Assert.AreEqual(6, completed[3].CycleTime);

        }

        [TestMethod]
        public void Test()
        {
            KanbanWorkProcess process = new KanbanWorkProcess(new RandomSizeInfiniteBacklog(new DeterministicDistribution(2)), 2);
            WorkStation ws1 = new WorkStation(new WorkInProcessLimit(1));
            WorkStation ws2 = new WorkStation(new WorkInProcessLimit(3));
            WorkStation ws3 = new WorkStation(new WorkInProcessLimit(2)); // Note: Bug in SetWorkstationWipLimit below?

            process.Add(ws1);
            process.Add(ws2);
            process.Add(ws3);

            process.SetWorkstationWipLevel(0, 1);
            process.SetWorkstationWipLevel(1, 3);
            process.SetWorkstationWipLevel(1, 3);

            IProbabilityDistribution distribution1 = new DeterministicDistribution(5);
            IProbabilityDistribution distribution2 = new DeterministicDistribution(30);
            IProbabilityDistribution distribution3 = new DeterministicDistribution(3);


            process[0].AddMachine(new Machine(distribution1));
            process[1].AddMachine(new Machine(distribution2));
            process[1].AddMachine(new Machine(distribution2));
            process[1].AddMachine(new Machine(distribution2));
            process[2].AddMachine(new Machine(distribution3));

            FlowTestHelper.TickAndAssert(process, 1, 0, 0, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(10,process, 1, 0, 1, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(10, process, 1, 0, 2, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(10, process, 1, 0, 3, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(10, process, 0, 1, 3, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(10, process, 0, 1, 3, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(10, process, 0, 1, 3, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(10, process, 1, 0, 3, 0, 1, 0, 0);
            FlowTestHelper.TickAndAssert(6, process, 1, 0, 3, 0, 0, 0, 1);
            FlowTestHelper.TickAndAssert(4, process, 1, 0, 3, 0, 1, 0, 1);
            //FlowTestHelper.TickAndAssert(10, process, 0, 1, 3, 0, 0, 0, 0);

            //List<WorkItem> completed = new List<WorkItem>(process.CompletedJobs);
            //Assert.AreEqual(3, completed[0].CycleTime);
            //Assert.AreEqual(4, completed[1].CycleTime);
            //Assert.AreEqual(5, completed[2].CycleTime);
            //Assert.AreEqual(6, completed[3].CycleTime);
            //Assert.AreEqual(6, completed[3].CycleTime);
            
        }
    }
}