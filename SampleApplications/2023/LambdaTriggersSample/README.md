# Lambda Triggers Sample

A sample app demonstrating an end-to-end mobile workflow using [.NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/?view=net-maui-7.0), + [Serverless AWS Lambda](https://docs.aws.amazon.com/lambda/latest/dg/lambda-csharp.html) + [AWS S3 Storage](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/csharp_s3_code_examples.html) in C#.

This sample demonstrates how to use AWS Lambda's [HTTP API Gateway Triggers](https://aws.amazon.com/blogs/developer/deploy-an-existing-asp-net-core-web-api-to-aws-lambda/) + [S3 Triggers](https://docs.aws.amazon.com/lambda/latest/dg/with-s3-example.html) to automatically generate thumbnails of an uploaded image from a mobile app.

1. The .NET MAUI mobile app captures a photo
2. The .NET MAUI mobile app uploads photo to AWS via an AWS Lambda using an API Gateway HTTP trigger
3. The AWS Lambda API Gateway Function saves the image to AWS S3 Storage
4. An AWS Lambda S3 Trigger automatically generates a downscaled thumbnail of the image and saves the thumbnail image back to S3 Storage
5. The .NET MAUI mobile app retrives the thumbnail image via an AWS Lambda using an API Gateway HTTP trigger and displays it on screen

![](https://user-images.githubusercontent.com/13558917/214541434-0244c7f0-cc13-4273-89b0-af5ffd9f9786.png)
