namespace Flow
{
    public class WipTokenPool
    {
        private readonly int _tokens;
        private int _availableTokens;


        public WipTokenPool(int tokenCount)
        {
            _tokens = tokenCount;
            _availableTokens = tokenCount;
        }

        public int WipTokens
        {
            get { return _tokens; }
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