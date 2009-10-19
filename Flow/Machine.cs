using System;
using Flow.ProbabilityDistribution;
using MonteCarloFlowTest;

namespace Flow
{
    public class Machine 
    {
        private readonly ResourcePool _pool;
        private readonly IProbabilityDistribution _distribution;
        private long _time;
        private long _utilizationTime;
        private long _completionTime;
        private WorkItem _workItem;
        private bool _complete;

        public Machine(IProbabilityDistribution distribution) : this(distribution,new ResourcePool(1))
        {
        }

        public Machine(IProbabilityDistribution distribution, ResourcePool resourcePool) 
        {
            _pool = resourcePool;
            _distribution = distribution;
        }

        public bool IsProcessing
        {
            get { return _workItem != null && !_complete; }
        }

        public bool HasCompletedJob
        {
            get { return _workItem != null && _complete; }
        }

        public bool IsIdle
        {
            get { return _workItem == null && _pool.HasResources; }
        }


        public void Tick()
        {
            _time++;
            if (_workItem != null)
            {
                _workItem.Tick();

                _utilizationTime++;

                if (!_complete && _time >= _completionTime)
                {
                    _complete = true;
                    _pool.UnlockResource();
                }

            }
        }

        public double Utilization
        {
            get
            {
                return ((double)_utilizationTime) / _time;
            }
        }

        public ResourcePool ResourcePool
        {
            get
            {
                return _pool;
            }
        }

        public double ExpectedProcessingTime
        {
            get { return _distribution.ExpectedValue; }
        }

        public WorkItem RemoveCompletedJob()
        {
            WorkItem workItem = _workItem;
            _workItem = null;

            return workItem;
        }

        public void StartJob(WorkItem workItem)
        {
            _workItem = workItem;
            long timeToCompletion = Convert.ToInt64(workItem.Size * _distribution.NextValue());
            _completionTime = _time + timeToCompletion;
            _complete = false;

            _pool.LockResource();
        }
    }
}