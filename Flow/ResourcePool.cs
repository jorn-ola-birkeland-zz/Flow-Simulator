using System;

namespace Flow
{
    public class ResourcePool
    {
        private int _freeResources;
        private readonly int _totalResources;


        public ResourcePool(int numberOfResources) : this(numberOfResources, Guid.NewGuid().ToString())
        {
        }

        public ResourcePool(int numberOfResources, string name)
        {
            _totalResources = numberOfResources;
            _freeResources = numberOfResources;
            Name = name;
        }

        public string Name
        {
            get;
            set;
        }
        
        public bool HasResources
        {
            get { return _freeResources>0; }
        }

        public int ResourceCount
        {
            get { return _totalResources; }
        }

        public double Utilization
        {
            get
            {
                return ((double) (_totalResources - _freeResources))/_totalResources;
            }
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