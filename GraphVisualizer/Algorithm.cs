using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphVisualizer
{
    abstract class Algorithm
    {
        // Prepare the algorithm. This should only be called _once_.
        public abstract void start(Graph g);

        // Run one step of the algorithm on the graph, modifying it in place
        // The return code indicates whether the algorithm has finished, "true" when done.
        public abstract bool step(Graph g);

        // run the entire algorithm
        public void run(Graph g)
        {
            g.finalize();
            bool terminated = false;
            while (!terminated)
            {
                terminated = step(g);
            }
        }
    }
}
