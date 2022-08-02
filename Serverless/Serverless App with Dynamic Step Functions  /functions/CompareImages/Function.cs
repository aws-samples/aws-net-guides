// <copyright file="Function.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Amazon.Lambda.Core;
using Amazon.Rekognition;
using DomainModel;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.CamelCaseLambdaJsonSerializer))]

namespace CompareImages
{
    public class Function
    {
        public Function() => this.ImageCompare = new ImageCompare();

        public Function(IAmazonRekognition client) => this.ImageCompare = new ImageCompare(client);

        private ImageCompare ImageCompare { get; set; }

        public async Task<FaceComparison> FunctionHandler(FaceComparisonRequest objectEvent, ILambdaContext context)
        {
            Console.WriteLine("Target Image is {0}", objectEvent.TargetImage);

            try
            {
                return await this.ImageCompare.CompareFacesAsync(objectEvent.BatchId, objectEvent.SourceBucketName, objectEvent.SourceImage, objectEvent.TargetBucketName, objectEvent.TargetImage);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Exception occured while comparing faces {0}", e);
                throw;
            }
        }
    }
}
