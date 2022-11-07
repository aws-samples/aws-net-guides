using System.Collections.Generic;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.StepFunctions;
using Amazon.CDK.AWS.StepFunctions.Tasks;
using Constructs;

namespace EventDrivenCdk.ReviewAnalysisService
{
    public static class WorkflowStep
    {
        public static CallAwsService AnalyzeSentiment(Construct scope)
        {
            return new CallAwsService(scope, "CallSentimentAnalysis", new CallAwsServiceProps()
            {
                Service = "comprehend",
                Action = "detectSentiment",
                Parameters = new Dictionary<string, object>(2)
                {
                    {"LanguageCode", "en"},
                    {
                        "Text", JsonPath.StringAt("$.reviewContents")
                    }
                },
                IamResources = new string[1] {"*"},
                ResultPath = "$.SentimentResult"
            });
        }

        public static Pass AddTranslationToState(Construct scope)
        {
            return new Pass(scope, "AddTranslatedTextToState", new PassProps()
            {
                Parameters = new Dictionary<string, object>(4)
                {
                    {"dominantLanguage", JsonPath.StringAt("$.dominantLanguage")},
                    {"reviewIdentifier", JsonPath.StringAt("$.reviewIdentifier")},
                    {"reviewId", JsonPath.StringAt("$.reviewId")},
                    {"emailAddress", JsonPath.StringAt("$.emailAddress")},
                    {"reviewContents", JsonPath.StringAt("$.Translation.TranslatedText")},
                    {"originalReviewContents", JsonPath.StringAt("$.reviewContents")},
                }
            });
        }

        public static CallAwsService TranslateNonEnglishLanguage(Construct scope)
        {
            return new CallAwsService(scope, "TranslateNonEn", new CallAwsServiceProps()
            {
                Service = "translate",
                Action = "translateText",
                Parameters = new Dictionary<string, object>(3)
                {
                    {"SourceLanguageCode", JsonPath.StringAt("$.dominantLanguage")},
                    {"TargetLanguageCode", "en"},
                    {"Text", JsonPath.StringAt("$.reviewContents")},
                },
                IamResources = new string[1] {"*"},
                ResultPath = "$.Translation"
            });
        }

        public static Pass FormatLanguageResults(Construct scope)
        {
            return new Pass(scope, "FormatResult", new PassProps()
            {
                Parameters = new Dictionary<string, object>(4)
                {
                    {"dominantLanguage", JsonPath.StringAt("$.DominantLanguage.Languages[0].LanguageCode")},
                    {"reviewIdentifier", JsonPath.StringAt("$.detail.reviewIdentifier")},
                    {"reviewId", JsonPath.StringAt("$.detail.reviewId")},
                    {"emailAddress", JsonPath.StringAt("$.detail.emailAddress")},
                    {"reviewContents", JsonPath.StringAt("$.detail.reviewContents")},
                    {"originalReviewContents", JsonPath.StringAt("$.detail.reviewContents")},
                }
            });
        }

        public static CallAwsService DetectLanguage(Construct scope)
        {
            return new CallAwsService(scope, "DetectReviewLanguage", new CallAwsServiceProps()
            {
                Service = "comprehend",
                Action = "detectDominantLanguage",
                Parameters = new Dictionary<string, object>(2)
                {
                    {
                        "Text", JsonPath.StringAt("$.detail.reviewContents")
                    }
                },
                IamResources = new string[1] {"*"},
                ResultPath = "$.DominantLanguage"
            });
        }
        
        public static EventBridgePutEvents PublishEvent(Construct scope, string stepName, string eventName, EventBus publishTo)
        {
            return new EventBridgePutEvents(scope, stepName, new EventBridgePutEventsProps()
            {
                Entries = new EventBridgePutEventsEntry[1]
                {
                    new EventBridgePutEventsEntry
                    {
                        Detail = TaskInput.FromObject(new Dictionary<string, object>(7)
                        {
                            {"dominantLanguage", JsonPath.StringAt("$.dominantLanguage")},
                            {"reviewIdentifier", JsonPath.StringAt("$.reviewIdentifier")},
                            {"reviewId", JsonPath.StringAt("$.reviewId")},
                            {"emailAddress", JsonPath.StringAt("$.emailAddress")},
                            {"reviewContents", JsonPath.StringAt("$.reviewContents")},
                            {"originalReviewContents", JsonPath.StringAt("$.originalReviewContents")},
                            {"type", eventName},
                        }),
                        DetailType = eventName,
                        Source = "event-driven-cdk.sentiment-analysis",
                        EventBus = publishTo
                    }
                }
            });
        }
    }
}