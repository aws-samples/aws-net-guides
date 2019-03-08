using System;
using System.Threading.Tasks;
using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace TestParameterStore
{
    class Program
    {
        static void Main(string[] args)
        {
            GetConfiguration().Wait();
			
			Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static async Task GetConfiguration()
        {
			// NOTE: set the region here to match the region used when you created
			// the parameter
            var region = Amazon.RegionEndpoint.USEast1;

            var request = new GetParameterRequest()
            {
                Name = "/TestParameterStore/EnvironmentName"
            };

            using (var client = new AmazonSimpleSystemsManagementClient(region))
            {
                try
                {
                    var response = await client.GetParameterAsync(request);
                    Console.WriteLine($"Parameter {request.Name} has value: {response.Parameter.Value}");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error occurred: {ex.Message}");
                }
            }
        }
    }
}
