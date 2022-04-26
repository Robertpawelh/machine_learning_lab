using System;
using System.IO;

namespace GeneticAlgorithm
{
    public class DataLoader
    {
        private void SkipLines(StreamReader reader, int n)
        {
            for (var i = 0; i < n; i++)
            {
                reader.ReadLine();
            }
        }
        public Problem LoadProblem(string filepath)
        {
            using (StreamReader reader = File.OpenText(filepath))
            {
                SkipLines(reader, 3);
                string[] line = reader.ReadLine().Split();
                int n_customers = int.Parse(line[2]);
                SkipLines(reader, 1);
                line = reader.ReadLine().Split();
                int capacity = int.Parse(line[2]);
                SkipLines(reader, 1);
                Console.WriteLine($"{n_customers} {capacity}");

                Customer[] customers = new Customer[n_customers];
                //line = reader.ReadLine().Split();
                //Customer vehicle = new Customer(coordinates: (int.Parse(line[1]), int.Parse(line[2]))));

                for (int i = 0; i < n_customers; i++)
                {
                    line = reader.ReadLine().Split();
                    customers[i] = new Customer(realId: int.Parse(line[1]), coordinates: (int.Parse(line[2]), int.Parse(line[3])));
                }

                SkipLines(reader, 1);

                for (int i = 0; i < n_customers; i++)
                {
                    line = reader.ReadLine().Split();
                    customers[i].Demand = int.Parse(line[1]);
                }

                Problem problem = new Problem(capacity: capacity, customers: customers);
                return problem;
            }
        }
    }
}
