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
        /// <summary>
        /// The amount of runs per test
        /// </summary>
        static int RunsPerTest = 5;

        /// <summary>
        /// The default maximum amount of iterations
        /// </summary>
        static int MaxIterationsPerTest = 15000;

        static void Main(string[]args)
        {

            // Upgrade this thread to maximum priority
            var p = System.Diagnostics.Process.GetCurrentProcess();
            p.PriorityClass = ProcessPriorityClass.RealTime;
            var t = System.Threading.Thread.CurrentThread;
            t.Priority = System.Threading.ThreadPriority.Highest;

            // Only run on processor 1 in a multicore/multiprocessor environment
            p.ProcessorAffinity = (IntPtr) 0x0001;
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
                case 5:
                    EadesVsFruchtmanReingold();
                    break;
                default:
                    Console.Error.WriteLine("Warning: no or invalid test case provided, running all test cases");
                    RuntimeVsVertices();
                    RuntimeVsIterations();
                    EdgeLengthVsC1();
                    EdgeLengthVsC3();
                    EadesVsFruchtmanReingold();
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

            float spring_multiplier = 2.0F;
            float spring_neutral_distance = 1.0F;
            float repellant_multiplier = 1.0F;
            float dampening = 0.1F;
            int max_iterations = 10000;

            results r = runner(inputFile, outputFile, spring_multiplier, spring_neutral_distance, repellant_multiplier, dampening, max_iterations, true);
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
        private static results runner(string inputFile, string outputFile, float spring_multiplier, float spring_neutral_distance, float repellant_multiplier, float dampening, int max_iterations, bool stabilise)
        {
            IInputLoader l;
            var ext = new System.IO.FileInfo(inputFile).Extension;
            if (ext.Equals(".txt"))
            {
                l = new TestInput();
            } else if (ext.Equals(".col"))
            {
                l = new GeneratedSampleInput();
            } else
            {
                l = new TestInput();
                Console.Error.WriteLine("Warning: could not determine input type for extension {0}", ext);
            }
            Graph g = l.load(inputFile);

            float stabilizationThreshold = 0.001f;

            Algorithm a = new EadesAlgorithm(spring_multiplier, spring_neutral_distance, repellant_multiplier, dampening, max_iterations, stabilizationThreshold, stabilise);

            return runner(g, outputFile, a);
        }

        private static results runner(Graph g, string outputFile, Algorithm a)
        {
            Visualizer v = new Visualizer();

            System.IO.FileInfo fi;

            if (outputFile != null)
                fi = new System.IO.FileInfo(outputFile);
            int i = 0;
            a.start(g);
            var start = Process.GetCurrentProcess().TotalProcessorTime;
            var stopwatch = Stopwatch.StartNew();
            while (!a.step(g))
            {
                i++;
            }
            stopwatch.Stop();
            var stop = Process.GetCurrentProcess().TotalProcessorTime;

            if (outputFile != null)
                v.Visualize(g, outputFile, 1024, 1024);

            results r = new results();
            r.iterations = i + 1;
            r.runtime = stopwatch.ElapsedMilliseconds;//(stop - start).TotalMilliseconds;
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
            foreach (var fi in dir.GetFiles("*.txt").Concat(dir.GetFiles("*.col")).OrderBy((x) => (x.Length)))
                names.Add(fi.FullName);

            return names.ToArray();
        }

        private static void RuntimeVsIterations(int MAX_ITERATIONS = 40000, float spring_multiplier = 1.0f, float spring_neutral_distance = 1.0f, float repellant_multiplier = 1.0f, float dampening = 1.0f)
        {
            foreach (var filename in TestFilePaths())
            {
                var outputStream = new System.IO.StreamWriter(new System.IO.FileInfo("RuntimeVsIterations_" + new System.IO.FileInfo(filename).Name + ".csv").OpenWrite());
                outputStream.WriteLine(PrintCSV(new String[] { "Iterations", "Runtime" }));

                var deltaI = 500;

                for (int iteration = deltaI; iteration < MAX_ITERATIONS; iteration += deltaI)
                {
                    //Multiple runs
                    for (var i = 0; i < RunsPerTest; i++)
                    {
                        var results = runner(filename, null, spring_multiplier, spring_neutral_distance, spring_multiplier, dampening, iteration, true);
                        outputStream.WriteLine(PrintCSV(new String[] { "" + results.iterations, "" + results.runtime }));
                        Console.Write("RuntimeVsIterations: {0}({1}) Iterations: {2}\r", filename, i, results.iterations);
                    }
                }
                outputStream.Close();
            }
        }

        private static void RuntimeVsVertices(float spring_multiplier = 1.0f, float spring_neutral_distance = 1.0f, float repellant_multiplier = 1.0f, float dampening = 1.0f)
        {
            var outputStream = new System.IO.StreamWriter(new System.IO.FileInfo("RuntimeVsVertices.csv").OpenWrite());
            outputStream.WriteLine(PrintCSV(new String[] { "Vertices", "Runtime" }));
            foreach (var filename in TestFilePaths())
            {
                // Multiple runs
                for (var i = 0; i < RunsPerTest; i++)
                {
                    Console.Write("RuntimeVsVertices: {0}({1}) Vertices:         \r", filename, i);
                    var results = runner(filename, null, spring_multiplier, spring_neutral_distance, spring_multiplier, dampening, MaxIterationsPerTest, true);
                    outputStream.WriteLine(PrintCSV(new String[] { "" + results.graph.nodes.Count, "" + results.runtime }));
                    Console.Write("RuntimeVsVertices: {0}({1}) Vertices: {2}, Time: {3}ms\n", filename, i, results.graph.nodes.Count, results.runtime);
                }
            }
            outputStream.Close();
        }

        private static void EdgeLengthVsC1(float spring_neutral_distance = 1.0f, float repellant_multiplier = 1.0f, float dampening = 1.0f)
        {
            float precision = 0.01f;
            float minC1 = 0.01f;
            float maxC1 = 1.00f;
            foreach (var filename in TestFilePaths())
            {
                //var outputStream = new System.IO.StreamWriter(new System.IO.FileInfo("EdgeLengthVsC1_" + new System.IO.FileInfo(filename).Name + ".csv").OpenWrite());\
                var outputStream = new System.IO.StreamWriter(new System.IO.FileInfo("EdgeLengthVsC1.csv").OpenWrite());
                outputStream.WriteLine(PrintCSV(new String[] { "C3", "TotalEdgeLength", "EdgeMean", "EdgeRatio", "EdgeStdDev" }));
                for (float c1 = minC1; c1 <= maxC1; c1+=precision)
                {
                    for (var i = 0; i < RunsPerTest; i++)
                    {
                        var results = runner(filename, null, c1, spring_neutral_distance, repellant_multiplier, dampening, MaxIterationsPerTest, true);
                        outputStream.WriteLine(PrintCSV(new String[] { c1 + "", results.stats.EdgeLenghtTotal + "", results.stats.EdgeMean + "", results.stats.EdgeRatio + "", results.stats.EdgeStdDev + "" }));
                        Console.Write("EdgeLengthVsC1: {0}({1}) C1={2}\r", filename, i, c1);
                    }
                }
                outputStream.Close();
            }
        }

        private static void EdgeLengthVsC3(float spring_multiplier = 1.0f, float spring_neutral_distance = 1.0f, float dampening = 1.0f)
        {
            float precision = 0.01f;
            float minC3 = 0.01f;
            float maxC3 = 1.00f;

            foreach (var filename in TestFilePaths())
            {
                //var outputStream = new System.IO.StreamWriter(new System.IO.FileInfo("EdgeLengthVsC3_" + new System.IO.FileInfo(filename).Name + ".csv").OpenWrite());
                var outputStream = new System.IO.StreamWriter(new System.IO.FileInfo("EdgeLengthVsC3.csv").OpenWrite());
                outputStream.WriteLine(PrintCSV(new String[] { "C3", "TotalEdgeLength", "EdgeMean", "EdgeRatio", "EdgeStdDev" }));
                for (float c3 = minC3; c3 <= maxC3; c3 += precision)
                {
                    for (var i = 0; i < RunsPerTest; i++)
                    {
                        var results = runner(filename, null, spring_multiplier, spring_neutral_distance, c3, dampening, MaxIterationsPerTest, true);
                        outputStream.WriteLine(PrintCSV(new String[] { c3 + "", results.stats.EdgeLenghtTotal + "", results.stats.EdgeMean + "", results.stats.EdgeRatio + "", results.stats.EdgeStdDev + "" }));
                        Console.Write("EdgeLengthVsC3: {0}({1}) C3={2}\r", filename, i, c3);
                    }
                }
                outputStream.Close();
            }
        }

        private static void EadesVsFruchtmanReingold(float spring_multiplier = 1.0f, float spring_neutral_distance = 1.0f, float C = 1f, float repellant_multiplier = 1.0f, float dampening = 1.0f)
        {
            float stabilizationThreshold = 0.1f;
            var fruchmanReingold = new FruchtermanReingoldAlgorithm(spring_multiplier, C, repellant_multiplier, dampening, int.MaxValue, stabilizationThreshold);
            var eades = new EadesAlgorithm(spring_multiplier, spring_neutral_distance, repellant_multiplier, dampening, int.MaxValue, stabilizationThreshold, true);

            var outputStream = new System.IO.StreamWriter(new System.IO.FileInfo("Algorithms.csv").OpenWrite());
            outputStream.WriteLine(PrintCSV(new String[] { "Eades", "FruchtermanAndReingold" }));


            IInputLoader l = new TestInput();

            foreach (var filename in TestFilePaths())
            {
                for (var i = 0; i < RunsPerTest; i++)
                {
                    Graph g = l.load(filename);
                    var resultsEades = runner(l.load(filename), null, eades);
                    var resultsFR = runner(l.load(filename), null, fruchmanReingold);

                    outputStream.WriteLine(PrintCSV(new String[] { "" + resultsEades.iterations, "" + resultsFR.iterations }));
                    Console.Write("EadesVsFruchtmanReingold: {0}({1})\r", filename, i);
                }
            }
            outputStream.Close();
        }
    }
}
