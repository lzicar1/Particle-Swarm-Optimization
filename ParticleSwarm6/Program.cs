using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using XPlot.Plotly;
using System.CommandLine;
// using CommandLine.Invocation;
using System.Threading.Tasks;

namespace ParticleSwarm
{
    class Program
    {

        public static XPlot.Plotly.PlotlyChart PlotTraces(List<Trace> traces, string plotName)
        {
            /*
            Plotting function for list of traces
            */

            var chart = Chart.Plot(traces);
            chart.WithWidth(800);
            chart.WithHeight(800);
            chart.WithTitle(plotName);
            return chart;
        }

        static async Task<int> Main(string[] args)
        {
            RootCommand rootCommand = new RootCommand(
                description: "Particle swarm optimization of a function.\nOutputs global minimum and optionally visualises the process in the browser."
            );
            var functionOption = new Option<string>(
                aliases: new string[] { "--function", "-f" },
                description: "The function to optimize. Options are: 'rastrigin', 'parabolla', 'saddle'.",
                getDefaultValue: () => "rastrigin"
            );
            rootCommand.AddOption(functionOption);
            var iterationOption = new Option<int>(
                aliases: new string[] { "--iteration", "-i" },
                description: "The target number of iterations.",
                getDefaultValue: () => 100
            );
            rootCommand.AddOption(iterationOption);
            var particleOption = new Option<int>(
                aliases: new string[] { "--particle", "-p" },
                description: "The number of particles in the swarm.",
                getDefaultValue: () => 30
            );
            rootCommand.AddOption(particleOption);
            var verbosityOption = new Option<bool>(
                aliases: new string[] { "--verbosity", "-v" },
                description: "The verbosity level. Options are: 'True', 'False', .",
                getDefaultValue: () => false
            );
            rootCommand.AddOption(verbosityOption);
            var plotOption = new Option<bool>(
                aliases: new string[] { "--plot", "-t" },
                description: "Whether to plot the optimization process. Options are: 'True', 'False', .",
                getDefaultValue: () => false
            );
            rootCommand.AddOption(plotOption);
            var plotHistoryOption = new Option<bool>(
                aliases: new string[] { "--plot-history", "-h" },
                description: "Whether to plot history traces. Options are: 'True', 'False', .",
                getDefaultValue: () => false
            );
            rootCommand.AddOption(plotHistoryOption);
            var numPlotsOption = new Option<int>(
                aliases: new string[] { "--num-plots", "-n" },
                description: "The number of plots to show.",
                getDefaultValue: () => 5
            );
            rootCommand.AddOption(numPlotsOption);
            rootCommand.SetHandler(
                    (function, iteration, particle, verbosity, plot, history, numPlots) => {
                        Optimize(function, iteration, particle, verbosity, plot, history, numPlots);
                    },
                    functionOption,
                    iterationOption,
                    particleOption,
                    verbosityOption,
                    plotOption,
                    plotHistoryOption,
                    numPlotsOption
                );
            return await rootCommand.InvokeAsync(args);


        }

        public static void Optimize(
            string function,
            int iteration,
            int particle,
            bool verbosity,
            bool plot,
            bool history,
            int numPlots
        )
        {
            /*
            Main function for optimization
            */

            // Initialize the swarm
            Console.WriteLine("Initializing swarm of " + particle + " particles");
            var lowerBounds = new double[] { 0, 0 };
            var upperBounds = new double[] { 60, 60 };
            Random random = new Random();

            Swarm swarm = new Swarm(particle, 2, lowerBounds, upperBounds, random);

            // Initialize the function
            Console.WriteLine("Initializing " + function + " function");
            Func<double[], double> optimizationFunction;
            XPlot.Plotly.PlotlyChart chart;


            if (function == "rastrigin")
            {
                optimizationFunction = Optimizer.Rastrigin3D;
            }
            else if (function == "parabolla")
            {
                optimizationFunction = Optimizer.Parabolla3D;
            }
            else if (function == "saddle")
            {
                optimizationFunction = Optimizer.Saddle3D;
            }
            else
            {
                Console.WriteLine("Function not recognized. Options are: 'rastrigin', 'parabolla', 'saddle'.");
                return;
            }

            int plotIteration = iteration / numPlots;

            // Plot input function 
            if (plot == true){
                var function_trace = Optimizer.ImplicitPlot(optimizationFunction, 30, 1);
                chart = PlotTraces(new List<Trace> {function_trace},
                                                "Input function: " + function); 
                chart.Show();
            }

            // Optimalization Process
            Console.WriteLine("Optimization start...");
            for (int i = 0; i < iteration; i++){

                swarm.UpdateValue(optimizationFunction);
                swarm.UpdateBest();
                swarm.UpdateVelocity(0.5, 0.01, 0.03, random);
                swarm.UpdatePosition();

                if (history == true){
                    swarm.UpdateHistory();
                }

                if (verbosity == true){
                    Console.WriteLine("Iteration: " + i);
                    Console.WriteLine("Best value: " + swarm.bestValue);
                    Console.WriteLine("Best position: " + swarm.bestPosition[0] + ", " + swarm.bestPosition[1]);
                }

                if (plot == true){
                    if (i % plotIteration == 0){
                        var function_trace = Optimizer.ImplicitPlot(optimizationFunction, 30, 0.1);
                        var swarm_trace = Optimizer.SwarmPlot(swarm);
                        if (history == true){
                            var swarm_position_history_trace = Optimizer.SwarmPositionHistoryPlot(swarm);
                            chart = PlotTraces(new List<Trace> {function_trace, swarm_trace, swarm_position_history_trace}, "Iteration: " + i + " Best value: " + swarm.bestValue); 
                        }else{
                            chart = PlotTraces(new List<Trace> {function_trace, swarm_trace},"Iteration: " + i + " Best value: " + swarm.bestValue); 
                        }
                        chart.Show();
                    }
                }
            }

            
        }
    }
}
