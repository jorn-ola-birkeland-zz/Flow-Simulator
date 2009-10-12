using System;
using System.Collections.Generic;
using Flow;
using MonteCarloFlow;
using MonteCarloFlowTest;
using Spring.Context;
using Spring.Context.Support;
using System.Linq;


namespace FlowSimulator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string path = args[0];

            IApplicationContext context = new XmlApplicationContext(path);

            WorkProcess process = (WorkProcess) context.GetObject("process");

            process.Tick(200000);

            List<WorkItem> workItems = new List<WorkItem>(process.CompletedWorktems);

            List<WorkItem> filteredWorkItems = workItems.FindAll(wi => wi.StartTime > 25000 && wi.EndTime <= 200000);
            List<WipEntry> filteredWipEntries = process.WipEntries.FindAll(we => we.TimeStamp > 25000 && we.TimeStamp <= 200000);


            FlowMetrics metrics = FlowMetrics.CalculateMetrics(filteredWorkItems);
            double wipAvg = CalculateAverageWip(filteredWipEntries);
            double wipStdDev = CalculateWipStdDev(filteredWipEntries, wipAvg);

            Console.WriteLine("{0}|{1:F6}|{2:F3}|{3:F3}|{4:F3}|{5:F2}|{6:F2} ", "Test process",
                              metrics.AverageThroughput,
                              metrics.AverageCycleTime, metrics.AverageWip, metrics.CycleTimeStdDev, wipAvg, wipStdDev);

            CreateHistogram(filteredWorkItems.ConvertAll(wi => (double)wi.CycleTime),50);
            CreateHistogram(filteredWipEntries.ConvertAll(wip => (double)wip.Wip), 1);
        }

        private static double CalculateWipStdDev(ICollection<WipEntry> entries, double avg)
        {
            double variance = 0;
            foreach (WipEntry wipEntry in entries)
            {
                double diff = wipEntry.Wip - avg;
                double diffSquared = Math.Pow(diff, 2);

                variance += diffSquared / entries.Count;
            }

            return  Math.Sqrt(variance);

        }

        private static double CalculateAverageWip(ICollection<WipEntry> wipEntries)
        {
            double sum=0;
            foreach (WipEntry entry in wipEntries)
            {
                sum += entry.Wip;
            }

            return sum/wipEntries.Count;
        }

        private static void CreateHistogram(ICollection<double> values, double incrementSize)
        {
            Dictionary<double,int> categories = new Dictionary<double, int>();
            double maxCategory = 0;

            foreach (double value in values)
            {
                double category = Math.Floor(value/incrementSize);
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
                Console.WriteLine("[{0:F0}-{1:F0}>:{2}({3:F2}%)", i*incrementSize,(i+1)*incrementSize, count, count/values.Count);
            }
        }
    }
}