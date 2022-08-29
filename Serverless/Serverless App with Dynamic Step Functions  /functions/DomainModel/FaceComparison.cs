// <copyright file="FaceComparison.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DomainModel
{
    public class FaceComparison
    {
        public FaceComparison()
        {
            this.FaceComparisonRequest = new List<FaceComparisonRequest>();
        }

        public List<FaceComparisonRequest> FaceComparisonRequest { get; }

        public bool HasFaceComparisonRequestItems
        {
            get
            {
                return this.FaceComparisonRequest.Count > 0;
            }
        }
    }
}