using System;
using System.Linq;

namespace GeneticAlgorithm
{
    public class Problem
    {
        public int Capacity;
        public Customer[] Customers;

        public Problem(int capacity, Customer[] customers)
        {
            Capacity = capacity;
            Customers = customers;
        }
        
        public double EuclideanDistance(Customer customer1, Customer customer2)
        {
            return Math.Sqrt(
                Math.Pow(customer1.Coordinates.Item1 - customer2.Coordinates.Item1, 2)
                + Math.Pow(customer1.Coordinates.Item2 - customer2.Coordinates.Item2, 2));
        }

        public bool IsSolutionCorrect(int [] solution)
        {
            int remainingCapacity = Capacity;
            for (int i = 0; i < solution.Length; i++)
            {
                Customer customer;

                if (i % 2 == 1)
                {
                    if (solution[i] == 0) // return to market
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
                        return false;
                    }
                }
            }

            return true;
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
                    if (solution[i] == 0) // return to market
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
                        fitness += 2 * EuclideanDistance(previousCustomer, Customers[0]);
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