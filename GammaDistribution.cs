using System;

namespace DentalQueueSim
{
    /// <summary>Gamma distribution given directly by mean and variance
    /// (e.g. inter-arrival time Gamma(mean=10, variance=20) in Example 3).</summary>
    public class GammaDistribution : Distribution
    {
        private readonly double _mean;
        private readonly double _variance;

        public GammaDistribution(double mean, double variance)
        {
            if (mean <= 0) throw new ArgumentException("mean must be positive.");
            if (variance < 0) throw new ArgumentException("variance cannot be negative.");
            _mean = mean;
            _variance = variance;
        }

        public override double Mean => _mean;
        public override double Variance => _variance;
        public override string Describe() => $"Gamma(mean = {_mean:0.##}, variance = {_variance:0.##})";
    }
}
