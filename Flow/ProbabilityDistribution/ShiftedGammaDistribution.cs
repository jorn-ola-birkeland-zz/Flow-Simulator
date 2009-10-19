using System;

namespace Flow.ProbabilityDistribution
{
    public class ShiftedGammaDistribution : IProbabilityDistribution
    {
        private readonly double _shift;
        private readonly GammaDistribution _gamma;
        private ShiftedGammaDistribution(double shift, GammaDistribution gamma)
        {
            _gamma = gamma;
            _shift = shift;
        }

        public double NextValue()
        {
            return _gamma.NextValue() + _shift;
        }

        public int Seed
        {
            get { return _gamma.Seed;  }
            set { _gamma.Seed = value; }
        }

        public double ExpectedValue
        {
            get { return _gamma.ExpectedValue+_shift; }
        }

        public static ShiftedGammaDistribution FromLeftAndModeAndMean(double leftMostValue, double mode, double mean)
        {
            double shift = leftMostValue;

            double nmean = mean - shift;
            double nmode = mode - shift;

            double k = nmean/(nmean - nmode);

            double theta = nmean/k;

            //Console.WriteLine("k:{0}, theta:{1}", k, theta);


            GammaDistribution g = GammaDistribution.FromScale(k, theta);

            return new ShiftedGammaDistribution(shift,g);
        }
    }
}
