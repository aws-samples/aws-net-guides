
# .NET 8 support on AWS
This guide describes .NET 8 support provided by AWS services and tools.  

Related Content:
- [.NET 7 support on AWS](https://github.com/aws-samples/aws-net-guides/tree/master/RuntimeSupport/dotnet7)
- [GitHub home for .NET on AWS](https://github.com/aws/dotnet)
- [AWS Developer Center - .NET on AWS](https://aws.amazon.com/developer/language/net/)

---
## Introduction

[.NET 8](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8/overview) s the latest Long Term Support (LTS) version of cross-platform .NET, released in November 2023. .NET 8 includes performance improvements, container enhancements, C# language simplified syntax, Blazor support for full-stack web applications, and ASP.NET Core partial support for Native Ahead of Time compilation (Native AOT). The instructions that follow explain how to use .NET 8 on AWS. 

If you’re running an older version of .NET, be aware that both .NET 6 and 7 reached end of support in 2024. Per Microsoft’s [.NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core), .NET 8 will be supported through November 10, 2026.

---
## Compute Services

Whether your workloads are self-managed, run in a managed service, use containers, or are serverless, you can use .NET 8 on AWS. You can run .NET 8 today on Amazon Elastic Compute Cloud ([Amazon EC2](https://aws.amazon.com/ec2/)), [AWS Elastic Beanstalk](https://aws.amazon.com/elasticbeanstalk/), Amazon Elastic Container Service ([Amazon ECS](https://aws.amazon.com/ecs/)), Amazon Elastic Kubernetes Service ([Amazon EKS](https://aws.amazon.com/eks/)), [AWS App Runner](https://aws.amazon.com/apprunner/), and [AWS Lambda](https://aws.amazon.com/lambda/).

### Amazon EC2

Amazon EC2 offers broad and deep compute functionality, with granular control for managing your infrastructure with a choice of processors, storage, and networking. Customers can install .NET 8 on over [400 instance types](https://aws.amazon.com/ec2/instance-types/).

To install .NET 8 on an EC2 instance, specify [User Data](https://docs.aws.amazon.com/AWSEC2/latest/UserGuide/user-data.html#user-data-console) commands when launching the instance. 

For instructions and scripts on installing .NET on Linux, refer to these references:

[.NET 8 installation instructions for Linux](https://github.com/dotnet/core/blob/main/release-notes/8.0/install-linux.md)  
[Install .NET on Linux](https://learn.microsoft.com/en-us/dotnet/core/install/linux)  
[Install .NET on Linux by using an install script or by extracting binaries](https://learn.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual#scripted-install)

You can use automation facilities in [AWS Systems Manager](https://aws.amazon.com/systems-manager) to automatically install .NET runtimes using automation documents, and use the [EC2 Image Builder](https://aws.amazon.com/image-builder/) service to pre-create EC2 Images with the .NET Runtime pre-installed.

---
### AWS Elastic Beanstalk

Elastic Beanstalk is a managed service that allows you to quickly deploy and manage applications in the AWS Cloud without having to learn about the infrastructure that runs those applications. Your EC2 assets are fully visible in your AWS account and you have full access to them.

As of the [2023-12-05 platform update](https://docs.aws.amazon.com/elasticbeanstalk/latest/relnotes/release-2023-12-05-windows.html), Elastic Beanstalk Windows supports .NET 8.

As of the [2024-05-17 platform update](https://docs.aws.amazon.com/elasticbeanstalk/latest/relnotes/release-2024-05-17-al2023NET8.html), Elastic Beanstalk Linux supports .NET 8 on AL2023. The 2023.4.20240513 AMI supports .NET 8.0.5.

---
### AWS Lambda

[AWS Lambda](https://aws.amazon.com/lambda/) supports the .NET 8 runtime. In the AWS Lambda console, there’s now a runtime option for .NET 8 (C#/F#/PowerShell). Refer to [Introducing the .NET 8 runtime for AWS Lambda](https://aws.amazon.com/blogs/compute/introducing-the-net-8-runtime-for-aws-lambda/) for complete information on creating and updating Lambda functions for .NET 8, and using Native AOT. You can also run .NET 8 applications on AWS Lambda in several other ways. You can create your own [custom runtime](https://docs.aws.amazon.com/lambda/latest/dg/runtimes-custom.html), [deploy a container](https://docs.aws.amazon.com/lambda/latest/dg/csharp-image.html), or publish native code to Lambda using .NET 8's Native AOT compilation

Video: [The Simplest Way To Build .NET 8 Native AOT Lambda Functions](https://www.youtube.com/watch?v=kyb16r-Oul0&t=6s)

Blog post: [Building serverless .NET applications on AWS Lambda using .NET 7](https://aws.amazon.com/blogs/compute/building-serverless-net-applications-on-aws-lambda-using-net-7/)

The [AWS .NET Lambda packages](https://aws.amazon.com/developer/language/net/tools/) have been updated to target .NET 8 and address Native AOT warnings. This makes it easier and safer to use them for Native AOT Lambda functions.

The [.NET Lambda Annotations Framework](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/aws-lambda-annotations.html) simplifies the programming model and lets you write .NET Lambda functions more naturally in C#. When using custom runtimes or native ahead of time (AOT) compilation, the framework removes the need to manually bootstrap the Lambda runtime and can auto-generate the Main method. Refer to [.NET Lambda Annotations Design - Auto Generate Main](https://github.com/aws/aws-lambda-dotnet/blob/master/Docs/lambda-annotations-design.md#auto-generate-main).

---
## Containers

You can deploy .NET applications running on either Windows or Linux containers to [Amazon ECS](https://aws.amazon.com/ecs/) or Amazon EKS ([Amazon EKS](https://aws.amazon.com/eks/)). [AWS Fargate](https://aws.amazon.com/fargate/) is a service that you can use to run and manage the lifecycle of ECS and EKS containers without the need to manage the container infrastructure yourself.

[AWS App Runner](https://aws.amazon.com/apprunner/) is a fully managed service that makes it easy to quickly deploy containerized web applications and APIs, scaling up or down automatically to meet application traffic needs. To use with .NET 8 applications, upload a container image with the .NET 8 application to Amazon Elastic Container Registry ([Amazon ECR](https://aws.amazon.com/ecr/)) and use the [source image](https://docs.aws.amazon.com/apprunner/latest/dg/service-source-image.html) support to configure AWS App Runner to start, run, scale, and load balance the application.

You can deploy a .NET 8 application to Elastic Beanstalk in a container. For more information, refer to [Deploying Elastic Beanstalk applications from Docker containers](https://docs.aws.amazon.com/elasticbeanstalk/latest/dg/create_deploy_docker.html).

---
## Security and Diagnostics

### AWS X-Ray

[AWS X-Ray](https://aws.amazon.com/xray/) helps developers analyze and debug distributed applications, such as those built using a microservices architecture. .NET 8 applications can integrate AWS X-Ray with [AWS X-Ray SDK for .NET](https://github.com/aws/aws-xray-sdk-dotnet) and the [AWS Distro for OpenTelemetry .NET](https://docs.aws.amazon.com/xray/latest/devguide/xray-dotnet-opentel-sdk.html). We don't recommend using AWS X-Ray with Native AOT .NET applications at this time.

---
## Tools, Libraries, and SDK

If you’ve been using an older version of .NET on AWS, be sure to update the AWS tools installed on your developer machine.

### AWS SDK for .NET

The [AWS SDK for .NET](https://aws.amazon.com/sdk-for-net/) allows .NET developers to integrate AWS services into their application code in a familiar and consistent manner. Starting with version 3.7.300 the SDK added a new .NET 8 target framework and became compatible with Native AOT by addressing all trimming warnings. The SDK is available from [NuGet](https://www.nuget.org/packages/awssdk.core/). Learn how to get started with the [AWS SDK for .NET in the Developer Guide](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/welcome.html).

### AWS Code Build

[AWS Code Build](https://aws.amazon.com/codebuild/) is a fully managed service that helps developers automatically build applications from source code. The CodeBuild service allows the user to customize the build environment, to suit the needs of the application being built. This includes the ability to install additional .NET runtimes. You can add support for building .NET 8 applications by adding the following snippet to your applications buildspec.yml file. Source: [Install .NET on Linux by using an install script or by extracting binaries](https://learn.microsoft.com/en-us/dotnet/core/install/linux-scripted-manual#scripted-install).

```yaml
  install:
    commands:
      - curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0 
```
This will automatically download and install the .NET 8 SDK as part of the Install phase of CodeBuild.

### AWS Deploy Tool for .NET

The [AWS Deploy Tool for .NET](https://aws.github.io/aws-dotnet-deploy/) command line interface (CLI) is an interactive assistant that provides compute recommendations for .NET applications and deploys them to AWS in a few easy steps. The Deploy Tool supports .NET 8 applications by creating container images for container based services like Amazon ECS and AWS AppRunner or using .NET self-contained publishing for Elastic Beanstalk.

### AWS Toolkit for Visual Studio

The [AWS Toolkit for Visual Studio](https://aws.amazon.com/visualstudio/) is an extension for Microsoft Visual Studio on Windows that makes it easier for developers to develop, debug, and deploy .NET applications using Amazon Web Services. Visual Studio 2022 supports .NET 8 development.

The Toolkit’s [Publish to AWS](https://docs.aws.amazon.com/toolkit-for-visual-studio/latest/user-guide/publish-experience.html) feature integrates with the AWS Deploy Tool for .NET, and can deploy .NET 8 projects to various AWS services from Visual Studio as of version 1.60.0. You can deploy ASP.NET Core projects to Amazon ECS, AWS App Runner, Elastic Beanstalk Windows, Elastic Beanstalk Linux, or the Amazon Elastic Container Registry (Amazon ECR).

You can download the AWS Toolkit for Visual Studio 2022 from the [Visual Studio Marketplace](https://marketplace.visualstudio.com/items?itemName=AmazonWebServices.AWSToolkitforVisualStudio2022). If you're already using the AWS Toolkit for Visual Studio, be sure to upgrade to the latest version by navigating to Manage Extensions > Updates in Visual Studio.

---
## .NET Modernization Tools

AWS provides assistive tools that help architects, developers, and IT professionals modernize .NET workloads. The following AWS modernization tools now support .NET 8:

[AWS App2Container](https://aws.amazon.com/app2container/) (A2C) is a command line tool that containerizes your applications. It automatically generates a container image configured with the correct dependencies, network configurations, and deployment instructions for Amazon ECS or Amazon EKS. A2C can now detect a .NET 8 runtime version and containerize the application using the corresponding runtime base images.

[AWS Microservice Extractor for .NET](https://aws.amazon.com/microservice-extractor/) is an assistive tool that serves as an advisor to assess and visualize monolithic code, and recommend microservice candidates using artificial intelligence and heuristics. It also serves as a robotic builder to simplify microservices extraction. Microservice Extractor now supports analyzing .NET 8 applications for visualization, grouping, and extraction. With its integrated strangler-fig porting capability, you can also use Microservice Extractor to break down a large .NET Framework-based application with hundreds of projects and thousands of classes into manageable groups and port those directly to .NET 8.

[Migration Hub Strategy Recommendations](https://docs.aws.amazon.com/migrationhub-strategy/latest/userguide/what-is-mhub-strategy.html) (MHSR) helps you plan migration and modernization initiatives by offering strategy recommendations for viable transformation paths for your applications. MHSR can now detect .NET 8 applications and provide recommendations for them.

[AWS Toolkit for .NET Refactoring](https://aws.amazon.com/visual-studio-net/) is a Visual Studio extension that helps you refactor traditional .NET Framework applications to cloud-based alternatives on AWS. It provides a compatibility assessment report and helps port your code. The Toolkit for .NET Refactoring can now target .NET 8 for assessment, porting, and test deployment.

You can take full advantage of .NET 8 as you plan, migrate, and modernize .NET workloads on AWS using these assistive tools. For more information on .NET modernization use cases and tools, refer to [Modernize .NET Workloads on AWS](https://aws.amazon.com/developer/language/net/modernize) at the .NET on AWS developer center.

---
## Conclusion

You can run .NET 8 workloads on AWS today across a spectrum of AWS compute services. The SDK for .NET, multiple developer tools, and multiple .NET modernization tools also support .NET 8. If you have existing AWS workloads on .NET 6 or .NET 7, be proactive in upgrading to .NET 8 before they reach end-of-support. You can keep up to date on .NET on AWS developments by visiting the AWS [.NET development center](https://aws.amazon.com/developer/language/net/).