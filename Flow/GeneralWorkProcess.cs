using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonteCarloFlow;
using MonteCarloFlowTest;

namespace Flow
{
    public class GeneralWorkProcess : WorkProcess, IWorkProcess
    {
        private readonly List<IWorkStation> _workStations = new List<IWorkStation>();
        private readonly DoneQueue _doneQueue = new DoneQueue();

        public GeneralWorkProcess(IWorkStation backlog)
        {
            _workStations.Add(backlog);
            _workStations.Add(_doneQueue);
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
                    while (_workStations[index - 1].HasFinishedJobs)
                    {
                        IWorkItemTransition workItemTransition = _workStations[index - 1].BeginWorkItemTransition();
                        if(!_workStations[index].TryAddWorkItem(workItemTransition))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public int CompletionQueueCount
        {
            get { return _doneQueue.Count; }
        }

        public IEnumerable<WorkItem> CompletedJobs
        {
            get { return _doneQueue; }
        }


        protected override void OnAdd(WorkStation station)
        {
            _workStations.Insert(_workStations.Count - 1, station);
        }
    }
}
