# **Deploying an ASP.NET Core Application to Fargate**

## Step-by-Step Walkthrough
### Level 200 (using Visual Studio), Level 300 (using CLI)
--------

#### Create an ASP.NET MVC Core web-application and Deploy to Fargate using Visual Studio or Command Line 

--------

**Overview and Services Used:**

This walk-through includes a Word document, and two zipped files:
+ Visual-Studio-fargate-sample-app.zip
+ Command-line-fargate-sample-app.zip

The Word document walks you through creating a new ASP.NET MVC Core application, and deploying it as a multi-AZ service to Fargate using the Container Publishing Wizard in the AWS Toolkit for Visual Studio. As part of the deployment, you will create and configure an Application Load Balancer (ALB) and ECS Cluster, all within the publishing wizard. Finally, you will view the website via the public IP addresses of the Tasks (containers) themselves, and also via the ALB's public DNS.

There is an optional second set of instructions for creating the application via the dotnet CLI, adding docker support, building and pushing the container image and registering the Task definition all from the command line (Windows/Mac OS X/Linux), along with creating the ALB and ECS Cluster via the AWS Management Console. These steps are somewhat less detailed, and are suitable for people comfortable with the command line and management console.

The two zipped project files are equivalent to what you will create in the walk-through, and are provided in case you wish to simply skip to the deployment step. If you do that, you will still need to edit the task definition file as indicated in the instructions. If you are following all the steps in the Word file, you do not need the zipped project files.
	
+ Links to documentation
	* [Fargate Service Page](https://aws.amazon.com/fargate/) (link)
	* [AWS Fargate Documentation](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/ECS_GetStarted.html) (link)

+ Prerequisites
	* [.NET Core 2.0](https://www.microsoft.com/net/download/) or higher installed
	* AWS Account with credentials configured locally in Visual Studio or using the CLI
	* [Docker for Windows](https://docs.docker.com/docker-for-windows/?install_site=vsonwin) (link)
	* Visual Studio 2017* (free community edition is sufficient)
	* [AWS Toolkit for Visual Studio](https://aws.amazon.com/visualstudio/)*

*If you are following the steps in the optional task using the command line, you do not need Visual Studio or the Toolkit, and can use Windows, Mac OS X or Linux.

+ External libraries: None