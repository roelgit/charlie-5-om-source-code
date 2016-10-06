using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphVisualizer
{
    class Edge
    {
        public readonly Node left, right;
        private readonly string representation;

        public Edge(Node left, Node right)
        {
            if (left == right) { throw new Exception("edge from node to same node is not allowed"); }

            this.left = left;
            this.right = right;
            if (left.label.CompareTo(right.label) < 0)
            {
                representation = left.label + "-" + right.label;
            } else {
                representation = right.label + "-" + left.label;
            }
        }

        public float Length
        {
            get
            {
                return (right.position - left.position).Length();
            }
        }
    }
}
