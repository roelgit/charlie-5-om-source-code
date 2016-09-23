using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphVisualizer
{
    interface IInputLoader
    {
        Graph load(string filename);
    }
}
