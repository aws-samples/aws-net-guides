using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.IAM;
using Amazon.CDK.AWS.S3;
using Amazon.CDK.AWS.SNS;

namespace ServerlessDocProcessing.Lambda
{
    public class FunctionCollectionProps
    {
        public string EnvironmentName { get; init; }

        public string ResourceNamePrefix { get; init; }

        public string FunctionCodeBaseDirectory { get; init; }

        public IBucket TextractBucket { get; init; }

        public IBucket InputBucket { get; init; }

        public ITopic TextractTopic { get; init; }

        public IRole TextractRole { get; init; }

        public Table ProcessDataTable { get; init; }

        public Table QueryConfigTable { get; set; }
    }
}
