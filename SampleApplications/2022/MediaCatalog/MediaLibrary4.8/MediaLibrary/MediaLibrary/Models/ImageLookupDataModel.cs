using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace MediaLibrary.Models
{
    [DynamoDBTable("MetadataService-Lookups")]
    public class ImageLookupDataModel
    {
        [DynamoDBHashKey]
        [DynamoDBProperty("label")]
        public string Label { get; set; }

        public List<string> Images { get; set; }

    }
}
