using Flow;
using Wintellect.PowerCollections;

namespace MonteCarloFlowTest
{
    public class InfiniteBacklog : IWorkStation
    {
        private long _time;

        public void Tick()
        {
            _time++;
        }

        public bool HasFinishedJobs
        {
            get { return true; }
        }

        public int WorkInProcess
        {
            get { return 0; }
        }

        public Set<ResourcePool> ResourcePools
        {
            get { return new Set<ResourcePool>(); }
        }

        private WorkItem RemoveFirstFinishedJob()
        {
            WorkItem workItem = new WorkItem(_time);

            PrepareJob(workItem);
            return workItem;
        }

        public IWorkItemTransition BeginWorkItemTransition()
        {
            return new WorkItemTransition(RemoveFirstFinishedJob());
        }

        public bool TryAddWorkItem(IWorkItemTransition workItemTransition)
        {
            return false;
        }

        protected virtual void PrepareJob(WorkItem workItem)
        {
            
        }
    }
}