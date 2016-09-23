using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Station_Data_Converter.objects
{
    /// <summary>
    /// This object represents a class within the train station JSON file
    /// A class needs to be created in order to parse the data file
    /// </summary>
    public class Properties
    {
        public String name { get; set; }
        public String abbr { get; set; }
    }
}
