using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Glacier;
using Amazon.Glacier.Model;

namespace TestGlacier
{
    class Program
    {
        static string vaultName = "vault-" + Guid.NewGuid().ToString("n").Substring(0, 8);
        static string key = "key-" + Guid.NewGuid().ToString("n").Substring(0, 8);
        //enter your aws accountID to watch this program run
        static string accountId = "";

        static void Main(string[] args)
        {
            AmazonGlacierClient glacier = new AmazonGlacierClient(RegionEndpoint.USWest2);

            CreateVault(glacier);   

            SetVaultAccessPolicy(glacier);

            AddTagsToVault(glacier);

            ListVaults(glacier);

            GetVaultAccessPolicy(glacier);

            string archiveId = UploadArchive(glacier);

            DeleteArchive(glacier, archiveId);

            DeleteVault(glacier);
        }

        private static void DeleteArchive(AmazonGlacierClient glacier, string archiveId)
        {
            DeleteArchiveRequest req = new DeleteArchiveRequest();
            req.VaultName = vaultName;
            req.ArchiveId = archiveId;

            Task<DeleteArchiveResponse> res = glacier.DeleteArchiveAsync(req);

            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                Console.Write("Delete ArchiveId: {0}", archiveId);
            }
        }

        private static string UploadArchive(AmazonGlacierClient glacier)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes("data to archive"));
            
            string treeHash = TreeHashGenerator.CalculateTreeHash(ms);

            UploadArchiveRequest req = new UploadArchiveRequest();
            req.VaultName = vaultName;
            req.Body = ms;
            req.Checksum = treeHash;

            Task<UploadArchiveResponse> res = glacier.UploadArchiveAsync(req);

            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                Console.WriteLine("ArchiveId: {0}", res.Result.ArchiveId);

                return res.Result.ArchiveId;
            }
            else
            {
                return string.Empty;
            }
        }

        private static void DeleteVault(AmazonGlacierClient glacier)
        {
           DeleteVaultRequest req = new DeleteVaultRequest();
           req.VaultName = vaultName;

           Task<DeleteVaultResponse> res = glacier.DeleteVaultAsync(req);

           Task.WaitAll(res);

           if (res.IsCompletedSuccessfully)
           {
               Console.WriteLine("Deleted Vault {0}", vaultName);
           }
        }

        private static void SetVaultAccessPolicy(AmazonGlacierClient glacier)
        {
             string jsonPolicy = @"{
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
                        ""arn:aws:glacier:us-west-2:" + accountId + @":vaults/" + vaultName + @"""
                    ]
                }
                ]
            }";

            SetVaultAccessPolicyRequest req = new SetVaultAccessPolicyRequest();
            req.VaultName = vaultName;
            req.Policy = new VaultAccessPolicy();
            req.Policy.Policy = jsonPolicy;

            Task<SetVaultAccessPolicyResponse> res = glacier.SetVaultAccessPolicyAsync(req);

            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
               Console.Write("Set Vault Access Policy on vault {0}", vaultName); 
            }
        }

        private static void CreateVault(AmazonGlacierClient glacier)
        {
            CreateVaultRequest req = new CreateVaultRequest();
            req.VaultName = vaultName;
            
            Task<CreateVaultResponse> res = glacier.CreateVaultAsync(req);

            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                Console.Write("Created Vault {0} successfully", vaultName);
            }
        }

        private static void AddTagsToVault(AmazonGlacierClient glacier)
        {
            AddTagsToVaultRequest req = new AddTagsToVaultRequest();
            req.VaultName = vaultName;
            req.Tags = new Dictionary<string, string>();
            req.Tags.Add("cost-center","1234");
            req.Tags.Add("stack","production");
            
            Task<AddTagsToVaultResponse> res = glacier.AddTagsToVaultAsync(req);

            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                Console.Write("Created Vault {0} successfully", vaultName);
            }
        }

        private static void GetVaultAccessPolicy(AmazonGlacierClient glacier)
        {
            GetVaultAccessPolicyRequest req = new GetVaultAccessPolicyRequest();
            req.VaultName = vaultName;

            Task<GetVaultAccessPolicyResponse> res = glacier.GetVaultAccessPolicyAsync(req);

            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                Console.Write("Vault Policy for {0}: {1}", vaultName, res.Result.Policy.Policy);
            }
        }

        private static void ListVaults(AmazonGlacierClient glacier)
        {
            ListVaultsRequest req = new ListVaultsRequest();
            req.Limit = 100;

            Task<ListVaultsResponse> res = glacier.ListVaultsAsync(req);

            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                foreach (var vault in res.Result.VaultList)
                {
                    Console.WriteLine("Vault: {0}", vault.VaultName);
                }
            }
        }
    }
}
