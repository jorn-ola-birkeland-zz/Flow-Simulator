using System.Collections;
using System.Collections.Generic;

namespace MonteCarloFlowTest
{
    internal class DoneQueue : IWorkStation, IEnumerable<WorkItem>
    {
        private readonly Queue<WorkItem> _jobs = new Queue<WorkItem>();

        public void Tick()
        {
        }

        public bool HasCapacity
        {
            get { return true; }
        }

        public bool HasFinishedJobs
        {
            get { return _jobs.Count>0; }
        }

        public int WorkInProcess
        {
            get { return 0; }
        }

        public int Count
        {
            get { return _jobs.Count; }
        }

        public WorkItem RemoveFirstFinishedJob()
        {
            return _jobs.Dequeue();
        }

        public void AddJob(WorkItem workItem)
        {
            _jobs.Enqueue(workItem);
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