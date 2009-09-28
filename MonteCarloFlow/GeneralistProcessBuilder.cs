using MonteCarloFlowTest;

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

        protected override void BuildWorkstations(IWorkProcess process)
        {
            IProbabilityDistribution distribution1 =
                ErlangDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(0), 5, Seed);
            IProbabilityDistribution distribution2 =
                ErlangDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(1), 5, Seed + 1);
            IProbabilityDistribution distribution3 =
                ErlangDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(2), 5, Seed + 2);

            ResourcePool pool = new ResourcePool(TeamSize);

            for (int i = 0; i < TeamSize; i++)
            {
                process[0].AddMachine(new PooledMachine(distribution1, pool));
                process[1].AddMachine(new PooledMachine(distribution2, pool));
                process[2].AddMachine(new PooledMachine(distribution3, pool));
            }
        }
    }
}