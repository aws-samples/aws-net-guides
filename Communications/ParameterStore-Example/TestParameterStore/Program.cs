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
            Console.ReadLine();
        }

        static async Task GetConfiguration()
        {
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
                    Console.WriteLine("Parameter Value: {0}", response.Parameter.Value);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Error occurred: {0}", ex.Message);
                }
            }
        }
    }
}
