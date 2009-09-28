using System.Collections.Generic;
using MonteCarloFlowTest;

namespace MonteCarloFlow
{
    public abstract class WorkProcess
    {
        private readonly List<WorkStation> _workstations = new List<WorkStation>();

        public void Add(WorkStation workStation)
        {
            _workstations.Add(workStation);

            OnAdd(workStation);
        }

        public WorkStation this[int index]
        {
            get { return _workstations[index]; }
        }

        public IEnumerable<WorkStation> WorkStations
        {
            get
            {
                return _workstations;
            }
        }

        protected abstract void OnAdd(WorkStation station);
    }
}