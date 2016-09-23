using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphVisualizer
{
    class Node
    {
        public readonly string label;
        public float x, y;
        private Graph parent;

        public Node(string label, Graph g)
        {
            this.label = label;
            this.x = this.y = 0;
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
    }
}
