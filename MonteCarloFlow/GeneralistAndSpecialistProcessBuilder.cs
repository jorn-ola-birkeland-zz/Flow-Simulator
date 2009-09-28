using MonteCarloFlowTest;

namespace MonteCarloFlow
{
    internal class GeneralistAndSpecialistProcessBuilder : ThreeWorkstationProcessBuilder
    {

        protected override string Description
        {
            get { return string.Format("{0} analyst-tester(s) and {1} specialized developer(s), random processing times (Erlang-5)",AnalystCount+TesterCount,DeveloperCount); }
        }

        protected override void BuildWorkstations(IWorkProcess process)
        {
            IProbabilityDistribution analyst =
                ErlangDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(0), 5, Seed);
            IProbabilityDistribution dev =
                ErlangDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(1), 5, Seed);
            IProbabilityDistribution tester =
                ErlangDistribution.FromExpectedValue(GetExpectedProcessingTimeOfStation(2), 5, Seed);

            for (int i = 0; i < DeveloperCount; i++)
            {
                process[1].AddMachine(new Machine(dev));
            }

            for (int i = 0; i < AnalystCount+TesterCount; i++)
            {
                ResourcePool testerAnalyst = new ResourcePool(1);
                process[0].AddMachine(new PooledMachine(analyst, testerAnalyst));
                process[2].AddMachine(new PooledMachine(tester, testerAnalyst));
            }

        }
    }
}