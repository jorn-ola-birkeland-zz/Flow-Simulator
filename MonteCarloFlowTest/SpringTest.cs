using Flow;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spring.Context;
using Spring.Context.Support;
using Flow.ProbabilityDistribution;

namespace MonteCarloFlowTest
{
    [TestClass]
    public class SpringTest
    {
        private const string _config = @"d:\Documents and Settings\Jørn Ola Birkeland\Flow-Simulator\MonteCarloFlowTest\example.xml";

        [TestMethod]
        public void ShouldGetMachine1Distribution()
        {

            IApplicationContext context = new XmlApplicationContext(_config);
            IProbabilityDistribution dist = (IProbabilityDistribution)context.GetObject("m1dist");
            Assert.AreNotEqual(0,dist.NextValue());
        }

        [TestMethod]
        public void ShouldGetBacklogDistribution()
        {

            IApplicationContext context = new XmlApplicationContext(_config);

            IProbabilityDistribution backlogdist = (IProbabilityDistribution)context.GetObject("backlogdist");
            Assert.AreEqual(2, backlogdist.NextValue());
        }

        [TestMethod]
        public void ShouldGetBacklog()
        {
            IApplicationContext context = new XmlApplicationContext(_config);

            IWorkStation backlog = (IWorkStation)context.GetObject("backlog");
            IWorkItemTransition trans = backlog.BeginWorkItemTransition();
            WorkItem item = trans.Commit();

            Assert.AreEqual(2, item.Size);
        }

        [TestMethod]
        public void ShouldGetWorkStation()
        {
            IApplicationContext context = new XmlApplicationContext(_config);

            IWorkStation ws = (IWorkStation)context.GetObject("ws1");

            Assert.IsNotNull(ws);
        }

        [TestMethod]
        public void ShouldGetResourcePool()
        {
            IApplicationContext context = new XmlApplicationContext(_config);

            ResourcePool resourcePool = (ResourcePool)context.GetObject("rp1");

            Assert.AreEqual(1,resourcePool.ResourceCount);
        }

        [TestMethod]
        public void ShouldGetMachine()
        {
            IApplicationContext context = new XmlApplicationContext(_config);

            Machine machine = (Machine)context.GetObject("machine1");

            Assert.IsNotNull(machine);
        }

        [TestMethod]
        public void ShouldGetWorkProcess()
        {
            IApplicationContext context = new XmlApplicationContext(_config);

            WorkProcess backlog = (WorkProcess)context.GetObject("process");

            Assert.IsNotNull(backlog);
        }


    }
}
