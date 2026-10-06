using System;
using System.Collections.Generic;
using System.Linq;

namespace DentalQueueSim
{
    /// <summary>
    /// One day's worth of patient visits for the single dentist (single server).
    /// Assumes patients are stored in the order they arrived (FCFS), which matches
    /// the raw data and the clinic's actual discipline.
    /// </summary>
    public class ClinicDay
    {
        public string DayLabel { get; }
        public List<Patient> Patients { get; }

        public ClinicDay(string dayLabel, List<Patient> patients)
        {
            DayLabel = dayLabel;
            Patients = patients.OrderBy(p => p.ArrivalTime).ToList();
        }

        /// <summary>Inter-arrival times (minutes) between consecutive patients on this day.</summary>
        public List<double> InterArrivalTimes()
        {
            var gaps = new List<double>();
            for (int i = 1; i < Patients.Count; i++)
                gaps.Add((Patients[i].ArrivalTime - Patients[i - 1].ArrivalTime).TotalMinutes);
            return gaps;
        }

        public double AverageServiceTime() => Patients.Average(p => p.ServiceTime);
        public double AverageWaitInQueue() => Patients.Average(p => p.WaitInQueue);
        public double AverageTimeInSystem() => Patients.Average(p => p.TimeInSystem);

        /// <summary>
        /// Server idle minutes between the end of one patient's service and the start
        /// of the next patient's service (0 if the next patient was already waiting).
        /// </summary>
        public double IdleTime()
        {
            double idle = 0;
            for (int i = 1; i < Patients.Count; i++)
            {
                var gap = (Patients[i].ServiceStartTime - Patients[i - 1].ServiceEndTime).TotalMinutes;
                if (gap > 0) idle += gap;
            }
            return idle;
        }

        /// <summary>Total clinic session span for the day: first arrival to last service end.</summary>
        public double SessionSpan() =>
            (Patients.Last().ServiceEndTime - Patients.First().ArrivalTime).TotalMinutes;

        public double IdleProportion() => SessionSpan() > 0 ? IdleTime() / SessionSpan() : 0;

        public void PrintReport()
        {
            Console.WriteLine($"\n=== {DayLabel} ({Patients.Count} patients) ===");
            foreach (var p in Patients) Console.WriteLine("  " + p);
            Console.WriteLine($"  -> Avg wait in queue : {AverageWaitInQueue():0.00} min");
            Console.WriteLine($"  -> Avg service time  : {AverageServiceTime():0.00} min");
            Console.WriteLine($"  -> Avg time in system: {AverageTimeInSystem():0.00} min");
            Console.WriteLine($"  -> Server idle time  : {IdleTime():0.00} min " +
                               $"({IdleProportion():P1} of session)");
        }
    }
}
