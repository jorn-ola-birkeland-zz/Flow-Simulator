using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GammaFit
{
    class GammaParameters
    {
        public GammaParameters(double k, double theta)
        {
            this.k = k;
            this.theta = theta;
        }

        public readonly double k;
        public readonly double theta;

        public override string ToString()
        {
            return string.Format("k:{0}, theta:{1}",k,theta);
        }
    }

    class GammaFitter
    {
        public GammaParameters EstimateParameters(IEnumerable<double> samples)
        {
            double k = EstimateK(samples);
            double theta = EstimateTheta(samples, k);

            return new GammaParameters(k, theta);
        }

        private static double EstimateTheta(IEnumerable<double> samples, double k)
        {
            double avg = CalculateAverage(samples);
            return avg/k;
        }

        private double EstimateK(IEnumerable<double> samples)
        {
            double k = CalculateInitialK(samples);
            double s = CalculateInitialS(samples);

            double nextK;
            double previousK;

            do
            {

                nextK = k - (Math.Log(k) - Digamma(k) - s)/(1/k - Trigamma(k));

                previousK = k;
                k = nextK;

            } while (Math.Abs(nextK - previousK) > 0.001);

            return nextK;
        }

        private static double Trigamma(double k)
        {
            double kSquared = Math.Pow(k, 2);
            if(k<8)
            {
                return Trigamma(k + 1) + 1/kSquared;
            }
            else
            {
                return (1 + (1 + (1 - (((double) 1)/5 - 1/(7*kSquared))/kSquared)/(3*k))/(2*k))/k;
            }
        }

        private static double Digamma(double k)
        {
            if(k<8)
            {
                return Digamma(k + 1) - 1/k;
            }
            else
            {
                double kSquared = Math.Pow(k, 2);

                return Math.Log(k) - (1 + (1 - (((double)1)/10 - 1/(21*kSquared))/kSquared)/(6*k))/(2*k);
            }
        }

        private double CalculateInitialK(IEnumerable<double> samples)
        {
            double s = CalculateInitialS(samples);

            double sMinus3Squred = Math.Pow(s-3,2);
            double k = (3 - s + Math.Sqrt(sMinus3Squred + 24*s))/(12*s);

            return k;
        }

        private static double CalculateInitialS(IEnumerable<double> samples)
        {
            double avgOfLns = CalculateAverageOfLogarithms(samples);
            double lnOfAvgs = CalcualteLogarithmOfAverage(samples);

            return lnOfAvgs - avgOfLns;
        }

        private static double CalcualteLogarithmOfAverage(IEnumerable<double> samples)
        {
            double avg = CalculateAverage(samples);

            return Math.Log(avg);
        }

        private static double CalculateAverage(IEnumerable<double> samples)
        {
            double sum = 0;
            int n = 0;
            foreach (double sample in samples)
            {
                n++;
                sum += sample;
            }

            return sum/n;
        }

        private static double CalculateAverageOfLogarithms(IEnumerable<double> samples)
        {
            double sum = 0;
            int n = 0;
            foreach (double sample in samples)
            {
                n++;
                sum += Math.Log(sample);
            }

            return sum / n;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string path = args[0];

            using(TextReader reader = File.OpenText(path))
            {
                IEnumerable<double> samples = GetSamples(reader);
                GammaFitter fitter = new GammaFitter();
                
                GammaParameters parameters = fitter.EstimateParameters(samples);

                Console.WriteLine(parameters);
            }
        }

        private static IEnumerable<double> GetSamples(TextReader reader)
        {
            string line;

            List<double> samples = new List<double>();

            while((line=reader.ReadLine())!=null)
            {
                samples.Add(Convert.ToDouble(line));
            }

            return samples;
        }
    }
}
