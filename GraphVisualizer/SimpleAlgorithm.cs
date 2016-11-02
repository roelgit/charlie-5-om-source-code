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
        /// <summary>
        /// Amount of steps before stopping
        /// </summary>
        readonly int M;
        /// <summary>
        /// Keep track of the steps done
        /// </summary>
        private int stepsDone;

        public SimpleAlgorithm(float c_1, float c_2, float c_3, float c_4, int M)
        {
            this.c_1 = c_1;
            this.c_2 = c_2;
            this.c_3 = c_3;
            this.c_4 = c_4;
            this.M = M;
        }

        private float springStrength(float length)
        {
            float strength = (float)(c_1 * Math.Log10(length / c_2));
            return strength;
        }

        private float nodeRepellantForce(Node a, Node b)
        {
            var ls = a.vector_to(b).LengthSquared();
            return ls == 0f ? 1000f : c_3 / ls;
        }

        public override void start(Graph g)
        {
            g.finalize();
            Random rnd = new Random();
            foreach (Node n in g.nodes)
            {
                float x = (float)rnd.NextDouble();
                float y = (float)rnd.NextDouble();
                Vector2 pos = new Vector2(x, y);
                n.position = pos;
            }
        }

        public override bool step(Graph g)
        {
            Dictionary<Edge, float> edge_forces = new Dictionary<Edge, float>();
            Vector2[] node_forces = new Vector2[g.nodes.Count];

            // loop variables
            Vector2 direction;
            Node other;

            Console.WriteLine("Stap {0}: {1} nodes, {2} edges", this.stepsDone, g.nodes.Count, g.edges.Count);

            // calculate the strength of the edges
            for (int i = 0; i < g.edges.Count; i++ ) {
                Edge e = g.edges.ElementAt(i);
                float edgeforce = springStrength(e.Length);

                Console.WriteLine("Adding force {0} to edge [{1};\t{2}]", edgeforce, e.left, e.right);

                edge_forces.Add(e, edgeforce);
            }

            // calculate the forces on the nodes
            for (int i = 0; i < g.nodes.Count; i++) {
                Node n = g.nodes.ElementAt(i);
                Vector2 sum = new Vector2(0,0);
                foreach(var other2 in g.nodes.Where((x)=>(x != n && !n.neighbours().Contains(x))))
                {
                    other = other2;
                    direction = other.vector_to(n);
                    sum += Vector2.Normalize(direction) * nodeRepellantForce(n, other);
                }
                /*for (int j = 0; j < g.nodes.Count; j++ ) {
                    if (i == j) { continue; } // do not calculate force with self
                    other = g.nodes.ElementAt(j);
                    bool is_neighbour = n.neighbours().Contains(other);
                    if (is_neighbour) { continue; } // adjacent nodes do not repel
                    direction = other.vector_to(n);
                    sum += Vector2.Normalize(direction) * nodeRepellantForce(n, other);
                }*/

                // add the edge forces on this node
                foreach (Edge e in n.edges()) {
                    if (e.left == n) { other = e.right; } else { other = e.left; }
                    direction = n.vector_to(other);

                    sum += Vector2.Normalize(direction) * edge_forces[e];
                }

                Console.WriteLine("Adding force {0} to node {1}", sum * c_4, n);

                // scale the final force
                node_forces[i] = sum * c_4;
            }

            // move the nodes
            for (int i = 0; i < g.nodes.Count; i++) {
                Node n = g.nodes.ElementAt(i);
                n.position += node_forces[i];
            }


            Console.WriteLine("Step done");
            //Console.ReadKey();
            return ++stepsDone >= M;
        }
    }
}
