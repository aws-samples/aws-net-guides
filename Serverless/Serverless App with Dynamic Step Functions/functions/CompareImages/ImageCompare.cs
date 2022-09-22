// <copyright file="ImageCompare.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CompareImages
{
    using Amazon.Rekognition;
    using Amazon.Rekognition.Model;

    public class ImageCompare
    {
        public ImageCompare() => this.RekognitionClient = new AmazonRekognitionClient();

        public ImageCompare(IAmazonRekognition client) => this.RekognitionClient = client;

        private IAmazonRekognition RekognitionClient { get; set; }

        public async Task<FaceComparison> CompareFacesAsync(string batchId, string sourceBucketName, string sourceImage, string targetBucketName, string targetImage)
        {
            float similarityThreshold = 70F;

            Amazon.Rekognition.Model.Image imageSource = new Amazon.Rekognition.Model.Image();
            try
            {
                imageSource.S3Object = new S3Object()
                {
                    Name = sourceImage,
                    Bucket = sourceBucketName,
                };
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load source image: " + sourceImage);
                throw;
            }

            Amazon.Rekognition.Model.Image imageTarget = new Amazon.Rekognition.Model.Image();
            try
            {
                imageTarget.S3Object = new S3Object()
                {
                    Name = targetImage,
                    Bucket = targetBucketName,
                };
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load target image: " + targetImage);
                throw;
            }

            CompareFacesRequest compareFacesRequest = new CompareFacesRequest()
            {
                SourceImage = imageSource,
                TargetImage = imageTarget,
                SimilarityThreshold = similarityThreshold,
            };
            Console.WriteLine("About to start comparison faces");

            // Call operation
            CompareFacesResponse compareFacesResponse;
            FaceComparison faceComparison = new FaceComparison(batchId, sourceImage, targetImage);

            try
            {
                compareFacesResponse = await this.RekognitionClient.CompareFacesAsync(compareFacesRequest);
            }
            catch (Amazon.Rekognition.Model.InvalidParameterException e)
            {
                Console.WriteLine("Failed to compare faces as the image does not contain a face : " + e);
                return faceComparison; 
            }   

            // Display results
            foreach (CompareFacesMatch match in compareFacesResponse.FaceMatches)
            {
                ComparedFace face = match.Face;
                BoundingBox position = face.BoundingBox;
                Console.WriteLine("Face at " + position.Left
                    + " " + position.Top
                    + " matches with " + match.Similarity
                    + "% confidence.");

                faceComparison.Results.Add(new FaceComparison.ComparisonResult(position.Left, position.Top, match.Similarity));
            }

            return faceComparison;
        }
    }
}