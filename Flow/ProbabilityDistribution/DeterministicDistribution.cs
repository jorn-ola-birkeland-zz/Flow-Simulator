using Flow.ProbabilityDistribution;
using System;

namespace MonteCarloFlowTest
{
    public class DeterministicDistribution : ProbabilityDistribution
    {
        private readonly double _value;

        public DeterministicDistribution(double value)
        {
            _value = value;
        }

        protected override double NextValue(Random rnd)
        {
            return _value;
        }

        protected override string Description
        {
            get { return "Deterministic distribution. Value: " + _value; }
        }

    }
}