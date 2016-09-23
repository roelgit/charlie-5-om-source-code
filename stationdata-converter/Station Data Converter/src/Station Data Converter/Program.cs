using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            } catch(System.IO.IOException e)
            {
                Console.Error.WriteLine("Failed to read JSON data: {0}", e.Message);
                return;
            }

            Console.WriteLine("Loaded JSON data ({0} bytes)", data.Length);

            
        }
    }
}
