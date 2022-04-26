using System;
using System.Linq;

namespace GeneticAlgorithm
{
    public class Problem
    {
        public int Capacity;
        public Customer[] Customers;
        public double[][] DistanceMatrix;
        private double InvalidSolutionPenalty = 1000;

        public Problem(int capacity, Customer[] customers)
        {
            Capacity = capacity;
            Customers = customers;
        }
        
        private double EuclideanDistance(Customer customer1, Customer customer2)
        {
            return Math.Sqrt(
                Math.Pow(customer1.Coordinates.Item1 - customer2.Coordinates.Item1, 2)
                + Math.Pow(customer1.Coordinates.Item2 - customer2.Coordinates.Item2, 2));
        }

        public void CalculateDistanceMatrix()
        {
            for (int i = 0; i < Customers.Length; i++)
            {
                for (int j = 0; j < Customers.Length; j++)
                {
                    DistanceMatrix[i][j] = EuclideanDistance(Customers[i], Customers[j]);
                }
            }
        }
        
        public double Fitness(int[] solution)
        {
            double fitness = 0;
            Customer previousCustomer = Customers[0];
            int remainingCapacity = Capacity;
            for (int i = 0; i < solution.Length; i++)
            {
                Customer customer;
                
                if (i % 2 == 1)
                {
                    if (solution[i] == -1)
                    {
                        customer = Customers[0];
                        remainingCapacity = Capacity;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    customer = Customers.First(x => x.RealId == solution[i]);
                    if (remainingCapacity >= customer.Demand)
                    {
                        remainingCapacity -= customer.Demand;
                    }
                    else
                    {
                        fitness += InvalidSolutionPenalty;
                    }
                }
                
                fitness += EuclideanDistance(previousCustomer, customer);
                previousCustomer = customer;
            }
            
            if (previousCustomer != Customers[0])
            {
                fitness += EuclideanDistance(previousCustomer, Customers[0]);
            }

            return fitness;
        }
    }
}