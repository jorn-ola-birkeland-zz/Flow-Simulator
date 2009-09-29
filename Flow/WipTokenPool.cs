namespace MonteCarloFlowTest
{
    public class WipTokenPool
    {
        private int _availableTokens;


        public WipTokenPool(int wipLimit)
        {
            _availableTokens = wipLimit;
        }

        public bool LockWipToken()
        {
            if(_availableTokens>0)
            {
                _availableTokens--;
                return true;
            }

            return false;
        }

        public void UnlockWipToken()
        {
            _availableTokens++;
        }
    }
}