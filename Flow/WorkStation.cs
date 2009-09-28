using System;
using System.Collections.Generic;

namespace MonteCarloFlowTest
{
    public class WorkInProcessLimit
    {
        private readonly int _wipLimit;


        public WorkInProcessLimit(int wipLimit)
        {
            _wipLimit = wipLimit;
        }

        public int WipLimitValue
        {
            get { return _wipLimit;  }
        }
    }

    public class WorkStation : IWorkStation
    {
        private readonly List<IMachine> _machines = new List<IMachine>();
        private readonly Queue<WorkItem> _completedJobs = new Queue<WorkItem>();
        private readonly WorkInProcessLimit _wipLimit;


        public WorkStation(WorkInProcessLimit wipLimit)
        {
            _wipLimit = wipLimit;
        }

        public int WorkInProcess
        {
            get
            {
                int wip = _completedJobs.Count;
                foreach (IMachine machine in _machines)
                {
                    if (machine.IsProcessing)
                    {
                        wip++;
                    }
                }
                return wip;
            }
        }

        public bool HasCapacity
        {
            get { return ExistIdleMachine; }
        }

        public bool HasFinishedJobs
        {
            get { return _completedJobs.Count > 0; }
        }

        public WorkItem RemoveFirstFinishedJob()
        {
            return _completedJobs.Dequeue();
        }

        public int InProcessCount
        {
            get
            {
                int wip = 0;
                foreach (IMachine machine in _machines)
                {
                    if (machine.IsProcessing)
                    {
                        wip++;
                    }
                }
                return wip;
            }
        }

        public int CompletionQueueCount
        {
            get
            {
                return _completedJobs.Count;
            }
        }

        public void AddMachine(IMachine machine)
        {
            _machines.Add(machine);
        }

        public void Tick()
        {
            foreach (WorkItem job in _completedJobs)
            {
                job.Tick();
            }

            foreach (IMachine machine in _machines)
            {
                machine.Tick();
                if (machine.HasCompletedJob)
                {
                    _completedJobs.Enqueue(machine.RemoveCompletedJob());
                }
            }

        }

        public void AddJob(WorkItem workItem)
        {
            foreach (IMachine machine in _machines)
            {
                if (machine.IsIdle)
                {
                    machine.StartJob(workItem);
                    return;
                }
            }

            throw new InvalidOperationException("Trying to add workItem when there are no idle machines");
        }

        private bool ExistIdleMachine
        {
            get
            {
                foreach (IMachine machine in _machines)
                {
                    if (machine.IsIdle)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public Queue<WorkItem> CompletionQueue
        {
            get { return _completedJobs; }
        }

        public void Stop()
        {
            foreach (IMachine machine in _machines)
            {
                machine.Stop();
            }
        }

        public void Start()
        {
            foreach (IMachine machine in _machines)
            {
                machine.Start();
            }

        }

        public double AverageUtilization
        {
            get
            {
                double sum = 0;

                foreach (IMachine machine in _machines)
                {
                    sum += machine.Utilization;
                }

                return sum/_machines.Count;
            }
        }

        public IEnumerable<IMachine> Machines
        {
            get
            {
                return _machines;
            }
        }
    }
}