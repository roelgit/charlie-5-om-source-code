using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphVisualizer
{
    class GeneratedSampleInput : IInputLoader
    {
        public Graph load(string filename)
        {
            Graph g = new Graph();
            IEnumerable<string> lines = File.ReadLines(filename);

            var nodes = new List<int>();
            var edges = new Dictionary<int, List<int>>();

            foreach (var line in lines)
            {
                var parts = line.Split();
                switch (parts[0][0])
                {
                    case 'c':
                        // Comment
                        break;
                    case 'e':
                        // Edge
                        int node1 = int.Parse(parts[1]);
                        int node2 = int.Parse(parts[2]);

                        if (!nodes.Contains(node1))
                            nodes.Add(node1);
                        if (!nodes.Contains(node2))
                            nodes.Add(node2);

                        if (!edges.ContainsKey(node1))
                        {
                            edges.Add(node1, new List<int>());
                        }
                        edges[node1].Add(node2);
                        break;
                    case 'p':
                        // Parameter
                        switch(parts[1])
                        {
                            case "edge":
                                var amountOfNodes = parts[2];
                                var amountOfEdges = parts[3];
                                break;
                            default:
                                Console.Error.WriteLine("Invalid parameter {0}", parts[1]);
                                break;
                        }
                        break;
                    default:
                        Console.Error.WriteLine("Invalid operation {1}", parts[0][0]);
                        break;
                }
            }
            foreach (var node in nodes)
            {
                g.addNode(node.ToString());
            }

            foreach(var edgeList in edges)
            {
                foreach(var edge in edgeList.Value)
                {
                    g.addEdge(edgeList.Key.ToString(), edge.ToString());
                }
            }

            return g;
        }
    }
}
