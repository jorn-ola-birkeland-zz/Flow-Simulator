namespace GammaFit
{
    class GammaParameters
    {
        public GammaParameters(double k, double theta)
        {
            this.k = k;
            this.theta = theta;
        }

        public readonly double k;
        public readonly double theta;

        public override string ToString()
        {
            return string.Format("k:{0}, theta:{1}",k,theta);
        }
    }
}