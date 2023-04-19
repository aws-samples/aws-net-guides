using Microsoft.Extensions.Options;
using MediaLibrary.Models;

namespace MediaLibrary.Services
{
    public class RekognitionModerationService : IModerationService
    {
        private readonly AwsSettings _configuration;
        private readonly ILogger _logger;



        public RekognitionModerationService(AwsSettings options, ILogger logger)
        {
            _configuration = options;
            _logger = logger;
        }

        public async Task<ModerationResultsViewModel> IsContentAllowed(string objectLocation)
        {
            Amazon.Rekognition.AmazonRekognitionClient client = new Amazon.Rekognition.AmazonRekognitionClient();

            var results = await client.DetectModerationLabelsAsync(new Amazon.Rekognition.Model.DetectModerationLabelsRequest()
            {
                MinConfidence = 50F,
                Image = new Amazon.Rekognition.Model.Image()
                {
                    S3Object = new Amazon.Rekognition.Model.S3Object()
                    {
                        Bucket = _configuration.BucketName,
                        Name = objectLocation
                    }
                }
            }); 

            ModerationResultsViewModel moderationResults = new ModerationResultsViewModel();
            moderationResults.ImageAllowed = results.ModerationLabels.Count == 0;
            if (results.ModerationLabels.Count > 0)
            {
                List<string> moderationFlags = new List<string>();
                foreach (var flag in results.ModerationLabels)
                {
                    float confidence = flag.Confidence;
                    if (!moderationFlags.Contains(flag.Name))
                    {
                        moderationFlags.Add(flag.Name);
                    }
                }
                moderationResults.ModerationFlags = moderationFlags.ToArray();
            }
            return moderationResults;


        }
    }
}
