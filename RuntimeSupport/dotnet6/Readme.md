# .NET 6 support on AWS

## Overview

This guide describes .NET 6 support provided by AWS services and tools. The guide will be updated as new support is added. 

* Related Content:
  * [GitHub home for .NET on AWS](https://github.com/aws/dotnet)
  * [AWS Developer Center - .NET on AWS](https://aws.amazon.com/developer/language/net/)

## Introduction

Customers and AWS teams are excited about the release of .NET 6. Many organizations will want to target .NET 6 because it is a long term support (LTS) release. .NET 6 applications can utilize many AWS services without additional work. For example, you can deploy a new instance of an Amazon EC2 instance and by adding a command to install the .NET 6 runtime as part of the deploy, the instance will support your .NET 6 application. 

This guide is focused on AWS services and tools that have either been updated, or plan to be updated, to provide .NET 6 support. Some services were able use preview and RC versions to validate and update for .NET 6 support, other services require a GA version of .NET 6 to provide support. While we don't preannounce specific dates, this document will reference future planned support.

Please open an Issue on this repository if you have questions about .NET 6 support on AWS.

## Compute Services

### Amazon EC2

Customers can install .NET 6 on any of the over 400 [Amazon EC2 instances types](https://aws.amazon.com/ec2/instance-types/). 

The following EC2 [User Data](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/user-data.html#user-data-console) example installs .NET 6 RC2 targeting ARM64 (AWS Graviton2):
```
#!/bin/bash
# Install .NET 6-RC2 SDK for ARM64
sudo apt-get update -y
curl -O https://download.visualstudio.microsoft.com/download/pr/1e7a9f1f-6128-4581-9d72-edfe196320d3/ad3b26879ddaca8b76e16ddddd091d5d/dotnet-sdk-6.0.100-rc.2.21505.57-linux-arm64.tar.gz
mkdir /usr/bin/dotnet
sudo tar -zxvf dotnet-sdk-6.0.100-rc.2.21505.57-linux-arm64.tar.gz -C /usr/bin/dotnet
sudo ln -s /usr/bin/dotnet/dotnet /usr/bin/dotnet
sudo sh -c 'echo "export DOTNET_ROOT=/usr/bin/dotnet" >> /etc/environment'
sudo sh -c 'echo "export PATH=$PATH:$DOTNET_ROOT" >> /etc/environment'
```

In the near future we will have an updated version of the preconfigured Amazon Machine Image (AMI) with [MATE Desktop Environment](https://mate-desktop.org/) that includes .NET 6. Before it is released customers can create an instance with the [current version that includes .NET 5 preinstalled](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/amazon-linux-ami-mate.html) and then install the .NET 6 SDK on that instance. 

EC2 Image Builder simplifies the building, testing, and deployment of virtual machine and container images for use on AWS or on-premises. Customers can add .NET 6 to the images using EC2 Image Builder in two ways:
1. CloudFormation Template - A sample [Ubuntu Server 20 Image with .NET 6](https://github.com/aws-samples/amazon-ec2-image-builder-samples/tree/master/CloudFormation/Linux/ubuntu-with-net6) 
EC2 Image Builder CloudFormation demostrates building an Ubuntu Server 20 Amazon Machine Image (AMI) with .NET 6. Currently the template targets .NET 6 Preview but will be updated to .NET 6 GA in the near future. 
1. EC2 Image Builder Components - EC2 Image Builder will offer Amazon managed .NET 6 components (runtime and SDK) available that can target Windows and Linux distributions in the near future. 

### AWS Elastic Beanstalk

[TODO]: Add Content 

### Containers

#### Amazon Elastic Container Software (ECS)

[TODO]: Add Content 

#### Amazon Elastic Kubernetes Service (EKS)

[TODO]: Add Content 

#### AWS Fargate

[TODO]: Add Content 

#### AWS App Runner 

[TODO]: Add Content 

### AWS Lambda

[TODO]: Add Content 

## Tools

### AWS Toolkit for Visual Studio

The AWS Toolkit for Visual Studio is an extension for Microsoft Visual Studio on Windows that makes it easier for developers to develop, debug, and deploy .NET applications using Amazon Web Services. Visual Studio 2022 supports .NET 6 development. Customers can download the [AWS Toolkit for .NET](https://marketplace.visualstudio.com/items?itemName=AmazonWebServices.AWSToolkitforVisualStudio2022) that is compatible with Visual Studio 2022 from the Visual Studio Marketplace. Currently the Toolkit is available as a Preview, with a GA version coming out in the near future. 

### AWS Toolkit for Rider

[TODO]: Add Content

### AWS Toolkit for Azure DevOps

[TODO]: Add Content

## AWS SDK for .NET

[TODO]: Add Content

## .NET deploymnet tool

[TODO]: Add Content

## Infrastructure and Diagnostic

### AWS Secrets Manager

[TODO]: Add Content

### AWS X-Ray

[TODO]: Add Content

## DevOps

### AWS CodeBuild

[TODO]: Add Content

### AWS Cloud Development Kit (CDK)

[TODO]: Add Content