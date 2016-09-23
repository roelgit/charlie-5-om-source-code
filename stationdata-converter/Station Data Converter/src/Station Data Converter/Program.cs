using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using Station_Data_Converter.objects;

namespace Station_Data_Converter
{
    public class Program
    {
        private static String loadFile(String filename)
        {
            var filestream = System.IO.File.OpenRead(filename);
            var reader = new System.IO.StreamReader(filestream);
            String jsonData = reader.ReadToEnd();

            reader.Dispose();
            filestream.Dispose();

            return jsonData;
        }

        public static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Please pass the path to the JSON file and the path to the output file as a command line argument");
                return;
            }

            String fileNameInput = args[0];
            String fileNameOutput = args[1];

            String data;
            try
            {
                data = loadFile(fileNameInput);
            }
            catch (System.IO.IOException e)
            {
                Console.Error.WriteLine("Failed to read JSON data: {0}", e.Message);
                return;
            }

            Console.WriteLine("Loaded JSON data ({0} bytes)", data.Length);

            // A dictionary to look up a station object by coordinates
            Dictionary<String, Station> stationData = new Dictionary<String, Station>();

            Item[] features = ParseData(data);

            FindStations(stationData, features);
            ConnectStations(stationData, features);

            PrintStationInfo(stationData, "Nunspeet");
            PrintStationInfo(stationData, "Zwolle");

            Console.WriteLine();
        }

        private static void PrintStationInfo(Dictionary<string, Station> stationData, string name)
        {
            Station searchedStation = stationData.Values.Where((station) => station.Name == name).First();
            Console.Write("{0} ({1}) is connected to: ", name, searchedStation.ID);
            foreach (Station s in searchedStation.ConnectedStations)
                Console.Write("{0} ({1}) ", s.Name, s.ID);
            Console.WriteLine();
        }

        private static Item[] ParseData(string data)
        {
            Console.Write("Parsing JSON data...");
            var baseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<DataFile>(data);
            Console.WriteLine("\rParsing JSON data complete");

            var features = baseObject.features;
            return features;
        }

        private static void FindStations(Dictionary<string, Station> stationData, Item[] features)
        {
            // First loop: finding all the train stations in the list
            Console.WriteLine("Importing station data...");
            foreach (var feature in features)
            {
                try
                {
                    if (feature.geometry != null && feature.geometry.type == "Point")
                    {
                        stationData[CoordString(feature.geometry.coordinates[0])] = new Station(feature.properties.name, feature.properties.abbr);
                    }
                }
                catch (NullReferenceException)
                {
                    Console.Error.WriteLine("Invalid or unexpected feature encountered (geometry = null)");
                }
            }
            Console.WriteLine("Done importing station data, found {0} train stations!", stationData.Count);
        }

        private static void ConnectStations(Dictionary<string, Station> stationData, Item[] features)
        {
            // Second loop: now that we have an overview of all train stations, we find the relations
            long interconnections = 0;
            Console.WriteLine("Connecting stations...");
            foreach (var feature in features)
            {
                if (feature.geometry == null)
                {
                    Console.Error.WriteLine("Invalid or unexpected feature encountered (geometry = null)");
                }
                else if (feature.geometry.type == "Point")
                {
                    // This is valid, but not what we want at this time
                    // skip this one
                }
                else if (feature.geometry.type == "LineString")
                {
                    // This is what we want!
                    String[] stationCoords = null;
                    try
                    {
                        stationCoords = feature.geometry.coordinates
                        .Where(coord => stationData.ContainsKey(CoordString(coord)))
                        .First();
                    }
                    catch (InvalidOperationException)
                    {
                        // This happens when a line that is not connected to any station is being processed
                        //Console.Error.WriteLine("No connected stations for this feature!");
                        continue;
                    }

                    var station = stationData[CoordString(stationCoords)];

                    foreach (var otherCoords in feature.geometry.coordinates
                        .Where(coord => coord != stationCoords)
                        .Where(coord => stationData.ContainsKey(CoordString(coord))))
                    {
                        var otherStation = stationData[CoordString(otherCoords)];

                        interconnectStations(station, otherStation);

                        interconnections++;
                    }
                }
            }
            Console.WriteLine("Done connecting stations, found {0} interconnections", interconnections);
        }

        private static string CoordString(string[] stationCoords)
        {
            return stationCoords[0] + stationCoords[1];
        }

        private static void interconnectStations(Station station, Station otherStation)
        {
            if (!otherStation.ConnectedStations.Contains(station))
                otherStation.ConnectedStations.Add(station);

            if (!station.ConnectedStations.Contains(otherStation))
                station.ConnectedStations.Add(otherStation);
        }
    }
}
