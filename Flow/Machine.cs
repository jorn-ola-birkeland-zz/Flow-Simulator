using System;

namespace MonteCarloFlowTest
{
    public class Machine : IMachine
    {
        private readonly IProbabilityDistribution _distribution;
        private long _time;
        private long _utilizationTime;
        private long _completionTime;
        private WorkItem _workItem;
        private bool _isRunning = true;
        private bool _complete;

        public Machine(IProbabilityDistribution distribution)
        {
            _distribution = distribution;
        }

        public bool IsProcessing
        {
            get { return _workItem!=null && !_complete; }
        }

        public bool HasCompletedJob
        {
            get { return _workItem!=null && _complete; }
        }

        public bool IsIdle
        {
            get { return _workItem==null && _isRunning && CanStart; }
        }


        public void Tick()
        {
            _time++;
            if(_workItem!=null)
            {
                _workItem.Tick();

                _utilizationTime++;

                if (!_complete && _time >= _completionTime)
                {
                    _complete = true;
                    OnJobCompletion();
                }

            }
        }

        public double Utilization
        {
            get
            {
                return ((double) _utilizationTime)/_time;
            }
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
            long timeToCompletion = Convert.ToInt64(workItem.Size*_distribution.NextValue());
            _completionTime = _time + timeToCompletion;
            _complete = false;

            OnJobStart();
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void Start()
        {
            _isRunning = true;
        }

        protected virtual bool CanStart
        {
            get
            {
                return true;
            }
        }

        protected virtual void OnJobCompletion()
        {

        }

        protected virtual void OnJobStart()
        {
            
        }

    }
}