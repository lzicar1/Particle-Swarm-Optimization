using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using XPlot.Plotly;


namespace ParticleSwarm
{


    class Optimizer{
        /*
        Create particle swarm on Rastrigin function
        */

        public static double Parabolla3D(double[] xy){
            /*
            Parabolla function in 3D
            */

            double x = xy[0];
            double y = xy[1];
            double scaling = 3;
            return (x*x + y*y) * scaling;
        }

        public static double Rastrigin3D(double[] xy){
            /*
            Rastrigin function in 3D
            */

            double x = xy[0] - 30;
            double y = xy[1] - 30;
            return 0.2* Math.Pow(x, 2) - 50 * Math.Cos(0.3 * Math.PI * x) + 0.2*Math.Pow(y, 2) - 50 * Math.Cos(0.3 * Math.PI * y) + 20; 
        }

        public static double Saddle3D(double[] xy){
            /*
            Saddle function in 3D
            */

            double x = xy[0];
            double y = xy[1];
            return - x*x + y*y;
        }

        public static double[,] NormalizeArray(double[,] array){
            /*
            Normalize array
            */

            double min = array.Cast<double>().Min();
            double max = array.Cast<double>().Max();
            for (int i = 0; i < array.GetLength(0); i++){
                for (int j = 0; j < array.GetLength(1); j++){
                    array[i, j] = (array[i, j] - min)/(max - min);
                }
            }
            return array;
        }

        public static (List<double> x, List<double> y, List<double> z) GenerateFunctionScatter(Func<double[], double> function, int count, Random rnd)
        {
            /*
            Generate scatter plot of function
            */

            var x = new List<double>();
            var y = new List<double>();
            var z = new List<double>();

            // create evenly spaced grid of x, y coordinates
            for (int i = -count; i < count; i++)
            {
                for (int j = -count; j < count; j++)
                {
                    x.Add(i);
                    y.Add(j);
                    z.Add(function(new double[] { i, j }));
                }
            }

            for (int digit = 0; digit < count; digit++)
            {
                z.Add(function(new double[] { x[digit], y[digit] }));
            }
            return (x, y, z);
        }

        public static double[,] GenerateFunctionSurface(Func<double[], double> function, int count, Random rnd){
            /*
            Generate surface of function
            z is 2D double array
            */
            
            var z = new double[count*2 , count*2];
            for (int i = 0; i < count*2; i++)
            {
                for (int j = 0; j < count*2; j++)
                {
                    z[i, j] = function(new double[] { i, j });
                }
            }

            return z;
        }

        public static Surface ImplicitPlot(Func<double[], double> function, int count, double opacity){
            /*
            Plotting function
            Parameter: 3D function outputting double on z axis for every x and y coordinate
            */

            var rnd = new Random();
            var z = GenerateFunctionSurface(function, count, rnd);

            var trace1 = new Surface()
            {
                z = z,
                opacity = opacity,
            };

            return trace1;
        }

        public static Scatter3d SwarmPlot(Swarm swarm){
            /*
            Plot the swarm
            */

            var x = new double[swarm.particles.Length];
            var y = new double[swarm.particles.Length];
            var z = new double[swarm.particles.Length];

            for (int i = 0; i < swarm.particles.Length; i++){
                x[i] = swarm.particles[i].position[0];
                y[i] = swarm.particles[i].position[1];
                z[i] = swarm.particles[i].value;
            }

            // output scatter3d 
            var scatter3d_trace = new Scatter3d(){
                x = x,
                y = y,
                z = z,
                mode = "markers",
                marker = new Marker()
                {
                    size = 4,
                    color = z,
                }
            };

            return scatter3d_trace;
            
        }

        public static Scatter3d SwarmPositionHistoryPlot(Swarm swarm){
            /*
            Plot the swarm position history lines
            */

            var x = new List<double>();
            var y = new List<double>();
            var z = new List<double>();

            for (int i = 0; i < swarm.particles.Length; i++){
                for (int j = 0; j < swarm.historySize; j++){
                    x.Add(swarm.positionHistory[i, 0, j]);
                    y.Add(swarm.positionHistory[i, 1, j]);
                    z.Add(swarm.positionHistory[i, 2, j]);
                }
            }

            var arrayrange = Enumerable.Range(0, swarm.historySize).ToArray();


            // output scatter3d 
            var scatter3d_lines = new Scatter3d(){
                x = x,
                y = y,
                z = z,
                mode = "markers",
                marker = new Marker()
                {
                    size = 2,
                    color = z, 
                    opacity = 1
                }
            };

            return scatter3d_lines;
            
            
        }

    }

}
