using Amazon.DynamoDBv2.DataModel;
using System.Collections.Generic;

namespace MediaLibrary.Models
{
    [DynamoDBTable("MetadataService-Images")]
    public class ImageMetadataDataModel
    {
        [DynamoDBHashKey]
        [DynamoDBProperty("image")]
        public string Image { get; set; }

        public List<string> Labels { get; set; }
    }
}
