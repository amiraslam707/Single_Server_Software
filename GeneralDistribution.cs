using System;

namespace DentalQueueSim
{
    /// <summary>Fallback for when you're simply told a mean and a variance, with no named
    /// distribution attached (still fully valid -- Marchal's approximation only ever
    /// needs these two numbers from each side anyway).</summary>
    public class GeneralDistribution : Distribution
    {
        private readonly double _mean;
        private readonly double _variance;

        public GeneralDistribution(double mean, double variance)
        {
            if (mean <= 0) throw new ArgumentException("mean must be positive.");
            if (variance < 0) throw new ArgumentException("variance cannot be negative.");
            _mean = mean;
            _variance = variance;
        }

        public override double Mean => _mean;
        public override double Variance => _variance;
        public override string Describe() => $"General(mean = {_mean:0.##}, variance = {_variance:0.##})";
    }
}
