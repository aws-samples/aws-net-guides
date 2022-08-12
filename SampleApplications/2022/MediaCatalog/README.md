# ML Integration - Media Catalog

This project contains a basic media catalog designed to highlight the integration of
AWS’s machine learning service Rekognition. The sample application uses Amazon Rekognition 
to moderate and tag content for the application.

## Project Structure:
### MLIntegration/CDK
This Directory contains a CDK project that creates all the backing infrastructure for the project. 
The infrastructure can be created using the CDK Deploy command into your AWS Account. Deployment 
of the stack is performed using the following command in the CDK directory.

```
CDK Deploy 
```

Instructions for installing CDK can be found here:
https://docs.aws.amazon.com/cdk/v2/guide/getting_started.html

### MLIntegration.sln
Main solution for the application. 

### MLIntegration/MLIntegration.Tests
Some basic unit tests for the application.

### MLIntegration/SampleImages
Some sample images that can be used to test the media catalog. All images in this directory were 
obtained from the Smithsonian Open Access platform.

In order to use this solution, users should first deploy the CDK project into their AWS Account. 
Once deployed, you can clean up the resources by removing the Cloud Formation stack that gets 
deployed. Once the CDK infrastructure has been deployed, you can run the application via 
Visual Studio, or deploy the solution into your AWS account. 
