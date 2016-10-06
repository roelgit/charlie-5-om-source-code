using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphVisualizer
{
    class SimpleAlgorithm : Algorithm
    {
        readonly float c_1, c_2, c_3, c_4;

        public SimpleAlgorithm(float c_1, float c_2, float c_3, float c_4)
        {
            this.c_1 = c_1;
            this.c_2 = c_2;
            this.c_3 = c_3;
            this.c_4 = c_4;
        }

        private float springStrength(float length)
        {
            return (float)(c_1 * Math.Log10(length / c_2));
        }

        private float nodeRepellantForce(Node a, Node b)
        {
            return c_3 / a.vector_to(b).LengthSquared();
        }

        public override bool step(Graph g)
        {
            Dictionary<Edge, float> edge_forces = new Dictionary<Edge, float>();
            Vector2[] node_forces = new Vector2[g.nodes.Count];

            // loop variables
            Vector2 direction;
            Node other;

            // calculate the strength of the edges
            Edge[] graph_edges = g.edges.ToArray();
            for (int i = 0; i < graph_edges.Length; i++ ) {
                Edge e = graph_edges[i];
                float edgeforce = springStrength(g.edges.ElementAt(i).Length);
                edge_forces.Add(e, edgeforce);
            }

            // calculate the forces on the nodes
            for (int i = 0; i < g.nodes.Count; i++) {
                Node n = g.nodes.ElementAt(i);
                Vector2 sum = new Vector2(0,0);
                for (int j = 0; j < g.nodes.Count; j++ ) {
                    other = g.nodes.ElementAt(j);
                    bool is_neighbour = n.neighbours().Contains(other);
                    if (is_neighbour) { continue; } // adjacent nodes do not repel
                    direction = other.vector_to(n);
                    sum += direction * nodeRepellantForce(n, other);
                }

                // add the edge forces on this node
                foreach (Edge e in n.edges()) {
                    if (e.left == n) { other = e.right; } else { other = e.left; }
                    direction = n.vector_to(other);
                    sum += direction * edge_forces[e];
                }

                // scale the final force
                node_forces[i] = sum * c_4;
            }

            // move the nodes
            for (int i = 0; i < g.nodes.Count; i++) {
                Node n = g.nodes.ElementAt(i);
                n.position += node_forces[i];
            }
            return true; // ignored for now
        }
    }
}
