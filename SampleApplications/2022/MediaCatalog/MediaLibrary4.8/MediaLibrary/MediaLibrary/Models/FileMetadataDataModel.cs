using Amazon.DynamoDBv2.DataModel;
using System;

namespace MediaLibrary.Models
{
    [DynamoDBTable("MetadataService-files")]
    public class FileMetadataDataModel
    {
        [DynamoDBHashKey]
        [DynamoDBProperty("keyname")]
        public string KeyName { get; set; }
        [DynamoDBProperty("origionalfilename")]
        public string OrigionalFileName { get; set; }
        [DynamoDBProperty("timestamp")]
        public DateTime TimeStamp { get; set; }
        [DynamoDBProperty("filetype")]
        public string FileType { get; set; }
        [DynamoDBProperty("imageurl")]
        public string ImageURL { get; set; }
        [DynamoDBProperty("isprocessed")]
        public bool IsProcessed { get; set; }
    }
}
