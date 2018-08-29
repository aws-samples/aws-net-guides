using System;
using AWSAppService.Data;
using AWSAppService.Auth;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string AppPoolID = "us-east-1:202ec49d-1496-4e69-9e59-6aa7bc268975";
            Amazon.RegionEndpoint AppRegion = Amazon.RegionEndpoint.USEast1;
            Console.WriteLine("Hello World!");

            //CloudAuthService cloudAuthService = new CloudAuthService(AppPoolID, AppRegion);
            //DBDataService dBDataService = new DBDataService(cloudAuthService.GetAWSCredentials());

            Song testSongObj = new Song() { Artist = "The Internet",
                                            Title = "Girl",
                                            };
            
            testSongObj.GetLyrics().Wait();

            Console.WriteLine(testSongObj.SongLyric);

            //CognitoAWSCredentials cognitoAWSCredentials = new CognitoAWSCredentials(AppPoolID, AppRegion);
        }
    }
}
