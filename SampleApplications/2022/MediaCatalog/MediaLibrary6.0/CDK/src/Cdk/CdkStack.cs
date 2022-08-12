using Amazon.CDK;
using Amazon.CDK.AWS.CloudFront;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.DynamoDB;
using Constructs;
using Amazon.CDK.AWS.CloudFront.Origins;
using Amazon.CDK.AWS.SSM;
using Amazon.CDK.AWS.IAM;

namespace Cdk
{
    public class CdkStack : Stack
    {
        internal CdkStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // Need to create an S3 Bucket
            Bucket projectBucket = new Bucket(this, "project-bucket", new BucketProps()
            {

            });

            // Need to create a Cloud Front Distribution
            OriginAccessIdentity originAccessIdentity = new OriginAccessIdentity(this, "OriginAccessIdentity");
            projectBucket.GrantRead(originAccessIdentity);

            Distribution projectDistribution = new Distribution(this, "project-distribution", new DistributionProps()
            {
                DefaultBehavior = new BehaviorOptions()
                {
                    Origin = new S3Origin(projectBucket, new S3OriginProps()
                    {
                        OriginAccessIdentity = originAccessIdentity
                    })
                }
            }); 

            // Need a DynamoDB Table to store the file metadata
            Table fileTable = new Table(this, "fileTable", new TableProps()
            {
                TableName = "MetadataService-files",
                ReadCapacity = 3,
                WriteCapacity = 3,
                PartitionKey = new Attribute ()
                {
                    Name = "keyname",
                    Type = AttributeType.STRING
                }
            });

            // Need a DynamoDB Table to store the image processing data
            Table imagesTable = new Table(this, "imagesTable", new TableProps()
            {
                TableName = "MetadataService-Images",
                ReadCapacity = 3,
                WriteCapacity = 3,
                PartitionKey = new Attribute()
                {
                    Name = "image",
                    Type = AttributeType.STRING
                }
            });

            // Need a DynamoDB Table to store the image processing data
            Table lookupTable = new Table(this, "lookupTable", new TableProps()
            {
                TableName = "MetadataService-Lookups",
                ReadCapacity = 3,
                WriteCapacity = 3,
                PartitionKey = new Attribute()
                {
                    Name = "label",
                    Type = AttributeType.STRING
                }
            });


            Role beanstalkRole = new Amazon.CDK.AWS.IAM.Role(this, "beanstalk-role", new RoleProps()
            {
                AssumedBy = new ServicePrincipal ("ec2.amazonaws.com"),
                ManagedPolicies = new IManagedPolicy[]
                {
                    ManagedPolicy.FromAwsManagedPolicyName ("CloudWatchLogsFullAccess"),
                    ManagedPolicy.FromAwsManagedPolicyName ("AmazonRekognitionReadOnlyAccess"),
                    ManagedPolicy.FromAwsManagedPolicyName ("AWSXrayFullAccess")
                }
            });
            projectBucket.GrantReadWrite(beanstalkRole);
            fileTable.GrantReadWriteData(beanstalkRole);
            imagesTable.GrantReadWriteData(beanstalkRole);
            lookupTable.GrantReadWriteData(beanstalkRole);

            CfnInstanceProfile beanstalkProfile = new CfnInstanceProfile(this, "beanstalk-profile", new CfnInstanceProfileProps()
            {
                InstanceProfileName = beanstalkRole.RoleName,
                Roles = new string[] { beanstalkRole.RoleName },
            });

            // Need to Store the Cloud Front URL in Parameter Store
            StringParameter cloudFrontUrl = new StringParameter(this, "cloud-front-url", new StringParameterProps()
            {
                StringValue = projectDistribution.DistributionDomainName,
                Description = "The base URL for the Cloud Front Distribution",
                ParameterName = "Cloud-Front-URL",
                SimpleName = true
            });
            cloudFrontUrl.GrantRead(beanstalkRole);

            // Need to Store the S3 Bucket in Parameter Store
            StringParameter mediaBucketName = new StringParameter(this, "media-bucket-name", new StringParameterProps()
            {
                StringValue = projectBucket.BucketName,
                Description = "The S3 Bucket that stores the files.",
                ParameterName = "Media-Bucket-name",
                SimpleName = true
            });
            mediaBucketName.GrantRead(beanstalkRole);

            // Store the DynamoDB table name.
            StringParameter fileMetaDataTable = new StringParameter(this, "file-metadata-table", new StringParameterProps()
            {
                StringValue = fileTable.TableName,
                Description = "Table where the file metadata gets placed after upload.",
                ParameterName = "File-Metadata-Table",
                SimpleName = true
            });
            fileMetaDataTable.GrantRead(beanstalkRole);

            // Store the DynamoDB table name.
            StringParameter imageMetaDataTableParameter = new StringParameter(this, "image-metadata-table", new StringParameterProps()
            {
                StringValue = imagesTable.TableName,
                Description = "Metadata about images, what Tags are included and saved in the image.",
                ParameterName = "Image-Metadata-Table",
                SimpleName = true
            });
            imageMetaDataTableParameter.GrantRead(beanstalkRole);

            // Store the DynamoDB table name.
            StringParameter lookupMetaDataTableParameter = new StringParameter(this, "lookup-metadata-table", new StringParameterProps()
            {
                StringValue = lookupTable.TableName,
                Description = "Metadata about images, what Tags are included and saved in the image.",
                ParameterName = "Lookup-Metadata-Table",
                SimpleName = true
            });
            lookupMetaDataTableParameter.GrantRead(beanstalkRole);

            // Beanstalk Role to use
            StringParameter beanstalkRoleParameter = new StringParameter(this, "beanstalk-role-rarameter", new StringParameterProps()
            {
                StringValue = beanstalkRole.RoleName,
                Description = "Role for your elastic beanstalk instance to use.",
                ParameterName = "Beanstalk-IAM-Role",
                SimpleName = true
            });
            beanstalkRoleParameter.GrantRead(beanstalkRole);

        }
    }
}
