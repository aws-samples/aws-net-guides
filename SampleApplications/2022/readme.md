Linked below you will find six sample applications that demonstrate how to use .NET with various AWS services. Each sample includes a readme file that describes a problem, and how it is solved.
Full source code, and instructions for deployment are included in each sample. 

### [AWS AI Services](https://github.com/aws-samples/aws-net-guides/tree/master/SampleApplications/2022/aws-ai-services-demo)

With AWS AI services, you can add capabilities like image and video analysis, natural language processing, personalized recommendations and translation, virtual assistants, and speech recognition. You can use each service standalone, or you can use them together to build a sophisticated AI-capable application. This sample application is designed to demo the usage of some of the AWS AI Services to enrich your existing .NET applications with AI capabilities using AWS SDK for .NET.

### [Compare Faces](https://github.com/aws-samples/aws-net-guides/tree/master/Serverless/Serverless%20App%20with%20Dynamic%20Step%20Functions)

This sample demonstrates how we can compare a photo image against several other photos images.

### [Using AWS Lambda functions, a non-public SQL Server, and AWS Secrets Manager together](https://github.com/aws-samples/aws-net-guides/tree/master/SampleApplications/2022/LambdaVPCSecretsManager)

Securing your data can have unexpected side effects. In this sample you will see how making AWS RDS SQL Server database non publicly accessible, and using AWS Secrets Manager to store the database credentials can cause problems when the AWS Lambda functions, and how to solve it with a VPC Endpoint or a NAT Gateway.

### [ML Integration - Media Catalog](https://github.com/aws-samples/aws-net-guides/tree/master/SampleApplications/2022/MediaCatalog)

This application catalogs images using Amazon Rekognition. Using the functionality in this application users can take advantage of Rekognition's ability to automatically apply moderation to images, and determine if images contain potentially offensive materials. Additionally, Rekognition will detect the content of images and build a cross reference between the items discovered and the images stored.

### [Event Driven Serverless CDK](https://github.com/aws-samples/aws-net-guides/tree/master/SampleApplications/2022/ServerlessEventDrivenSentimentAnalysis)

This sample application demonstrates building an AWS native, event driven, customer review analysis application. It uses serverless components and native AWS service integrations. The application is deployed using the AWS CDK, and is written in C#.

### [AWS Text To Speech Assistant](https://github.com/aws-samples/aws-net-guides/tree/master/SampleApplications/2022/ServerlessTextToSpeech)

The serverless text to speech assistant is a .NET 6.0-based serverless application (using AWS SAM) that allows you to upload a PDF file into an S3 bucket. The file will be sent to Amazon Textract to read the text that is in the document. Then, the text from the document will be sent to Amazon Polly to convert the text into an MP3 file. The file will be available via a Lambda function, which will provide a pre-signed URL that you can use to retrieve the file from the output S3 bucket. Two SNS topics are provided, one to deliver a notification in the event of failure, and one for success.