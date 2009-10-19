using System.Collections.Generic;
using Flow;
using Flow.ProbabilityDistribution;
using MonteCarloFlowTest;

namespace MonteCarloFlow
{
    internal class VariableSpecialistProcessBuilder : ThreeWorkstationProcessBuilder
    {
        protected override string Description
        {
            get { return string.Format("{0} specialist(s). {1} analyst(s), {2} tester(s), {3} developer(s). Random processing time, with random mean",TeamSize, AnalystCount,TesterCount,DeveloperCount); }
        }

        protected override void BuildWorkstations(WorkProcess process)
        {
            List<ProbabilityDistribution> distributions = new List<ProbabilityDistribution>();

            for (int workstationIndex = 0; workstationIndex < 3; workstationIndex++ )
            {
                double expectedProcessingTime = GetExpectedProcessingTimeOfStation(workstationIndex);

                double offset = expectedProcessingTime * 0.50;

                UniformDistribution ud = new UniformDistribution(expectedProcessingTime - offset, expectedProcessingTime + offset, Seed); 

                //distributions.Add(ErlangDistribution.FromExpectedValue(ud.NextValue(),5,Seed+workstationIndex));
                distributions.Add(ExponentialDistribution.FromExpectedValue(ud.NextValue(), Seed + workstationIndex));
            }

            for (int i = 0; i < AnalystCount; i++)
            {
                process[0].AddMachine(new Machine(distributions[0]));
            }

            for (int i = 0; i < DeveloperCount; i++)
            {
                process[1].AddMachine(new Machine(distributions[1]));
            }

            for (int i = 0; i < TesterCount; i++)
            {
                process[2].AddMachine(new Machine(distributions[2]));
            }


        }
    }
}