using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace AWSAppService
{
    namespace Data
    {
        [DynamoDBTable("Musiquizza")]
        public class Song : IData
        {
            //Change to enum-TEW override ToString
            private struct MusixMatchAPI
            { 
                public const string API_URL = "http://api.musixmatch.com/ws/1.1/matcher.lyrics.get?format=json&callback=result&";
                public const string API_APIKEY = "apikey=2dfada4b94093d135bf491a7965ec6ed";
                public const string API_TRACK_PARAM = "q_track=";
                public const string API_ARTIST_PARAM = "q_artist=";
            }

            private string _songTitle;
            private int _songID;
            private string _songArtist = String.Empty;
            private string _songLyric = String.Empty;

            [DynamoDBHashKey]
            public int SongID
            {
                get { return _songID; }
                set { _songID = value; }
            }

            [DynamoDBProperty("SongTitle")]
            public string Title
            {
                get { return _songTitle; }
                set { _songTitle = value; }
            }

            public string Artist
            {
                get { return _songArtist; }
                set { _songArtist = value; }
            }

            [DynamoDBIgnore]
            public string SongLyric
            {
                get { return _songLyric; }
                private set { _songLyric = value; }
            }

            public async Task GetLyrics()
            {
                HttpClient httpAPIClient; // new HttpClient();
              
                StringBuilder buildString = new StringBuilder(MusixMatchAPI.API_URL);
                string apiResult=String.Empty;

                if (this.Artist == String.Empty && this.Title == String.Empty)
                    { throw new MemberAccessException(); }

                buildString.AppendJoin('&', MusixMatchAPI.API_APIKEY, MusixMatchAPI.API_TRACK_PARAM + Uri.EscapeUriString(this.Title), MusixMatchAPI.API_ARTIST_PARAM + Uri.EscapeUriString(this.Artist));
                Console.WriteLine(buildString.ToString());

                using (httpAPIClient = new HttpClient())
                {
                    apiResult = await httpAPIClient.GetStringAsync(buildString.ToString());
                    Console.WriteLine(apiResult);
                }
               
                JObject apiJSON = JObject.Parse(apiResult);
                this.SongLyric = apiJSON["message"]["body"]["lyrics"]["lyrics_body"].ToString();


                }
            
        }
    }
}   
