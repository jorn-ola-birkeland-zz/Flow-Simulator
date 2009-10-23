using System;
using System.Collections.Generic;
using System.Globalization;
using Flow;
using MonteCarloFlow;
using MonteCarloFlowTest;
using Spring.Context;
using Spring.Context.Support;
using System.Linq;
using Wintellect.PowerCollections;
using System.Threading;


namespace FlowSimulator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");

            string path = args[0];

            IApplicationContext context = new XmlApplicationContext(path);

            var process = (WorkProcess) context.GetObject("process");

            process.Tick(200000);

            IEnumerable<WorkItem> filteredWorkItems = process.CompletedWorktems.Where(wi => wi.StartTime > 25000);
            IEnumerable<WipEntry> filteredWipEntries = process.WipEntries.Where(we => we.TimeStamp > 25000);
            IEnumerable<UtilizationEntry> filteredUtilizationEntries = process.UtilizationEntries.Where(ue => ue.TimeStamp > 25000);


            FlowMetrics metrics = FlowMetrics.CalculateMetrics(filteredWorkItems);
            WriteMetrics(metrics);
            WriteWip(filteredWipEntries);
            WriteUtilization(filteredUtilizationEntries);


            const string title = "Cycle time histogram";
            Console.WriteLine();
            Console.WriteLine(title);
            CreateHistogram(filteredWorkItems, wi => wi.CycleTime,50);

            Console.WriteLine();
            Console.WriteLine("WIP histogram");
            CreateHistogram(filteredWipEntries,wip => wip.Wip, 1);

            WriteResourceUtilization(filteredUtilizationEntries);
        }

        private static void WriteUtilization(IEnumerable<UtilizationEntry> utilizationEntries)
        {
            double utilAvg = utilizationEntries.Average(ue => ue.Utilization);
            double utilStdDev = utilizationEntries.StandardDeviation(ue => ue.Utilization);

            Console.WriteLine("Avg. utilization:   {0:###.00%}", utilAvg);
            Console.WriteLine("Util. std.dev.:     {0}", utilStdDev);
        }


        private static void WriteResourceUtilization(IEnumerable<UtilizationEntry> entries)
        {
            var utilizationEntries = GroupUtilizationEntries(entries);

            foreach (var key in utilizationEntries.Keys.OrderBy(key => key))
            {
                Console.WriteLine();
                Console.WriteLine(key);
                Console.WriteLine("Avg. utilization: {0:###.00%}", utilizationEntries[key].Average(d => d));
                //Console.WriteLine("Utilization std.dev.: {0}", utilizationEntries[key].StandardDeviation(d => d));
            }
        }

        private static MultiDictionary<string, double> GroupUtilizationEntries(IEnumerable<UtilizationEntry> entries)
        {
            var utilizationEntries = new MultiDictionary<string, double>(true);
            foreach (var entry in entries)
            {
                utilizationEntries.Add(entry.ResourcePoolName, entry.Utilization);
            }
            return utilizationEntries;
        }

        private static void WriteWip(IEnumerable<WipEntry> filteredWipEntries)
        {
            var wipAvg = filteredWipEntries.Average(wipEntry => wipEntry.Wip);
            var wipStdDev = filteredWipEntries.StandardDeviation(wipEntry => wipEntry.Wip);

            Console.WriteLine("Avg. WIP:           {0}", wipAvg);
            Console.WriteLine("WIP std.dev.:       {0}", wipStdDev);
        }

        private static void WriteMetrics(FlowMetrics metrics)
        {
            Console.WriteLine("Avg. throughput:    {0}", metrics.AverageThroughput);
            Console.WriteLine("Avg. cycle time:    {0}", metrics.AverageCycleTime);
            Console.WriteLine("Calculated WIP:     {0}", metrics.AverageWip);
            Console.WriteLine("Cycle time std.dev: {0}", metrics.CycleTimeStdDev);
        }

        private static void CreateHistogram<T>(IEnumerable<T> values, Converter<T,double> toDouble,  double incrementSize)
        {
            var categories = new Dictionary<double, int>();
            double maxCategory = 0;
            int total = 0;


            foreach (T value in values)
            {
                total++;
                double category = Math.Floor(toDouble(value)/incrementSize);
                maxCategory = category > maxCategory ? category : maxCategory;

                if(!categories.ContainsKey(category))
                {
                    categories.Add(category,1);
                }
                else
                {
                    categories[category]+=1;
                }
            }

            for(double i=0;i<maxCategory+1;i++)
            {
                double count = categories.ContainsKey(i) ? categories[i] : 0;
                Console.WriteLine("[{0:F0}-{1:F0}>: {2} ({3:F2}%)", i*incrementSize,(i+1)*incrementSize, count, count/total*100);
            }
        }
    }
}