using System;
using System.Collections.Generic;
using Flow;
using MonteCarloFlowTest;
using Wintellect.PowerCollections;

namespace Flow
{
    internal interface IWorkItemStack
    {
        WorkItem Pop();
        void Push(WorkItem workItem);
    }

    public class WorkStation : IWorkStation, IWorkItemStack
    {
        private class WorkItemTransition : IWorkItemTransition
        {
            private readonly IWorkItemStack _workItemStack;
            private readonly WorkItem _workItem;

            public WorkItemTransition(IWorkItemStack workItemStack)
            {
                _workItemStack = workItemStack;
                _workItem = _workItemStack.Pop();
            }

            public WorkItem Commit()
            {
                return _workItem;
            }

            public void Rollback()
            {
                _workItemStack.Push(_workItem);
            }
        }


        private readonly IList<Machine> _machines;
        private readonly IList<WorkItem> _completedJobs = new List<WorkItem>();
        private readonly WipTokenPool _wipLimit;

        public WorkStation(WipTokenPool wipLimit) : this(wipLimit, new List<Machine>())
        {
            _wipLimit = wipLimit;
        }

        public WorkStation(WipTokenPool wipLimit, IList<Machine> machines)
        {
            _wipLimit = wipLimit;
            _machines = machines;
        }

        public int WorkInProcess
        {
            get
            {
                int wip = _completedJobs.Count;
                foreach (Machine machine in _machines)
                {
                    if (machine.IsProcessing)
                    {
                        wip++;
                    }
                }
                return wip;
            }
        }


        public bool HasFinishedJobs
        {
            get { return _completedJobs.Count > 0; }
        }

        public IWorkItemTransition BeginWorkItemTransition()
        {
            return new WorkItemTransition(this);
        }

        public bool TryAddWorkItem(IWorkItemTransition workItemTransition)
        {
            if (ExistIdleMachine && _wipLimit.LockWipToken())
            {
                WorkItem workItem = workItemTransition.Commit();
                AddJob(workItem);
                return true;
            }
            else
            {
                workItemTransition.Rollback();
                return false;
            }
        }

        WorkItem IWorkItemStack.Pop()
        {
            return RemoveFirstFinishedJob();
        }

        void IWorkItemStack.Push(WorkItem workItem)
        {
            _wipLimit.LockWipToken();
            _completedJobs.Insert(0, workItem);
        }

        private WorkItem RemoveFirstFinishedJob()
        {
            _wipLimit.UnlockWipToken();
            WorkItem wi = _completedJobs[0];
            _completedJobs.RemoveAt(0);
            return wi;
        }

        public int InProcessCount
        {
            get
            {
                int wip = 0;
                foreach (Machine machine in _machines)
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
            get { return _completedJobs.Count; }
        }

        public void AddMachine(Machine machine)
        {
            _machines.Add(machine);
        }

        public void Tick()
        {
            foreach (WorkItem job in _completedJobs)
            {
                job.Tick();
            }

            foreach (Machine machine in _machines)
            {
                machine.Tick();
                if (machine.HasCompletedJob)
                {
                    _completedJobs.Add(machine.RemoveCompletedJob());
                }
            }
        }

        private void AddJob(WorkItem workItem)
        {
            Machine fastestMachine = GetFastestMachine();

            if (fastestMachine != null)
            {
                fastestMachine.StartJob(workItem);
            }
            else
            {
                throw new InvalidOperationException("Trying to add workItem when there are no idle machines");
            }
        }

        private Machine GetFastestMachine()
        {
            Machine fastestMachine = null;
            foreach (Machine machine in _machines)
            {
                if (machine.IsIdle)
                {
                    if (fastestMachine == null || machine.ExpectedProcessingTime < fastestMachine.ExpectedProcessingTime)
                    {
                        fastestMachine = machine;
                    }
                }
            }
            return fastestMachine;
        }

        private bool ExistIdleMachine
        {
            get
            {
                foreach (Machine machine in _machines)
                {
                    if (machine.IsIdle)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public Set<ResourcePool> ResourcePools
        {
            get
            {
                Set<ResourcePool> _resourcePools = new Set<ResourcePool>();
                foreach (Machine machine in _machines)
                {
                    _resourcePools.Add(machine.ResourcePool);
                }

                return _resourcePools;
            }
        }

        public double AverageUtilization
        {
            get
            {
                double sum = 0;

                foreach (Machine machine in _machines)
                {
                    sum += machine.Utilization;
                }

                return sum/_machines.Count;
            }
        }

        public IEnumerable<Machine> Machines
        {
            get { return _machines; }
        }
    }
}