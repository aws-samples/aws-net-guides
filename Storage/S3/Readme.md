# Working with Amazon S3 from .NET

## Overview

This walk-through illustrates how to access Amazon S3 buckets, and the objects they contain, from a .NET application using the [AWS SDK for .NET](https://docs.aws.amazon.com/sdk-for-net/). The walk-through uses a .NET Core console application but the code shown in also applicable to .NET Framework-based applications. The walk-through will show you how to create an Amazon S3 bucket, how to write (upload) an object to the bucket, how to list your buckets (and objects in the newly created S3 bucket) along with how to retrieve (download) a specific object.

* Links to documentation
  * [Amazon S3](https://aws.amazon.com/s3/)
  * [Amazon S3 Developer Guide](https://docs.aws.amazon.com/AmazonS3/latest/dev/Welcome.html)

### Prerequisites

* .NET Core 2.0 or higher installed

* AWS Account with credentials configured locally in the [AWS Toolkit for Visual Studio](https://aws.amazon.com/visualstudio/), or using the [AWS Tools for PowerShell](https://aws.amazon.com/powershell/) or the [AWS CLI](https://aws.amazon.com/cli/)

* Optional: Visual Studio 2017. This walk-through illustrates using the dotnet CLI but you can also use the *File* > *New Project* and the NuGet Package Manager inside Visual Studio to create and manipulate the project.

## Introduction

Amazon S3 is object storage built to store and retrieve any amount of data from anywhere -- web sites and mobile apps, corporate applications, and data from IoT sensors or devices. It is designed to deliver 99.999999999% durability, and stores data for millions of applications used by market leaders in every industry. S3 provides comprehensive security and compliance capabilities that meet even the most stringent regulatory requirements. It gives customers flexibility in the way they manage data for cost optimization, access control, and compliance.

For this walk-through, we'll create a new .NET Core console application, add a dependency on the NuGet package for S3 from the AWS SDK for .NET to it, and walk through common functionality such as creating buckets, writing/reading objects, listing buckets and objects, and deleting buckets and objects.

## Create and Configure a Console Application

### Step 1: Create an empty console application project

In this step we will create a new directory to hold our application code, and then create a console application project in it using the following commands in either a Windows command prompt, MacOS bash shell or Linux bash shell. Alternatively you can use the sample application code in the SampleApplication subfolder.

First we create our new project:

```bash
mkdir s3-sample
cd s3-sample
dotnet new console
```

The *dotnet new* command will create the project files, and restore packages referenced by the template.

### Step 2: Add the S3 NuGet Package from the AWS SDK for .NET

Run the following command in your command shell, or use the NuGet package manager tools in Visual Studio, to add the NuGet package *AWSSDK.S3* to the project:

```bash
dotnet add package AWSSDK.S3
```

### Step 3: Edit the C# Code

The complete sample application can be found in the SampleApplication subfolder of this guide but is presented here for reference.

```csharp
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;

namespace S3_Example
{
    class Program
    {
        // bucket names in Amazon S3 must be globally unique and lowercase
        static string bucketName = $"bucket-{Guid.NewGuid().ToString("n").Substring(0, 8)}";
        static string key = $"key-{Guid.NewGuid().ToString("n").Substring(0, 8)}";

        static void Main(string[] args)
        {
            // note: this constructor relies on you having set up a credential profile
            // named 'default', or have set credentials in environment variables
            // AWS_ACCESS_KEY_ID & AWS_SECRET_ACCESS_KEY, or have an application settings
            // file. See https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-config-creds.html
            // for more details and other
            using (var s3 = new AmazonS3Client(RegionEndpoint.USWest2))
            {
                CreateBucket(s3);
                Console.WriteLine("Press enter to continue...");
                Console.Read();

                ListBuckets(s3);
                Console.WriteLine("Press enter to continue...");
                Console.Read();

                WriteObject(s3);
                Console.WriteLine("Press enter to continue...");
                Console.Read();

                ListObjects(s3);
                Console.WriteLine("Press enter to continue...");
                Console.Read();

                ReadObject(s3);
                Console.WriteLine("Press enter to continue...");
                Console.Read();

                DeleteObject(s3);
                Console.WriteLine("Press enter to continue...");
                Console.Read();

                DeleteBucket(s3);
            }
        }

        static void CreateBucket(IAmazonS3 s3)
        {
            var req = new PutBucketRequest
            {
                BucketName = bucketName,
                BucketRegion = S3Region.USW2
            };
            Task<PutBucketResponse> res = s3.PutBucketAsync(req);
            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                Console.WriteLine("New S3 bucket created: {0}", bucketName);
            }
        }

        static void WriteObject(IAmazonS3 s3)
        {
            // The api call used in this method equates to S3's Put api and is
            // suitable for smaller files. To upload larger files and entire
            // folder hierarchies, with automatic usage of S3's multi-part apis for
            // files over 5MB in size, consider using the TransferUtility class
            // in the Amazon.S3.Transfer namespace.
            // See https://docs.aws.amazon.com/AmazonS3/latest/dev/HLuploadFileDotNet.html.
            var ms = new MemoryStream(Encoding.UTF8.GetBytes("Test S3 data"));
            var req = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = ms
            };

            Task<PutObjectResponse> res = s3.PutObjectAsync(req);
            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                Console.WriteLine("Created object '{0}' in bucket '{1}'", key, bucketName);
            }
        }

        static void ReadObject(IAmazonS3 s3)
        {
            // The api call used in this method equates to S3's Get api and is
            // suitable for smaller files. To download larger files, with
            // automatic usage of S3's multi-part apis for files over 5MB in size,
            // consider using the TransferUtility class in the Amazon.S3.Transfer
            // namespace.
            // See https://docs.aws.amazon.com/AmazonS3/latest/dev/HLuploadFileDotNet.html.
            var req = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            Task<GetObjectResponse> res = s3.GetObjectAsync(req);
            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                using (var reader = new StreamReader(res.Result.ResponseStream))
                {
                    Console.WriteLine("Retrieved contents of object '{0}' in bucket '{1}'", key, bucketName);
                    Console.WriteLine(reader.ReadToEnd());
                }
            }
        }

        static void ListBuckets(IAmazonS3 s3)
        {
            // listing buckets takes no request parameters, so a
            // parameterless override is used here instead of creating
            // a ListBucketsRequest object.
            Task<ListBucketsResponse> res = s3.ListBucketsAsync();
            Task.WaitAll(res);

            Console.WriteLine("List of S3 Buckets in your AWS Account");
            foreach (var bucket in res.Result.Buckets)
            {
                Console.WriteLine(bucket.BucketName);
            }
        }

        static void ListObjects(IAmazonS3 s3)
        {
            var req = new ListObjectsRequest
            {
                BucketName = bucketName,
                MaxKeys = 100
            };

            Task<ListObjectsResponse> res = s3.ListObjectsAsync(req);
            Task.WaitAll(res);

            Console.WriteLine("List of objects in your S3 Bucket '{0}'", bucketName);
            foreach (var s3Object in res.Result.S3Objects)
            {
                Console.WriteLine(s3Object.Key);
            }
        }

        static void DeleteObject(IAmazonS3 s3)
        {
            var req = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            Task<DeleteObjectResponse> res = s3.DeleteObjectAsync(req);
            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                Console.WriteLine("Deleted object '{0}' from bucket '{1}'", key, bucketName);
            }
        }

        static void DeleteBucket(IAmazonS3 s3)
        {
            // S3 requires that buckets are empty of objects before they can
            // be deleted. The SDK also contains a helper utility, AmazonS3Util,
            // in the Amazon.S3.Util namespace with various methods that allow
            // you to delete non-empty buckets.
            var req = new DeleteBucketRequest
            {
                BucketName = bucketName
            };

            Task<DeleteBucketResponse> res = s3.DeleteBucketAsync(req);
            Task.WaitAll(res);

            if (res.IsCompletedSuccessfully)
            {
                Console.WriteLine("Deleted bucket - '{0}'", bucketName);
            }
        }
    }
}
```

This sample code demonstrates a common pattern when developing with the AWS SDK for .NET, which is the use of a client object to represent an AWS service. That client object then exposes functionality via methods, such as the "*CreateBucket*" method in this example.

## Run the Sample

To run through the S3 functionality using the console application, use the following command to build (compile) and run the app:

```bash
dotnet run
```

If you want to run the app again without compiling, just pass the \--no-build flag:

```bash
dotnet run \--no-build
```
