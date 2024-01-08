using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.Events;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.SNS;
using Amazon.CDK.AWS.SQS;
using System.Collections.Generic;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;
using Rule = Amazon.CDK.AWS.Events.Rule;

namespace ServerlessDocProcessing.Infrastructure;

public class InfrastructureConstruct : Construct
{
    private string EnvironmentName { get; }
    private string ResourceNamePrefix { get; }

    public Table ConfigTable { get; init; }
    public Table DataTable { get; init; }

    public Bucket InputBucket { get; init; }
    public Bucket TextractBucket { get; init; }

    public Queue InputDLQ { get; init; }
    public Queue SuccessQueue { get; init; }
    public Queue FailureQueue { get; init; }

    public Topic TextractTopic { get; init; }

    public Rule InputBucketRule { get; init; }

    public Role EventRole { get; init; }
    public Role TextractRole { get; set; }

    public InfrastructureConstruct(Construct scope, string id, InfrastructureProps props)
        : base(scope, id)
    {
        EnvironmentName = props.EnvironmentName;
        ResourceNamePrefix = props.ResourceNamePrefix;

        /// =====================================
        // DynamoDB tables
        /// =====================================

        // Table that contains query information
        ConfigTable = new(this, "queryData", new TableProps
        {
            TableName = GetTableName("QueryData"),
            PartitionKey = new Attribute { Name = "query", Type = AttributeType.STRING },
            BillingMode = BillingMode.PAY_PER_REQUEST,
            RemovalPolicy = RemovalPolicy.DESTROY
        });

        // Main data table for the application that contans all the information about the process
        DataTable = new(this, "dataTable", new TableProps
        {
            TableName = GetTableName("ProcessData"),
            PartitionKey = new Attribute { Name = "id", Type = AttributeType.STRING },
            BillingMode = BillingMode.PAY_PER_REQUEST,
            RemovalPolicy = RemovalPolicy.DESTROY
        });

        // Secondary index for querying the table by executionname (needed for the failure step)
        DataTable.AddGlobalSecondaryIndex(new GlobalSecondaryIndexProps
        {
            IndexName = "executionIndex",
            PartitionKey = new Attribute() { Name = "execution", Type = AttributeType.STRING },
            ProjectionType = ProjectionType.ALL
        });


        /// =====================================
        /// Storage - S3 Buckets
        /// =====================================

        // Bucket that the client application uploads files to
        InputBucket = new(this, "inputBucket", new BucketProps
        {
            Encryption = BucketEncryption.S3_MANAGED,
            BucketName = GetBucketName("input"),
            EventBridgeEnabled = true,
            RemovalPolicy = RemovalPolicy.DESTROY,
            AutoDeleteObjects = true
        });

        // Intermediate bucket where textract results are stored
        TextractBucket = new(this, "textractBucket", new BucketProps
        {
            Encryption = BucketEncryption.S3_MANAGED,
            BucketName = GetBucketName("textract"),
            RemovalPolicy = RemovalPolicy.DESTROY,
            AutoDeleteObjects = true
        });

        /// =====================================
        /// Messaging (SQS / SNS / Event Bridge)
        /// =====================================

        // Queue that the consuming application will poll to get results
        SuccessQueue = new(this, "successQueue", new QueueProps
        {
            QueueName = GetQueueName("successQueue")
        });

        // Queue that the consuming application will poll to get failure messages
        FailureQueue = new(this, "failureQueue", new QueueProps
        {
            QueueName = GetQueueName("failureQueue"),

        });

        // Dead leatter queue for the EventBridge rule
        InputDLQ = new(this, "inputDlq", new QueueProps
        {
            Encryption = QueueEncryption.SQS_MANAGED,
            EnforceSSL = true,
        });

        // The SNS topuic that Textract will use to notify analysis completion
        TextractTopic = new(this, "textractSuccessTopic", new TopicProps
        {
            Fifo = false,
            TopicName = GetTopicname("TextractSuccess"),
            DisplayName = "Textract Success Topic"
        });

        // EventBridge Rule that reacts to S3
        InputBucketRule = new(this, "inputBucketRule", new RuleProps
        {
            Enabled = true,
            RuleName = GetEventbridgeRuleName("input"),
            EventPattern = new EventPattern
            {

                Source = new[] { "aws.s3" },
                DetailType = new[] { "Object Created" },
                Detail = new Dictionary<string, object> {
                     {
                         "bucket", new Dictionary<string, object> { {"name", new [] {InputBucket.BucketName } }
                     }
                }
            }
            }
        });

        /// =====================================
        /// Security Structures (roles etc)
        /// =====================================

        // This is the role that the eventbridge rule will use 
        EventRole = new(this, "inputEventRole", new RoleProps
        {
            AssumedBy = new ServicePrincipal("events.amazonaws.com")
        });

        // The role that Textract will assume (so it can publish to SNS, write to S3 etc...)
        TextractRole = new(this, "textractRole", new RoleProps
        {
            AssumedBy = new ServicePrincipal("textract.amazonaws.com")
        });

        // Allows textract to read and write an S3 bucket
        TextractRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
        {
            Actions = new[] { "s3:Get*", "s3:Write*" },
            Resources = new[] { TextractBucket.BucketArn },
            Effect = Effect.ALLOW
        }));

        // Allows textract to publish to a topic
        TextractRole.AddToPolicy(new PolicyStatement(new PolicyStatementProps
        {
            Actions = new[] { "sns:Publish" },
            Resources = new[] { TextractTopic.TopicArn },
            Effect = Effect.ALLOW
        }));
    }



    // Functions to create unique names
    private string GetEventbridgeRuleName(string basename) => $"{ResourceNamePrefix}-{basename}-{EnvironmentName}";
    private string GetQueueName(string baseName) => $"{ResourceNamePrefix}-{baseName}-{EnvironmentName}-{Aws.ACCOUNT_ID}";
    private string GetTableName(string baseName) => $"{EnvironmentName}-{baseName}";
    private string GetBucketName(string baseName) => $"{ResourceNamePrefix}-{baseName}-{EnvironmentName}-{Aws.ACCOUNT_ID}-{Aws.REGION}";
    private string GetTopicname(string baseName) => $"{ResourceNamePrefix}-{baseName}-{EnvironmentName}-{Aws.ACCOUNT_ID}";
}
