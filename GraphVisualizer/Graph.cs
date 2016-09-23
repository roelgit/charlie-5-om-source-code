using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphVisualizer
{
    class Graph
    {
        public readonly List<Node> nodes;
        public readonly List<Edge> edges;
        private bool finalized = false;

        public Graph()
        {
            nodes = new List<Node>();
            edges = new List<Edge>();
        }

        public void addNode(string label)
        {
            if (finalized) { throw new Exception("Cannot add a node to a finalized graph");  }
            //Node n = new Node(label, this);
            //nodes.Add(n); // should throw exception if node is already in set?
        }

        public void addEdge(string left, string right)
        {
            if (finalized) { throw new Exception("Cannot add an edge to a finalized graph"); }
            // todo: check if edge OR its reverse already exists in edge list
            // todo: check if nodes exist
        }

        // call this method when you are done adding nodes and edges.
        // it will turn the temporary data into a more efficient one
        public void finalize()
        {
            finalized = true;
        }
    }
}
