using System;

namespace DentalQueueSim
{
    /// <summary>
    /// Represents any distribution used for inter-arrival or service times.
    /// All the queueing formulas only ever need two numbers from a distribution:
    /// its Mean and its Variance. This is the Strategy pattern -- SingleServerQueueModel
    /// doesn't care HOW a distribution is described (Exponential, Uniform, Normal, ...),
    /// only that it can report Mean and Variance.
    /// </summary>
    public abstract class Distribution
    {
        public abstract double Mean { get; }
        public abstract double Variance { get; }
        public abstract string Describe();
    }
}
