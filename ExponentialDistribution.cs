using System;

namespace DentalQueueSim
{
    /// <summary>Exponential distribution: only a mean is needed; variance = mean^2 by definition.
    /// This is what makes a queue "Markovian" (the M in M/M/1, M/G/1, etc.).</summary>
    public class ExponentialDistribution : Distribution
    {
        private readonly double _mean;

        public ExponentialDistribution(double mean)
        {
            if (mean <= 0) throw new ArgumentException("mean must be positive.");
            _mean = mean;
        }

        public override double Mean => _mean;
        public override double Variance => Math.Pow(_mean, 2);
        public override string Describe() => $"Exponential(mean = {_mean:0.##})";
    }
}
