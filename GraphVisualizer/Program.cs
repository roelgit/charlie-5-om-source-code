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
            string inputFile = args[0];
            string outputFile = args[1];

            IInputLoader l = new NSInput();
            Graph g = l.load(inputFile);
            Graph sub = SubgraphSelector.process(g, "", 0);
            Algorithm a = new SimpleAlgorithm();

            // Todo: instrument this call
            a.run(sub);

            Visualizer.visualize(sub, outputFile);
        }
    }
}
