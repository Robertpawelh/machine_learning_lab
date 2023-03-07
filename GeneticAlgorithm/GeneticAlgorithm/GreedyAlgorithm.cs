using System;
using System.Collections.Generic;
using System.Linq;
using GeneticAlgorithm;
using MoreLinq.Extensions;

public class GreedyAlgorithm : Model
{
    private Problem Problem;
    private double BestFitness;
    private int[] BestSolution;
    private int NIterations;
    
    public GreedyAlgorithm(Problem problem)
    {
        Problem = problem;
        BestFitness = Double.PositiveInfinity;
        BestSolution = null;
    }

    public Customer FindClosestCustomer(Customer previousCustomer, List<Customer> listOfCustomers)
    {
        return listOfCustomers.MinBy(x => Problem.EuclideanDistance(previousCustomer, x)).First();
    }
    
    public Customer FindClosestCustomer(Customer previousCustomer, List<Customer> listOfCustomers, int remainingCapacity)
    {
        return listOfCustomers.MinBy(x => 
            (x.Demand <= remainingCapacity) ? 
                Problem.EuclideanDistance(previousCustomer, x) : 
                Problem.EuclideanDistance(previousCustomer, Problem.Customers[0]) + Problem.EuclideanDistance(Problem.Customers[0], x))
            .First();
    }

    public int GetCapacityBeforeTravel(Customer closestCustomer, int remainingCapacity, ref int[] genotype, ref int geneIndex)
    {
        if (closestCustomer.Demand > remainingCapacity)
        {
            genotype[geneIndex++] = 1;
            return Problem.Capacity;
        }
        else
        {
            genotype[geneIndex++] = 0;
            return remainingCapacity;
        }
    }

    public int[] Run()
    {
        foreach (Customer firstCustomer in Problem.Customers.Skip(1))
        {
            Customer previousCustomer = firstCustomer;
            List<Customer> remainingCustomers = Problem.Customers.Skip(1).ToList().Where(x => x != previousCustomer).ToList();
            int[] genotype = new int[2 * (Problem.Customers.Length - 1) - 1];
            int geneIndex = 0;
            int remainingCapacity = Problem.Capacity - previousCustomer.Demand;
            genotype[geneIndex++] = previousCustomer.RealId;
            
            while (remainingCustomers.Count > 0)
            {
                Customer closestCustomer = FindClosestCustomer(previousCustomer, remainingCustomers);
                //Customer closestCustomer = FindClosestCustomer(previousCustomer, remainingCustomers, remainingCapacity);
                remainingCapacity =
                    GetCapacityBeforeTravel(closestCustomer, remainingCapacity, ref genotype, ref geneIndex);
                remainingCapacity -= closestCustomer.Demand;
                genotype[geneIndex++] = closestCustomer.RealId;

                remainingCustomers.Remove(closestCustomer);
                previousCustomer = closestCustomer;
            }
            
            double fitness = Problem.Fitness(genotype);
            if (fitness < BestFitness)
            {
                BestFitness = fitness;
                BestSolution = new int[genotype.Length];
                genotype.CopyTo(BestSolution, 0);
                //Console.WriteLine($"Fitness: {BestFitness}");
            }
        }
        return BestSolution;
    }
}