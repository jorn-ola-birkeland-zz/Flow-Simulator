using System;
using System.Collections.Generic;
using System.Linq;

namespace Flow
{
    public static class Statistics
    {
        public static double StandardDeviation<T>(this IEnumerable<T> values, Func<T,double> toDoubleConverter)
        {
            double variance = 0;
            double avg = values.Average(toDoubleConverter);
            int count = values.Count();

            foreach (T value in values)
            {
                double diff = toDoubleConverter(value) - avg;
                double diffSquared = Math.Pow(diff, 2);

                variance += diffSquared / count;
            }

            return Math.Sqrt(variance);
        }
    }
}