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

        public Node[] neigbours(Node n)
        {
            List<Node> tmp = new List<Node>();
            foreach (Edge e in edges)
            {
                if (e.left == n)
                {
                    tmp.Add(e.right);
                }
                else if (e.right == n)
                {
                    tmp.Add(e.left);
                }
            }
            return tmp.ToArray();
        }

        public Edge[] node_edges(Node n)
        {
            List<Edge> tmp = new List<Edge>();
            foreach (Edge e in edges)
            {
                if (e.left == n || e.right == n)
                {
                    tmp.Add(e);
                }
            }
            return tmp.ToArray();
        }

        // call this method when you are done adding nodes and edges.
        // it will turn the temporary data into a more efficient one
        public void finalize()
        {
            finalized = true;
        }
    }
}
