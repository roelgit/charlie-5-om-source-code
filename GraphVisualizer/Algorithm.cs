using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphVisualizer
{
    abstract class Algorithm
    {
        /// <summary>
        /// Prepare the algorithm. This should only be called _once_.
        /// </summary>
        /// <param name="g">The graph to start processing</param>
        public abstract void start(Graph g);

        /// <summary>
        /// Run one step of the algorithm on the graph, modifying it in place
        /// </summary>
        /// <param name="g">The graph to step through</param>
        /// <returns>Whether the algorithm has finished or not. True if it is done, False otherwise</returns>
        public abstract bool step(Graph g);

        /// <summary>
        /// Run the entire algorithm
        /// </summary>
        /// <param name="g">The graph to run on</param>
        public void run(Graph g)
        {
            start(g);
            while (!step(g)) ;
        }
    }
}
