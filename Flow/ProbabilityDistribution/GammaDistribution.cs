using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonteCarloFlowTest;

namespace Flow.ProbabilityDistribution
{
    public class GammaDistribution : ProbabilityDistribution
    {
        private readonly double _k;
        private readonly double _theta;
        private readonly int _kFloor; 
        private readonly double _A;
        private readonly double _B;

        public GammaDistribution(double k, double theta)
        {
            _k = k;
            _theta = theta;
            _kFloor = (int) Math.Floor(_k);
            _A = _k - _kFloor;
            _B = 1 - _A;
        }

        protected override double NextValue(Random rnd)
        {
            double x = CalculateLnOfUniformProducts(rnd);

            double y1;
            double y2;
            do
            {
                y1 = Math.Pow(rnd.NextDouble(), (1 / _A));
                y2 = Math.Pow(rnd.NextDouble(), (1 / _B));
            } while (y1 + y2 > 1);

            double z = y1 / (y1 + y2);

            double Q = -Math.Log(rnd.NextDouble());

            return (x + z * Q) * _theta;
        }

        protected override string Description
        {
            get
            {
                return string.Format("Gamma distribution. k: {0} theta: {1}", _k, _theta);
            }
        }

        public static GammaDistribution FromScale(double k, double theta)
        {
            return new GammaDistribution(k, theta);
        }

        public static GammaDistribution FromRate(double alpha, double beta)
        {
            return new GammaDistribution(alpha, 1/beta);
        }

        private double CalculateLnOfUniformProducts(Random rnd)
        {
            double uProd = 1;
            for (int i = 0; i < _kFloor;i++ )
            {
                uProd *= rnd.NextDouble();
            }

            return -Math.Log(uProd);
        }
    }
}
