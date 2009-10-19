namespace Flow.ProbabilityDistribution
{
    public interface IProbabilityDistribution
    {
        double NextValue();
        int Seed { get; set;}

        double ExpectedValue { get; }
    }
}