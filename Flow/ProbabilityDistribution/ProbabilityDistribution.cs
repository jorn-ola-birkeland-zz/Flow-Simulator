using MonteCarloFlowTest;
using System;
namespace Flow.ProbabilityDistribution
{
    public abstract class ProbabilityDistribution : IProbabilityDistribution
    {
        private int _seed;
        private Random _rnd;


        protected ProbabilityDistribution()
        {
            _rnd = new Random();
        }

        public double NextValue()
        {
            return NextValue(_rnd);
        }


        public override string ToString()
        {
            return Description;
        }

        public int Seed
        {
            get
            {
                return _seed;
            }
            set
            {
                _seed = value;
                _rnd = new Random(_seed);
            }
        }

        protected abstract double NextValue(Random rnd);
        protected abstract string Description { get; }


    }
}
