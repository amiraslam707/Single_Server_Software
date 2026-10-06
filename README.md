# Single-Server Queueing System Software

A C# console application that solves single-server queueing problems (M/M/1, M/G/1, G/G/1, and G/M/1) using object-oriented design, and validates the model against real appointment data from a dental clinic.

> Built for an Operations Research course (DCS-UOK) — Group 10, Supervisor: Dr. Shaista Rais.

---

## What this is

Classic queueing-theory coursework usually gives you three separate formulas depending on the case:

| Case | Meaning | Lq formula |
|---|---|---|
| **M/M/1** | Exponential arrivals, exponential service | `Lq = ρ² / (1 − ρ)` |
| **M/G/1** | Exponential arrivals, any service distribution | `Lq = (λ²σs² + ρ²) / (2(1 − ρ))` |
| **G/G/1** | Any arrivals, any service distribution | Marchal's approximation |

Instead of hard-coding three separate solvers, this project uses **one general model** built around [Marchal's approximation](#why-one-formula-covers-every-case), which is proven (not just assumed) to collapse into the exact M/M/1 and M/G/1 formulas as special cases. You just describe the arrival process and the service process — the software works out which named case it is and computes everything else.

It's also tested on a **real dataset**: 21 patient visits across 6 days at a single-dentist clinic, used to fit a model from actual data and compare it against the theoretical prediction.

---

## Features

- Solve any single-server queue by describing arrivals and service independently as:
  - **Exponential** (mean only)
  - **Uniform** (min, max)
  - **Normal** (mean, variance)
  - **Gamma** (mean, variance)
  - **General** (just a mean and variance, no named distribution)
- Auto-detects and labels the result as M/M/1, M/G/1, G/M/1, or G/G/1
- Prints every formula alongside its computed value (for clarity, not just raw numbers)
- Flags unstable queues (ρ ≥ 1) instead of silently printing meaningless negative results
- Includes a full real-world case study (dental clinic data) comparing theoretical vs. actual queue behavior

---

## Project structure

```
DentalQueueSim/
├── Program.cs                     # Console menu / entry point
├── Distribution.cs                # Abstract base: Mean, Variance, Describe()
├── ExponentialDistribution.cs     # Mean only; Variance = Mean²
├── UniformDistribution.cs         # Built from (min, max)
├── NormalDistribution.cs          # Built from (mean, variance)
├── GammaDistribution.cs           # Built from (mean, variance)
├── GeneralDistribution.cs         # Fallback: raw (mean, variance)
├── SingleServerQueueModel.cs      # Core model: takes 2 Distributions, computes everything
├── Patient.cs                     # One real clinic visit (arrival/start/end timestamps)
├── ClinicDay.cs                   # Groups a day's patients, computes per-day stats
├── ClinicQueueSimulation.cs       # Aggregates all days, fits a model from real data
└── DentalQueueSim.csproj          # Project file (.NET)
```

---

## How it works (architecture)

Two class hierarchies work together:

**1. `Distribution` (Strategy pattern)** — an abstract class with five concrete implementations. Each one only needs to know how to report its own `Mean` and `Variance`:

```
Distribution (abstract)
 ├── ExponentialDistribution(mean)
 ├── UniformDistribution(min, max)
 ├── NormalDistribution(mean, variance)
 ├── GammaDistribution(mean, variance)
 └── GeneralDistribution(mean, variance)
```

**2. `SingleServerQueueModel`** — takes one `Distribution` for arrivals and one for service. It never needs to know *which* subclass it was given; it just calls `.Mean` and `.Variance` on each and computes:

- `λ = 1 / arrival.Mean`, `μ = 1 / service.Mean`, `ρ = λ/μ`
- `Ca² = arrival.Variance / arrival.Mean²`, `Cs² = service.Variance / service.Mean²`
- `ModelName` — auto-labeled M/M/1 / M/G/1 / G/M/1 / G/G/1 based on whether `Ca²`/`Cs²` equal 1 (exponential always has a squared coefficient of variation of exactly 1)
- `Lq()`, `Wq()`, `Ws()`, `Ls()`, idle proportion — all from one formula (see below)

That's real polymorphism doing work, not just existing for show — the model class is completely agnostic to how each side's distribution was described.

---

## Why one formula covers every case

Marchal's approximation:

```
Lq = ρ²(1+Cs²)(Ca²+ρ²Cs²) / [2(1−ρ)(1+ρ²Cs²)]
```

**Set Ca² = Cs² = 1** (both exponential) — the `(1+ρ²)` factor cancels top and bottom:

```
Lq = ρ² / (1 − ρ)      ← exactly the M/M/1 formula
```

**Set only Ca² = 1** (exponential arrivals, general service) — the `(1+ρ²Cs²)` factor cancels, and since `Cs² = σs²μ²`, the term `ρ²Cs²` simplifies to `λ²σs²`:

```
Lq = (λ²σs² + ρ²) / (2(1 − ρ))      ← exactly the M/G/1 formula
```

So the software isn't approximating the simpler cases — it produces their *exact* textbook formulas as special cases. This also means it correctly handles combinations the course notes don't explicitly cover, like G/M/1 (variance given on the arrival side instead of the service side).

---

## Getting started

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download) (8.0 or later — adjust `TargetFramework` in the `.csproj` to match whatever SDK you have installed)

