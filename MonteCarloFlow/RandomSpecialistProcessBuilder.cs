using Flow;
using Flow.ProbabilityDistribution;

namespace MonteCarloFlow
{
    internal class RandomSpecialistProcessBuilder : ThreeWorkstationProcessBuilder
    {
        protected override string Description
        {
            get { return string.Format("{0} specialist(s). Random processing times. {1} analyst(s), {2} tester(s), {3} developer(s)",TeamSize,AnalystCount,TesterCount,DeveloperCount); }
        }

        protected override void BuildWorkstations(WorkProcess process)
        {
            //IProbabilityDistribution analyst =
            //    ErlangDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(0), 5, Seed);
            //IProbabilityDistribution dev =
            //    ErlangDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(1), 5, Seed + 1);
            //IProbabilityDistribution tester =
            //    ErlangDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(2), 5, Seed + 2);

            IProbabilityDistribution analyst =
                ExponentialDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(0), Seed);
            IProbabilityDistribution dev =
                ExponentialDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(1), Seed + 1);
            IProbabilityDistribution tester =
                ExponentialDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(2), Seed + 2);

            for (int i = 0; i < AnalystCount; i++)
            {
                process[0].AddMachine(new Machine(analyst));
            }

            for (int i = 0; i < DeveloperCount; i++)
            {
                process[1].AddMachine(new Machine(dev));
            }

            for (int i = 0; i < TesterCount; i++)
            {
                process[2].AddMachine(new Machine(tester));
            }

        }
    }
}