# .NET Core Console Application using Parameter Store for configuration

## Step-by-Step Walkthrough
--------

**Overview and Services Used:**

This walk-through comprises a Microsoft Word doc, along with a .NET Core application. The application provides a basic console application that retrieves configuration values from AWS Systems Manager Parameter Store.

The walk-through includes creating test values in Parameter Store, creating and testing an application that reads from Parameter Store in either Visual Studio 2017, or via the .NET Core CLI, and then deleting the test values from Parameter Store.

+ Links to documentation
  * AWS Systems Manager Page: https://aws.amazon.com/systems-manager/
  * AWS Systems Manager Parameter User Guide: https://docs.aws.amazon.com/systems-manager/latest/userguide/systems-manager-paramstore.html

+ Prerequisites
  * AWS CLI
	* .NET Core 2.x SDK or higher installed
	* AWS Account with credentials configured locally in Visual Studio or using the CLI
	* Optional: Visual Studio 2017 or Visual Studio Mac

+ External libraries:
  * NuGet Package AWSSDK.SimpleSystemsManagement
N.B. Adding packages is included in the walk-through
