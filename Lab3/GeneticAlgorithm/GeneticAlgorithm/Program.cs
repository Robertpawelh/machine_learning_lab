using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// using Plotly.NET;
using XPlot.Plotly;

namespace GeneticAlgorithm
{
    class Program
    {

        static void AlgComparisons(Problem problem)
        {
            GeneticAlgorithm genetic =
                new GeneticAlgorithm(problem: problem, iterations: 300, populationSize: 175, tournamentSize: 128, crossoverProbability: 0.15, mutationProbability: 0.15
                );
        }
        static void Main(string[] args)
        {
            DataLoader dataLoader = new DataLoader();
            //Problem problem = dataLoader.LoadProblem(@"../../../Data/cVRP.txt");
            //Problem problem = dataLoader.LoadProblem(@"../../../Data/A-n32-k5.vrp");
            Problem problem = dataLoader.LoadProblem(@"../../../Data/A-n61-k9.vrp");
            //problem.CalculateDistanceMatrix();
            GeneticAlgorithm genetic =
                new GeneticAlgorithm(problem: problem, iterations: 300, populationSize: 175, tournamentSize: 128, crossoverProbability: 0.15, mutationProbability: 0.15
                );

            RandomAlgorithm random = new RandomAlgorithm(problem);
            int[] solution = genetic.Run();
            genetic.PrintSolution(solution);
            Console.WriteLine($"Best fitness: {problem.Fitness(solution)}");
            Console.WriteLine(solution);
            
            Scatter scatter_i1 = new Scatter() {
                x = Enumerable.Range(0, genetic.AverageFitnesses.Count).ToList().ConvertAll(x => (double) x),
                y = genetic.AverageFitnesses,   //  y = df1.Columns["Ages"],
                mode = "lines+markers",
            };
            
            Scatter scatter_i2 = new Scatter() {
                x = Enumerable.Range(0, genetic.WorstFitnesses.Count).ToList().ConvertAll(x => (double) x),
                y = genetic.WorstFitnesses,   //  y = df1.Columns["Ages"],
                mode = "lines+markers",
            };
            
            Scatter scatter_i3 = new Scatter() {
                x = Enumerable.Range(0, genetic.BestFitnesses.Count).ToList().ConvertAll(x => (double) x),
                y = genetic.BestFitnesses,   //  y = df1.Columns["Ages"],
                mode = "lines+markers",
                name = "Best fitness",
            };
            
            List<Scatter> scatters = new List<Scatter>();
            scatters.Add(scatter_i1); scatters.Add(scatter_i2); scatters.Add(scatter_i3);

            var chart = Chart.Plot(scatters);
            chart.Show();
            
            // GenericChart.GenericChart chart = Chart2D.Chart.Point<double, double, string>(x: Enumerable.Range(0, genetic.BestFitnesses.Count).ToList().ConvertAll(x => (double) x), y: genetic.BestFitnesses);
            // chart
            //     .WithXAxisStyle(title: Title.init("xAxis"), ShowGrid: false, ShowLine: true)
            //     .WithYAxisStyle(title: Title.init("yAxis"), ShowGrid: false, ShowLine: true);
            //
            // GenericChart.GenericChart chart2 = Chart2D.Chart.Point<double, double, string>(x: Enumerable.Range(0, genetic.BestFitnesses.Count).ToList().ConvertAll(x => (double) x), y: genetic.AverageFitnesses);
            // chart
            //     .WithXAxisStyle(title: Title.init("xAxis"), ShowGrid: false, ShowLine: true)
            //     .WithYAxisStyle(title: Title.init("yAxis"), ShowGrid: false, ShowLine: true);
            //
            // chart.Show();
            // chart2.Show();
            
            solution = random.Run();
            // genetic.PrintSolution(solution);
            Console.WriteLine($"Best fitness: {problem.Fitness(solution)}");
            Console.WriteLine(solution);
            
            Console.ReadKey();
            
            // double[] x = new double[] { 1, 2 };
            // double[] y = new double[] { 5, 10 };
            // GenericChart.GenericChart chart = Chart2D.Chart.Point<double, double, string>(x: x, y: y);
            // chart
            //     .WithXAxisStyle(title: Title.init("xAxis"), ShowGrid: false, ShowLine: true)
            //     .WithYAxisStyle(title: Title.init("yAxis"), ShowGrid: false, ShowLine: true)
            //     .Show();
        }
    }
}