### Run it

```bash
cd DentalQueueSim
dotnet restore
dotnet run
```

You'll get a menu:

```
1. Solve a single-server queue (you describe arrival & service)
2. Run real dataset case study (Wasay Dental Clinic)
3. Exit
```

**Option 1** asks you to describe the arrival side and service side separately — pick a distribution type and enter the relevant numbers (mean, or min/max, or mean+variance). It then prints the auto-detected model name, every formula used, and the final results (Lq, Wq, W, L, idle proportion).

**Option 2** runs the full case study on real clinic data (see below).

---

## Verification against textbook examples

Running the software with each example's inputs reproduces the published results exactly:

| Case | Ca² | Cs² | Lq | Wq (min) | W (min) | L |
|---|---|---|---|---|---|---|
| Example 1 (M/M/1): mean inter-arrival 10, mean service 8 | 1.0000 | 1.0000 | 3.200 | 32.00 | 40.00 | 4.000 |
| Example 2 (M/G/1): mean inter-arrival 10, service Uniform(7,9) | 1.0000 | 0.0052 | 1.608 | 16.08 | 24.08 | 2.408 |
| Example 3 (G/G/1): arrival Gamma(mean 10, var 20), service Normal(mean 8, var 25) | 0.2000 | 0.3906 | 0.801 | 8.01 | 16.01 | 1.601 |

---

## Case study: Wasay Dental Clinic

21 patient visits across 6 days, one dentist acting as the single server, served strictly first-come-first-served. For each patient: arrival time, service-start time, and service-end time were recorded.

Fitting an M/M/1 model from the observed data:

| Parameter | Value |
|---|---|
| Mean inter-arrival time | 29.93 min |
| Mean service time | 35.33 min |
| λ | 0.0334 patients/min |
| μ | 0.0283 patients/min |
| **ρ (traffic intensity)** | **1.18** |

Since ρ > 1, the theoretical formulas return negative (non-physical) results — correctly signalling that no steady state exists for this arrival pattern. This isn't a bug: it's the formulas doing their job. In the real, finite-length clinic sessions, this shows up not as infinite queue growth but as **steadily worsening wait times within each session** (e.g. climbing from 1 to 49 minutes across Day 3) and **0% server idle time** once the first patient arrives — exactly what the raw data shows.

| Metric | Theoretical (M/M/1) | Actual (observed) |
|---|---|---|
| Wait in queue (Wq) | −231.19 min *(invalid)* | 16.43 min |
| Wait in system (W) | −195.86 min *(invalid)* | 51.76 min |
| Proportion idle | −0.18 *(invalid)* | 0.0% |

---

## Notes / known limitations

- Marchal's formula is an **approximation** for the true G/G/1 and G/M/1 cases (it is *exact* only for M/M/1 and M/G/1) — standard in queueing theory when a closed-form solution doesn't exist.
- The model name is auto-detected using a small floating-point tolerance (`Ca²`/`Cs²` within `1e-6` of 1 counts as exponential).
- The dental clinic case study pools all 6 days into one λ/μ estimate; per-day traffic intensity could individually be more or less stable and isn't broken out separately in the current version.

---

## Credits

Operation Research, DCS-UOK — Group 10, Supervisor: Dr. Shaista Rais.