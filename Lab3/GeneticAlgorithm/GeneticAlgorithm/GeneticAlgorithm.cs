using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticAlgorithm
{
    public class GeneticAlgorithm
    {
        private Problem Problem;
        private readonly int PopulationSize;
        private List<Individual> Population;
        private double BestFitness;
        private int[] BestSolution;

        private double CrossoverProbability;
        private double MutationProbability;
        private int TournamentSize;

        private readonly int MaxIterations;
        private Random Rng;

        public List<double> BestFitnesses;
        public List<double> WorstFitnesses;
        public List<double> AverageFitnesses;

        public GeneticAlgorithm(Problem problem, int iterations, int populationSize, int tournamentSize, double crossoverProbability, double mutationProbability)
        {
            Rng = new Random();
            
            Problem = problem;
            MaxIterations = iterations;
            PopulationSize = populationSize;
            TournamentSize = tournamentSize;
            CrossoverProbability = crossoverProbability;
            MutationProbability = mutationProbability;

            Population = new List<Individual>();
            BestFitness = Double.PositiveInfinity;
            BestSolution = null;

            BestFitnesses = new List<double>();
            WorstFitnesses = new List<double>();
            AverageFitnesses = new List<double>();
        }

        private void InitPopulation()
        {
            for (int i = 0; i < PopulationSize; i++)
            {
                Population.Add(Individual.GenerateRandom(Problem, Rng));
            }
        }
        
        public double Fitness(Individual individual)
        {
            double fitness = 0;
            
            if (individual.Fitness == -1)
            {
                fitness = Problem.Fitness(individual.Genotype);
            }
            else
            {
                fitness = individual.Fitness;
            }

            return fitness;
        }

        public void Fitness(List<Individual> Population)
        {
            foreach (Individual individual in Population)
            {
                individual.Fitness = Fitness(individual);
            }
        }

        private bool StopCondition(int n_iteration)
        {
            return n_iteration > MaxIterations;
        }

        private Individual Tournament()
        {
            return Population.OrderBy(x => Rng.Next()).Take(TournamentSize).OrderBy(x => x.Fitness).First();
        }

        private Individual Selection()
        {
            return Tournament();
        }

        private Individual Crossover(Individual parent1, Individual parent2)
        {
            int[] newGenotype = new int[parent1.Genotype.Length];
            for (int i = 1; i < parent1.Genotype.Length; i += 2)
            {
                if (i % 4 == 1)
                {
                    newGenotype[i] = parent1.Genotype[i];
                }
                else
                {
                    newGenotype[i] = parent2.Genotype[i];
                }
            }

            int[] helper = parent1.Genotype.Where(x => x >= 0).ToArray();
            List<int> missingGenes = new List<int>(helper); // WERYFIKACJA - KOPIA TU JEST?
            
            int gene = missingGenes[Rng.Next(missingGenes.Count)];
            while (missingGenes.Count > newGenotype.Length)
            {
                missingGenes.Remove(gene);
                int geneId = Array.FindIndex(parent1.Genotype, x => x == gene);
                if (geneId < 0 || geneId > newGenotype.Length)
                newGenotype[geneId] = gene;
                int equivalentGene = parent2.Genotype[geneId];
                if (!missingGenes.Contains(equivalentGene))
                {
                    gene = missingGenes[Rng.Next(missingGenes.Count)];
                }
                else
                {
                    gene = equivalentGene;
                }
            }
            
            gene = missingGenes[Rng.Next(missingGenes.Count)];
            while (missingGenes.Count > 0)
            {
                missingGenes.Remove(gene);
                int geneId = Array.FindIndex(parent2.Genotype, x => x == gene);
                // Console.WriteLine($"{parent1.Genotype.Length} {parent2.Genotype.Length} {geneId}");
                //
                // Console.WriteLine("New");
                // foreach (var VARIABLE in parent2.Genotype)
                // {
                //     Console.Write($"{VARIABLE},");
                // }
                // Console.WriteLine("EndNew");
                newGenotype[geneId] = gene;
                int equivalentGene = parent1.Genotype[geneId];
                if (!missingGenes.Contains(equivalentGene) && missingGenes.Count > 0)
                {
                    gene = missingGenes[Rng.Next(missingGenes.Count)];
                }
                else
                {
                    gene = equivalentGene;
                }
            }

            // foreach (var VARIABLE in newGenotype)
            // {
            //     Console.Write($"{VARIABLE},");
            // }
            //Console.WriteLine();
            return new Individual(newGenotype); //TODO;
        }

        private void Mutation(Individual individual)
        {
            if (MutationProbability > Rng.NextDouble())
            {
                int geneId1;
                int geneId2;
                if (Rng.NextDouble() > 0.5)
                {
                    geneId1 = Rng.Next(individual.Genotype.Length/2) * 2;
                    geneId2 = Rng.Next(individual.Genotype.Length/2) * 2;
                    (individual.Genotype[geneId1], individual.Genotype[geneId2]) = (individual.Genotype[geneId2], individual.Genotype[geneId1]);
                }
                if (Rng.NextDouble() > 0.5)
                {
                    geneId1 = Rng.Next(individual.Genotype.Length/2) * 2 + 1;
                    geneId2 = Rng.Next(individual.Genotype.Length/2) * 2 + 1;
                    (individual.Genotype[geneId1], individual.Genotype[geneId2]) = (individual.Genotype[geneId2], individual.Genotype[geneId1]);
                }
                
            }
        }
        
        public void PrintSolution(int[] solution)
        {
            int routeId = 1;
            Console.Write($"Route #{routeId}:");
            for (int i = 0; i < solution.Length; i++)
            {
                if (i % 2 == 1)
                {
                    if (solution[i] == -1)
                    {
                        routeId += 1;
                        Console.WriteLine();
                        Console.Write($"Route #{routeId}:");
                    }
                }
                else
                {
                    Console.Write($" {solution[i]}");
                }
            }
            Console.WriteLine();
        }

        public void EvaluatePopulation(List<Individual> population)
        {
            BestFitnesses.Add(population.Min(x => x.Fitness));
            WorstFitnesses.Add(population.Max(x => x.Fitness));
            AverageFitnesses.Add(population.Average(x => x.Fitness));
        }
        public int[] Run()
        {
            int iteration = 0;
            InitPopulation();
            EvaluatePopulation(Population);
            Fitness(Population);

            while (!StopCondition(iteration))
            {
                if (iteration%25 == 0) Console.WriteLine(iteration);
                List<Individual> newPopulation = new List<Individual>();
                while (newPopulation.Count != PopulationSize)
                {
                    Individual parent1 = Selection(); // index to make it faster?
                    Individual child = null;
                    if (CrossoverProbability > Rng.NextDouble()) // Random 
                    {
                        Individual parent2 = Selection();
                        child = Crossover(parent1, parent2);
                    }
                    else
                    {
                        child = new Individual(parent1);
                    }

                    Mutation(child);
                    child.Fitness = Fitness(child);
                    newPopulation.Add(child);
                    // Console.WriteLine($"{child.Fitness}, {BestFitness}");
                    if (child.Fitness < BestFitness)
                    {
                        BestFitness = child.Fitness;
                        BestSolution = new int[child.Genotype.Length];
                        child.Genotype.CopyTo(BestSolution, 0);
                        //PrintSolution(BestSolution);
                        Console.WriteLine($"Fitness: {BestFitness}");
                    }
                }
                
                Population = newPopulation;
                EvaluatePopulation(Population);
                iteration += 1;
            }

            return BestSolution;
        }
    }
}


// NOTKA: NA KONCU NIE MAM ZJAZDU DO MAGAZYNU