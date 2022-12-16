# Sample Application

Throughout this guide, we will be refactoring a sample application. This application is available in the AWS .NET Samples GitHub repository. 

Download the code [here](https://github.com/aws-samples/aws-net-guides).

At a command line, using your git client type:

```
git clone git@github.com:aws-samples/aws-net-guides.git
```

The sample application that we will use is in the sub folder: 

```
SampleApplications\2022\MediaCatalog\MediaLibrary4.8
```

The sample application has a required set of infrastructure that must be deployed into your account. The infrastructure is deployed using Cloud Formation. From the AWS Console, you can create a new CloudFormation stack and use the template located in the sample application.
```
aws-net-guides\SampleApplications\2022\MediaCatalog\CloudFormation\mediacatalog.yml
```

You can also deploy the stack from the AWS Command line using the following command in the aws-net-guides\SampleApplications\2022\MediaCatalog\CloudFormation\ folder.
```
aws cloudformation deploy --template-file MediaCatalog.yml --stack-name MediaCatalogStack --capabilities CAPABILITY_IAM
```

You should make a copy of the Sample Application code, as the actions in this guide will be destructive to the project and source code. For example copy the folder "SampleApplications\2022\MediaCatalog\MediaLibrary4.8" to "C:\Source\MediaLibrary4.8" as this will allow us to change the application source files, while still having the original source code available.

The rest of this guide will assume that your sample project is stored in "C:\Source\MediaLibrary4.8\".

[Next](./03-Sample-Tour.md) <br/>
[Back to Start](../README.md)