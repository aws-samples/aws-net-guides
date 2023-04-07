using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TMDBAlexa.Shared;

namespace TMDBAlexa.Skill
{
    public class LambdaEntryPoint
    {
        private readonly ILogger<LambdaEntryPoint> _logger;
        private readonly DynamoDBService _dbService;

        public LambdaEntryPoint(ILogger<LambdaEntryPoint> logger, DynamoDBService dbService)
        {
            _logger = logger;
            _dbService = dbService;
        }

        public async Task<SkillResponse> Handler(SkillRequest input)
        {            
            Type requestType = input.GetRequestType();
            if (input.GetRequestType() == typeof(LaunchRequest))
            {
                string speech = "Please provide a movie you would like to know more about";
                Reprompt rp = new Reprompt("Provide a movie");
                return ResponseBuilder.Ask(speech, rp);
            }
            else if (input.GetRequestType() == typeof(SessionEndedRequest))
            {
                return ResponseBuilder.Tell("Goodbye!");
            }
            else if (input.GetRequestType() == typeof(IntentRequest))
            {
                var intentRequest = (IntentRequest)input.Request;
                switch (intentRequest.Intent.Name)
                {
                    case "AMAZON.CancelIntent":
                    case "AMAZON.StopIntent":
                        return ResponseBuilder.Tell("Thanks for using the TMDB Skill!");
                    case "AMAZON.HelpIntent":
                        {
                            Reprompt rp = new Reprompt("What's next?");
                            return ResponseBuilder.Ask("Here's some help. What's next?", rp);
                        }
                    case "GetMovieInfo":
                        {
                            var movie = await _dbService.SearchTable(intentRequest.Intent.Slots["Movie"].Value);

                            if (movie != null)
                            {
                                return ResponseBuilder.Tell($"{movie.Title} was released on {DateTime.Parse(movie.ReleaseDate).ToShortDateString()}");
                            }
                            return ResponseBuilder.Tell($"No movies found, please try again");
                        }
                    default:
                        {
                            _logger.LogInformation($"Unknown intent: " + intentRequest.Intent.Name);
                            string speech = "I didn't understand - try again?";
                            Reprompt rp = new Reprompt(speech);
                            return ResponseBuilder.Ask(speech, rp);
                        }
                }
            }
            return ResponseBuilder.Tell("Thanks for using the TMDB Skill!");
        }
    }
}
