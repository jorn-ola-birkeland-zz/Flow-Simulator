namespace MonteCarloFlowTest
{
    public class InfiniteBacklog : IWorkStation
    {
        private long _time;

        public void Tick()
        {
            _time++;
        }

        public bool HasCapacity
        {
            get { return false; }
        }

        public bool HasFinishedJobs
        {
            get { return true; }
        }

        public int WorkInProcess
        {
            get { return 0; }
        }

        public void AddJob(WorkItem o)
        {
        }

        public WorkItem RemoveFirstFinishedJob()
        {
            WorkItem workItem = new WorkItem(_time);

            PrepareJob(workItem);
            return workItem;
        }

        protected virtual void PrepareJob(WorkItem workItem)
        {
            
        }
    }
}