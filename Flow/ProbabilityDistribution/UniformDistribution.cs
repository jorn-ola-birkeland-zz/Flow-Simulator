using System;
using Flow.ProbabilityDistribution;

namespace MonteCarloFlow
{
    public class UniformDistribution : ProbabilityDistribution
    {
        private readonly double _lower;
        private readonly double _upper;


        public UniformDistribution(double lower, double upper, int seed)
        {
            _lower = lower;
            _upper = upper;
            Seed = seed;
        }

        protected override double NextValue(Random rnd)
        {
            double range = _upper - _lower;

            return _lower + rnd.NextDouble()*range;
        }

        protected override string Description
        {
            get
            {
                return string.Format("Uniform distribution between {0} and {1}", _lower, _upper);
            }
        }

    }
}
