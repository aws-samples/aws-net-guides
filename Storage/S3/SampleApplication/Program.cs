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
            // for more details and other constructor options.
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
