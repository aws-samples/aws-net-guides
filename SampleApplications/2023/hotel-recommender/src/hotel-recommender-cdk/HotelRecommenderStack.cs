using Amazon.CDK;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.Personalize;
using System.IO;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3.Deployment;


namespace HotelRecommender
{
    public class HotelRecommenderStack : Stack
    {
        public readonly string LOGICALID = "hotel-picker";
        internal HotelRecommenderStack(App scope, string id, IStackProps props = null) : base(scope, id, props)
        {

            var bucket = new Bucket(this, "CfnHotelRecommenderBucket", new BucketProps
            {
                BlockPublicAccess = BlockPublicAccess.BLOCK_ALL,
                Encryption = BucketEncryption.S3_MANAGED,
                EnforceSSL = true,
                Versioned = true,
                RemovalPolicy = RemovalPolicy.RETAIN,
                BucketName = LOGICALID
            });

            var result = bucket.AddToResourcePolicy(
            new PolicyStatement(new PolicyStatementProps
            {
                Effect = Effect.ALLOW,
                Actions = new[] { "s3:GetObject", "s3:ListBucket" },
                Resources = new[] { $"arn:aws:s3:::{bucket.BucketName}", $"arn:aws:s3:::{bucket.BucketName}/*" },
                Principals = new[] { new ServicePrincipal("personalize.amazonaws.com") }
            }));

            var deployment = new BucketDeployment(this, "DeployWebsite", new BucketDeploymentProps
            {
                Sources = new[] {
                    Source.Asset("./src/hotel-recommender-cdk/data")
                    },
                DestinationBucket = bucket
            });

            var personalizeRole = new Role(this, "PersonalizeRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("personalize.amazonaws.com"),
                Description = "Role for Personlize to access to Access S3...",
                RoleName = $"{LOGICALID}-PersonalizeRole"
            });

            personalizeRole.AddManagedPolicy(
                ManagedPolicy.FromAwsManagedPolicyName("AmazonS3ReadOnlyAccess")
            );

            personalizeRole.AddManagedPolicy(
                ManagedPolicy.FromAwsManagedPolicyName("service-role/AmazonPersonalizeFullAccess")
            );

            var cfnInteractSchema = new CfnSchema(this, "CfnHotelRecommenderInteractSchema", new CfnSchemaProps
            {
                Name = "interact-schema",
                Schema = File.ReadAllText(@"./src/hotel-recommender-cdk/interact-schema.json"),
            });

            var cfnUserSchema = new CfnSchema(this, "CfnHotelRecommenderUserSchema", new CfnSchemaProps
            {
                Name = "user-schema",
                Schema = File.ReadAllText(@"./src/hotel-recommender-cdk/user-schema.json"),
            });

            var cfnItemSchemaNoMarket = new CfnSchema(this, "CfnHotelRecommenderItemSchemaNoMarket", new CfnSchemaProps
            {
                Name = "item-schema-nomarket",
                Schema = File.ReadAllText(@"./src/hotel-recommender-cdk/item-schema-nomarket.json"),
            });

            var cfnDatasetGroup = new CfnDatasetGroup(this, "CfnHotelRecommenderDatasetGroup", new CfnDatasetGroupProps
            {
                Name = $"{LOGICALID}-DatasetGroup",
            });

            var cfnInteractionsDataset = new CfnDataset(this, "CfnInteractionsDataset", new CfnDatasetProps
            {
                DatasetGroupArn = cfnDatasetGroup.AttrDatasetGroupArn,
                DatasetType = "Interactions",
                Name = "interactions-ds",
                SchemaArn = cfnInteractSchema.AttrSchemaArn,
            });

            var cfnUsersDataset = new CfnDataset(this, "CfnUsersDataset", new CfnDatasetProps
            {
                DatasetGroupArn = cfnDatasetGroup.AttrDatasetGroupArn,
                DatasetType = "Users",
                Name = "users-ds",
                SchemaArn = cfnUserSchema.AttrSchemaArn,
            });

            var cfnItemsDataset = new CfnDataset(this, "CfnItemsDataset", new CfnDatasetProps
            {
                DatasetGroupArn = cfnDatasetGroup.AttrDatasetGroupArn,
                DatasetType = "Items",
                Name = "Items-ds",
                SchemaArn = cfnItemSchemaNoMarket.AttrSchemaArn,
            });          
        }
    }
}
