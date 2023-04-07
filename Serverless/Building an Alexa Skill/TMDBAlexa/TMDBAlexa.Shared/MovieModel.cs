using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMDBAlexa.Shared
{
    [DynamoDBTable("movies")]
    public class MovieModel
    {
        [DynamoDBHashKey("id")]
        public int Id { get; set; }
        [DynamoDBProperty("title")]
        public string Title { get; set; }
        [DynamoDBProperty("title_lower")]
        public string TitleLower { get; set; }
        [DynamoDBProperty("overview")]
        public string Overview { get; set; }
        [DynamoDBProperty("popularity")]
        public double Popularity { get; set; }
        [DynamoDBProperty("voteAverage")]
        public double VoteAverage { get; set; }
        [DynamoDBProperty("voteCount")]
        public int VoteCount { get; set; }
        [DynamoDBProperty("releaseDate")]
        public string ReleaseDate { get; set; }
    }
}
