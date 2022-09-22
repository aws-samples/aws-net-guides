// <copyright file="S3Object.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DomainModel.EventBridgeEvent
{
    /// <summary>
    /// This class represents an S3 object.
    /// </summary>
    public class S3Object
    {
        /// <summary>
        /// Gets or sets the key for the object stored in S3.
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// Gets or sets the size of the object.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the etag of the object.
        /// </summary>
        public string? ETag { get; set; }

        /// <summary>
        /// Gets or sets the version ID of the object.
        /// </summary>
#if NETCOREAPP_3_1
            [System.Text.Json.Serialization.JsonPropertyName("version-id")]
#endif
        public string? VersionId { get; set; }

        /// <summary>
        /// Gets or sets a string used to determine event sequence in PUTs and DELETEs.
        /// </summary>
        public string? Sequencer { get; set; }
    }
}