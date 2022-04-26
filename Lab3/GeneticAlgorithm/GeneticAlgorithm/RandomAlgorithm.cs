using System;

namespace GeneticAlgorithm
{
    public class RandomAlgorithm
    {
        private Problem Problem;
        private double BestFitness;
        private int[] BestSolution;

        public RandomAlgorithm(Problem problem)
        {
            Problem = problem;
            BestFitness = Double.PositiveInfinity;
            BestSolution = null;
        }

        public int[] Run()
        {
            Random Rng = new Random();

            for (int i = 0; i < 1_000_000; i++)
            {
                Individual solution = Individual.GenerateRandom(Problem, Rng);
                double fitness = Problem.Fitness(solution.Genotype);

                if (fitness < BestFitness)
                {
                    BestFitness = fitness;
                    BestSolution = new int[solution.Genotype.Length];
                    solution.Genotype.CopyTo(BestSolution, 0);
                    Console.WriteLine($"Fitness: {BestFitness}");
                }
            }

            return BestSolution;
        }
    }
}