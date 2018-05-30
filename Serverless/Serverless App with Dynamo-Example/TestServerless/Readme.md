# Test AWS Serverless Application Project

This project consists of:
* serverless.template - an AWS CloudFormation Serverless Application Model template file declaring a Serverless function, API Gateway endpoint, and a DynamoDB table
* Function.cs - class file containing the C# methods to retrieve data from DyanmoDB and return the contents as JSON
* aws-lambda-tools-defaults.json - default argument settings for use with Visual Studio and command line deployment tools for AWS

The project's single AWS Lambda function is exposed through Amazon API Gateway as a HTTP *Get* operation. Edit the template to customize the function or add more functions and other resources needed by your application, and edit the function code in Function.cs. You can then deploy your Serverless application.

## Here are some steps to follow from Visual Studio:

To deploy your Serverless application, right click the project in Solution Explorer and select *Publish to AWS Lambda*.

To view your deployed application open the Stack View window by double-clicking the stack name shown beneath the AWS CloudFormation node in the AWS Explorer tree. The Stack View also displays the root URL to your published application.

## Here are some steps to follow to get started from the command line:

Once you have edited your template and code you can use the following command lines to deploy your application from the command line (these examples assume the project name is *EmptyServerless*):

Restore dependencies
```
    cd "TestServerless"
    dotnet restore
```

Deploy application
```
    cd "TestServerless/src/TestServerless"
    dotnet lambda deploy-serverless
```
