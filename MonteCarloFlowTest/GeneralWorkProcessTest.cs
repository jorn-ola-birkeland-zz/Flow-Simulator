using System.Collections.Generic;
using Flow;
using Flow.ProbabilityDistribution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MonteCarloFlowTest
{
    [TestClass]
    public class GeneralWorkProcessTest
    {
        [TestMethod]
        public void ShouldRunSingleStationSingleDeterministicMachine()
        {
            WorkProcess process = new WorkProcess(new InfiniteBacklog());
            WorkStation ws1 = new WorkStation(new WipTokenPool(4));

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
        public void ShouldForKanbanRunTwoStationSingleDeterministicMachineWorkProcess()
        {
            WorkProcess process = new WorkProcess(new InfiniteBacklog());
            WorkStation ws1 = new WorkStation(new WipTokenPool(4));
            WorkStation ws2 = new WorkStation(new WipTokenPool(2)); // Note: Bug in SetWorkstationWipLimit below?

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
        public void ShouldForKanbanRunTwoStationDeterministicMachineWorkProcessAndLastWorkStationWithTwoMachines()
        {
            WorkProcess process = new WorkProcess(new InfiniteBacklog());
            WorkStation ws1 = new WorkStation(new WipTokenPool(8));
            WorkStation ws2 = new WorkStation(new WipTokenPool(8));

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
        public void ShouldForKanbanPreventExcessWipWhenFirstStationIsFaster()
        {
            WorkProcess process = new WorkProcess(new InfiniteBacklog());
            WorkStation ws1 = new WorkStation(new WipTokenPool(3));
            WorkStation ws2 = new WorkStation(new WipTokenPool(2));

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
            FlowTestHelper.TickAndAssert(process, 1, 2, 1, 0, 4);
            FlowTestHelper.TickAndAssert(process, 0, 3, 1, 0, 4);
            FlowTestHelper.TickAndAssert(process, 1, 2, 1, 0, 5);

            List<WorkItem> completed = new List<WorkItem>(process.CompletedWorktems);
            Assert.AreEqual(3, completed[0].CycleTime);
            Assert.AreEqual(4, completed[1].CycleTime);
            Assert.AreEqual(5, completed[2].CycleTime);
            Assert.AreEqual(6, completed[3].CycleTime);
            Assert.AreEqual(6, completed[3].CycleTime);

        }

        [TestMethod]
        public void ShouldForKanbanRunThreeWorkstationProcess()
        {
            WorkProcess process = new WorkProcess(new RandomSizeInfiniteBacklog(new DeterministicDistribution(2)));
            WorkStation ws1 = new WorkStation(new WipTokenPool(1));
            WorkStation ws2 = new WorkStation(new WipTokenPool(3));
            WorkStation ws3 = new WorkStation(new WipTokenPool(2)); // Note: Bug in SetWorkstationWipLimit below?

            process.Add(ws1);
            process.Add(ws2);
            process.Add(ws3);

            IProbabilityDistribution distribution1 = new DeterministicDistribution(5);
            IProbabilityDistribution distribution2 = new DeterministicDistribution(30);
            IProbabilityDistribution distribution3 = new DeterministicDistribution(3);


            process[0].AddMachine(new Machine(distribution1));
            process[1].AddMachine(new Machine(distribution2));
            process[1].AddMachine(new Machine(distribution2));
            process[1].AddMachine(new Machine(distribution2));
            process[2].AddMachine(new Machine(distribution3));

            FlowTestHelper.TickAndAssert(process, 1, 0, 0, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(10, process, 1, 0, 1, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(10, process, 1, 0, 2, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(10, process, 1, 0, 3, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(10, process, 0, 1, 3, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(10, process, 0, 1, 3, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(10, process, 0, 1, 3, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(10, process, 1, 0, 3, 0, 1, 0, 0);
            FlowTestHelper.TickAndAssert(6, process, 1, 0, 3, 0, 0, 0, 1);
            FlowTestHelper.TickAndAssert(4, process, 1, 0, 3, 0, 1, 0, 1);
            //FlowTestHelper.TickAndAssert(10, process, 0, 1, 3, 0, 0, 0, 0);

            //List<WorkItem> completed = new List<WorkItem>(process.CompletedWorktems);
            //Assert.AreEqual(3, completed[0].CycleTime);
            //Assert.AreEqual(4, completed[1].CycleTime);
            //Assert.AreEqual(5, completed[2].CycleTime);
            //Assert.AreEqual(6, completed[3].CycleTime);
            //Assert.AreEqual(6, completed[3].CycleTime);

        }

        [TestMethod]
        public void ShouldForConwipRunTwoStationSingleDeterministicMachineWorkProcess()
        {
            WorkProcess process = new WorkProcess(new InfiniteBacklog());

            WipTokenPool wipLimit = new WipTokenPool(8);

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
        public void ShouldForConwipRunTwoStationDeterministicMachineWorkProcessAndLastWorkStationWithTwoMachines()
        {
            WorkProcess process = new WorkProcess(new InfiniteBacklog());

            WipTokenPool wipLimit = new WipTokenPool(8);

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
        public void ShouldForConwipRunPooledMachines()
        {
            WorkProcess process = new WorkProcess(new InfiniteBacklog());

            WipTokenPool wipLimit = new WipTokenPool(8);

            WorkStation ws1 = new WorkStation(wipLimit);
            WorkStation ws2 = new WorkStation(wipLimit);

            process.Add(ws1);
            process.Add(ws2);

            IProbabilityDistribution distribution1 = new DeterministicDistribution(4);
            IProbabilityDistribution distribution2 = new DeterministicDistribution(4);

            ResourcePool pool = new ResourcePool(3);

            ws1.AddMachine(new Machine(distribution1, pool));
            ws1.AddMachine(new Machine(distribution1, pool));
            ws1.AddMachine(new Machine(distribution1, pool));

            ws2.AddMachine(new Machine(distribution2, pool));
            ws2.AddMachine(new Machine(distribution2, pool));
            ws2.AddMachine(new Machine(distribution2, pool));

            FlowTestHelper.TickAndAssert(process, 3, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(process, 3, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(process, 3, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(process, 3, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(process, 0, 0, 3, 0, 0);
            FlowTestHelper.TickAndAssert(process, 0, 0, 3, 0, 0);
            FlowTestHelper.TickAndAssert(process, 0, 0, 3, 0, 0);
            FlowTestHelper.TickAndAssert(process, 0, 0, 3, 0, 0);
            FlowTestHelper.TickAndAssert(process, 3, 0, 0, 0, 3);
        }

        [TestMethod]
        public void ShouldForConwipHandleProcessingTimeOfZeroForPooledMachine()
        {
            WorkProcess process = new WorkProcess(new InfiniteBacklog());

            WipTokenPool wipLimit = new WipTokenPool(5);

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

            ws1.AddMachine(new Machine(distribution1, pool));
            ws1.AddMachine(new Machine(distribution1, pool));
            ws1.AddMachine(new Machine(distribution1, pool));
            ws1.AddMachine(new Machine(distribution1, pool));
            ws1.AddMachine(new Machine(distribution1, pool));

            ws2.AddMachine(new Machine(distribution2, pool));
            ws2.AddMachine(new Machine(distribution2, pool));
            ws2.AddMachine(new Machine(distribution2, pool));
            ws2.AddMachine(new Machine(distribution3, pool));
            ws2.AddMachine(new Machine(distribution3, pool));

            ws3.AddMachine(new Machine(distribution4, pool));
            ws3.AddMachine(new Machine(distribution4, pool));
            ws3.AddMachine(new Machine(distribution4, pool));
            ws3.AddMachine(new Machine(distribution4, pool));
            ws3.AddMachine(new Machine(distribution4, pool));

            FlowTestHelper.TickAndAssert(process, 2, 0, 0, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(process, 0, 0, 2, 0, 0, 0, 0);
            FlowTestHelper.TickAndAssert(process, 0, 0, 0, 0, 2, 0, 0);
            FlowTestHelper.TickAndAssert(process, 2, 0, 0, 0, 0, 0, 2);
            FlowTestHelper.TickAndAssert(process, 0, 0, 2, 0, 0, 0, 2);
            FlowTestHelper.TickAndAssert(process, 0, 0, 0, 0, 2, 0, 2);
            FlowTestHelper.TickAndAssert(process, 2, 0, 0, 0, 0, 0, 4);
        }

        [TestMethod]
        public void ShouldForConwipPreventExcessWipWhenFirstStationIsFaster()
        {
            WorkProcess process = new WorkProcess(new InfiniteBacklog());

            WipTokenPool wipLimit = new WipTokenPool(4);
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

            List<WorkItem> completed = new List<WorkItem>(process.CompletedWorktems);
            Assert.AreEqual(3, completed[0].CycleTime);
            Assert.AreEqual(4, completed[1].CycleTime);
            Assert.AreEqual(5, completed[2].CycleTime);

        }
    }
}
