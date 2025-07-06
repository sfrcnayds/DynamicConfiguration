using System;
using System.Threading.Tasks;
using DynamicConfiguration.Library;

namespace DynamicConfiguration.TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configurationReader = new ConfigurationReader("SERVICE-B",
                    "mongodb://localhost:27017",
                    1000000);

            var appTitle = configurationReader.GetValue<string>("BaseUrl");
            Console.WriteLine($"AppTitle = {appTitle}");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
