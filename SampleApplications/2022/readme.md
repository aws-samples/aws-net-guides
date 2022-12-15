## AWS .NET Samples

In 2022 AWS employees who love .NET on AWS got together to create a set of sample application that show how to use .NET with a variety of AWS services. All the samples are written in C#, and are easy to follow. None will take more than 45 minutes to complete, and the shortest will take as little as 5 minutes. The goal is to show you how easy it is to learn to use .NET with AWS  

Linked below you will find six sample applications. Each sample includes a readme file that describes a problem, and how it is solved. Full source code, and instructions for deployment are included in each sample.

### [AWS AI Services](https://github.com/aws-samples/aws-net-guides/tree/master/SampleApplications/2022/aws-ai-services-demo)

With AWS AI services, you can add capabilities like image and video analysis, natural language processing, personalized recommendations and translation, virtual assistants, and speech recognition. You can use each service standalone, or you can use them together to build a sophisticated AI-capable application. This sample application demonstrates the usage of some of the AWS AI Services to enrich your existing .NET applications with AI capabilities using AWS SDK for .NET.

### [AWS Text To Speech Assistant](https://github.com/aws-samples/aws-net-guides/tree/master/SampleApplications/2022/ServerlessTextToSpeech)

The serverless text to speech assistant is a .NET 6.0-based serverless application (using AWS SAM) that allows you to upload a PDF file into an S3 bucket. The file will be sent to Amazon Textract to read the text that is in the document. Then, the text from the document will be sent to Amazon Polly to convert the text into an MP3 file. The file will be available via a Lambda function, which will provide a pre-signed URL that you can use to retrieve the file from the output S3 bucket. Two SNS topics are provided, one to deliver a notification in the event of failure, and one for success.

### [Compare Faces](https://github.com/aws-samples/aws-net-guides/tree/master/Serverless/Serverless%20App%20with%20Dynamic%20Step%20Functions)

This sample shows how we can compare a photo against several other photos.

### [Event Driven Serverless CDK](https://github.com/aws-samples/aws-net-guides/tree/master/SampleApplications/2022/ServerlessEventDrivenSentimentAnalysis)

This sample shows how to build an AWS native, event driven, customer review analysis application. It uses serverless components and native AWS service integrations. The application is deployed using the AWS CDK, and is written in C#.

### [ML Integration - Media Catalog](https://github.com/aws-samples/aws-net-guides/tree/master/SampleApplications/2022/MediaCatalog)

This application catalogs images using Amazon Rekognition, letting you automatically apply moderation to images, and determine if images contain potentially offensive materials. Additionally, Rekognition will detect the content of images and build a cross reference between the items discovered, and the images stored.

### [Using AWS Lambda functions, a non-public SQL Server, and AWS Secrets Manager together](https://github.com/aws-samples/aws-net-guides/tree/master/SampleApplications/2022/LambdaVPCSecretsManager)

Securing your data can have unexpected side effects. In this sample you will see how making an AWS RDS SQL Server database non-publicly accessible, and using AWS Secrets Manager to store the database credentials can cause problems for AWS Lambda functions. Solutions using a VPC Endpoint, and a NAT Gateway are shown.