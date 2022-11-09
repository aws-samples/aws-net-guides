using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

namespace EventDriven.Front.Data
{
    public class RequestService
    {
        private readonly HttpClient httpClient;

        private readonly IConfiguration _configuration;

        public RequestService(HttpClient client, IConfiguration configuration)
        {
            this.httpClient = client;
            this._configuration = configuration;
        }

        public async Task<CreateReviewResponse> SendRequest(string emailAddress, string submitReviewContents)
        {
            var request = new CreateReviewRequest()
            {
                ReviewContents = submitReviewContents,
                EmailAddress = emailAddress,
                ReviewIdentifier = Guid.NewGuid().ToString()
            };

            var json = JsonSerializer.Serialize(request);

            Console.WriteLine(json);

            using var response = await httpClient.PostAsJsonAsync<CreateReviewRequest>(this._configuration["FrontEndApiEndpoint"], request);

            return JsonSerializer.Deserialize<CreateReviewResponse>(await response.Content.ReadAsStringAsync());
        }
    
        public async Task<IEnumerable<string>> GetReviewResults(string reviewId)
        {
            var response = await httpClient.GetStringAsync($"{this._configuration["AuditApiEndpoint"]}?reviewId={reviewId}");

            var parsedResponseData = JsonSerializer.Deserialize<List<QueryResponse>>(response);

            return parsedResponseData.Select(p => p.EventData);
        }
    }

    public class CreateReviewRequest
    {
        [JsonPropertyName("reviewIdentifier")]
        public string ReviewIdentifier { get; set; }

        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonPropertyName("reviewContents")]
        public string ReviewContents { get; set; }
    }

    public class CreateReviewResponse
    {
        [JsonPropertyName("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonPropertyName("reviewContents")]
        public string ReviewContents { get; set; }

        [JsonPropertyName("reviewId")]
        public string ReviewId { get; set; }

        [JsonPropertyName("reviewIdentifier")]
        public string ReviewIdentifier { get; set; }
    }

    public class QueryResponse
    {
        [JsonPropertyName("eventData")]
        public string EventData { get; set; }
    }

}
