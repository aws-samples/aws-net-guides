// <copyright file="Function.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using DomainModel;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.CamelCaseLambdaJsonSerializer))]

namespace GetTargetImages
{
    public class Function
    {
        private const string TargetBucketName = "photos-target-images";

        private const string SourceBucketName = "photos-source-images";

        public Function() => this.S3Client = new AmazonS3Client();

        public Function(IAmazonS3 client) => this.S3Client = client;

        private IAmazonS3 S3Client { get; set; }

        public async Task<FaceComparison> FunctionHandler(DomainModel.EventBridgeEvent.S3ObjectCreateEvent s3ObjectCreatedEvent, ILambdaContext context)
        {
            FaceComparison faceComparison = new FaceComparison();

            try
            {
                Console.WriteLine("e is {0}", s3ObjectCreatedEvent);
                Console.WriteLine("e detail is {0}", s3ObjectCreatedEvent.Detail);

                if (s3ObjectCreatedEvent.Detail == null)
                {
                    return faceComparison;
                }

                if (s3ObjectCreatedEvent.Detail.Bucket == null
                    || s3ObjectCreatedEvent.Detail.Bucket.Name == null)
                {
                    return faceComparison;
                }

                if (s3ObjectCreatedEvent.Detail.Object == null
                    || s3ObjectCreatedEvent.Detail.Object.Key == null)
                {
                    return faceComparison;
                }

                Console.WriteLine("bucket name is {0}", s3ObjectCreatedEvent.Detail.Bucket.Name);
                Console.WriteLine("object name is {0}", s3ObjectCreatedEvent.Detail.Object.Key);

                string sourceObjectKey = s3ObjectCreatedEvent.Detail.Object.Key;

                ListObjectsRequest request = new ListObjectsRequest
                {
                    BucketName = TargetBucketName,
                };

                ListObjectsResponse listObjectsResponse = await this.S3Client.ListObjectsAsync(request, default(CancellationToken));

                do
                {
                    List<S3Object> s3Objects = listObjectsResponse.S3Objects;
                    var batchId = Guid.NewGuid().ToString();

                    foreach (S3Object s3Object in s3Objects)
                    {
                        Console.WriteLine("Key " + s3Object.Key);
                        faceComparison.FaceComparisonRequest.Add(new FaceComparisonRequest(batchId, SourceBucketName, sourceObjectKey, TargetBucketName, s3Object.Key));
                    }

                    request.Marker = listObjectsResponse.NextMarker;
                    listObjectsResponse = await this.S3Client.ListObjectsAsync(request);
                }
                while (listObjectsResponse.IsTruncated);
                Console.WriteLine("Total Count is " + faceComparison.FaceComparisonRequest.Count());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error while processing " + e);
            }

            return faceComparison;
        }
    }
}
