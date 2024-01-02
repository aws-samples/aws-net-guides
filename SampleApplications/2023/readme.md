## AWS .NET Samples

In 2023 AWS employees who love .NET on AWS got together to create a set of sample applications that show how to use .NET with a variety of AWS services. All the samples are written in C#, and are easy to follow. No sample will take more than 45 minutes to complete, and the shortest will take as little as 5 minutes. The goal is to show you how easy it is to use .NET with AWS.

Linked below you will find six sample applications. Each sample includes a readme file that describes a problem, and how it is solved. Full source code, and instructions for deployment are included in each sample.

### [AWS AI Services](https://github.com/aws-samples/aws-net-guides/tree/master/SampleApplications/2023/SeverlessDocumentAnalysis)

This is a sample project that is meant to show a full end-to-end serverless application using entirely AWS Serverless services. It uses [Amazon Textract](https://aws.amazon.com/pm/textract) to analyze a PDF file. You can preconfigure natural language queries that Textract will attempt to answer (e.g. 'What is the date of service of this invoice'). Also it will submit the document for expense document analysis, and return data about the document as a set of expense metadata elements.