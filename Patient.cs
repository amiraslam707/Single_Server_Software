using System;

namespace DentalQueueSim
{
    /// <summary>
    /// Represents a single patient visit record as captured in the raw data
    /// (Day, Name, Service Type, Arrival Time, Service Start Time, Service End Time).
    /// All times are stored as TimeSpan (time-of-day) for easy arithmetic.
    /// </summary>
    public class Patient
    {
        public string Name { get; }
        public string ServiceType { get; }
        public TimeSpan ArrivalTime { get; }
        public TimeSpan ServiceStartTime { get; }
        public TimeSpan ServiceEndTime { get; }

        public Patient(string name, string serviceType, TimeSpan arrivalTime,
                        TimeSpan serviceStartTime, TimeSpan serviceEndTime)
        {
            Name = name;
            ServiceType = serviceType;
            ArrivalTime = arrivalTime;
            ServiceStartTime = serviceStartTime;
            ServiceEndTime = serviceEndTime;
        }

        /// <summary>Time this patient spent waiting in the queue before being served (minutes).</summary>
        public double WaitInQueue => (ServiceStartTime - ArrivalTime).TotalMinutes;

        /// <summary>Actual service (chair) time for this patient (minutes).</summary>
        public double ServiceTime => (ServiceEndTime - ServiceStartTime).TotalMinutes;

        /// <summary>Total time this patient spent in the system: queue + service (minutes).</summary>
        public double TimeInSystem => (ServiceEndTime - ArrivalTime).TotalMinutes;

        public override string ToString() =>
            $"{Name,-18} Arrival={ArrivalTime:hh\\:mm} Start={ServiceStartTime:hh\\:mm} " +
            $"End={ServiceEndTime:hh\\:mm}  WaitQ={WaitInQueue,5:0} min  Svc={ServiceTime,5:0} min  " +
            $"InSystem={TimeInSystem,5:0} min";
    }
}
