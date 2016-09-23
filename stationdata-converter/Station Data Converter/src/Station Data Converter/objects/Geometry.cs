using Newtonsoft.Json;
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
    public class Geometry
    {
        public String type { get; set; }

        [JsonConverter(typeof(CoordinateJsonConverter))]
        public List<String[]> coordinates { get; set; }

        public String CoordinateString()
        {
            var output = "";
            foreach (var coord in coordinates)
                output += "(" + coord[0] + "," + coord[1] + ")";

            return output;
        }
    }
}
