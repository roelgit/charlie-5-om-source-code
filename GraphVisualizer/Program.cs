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
#if !DEBUG

            int testcase = 0;
            if (args.Length >= 1) {
                testcase = Int32.Parse(args[0]);
            }

            switch (testcase)
            {
                case 1:
                    RuntimeVsVertices();
                    break;
                case 2:
                    RuntimeVsIterations();
                    break;
                case 3:
                    EdgeLengthVsC1();
                    break;
                case 4:
                    EdgeLengthVsC3();
                    break;
                default:
                    Console.Error.WriteLine("Warning: no or invalid test case provided, running all test cases");
                    RuntimeVsVertices();
                    RuntimeVsIterations();
                    EdgeLengthVsC1();
                    EdgeLengthVsC3();
                    break;
            }
            return;
#else
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

        /// <summary>
        /// This function has been kept for compatibility. Use private static results runner(string inputFile, string outputFile, Algorithm a) in the future.
        /// </summary>
        private static results runner(string inputFile, string outputFile, float spring_multiplier, float spring_neutral_distance, float repellant_multiplier, float dampening, int max_iterations)
        {
            IInputLoader l = new TestInput();
            Graph g = l.load(inputFile);

            float stabilizationThreshold = 0.1f;

            Algorithm a = new SimpleAlgorithm(spring_multiplier, spring_neutral_distance, repellant_multiplier, dampening, max_iterations, stabilizationThreshold);

            return runner(g, outputFile, a);
        }

        private static results runner(Graph g, string outputFile, Algorithm a)
        {
            Visualizer v = new Visualizer();

            System.IO.FileInfo fi;

            if (outputFile != null)
                fi = new System.IO.FileInfo(outputFile);
            int i = 1;
            a.start(g);
            var start = Process.GetCurrentProcess().TotalProcessorTime;
            while (!a.step(g))
            {
                i++;
            }
            var stop = Process.GetCurrentProcess().TotalProcessorTime;

            if (outputFile != null)
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
                    Console.Write("{0} Iterations: {1}\r", filename, results.iterations);
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

        private static void EdgeLengthVsC1(float spring_neutral_distance = 1.0f, float repellant_multiplier = 1.0f, float dampening = 1.0f)
        {
            int iterations = 5000;

            float precision = 0.01f;
            float minC1 = 0.01f;
            float maxC1 = 1.00f;

            foreach (var filename in TestFilePaths())
            {
                var outputStream = new System.IO.StreamWriter(new System.IO.FileInfo("EdgeLengthVsC1_" + new System.IO.FileInfo(filename).Name + ".csv").OpenWrite());
                outputStream.WriteLine(PrintCSV(new String[] { "C3", "EdgeMean", "EdgeRatio", "EdgeStdDev"}));

                for(float c1 = minC1; c1 <= maxC1; c1+=precision)
                {
                    var results = runner(filename, null, c1, spring_neutral_distance, repellant_multiplier, dampening, iterations);
                    outputStream.WriteLine(PrintCSV(new String[] { c1+"", results.stats.EdgeMean + "", results.stats.EdgeRatio + "", results.stats.EdgeStdDev + ""}));
                }
            }
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
                outputStream.WriteLine(PrintCSV(new String[] { "C3", "EdgeMean", "EdgeRatio", "EdgeStdDev" }));

                for (float c3 = minC3; c3 <= maxC3; c3 += precision)
                {
                    var results = runner(filename, null, spring_multiplier, spring_neutral_distance, c3, dampening, iterations);
                    outputStream.WriteLine(PrintCSV(new String[] { c3 + "", results.stats.EdgeMean + "", results.stats.EdgeRatio + "", results.stats.EdgeStdDev + "" }));
                }
            }
        }

        private static void EadesVsFruchtmanReingold(float spring_multiplier = 1.0f, float spring_neutral_distance = 1.0f, float C = 1f, float repellant_multiplier = 1.0f, float dampening = 1.0f)
        {
            float stabilizationThreshold = 0.1f;
            var fruchmanReingold = new FruchtermanReingoldAlgorithm(spring_multiplier, C, repellant_multiplier, dampening, int.MaxValue, stabilizationThreshold);
            var eades = new SimpleAlgorithm(spring_multiplier, spring_neutral_distance, repellant_multiplier, dampening, int.MaxValue, stabilizationThreshold);

            var outputStream = new System.IO.StreamWriter(new System.IO.FileInfo("Algorithms.csv").OpenWrite());
            outputStream.WriteLine(PrintCSV(new String[] { "Eades", "FruchtermanAndReingold" }));


            IInputLoader l = new TestInput();

            foreach (var filename in TestFilePaths())
            {
                Graph g = l.load(filename);
                var resultsEades = runner(l.load(filename), null, eades);
                var resultsFR = runner(l.load(filename), null, fruchmanReingold);

                outputStream.WriteLine(PrintCSV(new String[] { "" + resultsEades.iterations, "" + resultsFR.iterations }));
            }
            outputStream.Close();
        }
    }
}
