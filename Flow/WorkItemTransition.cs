namespace MonteCarloFlowTest
{
    public class WorkItemTransition : IWorkItemTransition
    {
        private readonly WorkItem _workItem;


        public WorkItemTransition(WorkItem workItem)
        {
            _workItem = workItem;
        }

        public WorkItem Commit()
        {
            return _workItem;
        }

        public void Rollback()
        {
        }
    }
}