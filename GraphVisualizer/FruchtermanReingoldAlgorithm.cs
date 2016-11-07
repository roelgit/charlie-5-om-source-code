using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GraphVisualizer
{
    class FruchtermanReingoldAlgorithm : Algorithm
    {
        protected readonly float spring_multiplier, repellant_multiplier;

        protected float heat;
        /// <summary>
        /// Amount of steps before stopping
        /// </summary>
        protected readonly int M;
        /// <summary>
        /// Keep track of the steps done
        /// </summary>
        protected int stepsDone;
        /// <summary>
        /// Constant to determine k
        /// </summary>
        private float C;
        /// <summary>
        /// The maximum amount of movement before a graph can be seen as stable
        /// </summary>
        protected float stabilizationThreshold;
        /// <summary>
        /// Whether or not to use the stabiliser
        /// </summary>
        protected readonly bool UseStabiliser;

        protected float k;

        protected float frameSize;

        protected float initialHeat;

        public FruchtermanReingoldAlgorithm(float spring_multiplier, float C, float repellant_multiplier, int M, float stabilizationThreshold, bool useStabilizer)
        {
            this.spring_multiplier = spring_multiplier;
            this.repellant_multiplier = repellant_multiplier;
            this.M = M;
            this.C = C;
            this.stabilizationThreshold = stabilizationThreshold;
            this.UseStabiliser = useStabilizer;
            this.frameSize = 25.0f;
            this.initialHeat = this.frameSize * 0.1f;
            this.heat = this.initialHeat;
        }

        protected float springStrength(float length)
        {
            return (length * length) / k;
        }

        protected float nodeRepellantForce(Node a, Node b)
        {
            return (k * k) / a.vector_to(b).Length();
        }

        public override void start(Graph g)
        {
            g.finalize();
            Random rnd = new Random();
            foreach (Node n in g.nodes)
            {
                float x = (float)rnd.NextDouble() * (frameSize / 2);
                float y = (float)rnd.NextDouble() * (frameSize / 2);
                Vector2 pos = new Vector2(x, y);
                n.position = pos;
            }
            this.k = C * (float)Math.Sqrt((frameSize * frameSize) / g.nodes.Count);
        }

        public override bool step(Graph g)
        {
            Dictionary<Edge, float> edge_forces = new Dictionary<Edge, float>();
            Vector2[] node_forces = new Vector2[g.nodes.Count];

            // loop variables
            Vector2 direction;
            Node other;

            // calculate the strength of the edges
            for (int i = 0; i < g.edges.Count; i++)
            {
                Edge e = g.edges.ElementAt(i);
                float edgeforce = springStrength(e.Length);

                edge_forces.Add(e, edgeforce);
            }

            // calculate the forces on the nodes
            for (int i = 0; i < g.nodes.Count; i++)
            {
                Node n = g.nodes.ElementAt(i);
                Vector2 sum = new Vector2(0, 0);
                foreach (Node other2 in g.nodes.Where((x) => (x != n)))
                {
                    direction = other2.direction_to(n);
                    float magnitude = nodeRepellantForce(n, other2);
                    sum += direction * magnitude;
                }

                // add the edge forces on this node
                foreach (Edge e in n.edges())
                {
                    if (e.left == n) { other = e.right; } else { other = e.left; }
                    direction = n.direction_to(other);

                    sum += direction * edge_forces[e];
                }

                // limit the final force
                if (sum.Length() > heat)
                {
                    sum = Vector2.Normalize(sum) * heat;
                }
                node_forces[i] = sum;
            }

            // reduce the heat
            heat -= (this.initialHeat / M);
            Console.WriteLine("Heat: " + heat.ToString());

            // Squared because we only need one square root at the end this way, making the calculations faster
            double totalForceLengthSquared = 0d;

            // move the nodes
            for (int i = 0; i < g.nodes.Count; i++)
            {
                Node n = g.nodes.ElementAt(i);
                n.position += node_forces[i];

                // restrain position to frame
                if (n.position.X < -frameSize)
                {
                    n.position.X = -frameSize;
                }
                else if (n.position.X > frameSize)
                {
                    n.position.X = frameSize;
                }

                if (n.position.Y < -frameSize)
                {
                    n.position.Y = -frameSize;
                }
                else if (n.position.Y > frameSize)
                {
                    n.position.Y = frameSize;
                }

                totalForceLengthSquared += node_forces[i].LengthSquared();
            }
            return (Math.Sqrt(totalForceLengthSquared) < stabilizationThreshold && UseStabiliser) || ++stepsDone >= M;
        }
    }
}
