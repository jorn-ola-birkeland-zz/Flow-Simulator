using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Flow.ProbabilityDistribution;
using MonteCarloFlowTest;
using System.Threading;
using Flow;
using System.IO;
using System.Xml;

namespace MonteCarloFlow
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if(args.Length>0)
            {
                ImportSimulation(args[0]);
                return;
            }

            //RunMultiGammaDistribution();


            //RunGammaIterationExperiment();


            //return;

            int analysts = 2;
            int developers = 4;
            int testers = 1;

            int[] processingTimes = new int[] {20,40,10};
            int[] kanbanWips = new int[] { 2, 4, 1 };
            int conwipWips = 7;

            //IProbabilityDistribution sizeDistribution;

            IProbabilityDistribution gamma = GammaDistribution.FromScale(1.5129, 2.5305);
            //IProbabilityDistribution sgamma = ShiftedGammaDistribution.FromLeftAndModeAndMean(3, 6, 8);
            ProbabilityDistribution sizeDistribution = new DeterministicDistribution(1);
            //ProbabilityDistribution sizeDistribution = ExponentialDistribution.FromExpectedValue(2, seed - 1);
            //IProbabilityDistribution sizeDistribution = ErlangDistribution.FromExpectedValue(1, 2);
            //IProbabilityDistribution sizeDistribution2 = ErlangDistribution.FromExpectedValue(1.25, 2);
            //IProbabilityDistribution sizeDistribution3 = ErlangDistribution.FromExpectedValue(1.5, 3);
            //IProbabilityDistribution sizeDistribution4 = ErlangDistribution.FromExpectedValue(3, 3);
            //IProbabilityDistribution sizeDistribution5 = ErlangDistribution.FromExpectedValue(5, 3);
            //ProbabilityDistribution sizeDistribution6 = ErlangDistribution.FromExpectedValue(1, 1);



            List<ThreeWorkstationProcessBuilder> builders = new List<ThreeWorkstationProcessBuilder>
            {
                new DeterministicSpecialistProcessBuilder(),
                new GeneralistAndSpecialistProcessBuilder(),
                new GeneralistProcessBuilder(),
                new RandomSpecialistProcessBuilder(),
                new SpecializedGeneralistProcessBuilder()
            }
            ;

            //RunTracking(sizeDistribution,new DeterministicSpecialistProcessBuilder(),kanbanWips);

            int experiment = 0;
            foreach (ThreeWorkstationProcessBuilder builder in builders)
            {

                builder.AnalystCount = analysts;
                builder.TesterCount = testers;
                builder.DeveloperCount = developers;
                builder.SetProccesingTimes(processingTimes);

                Console.WriteLine();
                Console.WriteLine("----- Experiment {0} -----", ++experiment);
                Console.WriteLine("Feature size: {0}", sizeDistribution);
                Console.WriteLine("Team type: {0}", builder);
                Console.WriteLine("Expected processing times: {0}-{1}-{2}", processingTimes[0], processingTimes[1], processingTimes[2]);
                Console.WriteLine("Kanban WIP limit: {0}-{1}-{2}", kanbanWips[0], kanbanWips[1], kanbanWips[2]);
                Console.WriteLine("CONWIP WIP limit: {0}", conwipWips);
                Console.WriteLine();
                Console.WriteLine("Type  |TP      |CT      |Wip  |CT Std");

                RunExperiment(sizeDistribution, builder, kanbanWips, conwipWips);
            }
        }

        private static void ImportSimulation(string path)
        {
        }

        private static void RunMultiGammaDistribution()
        {
            int i = 6000;

            IProbabilityDistribution sizeDistribution = ShiftedGammaDistribution.FromLeftAndModeAndMean(0, 0.5, 1);
            IProbabilityDistribution sizeDistribution2 = ShiftedGammaDistribution.FromLeftAndModeAndMean(0.5, 1.25, 2);
            IProbabilityDistribution sizeDistribution3 = ShiftedGammaDistribution.FromLeftAndModeAndMean(1, 2, 3);
            IProbabilityDistribution sizeDistribution4 = ShiftedGammaDistribution.FromLeftAndModeAndMean(2, 3.5, 5);
            IProbabilityDistribution sizeDistribution5 = ShiftedGammaDistribution.FromLeftAndModeAndMean(4, 6, 8);

            sizeDistribution.Seed = 1;
            sizeDistribution2.Seed = 2;
            sizeDistribution3.Seed = 3;
            sizeDistribution4.Seed = 4;
            sizeDistribution5.Seed = 5;

            while (i-- > 0)
            {
                Console.WriteLine(sizeDistribution.NextValue());
                Console.WriteLine(sizeDistribution2.NextValue());
                Console.WriteLine(sizeDistribution3.NextValue());
                Console.WriteLine(sizeDistribution4.NextValue());
                Console.WriteLine(sizeDistribution5.NextValue());
            }
        }

        private static void RunGammaIterationExperiment()
        {

            Random rnd = new Random();

            Console.WriteLine("Expected features;Actual features");


            int minimumExpectedSize = 74;
            int runs = 3000;
            int i = runs;

            while(i-->0)
            {
                int expectedSize = 0;

                List<IProbabilityDistribution> distributions = new List<IProbabilityDistribution>();
                while (expectedSize < minimumExpectedSize)
                {
                    ShiftedGammaDistribution distribution;
                    switch (rnd.Next(0, 5))
                    {
                        case 0:
                            distribution = ShiftedGammaDistribution.FromLeftAndModeAndMean(0, 0.5, 1);
                            expectedSize += 1;
                            break;
                        case 1:
                            distribution = ShiftedGammaDistribution.FromLeftAndModeAndMean(0.5, 1.25, 2);
                            expectedSize += 2;
                            break;
                        case 2:
                            distribution = ShiftedGammaDistribution.FromLeftAndModeAndMean(1, 2, 3);
                            expectedSize += 3;
                            break;
                        case 3:
                            distribution = ShiftedGammaDistribution.FromLeftAndModeAndMean(2, 3.5, 5);
                            expectedSize += 5;
                            break;
                        default:
                            distribution = ShiftedGammaDistribution.FromLeftAndModeAndMean(4, 6, 8);
                            expectedSize += 8;
                            break;
                    }

                    distribution.Seed = expectedSize;
                    distributions.Add(distribution);
                }

                int plannedFeatures = distributions.Count;

                double sum = 0;
                int features = 0;

                foreach (IProbabilityDistribution distribution in distributions)
                {
                    sum += distribution.NextValue();
                    if (sum < expectedSize)
                    {
                        features++;
                    }
                }

                Console.WriteLine("{0};{1}", plannedFeatures, features);

            }
        }

        private static void RunTracking(ProbabilityDistribution sizeDistribution,
                                        ThreeWorkstationProcessBuilder processBuilder, int[] kanbanWips)
        {
            WorkProcess kanbanWorkProcess =
                new WorkProcess(new RandomSizeInfiniteBacklog(sizeDistribution));
            processBuilder.Build(kanbanWorkProcess, GetWipTokens(kanbanWips));

            for (int i = 0; i < 20000; i++)
            {
                kanbanWorkProcess.Tick(150);

                for (int j = 0; j < 3; j++)
                {
                    Console.Write("{0} {1} ", kanbanWorkProcess[j].InProcessCount,
                                  kanbanWorkProcess[j].CompletionQueueCount);
                }
                FlowMetrics metrics = FlowMetrics.CalculateMetrics(kanbanWorkProcess.CompletedWorktems);
                Console.WriteLine("{0} {1:F6} {2:F2} {3:F2}", kanbanWorkProcess.CompletionQueueCount,
                                  metrics.AverageThroughput, metrics.AverageCycleTime, metrics.AverageWip);
            }
        }

        private static WipTokenPool[] GetWipTokens(int[] wips)
        {
            List<WipTokenPool> wipLimits = new List<WipTokenPool>();

            foreach (int wip in wips)
            {
                wipLimits.Add(new WipTokenPool(wip));
            }

            return wipLimits.ToArray();
        }


        private static void RunExperiment(ProbabilityDistribution sizeDistribution,
                                          ThreeWorkstationProcessBuilder processBuilder, int[] kanbanWips,
                                          int conwipWips)
        {
            int run = 10;

            Random rnd = new Random();

            List<FlowMetrics> conwipMetricsList = new List<FlowMetrics>();
            List<FlowMetrics> kanbanMetricsList = new List<FlowMetrics>();

            while (run-- > 0)
            {
                WipTokenPool conwipWipToken = new WipTokenPool(conwipWips);
                WipTokenPool[] kanbanWipTokens = GetWipTokens(kanbanWips);
                WipTokenPool[] conwipWipTokens = new WipTokenPool[] { conwipWipToken, conwipWipToken, conwipWipToken };

                int seed = rnd.Next();

                sizeDistribution.Seed = seed;
                processBuilder.Seed = seed;


                WorkProcess conwipWorkProcess =
                    new WorkProcess(new RandomSizeInfiniteBacklog(sizeDistribution));

                processBuilder.Build(conwipWorkProcess, conwipWipTokens);

                WorkProcess kanbanWorkProcess =
                    new WorkProcess(new RandomSizeInfiniteBacklog(sizeDistribution));
                processBuilder.Build(kanbanWorkProcess, kanbanWipTokens);

                FlowMetrics conwipMetrics = RunProcess(conwipWorkProcess, seed);
                FlowMetrics kanbanMetrics = RunProcess(kanbanWorkProcess, seed);

                conwipMetricsList.Add(conwipMetrics);
                kanbanMetricsList.Add(kanbanMetrics);

                PrintResults(conwipMetrics, "CONWIP");
                PrintResults(kanbanMetrics, "Kanban");
            }

            PrintResults(FlowMetrics.CalculateAverage(conwipMetricsList), "CON_Av");
            PrintResults(FlowMetrics.CalculateAverage(kanbanMetricsList), "Kan_Av");
        }


        private static FlowMetrics RunProcess(WorkProcess process, int seed)
        {
            process.Tick(200000);
            return CaclulateMetrics(process);
        }

        private static void PrintResults(FlowMetrics metrics, string processType)
        {
            Console.WriteLine("{0}|{1:F6}|{2:F3}|{3:F3}|{4:F3}", processType,
                              metrics.AverageThroughput,
                              metrics.AverageCycleTime, metrics.AverageWip, metrics.CycleTimeStdDev);
        }

        private static FlowMetrics CaclulateMetrics(WorkProcess process)
        {
            CycleTimeRangeEnumerator enumerator = new CycleTimeRangeEnumerator(25000, 500000, process.CompletedWorktems);

            return FlowMetrics.CalculateMetrics(enumerator);
        }
    }

    internal class CycleTimeRangeEnumerator : IEnumerable<WorkItem>
    {
        private readonly long _startTime;
        private readonly long _endTime;
        private IEnumerable<WorkItem> _jobs;

        public CycleTimeRangeEnumerator(long startTime, long endTime, IEnumerable<WorkItem> jobs)
        {
            _startTime = startTime;
            _endTime = endTime;
            _jobs = jobs;
        }

        IEnumerator<WorkItem> IEnumerable<WorkItem>.GetEnumerator()
        {
            foreach (WorkItem job in _jobs)
            {
                if (job.StartTime >= _startTime && job.EndTime <= _endTime)
                {
                    yield return job;
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable<WorkItem>) this).GetEnumerator();
        }
    }
}