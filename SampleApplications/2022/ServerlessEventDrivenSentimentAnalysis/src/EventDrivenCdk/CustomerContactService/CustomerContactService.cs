using System.Collections.Generic;
using Amazon.CDK.AWS.StepFunctions;
using Constructs;
using EventDrivenCdk.SharedConstruct;

namespace EventDrivenCdk.CustomerContactService
{
    public class CustomerContactService : Construct
    {
        public CustomerContactService(Construct scope, string id) : base(scope, id)
        {
            var workflow =
                // First notify agents that a bad review has been entered
                WorkflowStep.NotifyBadReview(this)
                    // Then send to a queue waiting for a customer service agent to claim
                    .Next(WorkflowStep.WaitForCustomerAgentClaim(this))
                    // Then store the customer service agent claim in a database
                    .Next(WorkflowStep.StoreCustomerServiceClaim(this))
                    .Next(CentralEventBus.PublishEvent(this, "PublishClaimEvent", "event-driven-cdk.customer-service", "customerServiceCaseClaimed", TaskInput.FromObject(new Dictionary<string, object>(1)
                    {
                        {"reviewId", JsonPath.StringAt("$.detail.reviewId")},
                        {"claimedBy", JsonPath.StringAt("$.claimResponse.ClaimedBy")},
                        {"emailAddress", JsonPath.StringAt("$.detail.emailAddress")},
                        {"type", "customerServiceCaseClaimed"},
                    })));
            
            // Create the customer contact workflow.
            var stateMachine = new DefaultStateMachine(this, "CustomerContactWorkflow",
                workflow, StateMachineType.STANDARD);

            CentralEventBus.AddRule(this, "NegativeReviewRule", "event-driven-cdk.sentiment-analysis",
                "negativeReview", stateMachine);
        }
    }
}