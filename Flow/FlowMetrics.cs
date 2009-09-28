using System;
using System.Collections.Generic;
using MonteCarloFlowTest;

namespace MonteCarloFlow
{
    public class FlowMetrics
    {
        private readonly double _avgThroughput;
        private readonly double _avgCycleTime;
        private readonly double _avgWip;
        private readonly double _cycleTimeStdDev;

        private FlowMetrics(double throughput, double cycleTime, double wip, double cycleTimeStdDev)
        {
            _avgThroughput = throughput;
            _avgCycleTime = cycleTime;
            _avgWip = wip;
            _cycleTimeStdDev = cycleTimeStdDev;
        }

        public double AverageCycleTime
        {
            get { return _avgCycleTime; }
        }

        public double AverageThroughput
        {
            get { return _avgThroughput; }
        }

        public double AverageWip
        {
            get { return _avgWip; }
        }
        
        public double CycleTimeStdDev
        {
            get { return _cycleTimeStdDev; }
        }

        public override string ToString()
        {
            return string.Format("TP={0}, CT={1}, WIP={2}", _avgThroughput, _avgCycleTime, _avgWip);
        }

        public static FlowMetrics CalculateMetrics(params WorkItem[] workItems)
        {
            return CalculateMetrics((IEnumerable<WorkItem>)workItems);    
        }

        public static FlowMetrics CalculateMetrics(IEnumerable<WorkItem> jobs)
        {
            long totalCycleTime = 0;
            int jobCount = 0;

            long earliestStartTime = long.MaxValue;
            long latestEndTime = long.MinValue;

            foreach (WorkItem job in jobs)
            {
                jobCount++;
                totalCycleTime += job.CycleTime;

                earliestStartTime = job.StartTime < earliestStartTime ? job.StartTime : earliestStartTime;
                latestEndTime = job.EndTime > latestEndTime ? job.EndTime : latestEndTime;
            }

            double avgThroughput = ((double)jobCount) / (latestEndTime - earliestStartTime);

            double avgCycleTime = ((double)totalCycleTime) / jobCount;

            double avgWip = avgThroughput*avgCycleTime;


            double variance=0;
            foreach (WorkItem job in jobs)
            {
                double diff = job.CycleTime - avgCycleTime;
                double diffSquared = Math.Pow(diff, 2);

                variance += diffSquared/jobCount;
            }

            double cycleTimeStdDev = Math.Sqrt(variance);

            return new FlowMetrics(avgThroughput,avgCycleTime,avgWip, cycleTimeStdDev);
        }

        public static FlowMetrics CalculateAverage(IEnumerable<FlowMetrics> metrics)
        {
            int count = 0;
            double wip = 0;
            double ct = 0;
            double tp = 0;
            double stdDev = 0;
            foreach (FlowMetrics metric in metrics)
            {
                count++;
                wip += metric.AverageWip;
                ct += metric.AverageCycleTime;
                tp += metric.AverageThroughput;
                stdDev += metric.CycleTimeStdDev;
            }

            return new FlowMetrics(tp/count,ct/count,wip/count,stdDev/count);
        }
    }
}
