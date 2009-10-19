using System;
using Flow.ProbabilityDistribution;
using MonteCarloFlowTest;
namespace MonteCarloFlow
{
    public class ErlangDistribution  : ProbabilityDistribution
    {
        private readonly int _k;
        private readonly double _lambda;


        public ErlangDistribution(double lambda, int k)
        {
            _k = k;
            _lambda = lambda;
        }

        protected override string Description
        {
            get
            {
                return string.Format("Erlang-{0} distribution. Expected value: {1}", _k, _k / _lambda);
            }
        }

        public override double ExpectedValue
        {
            get { return _k/_lambda; }
        }

        public static ErlangDistribution FromExpectedValue(double expectedValue, int k)
        {
            double lambda = k / expectedValue;

            return new ErlangDistribution(lambda, k);
        }

        public static ErlangDistribution FromExpectedValue(double expectedValue, int k, int seed)
        {
            ErlangDistribution e = FromExpectedValue(expectedValue, k);
            e.Seed = seed;

            return e;
        }

        protected override double NextValue(Random rnd)
        {
            double value=0;
            for(int i = 0; i<_k;i++)
            {
                double u = rnd.NextDouble();
                value += -Math.Log(u)/_lambda;
            }

            return value;
        }
    }
}
