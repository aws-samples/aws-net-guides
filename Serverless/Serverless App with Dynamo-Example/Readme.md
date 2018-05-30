# Serverless Application with .NET

## Step-by-Step Walkthrough
--------

**Overview and Services Used:**

This walk-through comprises a Microsoft Word doc, along with a .NET Core serverless application. The application provides a basic web service using Amazon API Gateway and AWS Lambda to retrieve reading list details stored in Amazon DynamoDB. In addition it uses AWS CloudFormation for deploying the application components to AWS.

The walk-through includes creating and configuring a simple Lambda function in either Visual Studio 2017, Visual Studio for Mac, or via the command line, testing the Lambda function, and then extending the application to retrieve data from Amazon DynamoDB.

+ Links to documentation

  * Amazon API Gateway Page: https://aws.amazon.com/apigateway/
  * Amazon API Gateway Developer Guide: https://docs.aws.amazon.com/apigateway/latest/developerguide/welcome.html
  * Amazon DynamoDB Page: https://aws.amazon.com/dynamodb/
  * Amazon DynamoDB Developer Guide: https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Introduction.html
  * AWS CloudFormation Page: https://aws.amazon.com/cloudformation/
  * AWS CloudFormation Developer Guide: https://docs.aws.amazon.com/AWSCloudFormation/latest/UserGuide/Welcome.html
  * AWS Lambda Page: https://aws.amazon.com/lambda/
  * AWS Lambda Developer Guide: https://docs.aws.amazon.com/lambda/latest/dg/welcome.html


+ Prerequisites
  * AWS CLI
  * .NET Core 2.x SDK or higher installed
  * AWS Account with credentials configured locally in Visual Studio or using the CLI
  * The AWS Lambda Tools, or AWS Toolkit for Visual Studio if using Visual Studio 2017
  * Optional: Visual Studio 2017 or Visual Studio Mac


+ External libraries:
	* NuGet Package Amazon.Lambda.Core
	* NuGet Package Amazon.Lambda.Serialization.Json
	* NuGet Package Amazon.Lambda.APIGatewayEvents
	* NuGet Package AWSSDK.DynamoDBv2
	* NuGet Package Newtonsoft.Json
	
N.B. Adding packages is included in the walk-through
