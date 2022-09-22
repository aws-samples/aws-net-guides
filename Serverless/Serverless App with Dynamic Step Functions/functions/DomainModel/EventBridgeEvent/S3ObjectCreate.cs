// <copyright file="S3ObjectCreate.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DomainModel.EventBridgeEvent
{
    /// <summary>
    /// This class represents the details of an S3 object create event sent via EventBridge.
    /// </summary>
    public class S3ObjectCreate : FaceComparisonDetails
    {
        /// <summary>
        /// Gets or sets the source IP of the API request.
        /// </summary>
#if NETCOREAPP_3_1
            [System.Text.Json.Serialization.JsonPropertyName("source-ip-address")]
#endif
        public string? SourceIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the reason the event was fired.
        /// </summary>
        public string? Reason { get; set; }
    }
}