using System.Collections.Generic;
using MonteCarloFlowTest;

namespace MonteCarloFlow
{
    public interface IWorkProcess
    {
        void Tick(long ticks);

        WorkStation this[int index] { get; }

        int CompletionQueueCount { get; }

        IEnumerable<WorkItem> CompletedJobs { get; }
        void Add(WorkStation workStation);
        IEnumerable<WorkStation> WorkStations { get; }
    }
}