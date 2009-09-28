namespace MonteCarloFlowTest
{
    public interface IMachine
    {
        bool IsProcessing { get; }
        bool HasCompletedJob { get; }
        bool IsIdle { get; }

        double Utilization { get; }

        void Tick();
        WorkItem RemoveCompletedJob();
        void StartJob(WorkItem workItem);
        void Stop();
        void Start();
    }
}