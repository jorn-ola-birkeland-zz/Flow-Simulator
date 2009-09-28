namespace MonteCarloFlowTest
{
    public interface IWorkStation
    {
        void Tick();

        bool HasCapacity { get; }

        bool HasFinishedJobs { get; }

        int WorkInProcess { get; }

        void AddJob(WorkItem workItem);
        WorkItem RemoveFirstFinishedJob();
    }
}