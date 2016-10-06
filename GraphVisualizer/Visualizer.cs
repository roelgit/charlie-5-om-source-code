using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphVisualizer
{
    static class Visualizer
    {
        public static void visualize(Graph g, string path)
        {
            foreach (Edge e in g.edges)
            {
                drawEdge(e);
            }

            foreach (Node n in g.nodes)
            {
                drawNode(n);
            }
            write(path);
        }

        private static void drawNode(Node n)
        {
            System.Console.WriteLine("Node " + n.label + " at " + n.position.ToString());
            //throw new NotImplementedException();
        }

        private static void drawEdge(Edge e)
        {
            System.Console.WriteLine("Edge from " + e.left.label + " at " + e.left.position.ToString() + " to " + e.right.label + " at " + e.right.position.ToString());
            //throw new NotImplementedException();
        }

        // write final document to file
        private static void write(string path)
        {
            //throw new NotImplementedException();
        }
    }
}
