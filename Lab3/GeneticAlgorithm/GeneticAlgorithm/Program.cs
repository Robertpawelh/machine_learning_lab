﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
// using Plotly.NET;
using XPlot.Plotly;

namespace GeneticAlgorithm
{
    class Program
    {

        public class Result
        {
            public String modelName { get; set; }
            public double paramValue { get; set; }
            public double solutionValue { get; set; }
        }
        static void AlgComparisons(Problem problem)
        {
            GeneticAlgorithm genetic =
                new GeneticAlgorithm(problem: problem, iterations: 300, populationSize: 175, tournamentSize: 128, crossoverProbability: 0.15, mutationProbability: 0.15
                );
        }
        
        public static void SaveToCsv(List<Result> results, string name, string path)
        {
            // by: https://code-maze.com/csharp-writing-csv-file/

            Console.WriteLine("\nSaving: " + path + name + ".csv");
            
            bool append = true;
            var config = new CsvConfiguration(CultureInfo.InvariantCulture);
            config.HasHeaderRecord = !append;
            
            using (var writer = new StreamWriter(path + name + ".csv", append))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(results);
            }
        }

        public static void Experiment(int n_samples, int iterations, string testName, Dictionary<String, Dictionary<String, Double>> models)
        {
            DataLoader dataLoader = new DataLoader();
            Dictionary<String, String> problems = new Dictionary<string, string>()
            {
                {"A-n46-k7", @"../../../Data/A-n46-k7.vrp"},
                {"A-n32-k5", @"../../../Data/A-n32-k5.vrp"},
                {"A-n61-k9", @"../../../Data/A-n61-k9.vrp"}
            };
            
            foreach(KeyValuePair<string, string> problemParams in problems)
            {
                Problem problem = dataLoader.LoadProblem(problemParams.Value);
                
                List<double> bestValues = new List<double>();
                double[] bestFitnesses = new double[iterations];
                double[] averageFitnesses = new double[iterations];
                double[] worstFitnesses = new double[iterations];


                foreach (KeyValuePair<string, Dictionary<String, double>> modelParams in models)
                {
                    for (int i = 0; i < n_samples; i++)
                    {
                        GeneticAlgorithm genetic =
                            new GeneticAlgorithm(problem: problem,
                                iterations: iterations,
                                populationSize: (int) modelParams.Value["PopulationSize"], 
                                tournamentSize: (int) modelParams.Value["TournamentSize"],
                                crossoverProbability: modelParams.Value["CrossoverProbability"], 
                                mutationProbability: modelParams.Value["MutationProbability"]
                            );

                        int[] solution = genetic.Run();

                        bestFitnesses = bestFitnesses.Zip(genetic.BestFitnesses, (x, y) => x + y).ToArray();
                        averageFitnesses = averageFitnesses.Zip(genetic.AverageFitnesses, (x, y) => x + y)
                            .ToArray();
                        worstFitnesses = worstFitnesses.Zip(genetic.WorstFitnesses, (x, y) => x + y)
                            .ToArray();

                        bestValues.Add(problem.Fitness(solution));
                        Console.WriteLine($"Best fitness: {problem.Fitness(solution)}");
                    }

                    List<Result> results = new List<Result>();

                    results.Add(
                        new Result
                        {
                            modelName = modelParams.Key,
                            paramValue = modelParams.Value[testName],
                            solutionValue = bestValues.Average()
                        }
                    );

                    Scatter scatter_i1 = new Scatter()
                    {
                        x = Enumerable.Range(0, averageFitnesses.Length).ToList().ConvertAll(x => (double) x),
                        y = averageFitnesses, //  y = df1.Columns["Ages"],
                        mode = "lines+markers",
                        name = "Average fitness",
                    };

                    Scatter scatter_i2 = new Scatter()
                    {
                        x = Enumerable.Range(0, worstFitnesses.Length).ToList().ConvertAll(x => (double) x),
                        y = worstFitnesses, //  y = df1.Columns["Ages"],
                        mode = "lines+markers",
                        name = "Worst fitness (but viable)",
                    };

                    Scatter scatter_i3 = new Scatter()
                    {
                        x = Enumerable.Range(0, bestFitnesses.Length).ToList().ConvertAll(x => (double) x),
                        y = bestFitnesses, //  y = df1.Columns["Ages"],
                        mode = "lines+markers",
                        name = "Best fitness",
                    };

                    List<Scatter> scatters = new List<Scatter>();
                    scatters.Add(scatter_i1);
                    scatters.Add(scatter_i2);
                    scatters.Add(scatter_i3);

                    var chart = Chart.Plot(scatters);
                    //chart.Show();
                    var chartHtml = chart.GetHtml();
                    System.IO.File.WriteAllText($"../../Results/{problemParams.Key}_{testName}_{modelParams.Key}_plot_results.html",
                        chartHtml);
                    SaveToCsv(results, $"{problemParams.Key}_{testName}_results", "../../Results/");
                }
            }
        }
        
        
        
