using System.Collections.Generic;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.StepFunctions;
using Amazon.CDK.AWS.StepFunctions.Tasks;
using Constructs;

namespace EventDrivenCdk.Frontend
{
    public static class WorkflowStep
    {
        public static Pass GenerateCaseId(Construct scope)
        {
            return new Pass(scope, "GenerateCaseId", new PassProps()
            {
                Parameters = new Dictionary<string, object>(4)
                {
                    {"payload", JsonPath.EntirePayload},
                    {"uuid.$", "States.UUID()" },
                }
            });
        }

        public static DynamoPutItem StoreApiData(Construct scope, Table apiTable)
        {
            return new DynamoPutItem(scope, "StoreApiInput", new DynamoPutItemProps()
            {
                Table = apiTable,
                ResultPath = "$.output",
                Item = new Dictionary<string, DynamoAttributeValue>(1)
                {
                    {"PK", DynamoAttributeValue.FromString(JsonPath.StringAt("$.uuid"))},
                    {
                        "Data", DynamoAttributeValue.FromMap(new Dictionary<string, DynamoAttributeValue>(3)
                        {
                            {
                                "reviewIdentifier",
                                DynamoAttributeValue.FromString(JsonPath.StringAt("$.uuid"))
                            },
                            {
                                "reviewId",
                                DynamoAttributeValue.FromString(
                                    JsonPath.StringAt("$.uuid"))
                            },
                            {
                                "emailAddress",
                                DynamoAttributeValue.FromString(JsonPath.StringAt("$.payload.body.emailAddress"))
                            },
                            {
                                "reviewContents",
                                DynamoAttributeValue.FromString(JsonPath.StringAt("$.payload.body.reviewContents"))
                            },
                        })
                    }
                },
            });
        }

        public static EventBridgePutEvents PublishNewApiRequestEvent(Construct scope, EventBus publishTo)
        {
            return new EventBridgePutEvents(scope, "PublishEvent", new EventBridgePutEventsProps()
            {
                Entries = new EventBridgePutEventsEntry[1]
                {
                    new EventBridgePutEventsEntry
                    {
                        Detail = TaskInput.FromObject(new Dictionary<string, object>(1)
                        {
                            {"reviewId", JsonPath.StringAt("$.uuid")},
                            {"reviewIdentifier", JsonPath.StringAt("$.uuid")},
                            {"emailAddress", JsonPath.StringAt("$.payload.body.emailAddress")},
                            {"reviewContents", JsonPath.StringAt("$.payload.body.reviewContents")},
                            {"type", "newReview"},
                        }),
                        DetailType = "newReview",
                        Source = "event-driven-cdk.api",
                        EventBus = publishTo
                    }
                },
                ResultPath = "$.eventOutput",
            });
        }

        public static Pass FormatStateForHttpResponse(Construct scope)
        {
            return new Pass(scope, "FormatHTTPresponse", new PassProps()
            {
                Parameters = new Dictionary<string, object>(4)
                {
                    {"reviewId", JsonPath.StringAt("$.uuid")},
                    {"reviewIdentifier", JsonPath.StringAt("$.uuid")},
                    {"emailAddress", JsonPath.StringAt("$.payload.body.emailAddress")},
                    {"reviewContents", JsonPath.StringAt("$.payload.body.reviewContents")}
                }
            });
        }
    }
}