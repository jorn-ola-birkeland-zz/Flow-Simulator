using System.Collections.Generic;
using MonteCarloFlowTest;

namespace MonteCarloFlow
{
    public class KanbanWorkProcess : WorkProcess, IWorkProcess
    {        
        private readonly List<IWorkStation> _workStations = new List<IWorkStation>();
        private readonly DoneQueue _doneQueue = new DoneQueue();
        private readonly List<int> _wipLimits = new List<int>();
        private readonly int _defaultWipLevel;

        public KanbanWorkProcess(IWorkStation backlog, int defaultWipLevel)
        {
            _workStations.Add(backlog);
            _workStations.Add(_doneQueue);
            _wipLimits.Add(int.MaxValue);
            _wipLimits.Add(int.MaxValue);
            _defaultWipLevel = defaultWipLevel;
        }

        public IEnumerable<WorkItem> CompletedJobs
        {
            get { return _doneQueue; }
        }

        public int WorkInProcess
        {
            get
            {
                int wip = 0;
                foreach (IWorkStation station in _workStations)
                {
                    wip += station.WorkInProcess;
                }
                return wip;
            }
        }

        public int CompletionQueueCount
        {
            get { return _doneQueue.Count; }
        }

        public void Tick(long ticks)
        {
            while (ticks-- > 0)
            {
                foreach (IWorkStation station in _workStations)
                {
                    station.Tick();
                }

                for (int index = _workStations.Count - 1; index > 0; index--)
                {
                    while (_wipLimits[index]>_workStations[index].WorkInProcess && _workStations[index].HasCapacity & _workStations[index - 1].HasFinishedJobs)
                    {
                        _workStations[index].AddJob(_workStations[index - 1].RemoveFirstFinishedJob());
                    }
                }
            }
        }
    
        protected override void OnAdd(WorkStation workStation)
        {
            _workStations.Insert(_workStations.Count-1,workStation);
            _wipLimits.Insert(_wipLimits.Count - 1, _defaultWipLevel);
        }

        public void SetWorkstationWipLevels(int[] wipLevels)
        {
            for(int i = 0; i<wipLevels.Length;i++)
            {
                SetWorkstationWipLevel(i,wipLevels[i]);
            }

           
        }


        public void SetWorkstationWipLevel(int index, int wipLevel)
        {
            _wipLimits[index + 1] = wipLevel;
        }

        public int GetWorkstationWipLevel(int index)
        {
            return _wipLimits[index + 1];
        }

    }
}
