using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Station_Data_Converter
{
    /// <summary>
    /// A node in the graphs
    /// </summary>
    public class Station
    {
        /// <summary>
        /// The name of the station
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The abbreviation of the station
        /// </summary>
        public String Abbreviation { get; set; }

        /// <summary>
        /// The connected stations
        /// </summary>
        public List<Station> ConnectedStations { get; set; }

        /// <summary>
        /// The global ID number variable
        /// It's static so it will increase as more stations get added
        /// </summary>
        private static int id;

        /// <summary>
        /// The station ID (each station gets its own unique ID)
        /// </summary>
        public int ID
        {
            get ;
            set ;
        }


        public Station(string name, string abbr)
        {
            this.Name = name;
            this.Abbreviation = abbr;
            this.ConnectedStations = new List<Station>();
            this.ID = id++;
        }
    }
}
