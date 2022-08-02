// <copyright file="FaceComparison.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace CompareImages
{
    public class FaceComparison
    {
        public FaceComparison(string batchId, string sourceImage, string targetImage)
        {
            this.Results = new List<ComparisonResult>();
            this.BatchId = batchId;
            this.SourceImage = sourceImage;
            this.TargetImage = targetImage;
        }

        public string BatchId { get; }

        public string SourceImage { get; }

        public string TargetImage { get; }

        public bool HasResults
        {
            get
            {
                return this.Results.Count > 0;
            }
        }

        public List<ComparisonResult> Results { get; }

        public struct ComparisonResult
        {
            public ComparisonResult(float left, float top, float similarity)
            {
                this.Left = left;
                this.Top = top;
                this.Similarity = similarity;
            }

            public float Left { get; }

            public float Top { get; }

            public float Similarity { get; }
        }
    }
}