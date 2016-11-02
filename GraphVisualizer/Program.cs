using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GraphVisualizer
{
    class Program
    {
        static void Main(string[]args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Please enter more parameters");
                return;
            }

            string inputFile = args[0];
            string outputFile = args[1];

            float spring_multiplier = 1.0F;
            float spring_neutral_distance = 1.0F;
            float repellant_multiplier = 5.0F;
            float dampening = 0.1F;
            int max_iterations = 100000;

            results r = runner(inputFile, outputFile, spring_multiplier, spring_neutral_distance, repellant_multiplier, dampening, max_iterations);
#if DEBUG
            Console.WriteLine("iterations: {0}, Time: {1}, Ratio: {2}, Mean: {3}, StdDev: {4}", r.iterations, r.runtime, r.stats.EdgeRatio, r.stats.EdgeMean, r.stats.EdgeStdDev);
            System.Console.ReadKey();
#endif
        }

        public struct results
        {
            public int iterations;
            public double runtime;
            public GraphStatistics stats;
        }

        private static results runner(string inputFile, string outputFile, float spring_multiplier, float spring_neutral_distance, float repellant_multiplier, float dampening, int max_iterations)
        {
            IInputLoader l = new TestInput();
            Graph g = l.load(inputFile);
            //Graph g = SubgraphSelector.process(g, "", 0);
            Algorithm a = new SimpleAlgorithm(spring_multiplier, spring_neutral_distance, repellant_multiplier, dampening, max_iterations);

            //a.run(g);
            Visualizer v = new Visualizer();

            System.IO.FileInfo fi = new System.IO.FileInfo(outputFile);
            int i = 1;
            a.start(g);
            var start = Process.GetCurrentProcess().TotalProcessorTime;
            while (!a.step(g))
            {
                //var frame = fi.DirectoryName + @"\" + fi.Name.Replace(fi.Extension, i++ + fi.Extension);
                //v.Visualize(g, frame, 300, 300, 10, 10);
                i++;
            }
            var stop = Process.GetCurrentProcess().TotalProcessorTime;
            v.Visualize(g, outputFile, 1024, 1024);

            results r = new results();
            r.iterations = i;
            r.runtime = (stop - start).TotalMilliseconds;
            r.stats = GraphStatistics.From(g);
            return r;
        }
    }
}
