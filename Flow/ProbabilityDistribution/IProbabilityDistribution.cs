namespace MonteCarloFlowTest
{
    public interface IProbabilityDistribution
    {
        double NextValue();
        int Seed { get; set;}
    }
}