        public static void Experiment_1(int n_samples, string testName, int iterations, Dictionary<String, Double> geneticModel)
        {
            DataLoader dataLoader = new DataLoader();
            Dictionary<String, String> problems = new Dictionary<string, string>()
            {
                {"A-n46-k7", @"../../../Data/A-n46-k7.vrp"},
                {"A-n32-k5", @"../../../Data/A-n32-k5.vrp"},
                {"A-n61-k9", @"../../../Data/A-n61-k9.vrp"}
            };
            
            foreach(KeyValuePair<string, string> problemParams in problems)
            {
                Problem problem = dataLoader.LoadProblem(problemParams.Value);
                
                List<double> bestValues = new List<double>();
                
                GeneticAlgorithm genetic =
                    new GeneticAlgorithm(problem: problem,
                        iterations: iterations,
                        populationSize: (int) geneticModel["PopulationSize"], 
                        tournamentSize: (int) geneticModel["TournamentSize"],
                        crossoverProbability: geneticModel["CrossoverProbability"], 
                        mutationProbability: geneticModel["MutationProbability"]
                    );
                
                Dictionary<string, Model> models = new Dictionary<string, Model>()
                {
                    {"genetic", genetic},
                    {"greedy", new GreedyAlgorithm(problem)},
                    {"randomSearch", new RandomAlgorithm(problem, (int) geneticModel["PopulationSize"] * iterations)}
                };

                foreach (KeyValuePair<string, Model> modelParams in models)
                {
                    for (int i = 0; i < n_samples; i++)
                    {
                        int[] solution = modelParams.Value.Run();
                        bestValues.Add(problem.Fitness(solution));
                        Console.WriteLine($"Best fitness for {modelParams.Key}: {problem.Fitness(solution)}");
                    }

                    List<Result> results = new List<Result>();

                    results.Add(
                        new Result
                        {
                            modelName = modelParams.Key,
                            paramValue = 2137,
                            solutionValue = bestValues.Average()
                        }
                    );
                    
                    SaveToCsv(results, $"{problemParams.Key}_{testName}_results", "../../Results/");
                }
            }
        }
        
        public static void Experiment_2(int n_samples, int iterations)
        {
            Dictionary<String, Dictionary<String, double>>
                models = new Dictionary<string, Dictionary<string, double>>();
            
            List<double> parameters = Enumerable.Range(0, 10).Select(x => (double)x/10).ToList();
            foreach (double cross in parameters)
            {
                foreach (double mutat in parameters)
                {
                    models[$"Cross_mutat_{cross}_{mutat}"] = new Dictionary<string, double>
                    {
                        {"PopulationSize", 100},
                        {"TournamentSize", 32},
                        {"CrossoverProbability", cross},
                        {"MutationProbability", mutat},
                    };
                }
            }
            Experiment(n_samples, iterations, "CrossoverMutationProbability", models);
        }
        
        public static void Experiment_4(int n_samples, int iterations)
        {
            Dictionary<String, Dictionary<String, double>> models = new Dictionary<string, Dictionary<string, double>>()
            {
                {
                    "Pop_100", new Dictionary<string, double>
                    {
                        {"PopulationSize", 100},
                        {"TournamentSize", 32},
                        {"CrossoverProbability", 0.5},
                        {"MutationProbability", 0.1},
                    }
                },
                {
                    "Pop_200", new Dictionary<string, double>
                    {
                        {"PopulationSize", 200},
                        {"TournamentSize", 32},
                        {"CrossoverProbability", 0.5},
                        {"MutationProbability", 0.1},
                    }
                },
                {
                    "Pop_500", new Dictionary<string, double>
                    {
                        {"PopulationSize", 500},
                        {"TournamentSize", 32},
                        {"CrossoverProbability", 0.5},
                        {"MutationProbability", 0.1},
                    }
                },
                {
                    "Pop_1000", new Dictionary<string, double>
                    {
                        {"PopulationSize", 1000},
                        {"TournamentSize", 32},
                        {"CrossoverProbability", 0.5},
                        {"MutationProbability", 0.1},
                    }
                }
            };
            
            Experiment(n_samples, iterations, "PopulationSize", models);
        }
        
        static void Main(string[] args)
        {
            const int iterations = 100;
            const int n_samples = 10;
            //problem.CalculateDistanceMatrix();
            //RandomAlgorithm random = new RandomAlgorithm(problem, 175*300);
            //GreedyAlgorithm greedy = new GreedyAlgorithm(problem);
            Dictionary<string, double> geneticModel = new Dictionary<string, double>()
            {
                {"PopulationSize", 200},
                {"TournamentSize", 32},
                {"CrossoverProbability", 0.5},
                {"MutationProbability", 0.1},
            };

            Experiment_1(n_samples, "AlgsComparison", iterations, geneticModel);
            Experiment_2(n_samples, iterations);
            Experiment_4(n_samples, iterations);
            
            Console.ReadKey();
        }
    }
}
