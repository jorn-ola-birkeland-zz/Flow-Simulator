using System;
using Flow.ProbabilityDistribution;
using MonteCarloFlowTest;

namespace Flow.ProbabilityDistribution
{
    public class ExponentialDistribution : ProbabilityDistribution
    {
        private readonly double _lambda;


        public static ExponentialDistribution FromRateParameter(double lambda)
        {
            return new ExponentialDistribution(lambda);
        }

        public static ExponentialDistribution FromExpectedValue(double expectedValue)
        {
            return new ExponentialDistribution(1.0 / expectedValue);
        }

        public static ExponentialDistribution FromExpectedValue(double expectedValue, int seed)
        {
            ExponentialDistribution e = FromExpectedValue(expectedValue);
            e.Seed = seed;

            return e;
        }

        private ExponentialDistribution(double lambda)
        {
            _lambda = lambda;
        }

        protected override double NextValue(Random rnd)
        {
            double p = rnd.NextDouble();

            return -Math.Log(1 - p) / _lambda;
        }

        protected override string Description
        {
            get
            {
                return string.Format("Exponential distribution. Expected value: {0}", 1 / _lambda);
            }
        }

        public override double ExpectedValue
        {
            get { return 1.0/_lambda; }
        }
    }
}