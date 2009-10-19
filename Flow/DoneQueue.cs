using System.Collections;
using System.Collections.Generic;
using Flow;
using Wintellect.PowerCollections;

namespace MonteCarloFlowTest
{
    internal class DoneQueue : IWorkStation, IEnumerable<WorkItem>
    {
        private readonly Queue<WorkItem> _jobs = new Queue<WorkItem>();

        public void Tick()
        {
        }

        public bool HasFinishedJobs
        {
            get { return _jobs.Count>0; }
        }

        public int WorkInProcess
        {
            get { return 0; }
        }

        public Set<ResourcePool> ResourcePools
        {
            get { return new Set<ResourcePool>(); }
        }

        public int Count
        {
            get { return _jobs.Count; }
        }

        public IWorkItemTransition BeginWorkItemTransition()
        {
            return new WorkItemTransition(_jobs.Dequeue());
        }

        public bool TryAddWorkItem(IWorkItemTransition workItemTransition)
        {
            _jobs.Enqueue(workItemTransition.Commit());

            return true;
        }

        IEnumerator<WorkItem> IEnumerable<WorkItem>.GetEnumerator()
        {
            return _jobs.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<WorkItem>) this).GetEnumerator();
        }
    }
}