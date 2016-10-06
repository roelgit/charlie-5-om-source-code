using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GraphVisualizer
{
    class TestInput : IInputLoader
    {
        public Graph load(string filename)
        {
            bool nodes_finished = false;
            Graph g = new Graph();
            IEnumerable<string> lines = File.ReadLines(filename);
            foreach (string s in lines)
            {
                string[] parts = s.Split(':');
                if (parts[0] == "N")
                {
                    if (nodes_finished)
                    {
                        throw new Exception("You can't do this");
                    }
                    g.addNode(parts[1]);
                }
                else if (parts[0] == "E")
                {
                    nodes_finished = true;
                    string[] parts2 = parts[1].Split('-');
                    g.addEdge(parts2[0], parts2[1]);
                }
            }
            return g;
        }
    }
}
