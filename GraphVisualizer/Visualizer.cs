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
            foreach (Node n in g.nodes)
            {
                drawNode(n);
            }

            foreach (Edge e in g.edges)
            {
                drawEdge(e);
            }
            write(path);
        }

        private static void drawNode(Node n)
        {
            throw new NotImplementedException();
        }

        private static void drawEdge(Edge e)
        {
            throw new NotImplementedException();
        }

        // write final document to file
        private static void write(string path)
        {
            throw new NotImplementedException();
        }
    }
}
