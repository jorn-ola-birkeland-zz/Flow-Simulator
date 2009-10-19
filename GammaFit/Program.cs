using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GammaFit
{
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
