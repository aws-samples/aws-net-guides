using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Glacier;
using Amazon.Glacier.Model;

namespace GlacierSample
{
    // IMPORTANT NOTE
    //
    // Glacier vaults CANNOT be deleted unless:
    // 1. they are empty,
    // 2. there has been no write activity since the last inventory was taken.
    //
    // Glacier prepares an inventory for each vault periodically, every 24 hours.
    //
    // This sample will create a new vault and then write and delete an archive. The sample
    // also illustrates how to delete a vault but the attempt will fail because Glacier has
    // not had chance to generate an inventory on its default schedule. To permit you to
    // delete the sample vault sooner, the sample will request Glacier create an inventory (which
    // is an async operation on the service). When the service completes the job, which can
    // take a few hours, you can then log into the AWS management console and delete the vault.


    class Program
    {
        private static readonly string VaultName = $"vault-{Guid.NewGuid().ToString("n").Substring(0, 8)}";

        // Optional: change this member to specify an explicit AWS account ID, which will be
        // needed by the SetVaultAccessPolicy call. If left as '-' the account ID will be
        // recovered from the vault location returned from the CreateVault API call.
        private static string _accountId = "-";

        static void Main(string[] args)
        {
            // note: this constructor relies on you having set up a credential profile
            // named 'default', or have set credentials in environment variables
            // AWS_ACCESS_KEY_ID & AWS_SECRET_ACCESS_KEY, or have an application settings
            // file. See https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-config-creds.html
            // for more details and other
            using (var glacier = new AmazonGlacierClient(RegionEndpoint.USWest2))
            {
                CreateVault(glacier);

                SetVaultAccessPolicy(glacier);

                AddTagsToVault(glacier);

                ListVaults(glacier);

                GetVaultAccessPolicy(glacier);

                var archiveId = UploadArchive(glacier);

                DeleteArchive(glacier, archiveId);

                // see important note above
                RequestVaultInventory(glacier);

                // see important note above
                DeleteVault(glacier);
            }

            Console.WriteLine("Glacier sample complete. Press any key to exit.");
            Console.ReadKey();
        }

        static void CreateVault(IAmazonGlacier glacier)
        {
            var req = new CreateVaultRequest
            {
                VaultName = VaultName
            };

            Task<CreateVaultResponse> res = glacier.CreateVaultAsync(req);
            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                Console.WriteLine($"Successfully created vault {VaultName}");

                // if the user did not update the sample code with their account ID before
                // running, recover the ID from the vault location path as we need the ID
                // to construct the Amazon Resource Name (ARN) of the vault when setting
                // the access policy
                if (_accountId.Equals("-"))
                {
                    _accountId = res.Result.Location.Split('/', StringSplitOptions.RemoveEmptyEntries)[0];
                }
            }
        }

        static void SetVaultAccessPolicy(IAmazonGlacier glacier)
        {
            // note - the actual account ID is required in the arn being constructed
            // here; it should have been set by modifying the sample code before running
            // or, if left as '-' by default, by the vault creation logic.
             var jsonPolicy = @"{
                ""Version"":""2012-10-17"",
                ""Statement"":[
                {
                    ""Sid"": ""glacier-perm"",
                    ""Principal"": ""*"",
                    ""Effect"": ""Allow"",
                    ""Action"": [
                        ""glacier:*""
                    ],
                    ""Resource"": [
                        ""arn:aws:glacier:us-west-2:" + _accountId + @":vaults/" + VaultName + @"""
                    ]
                }
                ]
            }";

            var req = new SetVaultAccessPolicyRequest
            {
                VaultName = VaultName,
                Policy = new VaultAccessPolicy
                {
                    Policy = jsonPolicy
                }
            };

