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

        protected float dampening;
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

        public FruchtermanReingoldAlgorithm(float spring_multiplier, float C, float repellant_multiplier, float dampening, int M, float stabilizationThreshold)
        {
            this.spring_multiplier = spring_multiplier;
            this.repellant_multiplier = repellant_multiplier;
            this.dampening = dampening;
            this.M = M;
            this.C = C;
            this.stabilizationThreshold = stabilizationThreshold;
        }

        protected float nodeRepellantForce(Node a, Node b, float k)
        {
            return k == 0 ? 1000 : (a.vector_to(b).LengthSquared() / k);
        }

        protected float springStrength(float length, float k)
        {
            return -(k * k) / length;
        }

        protected float calculateArea(Graph g)
        {
            float XMin = 0, XMax = 0, YMin = 0, YMax = 0;

            foreach(Node n in g.nodes)
            {
                if (XMin > n.position.X)
                    XMin = n.position.X;
                if (XMax < n.position.X)
                    XMax = n.position.X;

                if (YMin > n.position.X)
                    YMin = n.position.Y;
                if (YMax < n.position.Y)
                    YMax = n.position.Y;
            }

            return (XMax - XMin) * (YMax - YMin);
        }

        protected float calculateK(Graph g)
        {
            var area = calculateArea(g);
            return C * (float)Math.Sqrt(area / g.nodes.Count);
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

            var k = calculateK(g);

            // calculate the strength of the edges
            for (int i = 0; i < g.edges.Count; i++)
            {
                Edge e = g.edges.ElementAt(i);
                float edgeforce = springStrength(e.Length, k);

                edge_forces.Add(e, edgeforce);
            }

            // calculate the forces on the nodes
            for (int i = 0; i < g.nodes.Count; i++)
            {
                Node n = g.nodes.ElementAt(i);
                Vector2 sum = new Vector2(0, 0);
                foreach (Node other2 in g.nodes.Where((x) => (x != n && !n.neighbours().Contains(x))))
                {
                    direction = other2.direction_to(n);
                    float magnitude = nodeRepellantForce(n, other2, k);
                    sum += direction * magnitude;
                }

                // add the edge forces on this node
                foreach (Edge e in n.edges())
                {
                    if (e.left == n) { other = e.right; } else { other = e.left; }
                    direction = n.direction_to(other);

                    sum += direction * edge_forces[e];
                }

                // scale the final force
                node_forces[i] = sum * dampening;

                // reduce the heat by 50%
                dampening *= 0.975F;
            }

            // Squared because we only need one square root at the end this way, making the calculations faster
            double totalForceLengthSquared = 0d;

            // move the nodes
            for (int i = 0; i < g.nodes.Count; i++)
            {
                Node n = g.nodes.ElementAt(i);
                n.position += node_forces[i];

                totalForceLengthSquared += node_forces[i].LengthSquared();
            }

            return Math.Sqrt(totalForceLengthSquared) < stabilizationThreshold || ++stepsDone >= M;
        }
    }
}
