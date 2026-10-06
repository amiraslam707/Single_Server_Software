using System;
using System.Collections.Generic;
using System.Linq;

namespace DentalQueueSim
{
    /// <summary>
    /// Ties together all the observed clinic days, computes the actual (empirical)
    /// queueing statistics from the raw data, and fits lambda / mu so they can be fed
    /// into an MM1QueueModel for a theoretical-vs-actual comparison.
    /// </summary>
    public class ClinicQueueSimulation
    {
        public List<ClinicDay> Days { get; }

        public ClinicQueueSimulation(List<ClinicDay> days)
        {
            Days = days;
        }

        private IEnumerable<Patient> AllPatients => Days.SelectMany(d => d.Patients);

        public double MeanInterArrivalTime()
        {
            var gaps = Days.SelectMany(d => d.InterArrivalTimes()).ToList();
            return gaps.Average();
        }

        public double MeanServiceTime() => AllPatients.Average(p => p.ServiceTime);

        public double ActualMeanWaitInQueue() => AllPatients.Average(p => p.WaitInQueue);
        public double ActualMeanTimeInSystem() => AllPatients.Average(p => p.TimeInSystem);

        public double ActualIdleProportion()
        {
            double totalIdle = Days.Sum(d => d.IdleTime());
            double totalSpan = Days.Sum(d => d.SessionSpan());
            return totalSpan > 0 ? totalIdle / totalSpan : 0;
        }

        /// <summary>Fits an M/M/1 model (both sides exponential) from the observed means.</summary>
        public SingleServerQueueModel FitMM1Model()
        {
            var arrival = new ExponentialDistribution(MeanInterArrivalTime());
            var service = new ExponentialDistribution(MeanServiceTime());
            return new SingleServerQueueModel(arrival, service);
        }

        public void PrintActualSummary()
        {
            Console.WriteLine("\n================ ACTUAL (EMPIRICAL) RESULTS ================");
            Console.WriteLine($"Total patients observed        : {AllPatients.Count()}");
            Console.WriteLine($"Mean inter-arrival time         : {MeanInterArrivalTime():0.00} min");
            Console.WriteLine($"Mean service time                : {MeanServiceTime():0.00} min");
            Console.WriteLine($"Actual mean wait in queue (Wq)  : {ActualMeanWaitInQueue():0.00} min");
            Console.WriteLine($"Actual mean wait in system (W)  : {ActualMeanTimeInSystem():0.00} min");
            Console.WriteLine($"Actual proportion server idle    : {ActualIdleProportion():P1}");
        }

        public void PrintComparison(SingleServerQueueModel model)
        {
            Console.WriteLine("\n================ THEORETICAL vs ACTUAL ================");
            Console.WriteLine($"{"Metric",-28}{"Theoretical (M/M/1)",-22}{"Actual (data)",-15}");
            Console.WriteLine($"{"Wait in queue (Wq, min)",-28}{model.Wq(),-22:0.00}{ActualMeanWaitInQueue(),-15:0.00}");
            Console.WriteLine($"{"Wait in system (W, min)",-28}{model.Ws(),-22:0.00}{ActualMeanTimeInSystem(),-15:0.00}");
            Console.WriteLine($"{"Proportion idle",-28}{model.IdleProportion(),-22:0.00}{ActualIdleProportion(),-15:0.00}");
            if (!model.IsStable)
                Console.WriteLine("\nNote: theoretical column is not meaningful here since rho >= 1 (unstable queue).");
        }
    }
}
