using System.Collections.Generic;
using Flow;
using Flow.ProbabilityDistribution;

namespace MonteCarloFlow
{
    internal class SpecializedGeneralistProcessBuilder : ThreeWorkstationProcessBuilder
    {
        protected override string Description
        {
            get { return string.Format("{0} specialized generalist(s). Random processing times",TeamSize); }
        }

        protected override void BuildWorkstations(WorkProcess process)
        {
            List<ResourcePool> teamMembers = new List<ResourcePool>();
 
            for(int i=0;i<TeamSize;i++)
            {
                teamMembers.Add(new ResourcePool(1));
            }

            for (int workstationIndex = 0; workstationIndex < 3; workstationIndex++ )
            {
                double expectedProcessingTime = GetExpectedProcessingTimeOfStation(workstationIndex);

                double offset = expectedProcessingTime*0.25;

                UniformDistribution ud = new UniformDistribution(expectedProcessingTime-offset,expectedProcessingTime+offset, Seed); 
  
                for(int machineIndex=0;machineIndex<TeamSize; machineIndex++)
                {
                    //process[workstationIndex].AddMachine(new Machine(ErlangDistribution.FromExpectedValue(ud.NextValue(), 5), teamMembers[machineIndex]));
                    process[workstationIndex].AddMachine(new Machine(ExponentialDistribution.FromExpectedValue(ud.NextValue(), Seed+workstationIndex), teamMembers[machineIndex]));
                }
            }
        }
    }
}