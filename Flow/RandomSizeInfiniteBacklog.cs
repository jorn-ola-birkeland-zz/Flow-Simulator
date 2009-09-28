namespace MonteCarloFlowTest
{
    public class RandomSizeInfiniteBacklog : InfiniteBacklog
    {
        private readonly IProbabilityDistribution _distribution;


        public RandomSizeInfiniteBacklog(IProbabilityDistribution distribution)
        {
            _distribution = distribution;
        }

        protected override void PrepareJob(WorkItem workItem)
        {
            workItem.Size = _distribution.NextValue();
        }  
    }
}