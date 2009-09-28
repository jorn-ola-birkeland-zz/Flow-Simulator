namespace MonteCarloFlowTest
{
    public class PooledMachine : Machine
    {
        private readonly ResourcePool _pool;

        public PooledMachine(IProbabilityDistribution distribution, ResourcePool pool) : base(distribution)
        {
            _pool = pool;
        }

        protected override bool CanStart
        {
            get { return _pool.HasResources; }
        }

        protected override void OnJobCompletion()
        {
            _pool.UnlockResource();
        }

        protected override void OnJobStart()
        {
            _pool.LockResource();
        }
    }
}