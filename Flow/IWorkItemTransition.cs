namespace MonteCarloFlowTest
{
    public interface IWorkItemTransition
    {
        WorkItem Commit();
        void Rollback();
    }
}