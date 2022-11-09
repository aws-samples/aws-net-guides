using System.Collections.Generic;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.Logs;
using Amazon.CDK.AWS.StepFunctions;
using Amazon.CDK.AWS.StepFunctions.Tasks;
using Constructs;
using EventDrivenCdk.SharedConstruct;
using EventBus = Amazon.CDK.AWS.Events.EventBus;
using LogGroupProps = Amazon.CDK.AWS.Events.Targets.LogGroupProps;

namespace EventDrivenCdk.ReviewAnalysisService
{
    public class ReviewAnalysisServiceProps
    {
        public EventBus CentralEventBus { get; set; }
    }

    public class ReviewAnalysisService : Construct
    {
        public ReviewAnalysisService(Construct scope, string id, ReviewAnalysisServiceProps props) :
            base(scope, id)
        {
            // Define workflow module to run sentiment analysis.
            var analyzeSentiment = WorkflowStep.AnalyzeSentiment(scope)
                // Publish a different event type depending on the sentiment results
                .Next(new Choice(this, "SentimentChoice")
                    .When(Condition.NumberGreaterThan("$.SentimentResult.SentimentScore.Positive", 0.95), 
                        WorkflowStep.PublishEvent(this, "PublishPositiveEvent", "positiveReview", props.CentralEventBus))
                    .When(Condition.NumberGreaterThan("$.SentimentResult.SentimentScore.Negative", 0.95), 
                        WorkflowStep.PublishEvent(this, "PublishNegativeEvent", "negativeReview", props.CentralEventBus))
                    .Otherwise(new Pass(this, "UnknownSentiment")));

            // Define workflow to run translation and call sentiment analysis module.
            var analyseSentiment = WorkflowStep.DetectLanguage(this)
                .Next(WorkflowStep.FormatLanguageResults(this))
                .Next(new Choice(this, "TranslateNonEnLanguage")
                    .When(Condition.Not(Condition.StringEquals(JsonPath.StringAt("$.dominantLanguage"), "en")),
                        WorkflowStep.TranslateNonEnglishLanguage(this) 
                        .Next(WorkflowStep.AddTranslationToState(this))
                        .Next(analyzeSentiment))
                    .Otherwise(analyzeSentiment));

            var stateMachine = new DefaultStateMachine(
                this, 
                "SentimentAnalysisStateMachine",
                analyseSentiment,
                StateMachineType.STANDARD);

            // Add rule to event bus.
            CentralEventBus.AddRule(this, "TriggerSentimentAnalysisRule", "event-driven-cdk.api", "newReview",
                stateMachine);
        }
    }
}