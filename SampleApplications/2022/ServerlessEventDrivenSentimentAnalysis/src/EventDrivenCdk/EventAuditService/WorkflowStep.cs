using Amazon.CDK.AWS.StepFunctions.Tasks;
using Amazon.CDK.AWS.StepFunctions;
using Constructs;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventDrivenCdk.EventAuditService
{
    public static class WorkflowStep
    {
        public static CallAwsService QueryDynamo(Construct scope)
        {
            return new CallAwsService(scope, "QueryDynamo", new CallAwsServiceProps()
            {
                Service = "dynamodb",
                Action = "query",
                Parameters = new Dictionary<string, object>(3)
                {
                    {"TableName", "EventAuditStore"},
                    {"KeyConditionExpression", "PK = :v1" },
                    {"ExpressionAttributeValues", new Dictionary<string, object>() {
                        {":v1", new Dictionary<string, object>()
                            {
                                {"S", JsonPath.StringAt("$.querystring.reviewId") }
                            } 
                        }
                    }}
                },
                IamResources = new string[1] { "*" },
                ResultPath = "$.QueryResult"
            });
        }

        public static Pass FormatResponse(Construct scope)
        {
            return new Pass(scope, "FormatResponse", new PassProps()
            {
                Parameters = new Dictionary<string, object>(1)
                {
                    {"eventData", JsonPath.StringAt("$.SK.S")},
                }
            });
        }
    }
}
