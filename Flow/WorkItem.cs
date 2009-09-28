namespace MonteCarloFlowTest
{
    public class WorkItem
    {
        private readonly long _startTime;
        private long _endTime;
        private double _size;

        public WorkItem(long startTime) : this(startTime,startTime)
        {
        }

        public WorkItem(long startTime, long endTime) 
        {
            _startTime = startTime;
            _endTime = endTime;
            _size = 1;
        }

        public long StartTime
        {
            get { return _startTime; }
        }


        public long EndTime
        {
            get { return _endTime; }
        }

        public long CycleTime
        {
            get { return EndTime - StartTime; }
        }

        public double Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public void Tick()
        {
            _endTime++;
        }

        public override string ToString()
        {
            return string.Format("Start:{0} End:{1} CT:{2}", StartTime, EndTime, CycleTime);
        }
    }
}