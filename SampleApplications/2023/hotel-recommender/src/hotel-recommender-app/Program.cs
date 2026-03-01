using Amazon;
using Amazon.Personalize;
using Amazon.PersonalizeRuntime;
using Amazon.Personalize.Model;
using Amazon.PersonalizeRuntime.Model;
using Amazon.IdentityManagement;
using Amazon.IdentityManagement.Model;
using CommandLine;
using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime;
using Amazon.SecurityToken;

namespace hotel_recommender;
class Program
{

    public static readonly string LOGICALID = "hotel-picker";
    public static string? AccountID;
    public static string? Region;

    public class Options
    {
        [Option('t', "waittime", Required = false, Default = 60000, HelpText = "Set wait time in milliseconds to wait for models to complete")]
        public int WaitTime { get; set; }

        [Option('i', "initialload", Required = false, Default = false, HelpText = "Set initial load to true if running for the first time")]
        public bool InitialLoad { get; set; }

        [Option('d', "dataimport", Required = false, Default = false, HelpText = "Create import jobs")]
        public bool CreateDataImports { get; set; }

        [Option('s', "solution", Required = false, Default = false, HelpText = "Create solutions")]
        public bool CreateSolution { get; set; }

        [Option('v', "solutionarn", Required = false, Default = "", HelpText = "Provide solution arn to create solution version")]
        public string? SolutionArn { get; set; }

        [Option('c', "solutionversionarn", Required = false, Default = "", HelpText = "Provide solution version arn to create campaign")]
        public string? SolutionVersionArn { get; set; }
    }

    static async Task Main(string[] args)
    {
        int timeout = 60000;
        bool createdataimports = false;
        bool createsolution = false;
        string? solutionarn = string.Empty;
        string? solutionversionarn = string.Empty;
        bool initialLoad = false;

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o =>
            {
                timeout = o.WaitTime;
                createdataimports = o.CreateDataImports;
                createsolution = o.CreateSolution;
                solutionarn = o.SolutionArn;
                solutionversionarn = o.SolutionVersionArn;
                initialLoad = o.InitialLoad;
            })

            .WithNotParsed(errs =>
            {
                Environment.Exit(0);
            });

        AmazonSecurityTokenServiceClient tokenServiceClient = new AmazonSecurityTokenServiceClient();
        var amazonPersonalizeRunTimeClient = new AmazonPersonalizeRuntimeClient();
        var response = await tokenServiceClient.GetCallerIdentityAsync(new Amazon.SecurityToken.Model.GetCallerIdentityRequest());
        AccountID = response.Account;
        Region = amazonPersonalizeRunTimeClient.Config.RegionEndpoint.SystemName;


        var amazonPersonalizeClient = new AmazonPersonalizeClient();

        var client = new AmazonIdentityManagementServiceClient();

        var datasets = new List<Tuple<string, string>>
            {
                Tuple.Create("INTERACTIONS", "inter.csv"),
                Tuple.Create("USERS", "users.csv"),
                Tuple.Create("ITEMS", "items.csv")
            };

        ParallelOptions parallelOptions = new()
        {
            MaxDegreeOfParallelism = 3
        };

        if (createdataimports || initialLoad)
        {
            var roleResponse = await client.GetRoleAsync(new GetRoleRequest
            {
                RoleName = $"{LOGICALID}-PersonalizeRole",
            });

            Console.WriteLine("RoleArn is {0}", roleResponse.Role.Arn);

            await Parallel.ForEachAsync(datasets, parallelOptions, async (dataset, token) =>
            {
                var importJobs = await amazonPersonalizeClient.ListDatasetImportJobsAsync(new ListDatasetImportJobsRequest
                {
                    DatasetArn = $"arn:aws:personalize:{Region}:{AccountID}:dataset/{LOGICALID}-DatasetGroup/{dataset.Item1}"
                });

                if (importJobs.DatasetImportJobs.Count == 0)
                {
                    var importjob = await amazonPersonalizeClient.CreateDatasetImportJobAsync(new CreateDatasetImportJobRequest
                    {
                        JobName = $"{dataset.Item1}-job",
                        DataSource = new DataSource { DataLocation = $"https://{LOGICALID}.s3.amazonaws.com/{dataset.Item2}" },
                        RoleArn = roleResponse.Role.Arn,
                        DatasetArn = $"arn:aws:personalize:{Region}:{AccountID}:dataset/{LOGICALID}-DatasetGroup/{dataset.Item1}"
                    });
                    Console.WriteLine(string.Format("interactions-job Response is {0}", importjob.DatasetImportJobArn));
                }
            });

            var userHandlers = new[]
                  {
                "INTERACTIONS-job",
                "ITEMS-job",
                "USERS-job"
            };

            await Parallel.ForEachAsync(userHandlers, parallelOptions, async (datasetgroupItem, token) =>
            {
                DescribeDatasetImportJobResponse importJobResponse;
                do
                {
                    Console.WriteLine($"Data Set Group Item is {datasetgroupItem}");

                    Console.WriteLine($"Checking status of {datasetgroupItem} import job, will sleep for 60 seconds before checking...");
                    Thread.Sleep(timeout);
                    importJobResponse = await amazonPersonalizeClient.DescribeDatasetImportJobAsync(new DescribeDatasetImportJobRequest
                    {
                        DatasetImportJobArn = $"arn:aws:personalize:{Region}:{AccountID}:dataset-import-job/{datasetgroupItem}"
                    });
                    Console.WriteLine($"Status of solution version is {importJobResponse.DatasetImportJob.Status}");
                } while (importJobResponse.DatasetImportJob.Status != "ACTIVE");
            });
        }

