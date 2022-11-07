using System.Collections.Generic;
using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.StepFunctions;
using Amazon.CDK.AWS.StepFunctions.Tasks;
using Constructs;
using EventDrivenCdk.SharedConstruct;

namespace EventDrivenCdk.EventAuditService
{
    public class EventAuditService : Construct
    {
        public EventAuditService(Construct scope, string id) : base(scope, id)
        {
            // Create table to store event audit data.
            var auditTable = new Table(this, "EventAuditStore", new TableProps()
            {
                TableName = "EventAuditStore",
                PartitionKey = new Attribute()
                {
                    Name = "PK",
                    Type = AttributeType.STRING
                },
                SortKey = new Attribute()
                {
                    Name = "SK",
                    Type = AttributeType.STRING
                },
                BillingMode = BillingMode.PAY_PER_REQUEST
            });

            // Simple workflow to take event bridge input and store in dynamodb.
            var sfnWorkflow = new DynamoPutItem(this, "StoreEventData", new DynamoPutItemProps()
            {
                Table = auditTable,
                Item = new Dictionary<string, DynamoAttributeValue>(3)
                {
                    { "PK", DynamoAttributeValue.FromString(JsonPath.StringAt("$.detail.reviewId")) },
                    { "SK", DynamoAttributeValue.FromString(JsonPath.Format("{}#{}", JsonPath.StringAt("$.time"), JsonPath.StringAt("$.detail.type"))) },
                    { "Data", DynamoAttributeValue.MapFromJsonPath("$.detail")}
                }
            });

            // Simple workflow to take event bridge input and store in dynamodb.
            var queryWorkflow = WorkflowStep.QueryDynamo(this)
                .Next(new Map(this, "LoopItems", new MapProps()
                {
                    ItemsPath = "$.QueryResult.Items"
                }).Iterator(WorkflowStep.FormatResponse(this)));

            var stateMachine =
                new DefaultStateMachine(this, "EventAuditStateMachine", sfnWorkflow, StateMachineType.EXPRESS);

            var apiStateMachine = new DefaultStateMachine(this, "EventAuditApiStateMachine", queryWorkflow, StateMachineType.EXPRESS);

            apiStateMachine.AddToRolePolicy(new PolicyStatement(new PolicyStatementProps()
            {
                Actions = new string[1] { "dynamodb:Query" },
                Resources = new string[1] { auditTable.TableArn }
            }));

            // Create the API
            var queryApi = new StepFunctionsRestApi(this, "EventAuditQueryApi", new StepFunctionsRestApiProps
            {
                StateMachine = apiStateMachine,
            });

            // Add rule to the central event bus.
            CentralEventBus.AddRule(this, "EventAuditRule",
                new string[4] {"event-driven-cdk.api", "event-driven-cdk.sentiment-analysis", "event-driven-cdk.notifications", "event-driven-cdk.customer-service"}, stateMachine);

            var output = new CfnOutput(this, "AuditApiEndpoint", new CfnOutputProps()
            {
                ExportName = "AuditAPIEndpoint",
                Description = "The endpoint for the created audit API",
                Value = queryApi.Url
            });
        }
    }
}