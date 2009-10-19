using System.Collections.Generic;
using Flow;

namespace MonteCarloFlow
{
    internal abstract class ThreeWorkstationProcessBuilder
    {
        private int _seed;
        private  int _developerCount = 3;
        private int _analystCount = 1;
        private int _testerCount = 1;

        private readonly List<double> _expectedProcessingTimes = new List<double>();

        protected ThreeWorkstationProcessBuilder()
        {
            _expectedProcessingTimes.Add(250);
            _expectedProcessingTimes.Add(1500);
            _expectedProcessingTimes.Add(150);
        }

        public void Build(WorkProcess process, WipTokenPool[] wipLimits)
        {
            process.Add(new WorkStation(wipLimits[0]));
            process.Add(new WorkStation(wipLimits[1]));
            process.Add(new WorkStation(wipLimits[2]));

            BuildWorkstations(process);
        }

        public int Seed
        {
            get { return _seed; }
            set { _seed = value; }
        }

        public override string ToString()
        {
            return Description;
        }

        protected abstract string Description { get; }
        protected abstract void BuildWorkstations(WorkProcess process);

        protected double GetExpectedProcessingTimeOfStation(int index)
        {
            return _expectedProcessingTimes[index];
        }

        protected int TeamSize
        {
            get { return DeveloperCount+AnalystCount+TesterCount; }
        }


        public int DeveloperCount
        {
            get { return _developerCount; }
            set { _developerCount = value; }
        }

        public int AnalystCount
        {
            get { return _analystCount; }
            set { _analystCount = value; }
        }

        public int TesterCount
        {
            get { return _testerCount; }
            set { _testerCount = value; }
        }

        public void SetProccesingTimes(int[] processingTimes)
        {
            for (int i = 0; i < processingTimes.Length;i++ )
            {
                _expectedProcessingTimes[i] = processingTimes[i];
            }
        }
    }
}