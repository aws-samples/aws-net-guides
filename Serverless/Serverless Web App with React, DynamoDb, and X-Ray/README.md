# Serverless Web Application with React Frontend with .NET

## Step-by-Step Walkthrough
--------

**Overview and Services Used:**

This walk-through provides a .NET Core serverless web application with a React App on the frontend hosted using Lambda and Api Gateway through the Serverless Appliation Model(SAM). The application provides a basic web application using Cognito to access AWS resources such as DynamoDb to retrieve songs stored in Amazon DynamoDB and hit a third party lyric API for a musical quiz. It also has X-ray installed for application debugging. In addition it uses the SAM CloudFormation template for deploying the application components to AWS.


+ Note
  *Updates to this project will be continued to be made as it is still under code review. A word document to guide a user through the project will be   added as well.

+ Authors
  *kneekey23
  *tawalke

+ Working Code Example
  *https://www.youtube.com/watch?v=r0YtDzNKwEU

+ Links to documentation

  * Amazon API Gateway Page: https://aws.amazon.com/apigateway/
  * Amazon API Gateway Developer Guide: https://docs.aws.amazon.com/apigateway/latest/developerguide/welcome.html
  * Amazon DynamoDB Page: https://aws.amazon.com/dynamodb/
  * Amazon DynamoDB Developer Guide: https://docs.aws.amazon.com/amazondynamodb/latest/developerguide/Introduction.html
  * AWS CloudFormation Page: https://aws.amazon.com/cloudformation/
  * AWS CloudFormation Developer Guide: https://docs.aws.amazon.com/AWSCloudFormation/latest/UserGuide/Welcome.html
  * AWS Serverless Application Model Github Page: https://github.com/awslabs/serverless-application-model
  * AWS Lambda Page: https://aws.amazon.com/lambda/
  * AWS Lambda Developer Guide: https://docs.aws.amazon.com/lambda/latest/dg/welcome.html
  * AWS Cognito Page: https://aws.amazon.com/cognito/
  * AWS Cognito Developer Guide: https://docs.aws.amazon.com/cognito/latest/developerguide/what-is-amazon-cognito.html
  * AWS X-Ray Page: https://aws.amazon.com/xray/
  * AWS X-Ray .NET Blog: https://aws.amazon.com/blogs/developer/new-aws-x-ray-net-core-support/
  


+ Prerequisites
  * AWS CLI
  * .NET Core 2.x SDK or higher installed
  * AWS Account with credentials configured locally in Visual Studio or using the CLI
  * The AWS Lambda Tools, or AWS Toolkit for Visual Studio if using Visual Studio 2017
  * node.js installed
  * Optional: Visual Studio 2017 or Visual Studio Mac


+ External libraries:
	* NuGet Package Amazon.Lambda.AspNetCoreServer
	* NuGet Package AWSSDK.CognitoIdentity
	* NuGet Pacakge AWSXRayRecorder.Core
	* NuGet Package AWSXRayRecorder.Handlers.AspNetCore
	* NuGet Package Microsoft.AspNetCore.SpaServices.Extensions
	* NuGet Package Amazon.Lambda.Serialization.Json
	* NuGet Package Amazon.Lambda.APIGatewayEvents
	* NuGet Package AWSSDK.DynamoDBv2
	* NuGet Package Microsoft.VisualStudio.Web.CodeGeneration.Design
	

