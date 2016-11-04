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
            RuntimeVsVertices();
            RuntimeVsIterations();
            return;
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
            public Graph graph;
        }

        private static results runner(string inputFile, string outputFile, float spring_multiplier, float spring_neutral_distance, float repellant_multiplier, float dampening, int max_iterations)
        {
            IInputLoader l = new TestInput();
            Graph g = l.load(inputFile);
            //Graph g = SubgraphSelector.process(g, "", 0);
            Algorithm a = new SimpleAlgorithm(spring_multiplier, spring_neutral_distance, repellant_multiplier, dampening, max_iterations);

            //a.run(g);
            Visualizer v = new Visualizer();

            System.IO.FileInfo fi;

            if (outputFile != null)
                fi= new System.IO.FileInfo(outputFile);
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

            if (outputFile!=null)
                v.Visualize(g, outputFile, 1024, 1024);

            results r = new results();
            r.iterations = i;
            r.runtime = (stop - start).TotalMilliseconds;
            r.stats = GraphStatistics.From(g);
            r.graph = g;
            return r;
        }

        private static String PrintCSV(String[] fields)
        {
            StringBuilder sb = new StringBuilder();
            foreach(var s in fields)
            {
                sb.Append(s);
                sb.Append(";");
            }

            return sb.ToString();
        }


        private static String[] TestFilePaths()
        {
            var dir = new System.IO.DirectoryInfo("../../../data");
            var names = new List<String>();
            foreach (var fi in dir.GetFiles("*.txt"))
                names.Add(fi.FullName);

            return names.ToArray();
        }

        private static void RuntimeVsIterations(int MAX_ITERATIONS = 40000, float spring_multiplier = 1.0f, float spring_neutral_distance = 1.0f, float repellant_multiplier = 1.0f, float dampening = 1.0f)
        {
            foreach (var filename in TestFilePaths())
            {
                var outputStream = new System.IO.StreamWriter(new System.IO.FileInfo("RuntimeVsIterations_" + new System.IO.FileInfo(filename).Name + ".csv").OpenWrite());
                outputStream.WriteLine(PrintCSV(new String[] { "Iterations", "Runtime" }));
                for (int iterations = 1; iterations < MAX_ITERATIONS; iterations += 1000)
                {
                    var results = runner(filename, null, spring_multiplier, spring_neutral_distance, spring_multiplier, dampening, iterations);
                    outputStream.WriteLine(PrintCSV(new String[] { "" + iterations, "" + results.runtime }));
                    Console.Write("{0} Iterations: {1}\r", filename, iterations);
                }
                outputStream.Close();
            }
        }

        private static void RuntimeVsVertices(float spring_multiplier = 1.0f, float spring_neutral_distance = 1.0f, float repellant_multiplier = 1.0f, float dampening = 1.0f)
        {
            int iterations = 5000;

            var outputStream = new System.IO.StreamWriter(new System.IO.FileInfo("RuntimeVsVertices.csv").OpenWrite());
            outputStream.WriteLine(PrintCSV(new String[] { "Vertices", "Runtime" }));
            foreach (var filename in TestFilePaths())
            {
                var results = runner(filename, null, spring_multiplier, spring_neutral_distance, spring_multiplier, dampening, iterations);
                outputStream.WriteLine(PrintCSV(new String[] { "" + results.graph.nodes.Count, "" + results.runtime }));
                Console.Write("{0} Vertices: {1}\r", filename, results.graph.nodes.Count);
            }
            outputStream.Close();
        }

        private static void EdgeLengthVsC3(float spring_multiplier = 1.0f, float spring_neutral_distance = 1.0f, float dampening = 1.0f)
        {
            int iterations = 5000;

            float precision = 0.01f;
            float minC3 = 0.01f;
            float maxC3 = 1.00f;

            foreach (var filename in TestFilePaths())
            {
                var outputStream = new System.IO.StreamWriter(new System.IO.FileInfo("EdgeLengthVsC3_" + new System.IO.FileInfo(filename).Name + ".csv").OpenWrite());
                outputStream.WriteLine(PrintCSV(new String[] { "EdgeLength", "C3" }));

                for(float c3 = minC3; c3 <= maxC3; c3+=precision)
                {
                    var results = runner(filename, null, spring_multiplier, spring_neutral_distance, c3, dampening, iterations);
                    outputStream.WriteLine(PrintCSV(new String[] { "" + results.graph.nodes.Count, "" + results.runtime }));
                    Console.Write("{0} C3: {1}\r", filename, results.graph.nodes.Count);
                }
            }
        }
    }
}
