using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.Events.Targets;
using Amazon.CDK.AWS.Sagemaker;
using Amazon.CDK.AWS.StepFunctions;
using Amazon.CDK.AWS.StepFunctions.Tasks;
using Constructs;
using EventDrivenCdk.SharedConstruct;
using Newtonsoft.Json;
using EventBus = Amazon.CDK.AWS.Events.EventBus;

namespace EventDrivenCdk.NotificationService
{
    public class NotificationServiceProps
    {
        public EventBus CentralEventBus { get; set; }
    }
    
    public class NotificationService : Construct
    {
        public NotificationService(Construct scope, string id, NotificationServiceProps props) : base(scope, id)
        {
            var choice = new Choice(this, "EventTypeChoice")
                .When(Condition.StringEquals(JsonPath.StringAt("$.detail.type"), "customerServiceCaseClaimed"),
                    WorkflowStep.SendEmail(this, "SendCustomerServiceClaimedEmail", new SendEmailProps()
                    {
                        To = JsonPath.StringAt("$.detail.emailAddress"),
                        Subject = "Your case is being worked on",
                        Body = "Your case is being worked on",
                    }).Next(WorkflowStep.PublishEvent(this, "PublishCaseClaimedEvent", "caseClaimedEmailSent", props.CentralEventBus)))
                .When(Condition.StringEquals(JsonPath.StringAt("$.detail.type"), "positiveReview"),
                    WorkflowStep.SendEmail(this, "SendPositiveEmail", new SendEmailProps()
                    {
                        To = JsonPath.StringAt("$.detail.emailAddress"),
                        Subject = "Thankyou for your review",
                        Body = "Thankyou for your positive review",
                    }).Next(WorkflowStep.PublishEvent(this, "PublishPositiveEmailEvent", "positiveEmailSent", props.CentralEventBus)))
                
                .When(Condition.StringEquals(JsonPath.StringAt("$.detail.type"), "negativeReview"),
                    WorkflowStep.SendEmail(this, "SendNegativeEmail", new SendEmailProps()
                    {
                        To = JsonPath.StringAt("$.detail.emailAddress"),
                        Subject = "Sorry!",
                        Body =
                            "I'm sorry our product didn't meet your satisfaction. One of our customer service agents will be in touch shortly",
                    }).Next(WorkflowStep.PublishEvent(this, "PublishNegativeEmailEvent", "negativeEmailSent", props.CentralEventBus)));

            var stateMachine = new DefaultStateMachine(this, "NotificationServiceStateMachine", choice,
                StateMachineType.STANDARD);

            CentralEventBus.AddRule(this, "NotificationRule", new string[2] {"event-driven-cdk.sentiment-analysis", "event-driven-cdk.customer-service"},
                new string[3] {"positiveReview", "negativeReview", "customerServiceCaseClaimed"}, stateMachine);
        }
    }
}