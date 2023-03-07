using System;
using GeneticAlgorithm;

public class RandomAlgorithm : Model
{
    private Problem Problem;
    private double BestFitness;
    private int[] BestSolution;
    private int NIterations;
    
    public RandomAlgorithm(Problem problem, int nIterations)
    {
        Problem = problem;
        BestFitness = Double.PositiveInfinity;
        BestSolution = null;
        NIterations = nIterations;
    }

    public int[] Run()
    {
        Random Rng = new Random();

        for (int i = 0; i < NIterations; i++)
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