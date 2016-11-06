using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphVisualizer
{
    internal class GraphStatistics
    {
        public float Height { get; protected set; }

        public float Width { get; protected set; }

        public RectangleF GraphArea { get; protected set; }

        public float EdgeRatio { get; protected set; }

        public float EdgeMean { get; protected set; }

        public float EdgeStdDev { get; protected set; }

        public double EdgeLenghtTotal { get; protected set; }

        /// <summary>
        /// A protected constructor. Don't use this unless you are extending this class. Use GraphStatistics.From instead
        /// </summary>
        protected GraphStatistics()
        {
            
        }

        /// <summary>
        /// Create a new set of statistics based on a graph
        /// </summary>
        /// <param name="g">The graph to base the stats on</param>
        /// <returns>A GraphStatistics object representing the stats of the graph</returns>
        public static GraphStatistics From(Graph g)
        {
            float minX = g.nodes[0].position.X, minY = g.nodes[0].position.Y;
            float maxX = g.nodes[0].position.X, maxY = g.nodes[0].position.Y;

            foreach(var n in g.nodes)
            {
                if (n.position.X < minX)
                    minX = n.position.X;
                if (n.position.X > maxX)
                    maxX = n.position.X;

                if (n.position.Y < minY)
                    minY = n.position.Y;
                if (n.position.Y > maxY)
                    maxY = n.position.Y;
            }

            GraphStatistics stats = new GraphStatistics
            {
                Height = maxY - minY,
                Width = maxX - minX
            };
            stats.GraphArea = new RectangleF(minX, minY, stats.Width, stats.Height);

            // Calculate stuff with edges
            float longest_edge = 0;
            float shortest_edge = 1;
            foreach (Edge e in g.edges)
            {
                float l = e.Length;
                if (l > longest_edge)
                {
                    longest_edge = l;
                }
                if (l < shortest_edge)
                {
                    shortest_edge = l;
                }
                stats.EdgeLenghtTotal += l;
            }
            stats.EdgeRatio = longest_edge / shortest_edge;

            IEnumerable<float> edge_lengths = g.edges.Select(s => s.Length);
            stats.EdgeMean = (float)mean(edge_lengths);
            stats.EdgeStdDev = (float)stddev(edge_lengths, stats.EdgeMean);

            return stats;
        }

        static private double mean(IEnumerable<float> values)
        {
            float running_mean = 0;
            int count = 0;
            foreach (float val in values)
            {
                running_mean += val;
                count++;
            }
            running_mean /= count;
            return running_mean;
        }

        static private double stddev(IEnumerable<float> values, float mean)
        {
            double running_variance = 0;
            int count = 0;
            foreach (float val in values) {
                double sqrdiff = Math.Pow(val - mean, 2.0);
                running_variance += sqrdiff;
                count++;
            }
            return Math.Sqrt(running_variance / count);
        }
    }
}
