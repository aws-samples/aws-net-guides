# .NET 7 support on AWS

## Overview

This guide describes .NET 7 support provided by AWS services and tools. The guide will be updated as new support is added. 
At the moment .NET 7 is still in a pre-release stage, so levels of support may change leading up to the final release.

Related Content:
  * [GitHub home for .NET on AWS](https://github.com/aws/dotnet)
  * [AWS Developer Center - .NET on AWS](https://aws.amazon.com/developer/language/net/)

## Introduction

The latest release of .NET 7 brings some exciting new features to .NET including performance increases, and Native AoT compilation of .net code. 
.NET 7 can be used with a number of AWS services such as ([Amazon EC2](https://aws.amazon.com/ec2/)) where the runtime is installed manually. 

This guide is focused on AWS services and tools that have either been updated, or are being updated, to provide .NET 7 support. While we work to 
test and validate support for the GA release of .NET 6 to all services and tools, there are some steps you can take to use .NET 7 today. 
These are called out in the sections below. 

Please open an [Issue on this repository](https://github.com/aws-samples/aws-net-guides/issues) with questions about .NET 7 support on AWS. 
We use this feedback to help us prioritize updates. 


## Compute Services

### Amazon EC2

Customers can install .NET 7 on over 400 [Amazon EC2 instances types](https://aws.amazon.com/ec2/instance-types/). 

The following EC2 [User Data](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/user-data.html#user-data-console) example installs .NET 7 on a linux server.
```
#!/bin/bash
# Install .NET 7 SDK for Linux
sudo apt-get update -y
curl -O https://download.visualstudio.microsoft.com/download/pr/253e5af8-41aa-48c6-86f1-39a51b44afdc/5bb2cb9380c5b1a7f0153e0a2775727b/dotnet-sdk-7.0.100-linux-x64.tar.gz
mkdir /usr/bin/dotnet
sudo tar -zxvf dotnet-sdk-7.0.100-linux-x64.tar.gz -C /usr/bin/dotnet
sudo ln -s /usr/bin/dotnet/dotnet /usr/bin/dotnet
sudo sh -c 'echo "export DOTNET_ROOT=/usr/bin/dotnet" >> /etc/environment'
sudo sh -c 'echo "export PATH=$PATH:$DOTNET_ROOT" >> /etc/environment'
```

Install scripts for .NET can be found here:
https://dotnet.microsoft.com/en-us/download/dotnet/scripts

Customers can use automation facilities in the [AWS Systems Manager Service](https://aws.amazon.com/systems-manager) to automatically install .NET runtimes using 
automation documents, and use the [EC2 Image Builder](https://aws.amazon.com/image-builder/) service to precreate EC2 Images with the .NET Runtime pre-installed. 

### AWS Lambda
[AWS Lambda](https://aws.amazon.com/lambda/) supports the ability to create your own 
[custom runtime](https://docs.aws.amazon.com/lambda/latest/dg/runtimes-custom.html). Users who want to run .NET 7 applications can create their own custom runtime 
and include .NET 7.


### Containers

AWS customers can deploy .NET applications running on either Windows or Linux containers using [Amazon Elastic Container Software](https://aws.amazon.com/ecs/) (ECS) and 
[Amazon Elastic Kubernetes Service](https://aws.amazon.com/eks/) (EKS). [AWS Fargate](https://aws.amazon.com/fargate/) is a service that you can use to run and manage the 
lifecycle of ECS and EKS containers without the need to manage the container infrastructure yourself. 

[AWS App Runner](https://aws.amazon.com/apprunner/) is a fully managed service that makes it easy to quickly deploy containerized web applications and APIs, 
scaling up or down automatically to meet application traffic needs. To use with .NET 7 applications, upload an image with the .NET 7 application to 
[Amazon Elastic Container Registry](https://aws.amazon.com/ecr/) (ECR) and use the [source image](https://docs.aws.amazon.com/apprunner/latest/dg/service-source-image.html) support 
to configure AWS App Runner to start, run, scale, and load balance the application. 

[AWS Elastic Beanstalk](https://aws.amazon.com/elasticbeanstalk/) allows customers to package up their applications and deploy to elastic beanstalk as a container image. 
By using containers, customer can deplooy .NET 7 applications on Elastic Beanstalk. 

## Tools, Libraries, and SDK

### AWS Code Build
[AWS Code Build](https://aws.amazon.com/codebuild/) is a fully managed service to help developers automatically build applications from source code. The code build service allows the user to customize the build environment, to suit the needs of the application being built. This includes the ability to install additional .NET runtimes. Users can add support for building .NET 7 applications by adding the following snippet to their applications buildspec.yml file.

```
  install:
    commands:
      - curl -O https://download.visualstudio.microsoft.com/download/pr/253e5af8-41aa-48c6-86f1-39a51b44afdc/5bb2cb9380c5b1a7f0153e0a2775727b/dotnet-sdk-7.0.100-linux-x64.tar.gz
      - sudo tar -zxvf dotnet-sdk-7.0.100-linux-x64.tar.gz -C /root/.dotnet
```
This will automatically download an install the .NET 7 SDK as part of the Install phase of CodeBuild.


### AWS Toolkit for Visual Studio

The [AWS Toolkit for Visual Studio](https://aws.amazon.com/visualstudio/) is an extension for Microsoft Visual Studio on Windows that makes it easier for developers to develop, debug, 
and deploy .NET applications using Amazon Web Services. Visual Studio 2022 supports .NET 7 development, and customers can download the AWS Toolkit for 
[Visual Studio 2022](https://marketplace.visualstudio.com/items?itemName=AmazonWebServices.AWSToolkitforVisualStudio2022) from the Visual Studio Marketplace. 

### AWS Toolkit for Rider

The [AWS Toolkit for Rider](https://aws.amazon.com/rider/) is an open source plug-in for the [JetBrains Rider](https://www.jetbrains.com/rider/) IDE that makes it easier to create, debug, and deploy .NET applications on Amazon Web Services. The Toolkit supports creating a new AWS App Runner service to manage containers, which can host .NET 6 applications. Follow the documentation [here](https://docs.aws.amazon.com/toolkit-for-jetbrains/latest/userguide/creating-service-apprunner.html) to setup. Note, select the "ECR/ECR public" option (image source) for .NET applications. 

### AWS Toolkit for Visual Studio Code

The [AWS Toolkit for Visual Studio Code](https://aws.amazon.com/visualstudiocode/) is an open source plug-in for the Visual Studio Code editor that makes it easier to create, 
debug, and deploy applications on Amazon Web Services. The Toolkit supports creating a new AWS App Runner service to manage containers, which can host .NET 7 applications. 

### AWS Toolkit for Azure DevOps

The [AWS Toolkit for Azure DevOps](https://aws.amazon.com/vsts/) is an extension for hosted and on-premises Microsoft Azure DevOps that makes it easy to manage and deploy applications to AWS. .NET 7 applications can be used with the Toolkit.

## AWS SDK for .NET

The [AWS SDK for .NET](https://github.com/aws/aws-sdk-net) allows .NET developers to integrate AWS services into their application code in a familiar and consistent manner. The library is compatible with .NET 7, and is available from [NuGet](https://www.nuget.org/packages/awssdk.core/). Learn how to get started with the 
[AWS SDK for .NET in the Developer Guide](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/welcome.html). 

### AWS App2Container
[AWS App2Container (A2C)](https://aws.amazon.com/app2container/) is a command-line tool for modernizing .NET and Java applications into containerized applications. A2C analyzes and builds an inventory of all applications running in virtual machines, on-premises or in the cloud. Applications are selected to be containerized, and A2C packages the application artifact and identified dependencies into container images, configures the network ports, and generates the ECS task and Kubernetes pod definitions. A2C provisions, through CloudFormation, the cloud infrastructure and CI/CD pipelines required to deploy the containerized .NET or Java application into production. 

## Security and Diagnostics

### AWS Secrets Manager

[AWS Secrets Manager](https://aws.amazon.com/secrets-manager/) helps you protect secrets needed to access your applications, services, and IT resources. 
The service enables you to easily rotate, manage, and retrieve database credentials, API keys, and other secrets throughout their lifecycle. 

The [AWS Secrets Manager Caching Client for .NET](https://github.com/aws/aws-secretsmanager-caching-net) enables in-process caching of secrets and is compatible with .NET 7. 
Please refer to this [blog post](https://aws.amazon.com/blogs/security/how-to-use-aws-secrets-manager-client-side-caching-in-dotnet/) to learn more about how to use 
AWS Secrets Manager client-side caching in .NET.

### AWS X-Ray

[AWS X-Ray](https://aws.amazon.com/xray/) helps developers analyze and debug distributed applications, such as those built using a microservices architecture. 
.NET 7 applications can integrate AWS X-Ray with [AWS X-Ray SDK for .NET](https://github.com/aws/aws-xray-sdk-dotnet) and the 
[AWS Distro for OpenTelemetry .NET](https://docs.aws.amazon.com/xray/latest/devguide/xray-dotnet-opentel-sdk.html). 


