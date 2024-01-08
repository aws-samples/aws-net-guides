# Serverless Document Analysis with .NET

## Introduction
This is a sample project that is meant to show a full end-to-end serverless application using entirely AWS Serverless services. It uses [Amazon Textract](https://aws.amazon.com/pm/textract) to analyze a PDF file. You can preconfigure natural language queries that Textract will attempt to answer (e.g. 'What is the date of service of this invoice'). Also it will submit the document for expense document analysis, and return data about the document as a set of expense metadata documents.

This demonstrates how you can use .NET for an end to end serverless document processing solution in AWS. This README will detail the solution in full. The service can be deployed into an AWS account, and because it is self contained, can serve as an addon to an existing application.

## What specifically does this sample demonstrate?
This solution is meant to be useful in real-world scenario, in which multiple technologies, techniques, and services are used. Specifically, this document analysis tool showcases the technologies listed below.

1. ### AWS Lambda with .NET
    - Custom runtime functions using .NET 8.0
    - Observability implemented using [Powertools for AWS Lambda (.NET)](https://docs.powertools.aws.dev/lambda/dotnet/)
    - [Lambda Annotations Framework](https://github.com/aws/aws-lambda-dotnet/blob/master/Libraries/src/Amazon.Lambda.Annotations/README.md) to implement dependency injection, with source generation to automatically create the "Main" method.
    
1. ### Infrastructure as Code with CDK
    - All infrastructure is expressed with [AWS CDK (C#/.NET)](https://docs.aws.amazon.com/cdk/v2/guide/work-with-cdk-csharp.html) with .NET 8.0.

1. ### Serverless AWS Services
    - Data and configuration are stored in an [Amazon DynamoDB](https://aws.amazon.com/dynamodb/) table. Data access uses the [.NET Object persistence model](https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DotNetSDKHighLevel.html) to simplify data access with POCO objects.
    - The Lambda functions are orchestrated using an [AWS Step Function](https://aws.amazon.com/step-functions/) standard workflow. Standard workflow was chosen because it supports the [Wait for Callback (task token)](https://docs.aws.amazon.com/step-functions/latest/dg/connect-to-services.html#connect-to-services-integration-patterns) integration pattern.
    - [Amazon Textract](https://aws.amazon.com/dynamodb/) provides document analysis (standard and expense) capabilities.
    - An [Amazon Event Bridge rule](https://docs.aws.amazon.com/eventbridge/latest/userguide/eb-rules.html) is used to automatically trigger the workflow when a document is uploaded to an [Amazon S3](https://aws.amazon.com./s3) bucket.
    - Feedback is provided to the client application through the use of two [Amazon SQS](https://aws.amazon.com/sqs) queues

## Overview of the solution

This is an overview of the process. The names of the resources are generic, since each deployment will yield resources with different physical names (to avoid resource name collission). Sone design decisions are noted below, but there are alternate ways of accomplishing some of the items.
![Overview of serverless document analysis](/SampleApplications/2023/ServerlessDocumentAnalysis/assets/doc-analysis-overview.jpg)

This application is self contained. We will refer to an external application that integrates with this system as the "client application". There can be more than one client application, and a client application that provides input (i.e. uploads a file) may be different than an application that responds to the output of the system.
1. A client application writes a PDF to the `InputBucket` S3 bucket. 

    - If the service has been configured to use natural language queries (explanation below), a subset of them can be specified using a colon separate list of query keys, supplied as a tag on the uploaded S3 object. For example:

        `Tag: "Queries"`

        `Value: 'q1:q2:q3'`

        If no queries are supplied, then all configured queries will be used.

    - The client can also supply an that will be passed through the entire system. This will allow the correlation of an uploaded file's result with the client's system. For example:

        `Tag: "Id"`

        `Value: "abc-12345"`         

    _Note: A client application must have permissions to write files to the InputBucket. A CloudFormation output is created when this is deployed, `inputBucketPolicyOutput`, that provides an example IAM policy that you can use to allow access to the bucket._

2. An EventBridge rule triggers the Step Function. 

3. The Step Function definition can be seen here. It consists of seven Lambda function integrations and two SQS integrations. Any unrecoverable errors (from any of the Lambda functions) are caught and sent to the `FailureFunction` function, which then writes a message to the `FailureQueue` with details for the client.

    ![Step Function Definition](/SampleApplications/2023/ServerlessDocumentAnalysis/assets/stepfunctions_graph.png)

4. The EventBridge message is parsed by the `InitializeProcessing` Lambda function, which creates a record in the `ProcessData` DynamoDB table. It also retrieves the query text from the `ConfigData` DynamoDB table for use in the next step.

5. In the `SubmitToTextract` Lambda function, the uploaded file is submitted to Textract for standard analysis. This step in the workflow uses the `Wait for Task Token` pattern; the step function will pause until restarted.

6. When Textract is complete, it writes the output to the `TextractBucket` S3 bucket and sends a message to the supplied SNS Topic, `TextractSuccessTopic`. The Lambda function `RestartStepFunction` then calls the _SendTaskSuccess_ or _SendTaskFailure_ depending on the Textract job status.

7. The function `ProcessTextractQueryResults` retrieves the results from the `TextractBucket` bucket, and writes all the query results to the `ProcessData` table.

8. In the `SubmitToTextractExpense` Lambda function, the uploaded file is submitted to Textract for expense analysis. This step in the workflow uses the `Wait for Task Token` pattern; the step function will pause until restarted.

9. When Textract is complete, step 6 is repeated, and the Step Function is restarted accordingly.

10. The function `ProcessTextractExpenseResults` retrieves the results from the `TextractBucket` bucket, and writes all the query results to the `ProcessData` table.

11. The `SuccessFunction` Lambda function formats the results from both query and expense analyses, and writes the data to the `SuccessQueue` queue.

_Note: A client application must have permissions access both the `SuccessQueue` and `FailureQueue`. A CloudFormation output is created for each when this application is deployed, `failureQueueOutput` and `successQueueOutput`. These provide example IAM policies that you can use to allow access to the queues._

## Codebase

This is a brief explanation of the solution's codebase.

`/assets` - Images and diagrams

`/functionss` - Lambda function source code

`/infrastructure` - CDK .NET project source code

## Deploying in your environment

### Prerequisites
To deploy this solution, you will need the following prerequisites.
- Clone this repository
- You will need an AWS account and and IAM user with adequate permissions to deploy resources. You will need to set up a [credentials profile](https://docs.aws.amazon.com/cli/latest/userguide/cli-configure-files.html). For the remainder of this exercise, we will assume the profile is named `my-profile`.
- Install and set up the [AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-quickstart.html)
- Install the [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Install the latest version of the [AWS CDK](https://docs.aws.amazon.com/cdk/v2/guide/getting_started.html), and [bootstrap the environment](https://docs.aws.amazon.com/cdk/v2/guide/bootstrapping.html);
- Install the [AWS Amazon.Lambda.Tools .NET Global CLI tools](https://docs.aws.amazon.com/lambda/latest/dg/csharp-package-cli.html)

### Build
Before you deploy the solution, you will need to build the .NET 8.0 Lambda functions. A script is included to build all the Lambda funtions, for both Windows and Linux/Mac.

Windows:
```
cd infrastructure
.\build.bat
```

Linux:
```
cd infrastructure
sh build.sh
```

The process will take several minutes. The Lambda function archives are output to the `infrasturcure/function-output/` directory

### Deploy

To deploy the CDK application, you will need to supply several context values. They are:

- `environmentName` - The environment deployed to (e.g. 'dev', 'test', prod)
- `stackName` - The name of the CloudFormation stack this will be deployed as. _Note: the stack name will have the environment name as a suffix._
- `functionDirectory` - The directory where the .NET Lambda functions archives are located. If not supplied, will default to './function-output'.
- `resourcePrefix` - A prefix used when physically naming resources. This must be all lowercase and alphanumeric. Defaults to 'docprocessing'

You can supply these [runtime context](https://docs.aws.amazon.com/cdk/v2/guide/context.html) values in several ways. For the purpose of this demo, you can use a local file.

Craete a file called `cdk.context.json` in the `infrastructure` directory.

Populate it similarily to the following:
```
{
    "environmentName":"dev",
    "stackName":"docAnalysis",
    "functionBaseDirectory":"./function-output",
    "resourcePrefix":"doc"
}
```

Synthesize the CDK stack with:

```
cdk synth
```
_Note:_ You can actually include the build in the synthesis step by adding the `--build` switch:

```
cdk synth --build .\build.bat
```

You can then deploy the stack with the following command:

```
cdk deploy --profile my-profile
```

### Configure queries

To configure natural language queries for your environment, you will need to add them to the "QueryData" dynamoDB table. Each record will represent one query. This is an example of what a query should look like:

```
{
    "query":"q1",
    "queryText":"What is the date of service?"
}
```

## Cleanup
You can remove the infrastructure by using the following command:

```
cdk destroy --profile my-profile
```
You can also manually delete the CloudFormation stack that was originally created.

This will delete any resources created, as well as any data contained within your S3 buckets or DynamoDB tables.

## TODO
These are some items that will be added at a later date to make the solution more extensible
1. Create a Systems Manager Parameter that will parameterize the following items:
    - The name of the tag that is used to specify queries to be applied to the document analysis (currently hardcoded to 'Queries')
1. Add a configuration swith that will enable selecting to build the .NET Lambda functions with Native AOT
