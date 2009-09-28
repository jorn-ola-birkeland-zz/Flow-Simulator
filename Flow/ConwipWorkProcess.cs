using System.Collections.Generic;
using MonteCarloFlowTest;

namespace MonteCarloFlow
{
    public class ConwipWorkProcess : WorkProcess, IWorkProcess
    {
        private readonly List<IWorkStation> _workStations = new List<IWorkStation>();
        private readonly DoneQueue _doneQueue = new DoneQueue();
        private readonly IWorkStation _backlog;
        private readonly int _wipLimit;

        public ConwipWorkProcess(IWorkStation backlog, int wipLimit)
        {
            _backlog = backlog;
            _wipLimit = wipLimit;
            _workStations.Add(_doneQueue);
        }

        public IEnumerable<WorkItem> CompletedJobs
        {
            get { return _doneQueue; }
        }

        private int WorkInProcess
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
                _backlog.Tick();
                foreach (IWorkStation station in _workStations)
                {
                    station.Tick();
                }

                for (int index = _workStations.Count - 1; index > 0; index--)
                {
                    while (_workStations[index].HasCapacity & _workStations[index - 1].HasFinishedJobs)
                    {
                        _workStations[index].AddJob(_workStations[index - 1].RemoveFirstFinishedJob());
                    }
                }

                if (WorkInProcess < _wipLimit && _workStations[0].HasCapacity && _backlog.HasFinishedJobs)
                {
                    _workStations[0].AddJob(_backlog.RemoveFirstFinishedJob());
                }
            }
        }
    
        protected override void OnAdd(WorkStation station)
        {
            _workStations.Insert(_workStations.Count - 1, station);
        }

    }
}
