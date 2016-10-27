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

        //TODO: Add insersections, angles, etc.

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

            return stats;
        }
    }
}
