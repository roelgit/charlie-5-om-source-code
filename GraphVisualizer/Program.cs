using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            IInputLoader l = new TestInput();
            Graph g = l.load(inputFile);
            //Graph g = SubgraphSelector.process(g, "", 0);
            float c_1 = 2.0F;
            float c_2 = 1.0F;
            float c_3 = 1.0F;
            float c_4 = 0.1F;
            Algorithm a = new SimpleAlgorithm(c_1, c_2, c_3, c_4, 100);

            //a.run(g);
            Visualizer v = new Visualizer();

            System.IO.FileInfo fi = new System.IO.FileInfo(outputFile);
            int i = 0;
            a.start(g);
            while (!a.step(g))
            {
                var frame = fi.DirectoryName + @"\" +fi.Name.Replace(fi.Extension, i++ + fi.Extension);
                v.Visualize(g, frame, 300, 300, 10, 10);
            }
            v.Visualize(g, outputFile, 1024, 1024);
            /*for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("-------------");
                a.run(g);
                v.Visualize(g, outputFile);
            }*/

#if DEBUG
            Console.WriteLine("Done");
            System.Console.ReadKey();
#endif
        }
    }
}
