using System;

namespace DentalQueueSim
{
    /// <summary>Normal distribution given directly by mean and variance
    /// (e.g. service time Normal(mean=8, variance=25) in Example 3).</summary>
    public class NormalDistribution : Distribution
    {
        private readonly double _mean;
        private readonly double _variance;

        public NormalDistribution(double mean, double variance)
        {
            if (mean <= 0) throw new ArgumentException("mean must be positive.");
            if (variance < 0) throw new ArgumentException("variance cannot be negative.");
            _mean = mean;
            _variance = variance;
        }

        public override double Mean => _mean;
        public override double Variance => _variance;
        public override string Describe() => $"Normal(mean = {_mean:0.##}, variance = {_variance:0.##})";
    }
}
