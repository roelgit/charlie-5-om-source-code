using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphVisualizer
{
    class Node
    {
        public readonly string label;
        public Vector2 position;
        private Graph parent;

        public Node(string label, Graph g)
        {
            this.label = label;
            this.position = new Vector2(0, 0);
            this.parent = g;
        }

        public override bool Equals(object obj)
        {
            return label.Equals(obj);
        }

        public override int GetHashCode()
        {
            return label.GetHashCode();
        }

        public Node[] neighbours()
        {
            return parent.neigbours(this);
        }

        public Edge[] edges()
        {
            return parent.node_edges(this);
        }

        public Vector2 vector_to(Node n)
        {
            // todo: memoize?
            return this.position - n.position;
        }
    }
}
