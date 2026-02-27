// <copyright file="FaceComparisonRequest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DomainModel
{
    public class FaceComparisonRequest
    {
        public FaceComparisonRequest(string batchId, string sourceBucketName, string sourceImage, string targetBucketName, string targetImage)
        {
            this.SourceImage = sourceImage;
            this.TargetImage = targetImage;
            this.BatchId = batchId;
            this.SourceBucketName = sourceBucketName;
            this.TargetBucketName = targetBucketName;
        }

        public string BatchId { get; }

        public string TargetImage { get; }

        public string TargetBucketName { get; }

        public string SourceImage { get; }

        public string SourceBucketName { get; }
    }
}
