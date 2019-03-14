# Archiving Objects with Amazon Glacier and .NET

## Overview

This walk-through illustrates how to use Amazon Glacier from a .NET applicaton to archive data using the [AWS SDK for .NET](https://docs.aws.amazon.com/sdk-for-net/). The walk-through uses a .NET Core console application but the code shown in also applicable to .NET Framework-based applications. The walk-through will show you how to create a Glacier Vault, set an access policy, write an archive to the vault, listing the vaults, and deleting archives and vaults.

* Links to documentation
  * [Amazon Glacier](https://aws.amazon.com/glacier/)
  * [Amazon Glacier Developer Guide](https://docs.aws.amazon.com/amazonglacier/latest/dev/)

### Prerequisites

* .NET Core 2.0 or higher installed

* AWS Account with credentials configured locally in the [AWS Toolkit for Visual Studio](https://aws.amazon.com/visualstudio/), or using the [AWS Tools for PowerShell](https://aws.amazon.com/powershell/) or the [AWS CLI](https://aws.amazon.com/cli/)

* Optional: Visual Studio 2017. This walk-through illustrates using the dotnet CLI but you can also use the *File* > *New Project* and the NuGet Package Manager inside Visual Studio to create and manipulate the project.

***
**Important Note**

The code included with this guide will create a Glacier Vault, and show how to upload and delete archives. Glacier vaults **cannot** be deleted unless

1. They are empty.
1. There has been no write activity since the last inventory was taken for the vault.

By default Glacier prepares an inventory for each vault periodically, every 24 hours. The sample code that illustrates how to delete a vault will fail because an inventory for the vault created by the sample has not been completed. To permit you to delete the vault sooner than 24 hours after running the sample, the sample code will request that Glacier initiate an inventory job, which is an asynchronous option that can take a few hours to complete.

You can check when the inventory is completed by logging into the AWS management console, navigating to the Glacier dashboard, and selecting the sample vault. The *Inventory Last Updated* field will show a date and time of completion - at this point you can delete the vault.
***

## Introduction

Amazon Glacier is a secure, durable, and extremely low-cost cloud storage service for data archiving and long-term backup. It is designed to deliver 99.999999999% durability and provides comprehensive security and compliance capabilities that can help meet even the most stringent regulatory requirements. Amazon Glacier provides query-in-place functionality, allowing you to run powerful analytics directly on your archive data at rest.

For this walk-through we'll create a new .NET Core console application, add a dependency on the NuGet package for Glacier from the AWS SDK for .NET to it, and walk through common functionality such as creating vaults, setting access policies on vaults, uploading archives to vaults, listing vaults and deleting archives and vaults.

## Create and Configure a Console Application

### Step 1: Create an empty console application project

In this step we will create a new directory to hold our application code, and then create a console application project in it using the following commands in either a Windows command prompt, MacOS bash shell or Linux bash shell. Alternatively you can use the sample application code in the SampleApplication subfolder.

First we create our new project:

```bash
mkdir glacier-sample
cd glacier-sample
dotnet new console
```

The *dotnet new* command will create the project files, and restore packages referenced by the template.

### Step 2: Add the Glacier NuGet Package

Run the following command in your command shell, or use the NuGet package manager tools in Visual Studio, to add the NuGet package *AWSSDK.Glacier* to the project:

```bash
dotnet add package AWSSDK.Glacier
```

### Step 3: Edit the C# Code

The complete sample application can be found in the SampleApplication subfolder of this guide but is presented here for reference.

> Glacier requires you to provide a vault name and your AWS account ID when calling its APIs. The AWS account ID value must match the AWS account ID associated with the credentials used by the SDK to sign each API request. For most Glacier API calls you can either explicitly specify an AWS account ID or optionally use a single '-' (hyphen) to indicate to Glacier that it should use the AWS account ID associated with the credentials used to sign the API request.
>
> The sample makes use of the SetVaultAccessPolicy API which requires you to include an Amazon Resource Name (ARN) in the policy, which requires your actual account ID. When a vault is created, the vault location value in the response contains the account ID as the first component. The sample application retrieves the value and uses it in the policy it builds for the SetVaultAccessPolicy API call **if** the value of the `_accountId` member is a hyphen. You may however elect to edit the value of `_accountId` in the sample instead.

```csharp
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
```

This sample code demonstrates the pattern that is common across all AWS services when programming against them with the AWS SDK for .NET. Each service exposes a client object which implements an interface with methods corresponding to the service API. Each API has a request and response class defined, named after the API operation, which enable you to set the input values for an operation and retrieve the results on return from an API call.

The SDK takes care of marshaling the request and response data to the service so that you do not need to consider whether the service accepts XML, JSON etc. It also takes care of programmatic signing of the request which is required by the majority of AWS services.

In the example code shown above we have used only the *Async* apis exposed on the service. These are the only APIs exposed for .NET Core. If you are using the .NET Framework then synchronous APIs are also available to you. For example, for .NET Framework the IAmazonGlacier interface exposes *CreateVault* and *CreateVaultAsync*. For .NET Core, only *CreateVaultAsync* is exposed.

## Run the Sample

To run through the Glacier functionality using the console application, use the following command to build (compile) and run the app:

```bash
dotnet run
```

If you want to run the app again without compiling, just pass the \--no-build flag:

```bash
dotnet run \--no-build
```
