using Flow;
using Flow.ProbabilityDistribution;

namespace MonteCarloFlow
{
    internal class GeneralistProcessBuilder : ThreeWorkstationProcessBuilder
    {
        public GeneralistProcessBuilder()
        {
        }

        protected override string Description
        {
            get { return string.Format("{0} generalist(s). Random processing times",TeamSize); }
        }

        protected override void BuildWorkstations(WorkProcess process)
        {
            //IProbabilityDistribution distribution1 =
            //    ErlangDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(0), 5, Seed);
            //IProbabilityDistribution distribution2 =
            //    ErlangDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(1), 5, Seed + 1);
            //IProbabilityDistribution distribution3 =
            //    ErlangDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(2), 5, Seed + 2);

            IProbabilityDistribution distribution1 =
                ExponentialDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(0), Seed);
            IProbabilityDistribution distribution2 =
                ExponentialDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(1), Seed + 1);
            IProbabilityDistribution distribution3 =
                ExponentialDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(2), Seed + 2);

            ResourcePool pool = new ResourcePool(TeamSize);

            for (int i = 0; i < TeamSize; i++)
            {
                process[0].AddMachine(new Machine(distribution1, pool));
                process[1].AddMachine(new Machine(distribution2, pool));
                process[2].AddMachine(new Machine(distribution3, pool));
            }
        }
    }
}