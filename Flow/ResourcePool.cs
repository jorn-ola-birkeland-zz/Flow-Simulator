namespace MonteCarloFlowTest
{
    public class ResourcePool
    {
        private int _freeResources;


        public ResourcePool(int numberOfResources)
        {
            _freeResources = numberOfResources;
        }

        public bool HasResources
        {
            get { return _freeResources>0; }
        }

        public void UnlockResource()
        {
            _freeResources++;
        }

        public void LockResource()
        {
            _freeResources--;
        }
    }
}