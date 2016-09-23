using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Station_Data_Converter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.Error.WriteLine("Please pass the path to the JSON file as a command line argument");
                return;
            }

            Console.ReadLine();
        }
    }
}
