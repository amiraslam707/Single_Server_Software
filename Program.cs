using System;
using System.Collections.Generic;

namespace DentalQueueSim
{
    public class Program
    {
        private static TimeSpan T(int h, int m) => new TimeSpan(h, m, 0);

        public static void Main()
        {
            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n================================================================");
                Console.WriteLine(" SINGLE-SERVER QUEUEING SYSTEM SOFTWARE - Operation Research");
                Console.WriteLine(" DCS-UOK | Group 10 | Supervisor: Dr. Shaista Rais");
                Console.WriteLine("================================================================");
                Console.WriteLine(" 1. Solve a single-server queue (you describe arrival & service)");
                Console.WriteLine(" 2. Run real dataset case study (Wasay Dental Clinic)");
                Console.WriteLine(" 3. Exit");
                Console.Write(" Choose an option: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": RunGeneralModel(); break;
                    case "2": RunCaseStudy(); break;
                    case "3": exit = true; break;
                    default: Console.WriteLine(" Invalid option, please choose 1-3."); break;
                }
            }
        }

        // ---- input helpers -------------------------------------------------

        private static double ReadDouble(string prompt)
        {
            double val;
            Console.Write(prompt);
            while (!double.TryParse(Console.ReadLine(), out val) || val <= 0)
            {
                Console.Write(" Please enter a positive number: ");
            }
            return val;
        }

        private static double ReadNonNegativeDouble(string prompt)
        {
            double val;
            Console.Write(prompt);
            while (!double.TryParse(Console.ReadLine(), out val) || val < 0)
            {
                Console.Write(" Please enter a number >= 0: ");
            }
            return val;
        }

