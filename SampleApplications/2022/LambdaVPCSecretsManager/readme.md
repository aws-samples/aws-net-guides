# Using AWS Lambda functions, a non-public SQL Server, and AWS Secrets Manager together

## Introduction

If you are using .NET Lambda functions you may have accessed the Secrets Manager to retrieve secrets, this works without any difficulty.

If you are using .NET Lambda functions, you may have accessed an AWS RDS SQL Server database, this works without any difficulty. 

If you wanted to make your database more secure, you may have removed its public access, i.e. made it accessible from within your [Virtual Private Cloud](https://aws.amazon.com/vpc/) (VPC) only. Now, your Lambda function can't access the database, because Lambda functions can't access your VPC by default. So you connected your Lambda function the VPC, now the function can access the database again. 

To make your database even more secure, you put its username and password in Secrets Manager.
But then you hit the problem, your Lambda function can't get to Secrets Manager anymore!

This is because Lambda functions that are connected to the VPC can't access the internet, and that is how the Lambda functions normally retrieve secrets from Secrets Manager.

You made your database more secure, but now you can't get at the secrets anymore. 

As you would expect, there is a solution to this. Two will be presented here.

## Background 

When you create an [AWS RDS SQL Server](https://aws.amazon.com/rds/sqlserver/), you have the choice to make it publicly accessible, or not publicly accessible. With the former, the database can be accessed from anywhere on the internet. With the latter, the database can only be accessed from within the VPC.
From a security standpoint, making the database publicly accessible is a significant risk.

The database credentials can be stored in [Secrets Manager](https://aws.amazon.com/secrets-manager/). Your applications should never store the database credentials in their source code, instead, they should request the credentials from Secrets Manager.

If you are running your application is in an [EC2 instance](https://aws.amazon.com/ec2/) located in the same VPC as the database, accessing the database from the application is very easy. There is nothing to configure other than making sure your security group(s) settings allow access to the database from the EC2 instance. The EC2 instance can also access Secrets Manager, because the EC2 instance has internet access. 

Connecting to the non-public database from a Lambda function is a bit more complicated.

Lambda functions run in their own VPC managed by AWS. Your Lambda functions do **not** run on your VPC. 
By default, functions (Lambda function and function mean the same thing in this document) do not have access to your VPC, but they do have access to the internet. So your function can get to Secrets Manager, but it can't get to the database.

When you connect your function to the VPC, it gains access to resources in the VPC, but it loses access to the internet, and can no longer get to Secrets Manager.

## The Solutions

This tutorial shows two ways to solve the issue of Lambda function connecting to both a non-publicly accessible database on a VPC and Secrets Manager. The rationale for each approach will be discussed in their relevant sections. 
Detailed instructions for both approaches will be given, showing how to use both the command line and AWS Console UI to achieve the same result. 

The first approach uses a NAT Gateway to give your VPC connected Lambda function access to the internet. The second uses a VPC Endpoint to give your VPC connected Lambda function access to the Secrets Manager service only.

## Prerequisites

To follow along with these tutorials, it is assumed you have an AWS account, and that you have not altered your VPC and associated settings drastically. If you have, then you will probably know what you need to change to get the tutorials to work.

### AWS Configuration Assumptions

- You have an AWS account
- You have a VPC
- Your VPC has at least two subnets
- You have an Internet Gateway on your VPC
- You have a main route table with a route for 0.0.0.0/0 via the Internet Gateway
- You have a default security group that allows all egress traffic, and all internal traffic within that security group

### Tools

Install the [latest tooling](https://github.com/aws/aws-extensions-for-dotnet-cli) for .NET, this lets you deploy and run Lambda functions.

`dotnet tool install -g Amazon.Lambda.Tools`

Install the [latest .NET project templates](https://github.com/aws/aws-lambda-dotnet/).

`dotnet new --install Amazon.Lambda.Templates`

Get the latest version of the AWS CLI, from [here](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html).

PowerShell/Bash - the commands in the tutorials have been tested in PowerShell and Bash. If you want to use another shell, you will need to adapt the commands accordingly.

### Solutions

As mentioned above, you can use either a NAT Gateway or a VPC Endpoint to solve the problem of VPC connected Lambda functions accessing Secrets Manager. Implementing both approaches can be done via the command line or the AWS UI Console. This tutorial will show both approaches for each solution.

#### Choosing the solution for your use case

Both approaches solve the problem of accessing Secrets Manager from a Lambda function that is connected to a VPC.
There are some pros and cons to each approach, and you should choose the one that best suits your use case.

##### NAT Gateway 

The NAT Gateway approach is the more complicated one to implement, but it will give your Lambda function access to not just Secrets Manager, but all AWS services that lie outside your VPC, and everything on the internet.

However, your secrets will travel over the internet, for most use cases that is fine as the data is encrypted end-to-end. But some organizations may not accept sensitive data transmission over Internet.

Using a NAT Gateway requires the allocation of an Elastic IP address, you have a finite number of these, so you may need to consider that too.

[NAT Gateway solution - Command Line](nat-gateway-cli.md)

[NAT Gateway solution - AWS Console UI](nat-gateway-console-ui.md) 

##### VPC Endpoint 

The VPC Endpoint approach is simpler to implement, but it will give your Lambda function access to Secrets Manager only, and not to any other AWS services that lie outside your VPC. Your Lambda function will not have access to the internet either. 

When you request secrets from Secrets Manager, the data will not travel over the internet, a private link is established between your VPC and the Secrets Manager service.

The cost of using a VPC Endpoint should be considered too, and this cost increases with the number of subnets that are attached to the VPC Endpoint.

[VPC Endpoint solution - Command Line](vpc-endpoint-cli.md)

[VPC Endpoint solution - AWS Console UI](vpc-endpoint-console-ui.md)
