namespace DomainModel.EventBridgeEvent
{
    public class FaceComparisonDetails
    {
        /// <summary>
        /// Gets or sets the version of the event.
        /// </summary>
        public string? Version { get; set; }

        /// <summary>
        /// Gets or sets the bucket details.
        /// </summary>
        public Bucket? Bucket { get; set; }

        /// <summary>
        /// Gets or sets the object details.
        /// </summary>
        public S3Object? Object { get; set; }

        /// <summary>
        /// Gets or sets the ID of the API request.
        /// </summary>
#if NETCOREAPP_3_1
            [System.Text.Json.Serialization.JsonPropertyName("request-id")]
#endif
        public string? RequestId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the API requester.
        /// </summary>
        public string? Requester { get; set; }
    }
}