using System.Collections.Generic;
using Flow;
using MonteCarloFlowTest;
using Wintellect.PowerCollections;

namespace Flow
{
    public interface IWorkStation
    {
        void Tick();

        bool HasFinishedJobs { get; }

        int WorkInProcess { get; }
        Set<ResourcePool> ResourcePools { get;}

        IWorkItemTransition BeginWorkItemTransition();
        bool TryAddWorkItem(IWorkItemTransition workItemTransition);
    }
}