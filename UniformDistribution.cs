using System;

namespace DentalQueueSim
{
    /// <summary>Uniform distribution given by min and max (e.g. service time Uniform(7,9)
    /// in Example 2) -- mean and variance are derived, not entered by hand.</summary>
    public class UniformDistribution : Distribution
    {
        public double Min { get; }
        public double Max { get; }

        public UniformDistribution(double min, double max)
        {
            if (max <= min) throw new ArgumentException("max must be greater than min.");
            Min = min;
            Max = max;
        }

        public override double Mean => (Min + Max) / 2.0;
        public override double Variance => Math.Pow(Max - Min, 2) / 12.0;
        public override string Describe() => $"Uniform(min = {Min:0.##}, max = {Max:0.##})";
    }
}