            Task<SetVaultAccessPolicyResponse> res = glacier.SetVaultAccessPolicyAsync(req);
            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
               Console.WriteLine($"Vault access policy set successfully on vault {VaultName}");
            }
        }

        static void AddTagsToVault(IAmazonGlacier glacier)
        {
            var req = new AddTagsToVaultRequest
            {
                VaultName = VaultName,
                Tags = new Dictionary<string, string>
                {
                    { "cost-center","1234" },
                    { "stack","production" }
                }
            };

            Task<AddTagsToVaultResponse> res = glacier.AddTagsToVaultAsync(req);
            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                Console.WriteLine($"Successfully added tags to vault {VaultName}");
            }
        }

        static void ListVaults(IAmazonGlacier glacier)
        {
            var req = new ListVaultsRequest
            {
                Limit = 100
            };

            Task<ListVaultsResponse> res = glacier.ListVaultsAsync(req);
            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                foreach (var vault in res.Result.VaultList)
                {
                    Console.WriteLine($"Vault: {vault.VaultName}");
                }
            }
        }

        static void GetVaultAccessPolicy(IAmazonGlacier glacier)
        {
            var req = new GetVaultAccessPolicyRequest
            {
                VaultName = VaultName
            };

            Task<GetVaultAccessPolicyResponse> res = glacier.GetVaultAccessPolicyAsync(req);
            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                Console.WriteLine($"Access policy for vault {VaultName} is '{res.Result.Policy.Policy}'");
            }
        }

        static string UploadArchive(IAmazonGlacier glacier)
        {
            var ms = new MemoryStream(Encoding.UTF8.GetBytes("some data to archive"));
            var treeHash = TreeHashGenerator.CalculateTreeHash(ms);

            var req = new UploadArchiveRequest
            {
                VaultName = VaultName,
                Body = ms,
                Checksum = treeHash
            };

            Task<UploadArchiveResponse> res = glacier.UploadArchiveAsync(req);
            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                Console.WriteLine($"Upload archive to ArchiveId {res.Result.ArchiveId}");
                return res.Result.ArchiveId;
            }

            return string.Empty;
        }

        static void DeleteArchive(IAmazonGlacier glacier, string archiveId)
        {
            var req = new DeleteArchiveRequest
            {
                VaultName = VaultName,
                ArchiveId = archiveId
            };

            Task<DeleteArchiveResponse> res = glacier.DeleteArchiveAsync(req);
            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                Console.WriteLine($"Deleted ArchiveId: {archiveId}");
            }
        }

        static void RequestVaultInventory(IAmazonGlacier glacier)
        {
            // This sample method forces Glacier to start inventorying the new vault
            // created by the sample. Glacier's default behavior is to inventory a
            // vault once every 24 hours. Vaults cannot be deleted unless (1) they
            // are empty and (2) there has been no write activity since the last
            // inventory. By requesting an inventory ahead of schedule, it allows
            // you the user to delete the sample vault created by this code earlier.
            // Note that it can take Glacier a few hours to finish inventory processing.

            var req = new InitiateJobRequest
            {
                VaultName = VaultName,
                JobParameters = new JobParameters
                {
                    Type = "inventory-retrieval"
                }
            };

            Task<InitiateJobResponse> res = glacier.InitiateJobAsync(req);
            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                Console.WriteLine($"Inventory job for vault {VaultName} has been queued. On completion you may delete the vault assuming no further write activity takes place.");
            }
        }
        
        static void DeleteVault(IAmazonGlacier glacier)
        {
            // PLEASE SEE THE IMPORTANT NOTE AT THE START OF THIS SAMPLE. 
            // 
            // This method shows how to delete a vault using the service api BUT will fail
            // to delete the vault created in the sample because the inventory job, started
            // in the RequestVaultInventory method, will not have completed at the time this
            // method runs. If the RequestVaultInventory method is not run, it can take up to
            // 24 hours for Glacier to schedule an inventory automatically.
            //
            // Check back in the AWS management console after a few hours and once the
            // inventory job is complete, delete the vault from within the console.

            var req = new DeleteVaultRequest
            {
                VaultName = VaultName
            };

            try
            {
                Task<DeleteVaultResponse> res = glacier.DeleteVaultAsync(req);
                Task.WaitAll(res);

                Console.WriteLine($"Deleted vault {VaultName}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to delete vault {VaultName}. The service returned error: {e.Message}");
            }
        }
    }
}
