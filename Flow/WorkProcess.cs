using System.Collections.Generic;
using MonteCarloFlowTest;
using Wintellect.PowerCollections;

namespace Flow
{
    public class WipEntry
    {
        public WipEntry(int timeStamp, int wip)
        {
            TimeStamp = timeStamp;
            Wip = wip;
        }

        public int TimeStamp
        {
            get;
            private set;
        }

        public int Wip
        {
            get;
            private set;
        }
    }

    public class UtilizationEntry
    {
        public UtilizationEntry(int timeStamp, string resourcePoolName, double utilization)
        {
            TimeStamp = timeStamp;
            ResourcePoolName = resourcePoolName;
            Utilization = utilization;
        }

        public int TimeStamp
        {
            get;
            private set;
        }

        public string ResourcePoolName
        {
            get;
            private set;
        }

        public double Utilization
        {
            get;
            private set;
        }
    }

    public class WorkProcess 
    {
        private readonly List<IWorkStation> _allWorkstations = new List<IWorkStation>();
        private readonly DoneQueue _doneQueue = new DoneQueue();
        private readonly List<WorkStation> _addedWorkstations = new List<WorkStation>();
        private readonly List<WipEntry> _wipEntries = new List<WipEntry>();
        private readonly List<UtilizationEntry> _utilizationEntries = new List<UtilizationEntry>();
        private readonly Set<ResourcePool> _resourcePools = new Set<ResourcePool>();
        private int _timeStamp;

        public WorkProcess(IWorkStation backlog)
        {
            _allWorkstations.Add(backlog);
            _allWorkstations.Add(_doneQueue);
        }

        public WorkProcess(IWorkStation backlog, IList<WorkStation> workStations)
        {
            _allWorkstations.Add(backlog);
            _allWorkstations.Add(_doneQueue);

            foreach (WorkStation workStation in workStations)
            {
                Add(workStation);
            }
        }

        public void Add(WorkStation workStation)
        {
            _addedWorkstations.Add(workStation);
            _allWorkstations.Insert(_allWorkstations.Count - 1, workStation);
            _resourcePools.AddMany(workStation.ResourcePools);
        }

        public WorkStation this[int index]
        {
            get { return _addedWorkstations[index]; }
        }

        public IEnumerable<WorkStation> WorkStations
        {
            get
            {
                return _addedWorkstations;
            }
        }


        public void Tick(long ticks)
        {
            while (ticks-- > 0)
            {
                _timeStamp++;
                TickWorkStations();

                MoveWorkItems();

                RecordWorkInProcess();

                RecordUtilization();
            }
        }

        private void MoveWorkItems()
        {
            for (int index = _allWorkstations.Count - 1; index > 0; index--)
            {
                while (_allWorkstations[index - 1].HasFinishedJobs)
                {
                    IWorkItemTransition workItemTransition = _allWorkstations[index - 1].BeginWorkItemTransition();
                    if(!_allWorkstations[index].TryAddWorkItem(workItemTransition))
                    {
                        break;
                    }
                }
            }
        }

        private void RecordUtilization()
        {
            foreach (ResourcePool resourcePool in _resourcePools)
            {
                _utilizationEntries.Add(new UtilizationEntry(_timeStamp, resourcePool.Name, resourcePool.Utilization));
            }
        }

        private void RecordWorkInProcess()
        {
            int wip = 0;
            foreach (IWorkStation station in _allWorkstations)
            {
                wip += station.WorkInProcess;
            }
            _wipEntries.Add(new WipEntry(_timeStamp, wip));
        }

        private void TickWorkStations()
        {
            foreach (IWorkStation station in _allWorkstations)
            {
                station.Tick();
            }
        }

        public int CompletionQueueCount
        {
            get { return _doneQueue.Count; }
        }

        public IEnumerable<WorkItem> CompletedWorktems
        {
            get { return _doneQueue; }
        }

        public List<WipEntry> WipEntries
        {
            get
            {
                return _wipEntries;
            }
        }
        
        public List<UtilizationEntry> UtilizationEntries
        {
            get { return _utilizationEntries; }
        }
    }
}