        /// <summary>
        /// Asks how ONE side (arrival or service) of the queue is described, and builds the
        /// matching Distribution. This is what lets the same tool handle Example 1 (exponential
        /// only), Example 2 (a uniform min/max, or a variance directly), and Example 3
        /// (mean + variance from any named distribution) without you needing to pre-guess
        /// which named case (M/M/1, M/G/1, G/G/1...) the question is.
        /// </summary>
        private static Distribution ReadDistribution(string sideLabel)
        {
            Console.WriteLine($"\n How is the {sideLabel} time described?");
            Console.WriteLine("  1. Exponential   - mean only (this is the 'M' / Markovian case)");
            Console.WriteLine("  2. Uniform       - minimum and maximum");
            Console.WriteLine("  3. Normal        - mean and variance");
            Console.WriteLine("  4. Gamma         - mean and variance");
            Console.WriteLine("  5. I just have a mean and a variance (any other distribution)");
            Console.Write(" Choose 1-5: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    {
                        double mean = ReadDouble($" Mean {sideLabel} time (minutes): ");
                        return new ExponentialDistribution(mean);
                    }
                case "2":
                    {
                        double min = ReadDouble($" Minimum {sideLabel} time (minutes): ");
                        double max = ReadDouble($" Maximum {sideLabel} time (minutes): ");
                        return new UniformDistribution(min, max);
                    }
                case "3":
                    {
                        double mean = ReadDouble($" Mean {sideLabel} time (minutes): ");
                        double variance = ReadNonNegativeDouble($" Variance of {sideLabel} time (minutes^2): ");
                        return new NormalDistribution(mean, variance);
                    }
                case "4":
                    {
                        double mean = ReadDouble($" Mean {sideLabel} time (minutes): ");
                        double variance = ReadNonNegativeDouble($" Variance of {sideLabel} time (minutes^2): ");
                        return new GammaDistribution(mean, variance);
                    }
                default:
                    {
                        double mean = ReadDouble($" Mean {sideLabel} time (minutes): ");
                        double variance = ReadNonNegativeDouble($" Variance of {sideLabel} time (minutes^2): ");
                        return new GeneralDistribution(mean, variance);
                    }
            }
        }

        // ---- menu actions ---------------------------------------------------

        private static void RunGeneralModel()
        {
            Console.WriteLine("\n-- General Single-Server Queue --");
            var arrival = ReadDistribution("inter-arrival");
            var service = ReadDistribution("service");
            var model = new SingleServerQueueModel(arrival, service);
            model.PrintReport();
        }

        private static void RunCaseStudy()
        {
            Console.WriteLine("\nWasay Dental Clinic - Gulshan Branch");
            Console.WriteLine("Single-Server (M/M/1) Case Study - Group 10");

            var days = new List<ClinicDay>
            {
                new ClinicDay("Day 1", new List<Patient>
                {
                    new Patient("Ayesha Malik", "Scaling and Polishing",      T(16,8),  T(16,9),  T(16,58)),
                    new Patient("Muhammad Ali", "Routine Dental Check-up",    T(16,42), T(16,58), T(17,35)),
                    new Patient("Hassan Ahmed", "Dental Filling",             T(17,20), T(17,35), T(18,10)),
                }),
                new ClinicDay("Day 2", new List<Patient>
                {
                    new Patient("Ahmed Raza",    "Dental X-ray",                                  T(16,6),  T(16,7),  T(16,55)),
                    new Patient("Bilal Tanveer", "Root Canal",                                     T(16,35), T(16,55), T(18,5)),
                    new Patient("Fatima Noor",   "Dental Crown and Caps",                          T(17,18), T(18,5),  T(18,55)),
                    new Patient("Usman Khosa",   "Teeth Whitening + Scaling and Polishing",         T(18,8),  T(18,55), T(19,35)),
                }),
                new ClinicDay("Day 3", new List<Patient>
                {
                    new Patient("Zainab Bibi",  "Dental Crown and Caps + Root Canal", T(16,12), T(16,13), T(17,37)),
                    new Patient("Mahnoor Khan", "Tooth Scaling and Polishing",        T(16,48), T(17,37), T(18,47)),
                    new Patient("Sana Mir",     "Routine Check-up and Oral Exam",     T(18,6),  T(18,47), T(19,12)),
                    new Patient("Hina Altaf",   "Dental X-ray and Imaging",           T(18,33), T(19,12), T(19,45)),
                }),
                new ClinicDay("Day 4", new List<Patient>
                {
                    new Patient("Tariq Jameel", "Routine Check-up and Oral Exam", T(16,5),  T(16,6),  T(16,21)),
                    new Patient("Hamza Sheikh", "Dental Filling",                 T(16,15), T(16,21), T(16,51)),
                    new Patient("Sara Khan",    "Tooth Scaling and Polishing",    T(16,40), T(16,51), T(17,21)),
                    new Patient("Omer Farooq",  "Dental X-ray and Imaging",       T(17,5),  T(17,21), T(17,33)),
                }),
                new ClinicDay("Day 5", new List<Patient>
                {
                    new Patient("Mariam Asif", "Suture Removal & Post-Op Exam",  T(16,7),  T(16,8),  T(16,20)),
                    new Patient("Bilal Ahmed", "Routine Check-up and Oral Exam", T(16,15), T(16,20), T(16,38)),
                    new Patient("Alizeh Shah", "Tooth Fluoride Treatment",       T(16,30), T(16,38), T(16,50)),
                }),
                new ClinicDay("Day 6", new List<Patient>
                {
                    new Patient("Saad Qureshi", "Dental X-ray and Imaging",    T(16,4),  T(16,5),  T(16,17)),
                    new Patient("Nida Yasir",   "Dental Filling",              T(16,10), T(16,17), T(16,47)),
                    new Patient("Fawad Khan",   "Tooth Scaling and Polishing", T(16,35), T(16,47), T(17,17)),
                }),
            };

            foreach (var day in days) day.PrintReport();

            var sim = new ClinicQueueSimulation(days);
            sim.PrintActualSummary();

            var model = sim.FitMM1Model();
            model.PrintReport();

            sim.PrintComparison(model);
        }
    }
}
