using Flow;
using Flow.ProbabilityDistribution;

namespace MonteCarloFlow
{
    internal class DeterministicSpecialistProcessBuilder : ThreeWorkstationProcessBuilder
    {
        protected override void BuildWorkstations(WorkProcess process)
        {
            IProbabilityDistribution analyst = new DeterministicDistribution(GetExpectedProcessingTimeOfStation(0));
            IProbabilityDistribution dev = new DeterministicDistribution(GetExpectedProcessingTimeOfStation(1));
            IProbabilityDistribution tester = new DeterministicDistribution(GetExpectedProcessingTimeOfStation(2));


            for (int i = 0; i < AnalystCount; i++)
            {
                process[0].AddMachine(new Machine(analyst));
            }

            for (int i = 0; i < DeveloperCount;i++ )
            {
                process[1].AddMachine(new Machine(dev));
            }

            for (int i = 0; i < TesterCount; i++)
            {
                process[2].AddMachine(new Machine(tester));
            }

        }

        protected override string Description
        {
            get { return string.Format("{0} specialist(s) with deterministic processing time. {1} analyst(s), {2} tester(s), {3} developer(s)",TeamSize, AnalystCount,TesterCount,DeveloperCount); }
        }
    }
}