        if (createsolution || initialLoad)
        {
            Console.WriteLine("Creating solution");
            var recommender = await amazonPersonalizeClient.CreateSolutionAsync(new CreateSolutionRequest
            {
                Name = $"{LOGICALID}",
                DatasetGroupArn = $"arn:aws:personalize:{Region}:{AccountID}:dataset-group/{LOGICALID}-DatasetGroup",
                PerformAutoML = true
            });
            Console.WriteLine($"SolutinArn is {recommender.SolutionArn}");
            solutionarn = recommender.SolutionArn;
        }

        if (!string.IsNullOrEmpty(solutionarn) || initialLoad)
        {
            Console.WriteLine($"SolutionArn is  {solutionarn}");
            Console.WriteLine("Creating solution version, which takes time...");
            var recommender_model = await amazonPersonalizeClient.CreateSolutionVersionAsync(new CreateSolutionVersionRequest
            {
                SolutionArn = solutionarn
            });

            DescribeSolutionVersionResponse solutionVersionResponse;

            do
            {
                Console.WriteLine("Checking status of solution version, will sleep for 60 seconds before checking...");
                Thread.Sleep(timeout);
                solutionVersionResponse = await amazonPersonalizeClient.DescribeSolutionVersionAsync(new DescribeSolutionVersionRequest
                {
                    SolutionVersionArn = recommender_model.SolutionVersionArn
                });
                Console.WriteLine($"Status of solution version is {solutionVersionResponse.SolutionVersion.Status}");
            } while (solutionVersionResponse.SolutionVersion.Status != "ACTIVE");
            solutionversionarn = recommender_model.SolutionVersionArn;
        }

        if (!string.IsNullOrEmpty(solutionversionarn) || initialLoad)
        {
            Console.WriteLine($"SolutionVersionArn is  {solutionversionarn}");
            Console.WriteLine("Creating campaign");
            var campaign = await amazonPersonalizeClient.CreateCampaignAsync(new CreateCampaignRequest
            {
                Name = $"{LOGICALID}-campaign",
                SolutionVersionArn = $"{solutionversionarn}",
                MinProvisionedTPS = 1
            });

            DescribeCampaignResponse campaignResponse;
            do
            {
                Console.WriteLine("Checking status of campaign version, will sleep for 60 seconds before checking...");
                Thread.Sleep(timeout);
                campaignResponse = await amazonPersonalizeClient.DescribeCampaignAsync(new DescribeCampaignRequest
                {
                    CampaignArn = campaign.CampaignArn
                });
                Console.WriteLine($"Status of campaign is {campaignResponse.Campaign.Status}");
            } while (campaignResponse.Campaign.Status != "ACTIVE");
        }

        if (!initialLoad)
        {
            Console.Write("Getting recommendations, please enter your userId -> ");

            var userID = Console.ReadLine();

            var recommendation = await amazonPersonalizeRunTimeClient.GetRecommendationsAsync(new GetRecommendationsRequest
            {
                CampaignArn = $"arn:aws:personalize:{Region}:{AccountID}:campaign/{LOGICALID}-campaign",
                UserId = userID
            });

            Console.WriteLine($"Hello User {userID}");
            foreach (var hotel in recommendation.ItemList)
            {
                Console.WriteLine($"Amazon Personlize recommends the following hotel for you {hotel.ItemId}");
            }
        }
    }
}