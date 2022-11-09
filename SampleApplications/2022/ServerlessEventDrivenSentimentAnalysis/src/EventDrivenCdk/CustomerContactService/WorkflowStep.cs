using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SNS.Subscriptions;
using Amazon.CDK.AWS.SQS;
using Amazon.CDK.AWS.StepFunctions;
using Amazon.CDK.AWS.StepFunctions.Tasks;
using Constructs;

namespace EventDrivenCdk.CustomerContactService
{
    public static class WorkflowStep
    {
        public static SnsPublish NotifyBadReview(Construct scope)
        {
            var negativeReviewNotification = new Topic(scope, "ReviewNotificationTopic", new TopicProps()
            {
                DisplayName = "Negative Review Notification",
                TopicName = "NegativeReviewNotification"
            });
            negativeReviewNotification.AddSubscription(new EmailSubscription("", new EmailSubscriptionProps()
            {
                
            }));
            
            return new SnsPublish(scope, "NotifyNewBadReview", new SnsPublishProps()
            {
                Topic = negativeReviewNotification,
                Message = TaskInput.FromText("There has been a new negative review"),
                ResultPath = "$.snsResult"
            });
        }

        public static SqsSendMessage WaitForCustomerAgentClaim(Construct scope)
        {
            var awaitingClaimQueue = new Queue(scope, "AwaitingClaimQueue", new QueueProps()
            {
                QueueName = "AwaitingClaim",
				VisibilityTimeout = Duration.Minutes(2)
            });

            return new SqsSendMessage(scope, "QueueForClaim", new SqsSendMessageProps()
            {
                Queue = awaitingClaimQueue,
                MessageBody = TaskInput.FromObject(new Dictionary<string, object>()
                {
                    {"Token", JsonPath.TaskToken},
                    {
                        "Payload", new Dictionary<string, object>()
                        {
                            {"emailAddress", JsonPath.StringAt("$.detail.emailAddress")},
                            {"reviewContent", JsonPath.StringAt("$.detail.reviewContents")},
                            {"originalReviewContents", JsonPath.StringAt("$.detail.originalReviewContents")},
                            {"reviewId", JsonPath.StringAt("$.detail.reviewId")},
                        }
                    },
                }),
                ResultPath = "$.claimResponse",
                IntegrationPattern = IntegrationPattern.WAIT_FOR_TASK_TOKEN,
            });
        }

        public static DynamoPutItem StoreCustomerServiceClaim(Construct scope)
        {
            var customerContactTable = new Table(scope, "CustomerContactClaim", new TableProps()
            {
                TableName = "CustomerContactTable",
                PartitionKey = new Attribute()
                {
                    Name = "PK",
                    Type = AttributeType.STRING
                },
                BillingMode = BillingMode.PAY_PER_REQUEST
            });

            return new DynamoPutItem(scope, "StoreCustomerServiceClaim", new DynamoPutItemProps()
            {
                Table = customerContactTable,
                ResultPath = "$.output",
                Item = new Dictionary<string, DynamoAttributeValue>(1)
                {
                    {"PK", DynamoAttributeValue.FromString(JsonPath.StringAt("$.detail.reviewId"))},
                    {
                        "Data", DynamoAttributeValue.FromMap(new Dictionary<string, DynamoAttributeValue>(3)
                        {
                            {
                                "reviewIdentifier",
                                DynamoAttributeValue.FromString(JsonPath.StringAt("$.detail.reviewIdentifier"))
                            },
                            {
                                "claimedBy",
                                DynamoAttributeValue.FromString(JsonPath.StringAt("$.claimResponse.ClaimedBy"))
                            },
                            {
                                "reviewId",
                                DynamoAttributeValue.FromString(JsonPath.StringAt("$.detail.reviewId"))
                            },
                            {
                                "emailAddress",
                                DynamoAttributeValue.FromString(JsonPath.StringAt("$.detail.emailAddress"))
                            },
                            {
                                "reviewContents",
                                DynamoAttributeValue.FromString(JsonPath.StringAt("$.detail.reviewContents"))
                            },
                        })
                    }
                },
            });
        }
    }
}