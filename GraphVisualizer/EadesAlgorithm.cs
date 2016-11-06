using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace GraphVisualizer
{
    class EadesAlgorithm : Algorithm
    {
        protected readonly float spring_multiplier, spring_neutral_distance, repellant_multiplier, dampening;
        /// <summary>
        /// Amount of steps before stopping
        /// </summary>
        protected readonly int M;
        /// <summary>
        /// Keep track of the steps done
        /// </summary>
        protected int stepsDone;
        /// <summary>
        /// The maximum amount of movement before a graph can be seen as stable
        /// </summary>
        protected float stabilizationThreshold;

        public EadesAlgorithm(float spring_multiplier, float spring_neutral_distance, float repellant_multiplier, float dampening, int M, float stabilizationThreshold)
        {
            this.spring_multiplier = spring_multiplier;
            this.spring_neutral_distance = spring_neutral_distance;
            this.repellant_multiplier = repellant_multiplier;
            this.dampening = dampening;
            this.M = M;
            this.stabilizationThreshold = stabilizationThreshold;
        }

        protected virtual float springStrength(float length)
        {
            float strength = (float)(spring_multiplier * Math.Log10(length / spring_neutral_distance));
            return strength;
        }

        protected virtual float nodeRepellantForce(Node a, Node b)
        {
            var ls = a.vector_to(b).LengthSquared();
            return repellant_multiplier / ls;
        }

        public override void start(Graph g)
        {
            g.finalize();
            Random rnd = new Random();
            foreach (Node n in g.nodes)
            {
                float x = (float)rnd.NextDouble() * 25;
                float y = (float)rnd.NextDouble() * 25;
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

            // calculate the strength of the edges
            for (int i = 0; i < g.edges.Count; i++ ) {
                Edge e = g.edges.ElementAt(i);
                float edgeforce = springStrength(e.Length);

                edge_forces.Add(e, edgeforce);
            }

            // calculate the forces on the nodes
            for (int i = 0; i < g.nodes.Count; i++) {
                Node n = g.nodes.ElementAt(i);
                Vector2 sum = new Vector2(0,0);
                foreach(Node other2 in g.nodes.Where((x)=>(x != n && !n.neighbours().Contains(x))))
                {
                    direction = other2.direction_to(n);
                    float magnitude = nodeRepellantForce(n, other2);
                    sum += direction * magnitude;
                }

                // add the edge forces on this node
                foreach (Edge e in n.edges()) {
                    if (e.left == n) { other = e.right; } else { other = e.left; }
                    direction = n.direction_to(other);

                    sum += direction * edge_forces[e];
                }

                // scale the final force
                node_forces[i] = sum * dampening;
            }

            // Squared because we only need one square root at the end this way, making the calculations faster
            double totalForceLengthSquared = 0d;

            // move the nodes
            for (int i = 0; i < g.nodes.Count; i++) {
                Node n = g.nodes.ElementAt(i);
                n.position += node_forces[i];

                totalForceLengthSquared += node_forces[i].LengthSquared();
            }

            return (Math.Sqrt(totalForceLengthSquared) < stabilizationThreshold && false) ||  ++stepsDone >= M;
        }
    }
}
