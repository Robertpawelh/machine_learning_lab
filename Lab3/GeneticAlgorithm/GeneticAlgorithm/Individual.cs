using System;
using System.Linq;

namespace GeneticAlgorithm
{
    public class Individual
    {
        public int[] Genotype;
        public double Fitness;

        public Individual()
        {
            Fitness = -1;
        }

        public Individual(int[] genotype)
        {
            Genotype = genotype;
            Fitness = -1;
        }
        
        public Individual(Individual individual)
        {
            Genotype = new int[individual.Genotype.Length];
            individual.Genotype.CopyTo(Genotype, 0);
            Fitness = individual.Fitness;
        }

        public static Individual GenerateRandom(Problem problem, Random rng)
        {
            int[] genotype = new int[2 * (problem.Customers.Length - 1) - 1];
            int[] customersOrder = problem.Customers.Skip(1).Select(x => x.RealId).ToArray();
            
            customersOrder = customersOrder.OrderBy(x => rng.Next()).ToArray();
            
            for (int i = 0; i < genotype.Length; i++)
            {
                if (i % 2 == 1)
                {
                    genotype[i] = rng.Next(-2, 0);
                }
                else
                {
                    genotype[i] = customersOrder[i / 2];
                }
                
            }
            return new Individual(genotype);
        }
    }
}

